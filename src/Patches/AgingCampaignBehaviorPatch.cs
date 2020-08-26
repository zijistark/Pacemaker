using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(AgingCampaignBehavior))]
	class AgingCampaignBehaviorPatch
	{
		private static readonly MethodInfo UpdateHeroDeathProbabilitiesMI = AccessTools.Method(typeof(AgingCampaignBehavior), "UpdateHeroDeathProbabilities");

		[HarmonyPrefix]
		[HarmonyPatch("WeeklyTick")]
		static bool WeeklyTick() => false; // not in use now, as the meager and yet now correct calculation has been moved to a DailyTick prefix

		[HarmonyPrefix]
		[HarmonyPatch("DailyTick")]
		static void DailyTick(ref AgingCampaignBehavior __instance)
		{
			int daysElapsed = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
			int updatePeriod = Util.NearEqual(Main.Settings.AgeFactor, 1f, 1e-2)
				? Main.TimeParam.DayPerYear
				: (int)(Main.TimeParam.DayPerYear / Main.Settings.AgeFactor);

			if (updatePeriod <= 0)
				updatePeriod = 1;

			// globally update death probabilities every year of accumulated age
			if ((daysElapsed % updatePeriod) == 0)
				UpdateHeroDeathProbabilitiesMI.Invoke(__instance, null);
		}
	}
}
