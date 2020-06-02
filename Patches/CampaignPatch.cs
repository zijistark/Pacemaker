using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

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

		[HarmonyPrefix]
		[HarmonyPatch("OnLoad")]
		static void OnLoad(ref Campaign __instance, MetaData metaData)
		{
			var saveTime = SaveBehavior.Time;
			var trace = new List<string>
            {
                $"CampaignTime.Now:  {CampaignTime.Now}",
                $"SaveBehavior.Time: {saveTime}",
            };

			if (!saveTime.IsValid())
			{
				trace.Add("SaveBehavior.Time.IsValid() == FALSE");
				Util.EventTracer.Trace(trace);
				return;
			}
			else
				trace.Add("SaveBehavior.Time.IsValid() == TRUE");

			if (saveTime.IsNull())
			{
                trace.Add("SaveBehavior.Time.IsNull() == TRUE");

                // Load of save that didn't have CP enabled.
                // Convert "current" campaign time back to what it would have been under the vanilla calendar.

                var adjustedYears = CampaignTime.Now.ToYears / Main.TimeParam.TickRatioYear;
                MapTimeTrackerSetMI.Invoke(__instance, new object[]
                {
                    MapTimeTrackerCtorCI.Invoke(new object[] { CampaignTime.Years((float)adjustedYears) })
                });
            }
			else
            {
                trace.Add("SaveBehavior.Time.IsNull() == FALSE");

                // Normal load of prior CP-enabled savegame.
                // Restore saved calendar date to MapTimeTracker [with current calendar parameters].

                MapTimeTrackerSetMI.Invoke(__instance, new object[]
                {
                    MapTimeTrackerCtorCI.Invoke(new object[] { saveTime.CampaignTime })
                });
            }

			Util.EventTracer.Trace(trace);
		}
	}
}
