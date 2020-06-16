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
		[HarmonyPatch("GetDailyHealingForRegulars")]
		static void GetDailyHealingForRegulars(MobileParty party, StatExplainer explanation, ref float __result)
		{
			if (Util.NearEqual(Main.Settings.TimeMultiplier, 1f, 1e-2))
				return;

			float newHealing = __result / Main.Settings.TimeMultiplier;
			new ExplainedNumber(newHealing, explanation, AdjustmentExplanation);
			__result = (float)Math.Round(newHealing, 2);
		}

		[HarmonyPostfix]
		[HarmonyPatch("GetDailyHealingHpForHeroes")]
		static void GetDailyHealingHpForHeroes(MobileParty party, StatExplainer explanation, ref float __result)
		{
			if (Util.NearEqual(Main.Settings.TimeMultiplier, 1f, 1e-2))
				return;

			float newHP = __result / Main.Settings.TimeMultiplier;
			new ExplainedNumber(newHP, explanation, AdjustmentExplanation);
			__result = newHP;
		}
	}
}
