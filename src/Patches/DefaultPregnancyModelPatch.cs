using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace CampaignPacer.Patches
{
	[HarmonyPriority(Priority.HigherThanNormal)]
	[HarmonyPatch(typeof(DefaultPregnancyModel), "PregnancyDurationInDays", MethodType.Getter)]
	class DefaultPregnancyModelPatch
	{
		static bool Prefix(ref float __result)
		{
			if (!Main.Settings.EnablePregnancyTweaks)
				return true;

			__result = Main.Settings.ScaledPregnancyDuration * Main.TimeParam.DayPerYearL;
			return false;
		}
	}
}
