using HarmonyLib;

using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Pacemaker.Patches
{
	[HarmonyPriority(Priority.HigherThanNormal)]
	[HarmonyPatch(typeof(DefaultPregnancyModel), "PregnancyDurationInDays", MethodType.Getter)]
	internal static class DefaultPregnancyModelPatch
	{
		private static bool Prefix(ref float __result)
		{
			if (!Main.Settings!.EnablePregnancyTweaks)
				return true;

			__result = Main.Settings.ScaledPregnancyDuration * Main.TimeParam.DayPerYear;
			return false;
		}
	}
}
