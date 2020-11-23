using System;

using HarmonyLib;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(CampaignTime))]
	internal static class CampaignTimePatch
	{
		private delegate long CurrentTicksDelegate();
		private static readonly Reflect.DeclaredGetter<CampaignTime> CurrentTicksRG = new("CurrentTicks");
		private static readonly CurrentTicksDelegate CurrentTicks = CurrentTicksRG.GetDelegate<CurrentTicksDelegate>();

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Elapsed[UNIT]sUntilNow

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedSeasonsUntilNow", MethodType.Getter)]
		static bool ElapsedSeasonsUntilNow(ref float __result, long ____numTicks)
		{
			__result = (CurrentTicks() - ____numTicks) / Main.TimeParam.TickPerSeasonF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("ElapsedYearsUntilNow", MethodType.Getter)]
		static bool ElapsedYearsUntilNow(ref float __result, long ____numTicks)
		{
			__result = (CurrentTicks() - ____numTicks) / Main.TimeParam.TickPerYearF;
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Remaining[UNIT]sFromNow

		[HarmonyPrefix]
		[HarmonyPatch("RemainingSeasonsFromNow", MethodType.Getter)]
		static bool RemainingSeasonsFromNow(ref float __result, long ____numTicks)
		{
			__result = (____numTicks - CurrentTicks()) / Main.TimeParam.TickPerSeasonF;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemainingYearsFromNow", MethodType.Getter)]
		static bool RemainingYearsFromNow(ref float __result, long ____numTicks)
		{
			__result = (____numTicks - CurrentTicks()) / Main.TimeParam.TickPerYearF;
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
			__result = (int)(____numTicks / TimeParams.TickPerDayL % Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetDayOfYear", MethodType.Getter)]
		static bool GetDayOfYear(ref int __result, long ____numTicks)
		{
			__result = (int)(____numTicks / TimeParams.TickPerDayL % Main.TimeParam.DayPerYear);
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
			__result = (float)Math.IEEERemainder(____numTicks / TimeParams.TickPerDayL, Main.TimeParam.DayPerSeason);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetSeasonOfYearf", MethodType.Getter)]
		static bool GetSeasonOfYearf(ref float __result, long ____numTicks)
		{
			__result = (float)Math.IEEERemainder(____numTicks / Main.TimeParam.TickPerSeasonL, TimeParams.SeasonPerYear);
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		/* [UNIT]s (factory methods) */

		[HarmonyPrefix]
		[HarmonyPatch("Seasons")]
		static bool Seasons(float valueInSeasons, ref CampaignTime __result)
		{
			__result = CampaignTimeExt.Ticks((long)(valueInSeasons * Main.TimeParam.TickPerSeasonF));
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("Years")]
		static bool Years(float valueInYears, ref CampaignTime __result)
		{
			__result = CampaignTimeExt.Ticks((long)(valueInYears * Main.TimeParam.TickPerYearF));
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// [UNIT]sFromNow (factory methods)

		// NOTE: SeasonsFromNow doesn't exist

		[HarmonyPrefix]
		[HarmonyPatch("YearsFromNow")]
		static bool YearsFromNow(float valueInYears, ref CampaignTime __result)
		{
			__result = CampaignTimeExt.Ticks(CurrentTicks() + (long)(valueInYears * Main.TimeParam.TickPerYearF));
			return false;
		}
	}
}
