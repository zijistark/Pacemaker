using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
    public class CustomSaveableTypeDefiner : SaveableTypeDefiner
    {
        public const int SaveBaseId_zijistark0 = 198_411_190; // zijistark's 0th allocated range is 1984111900 through 1984111999
        public const int SaveBaseId = SaveBaseId_zijistark0 + 0;

        public CustomSaveableTypeDefiner() : base(SaveBaseId) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(SimpleTime), 1);
        }
    }
}
