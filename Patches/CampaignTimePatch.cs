using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
    [HarmonyPatch(typeof(CampaignTime))]
	class CampaignTimePatch
	{
		public static TimeParams TP;

		/* necessary reflection info for the largely-internal class CampaignTime */
		public static readonly FieldInfo TicksFI = AccessTools.Field(typeof(CampaignTime), "_numTicks");
		public static readonly ConstructorInfo CtorCI = AccessTools.Constructor(typeof(CampaignTime), new[] { typeof(long) });
		public static readonly MethodInfo CurrentTicksMI = AccessTools.PropertyGetter(typeof(CampaignTime), "CurrentTicks");

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Elapsed[UNIT]sUntilNow */

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedMillisecondsUntilNow", MethodType.Getter)]
		static void ElapsedMillisecondsUntilNow(ref float __result)
		{
			__result *= TP.TickRatioMsecF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedSecondsUntilNow", MethodType.Getter)]
		static void ElapsedSecondsUntilNow(ref float __result)
		{
			__result *= TP.TickRatioSecF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedHoursUntilNow", MethodType.Getter)]
		static void ElapsedHoursUntilNow(ref float __result)
		{
			__result *= TP.TickRatioHourF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedDaysUntilNow", MethodType.Getter)]
		static void ElapsedDaysUntilNow(ref float __result)
		{
			__result *= TP.TickRatioDayF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedWeeksUntilNow", MethodType.Getter)]
		static void ElapsedWeeksUntilNow(ref float __result)
		{
			__result *= TP.TickRatioWeekF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
		static void ElapsedSeasonsUntilNow(ref float __result)
		{
			__result *= TP.TickRatioSeasonF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
		static void ElapsedYearsUntilNow(ref float __result)
		{
			__result *= TP.TickRatioYearF;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Remaining[UNIT]sFromNow */

		[HarmonyPostfix]
		[HarmonyPatch("RemainingMillisecondsFromNow", MethodType.Getter)]
		static void RemainingMillisecondsFromNow(ref float __result)
		{
			__result *= TP.TickRatioMsecF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingSecondsFromNow", MethodType.Getter)]
		static void RemainingSecondsFromNow(ref float __result)
		{
			__result *= TP.TickRatioSecF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingHoursFromNow", MethodType.Getter)]
		static void RemainingHoursFromNow(ref float __result)
		{
			__result *= TP.TickRatioHourF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingDaysFromNow", MethodType.Getter)]
		static void RemainingDaysFromNow(ref float __result)
		{
			__result *= TP.TickRatioDayF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingWeeksFromNow", MethodType.Getter)]
		static void RemainingWeeksFromNow(ref float __result)
		{
			__result *= TP.TickRatioWeekF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
		static void RemainingSeasonsFromNow(ref float __result)
		{
			__result *= TP.TickRatioSeasonF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
		static void RemainingYearsFromNow(ref float __result)
		{
			__result *= TP.TickRatioYearF;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* To[UNIT]s */

		[HarmonyPostfix]
		[HarmonyPatch("ToMilliseconds", MethodType.Getter)]
		static void ToMilliseconds(ref double __result)
		{
			__result *= TP.TickRatioMsec;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToSeconds", MethodType.Getter)]
		static void ToSeconds(ref double __result)
		{
			__result *= TP.TickRatioSec;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToMinutes", MethodType.Getter)]
		static void ToMinutes(ref double __result)
		{
			__result *= TP.TickRatioMin;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToHours", MethodType.Getter)]
		static void ToHours(ref double __result)
		{
			__result *= TP.TickRatioHour;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToDays", MethodType.Getter)]
		static void ToDays(ref double __result)
		{
			__result *= TP.TickRatioDay;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToWeeks", MethodType.Getter)]
		static void ToWeeks(ref double __result)
		{
			__result *= TP.TickRatioWeek;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToSeasons", MethodType.Getter)]
		static void ToSeasons(ref double __result)
		{
			__result *= TP.TickRatioSeason;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToYears", MethodType.Getter)]
		static void ToYears(ref double __result)
		{
			__result *= TP.TickRatioYear;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT] */

		[HarmonyPrefix]
		[HarmonyPatch("GetHourOfDay", MethodType.Getter)]
		static bool GetHourOfDay(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TP.TickPerHourL) % TimeParams.HourPerDayL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfWeek", MethodType.Getter)]
		static bool GetDayOfWeek(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TP.TickPerDayL) % TP.DayPerWeekL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeason", MethodType.Getter)]
		static bool GetDayOfSeason(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TP.TickPerDayL) % TP.DayPerSeasonL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfYear", MethodType.Getter)]
		static bool GetDayOfYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TP.TickPerDayL) % TP.DayPerYearL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetWeekOfSeason", MethodType.Getter)]
		static bool GetWeekOfSeason(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TP.TickPerWeekL) % TP.WeekPerSeasonL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYear", MethodType.Getter)]
		static bool GetSeasonOfYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(((long)TicksFI.GetValue(__instance) / TP.TickPerSeasonL) % TimeParams.SeasonPerYearL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetYear", MethodType.Getter)]
		static bool GetYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TP.TickPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT]f */

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeasonf", MethodType.Getter)]
		static bool GetDayOfSeasonf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder((double)((long)TicksFI.GetValue(__instance) / TP.TickPerDayL), (double)TP.DayPerSeasonL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYearf", MethodType.Getter)]
		static bool GetSeasonOfYearf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder((double)((long)TicksFI.GetValue(__instance) / TP.TickPerSeasonL), (double)TimeParams.SeasonPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]s (factory methods) */

		[HarmonyPrefix]
		[HarmonyPatch("Milliseconds")]
		static bool Milliseconds(long valueInMilliseconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInMilliseconds * TP.TickPerMsecL });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Seconds")]
		static bool Seconds(long valueInSeconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInSeconds * TP.TickPerSecL });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Minutes")]
		static bool Minutes(long valueInMinutes, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInMinutes * TP.TickPerMinL });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Hours")]
		static bool Hours(float valueInHours, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(TP.TickPerHour * valueInHours) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Days")]
		static bool Days(float valueInDays, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(TP.TickPerDay * valueInDays) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Weeks")]
		static bool Weeks(float valueInWeeeks, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(TP.TickPerWeek * valueInWeeeks) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Seasons")]
		static bool Seasons(float valueInSeasons, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(TP.TickPerSeason * valueInSeasons) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Years")]
		static bool Years(float valueInYears, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(TP.TickPerYear * valueInYears) });
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]sFromNow (factory methods) */

		[HarmonyPrefix]
		[HarmonyPatch("MillisecondsFromNow")]
		static bool MillisecondsFromNow(long valueInMilliseconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + valueInMilliseconds * TP.TickPerMsecL
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("SecondsFromNow")]
		static bool SecondsFromNow(long valueInSeconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + valueInSeconds * TP.TickPerSecL
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("MinutesFromNow")]
		static bool MinutesFromNow(long valueInMinutes, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + valueInMinutes * TP.TickPerMinL
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("HoursFromNow")]
		static bool HoursFromNow(float valueInHours, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(TP.TickPerHour * valueInHours)
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("DaysFromNow")]
		static bool DaysFromNow(float valueInDays, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(TP.TickPerDay * valueInDays)
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("WeeksFromNow")]
		static bool WeeksFromNow(float valueInWeeks, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(TP.TickPerWeek * valueInWeeks)
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("YearsFromNow")]
		static bool YearsFromNow(float valueInYears, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(TP.TickPerYear * valueInYears)
			});
			return false;
		}
	}
}
