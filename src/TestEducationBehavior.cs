using System;
using System.Drawing.Text;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Pacemaker
{
    internal sealed class TestEducationBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_canSetup", ref _canSetup);
        }

        /* OnGameEarlyLoaded is only present so that we can still initialize when adding the mod to a save
         * that didn't previously have it enabled (so-called "vanilla save"). This is because SyncData does
         * not even get called during game loading for behaviors that were not previously not part of the save.
         */
        private void OnDailyTick()
        {
            if (!_canSetup)
                return;

            _canSetup = false;

            static T GetObject<T>(string id) where T : MBObjectBase => MBObjectManager.Instance.GetObject<T>(id) as T
                ?? throw new Exception($"Failed to find '{id}' object!");

            var me = Hero.MainHero;
            var party = MobileParty.MainParty;

            var palatineGuard = GetObject<CharacterObject>("imperial_palatine_guard");
            var kingdom = GetObject<Kingdom>("empire");
            var mule = GetObject<ItemObject>("mule");
            var grain = GetObject<ItemObject>("grain");
            var vlandiaHorse = GetObject<ItemObject>("vlandia_horse");

            var wifeClan = kingdom.RulingClan;
            var home = wifeClan.Fortifications.OrderByDescending(t => t.IsTown && wifeClan.HomeSettlement?.Town == t ? 3 : t.IsTown ? 2 : 1).FirstOrDefault()
                ?? throw new Exception("Could not find a home!");

            ChangeOwnerOfSettlementAction.ApplyByDefault(me, home.Settlement);
            me.Clan.UpdateHomeSettlement(home.Settlement);
            me.Clan.AddRenown(1000);
            
            var wife = SpawnNoble(wifeClan, age: 32, female: true) ?? throw new Exception("Could not spawn wife!");

            if (kingdom.Ruler.IsFemale)
            {
                wife.Mother = kingdom.Ruler;
                wife.Father = kingdom.Ruler.Spouse ?? kingdom.Ruler.ExSpouses?.OrderByDescending(h => h.Age).FirstOrDefault();
            }
            else
            {
                wife.Father = kingdom.Ruler;
                wife.Mother = kingdom.Ruler.Spouse ?? kingdom.Ruler.ExSpouses?.OrderByDescending(h => h.Age).FirstOrDefault();
            }

            wife.BornSettlement = home.Settlement;
            wife.HasMet = true;
            wife.IsFertile = false;

            EnterSettlementAction.ApplyForCharacterOnly(wife, home.Settlement);
            //EnterSettlementAction.ApplyForParty(party, home.Settlement);

            MarriageAction.Apply(me, wife);
            Util.Log.NotifyGood($"Married {wife.Name} of {wife.Age:F1} years.");

            ChangeGovernorAction.Apply(home, wife);
            //LeaveSettlementAction.ApplyForParty(party);

            var kid = HeroCreator.DeliverOffSpring(wife, me, isOffspringFemale: true, age: 0);
            Util.Log.NotifyGood($"Spawned son {kid.Name} of {kid.Age:F1} years.");

            SetNobleSkills(me);
            me.BattleEquipment.FillFrom(palatineGuard.Equipment);
            GiveGoldAction.ApplyBetweenCharacters(null, me, 10_000_000);

            party.ItemRoster.AddToCounts(new EquipmentElement(mule), 5);
            party.ItemRoster.AddToCounts(new EquipmentElement(grain), 30);
            party.ItemRoster.AddToCounts(new EquipmentElement(vlandiaHorse), 34);
            party.AddElementToMemberRoster(palatineGuard, 34);
        }

        public static Hero? SpawnNoble(Clan clan, int age, bool female = false)
        {
            var templateSeq = Hero.All.Where(h =>
                h.IsNoble &&
                h.CharacterObject.Occupation == Occupation.Lord &&
                (female && h.IsFemale || !female && !h.IsFemale));

            var template = templateSeq.Where(h => h.Culture == clan.Culture).GetRandomElement() ?? templateSeq.GetRandomElement();

            if (template is null)
                return null;

            Util.Log.ToFile($"SpawnNoble: Using character template '{template.CharacterObject.StringId}'");
            Util.Log.ToFile($"BPMin: {template.CharacterObject.GetBodyPropertiesMin()}");
            Util.Log.ToFile($"BPMax: {template.CharacterObject.GetBodyPropertiesMax()}");

            var hero = HeroCreator.CreateSpecialHero(template.CharacterObject, faction: clan, age: age);

            SetNobleSkills(hero);
            hero.Name = hero.FirstName;
            hero.IsNoble = true;
            hero.ChangeState(Hero.CharacterStates.Active);
            CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
            return hero;
        }

        public static void SetNobleSkills(Hero hero)
        {
            // Attributes
            for (var attr = CharacterAttributesEnum.First; attr < CharacterAttributesEnum.End; ++attr)
                hero.SetAttributeValue(attr, MBRandom.RandomInt(6, 9));

            // Skills: levels & focus point minimums
            foreach (var skillObj in Game.Current.SkillList)
            {
                var curSkill = hero.GetSkillValue(skillObj);
                var curFocus = hero.HeroDeveloper.GetFocus(skillObj);

                int minSkill = MBRandom.RandomInt(125, 180);
                int minFocus = minSkill > 150 ? 5 : MBRandom.RandomInt(3, 4);

                if (curSkill < minSkill)
                    hero.HeroDeveloper.ChangeSkillLevel(skillObj, minSkill - curSkill, false);

                if (curFocus < minFocus)
                    hero.HeroDeveloper.AddFocus(skillObj, minFocus - curFocus, false);
            }
        }

        private bool _canSetup = true;
    }
}
