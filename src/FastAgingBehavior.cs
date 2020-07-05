using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Pacemaker
{
	class FastAgingBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents() => CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);

		public override void SyncData(IDataStore dataStore) { }

		/* Accelerated Aging Factor's implementation.
		 *
		 * If the setting is in use, simply shift the birthdate of every alive hero in this game by
		 * the correct amount of extra days to age for the single day's passage already implied by
		 * this tick.
		 */
		protected void OnDailyTick()
		{
			if (Main.Settings.AgeFactor < 1.01f)
				return;

			// Subtract 1 for the daily tick's implicitly-aged day & the rest is explicit [incremental] age to add.
			CampaignTime birthdayDelta = CampaignTime.Days(Main.Settings.AgeFactor - 1f);

			foreach (var hero in Hero.All.Where(h => h.IsAlive))
			{
				hero.BirthDay -= birthdayDelta;

				// But wait! There's more. We need to keep the hero's cosmetic age synchronized too.
				var dynBodyProperties = hero.DynamicBodyProperties;
				dynBodyProperties.Age = hero.BirthDay.ElapsedYearsUntilNow;
				hero.DynamicBodyProperties = dynBodyProperties;
			}
		}
	}
}
