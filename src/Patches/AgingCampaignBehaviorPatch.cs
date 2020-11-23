using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(AgingCampaignBehavior))]
	internal static class AgingCampaignBehaviorPatch
	{
		private delegate void IsItTimeOfDeathDelegate(AgingCampaignBehavior instance, Hero hero);
		private static readonly Reflect.DeclaredMethod<AgingCampaignBehavior> IsItTimeOfDeathRM = new("IsItTimeOfDeath");
		private static readonly IsItTimeOfDeathDelegate IsItTimeOfDeath = IsItTimeOfDeathRM.GetOpenDelegate<IsItTimeOfDeathDelegate>();

		[HarmonyPrefix]
		[HarmonyPatch("WeeklyTick")]
		private static bool WeeklyTick() => false; // Disabled, as the death probability calculation is now triggered by FastAgingBehavior.OnDailyTick()

		[HarmonyPrefix]
		[HarmonyPatch("DailyTick")]
		private static bool DailyTick(AgingCampaignBehavior __instance, ref int ____extraLives)
		{
			/* Replace DailyTick implementation -- code is mostly as decompiled, minus
			   child growth stage stuff. */

			foreach (var hero in Hero.All.ToList())
			{
				if (hero.IsAlive && !hero.IsOccupiedByAnEvent())
				{
					if (hero.DeathMark != KillCharacterAction.KillCharacterActionDetail.None
						&& (hero.PartyBelongedTo is null || hero.PartyBelongedTo.MapEvent is null && hero.PartyBelongedTo.SiegeEvent is null))
						KillCharacterAction.ApplyByDeathMark(hero, false);
					else
						IsItTimeOfDeath(__instance, hero);
				}

				// Mainly, we've removed the whole section on detecting transitions in childhood
				// growth stages and firing associated campaign events from here. The improved logic
				// is now in FastAgingBehavior.OnDailyTick(), which also fires the events.
			}

			if (Hero.IsMainHeroIll && Hero.MainHero.HeroState != Hero.CharacterStates.Dead)
			{
				Campaign.Current.MainHeroIllDays++;

				if (Campaign.Current.MainHeroIllDays > 3)
				{
					Hero.MainHero.HitPoints -= (int)Math.Ceiling(Hero.MainHero.HitPoints * (0.05f * Campaign.Current.MainHeroIllDays));

					if (Hero.MainHero.HitPoints <= 1)
					{
						if (____extraLives == 0)
						{
							Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
							KillCharacterAction.ApplyByOldAge(Hero.MainHero, true);
							return false; // Return now & skip entire original method
						}

						Campaign.Current.MainHeroIllDays = -1;
						____extraLives--;
					}
				}
			}

			return false; // Skip entire original method
		}
	}
}
