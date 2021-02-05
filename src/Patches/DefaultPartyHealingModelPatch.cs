using System;
using System.Runtime.CompilerServices;

using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace Pacemaker.Patches
{
    [HarmonyPatch(typeof(DefaultPartyHealingModel))]
    internal static class DefaultPartyHealingModelPatch
    {
        private static class Helpers
        {
            private static readonly TextObject _timeMultAdjustmentExplanation = new($"[{Main.DisplayName}] Time Multiplier");
            private static readonly TextObject _configAdjustmentExplanation = new($"[{Main.DisplayName}] Adjustment Factor");

            /* Both of the daily healing rate postfix patches have exactly the same body, so
             * this does all the work for them:
             */
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
        }

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("GetDailyHealingForRegulars")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void GetDailyHealingForRegulars(ref ExplainedNumber __result) => Helpers.GetDailyHealing(ref __result);

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("GetDailyHealingHpForHeroes")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void GetDailyHealingHpForHeroes(ref ExplainedNumber __result) => Helpers.GetDailyHealing(ref __result);
    }
}
