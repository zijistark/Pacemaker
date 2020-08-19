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
		private static readonly TextObject TimeMultAdjustmentExplanation = new TextObject($"[{Main.DisplayName}] Time Multiplier");
		private static readonly TextObject ConfigAdjustmentExplanation = new TextObject($"[{Main.DisplayName}] Adjustment Factor");

		private static class Helpers
		{
			/* Both of the daily healing rate postfix patches have exactly the same body, so
			 * this does all the work for them:
			 */
			internal static void GetDailyHealing(StatExplainer explanation, ref float __result)
			{
				if (!Main.Settings.EnableHealingTweaks || __result <= 0f)
					return;

				// Our factors to apply to [hopefully] final values
				float configFactor = Main.Settings.HealingRateFactor;
				float timeMultFactor = 1f / (float)Math.Pow(Main.Settings.TimeMultiplier, 0.35);

				// Healing Rate Adjustment Factor
				float newHealFromConfig = __result * configFactor;
				float offsetFromConfig = newHealFromConfig - __result;

				if (!Util.NearEqual(offsetFromConfig, 0f, 1e-2f))
				{
					new ExplainedNumber(offsetFromConfig, explanation, ConfigAdjustmentExplanation);
					__result += offsetFromConfig;
				}

				// Time Multiplier
				float newHealFromTimeMult = __result * timeMultFactor;
				float offsetFromTimeMult = newHealFromTimeMult - __result;

				if (!Util.NearEqual(offsetFromTimeMult, 0f, 1e-2f))
				{
					new ExplainedNumber(offsetFromTimeMult, explanation, TimeMultAdjustmentExplanation);
					__result += offsetFromTimeMult;
				}

				__result = (float)Math.Round(__result, 2);
			}
		}

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("GetDailyHealingForRegulars")]
		static void GetDailyHealingForRegulars(MobileParty party, StatExplainer explanation, ref float __result) =>
			Helpers.GetDailyHealing(explanation, ref __result);

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch("GetDailyHealingHpForHeroes")]
		static void GetDailyHealingHpForHeroes(MobileParty party, StatExplainer explanation, ref float __result) =>
			Helpers.GetDailyHealing(explanation, ref __result);
	}
}
