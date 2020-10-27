using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(CampaignTime))]
	class CampaignTimePatch
	{
		private static readonly MethodInfo CurrentTicksMI = AccessTools.PropertyGetter(typeof(CampaignTime), "CurrentTicks");

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Elapsed[UNIT]sUntilNow

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
		static bool ElapsedSeasonsUntilNow(ref float __result, long ____numTicks)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - ____numTicks;
			__result = dTicks / Main.TimeParam.TickPerSeasonF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
		static bool ElapsedYearsUntilNow(ref float __result, long ____numTicks)
		{
			long dTicks = (long)CurrentTicksMI.Invoke(null, null) - ____numTicks;
			__result = dTicks / Main.TimeParam.TickPerYearF;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Remaining[UNIT]sFromNow

		[HarmonyPrefix]
		[HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
		static bool RemainingSeasonsFromNow(ref float __result, long ____numTicks)
		{
			long dTicks = ____numTicks - (long)CurrentTicksMI.Invoke(null, null);
			__result = dTicks / Main.TimeParam.TickPerSeasonF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
		static bool RemainingYearsFromNow(ref float __result, long ____numTicks)
		{
			long dTicks = ____numTicks - (long)CurrentTicksMI.Invoke(null, null);
			__result = dTicks / Main.TimeParam.TickPerYearF;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// To[UNIT]s

		[HarmonyPrefix]
		[HarmonyPatch("ToSeasons", MethodType.Getter)]
		static bool ToSeasons(ref double __result, long ____numTicks)
		{
			__result = ____numTicks / Main.TimeParam.TickPerSeasonD;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ToYears", MethodType.Getter)]
		static bool ToYears(ref double __result, long ____numTicks)
		{
			__result = ____numTicks / Main.TimeParam.TickPerYearD;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Get[UNIT]Of[UNIT]

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeason", MethodType.Getter)]
		static bool GetDayOfSeason(ref int __result, long ____numTicks)
		{
			long nDay = ____numTicks / TimeParams.TickPerDayL;
			__result = (int)(nDay % Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfYear", MethodType.Getter)]
		static bool GetDayOfYear(ref int __result, long ____numTicks)
		{
			long nDay = ____numTicks / TimeParams.TickPerDayL;
			__result = (int)(nDay % Main.TimeParam.DayPerYear);
			return false;
		}

		//[HarmonyPrefix]
		//[HarmonyPatch("GetWeekOfSeason", MethodType.Getter)]
		//static bool GetWeekOfSeason(ref int __result, long ____numTicks)
		//{
		//	__result = (int)((____numTicks / Main.TimeParam.TickPerWeekL) % Main.TimeParam.WeekPerSeasonL);
		//	return false;
		//}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYear", MethodType.Getter)]
		static bool GetSeasonOfYear(ref int __result, long ____numTicks)
		{
			long nSeason = ____numTicks / Main.TimeParam.TickPerSeasonL;
			__result = (int)(nSeason % TimeParams.SeasonPerYear);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetYear", MethodType.Getter)]
		static bool GetYear(ref int __result, long ____numTicks)
		{
			__result = (int)(____numTicks / Main.TimeParam.TickPerYearL);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Get[UNIT]Of[UNIT]f

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfSeasonf", MethodType.Getter)]
		static bool GetDayOfSeasonf(ref float __result, long ____numTicks)
		{
			long nDay = ____numTicks / TimeParams.TickPerDayL;
			__result = (float)Math.IEEERemainder(nDay, Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYearf", MethodType.Getter)]
		static bool GetSeasonOfYearf(ref float __result, long ____numTicks)
		{
			long nSeason = ____numTicks / Main.TimeParam.TickPerSeasonL;
			__result = (float)Math.IEEERemainder(nSeason, TimeParams.SeasonPerYear);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]s (factory methods) */

		[HarmonyPrefix]
		[HarmonyPatch("Seasons")]
		static bool Seasons(float valueInSeasons, ref CampaignTime __result)
		{
			long ticks = (long)(valueInSeasons * Main.TimeParam.TickPerSeasonF);
			__result = CampaignTimeExt.Ticks(ticks);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Years")]
		static bool Years(float valueInYears, ref CampaignTime __result)
		{
			long ticks = (long)(valueInYears * Main.TimeParam.TickPerYearF);
			__result = CampaignTimeExt.Ticks(ticks);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// [UNIT]sFromNow (factory methods)

		// NOTE: SeasonsFromNow doesn't exist

		[HarmonyPrefix]
		[HarmonyPatch("YearsFromNow")]
		static bool YearsFromNow(float valueInYears, ref CampaignTime __result)
		{
			var now = (long)CurrentTicksMI.Invoke(null, null);
			var ticks = (long)(valueInYears * Main.TimeParam.TickPerYearF);
			__result = CampaignTimeExt.Ticks(now + ticks);
			return false;
		}
	}
}
