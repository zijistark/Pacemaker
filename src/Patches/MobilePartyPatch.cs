using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Pacemaker.Patches
{
	[HarmonyPatch(typeof(MobileParty))]
	class MobilePartyPatch
	{
		/* Why this patch:
		 *
		 * MobileParty.HealHeroes, left to its own devices, will always heal heroes by
		 * at least 1 HP per hour, even though slower healing rates are totally
		 * reasonable. I assume that this is because the authors wanted HP to only
		 * be able to change by an integer amount (even though it's actually a float),
		 * and thus 1 is the smallest change you can allow.
		 *
		 * What this patch does:
		 *
		 * To allow for hero healing rates that are less than 1 HP per hour and avoid
		 * overriding the entire HealHeroes method, we now treat any amount of hourly
		 * HP to heal that's in (0,1) as the probability that the party's heroes will
		 * heal at all that hour. It'd be more swank if this was on an individual
		 * party member hero basis, but that would require overriding the entire
		 * method, and in reality, it doesn't add much.
		 */

		private const float MinHourlyChanceToHeal = 0.05f; // never lower than 5% chance to heal per hour

		[HarmonyPrefix]
		[HarmonyPatch("HealHeroes")]
		static bool HealHeroesPrefix(float heroHp)
		{
			if (heroHp >= 1f)
				return true;

			var healChance = (float)Math.Max(MinHourlyChanceToHeal, heroHp);

			if (MBRandom.RandomFloat < healChance)
				return true; // hooray, you get to heal

			return false; // boohoo, hope that wound isn't infected
		}
	}
}
