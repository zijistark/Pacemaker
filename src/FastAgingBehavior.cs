using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Pacemaker
{
	class FastAgingBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
		}

		public override void SyncData(IDataStore dataStore) { }

		protected void OnSessionLaunched(CampaignGameStarter starter)
		{
			// Save these for later:
			adultAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
			teenAge = Campaign.Current.Models.AgeModel.BecomeTeenagerAge;
			childAge = Campaign.Current.Models.AgeModel.BecomeChildAge;

			// Days to reach each growth stage transition:
			daysFromAdult = adultAge * Main.TimeParam.DayPerYear;
			daysFromTeen = teenAge * Main.TimeParam.DayPerYear;
			daysFromChild = childAge * Main.TimeParam.DayPerYear;
		}

		protected void OnDailyTick()
		{
			if (Main.Settings.AgeFactor < 1.01f)
			{
				OnDailyTickWithoutAAF();
				return;
			}

			// Subtract 1 for the daily tick's implicitly-aged day & the rest is
			// explicit, incremental age to add.
			var birthDayDelta = CampaignTime.Days(Main.Settings.AgeFactor - 1f);

			// And this is just hoisted.
			var oneDay = CampaignTime.Days(1f);

			foreach (var hero in Hero.All.Where(h => h.IsAlive))
			{
				// When calculating the prevAge, we must take care to include the day
				// which the daily tick implicitly aged us since we last did this, or
				// else we could miss age transitions. Ergo, prevAge is the age we
				// were as if we were one day younger than our current BirthDay.
				int prevAge = (int)(hero.BirthDay + oneDay).ElapsedYearsUntilNow;

				// AAF at its core:
				hero.BirthDay -= birthDayDelta;
				hero.CharacterObject.Age = hero.Age;

				// And our new age, if different.
				int newAge = (int)hero.Age;

				// Did a relevant transition in age(s) occur?
				if (newAge > prevAge && !hero.IsTemplate)
				{
					// Loop over the aged years (extremely aggressive Days Per Season + AAF
					// could make it multiple), and thus we need to be able to handle the
					// possibility of multiple growth stage events needing to be fired.

					for (int age = prevAge + 1; age <= Math.Min(newAge, adultAge); ++age)
					{
						// These are not exclusive branches for a single age, because I simply
						// don't trust tweak mods to enforce at least a year apart from each stage
						// (but am trusting they're not out of order).

						if (age == childAge)
							SendHeroGrowsOutOfInfancyEvent(hero);

						if (age == teenAge)
							SendHeroReachesTeenAgeEvent(hero);

						if (age == adultAge)
							SendHeroComesOfAgeEvent(hero);
					}
				}
			}
		}

		protected void OnDailyTickWithoutAAF()
		{
			// Without AAF, we can just do an exact match between the current number of days elapsed
			// since Hero.BirthDay and the exact number of days required to reach each threshold.
			// This was, more or less, the vanilla approach.
			foreach (var hero in Hero.All.Where(h => h.IsAlive && !h.IsTemplate))
			{
				int daysFromBirth = (int)hero.BirthDay.ElapsedDaysUntilNow;

				if (daysFromBirth > daysFromAdult)
					continue;

				// Still don't trust these thresholds to never be equal (due to tweak mods), though,
				// so these branches are not exclusive.

				if (daysFromBirth == daysFromChild)
					SendHeroGrowsOutOfInfancyEvent(hero);

				if (daysFromBirth == daysFromTeen)
					SendHeroReachesTeenAgeEvent(hero);

				if (daysFromBirth == daysFromAdult)
					SendHeroComesOfAgeEvent(hero);
			}
		}

		protected void SendHeroGrowsOutOfInfancyEvent(Hero hero) =>
			OnHeroGrowsOutOfInfancyMI.Invoke(CampaignEventDispatcher.Instance, new object[] { hero });

		protected void SendHeroReachesTeenAgeEvent(Hero hero) =>
			OnHeroReachesTeenAgeMI.Invoke(CampaignEventDispatcher.Instance, new object[] { hero });

		protected void SendHeroComesOfAgeEvent(Hero hero)
		{
			if (!hero.IsActive) // This extra check is inherited from the old vanilla code.
				OnHeroComesOfAgeMI.Invoke(CampaignEventDispatcher.Instance, new object[] { hero });
		}

		// Year thresholds:
		private int adultAge;
		private int teenAge;
		private int childAge;

		// Day thresholds:
		private int daysFromAdult;
		private int daysFromTeen;
		private int daysFromChild;

		// Reflection to send these internal-access campaign events:
		private static readonly MethodInfo OnHeroComesOfAgeMI = AccessTools.Method(
			typeof(CampaignEventDispatcher), "OnHeroComesOfAge");

		private static readonly MethodInfo OnHeroReachesTeenAgeMI = AccessTools.Method(
			typeof(CampaignEventDispatcher), "OnHeroReachesTeenAge");

		private static readonly MethodInfo OnHeroGrowsOutOfInfancyMI = AccessTools.Method(
			typeof(CampaignEventDispatcher), "OnHeroGrowsOutOfInfancy");
	}
}
