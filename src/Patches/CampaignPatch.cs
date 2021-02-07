using Pacemaker.Extensions;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Pacemaker.Patches
{
    [HarmonyPatch(typeof(Campaign))]
    internal class CampaignPatch
    {
        // PRE-CALCULATED REFLECTION
        //////////////////////////////////////////////////////////////////////////////////////////

        private static readonly MethodInfo MapTimeTrackerSetMI = AccessTools.PropertySetter(typeof(Campaign), "MapTimeTracker");
        private static readonly Type MapTimeTrackerT = typeof(Campaign).Assembly.GetType("TaleWorlds.CampaignSystem.MapTimeTracker");
        private static readonly ConstructorInfo MapTimeTrackerCtorCI = AccessTools.Constructor(MapTimeTrackerT, new[] { typeof(CampaignTime) });
        private static readonly MethodInfo CampaignStartTimeSetMI = AccessTools.PropertySetter(typeof(Campaign), "CampaignStartTime");
        private static readonly MethodInfo PlayerDefaultFactionGetMI = AccessTools.PropertyGetter(typeof(Campaign), "PlayerDefaultFaction");

        // HELPERS
        //////////////////////////////////////////////////////////////////////////////////////////

        class Helpers
        {
            internal static object CreateMapTimeTracker(CampaignTime campaignTime) =>
                MapTimeTrackerCtorCI.Invoke(new object[] { campaignTime });

            internal static void SetMapTimeTracker(Campaign campaign, CampaignTime campaignTime) =>
                MapTimeTrackerSetMI.Invoke(campaign, new object[] { CreateMapTimeTracker(campaignTime) });

            internal static void ResetCampaignStartTime(Campaign campaign) =>
                CampaignStartTimeSetMI.Invoke(campaign, new object[] { StandardStartTime });

            internal static CampaignTime StandardStartTime =>
                CampaignTimeExtensions.YearsD(1084) + CampaignTimeExtensions.SeasonsD(1) + CampaignTimeExtensions.HoursD(9);
        }

        // PATCHES
        //////////////////////////////////////////////////////////////////////////////////////////

        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, new[] { typeof(CampaignGameMode) })]
        internal static void CtorPostfix(Campaign __instance)
        {
            // Since vanilla uses 1084yr + 3wk + 9hr for the campaign start time and this implicitly
            // assumes that seasons are 3 weeks long (which they most certainly are not for us),
            // this patch cleans up the start time and current time after the main Campaign
            // constructor code runs. Ideally, we'd override the whole thing, because there are
            // a number of constructed elements which may depend upon time (or do so at some point)
            // that are escaping correction, but I'd rather not override the entire constructor.
            Helpers.ResetCampaignStartTime(__instance);
            Helpers.SetMapTimeTracker(__instance, __instance.CampaignStartTime);
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.VeryHigh)]
        [HarmonyPatch("OnLoad")]
        static void OnLoad(Campaign __instance, MetaData metaData)
        {
            var trace = new List<string> { "Attempting to ensure the game loads with the correct time parameters for this save...\n" };

            // Find the player character's name
            if (metaData.TryGetValue("CharacterName", out string charName))
                trace.Add($"Player character name: \"{charName}\"");
            else
            {
                trace.Add($"Entry 'CharacterName' not found in savegame MetaData! Aborting.");
                return;
            }

            // Find the player character's clan's name
            var clan = (Clan)PlayerDefaultFactionGetMI.Invoke(__instance, null);

            if (clan is null || clan.Name is null)
            {
                trace.Add($"Could not find player clan name! Aborting.");
                return;
            }
            else
                trace.Add($"Player clan name: \"{clan.Name}\"");

            // Load external saved values for this campaign (approximately identified by the combo of character name & clan name)
            var esv = Main.ExternalSavedValues.Get(charName, clan.Name.ToString());

            // Determine what DayPerSeason-derived time parameters we should be using
            int saveDps = TimeParams.OldDayPerSeason; // vanilla

            if (esv is null)
                trace.Add("Failed to find associated external saved values. Assuming vanilla savegame " +
                          "(but compromise of the external data store could also be the case).");
            else
            {
                trace.Add($"Externally saved values: {esv}\n");
                saveDps = esv.DaysPerSeason;
            }

            // Use the [projected] correct DayPerSeason-derived time parameters if we're not already
            if (saveDps != Main.TimeParam.DayPerSeason)
            {
                trace.Add($"DayPerSeason={Main.TimeParam.DayPerSeason} is incorrect for this campaign. Fixing...\n");
                Main.SetTimeParams(new TimeParams(saveDps), trace);
            }
            else
                trace.Add("Current time parameters already seem appropriate for this save.");

            Util.EventTracer.Trace(trace);
        }
    }
}
