using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace CampaignPacer.Patches
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
			int seasonsElapsed = (int)Campaign.Current.CampaignStartTime.ElapsedSeasonsUntilNow;

			// globally update death probabilities yearly
			if ((seasonsElapsed % TimeParams.SeasonPerYear) == 0)
				UpdateHeroDeathProbabilitiesMI.Invoke(__instance, null);
		}
	}
}
