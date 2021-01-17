using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace Pacemaker.Patches
{
    internal sealed class DefaultMobilePartyFoodConsumptionModelPatch : Patch
    {
        internal DefaultMobilePartyFoodConsumptionModelPatch()
            : base(Type.Postfix, TargetMethod, PatchMethod, Priority.Last) { }

        private static readonly Reflect.Method<DefaultMobilePartyFoodConsumptionModel> TargetMethod = new("CalculateDailyFoodConsumptionf");
        private static readonly Reflect.Method<DefaultMobilePartyFoodConsumptionModelPatch> PatchMethod = new(nameof(CalculateDailyFoodConsumptionf));
        private static readonly TextObject Explanation = new($"[{Main.DisplayName}] Time Multiplier");

#if STABLE
        private static void CalculateDailyFoodConsumptionf(MobileParty party, StatExplainer explainer, ref float __result)
        {
            _ = party;

            if (!Main.Settings!.EnableFoodTweaks)
                return;

            var offset = (__result / Main.Settings.TimeMultiplier) - __result;

            if (!Util.NearEqual(offset, 0f, 1e-2f))
            {
                new ExplainedNumber(offset, explainer, Explanation);
                __result += offset;
            }
        }
#else
        private static void CalculateDailyFoodConsumptionf(MobileParty party, bool includeDescription, ref ExplainedNumber __result)
        {
            _ = (party, includeDescription);

            if (!Main.Settings!.EnableFoodTweaks)
                return;

            var offset = (__result.ResultNumber / Main.Settings.TimeMultiplier) - __result.ResultNumber;

            if (!Util.NearEqual(offset, 0f, 1e-2f))
                __result.Add(offset, Explanation);
        }
#endif
    }
}
