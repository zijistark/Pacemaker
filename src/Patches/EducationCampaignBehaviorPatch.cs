using HarmonyLib;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
    [HarmonyPatch(typeof(EducationCampaignBehavior))]
    internal static class EducationCampaignBehaviorPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch("RegisterEvents")]
        private static bool RegisterEvents() => false;
    }
}
