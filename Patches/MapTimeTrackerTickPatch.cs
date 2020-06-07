using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
	[HarmonyPatch]
	class MapTimeTrackerTickPatch
	{
		public static readonly MethodInfo TargetMI = AccessTools.Method(AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker").ReturnType, "Tick");

		static MethodBase TargetMethod()
		{
			return TargetMI;
		}

		static void Prefix(ref float seconds)
		{
			seconds *= Main.Config.TimeMultiplier;
		}

		static bool Prepare()
		{
			return Main.Config.UsesTimeMultiplier;
		}
	}
}
