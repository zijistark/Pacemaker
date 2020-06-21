using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(DefaultPartyHealingModel))]
	class DefaultPartyHealingModelPatch
	{
		private static readonly TextObject AdjustmentExplanation = new TextObject(Main.DisplayName);

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("GetDailyHealingForRegulars")]
		static void GetDailyHealingForRegulars(MobileParty party, StatExplainer explanation, ref float __result)
		{
			if (!Main.Settings.EnableHealingTweaks)
				return;

			float factor = Main.Settings.HealingRateFactor / Main.Settings.TimeMultiplier;

			if (Util.NearEqual(factor, 1f, 1e-2f))
				return;

			float newHealing = __result * factor;
			new ExplainedNumber(newHealing - __result, explanation, AdjustmentExplanation);
			__result = (float)Math.Round(newHealing, 2);
		}

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("GetDailyHealingHpForHeroes")]
		static void GetDailyHealingHpForHeroes(MobileParty party, StatExplainer explanation, ref float __result)
		{
			if (!Main.Settings.EnableHealingTweaks)
				return;

			float factor = Main.Settings.HealingRateFactor / Main.Settings.TimeMultiplier;

			if (Util.NearEqual(factor, 1f, 1e-2f))
				return;

			float newHealing = __result * factor;
			new ExplainedNumber(newHealing - __result, explanation, AdjustmentExplanation);
			__result = (float)Math.Round(newHealing, 2);
		}
	}
}
