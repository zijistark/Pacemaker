using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(CampaignTime))]
	class CampaignTimePatch
	{
		private class Ticks
		{
			private const long RawTicksPerSeason = TimeParams.OldTickPerSeasonL;
			private const long RawDaysPerSeason = TimeParams.OldDayPerWeek * TimeParams.OldWeekPerSeason;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static long Convert(long rawTicks)
			{
				long nonSeasonal = rawTicks % RawTicksPerSeason;
				long seasonal = rawTicks - nonSeasonal;
				seasonal *= Main.TimeParam.DayPerSeason;
				seasonal /= RawDaysPerSeason;
				return seasonal + nonSeasonal;
			}
		}

		// Necessary reflection info for the largely-internal class CampaignTime:
		private static readonly MethodInfo CurrentTicksMI = AccessTools.PropertyGetter(typeof(CampaignTime), "CurrentTicks");
		private static readonly FieldInfo TicksFI = AccessTools.Field(typeof(CampaignTime), "_numTicks");

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Elapsed[UNIT]sUntilNow */

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedMillisecondsUntilNow", MethodType.Getter)]
		static bool ElapsedMillisecondsUntilNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - (long)TicksFI.GetValue(__instance);
			__result = Ticks.Convert(dTicks) / TimeParams.TickPerMsecF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedSecondsUntilNow", MethodType.Getter)]
		static bool ElapsedSecondsUntilNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - (long)TicksFI.GetValue(__instance);
			__result = Ticks.Convert(dTicks) / TimeParams.TickPerSecF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedDaysUntilNow", MethodType.Getter)]
		static bool ElapsedDaysUntilNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - (long)TicksFI.GetValue(__instance);
			__result = Ticks.Convert(dTicks) / TimeParams.TickPerDayF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedWeeksUntilNow", MethodType.Getter)]
		static bool ElapsedWeeksUntilNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - (long)TicksFI.GetValue(__instance);
			__result = Ticks.Convert(dTicks) / TimeParams.OldTickPerWeekF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
		static bool ElapsedSeasonsUntilNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - (long)TicksFI.GetValue(__instance);
			__result = Ticks.Convert(dTicks) / Main.TimeParam.TickPerSeasonF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
		static bool ElapsedYearsUntilNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - (long)TicksFI.GetValue(__instance);
			__result = Ticks.Convert(dTicks) / Main.TimeParam.TickPerYearF;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Remaining[UNIT]sFromNow */

		[HarmonyPrefix]
		[HarmonyPatch("RemainingMillisecondsFromNow", MethodType.Getter)]
		static bool RemainingMillisecondsFromNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)TicksFI.GetValue(__instance) - (long)CurrentTicksMI.Invoke(null, null);
			__result = Ticks.Convert(dTicks) / TimeParams.TickPerMsecF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingSecondsFromNow", MethodType.Getter)]
		static bool RemainingSecondsFromNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)TicksFI.GetValue(__instance) - (long)CurrentTicksMI.Invoke(null, null);
			__result = Ticks.Convert(dTicks) / TimeParams.TickPerSecF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingDaysFromNow", MethodType.Getter)]
		static bool RemainingDaysFromNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)TicksFI.GetValue(__instance) - (long)CurrentTicksMI.Invoke(null, null);
			__result = Ticks.Convert(dTicks) / TimeParams.TickPerDayF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingWeeksFromNow", MethodType.Getter)]
		static bool RemainingWeeksFromNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)TicksFI.GetValue(__instance) - (long)CurrentTicksMI.Invoke(null, null);
			__result = Ticks.Convert(dTicks) / TimeParams.OldTickPerWeekF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
		static bool RemainingSeasonsFromNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)TicksFI.GetValue(__instance) - (long)CurrentTicksMI.Invoke(null, null);
			__result = Ticks.Convert(dTicks) / Main.TimeParam.TickPerSeasonF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
		static bool RemainingYearsFromNow(ref CampaignTime __instance, ref float __result)
		{
			long dTicks = (long)TicksFI.GetValue(__instance) - (long)CurrentTicksMI.Invoke(null, null);
			__result = Ticks.Convert(dTicks) / Main.TimeParam.TickPerYearF;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* To[UNIT]s */

		[HarmonyPrefix]
		[HarmonyPatch("ToMilliseconds", MethodType.Getter)]
		static bool ToMilliseconds(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerMsecD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToSeconds", MethodType.Getter)]
		static bool ToSeconds(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerSecD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToMinutes", MethodType.Getter)]
		static bool ToMinutes(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerMinD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToHours", MethodType.Getter)]
		static bool ToHours(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerHourD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToDays", MethodType.Getter)]
		static bool ToDays(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerDayD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToWeeks", MethodType.Getter)]
		static bool ToWeeks(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.OldTickPerWeekD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToSeasons", MethodType.Getter)]
		static bool ToSeasons(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / Main.TimeParam.TickPerSeasonD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToYears", MethodType.Getter)]
		static bool ToYears(ref CampaignTime __instance, ref double __result)
		{
			__result = Ticks.Convert((long)TicksFI.GetValue(__instance)) / Main.TimeParam.TickPerYearD;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT] */

		[HarmonyPrefix]
		[HarmonyPatch("GetHourOfDay", MethodType.Getter)]
		static bool GetHourOfDay(ref CampaignTime __instance, ref int __result)
		{
			long nHour = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerHourL;
			__result = (int)(nHour % TimeParams.HourPerDay);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfWeek", MethodType.Getter)]
		static bool GetDayOfWeek(ref CampaignTime __instance, ref int __result)
		{
			long nDay = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerDayL;
			__result = (int)(nDay % TimeParams.DayPerWeek);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeason", MethodType.Getter)]
		static bool GetDayOfSeason(ref CampaignTime __instance, ref int __result)
		{
			long nDay = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerDayL;
			__result = (int)(nDay % Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfYear", MethodType.Getter)]
		static bool GetDayOfYear(ref CampaignTime __instance, ref int __result)
		{
			long nDay = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerDayL;
			__result = (int)(nDay % Main.TimeParam.DayPerYear);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetWeekOfSeason", MethodType.Getter)]
		static bool GetWeekOfSeason(ref CampaignTime __instance, ref int __result)
		{
			long nWeek = Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.OldTickPerWeekL;
			__result = (int)(nWeek % TimeParams.OldWeekPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYear", MethodType.Getter)]
		static bool GetSeasonOfYear(ref CampaignTime __instance, ref int __result)
		{
			long nSeason = Ticks.Convert((long)TicksFI.GetValue(__instance)) / Main.TimeParam.TickPerSeasonL;
			__result = (int)(nSeason % TimeParams.SeasonPerYear);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetYear", MethodType.Getter)]
		static bool GetYear(ref CampaignTime __instance, ref int __result)
		{
			__result = (int)(Ticks.Convert((long)TicksFI.GetValue(__instance)) / Main.TimeParam.TickPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* Get[UNIT]Of[UNIT]f */

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeasonf", MethodType.Getter)]
		static bool GetDayOfSeasonf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder(
				Ticks.Convert((long)TicksFI.GetValue(__instance)) / TimeParams.TickPerDayL,
				Main.TimeParam.DayPerSeason
				);

			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYearf", MethodType.Getter)]
		static bool GetSeasonOfYearf(ref CampaignTime __instance, ref float __result)
		{
			__result = (float)Math.IEEERemainder(
				Ticks.Convert((long)TicksFI.GetValue(__instance)) / Main.TimeParam.TickPerSeasonL,
				TimeParams.SeasonPerYear
				);

			return false;
		}
	}
}
