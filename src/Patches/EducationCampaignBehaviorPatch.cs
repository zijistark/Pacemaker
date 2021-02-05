using System.Runtime.CompilerServices;

using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
    internal sealed class EducationCampaignBehaviorPatch : Patch
    {
        public EducationCampaignBehaviorPatch() : base(Type.Prefix, _targetRM, _patchRM) { }

        private static readonly Reflect.Method<EducationCampaignBehavior> _targetRM = new("RegisterEvents");
        private static readonly Reflect.Method<EducationCampaignBehaviorPatch> _patchRM = new(nameof(RegisterEventsDisabled));

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool RegisterEventsDisabled() => false;
    }
}
