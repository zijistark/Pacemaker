using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(Campaign))]
	internal class CampaignPatch
	{
		// PRE-CALCULATED REFLECTION
		//////////////////////////////////////////////////////////////////////////////////////////

		// Note that MapTimeTracker is an `internal` type, so we use the return type of its PropertyGetter to get its ConstructorInfo
		private static readonly MethodInfo MapTimeTrackerGetMI = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker");
		private static readonly MethodInfo MapTimeTrackerSetMI = AccessTools.PropertySetter(typeof(Campaign), "MapTimeTracker");
		private static readonly Type MapTimeTrackerT = MapTimeTrackerGetMI.ReturnType;
		private static readonly ConstructorInfo MapTimeTrackerCtorCI = AccessTools.Constructor(MapTimeTrackerT, new[] { typeof(CampaignTime) });
		private static readonly MethodInfo CampaignStartTimeSetMI = AccessTools.PropertySetter(typeof(Campaign), "CampaignStartTime");

		// HELPERS
		//////////////////////////////////////////////////////////////////////////////////////////

		internal class Helpers
		{
			internal static void SetMapTimeTracker(Campaign campaign, CampaignTime campaignTime)
			{
				MapTimeTrackerSetMI.Invoke(campaign, new object[]
				{
					MapTimeTrackerCtorCI.Invoke(new object[] { campaignTime })
				});
			}

			internal static CampaignTime StandardStartTime => CampaignTime.Years(1084f) + CampaignTime.Seasons(1f) + CampaignTime.Hours(9f);

			internal static void ResetCampaignStartTime(Campaign campaign)
			{
				CampaignStartTimeSetMI.Invoke(campaign, new object[] { StandardStartTime });
			}
		}

		// PATCHES
		//////////////////////////////////////////////////////////////////////////////////////////

		[HarmonyPostfix]
		[HarmonyPatch(MethodType.Constructor, new[] { typeof(CampaignGameMode) })]
		internal static void CtorPostfix(ref Campaign __instance, CampaignGameMode gameMode)
		{
			Helpers.ResetCampaignStartTime(__instance);
			Helpers.SetMapTimeTracker(__instance, __instance.CampaignStartTime);
		}
	}
}
