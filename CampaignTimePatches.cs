using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer
{
    [HarmonyPatch(typeof(CampaignTime))]
	class CampaignTimePatches
	{
		/* units (only one adjusted from vanilla: days per week) */
		public const long MsecPerSecL    = 1000L;
		public const long SecPerMinL     = 60L;
		public const long MinPerHourL    = 60L;
		public const long HourPerDayL    = 24L;
		public const long DayPerWeekL    = 3L;
		public const long WeekPerSeasonL = 3L;
		public const long SeasonPerYearL = 4L;

		/* units (vanilla) */
		public const long OldDayPerWeekL = 7L;

		/* ticks per unit */
		public const long TickPerMsecL   = 5L;
		public const long TickPerSecL    = TickPerMsecL * MsecPerSecL;
		public const long TickPerMinL    = TickPerSecL * SecPerMinL;
		public const long TickPerHourL   = TickPerMinL * MinPerHourL;
		public const long TickPerDayL    = TickPerHourL * HourPerDayL;
		public const long TickPerWeekL   = TickPerDayL * DayPerWeekL;
		public const long TickPerSeasonL = TickPerWeekL * WeekPerSeasonL;
		public const long TickPerYearL   = TickPerSeasonL * SeasonPerYearL;

		/* derived time rate multiplier (from given ticks per millsecond) */
		public const double TimeMult = 10.0 / (double)TickPerMsecL;
		public const float TimeMultF = (float)TimeMult;

		/* floating-point versions (casting now to reduce busy-ness of All The Little Patches) */
		public const float TickPerMsecF   = (float)TickPerMsecL;
		public const float TickPerSecF    = (float)TickPerSecL;
		public const float TickPerMinF    = (float)TickPerMinL;
		public const float TickPerHourF   = (float)TickPerHourL;
		public const float TickPerDayF    = (float)TickPerDayL;
		public const float TickPerWeekF   = (float)TickPerWeekL;
		public const float TickPerSeasonF = (float)TickPerSeasonL;
		public const float TickPerYearF   = (float)TickPerYearL;

		/* old/vanilla floating-point versions, where different */
		public const float OldTickPerWeekF   = (float)OldDayPerWeekL * TickPerDayF;
		public const float OldTickPerSeasonF = (float)WeekPerSeasonL * OldTickPerWeekF;
		public const float OldTickPerYearF   = (float)SeasonPerYearL * OldTickPerSeasonF;

		/* necessary reflection info for the largely-internal class CampaignTime */
		private static readonly FieldInfo TicksFI = AccessTools.Field(typeof(CampaignTime), "_numTicks");
		private static readonly ConstructorInfo CtorCI = AccessTools.Constructor(typeof(CampaignTime), new[] { typeof(long) });
		private static readonly MethodInfo CurrentTicksMI = AccessTools.PropertyGetter(typeof(CampaignTime), "CurrentTicks");

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Elapsed[UNIT]sUntilNow */

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedMillisecondsUntilNow", MethodType.Getter)]
		static void ElapsedMillisecondsUntilNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedSecondsUntilNow", MethodType.Getter)]
		static void ElapsedSecondsUntilNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedHoursUntilNow", MethodType.Getter)]
		static void ElapsedHoursUntilNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedDaysUntilNow", MethodType.Getter)]
		static void ElapsedDaysUntilNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedWeeksUntilNow", MethodType.Getter)]
		static void ElapsedWeeksUntilNow(ref float __result)
		{
			__result *= TimeMultF * OldTickPerWeekF / TickPerWeekF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
		static void ElapsedSeasonsUntilNow(ref float __result)
		{
			__result *= TimeMultF * OldTickPerSeasonF / TickPerSeasonF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
		static void ElapsedYearsUntilNow(ref float __result)
		{
			__result *= TimeMultF * OldTickPerYearF / TickPerYearF;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Remaining[UNIT]sFromNow */

		[HarmonyPostfix]
		[HarmonyPatch("RemainingMillisecondsFromNow", MethodType.Getter)]
		static void RemainingMillisecondsFromNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingSecondsFromNow", MethodType.Getter)]
		static void RemainingSecondsFromNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingHoursFromNow", MethodType.Getter)]
		static void RemainingHoursFromNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingDaysFromNow", MethodType.Getter)]
		static void RemainingDaysFromNow(ref float __result)
		{
			__result *= TimeMultF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingWeeksFromNow", MethodType.Getter)]
		static void RemainingWeeksFromNow(ref float __result)
		{
			__result *= TimeMultF * OldTickPerWeekF / TickPerWeekF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
		static void RemainingSeasonsFromNow(ref float __result)
		{
			__result *= TimeMultF * OldTickPerSeasonF / TickPerSeasonF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
		static void RemainingYearsFromNow(ref float __result)
		{
			__result *= TimeMultF * OldTickPerYearF / TickPerYearF;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* To[UNIT]s */

		[HarmonyPostfix]
		[HarmonyPatch("ToMilliseconds", MethodType.Getter)]
		static void ToMilliseconds(ref double __result)
		{
			__result *= TimeMult;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToSeconds", MethodType.Getter)]
		static void ToSeconds(ref double __result)
		{
			__result *= TimeMult;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToMinutes", MethodType.Getter)]
		static void ToMinutes(ref double __result)
		{
			__result *= TimeMult;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToHours", MethodType.Getter)]
		static void ToHours(ref double __result)
		{
			__result *= TimeMult;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToDays", MethodType.Getter)]
		static void ToDays(ref double __result)
		{
			__result *= TimeMult;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToWeeks", MethodType.Getter)]
		static void ToWeeks(ref double __result)
		{
			__result *= TimeMult * OldTickPerWeekF / TickPerWeekF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToSeasons", MethodType.Getter)]
		static void ToSeasons(ref double __result)
		{
			__result *= TimeMult * OldTickPerSeasonF / TickPerSeasonF;
		}

		[HarmonyPostfix]
		[HarmonyPatch("ToYears", MethodType.Getter)]
		static void ToYears(ref double __result)
		{
			__result *= TimeMult * OldTickPerYearF / TickPerYearF;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT] */

		[HarmonyPrefix]
		[HarmonyPatch("GetHourOfDay", MethodType.Getter)]
		static bool GetHourOfDay(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerHourL % HourPerDayL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfWeek", MethodType.Getter)]
		static bool GetDayOfWeek(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerDayL % DayPerWeekL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeason", MethodType.Getter)]
		static bool GetDayOfSeason(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerDayL % (DayPerWeekL * WeekPerSeasonL));
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfYear", MethodType.Getter)]
		static bool GetDayOfYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerDayL % (DayPerWeekL * WeekPerSeasonL * SeasonPerYearL));
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetWeekOfSeason", MethodType.Getter)]
		static bool GetWeekOfSeason(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerWeekL % WeekPerSeasonL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYear", MethodType.Getter)]
		static bool GetSeasonOfYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerSeasonL % SeasonPerYearL);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetYear", MethodType.Getter)]
		static bool GetYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)((long)TicksFI.GetValue(__instance) / TickPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT]f */

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeasonf", MethodType.Getter)]
		static bool GetDayOfSeasonf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder((double)((long)TicksFI.GetValue(__instance) / TickPerDayL), (double)(DayPerWeekL * WeekPerSeasonL));
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYearf", MethodType.Getter)]
		static bool GetSeasonOfYearf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder((double)((long)TicksFI.GetValue(__instance) / TickPerSeasonL), (double)SeasonPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]s (factory methods) */

		[HarmonyPrefix]
		[HarmonyPatch("Milliseconds")]
		static bool Milliseconds(long valueInMilliseconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInMilliseconds * TickPerMsecL });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Seconds")]
		static bool Seconds(long valueInSeconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInSeconds * TickPerSecL });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Minutes")]
		static bool Minutes(long valueInMinutes, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { valueInMinutes * TickPerMinL });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Hours")]
		static bool Hours(float valueInHours, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(valueInHours * TickPerHourF) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Days")]
		static bool Days(float valueInDays, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(valueInDays * TickPerDayF) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Weeks")]
		static bool Weeks(float valueInWeeeks, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(valueInWeeeks * TickPerWeekF) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Seasons")]
		static bool Seasons(float valueInSeasons, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(valueInSeasons * TickPerSeasonF) });
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Years")]
		static bool Years(float valueInYears, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[] { (long)(valueInYears * TickPerYearF) });
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
				(long)CurrentTicksMI.Invoke(null, null) + valueInMilliseconds * TickPerMsecL
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("SecondsFromNow")]
		static bool SecondsFromNow(long valueInSeconds, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + valueInSeconds * TickPerSecL
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("MinutesFromNow")]
		static bool MinutesFromNow(long valueInMinutes, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + valueInMinutes * TickPerMinL
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("HoursFromNow")]
		static bool HoursFromNow(float valueInHours, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(valueInHours * TickPerHourF)
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("DaysFromNow")]
		static bool DaysFromNow(float valueInDays, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(valueInDays * TickPerDayF)
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("WeeksFromNow")]
		static bool WeeksFromNow(float valueInWeeks, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(valueInWeeks * TickPerWeekF)
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("YearsFromNow")]
		static bool YearsFromNow(float valueInYears, ref CampaignTime __result)
		{
			__result = (CampaignTime)CtorCI.Invoke(new object[]
			{
				(long)CurrentTicksMI.Invoke(null, null) + (long)(valueInYears * TickPerYearF)
			});
			return false;
		}
	}
}
