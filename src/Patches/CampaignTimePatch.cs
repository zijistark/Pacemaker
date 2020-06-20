using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(CampaignTime))]
	class CampaignTimePatch
	{
		/* necessary reflection info for the largely-internal class CampaignTime */
		private static readonly FieldInfo TicksFI = AccessTools.Field(typeof(CampaignTime), "_numTicks");
		private static readonly ConstructorInfo CtorCI = AccessTools.Constructor(typeof(CampaignTime), new[] { typeof(long) });
		private static readonly MethodInfo CurrentTicksMI = AccessTools.PropertyGetter(typeof(CampaignTime), "CurrentTicks");

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Elapsed[UNIT]sUntilNow */

		//[HarmonyPostfix]
		//[HarmonyPatch("ElapsedMillisecondsUntilNow", MethodType.Getter)]
		//static void ElapsedMillisecondsUntilNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioMsecF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ElapsedSecondsUntilNow", MethodType.Getter)]
		//static void ElapsedSecondsUntilNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioSecF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ElapsedHoursUntilNow", MethodType.Getter)]
		//static void ElapsedHoursUntilNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioHourF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ElapsedDaysUntilNow", MethodType.Getter)]
		//static void ElapsedDaysUntilNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioDayF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ElapsedWeeksUntilNow", MethodType.Getter)]
		//static void ElapsedWeeksUntilNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioWeekF;
		//}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
		static void ElapsedSeasonsUntilNow(ref float __result) => __result *= Main.TimeParam.TickRatioSeasonF;

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
		static void ElapsedYearsUntilNow(ref float __result) => __result *= Main.TimeParam.TickRatioYearF;

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Remaining[UNIT]sFromNow */

		//[HarmonyPostfix]
		//[HarmonyPatch("RemainingMillisecondsFromNow", MethodType.Getter)]
		//static void RemainingMillisecondsFromNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioMsecF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("RemainingSecondsFromNow", MethodType.Getter)]
		//static void RemainingSecondsFromNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioSecF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("RemainingHoursFromNow", MethodType.Getter)]
		//static void RemainingHoursFromNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioHourF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("RemainingDaysFromNow", MethodType.Getter)]
		//static void RemainingDaysFromNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioDayF;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("RemainingWeeksFromNow", MethodType.Getter)]
		//static void RemainingWeeksFromNow(ref float __result)
		//{
		//	__result *= Main.TimeParam.TickRatioWeekF;
		//}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
		static void RemainingSeasonsFromNow(ref float __result) => __result *= Main.TimeParam.TickRatioSeasonF;

		[HarmonyPostfix]
		[HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
		static void RemainingYearsFromNow(ref float __result) => __result *= Main.TimeParam.TickRatioYearF;

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* To[UNIT]s */

		//[HarmonyPostfix]
		//[HarmonyPatch("ToMilliseconds", MethodType.Getter)]
		//static void ToMilliseconds(ref double __result)
		//{
		//	__result *= Main.TimeParam.TickRatioMsec;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ToSeconds", MethodType.Getter)]
		//static void ToSeconds(ref double __result)
		//{
		//	__result *= Main.TimeParam.TickRatioSec;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ToMinutes", MethodType.Getter)]
		//static void ToMinutes(ref double __result)
		//{
		//	__result *= Main.TimeParam.TickRatioMin;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ToHours", MethodType.Getter)]
		//static void ToHours(ref double __result)
		//{
		//	__result *= Main.TimeParam.TickRatioHour;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ToDays", MethodType.Getter)]
		//static void ToDays(ref double __result)
		//{
		//	__result *= Main.TimeParam.TickRatioDay;
		//}

		//[HarmonyPostfix]
		//[HarmonyPatch("ToWeeks", MethodType.Getter)]
		//static void ToWeeks(ref double __result)
		//{
		//	__result *= Main.TimeParam.TickRatioWeek;
		//}

		[HarmonyPostfix]
		[HarmonyPatch("ToSeasons", MethodType.Getter)]
		static void ToSeasons(ref double __result) => __result *= Main.TimeParam.TickRatioSeasonD;

		[HarmonyPostfix]
		[HarmonyPatch("ToYears", MethodType.Getter)]
		static void ToYears(ref double __result) => __result *= Main.TimeParam.TickRatioYearD;

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT] */

		//[HarmonyPrefix]
		//[HarmonyPatch("GetHourOfDay", MethodType.Getter)]
		//static bool GetHourOfDay(ref CampaignTime __instance, ref int __result)
		//{
		//	__result = (int)(((long)TicksFI.GetValue(__instance) / TimeParams.TickPerHourL) % TimeParams.HourPerDayL);
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("GetDayOfWeek", MethodType.Getter)]
		//static bool GetDayOfWeek(ref CampaignTime __instance, ref int __result)
		//{
		//	__result = (int)(((long)TicksFI.GetValue(__instance) / TimeParams.TickPerDayL) % Main.TimeParam.DayPerWeekL);
		//	return false;
		//}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeason", MethodType.Getter)]
		static bool GetDayOfSeason(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TimeParams.TickPerDayL) % Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfYear", MethodType.Getter)]
		static bool GetDayOfYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TimeParams.TickPerDayL) % Main.TimeParam.DayPerYear);
			return false;
		}

		//[HarmonyPrefix]
		//[HarmonyPatch("GetWeekOfSeason", MethodType.Getter)]
		//static bool GetWeekOfSeason(ref CampaignTime __instance, ref int __result)
		//{
		//	__result = (int)(((long)TicksFI.GetValue(__instance) / Main.TimeParam.TickPerWeekL) % Main.TimeParam.WeekPerSeasonL);
		//	return false;
		//}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYear", MethodType.Getter)]
		static bool GetSeasonOfYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / Main.TimeParam.TickPerSeasonL) % TimeParams.SeasonPerYear);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetYear", MethodType.Getter)]
		static bool GetYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / Main.TimeParam.TickPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT]f */

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeasonf", MethodType.Getter)]
		static bool GetDayOfSeasonf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder((double)((long)TicksFI.GetValue(__instance) / TimeParams.TickPerDayL), Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYearf", MethodType.Getter)]
		static bool GetSeasonOfYearf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder((double)((long)TicksFI.GetValue(__instance) / Main.TimeParam.TickPerSeasonL), TimeParams.SeasonPerYear);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]s (factory methods) */

		//[HarmonyPrefix]
		//[HarmonyPatch("Milliseconds")]
		//static bool Milliseconds(long valueInMilliseconds, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInMilliseconds * Main.TimeParam.TickPerMsecL });
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("Seconds")]
		//static bool Seconds(long valueInSeconds, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInSeconds * Main.TimeParam.TickPerSecL });
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("Minutes")]
		//static bool Minutes(long valueInMinutes, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInMinutes * Main.TimeParam.TickPerMinL });
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("Hours")]
		//static bool Hours(float valueInHours, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(Main.TimeParam.TickPerHour * valueInHours) });
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("Days")]
		//static bool Days(float valueInDays, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(Main.TimeParam.TickPerDay * valueInDays) });
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("Weeks")]
		//static bool Weeks(float valueInWeeeks, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(Main.TimeParam.TickPerWeek * valueInWeeeks) });
		//	return false;
		//}

		[HarmonyPrefix]
		[HarmonyPatch("Seasons")]
		static bool Seasons(float valueInSeasons, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(Main.TimeParam.TickPerSeasonD * valueInSeasons) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Years")]
		static bool Years(float valueInYears, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(Main.TimeParam.TickPerYearD * valueInYears) });
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]sFromNow (factory methods) */

		//[HarmonyPrefix]
		//[HarmonyPatch("MillisecondsFromNow")]
		//static bool MillisecondsFromNow(long valueInMilliseconds, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[]
		//	{
		//		(long)CurrentTicksMI.Invoke(null, null) + valueInMilliseconds * Main.TimeParam.TickPerMsecL
		//	});
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("SecondsFromNow")]
		//static bool SecondsFromNow(long valueInSeconds, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[]
		//	{
		//		(long)CurrentTicksMI.Invoke(null, null) + valueInSeconds * Main.TimeParam.TickPerSecL
		//	});
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("MinutesFromNow")]
		//static bool MinutesFromNow(long valueInMinutes, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[]
		//	{
		//		(long)CurrentTicksMI.Invoke(null, null) + valueInMinutes * Main.TimeParam.TickPerMinL
		//	});
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("HoursFromNow")]
		//static bool HoursFromNow(float valueInHours, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[]
		//	{
		//		(long)CurrentTicksMI.Invoke(null, null) + (long)(Main.TimeParam.TickPerHour * valueInHours)
		//	});
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("DaysFromNow")]
		//static bool DaysFromNow(float valueInDays, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[]
		//	{
		//		(long)CurrentTicksMI.Invoke(null, null) + (long)(Main.TimeParam.TickPerDay * valueInDays)
		//	});
		//	return false;
		//}

		//[HarmonyPrefix]
		//[HarmonyPatch("WeeksFromNow")]
		//static bool WeeksFromNow(float valueInWeeks, ref CampaignTime __result)
		//{
		//	__result = (CampaignTime)CtorCI.Invoke(new object[]
		//	{
		//		(long)CurrentTicksMI.Invoke(null, null) + (long)(Main.TimeParam.TickPerWeek * valueInWeeks)
		//	});
		//	return false;
		//}

		// NOTE: SeasonsFromNow doesn't exist

		[HarmonyPrefix]
		[HarmonyPatch("YearsFromNow")]
		static bool YearsFromNow(float valueInYears, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(Main.TimeParam.TickPerYearD * valueInYears)
			});
			return false;
		}
	}
}
