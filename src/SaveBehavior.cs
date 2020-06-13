using System;
using System.Collections.Generic;
using System.Reflection;
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
			if (!Main.Settings.EnablePregnancyTweaks || !Main.Settings.AdjustPregnancyDueDates)
				return;

			var newDuration = Main.Settings.ScaledPregnancyDuration * Main.TimeParam.DayPerYear;

			// Check whether our pregnancy duration is actually in force (i.e., no interference from other mods) before
			// we do any auto-adjustment. Check whichever PregnanacyModel-derived class is currently in place and ask
			// of it the pregnanacy duration.
			var pregModel = Campaign.Current.Models.PregnancyModel;

			if (!Util.NearEqual(pregModel.PregnancyDurationInDays, newDuration))
			{
				trace.Add($"Current PregnancyModel-derived type: {pregModel.GetType().FullName}");
				trace.Add($"{Main.Name}'s pregnancy duration patch isn't in effect. Skipping auto-adjustment " +
					"of in-progress pregnancy due dates.");
			}

			var oldDuration = (SavedValues.DaysPerSeason != 0 && SavedValues.ScaledPregnancyDuration != 0f)
				? SavedValues.ScaledPregnancyDuration * SavedValues.DaysPerSeason * TimeParams.SeasonPerYear
				: VanillaPregnancyDuration;

			// Don't bother if the effective old and new durations barely differ if at all.
			if (Util.NearEqual(oldDuration, newDuration, 1e-3f))
				return;

			trace.Add("\nAuto-adjusting in-progress pregnancy due dates due to a change in pregnancy duration...\n");

			var dueDateDelta = newDuration - oldDuration;
			trace.Add($"Pregnancy due dates will be offset by {dueDateDelta:F3} days.");

			// We need to iterate over the global List<PregnancyCampaignBehavior.Pregnancy> (Pregnancy is a private
			// nested class) stored in that behavior's private instance field _heroPregnancies. We'll then need to
			// access the Pregnancy.DueDate, a private instance field, for all of those. So let's do some reflection.

			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

			var pregListFI = typeof(PregnancyCampaignBehavior).GetField("_heroPregnancies", bindingFlags);

			if (pregListFI == null)
			{
				trace.Add($"Could not resolve {typeof(PregnancyCampaignBehavior).FullName}._heroPregnancies field! Aborting.");
				return;
			}

			var pregT = typeof(PregnancyCampaignBehavior).GetNestedType("Pregnancy", bindingFlags);

			if (pregT == null)
			{
				trace.Add($"Could not resolve {typeof(PregnancyCampaignBehavior).FullName}.Pregnancy type! Aborting.");
				return;
			}

			var pregDueDateFI = pregT.GetField("DueDate", bindingFlags);

			if (pregDueDateFI == null)
			{
				trace.Add($"Could not resolve {pregT.FullName}.DueDate field! Aborting.");
				return;
			}

			// OK, done setting up reflection info. Start by grabbing the instance of the behavior (gee, a public API!):
			var pregBehavior = GetCampaignBehavior<PregnancyCampaignBehavior>();

			if (pregBehavior == null)
			{
				trace.Add($"Could not find campaign behavior {typeof(PregnancyCampaignBehavior).FullName}! Aborting.");
				return;
			}

			// Now iterate over the pregnancy list:
			var pregList = pregListFI.GetValue(pregBehavior) as IReadOnlyList<object>;

			if (pregList == null)
			{
				trace.Add($"Could not access {pregListFI.Name} as IReadOnlyList<object>! Aborting.");
				return;
			}

			var dueDateDeltaCT = CampaignTime.Days(dueDateDelta);
			int nPregs = 0;

			foreach (object preg in pregList)
			{
				CampaignTime dueDateCT = (CampaignTime)pregDueDateFI.GetValue(preg);
				pregDueDateFI.SetValue(preg, dueDateCT + dueDateDeltaCT);
				++nPregs;
			}

			trace.Add($"Processed {nPregs} in-progress pregnancies.");
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
		private SavedValues _savedValues = new SavedValues();
		private const float VanillaPregnancyDuration = 36f;
	}
}
