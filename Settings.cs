using System.Collections.Generic;
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

		[SettingPropertyFloatingInteger("Time Multiplier", 0.25f, 8f, Order = 0, RequireRestart = false, HintText = "Multiplies the rate at which campaign time passes. Note that the same general pace is maintained: days simply pass more quickly or more slowly. [ Default: 2 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public float TimeMultiplier { get; set; } = 2f;

		[SettingPropertyInteger("Days Per Season", 1, 30, Order = 1, RequireRestart = false, HintText = "Alters the length of a season. Vanilla uses 21 days. [ Default: 7 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public int DaysPerSeason { get; set; } = 7;

		//[SettingPropertyInteger("Weeks Per Season", 1, 12, Order = 2, RequireRestart = true, HintText = "Alters the length of a season. Vanilla uses 3 weeks. [ Default: 3 ]")]
		//[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		//public int WeeksPerSeason { get; set; } = 3;

		public List<string> ToStringLines(uint indentSize = 0)
		{
			string prefix = string.Empty;

			for (uint i = 0; i < indentSize; ++i)
				prefix += " ";

			return new List<string>
			{
				$"{prefix}{nameof(TimeMultiplier)} = {TimeMultiplier}",
				$"{prefix}{nameof(DaysPerSeason)}  = {DaysPerSeason}",
			};
		}
	}
}
