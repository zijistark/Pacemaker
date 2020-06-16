using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace Pacemaker.Patches
{
	[HarmonyPatch]
	class MapTimeTrackerTickPatch
	{
		private static readonly Type MapTimeTrackerT = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker").ReturnType;
		private static readonly MethodInfo TargetMI = AccessTools.Method(MapTimeTrackerT, "Tick");

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
