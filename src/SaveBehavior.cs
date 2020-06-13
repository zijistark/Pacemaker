using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

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
				SavedValues.Snapshot();
			}

			dataStore.SyncData($"{Main.Name}SavedTime", ref _savedTime);
			dataStore.SyncData($"{Main.Name}SavedValues", ref _savedValues);

			trace.AddRange(new List<string>
			{
				$"Campaign time: {new SimpleTime(CampaignTime.Now)}",
				$"Stored calendar time: {((SavedTime != null) ? SavedTime.ToString() : "NULL")}",
				$"Stored values: {SavedValues}",
			});

			if (!HasLoaded)
				OnLoad(trace);

			Util.EventTracer.Trace(trace);
		}

		protected void OnNewGameCreated(CampaignGameStarter starter)
		{
			HasLoaded = true;
			Util.EventTracer.Trace();
		}

		/* OnGameEarlyLoaded is only present so that we can still initialize when adding the mod to a save
		 * that didn't previously have it enabled (so-called "vanilla save"). This is because SyncData does
		 * not even get called during game loading for behaviors belonging to submodules that were previously
		 * not part of the save.
		 */
		protected void OnGameEarlyLoaded(CampaignGameStarter starter)
		{
			var trace = new List<string>();

			if (!HasLoaded) // if SyncData were to be called, it would've been by now
				OnLoad(trace);

			Util.EventTracer.Trace(trace);
		}

		protected void OnLoad(List<string> trace)
		{
			AdjustTimeOnLoad(trace);
			AdjustPregnanciesOnLoad(trace);

			HasLoaded = true;
		}

		protected void AdjustTimeOnLoad(List<string> trace)
		{
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

				if (SavedValues.DaysPerSeason != Main.TimeParam.DayPerSeason)
				{
					trace.Add($"Configured days/season changed from {SavedValues.DaysPerSeason} to {Main.TimeParam.DayPerSeason}.");
					adjustedTime = SavedTime.ToCampaignTime();
				}
			}
			else
				trace.Add("Loaded an invalid calendar date! Skipping campaign time adjustment...");

			// Adjust campaign start time if it's not exactly equal to the standard start time:
			var startTime = Campaign.Current.CampaignStartTime;

			if (startTime != Patches.CampaignPatch.Helpers.StandardStartTime)
			{
				trace.Add($"Apparent campaign start time: {new SimpleTime(startTime)}");
				Patches.CampaignPatch.Helpers.ResetCampaignStartTime(Campaign.Current);
				trace.Add($"New campaign start time: {new SimpleTime(Campaign.Current.CampaignStartTime)}");
			}

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
			}
		}

		protected void AdjustPregnanciesOnLoad(List<string> trace)
		{
			var oldDuration = (SavedValues.DaysPerSeason != 0 && SavedValues.ScaledPregnancyDuration != 0f)
				? SavedValues.ScaledPregnancyDuration * SavedValues.DaysPerSeason * TimeParams.SeasonPerYear
				: VanillaPregnancyDuration;

			var newDuration = Main.Settings.ScaledPregnancyDuration * Main.Settings.DaysPerSeason * TimeParams.SeasonPerYear;

			if (Util.NearEqual(oldDuration, newDuration, 1e-3f))
				return;

			//trace.Add("\nAuto-adjusting in-progress pregnancy due dates due to a change in pregnancy duration...");

			//var pregBehavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<PregnancyCampaignBehavior>();

			// TODO: Finish up, mang.
		}

		protected SimpleTime SavedTime
		{
			get => _savedTime;
			set => _savedTime = value;
		}

		protected SavedValues SavedValues
		{
			get => _savedValues;
			set => _savedValues = value;
		}

		protected bool HasLoaded { get; set; }

		private SimpleTime _savedTime = null;
		private SavedValues _savedValues = new SavedValues(); // empty
		private const float VanillaPregnancyDuration = 36f;
	}
}
