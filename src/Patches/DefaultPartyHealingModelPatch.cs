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
		private static readonly TextObject ConfigAdjustmentExplanation = new TextObject($"{Main.DisplayName}: Adjustment Factor");
		private static readonly TextObject TimeMultAdjustmentExplanation = new TextObject($"{Main.DisplayName}: Time Multiplier Calibration");

		private static class Helpers
		{
			internal static void GetDailyHealing(StatExplainer explanation, ref float __result)
			{
				// Our factors to apply to baseNum
				float timeMultFactor = 1f / Main.Settings.TimeMultiplier;
				float configFactor = Main.Settings.HealingRateFactor;

				// Time Multiplier
				float newHealFromTimeMult = __result * timeMultFactor;
				float offsetFromTimeMult = newHealFromTimeMult - __result;

				if (!Util.NearEqual(offsetFromTimeMult, 0f, 1e-2f))
				{
					new ExplainedNumber(offsetFromTimeMult, explanation, TimeMultAdjustmentExplanation);
					__result += offsetFromTimeMult;
				}

				// Healing Rate Adjustment Factor
				float newHealFromConfig = __result * configFactor;
				float offsetFromConfig = newHealFromConfig - __result;

				if (!Util.NearEqual(offsetFromConfig, 0f, 1e-2f))
				{
					new ExplainedNumber(offsetFromConfig, explanation, ConfigAdjustmentExplanation);
					__result += offsetFromConfig;
				}

				__result = (float)Math.Round(__result, 2);
			}
		}

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("GetDailyHealingForRegulars")]
		static void GetDailyHealingForRegularsPostfix(MobileParty party, StatExplainer explanation, ref float __result)
		{
			if (!Main.Settings.EnableHealingTweaks || __result <= 0f)
				return;

			Helpers.GetDailyHealing(explanation, ref __result);
		}

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("GetDailyHealingHpForHeroes")]
		static void GetDailyHealingHpForHeroesPostfix(MobileParty party, StatExplainer explanation, ref float __result)
		{
			if (!Main.Settings.EnableHealingTweaks || __result <= 0f)
				return;

			Helpers.GetDailyHealing(explanation, ref __result);
		}
	}
}
