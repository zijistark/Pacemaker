using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CampaignPacer.Patches
{
	[HarmonyPatch]
	class MapTimeTrackerTickPatch
	{
		public static readonly Type MapTimeTrackerT = AccessTools.PropertyGetter(typeof(Campaign), "MapTimeTracker").ReturnType;
		public static readonly FieldInfo TicksFI = AccessTools.Field(MapTimeTrackerT, "_numTicks");
		public static readonly FieldInfo DeltaTicksFI = AccessTools.Field(MapTimeTrackerT, "_deltaTimeInTicks");

		static MethodBase TargetMethod()
		{
			return AccessTools.Method(MapTimeTrackerT, "Tick");
		}

		static void Prefix(ref object __instance, float seconds)
		{
			var dTicks = (long)(seconds * Main.Config.TimeMultiplier * TimeParams.TickPerSecF);
			var nTicks = (long)TicksFI.GetValue(__instance);
			nTicks += dTicks;
			DeltaTicksFI.SetValue(__instance, dTicks);
			TicksFI.SetValue(__instance, nTicks);
		}
	}
}
