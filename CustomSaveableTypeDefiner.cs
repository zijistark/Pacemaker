using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
    public class CustomSaveableTypeDefiner : SaveableTypeDefiner
    {
        public const int SaveBaseId_zijistark0 = 198_411_190; // zijistark's 0th allocated range is 198411190 through 198411199
        public const int SaveBaseId = SaveBaseId_zijistark0 + 0;

        public CustomSaveableTypeDefiner() : base(SaveBaseId) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(SimpleTime), 1);
        }
    }
}
