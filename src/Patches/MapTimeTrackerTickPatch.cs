using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
	[HarmonyPatch]
	class MapTimeTrackerTickPatch
	{
		internal static readonly Type MapTimeTrackerT = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker").ReturnType;
		internal static readonly MethodInfo TargetMI = AccessTools.Method(MapTimeTrackerT, "Tick");

		static MethodBase TargetMethod()
		{
			return TargetMI;
		}

		static void Prefix(ref float seconds)
		{
			seconds *= Main.Settings.TimeMultiplier;
		}
	}
}
