using System;
using System.Reflection;

using HarmonyLib;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
    [HarmonyPatch]
    internal sealed class MapTimeTrackerTickPatch
    {
        private static readonly Type MapTimeTrackerT = typeof(Campaign).Assembly.GetType("TaleWorlds.CampaignSystem.MapTimeTracker");
        private static readonly MethodInfo TargetMI = AccessTools.DeclaredMethod(MapTimeTrackerT, "Tick");

        private static MethodBase TargetMethod() => TargetMI;

        private static void Prefix(ref float seconds) => seconds *= Main.Settings!.TimeMultiplier;
    }
}
