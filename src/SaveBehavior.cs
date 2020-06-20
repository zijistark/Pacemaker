using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Pacemaker
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
				SavedValues.Snapshot();
			}

			dataStore.SyncData($"{Main.Name}SavedValues", ref _savedValues);

			trace.Add($"Stored values: {SavedValues}");

			if (!HasLoaded)
				OnLoad(isVanilla: false, trace);

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
				OnLoad(isVanilla: true, trace);

			Util.EventTracer.Trace(trace);
		}

		protected void OnLoad(bool isVanilla, List<string> trace)
		{
			if (isVanilla)
				trace.Add("Loading vanilla save...");

			AdjustTimeParams(isVanilla, trace);
			WarnDayPerSeasonMismatch(isVanilla);
			AdjustPregnanciesOnLoad(trace);
			HasLoaded = true;
		}

		protected void AdjustTimeParams(bool isVanilla, List<string> trace)
		{
			var neededDps = (isVanilla) ? 21 : SavedValues.DaysPerSeason;

			if (Main.TimeParam.DayPerSeason != neededDps)
			{
				trace.Add($"Time param DayPerSeason={Main.TimeParam.DayPerSeason} is incorrect for this campaign.");
				Main.SetTimeParams(new TimeParams(neededDps), trace);
			}
		}

		protected void WarnDayPerSeasonMismatch(bool isVanilla)
		{
			if (isVanilla && Main.Settings.DaysPerSeason != 21)
			{
				// special popup reminder re: days/season, will only happen when first loading a vanilla save
			}

			if (Main.Settings.DaysPerSeason != Main.TimeParam.DayPerSeason)
			{
				InformationManager.DisplayMessage(
					new InformationMessage(
						$"{Main.DisplayName}: Using {Main.TimeParam.DayPerSeason} Days/Season",
						Main.ImportantTextColor));
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
				return;
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

			if (pregList.Count == 0)
			{
				trace.Add("No pregnancies are in-progress. Aborting.");
				return;
			}
			else
				trace.Add($"Pregnancies in-progress: {pregList.Count}");

			var dueDateDeltaCT = CampaignTime.Days(dueDateDelta);
			int nPregs = 0;

			foreach (var preg in pregList)
			{
				CampaignTime dueDateCT = (CampaignTime)pregDueDateFI.GetValue(preg);
				pregDueDateFI.SetValue(preg, dueDateCT + dueDateDeltaCT);
				++nPregs;
			}

			trace.Add($"Processed {nPregs} in-progress pregnancies.");
		}

		protected SavedValues SavedValues
		{
			get => _savedValues;
			set => _savedValues = value;
		}

		protected bool HasLoaded { get; set; }

		private SavedValues _savedValues = new SavedValues();
		private const float VanillaPregnancyDuration = 36f;
	}
}
