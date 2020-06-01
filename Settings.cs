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
		public override string Format => "json";

		/* basic time & calendar config */

		// [SettingPropertyInteger("Ticks per Millisecond", 1, 30, Order = 0, RequireRestart = true, HintText = "Defines the number of game ticks that represent a millisecond. Increases campaign time's speed with values less than 10 (the vanilla value). The effective speed factor for N ticks is 10 ÷ N. [ Default: 5 (2x speed factor) ]")]
		// [SettingPropertyGroup(groupName: "Campaign Time and Calendar", GroupOrder = 0)]
		// public int TicksPerMillisecond { get; set; } = 5;

		[SettingPropertyFloatingInteger("Time Multiplier", 0.2f, 8f, Order = 0, RequireRestart = true, HintText = "Multiplies the rate at which campaign time passes by the given factor. [ Default: 2.0 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public float TimeMultiplier { get; set; } = 2f;

		[SettingPropertyInteger("Days Per Week", 1, 14, Order = 1, RequireRestart = true, HintText = "Alters the length of a week. Vanilla uses 7 days. Note that weeks aren't actually shown in-game. [ Default: 3 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public int DaysPerWeek { get; set; } = 3;

		[SettingPropertyInteger("Weeks Per Season", 1, 12, Order = 2, RequireRestart = true, HintText = "Alters the length of a season. Vanilla uses 3 weeks. [ Default: 3 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public int WeeksPerSeason { get; set; } = 3;

		public List<string> ToStringLines(string indent = null)
		{
			var prefix = (indent != null) ? indent : String.Empty;

			return new List<string>
			{
				$"{prefix}{nameof(TimeMultiplier)} = {TimeMultiplier}",
				$"{prefix}{nameof(DaysPerWeek)}    = {DaysPerWeek}",
				$"{prefix}{nameof(WeeksPerSeason)} = {WeeksPerSeason}",
			};
		}
	}
}
