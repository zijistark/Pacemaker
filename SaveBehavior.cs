using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

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
			bool isLoad = !_isNewGame && !_hasLoadedData;

			if (isLoad)
				trace.Add("Loading saved data...");
			else
			{
				trace.Add("Saving data...");
				_savedTime = new SimpleTime(CampaignTime.Now);
			}

			dataStore.SyncData($"{Main.Name}SavedTime", ref _savedTime);

			trace.AddRange(new List<string>
			{
				$"Campaign tick date:   {CampaignTime.Now}",
				$"Stored calendar date: {((_savedTime != null) ? _savedTime.ToString() : "NULL")}",
			});

			if (isLoad)
			{
				AdjustTimeOnLoad(trace);
				_hasLoadedData = true;
			}

			Util.EventTracer.Trace(trace);
		}

		protected void OnNewGameCreated(CampaignGameStarter starter)
		{
			_isNewGame = true;
			Util.EventTracer.Trace();
		}

		// OnGameEarlyLoaded is only present so that we can still adjust the [incorrect] apparent time when adding
		// the mod to a save that didn't have it enabled (so-called "vanilla save"). This is because SyncData does
		// not even get called during game loading for behaviors belonging to submodules that were previously not
		// part of the save.
		protected void OnGameEarlyLoaded(CampaignGameStarter starter)
		{
			var trace = new List<string>();

			if (!_hasLoadedData && _savedTime == null)
			{
				AdjustTimeOnLoad(trace);
				_hasLoadedData = true;
			}

			Util.EventTracer.Trace(trace);
		}

		private void AdjustTimeOnLoad(List<string> trace)
		{
			var adjustedTime = CampaignTime.Zero;

			if (_savedTime == null)
			{
				// Load of save that didn't have CP enabled.
				// Convert "current" campaign time back to what it would have been under the vanilla calendar.

				trace.Add("Loading a vanilla save...");
				trace.Add($"Apparent campaign start time: {Campaign.Current.CampaignStartTime}");
				var now = CampaignTime.Now;
				trace.Add($"Apparent years: {now.ToYears:F4}");
				double adjustedYears = (double)now.ToYears / Main.TimeParam.TickRatioYear;
				trace.Add($"Adjusted years: {adjustedYears:F4}");
				adjustedTime = CampaignTime.Years((float)adjustedYears);
				_savedTime = new SimpleTime(adjustedTime); // become non-null
			}
			else if (_savedTime.IsValid)
			{
				// Normal load of prior CP-enabled savegame.
				// Simply restore saved calendar date as-is.
				// This allows switching calendar parameters mid-playthrough, but it doesn't need to track the settings.

				trace.Add($"Loading a save that had {Main.Name} enabled...");
				adjustedTime = _savedTime.ToCampaignTime();
			}
			else
			{
				trace.Add("Loaded an invalid calendar date! Skipping any adjustment...");
				return;
			}

			if (adjustedTime != CampaignTime.Zero)
			{
				Patches.CampaignPatch.ResetCampaignStartTime(Campaign.Current);
				Patches.CampaignPatch.SetMapTimeTracker(Campaign.Current, adjustedTime);
			}
		}

		private bool _isNewGame = false;
		private bool _hasLoadedData = false;
		private SimpleTime _savedTime = null;
	}
}
