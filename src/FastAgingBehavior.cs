using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace Pacemaker
{
    internal sealed class FastAgingBehavior : CampaignBehaviorBase
    {
        public FastAgingBehavior()
        {
            OnHeroComesOfAge = OnHeroComesOfAgeRM.GetDelegate<OnHeroComesOfAgeDelegate>(CampaignEventDispatcher.Instance);
            OnHeroReachesTeenAge = OnHeroReachesTeenAgeRM.GetDelegate<OnHeroReachesTeenAgeDelegate>(CampaignEventDispatcher.Instance);
            OnHeroGrowsOutOfInfancy = OnHeroGrowsOutOfInfancyRM.GetDelegate<OnHeroGrowsOutOfInfancyDelegate>(CampaignEventDispatcher.Instance);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore) { }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            var educationBehavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<EducationCampaignBehavior>();
            DoEducation = DoEducationRM.GetDelegate<DoEducationDelegate>(educationBehavior);

            var agingBehavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<AgingCampaignBehavior>();
            UpdateHeroDeathProbabilities = UpdateHeroDeathProbabilitiesRM.GetDelegate<UpdateHeroDeathProbabilitiesDelegate>(agingBehavior);

            // Save these for later:
            adultAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            teenAge = Campaign.Current.Models.AgeModel.BecomeTeenagerAge;
            childAge = Campaign.Current.Models.AgeModel.BecomeChildAge;
        }

        private void OnDailyTick()
        {
            bool aafEnabled = !Util.NearEqual(Main.Settings!.AgeFactor, 1f, 1e-2);

            /* Update Hero Death Probabilities */

            int daysElapsed = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
            int updatePeriod = !aafEnabled ? Main.TimeParam.DayPerYear : (int)(Main.TimeParam.DayPerYear / Main.Settings.AgeFactor);

            if (updatePeriod <= 0)
                updatePeriod = 1;

            // Globally update death probabilities every year of accumulated age
            if (daysElapsed % updatePeriod == 0)
                UpdateHeroDeathProbabilities!();

            /* Send childhood growth stage transition events & perform AAF if enabled */

            // Subtract 1 for the daily tick's implicitly-aged day & the rest is
            // explicit, incremental age to add.
            var birthDayDelta = CampaignTime.Days(Main.Settings.AgeFactor - 1f);

            // And this is just hoisted.
            var oneDay = CampaignTime.Days(1f);

            foreach (var hero in Hero.All)
            {
                if (hero.IsDead)
                    continue;

                // When calculating the prevAge, we must take care to include the day
                // which the daily tick implicitly aged us since we last did this, or
                // else we could miss age transitions. Ergo, prevAge is the age we
                // were as if we were one day younger than our current BirthDay.
                int prevAge = (int)(hero.BirthDay + oneDay).ElapsedYearsUntilNow;

                if (aafEnabled)
                {
                    hero.BirthDay -= birthDayDelta;
                    hero.CharacterObject.Age = hero.Age;
                }

                // And our new age, if different.
                int newAge = (int)hero.Age;

                // Did a relevant transition in age(s) occur?
                if (newAge > prevAge && prevAge < adultAge && !hero.IsTemplate)
                {
                    // Loop over the aged years (extremely aggressive Days Per Season + AAF
                    // could make it multiple), and thus we need to be able to handle the
                    // possibility of multiple growth stage events needing to be fired.

                    for (int age = prevAge + 1; age <= Math.Min(newAge, adultAge); ++age)
                    {
                        // Replacement for EducationCampaignBehavior.OnDailyTick()
                        if (hero.Clan == Clan.PlayerClan && GetChildAgeState(age) != ChildAgeState.Invalid)
                        {
                            DoEducation!(hero);

                            // WTF is this doing after the DoEducation call? Magic, dnSpy fucking up, or TaleWorlds fucking up?
                            new TextObject("{=Z5qYQV08}Your kin has reached the age of {CHILD.AGE} and needs your guidance on {?CHILD.GENDER}her{?}his{\\?} development.", null)
                                .SetCharacterProperties("CHILD", hero.CharacterObject, null, false);
                        }

                        // This replaces AgingCampaignBehavior.OnDailyTick's campaign event emission:

                        // These are not exclusive branches for a single age, because I simply
                        // don't trust tweak mods to enforce at least a year apart from each stage
                        // (but am trusting they're not out of order).

                        if (age == childAge)
                            OnHeroGrowsOutOfInfancy(hero);

                        if (age == teenAge)
                            OnHeroReachesTeenAge(hero);

                        if (age == adultAge && !hero.IsActive)
                            OnHeroComesOfAge(hero);
                    }
                }
            }
        }
        private ChildAgeState GetChildAgeState(int age)
        {
            if (age <= 8)
            {
                if (age == 2)
                    return ChildAgeState.Year2;
                if (age == 5)
                    return ChildAgeState.Year5;
                if (age == 8)
                    return ChildAgeState.Year8;
            }
            else
            {
                if (age == 12)
                    return ChildAgeState.Year12;
                if (age == 15)
                    return ChildAgeState.Year15;
                if (age == 17)
                    return ChildAgeState.Year17;
            }
            return ChildAgeState.Invalid;
        }

        private enum ChildAgeState : short
        {
            Invalid = -1,
            Year2,
            Year5,
            Year8,
            Year12,
            Year15,
            Year17,
            Count
        }

        // Year thresholds (cached):
        private int adultAge;
        private int teenAge;
        private int childAge;

        private delegate void DoEducationDelegate(Hero child);
        private delegate void UpdateHeroDeathProbabilitiesDelegate();
        private delegate void OnHeroComesOfAgeDelegate(Hero hero);
        private delegate void OnHeroReachesTeenAgeDelegate(Hero hero);
        private delegate void OnHeroGrowsOutOfInfancyDelegate(Hero hero);

        private DoEducationDelegate? DoEducation;
        private UpdateHeroDeathProbabilitiesDelegate? UpdateHeroDeathProbabilities;
        private readonly OnHeroComesOfAgeDelegate OnHeroComesOfAge;
        private readonly OnHeroReachesTeenAgeDelegate OnHeroReachesTeenAge;
        private readonly OnHeroGrowsOutOfInfancyDelegate OnHeroGrowsOutOfInfancy;

        // Reflection to send these internal-access campaign events:
        private static readonly Reflect.DeclaredMethod<EducationCampaignBehavior> DoEducationRM = new("DoEducation");
        private static readonly Reflect.DeclaredMethod<AgingCampaignBehavior> UpdateHeroDeathProbabilitiesRM = new("UpdateHeroDeathProbabilities");
        private static readonly Reflect.DeclaredMethod<CampaignEventDispatcher> OnHeroComesOfAgeRM = new("OnHeroComesOfAge");
        private static readonly Reflect.DeclaredMethod<CampaignEventDispatcher> OnHeroReachesTeenAgeRM = new("OnHeroReachesTeenAge");
        private static readonly Reflect.DeclaredMethod<CampaignEventDispatcher> OnHeroGrowsOutOfInfancyRM = new("OnHeroGrowsOutOfInfancy");
    }
}
