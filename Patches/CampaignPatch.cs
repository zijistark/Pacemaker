using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
	[HarmonyPatch(typeof(Campaign))]
	class CampaignPatch
	{
        public static readonly MethodInfo MapTimeTrackerGetMI = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker");
        public static readonly MethodInfo MapTimeTrackerSetMI = AccessTools.PropertySetter(typeof(Campaign), "MapTimeTracker");
        public static readonly MethodInfo SetCampaignStartTimeMI = AccessTools.PropertySetter(typeof(Campaign), "CampaignStartTime");
        public static readonly Type MapTimeTrackerT = MapTimeTrackerGetMI.ReturnType;
        public static readonly ConstructorInfo MapTimeTrackerCtorCI = AccessTools.Constructor(MapTimeTrackerT, new[] { typeof(CampaignTime) });

        [HarmonyPostfix]
		[HarmonyPatch(MethodType.Constructor, new[] { typeof(CampaignGameMode) })]
		static void CtorPostfix(ref Campaign __instance, CampaignGameMode gameMode)
		{
            var startTime = CampaignTime.Years(1084f) + CampaignTime.Seasons(1f) + CampaignTime.Hours(9f);
            var mapTimeTracker = MapTimeTrackerCtorCI.Invoke(new object[] { startTime });
            MapTimeTrackerSetMI.Invoke(__instance, new object[] { mapTimeTracker });
            SetCampaignStartTimeMI.Invoke(__instance, new object[] { startTime });
		}
	}
}
