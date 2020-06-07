using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer
{
	class SaveBehavior : CampaignBehaviorBase
	{
		public SaveBehavior()
		{
			_isNewGame = false;
			_savedTime = null;
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnNewGameCreated));
		}

		public override void SyncData(IDataStore dataStore) {
			var trace = new List<string>();

			if (Campaign.Current != null)
			{
				bool isLoad = true;

				if (_savedTime == null && !_isNewGame)
					trace.Add("Loading data from save...");
				else
				{
					trace.Add("Saving data to save...");
					isLoad = false;
					_savedTime = new SimpleTime(CampaignTime.Now);
				}

				dataStore.SyncData<SimpleTime>("CampaignPacer.CalendarTime", ref _savedTime);

				trace.AddRange(new List<string>
				{
					$"Campaign tick date:   {CampaignTime.Now}",
					$"Stored calendar date: {_savedTime}",
				});

				if (isLoad)
					AdjustTimeOnLoad(trace);
			}
			else
				trace.Add("Campaign.Current is null, so skipped synchronization.");

			Util.EventTracer.Trace(trace);
		}

		private void AdjustTimeOnLoad(List<string> trace)
		{
			CampaignTime adjustedTime;

			if (_savedTime == null || _savedTime.IsNull)
			{
				// Load of save that didn't have CP enabled.
				// Convert "current" campaign time back to what it would have been under the vanilla calendar.

				trace.Add("Loading a vanilla save...");
				trace.Add($"Apparent campaign start time: {Campaign.Current.CampaignStartTime}");
				var now = CampaignTime.Now;
				trace.Add($"Apparent CampaignTime.Now.ToYears: {now.ToYears}");
				double adjustedYears = now.ToYears / Main.TimeParam.TickRatioYear;
				trace.Add($"Adjusted years: {adjustedYears}");
				adjustedTime = CampaignTime.Years((float)adjustedYears);
				_savedTime = new SimpleTime(adjustedTime); // must become non-null
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

			Patches.CampaignPatch.MapTimeTrackerSetMI.Invoke(Campaign.Current, new object[]
			{
				Patches.CampaignPatch.MapTimeTrackerCtorCI.Invoke(new object[] { adjustedTime })
			});
		}

		protected void OnNewGameCreated(CampaignGameStarter starter)
		{
			_isNewGame = true;
		}

		private bool _isNewGame;
		private SimpleTime _savedTime;
	}
}
