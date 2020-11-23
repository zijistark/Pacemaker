using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(DefaultMobilePartyFoodConsumptionModel))]
	internal static class DefaultMobilePartyFoodConsumptionModelPatch
	{
		private static readonly TextObject Explanation = new($"[{Main.DisplayName}] Time Multiplier");

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("CalculateDailyFoodConsumptionf")]
		private static void CalculateDailyFoodConsumptionf(MobileParty party, StatExplainer explainer, ref float __result)
		{
			if (!Main.Settings!.EnableFoodTweaks)
				return;

			float offset = (__result / Main.Settings.TimeMultiplier) - __result;

			if (!Util.NearEqual(offset, 0f, 1e-2f))
			{
				new ExplainedNumber(offset, explainer, Explanation);
				__result += offset;
			}
		}
	}
}
