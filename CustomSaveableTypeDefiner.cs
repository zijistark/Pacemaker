using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
    public class CustomSaveableTypeDefiner : SaveableTypeDefiner
    {
        public const int Ziji0_SaveBaseId = 1984111900; // zijistark's 0th allocated range is 1984111900 through 1984111999
        public const int SaveBaseId = Ziji0_SaveBaseId + 0;

        public CustomSaveableTypeDefiner() : base(SaveBaseId) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(SaveBehavior), 1);
            AddClassDefinition(typeof(SimpleTime), 2);
        }
    }
}
