using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CampaignPacer
{
	class SaveBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnNewGameCreated));
			CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnGameEarlyLoaded));
		}

		public override void SyncData(IDataStore dataStore)
		{
			var trace = new List<string>();

			if (!HasLoaded)
				trace.Add("Loading saved data...");
			else
			{
				trace.Add("Saving data...");
				SavedTime = new SimpleTime(CampaignTime.Now);
			}

			dataStore.SyncData($"{Main.Name}SavedTime", ref _savedTime);
			dataStore.SyncData($"{Main.Name}SavedSettings", ref _savedSettings);

			trace.AddRange(new List<string>
			{
				$"Campaign tick date:   {new SimpleTime(CampaignTime.Now)}",
				$"Stored calendar date: {((SavedTime != null) ? SavedTime.ToString() : "NULL")}",
			});

			if (!HasLoaded)
			{
				if (SavedTime?.IsNull ?? false)
					SavedTime = null;

				if (SavedSettings?.IsNull ?? false)
					SavedSettings = null;

				AdjustTimeOnLoad(trace);
				HasLoaded = true;
			}

			Util.EventTracer.Trace(trace);
		}

		protected void OnNewGameCreated(CampaignGameStarter starter)
		{
			HasLoaded = true;
			Util.EventTracer.Trace();
		}

		// OnGameEarlyLoaded is only present so that we can still adjust the [incorrect] apparent time when adding
		// the mod to a save that didn't have it enabled (so-called "vanilla save"). This is because SyncData does
		// not even get called during game loading for behaviors belonging to submodules that were previously not
		// part of the save.
		protected void OnGameEarlyLoaded(CampaignGameStarter starter)
		{
			var trace = new List<string>();

			if (!HasLoaded && SavedTime == null)
			{
				AdjustTimeOnLoad(trace);
				HasLoaded = true;
			}

			Util.EventTracer.Trace(trace);
		}

		private void AdjustTimeOnLoad(List<string> trace)
		{
			var adjustedTime = CampaignTime.Zero;

			if (SavedTime == null)
			{
				// Load of save that didn't have CP enabled.
				// Convert "current" campaign time back to what it would have been under the vanilla calendar.

				trace.Add("Loading a vanilla save...");

				var now = CampaignTime.Now;
				trace.Add($"Apparent time: {new SimpleTime(now)}");

				// NOTE: this formula assumes that the vanilla tick ratio for years equals that of seasons.
				var seasons = now.ToSeasons;
				var nSeasons = Math.Floor(seasons);
				var fracSeason = seasons - nSeasons;
				trace.Add($"Apparent seasons: {(int)nSeasons} (remainder: {fracSeason})");

				// extract the days of the season. we assume vanilla days = our days.
				var daysOfSeason = Math.Max(+0.0, fracSeason * Main.TimeParam.DayPerSeason);
				trace.Add($"Apparent days of season: {daysOfSeason}");

				// but vanilla seasons != our seasons.
				var adjustedSeasons = nSeasons / Main.TimeParam.TickRatioSeasonD;
				trace.Add($"Adjusted seasons: {adjustedSeasons}");

				adjustedTime = CampaignTimeExt.SeasonsD(adjustedSeasons) + CampaignTimeExt.DaysD(daysOfSeason);

				SavedTime = new SimpleTime(adjustedTime);
			}
			else if (SavedTime.IsValid)
			{
				// Normal load of prior CP-enabled savegame.
				// Simply restore saved calendar date as-is if the configured days/season has changed.
				// If the configuration hasn't changed, do nothing at all.
				// This allows switching calendar parameters mid-playthrough.

				trace.Add($"Loading a {Main.Name}-enabled save...");
				trace.Add($"Saved settings: {SavedSettings}");

				if (SavedSettings == null || SavedSettings.DaysPerSeason != Main.TimeParam.DayPerSeason)
				{
					trace.Add($"Configured days/season changed from {SavedSettings?.DaysPerSeason} to {Main.TimeParam.DayPerSeason}.");
					adjustedTime = SavedTime.ToCampaignTime();
				}
			}
			else
			{
				trace.Add("Loaded an invalid calendar date! Skipping any adjustment...");
				return;
			}

			// adjust campaign start time if it's not _exactly_ equal to the standard start time:
			var startTime = Campaign.Current.CampaignStartTime;
			trace.Add($"Apparent campaign start time: {new SimpleTime(startTime)}");

			if (startTime != Patches.CampaignPatch.Helpers.StandardStartTime)
			{
				Patches.CampaignPatch.Helpers.ResetCampaignStartTime(Campaign.Current);
				trace.Add($"New campaign start time: {new SimpleTime(Campaign.Current.CampaignStartTime)}");
			}

			if (adjustedTime != CampaignTime.Zero)
			{
				// first, ensure that elapsed time since campaign start is absolutely never negative:
				if (adjustedTime < Campaign.Current.CampaignStartTime)
				{
					var daysBeforeStart = Campaign.Current.CampaignStartTime.ToDays - adjustedTime.ToDays;
					trace.Add($"Adjusted time was {daysBeforeStart} days before the campaign start time, so clamping...");
					adjustedTime = Campaign.Current.CampaignStartTime;
				}

				Patches.CampaignPatch.Helpers.SetMapTimeTracker(Campaign.Current, adjustedTime);
				trace.Add($"New campaign time: {new SimpleTime(CampaignTime.Now)}");
			}
		}

		protected SavedSettings SavedSettings
		{
			get => _savedSettings;
			set => _savedSettings = value;
		}

		protected SimpleTime SavedTime
		{
			get => _savedTime;
			set => _savedTime = value;
		}

		protected bool HasLoaded { get; set; }

		private SimpleTime _savedTime = null;
		private SavedSettings _savedSettings = null;
	}
}
