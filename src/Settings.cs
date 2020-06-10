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

		[SettingPropertyFloatingInteger("Time Multiplier", 0.25f, 8f, Order = 0, RequireRestart = false, HintText = "Multiplies the rate at which campaign time passes. Note that the same general pace is maintained: days simply pass more quickly or more slowly. [ Default: 2 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public float TimeMultiplier { get; set; } = 2f;

		[SettingPropertyInteger("Days Per Season", 1, 30, Order = 1, RequireRestart = false, HintText = "Alters the length of a season. Vanilla uses 21 days. [ Default: 7 ]")]
		[SettingPropertyGroup(groupName: "Campaign Time and Calendar")]
		public int DaysPerSeason { get; set; } = 7;

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
