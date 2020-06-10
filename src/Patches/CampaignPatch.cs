using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
	[HarmonyPatch(typeof(Campaign))]
	internal class CampaignPatch
	{
		// PRE-CALCULATED REFLECTION
		//////////////////////////////////////////////////////////////////////////////////////////

		// Note that MapTimeTracker is an `internal` type, so we use the return type of its PropertyGetter to get its ConstructorInfo
		internal static readonly MethodInfo MapTimeTrackerGetMI = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker");
		internal static readonly MethodInfo MapTimeTrackerSetMI = AccessTools.PropertySetter(typeof(Campaign), "MapTimeTracker");
		internal static readonly Type MapTimeTrackerT = MapTimeTrackerGetMI.ReturnType;
		internal static readonly ConstructorInfo MapTimeTrackerCtorCI = AccessTools.Constructor(MapTimeTrackerT, new[] { typeof(CampaignTime) });
		internal static readonly MethodInfo CampaignStartTimeSetMI = AccessTools.PropertySetter(typeof(Campaign), "CampaignStartTime");

		// HELPERS
		//////////////////////////////////////////////////////////////////////////////////////////

		internal static void SetMapTimeTracker(Campaign campaign, CampaignTime campaignTime)
		{
			MapTimeTrackerSetMI.Invoke(campaign, new object[]
			{
				MapTimeTrackerCtorCI.Invoke(new object[] { campaignTime })
			});
		}

		internal static CampaignTime StandardCampaignStartTime => CampaignTime.Years(1084f) + CampaignTime.Seasons(1f) + CampaignTime.Hours(9f);

		internal static void ResetCampaignStartTime(Campaign campaign)
		{
			CampaignStartTimeSetMI.Invoke(campaign, new object[] { StandardCampaignStartTime });
		}

		// PATCHES
		//////////////////////////////////////////////////////////////////////////////////////////

		[HarmonyPostfix]
		[HarmonyPatch(MethodType.Constructor, new[] { typeof(CampaignGameMode) })]
		static void CtorPostfix(ref Campaign __instance, CampaignGameMode gameMode)
		{
			ResetCampaignStartTime(__instance);
			SetMapTimeTracker(__instance, __instance.CampaignStartTime);
		}
	}
}
