using System.Runtime.CompilerServices;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
    internal sealed class MapTimeTrackerTickPatch : Patch
    {
        private static readonly System.Type MapTimeTrackerT = typeof(Campaign).Assembly.GetType("TaleWorlds.CampaignSystem.MapTimeTracker");
        private static readonly Reflect.Method TargetRM = new(MapTimeTrackerT, "Tick");
        private static readonly Reflect.Method<MapTimeTrackerTickPatch> PatchRM = new(nameof(TickPrefix));

        internal MapTimeTrackerTickPatch() : base(Type.Prefix, TargetRM, PatchRM) { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void TickPrefix(ref float seconds) => seconds *= Main.Settings!.TimeMultiplier;
    }
}
