using TaleWorlds.CampaignSystem;

namespace Pacemaker
{
    internal sealed class TickTraceBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, OnWeeklyTick);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, OnDailyTickClan);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        }

        public override void SyncData(IDataStore dataStore)	{ }

        public void OnWeeklyTick() => Util.EventTracer.Trace();

        public void OnDailyTick() => Util.EventTracer.Trace();

        public void OnDailyTickClan(Clan clan)
        {
            if (clan != Clan.PlayerClan)
                return;

            Util.EventTracer.Trace($"Fired for Player Clan: {clan.Name}");
        }

        public void OnHourlyTick() => Util.EventTracer.Trace();
    }
}
