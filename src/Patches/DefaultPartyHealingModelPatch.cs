using System;

using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace Pacemaker.Patches
{
    [HarmonyPatch(typeof(DefaultPartyHealingModel))]
    internal static class DefaultPartyHealingModelPatch
    {
        /*
        public static Patch[] PatchSet { get; } = new Patch[]
        {
            new(Patch.Type.Postfix,
                new Reflect.Method<DefaultPartyHealingModel>("GetDailyHealingForRegulars"),
                new Reflect.Method<DefaultPartyHealingModelPatch>(nameof(GetDailyHealingForRegulars)),
                Priority.Last),
            new(Patch.Type.Postfix,
                new Reflect.Method<DefaultPartyHealingModel>("GetDailyHealingHpForHeroes"),
                new Reflect.Method<DefaultPartyHealingModelPatch>(nameof(GetDailyHealingHpForHeroes)),
                Priority.Last),
        };
        */

        private static class Helpers
        {
            private static readonly TextObject _timeMultAdjustmentExplanation = new($"[{Main.DisplayName}] Time Multiplier");
            private static readonly TextObject _configAdjustmentExplanation = new($"[{Main.DisplayName}] Adjustment Factor");

            /* Both of the daily healing rate postfix patches have exactly the same body, so
             * this does all the work for them:
             */
#if STABLE
            internal static void GetDailyHealing(StatExplainer explanation, ref float __result)
            {
                if (!Main.Settings!.EnableHealingTweaks || __result <= 0f)
                    return;

                // Our factors to apply to [hopefully] final values
                float configFactor = Main.Settings.HealingRateFactor;
                float timeMultFactor = 1f / (float)Math.Pow(Main.Settings.TimeMultiplier, 0.35);

                // Healing Rate Adjustment Factor
                float newHealFromConfig = __result * configFactor;
                float offsetFromConfig = newHealFromConfig - __result;

                if (!Util.NearEqual(offsetFromConfig, 0f, 1e-2f))
                {
                    new ExplainedNumber(offsetFromConfig, explanation, _configAdjustmentExplanation);
                    __result += offsetFromConfig;
                }

                // Time Multiplier
                float newHealFromTimeMult = __result * timeMultFactor;
                float offsetFromTimeMult = newHealFromTimeMult - __result;

                if (!Util.NearEqual(offsetFromTimeMult, 0f, 1e-2f))
                {
                    new ExplainedNumber(offsetFromTimeMult, explanation, _timeMultAdjustmentExplanation);
                    __result += offsetFromTimeMult;
                }

                __result = (float)Math.Round(__result, 2);
            }
#else
            internal static void GetDailyHealing(ref ExplainedNumber __result)
            {
                if (!Main.Settings!.EnableHealingTweaks || __result.ResultNumber <= 0f)
                    return;

                // Our factors to apply to [hopefully] final values
                float configFactor = Main.Settings.HealingRateFactor;
                float timeMultFactor = 1f / (float)Math.Pow(Main.Settings.TimeMultiplier, 0.35);

                // Healing Rate Adjustment Factor
                float newHealFromConfig = __result.ResultNumber * configFactor;
                float offsetFromConfig = newHealFromConfig - __result.ResultNumber;

                if (!Util.NearEqual(offsetFromConfig, 0f, 1e-2f))
                    __result.Add(offsetFromConfig, _configAdjustmentExplanation);

                // Time Multiplier
                float newHealFromTimeMult = __result.ResultNumber * timeMultFactor;
                float offsetFromTimeMult = newHealFromTimeMult - __result.ResultNumber;

                if (!Util.NearEqual(offsetFromTimeMult, 0f, 1e-2f))
                    __result.Add(offsetFromTimeMult, _timeMultAdjustmentExplanation);
            }
#endif
        }

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("GetDailyHealingForRegulars")]
#if STABLE
        private static void GetDailyHealingForRegulars(MobileParty party, StatExplainer explanation, ref float __result) =>
            Helpers.GetDailyHealing(explanation, ref __result);
#else
        private static void GetDailyHealingForRegulars(MobileParty party, bool includeDescriptions, ref ExplainedNumber __result)
        {
            _ = (party, includeDescriptions);
            Helpers.GetDailyHealing(ref __result);
        }
#endif

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("GetDailyHealingHpForHeroes")]
#if STABLE
        private static void GetDailyHealingHpForHeroes(MobileParty party, StatExplainer explanation, ref float __result) =>
            Helpers.GetDailyHealing(explanation, ref __result);
#else
        private static void GetDailyHealingHpForHeroes(MobileParty party, bool includeDescriptions, ref ExplainedNumber __result)
        {
            _ = (party, includeDescriptions);
            Helpers.GetDailyHealing(ref __result);
        }
#endif
    }
}
