using HarmonyLib;

using System.Reflection;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Extensions
{
    internal static class CampaignTimeExtensions
    {
        private static readonly ConstructorInfo CtorCI = AccessTools.Constructor(typeof(CampaignTime), new[] { typeof(long) });
        private static readonly FieldInfo TicksFI = AccessTools.Field(typeof(CampaignTime), "_numTicks");

        // more static factory methods for CampaignTime
        public static CampaignTime Ticks(long ticks) => (CampaignTime)CtorCI.Invoke(new object[] { ticks });

        public static CampaignTime HoursD(double value) => Ticks((long)(value * TimeParams.TickPerHourD));

        public static CampaignTime DaysD(double value) => Ticks((long)(value * TimeParams.TickPerDayD));

        public static CampaignTime WeeksD(double value) => Ticks((long)(value * TimeParams.TickPerWeekD));

        public static CampaignTime SeasonsD(double value) => Ticks((long)(value * Main.TimeParam.TickPerSeasonD));

        public static CampaignTime YearsD(double value) => Ticks((long)(value * Main.TimeParam.TickPerYearD));

        // extension method for CampaignTime to access its raw tick value:
        public static long GetTicks(this CampaignTime instance) => (long)TicksFI.GetValue(instance);
    }
}
