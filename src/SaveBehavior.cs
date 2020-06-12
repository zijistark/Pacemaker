using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CampaignPacer
{
	class SaveBehavior : CampaignBehaviorBase
	{
		protected SimpleTime SavedTime
		{
			get => _savedTime;
			set => _savedTime = value;
		}

		protected SavedSettings SavedSettings
		{
			get => _savedSettings;
			set => _savedSettings = value;
		}

		protected bool HasLoaded { get; set; }

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
				SavedSettings = new SavedSettings(Main.Settings);
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

				AdjustOnLoad(trace);
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

			if (!HasLoaded)
			{
				AdjustOnLoad(trace);
				HasLoaded = true;
			}

			Util.EventTracer.Trace(trace);
		}

		protected void AdjustOnLoad(List<string> trace)
		{
			AdjustTimeOnLoad(trace);
			AdjustPregnanciesOnLoad(trace);
		}

		protected void AdjustTimeOnLoad(List<string> trace)
		{
			// adjust campaign start time if it's not _exactly_ equal to the standard start time:
			var startTime = Campaign.Current.CampaignStartTime;
			trace.Add($"Apparent campaign start time: {new SimpleTime(startTime)}");

			if (startTime != Patches.CampaignPatch.Helpers.StandardStartTime)
			{
				Patches.CampaignPatch.Helpers.ResetCampaignStartTime(Campaign.Current);
				trace.Add($"New campaign start time: {new SimpleTime(Campaign.Current.CampaignStartTime)}");
				trace.Add($"New campaign start time (ticks): {Campaign.Current.CampaignStartTime.GetTicks()}");
			}

			var adjustedTime = CampaignTime.Zero;

			if (SavedTime == null)
			{
				// Load of save that didn't have CP enabled.
				// Convert "current" campaign time back to what it would have been under the vanilla calendar.
				trace.Add("Loading a vanilla save...");

				var now = CampaignTime.Now;
				trace.Add($"Apparent time: {new SimpleTime(now)}");
				trace.Add($"Apparent time (ticks): {now.GetTicks()}");
				trace.Add(string.Empty);

				// Swap out CP's configured calendar parameters for vanilla calendar parameters, momentarily
				var vanillaTimeParams = new TimeParams(TimeParams.OldDayPerWeek * TimeParams.OldWeekPerSeason);
				var ourTimeParams = Main.SetTimeParams(vanillaTimeParams, trace);

				// Since we're using vanilla parameters, the CampaignTime API should automatically be reporting
				// correct symbolic calendar information (e.g., year, season, day of season). Now we just mimic
				// the process which CP-enabled savegames go through: save our symbolic time and then restore it,
				// except restoration must happen under CP's calendar parameters.

				SavedTime = new SimpleTime(now);

				// Restore CP's configured calendar parameters:
				trace.Add(string.Empty);
				Main.SetTimeParams(ourTimeParams, trace);

				// And tada, let the vanilla symbolic time express in terms of our configured calendar:
				adjustedTime = SavedTime.ToCampaignTime();
				trace.Add(string.Empty);
				trace.Add($"Intermediate symbolic time: {SavedTime}");
			}
			else if (SavedTime.IsValid)
			{
				// Normal load of prior CP-enabled savegame.
				// Simply restore saved calendar date as-is if the configured days/season has changed.
				// If the configuration hasn't changed, do nothing at all.

				trace.Add($"Loading a {Main.Name}-enabled save...");
				trace.Add($"Saved settings: {SavedSettings}");

				if (SavedSettings == null || SavedSettings.DaysPerSeason != Main.TimeParam.DayPerSeason)
				{
					trace.Add($"Configured days/season changed from {SavedSettings?.DaysPerSeason} to {Main.TimeParam.DayPerSeason}.");
					adjustedTime = SavedTime.ToCampaignTime();
				}
			}
			else
				trace.Add("Loaded an invalid calendar date! Skipping campaign time adjustment...");

			if (adjustedTime != CampaignTime.Zero)
			{
				// First, ensure that elapsed time since campaign start is absolutely never negative:
				if (adjustedTime < Campaign.Current.CampaignStartTime)
				{
					var daysBeforeStart = Campaign.Current.CampaignStartTime.ToDays - adjustedTime.ToDays;
					trace.Add($"Adjusted time was {daysBeforeStart} days before the campaign start time, so clamping...");
					adjustedTime = Campaign.Current.CampaignStartTime;
				}

				Patches.CampaignPatch.Helpers.SetMapTimeTracker(Campaign.Current, adjustedTime);
				trace.Add($"New campaign time: {new SimpleTime(CampaignTime.Now)}");
				trace.Add($"New campaign time (ticks): {CampaignTime.Now.GetTicks()}");
			}
		}

		protected void AdjustPregnanciesOnLoad(List<string> trace)
		{
			if (SavedSettings != null)
			{
				if (!SavedSettings.IsValid)
					return;

				if (SavedSettings.SameDaysPerSeason && SavedSettings.SameScaledPregnancyDuration)
					return;
			}

			float oldDuration = (SavedSettings == null)
				? 36f // vanilla
				: SavedSettings.ScaledPregnancyDuration * SavedSettings.DaysPerSeason * TimeParams.SeasonPerYear;

			float newDuration = Main.Settings.ScaledPregnancyDuration * Main.TimeParam.DayPerSeason * TimeParams.SeasonPerYear;

			if (Util.NearlyEqual(oldDuration, newDuration, 1e-2))
				return;

			// TODO: Finish up, mang.
		}

		private SimpleTime _savedTime = null;
		private SavedSettings _savedSettings = null;
	}
}
