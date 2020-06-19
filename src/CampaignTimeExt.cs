using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer
{
	public static class CampaignTimeExt
	{
		private static readonly ConstructorInfo CtorCI = AccessTools.Constructor(typeof(CampaignTime), new[] { typeof(long) });
		private static readonly FieldInfo TicksFI = AccessTools.Field(typeof(CampaignTime), "_numTicks");

		// more static factory methods for CampaignTime
		public static CampaignTime Ticks(long ticks) => (CampaignTime)CtorCI.Invoke(new object[] { ticks });

		public static CampaignTime HoursD(double value) => Ticks((long)(value * TimeParams.OldTickPerHourD));

		public static CampaignTime DaysD(double value) => Ticks((long)(value * TimeParams.OldTickPerDayD));

		public static CampaignTime WeeksD(double value) => Ticks((long)(value * TimeParams.OldTickPerWeekD));

		public static CampaignTime SeasonsD(double value) => Ticks((long)(value * TimeParams.OldTickPerSeasonD));

		public static CampaignTime YearsD(double value) => Ticks((long)(value * TimeParams.OldTickPerYearD));

		// extension method for CampaignTime to access its raw tick value:
		public static long GetTicks(this CampaignTime instance) => (long)TicksFI.GetValue(instance);
	}
}
