using TaleWorlds.CampaignSystem;

namespace CampaignPacer
{
    class TickTraceBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, OnWeeklyTick);
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
		}

		public override void SyncData(IDataStore dataStore)	{ }

		public void OnWeeklyTick()
        {
			Util.EventTracer.Trace();
		}

		public void OnDailyTick()
		{
			Util.EventTracer.Trace();
		}

		public void OnHourlyTick()
		{
			Util.EventTracer.Trace();
		}
	}
}
