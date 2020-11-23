using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(HeroHelper))]
	internal static class HeroHelperPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("GetRandomBirthDayForAge")]
		private static bool GetRandomBirthDayForAge(float age, ref CampaignTime __result)
		{
			var now = CampaignTime.Now;
			float birthYear = now.GetYear - age;
			float randDayOfYear = MBRandom.RandomFloatRanged(1, Main.TimeParam.DayPerYear);

			if (randDayOfYear > now.GetDayOfYear)
				--birthYear;

			__result = CampaignTime.Years(birthYear) + CampaignTime.Days(randDayOfYear);
			return false;
		}
	}
}
