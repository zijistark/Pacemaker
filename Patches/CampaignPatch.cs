using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
	[HarmonyPatch(typeof(Campaign))]
	internal class CampaignPatch
	{
		internal static readonly MethodInfo MapTimeTrackerGetMI = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker");
		internal static readonly MethodInfo MapTimeTrackerSetMI = AccessTools.PropertySetter(typeof(Campaign), "MapTimeTracker");
		internal static readonly ConstructorInfo MapTimeTrackerCtorCI = AccessTools.Constructor(MapTimeTrackerGetMI.ReturnType, new[] { typeof(CampaignTime) });

		[HarmonyPostfix]
		[HarmonyPatch(MethodType.Constructor, new[] { typeof(CampaignGameMode) })]
		static void CtorPostfix(ref Campaign __instance, CampaignGameMode gameMode)
		{
			var startTime = CampaignTime.Years(1084f) + CampaignTime.Seasons(1f) + CampaignTime.Hours(9f);
			var mapTimeTracker = MapTimeTrackerCtorCI.Invoke(new object[] { startTime });
			MapTimeTrackerSetMI.Invoke(__instance, new object[] { mapTimeTracker });
			AccessTools.PropertySetter(typeof(Campaign), "CampaignStartTime").Invoke(__instance, new object[] { startTime });
		}
	}
}
