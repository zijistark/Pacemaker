using HarmonyLib;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
    [HarmonyPatch(typeof(EducationCampaignBehavior))]
    internal static class EducationCampaignBehaviorPatch
    {
        // FIXME: remove in e1.5.5 version, as it's already done for us.
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch("RegisterEvents")]
        private static bool RegisterEvents() => false;
    }
}
