﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Pacemaker.Patches
{
    [HarmonyPatch(typeof(AgingCampaignBehavior))]
    internal static class AgingCampaignBehaviorPatch
    {
        internal static class ForOptimizer
        {
            internal static void WeeklyTick() => AgingCampaignBehaviorPatch.WeeklyTick();
            internal static void DailyTickHero() => AgingCampaignBehaviorPatch.DailyTickHero(null!, null!, null!, null!);
        }

        private delegate void IsItTimeOfDeathDelegate(AgingCampaignBehavior instance, Hero hero);
        private static readonly Reflect.Method<AgingCampaignBehavior> IsItTimeOfDeathRM = new("IsItTimeOfDeath");
        private static readonly IsItTimeOfDeathDelegate IsItTimeOfDeath = IsItTimeOfDeathRM.GetOpenDelegate<IsItTimeOfDeathDelegate>();

        [HarmonyPrefix]
        [HarmonyPatch("WeeklyTick")]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool WeeklyTick() => false; // Disabled, as its work is now triggered by FastAgingBehavior.OnDailyTick()

        [HarmonyPrefix]
        [HarmonyPriority(Priority.High)]
        [HarmonyPatch("DailyTickHero")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool DailyTickHero(Hero hero,
                                          AgingCampaignBehavior __instance,
                                          Dictionary<Hero, int> ____extraLivesContainer,
                                          Dictionary<Hero, int> ____heroesYoungerThanHeroComesOfAge)
        {
            /* Replace DailyTick implementation -- code is mostly as decompiled, minus
               child growth stage stuff. */

            if (CampaignOptions.IsLifeDeathCycleDisabled)
                return false;

            if (hero.IsAlive && !hero.IsHeroOccupied(Hero.EventRestrictionFlags.CantDie))
            {
                if (hero.DeathMark != KillCharacterAction.KillCharacterActionDetail.None
                    && (hero.PartyBelongedTo is null
                        || (hero.PartyBelongedTo.MapEvent is null && hero.PartyBelongedTo.SiegeEvent is null)))
                {
                    KillCharacterAction.ApplyByDeathMark(hero, false);
                }
                else
                    IsItTimeOfDeath(__instance, hero);
            }

            // Mainly, we've removed the whole section on detecting transitions in childhood
            // growth stages and firing associated campaign events from here. The improved logic
            // is now in FastAgingBehavior.OnDailyTick(), which also fires the events.

            int age = (int)hero.Age;

            if (____heroesYoungerThanHeroComesOfAge.TryGetValue(hero, out var storedAge) && storedAge != age)
            {
                if (age >= Campaign.Current.Models.AgeModel.HeroComesOfAge)
                    ____heroesYoungerThanHeroComesOfAge.Remove(hero);
                else
                    ____heroesYoungerThanHeroComesOfAge[hero] = age;
            }

            if (hero == Hero.MainHero && Hero.IsMainHeroIll && Hero.MainHero.HeroState != Hero.CharacterStates.Dead)
            {
                Campaign.Current.MainHeroIllDays++;

                if (Campaign.Current.MainHeroIllDays > 3)
                {
                    Hero.MainHero.HitPoints -= (int)Math.Ceiling(Hero.MainHero.HitPoints * (0.05f * Campaign.Current.MainHeroIllDays));

                    if (Hero.MainHero.HitPoints <= 1 && Hero.MainHero.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
                    {
                        if (____extraLivesContainer.TryGetValue(Hero.MainHero, out int extraLives) && extraLives > 0)
                        {
                            Campaign.Current.MainHeroIllDays = -1;
                            --extraLives;

                            if (extraLives == 0)
                                ____extraLivesContainer.Remove(Hero.MainHero);
                            else
                                ____extraLivesContainer[Hero.MainHero] = extraLives;

                            return false;
                        }

                        Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                        KillCharacterAction.ApplyByOldAge(Hero.MainHero, true);
                    }
                }
            }

            return false;
        }
    }
}
