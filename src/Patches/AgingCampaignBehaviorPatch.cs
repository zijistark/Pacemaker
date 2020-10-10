using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(AgingCampaignBehavior))]
	class AgingCampaignBehaviorPatch
	{
		private static readonly MethodInfo UpdateHeroDeathProbabilitiesMI = AccessTools.Method(typeof(AgingCampaignBehavior), "UpdateHeroDeathProbabilities");
		private static readonly MethodInfo IsItTimeOfDeathMI = AccessTools.Method(typeof(AgingCampaignBehavior), "IsItTimeOfDeath");

		///////

		[HarmonyPrefix]
		[HarmonyPatch("WeeklyTick")]
		static bool WeeklyTick() => false; // not in use now, as the meager and yet now correct calculation has been moved to a DailyTick prefix

		//////

		[HarmonyPrefix]
		[HarmonyPatch("DailyTick")]
		static bool DailyTick(ref AgingCampaignBehavior __instance, ref int ____extraLives)
		{
			/* Update Hero Death Probabilities */

			int daysElapsed = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
			int updatePeriod = Util.NearEqual(Main.Settings.AgeFactor, 1f, 1e-2)
				? Main.TimeParam.DayPerYear
				: (int)(Main.TimeParam.DayPerYear / Main.Settings.AgeFactor);

			if (updatePeriod <= 0)
				updatePeriod = 1;

			// Globally update death probabilities every year of accumulated age
			if ((daysElapsed % updatePeriod) == 0)
				UpdateHeroDeathProbabilitiesMI.Invoke(__instance, null);

			/* Replace DailyTick implementation -- code is mostly as decompiled, minus
			   child growth stage stuff. */

			foreach (var hero in Hero.All)
			{
				if (hero.IsAlive && !hero.IsOccupiedByAnEvent())
				{
					if (hero.DeathMark != KillCharacterAction.KillCharacterActionDetail.None &&
						(hero.PartyBelongedTo == null ||
						(hero.PartyBelongedTo.MapEvent == null && hero.PartyBelongedTo.SiegeEvent == null)))
					{
						KillCharacterAction.ApplyByDeathMark(hero, false);
					}
					else
						IsItTimeOfDeathMI.Invoke(__instance, new object[] { hero });
				}

				// Mainly, we've removed the whole section on detecting transitions in child
				// growth stages and firing associated campaign events from here.
			}

			if (Hero.IsMainHeroIll && Hero.MainHero.HeroState != Hero.CharacterStates.Dead)
			{
				Campaign.Current.MainHeroIllDays++;

				if (Campaign.Current.MainHeroIllDays > 3)
				{
					Hero.MainHero.HitPoints -= (int)Math.Ceiling(
						Hero.MainHero.HitPoints * (0.05f * Campaign.Current.MainHeroIllDays));

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
