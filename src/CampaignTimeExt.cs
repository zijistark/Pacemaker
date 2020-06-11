using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer
{
	public static class CampaignTimeExt
	{
		private static readonly ConstructorInfo CtorCI = AccessTools.Constructor(typeof(CampaignTime), new[] { typeof(long) });

		public static CampaignTime Ticks(long ticks) => (CampaignTime)CtorCI.Invoke(new object[] { ticks });

		public static CampaignTime HoursD(double value) => Ticks((long)(value * TimeParams.TickPerHourD));

		public static CampaignTime DaysD(double value) => Ticks((long)(value * TimeParams.TickPerDayD));

		public static CampaignTime WeeksD(double value) => Ticks((long)(value * TimeParams.TickPerWeekD));

		public static CampaignTime SeasonsD(double value) => Ticks((long)(value * Main.TimeParam.TickPerSeasonD));

		public static CampaignTime YearsD(double value) => Ticks((long)(value * Main.TimeParam.TickPerYearD));
	}
}
