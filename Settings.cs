using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace CampaignPacer
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => $"{Main.Name}_v1";
        public override string DisplayName => Main.DisplayName;
        public override string FolderName => Main.Name;

        /* basic time & calendar config */

        [SettingPropertyInteger("Ticks per Millisecond", 1, 30, Order = 0, RequireRestart = true, HintText = "Defines the number of game ticks that represent a millisecond. Increases campaign time's speed with values less than 10 (the vanilla value). The effective speed factor for N ticks is 10 ÷ N. [ Default: 5 (2x speed factor) ]")]
        [SettingPropertyGroup(groupName: "Campaign Time and Calendar", GroupOrder = 0)]
        public int TicksPerMillisecond { get; set; } = 5;

        [SettingPropertyInteger("Days per Week", 1, 14, Order = 1, RequireRestart = true, HintText = "Alters the length of a week. Vanilla uses 7 days. Note that weeks aren't actually shown in-game. [ Default: 3 ]")]
        [SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
        public int DaysPerWeek { get; set; } = 3;
 
        [SettingPropertyInteger("Weeks Per Season", 1, 12, Order = 2, RequireRestart = true, HintText = "Alters the length of a season. Vanilla uses 3 weeks. CAUTION: Values other than the default seem to offset the start date by a few days. [ Default: 3 ]")]
        [SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
        public int WeeksPerSeason { get; set; } = 3;
    }
}
