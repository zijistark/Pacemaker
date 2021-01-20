using HarmonyLib;

using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Pacemaker.Patches
{
    internal sealed class DefaultPregnancyModelPatch : Patch
    {
        internal DefaultPregnancyModelPatch()
            : base(Type.Prefix,
                  new Reflect.Getter<DefaultPregnancyModel>("PregnancyDurationInDays"),
                  new Reflect.Method<DefaultPregnancyModelPatch>(nameof(PregnancyDurationInDays)),
                  Priority.HigherThanNormal) { }

        private static bool PregnancyDurationInDays(ref float __result)
        {
            if (!Main.Settings!.EnablePregnancyTweaks)
                return true;

            __result = Main.Settings.ScaledPregnancyDuration * Main.TimeParam.DayPerYear;
            return false;
        }
    }
}
