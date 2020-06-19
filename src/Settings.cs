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

		private const string TimeMultiplier_Hint = "Multiplies the rate at which campaign time passes. " +
			"Note that the same general pace is maintained: days simply pass more quickly/slowly. [ Default: 2 ]";

		private const string DaysPerSeason_Hint = "Alters the length of a season. Vanilla uses 21 days. [ Default: 7 ]";

		private const string EnablePregnancyTweaks_Hint = "Adjust the duration of pregnancies. [ Default: ON ]";

		private const string ScaledPregnancyDuration_Hint = "Scale pregnancy duration to this proportion of a " +
			"year. [ Default: 75% ]";

		private const string AdjustPregnancyDueDates_Hint = "Auto-adjust in-progress pregnancies' due dates to " +
			"match settings upon load of a game. [ Default: ON ]";

		[SettingPropertyFloatingInteger("Time Multiplier", 0.25f, 8f, HintText = TimeMultiplier_Hint, RequireRestart = false, Order = 0)]
		[SettingPropertyGroup("General Settings", GroupOrder = 0)]
		public float TimeMultiplier { get; set; } = 2f;

		[SettingPropertyInteger("Days Per Season", 1, 30, HintText = DaysPerSeason_Hint, RequireRestart = false, Order = 1)]
		[SettingPropertyGroup("General Settings")]
		public int DaysPerSeason { get; set; } = 7;

		[SettingPropertyBool("Pregnancy Duration", HintText = EnablePregnancyTweaks_Hint, RequireRestart = false, Order = 0)]
		[SettingPropertyGroup("Pregnancy Duration", GroupOrder = 1, IsMainToggle = true)]
		public bool EnablePregnancyTweaks { get; set; } = true;

		[SettingPropertyFloatingInteger("Year-Scaled Pregnancy Duration Factor", 0.2f, 4f, "#0%", HintText = ScaledPregnancyDuration_Hint, RequireRestart = false, Order = 1)]
		[SettingPropertyGroup("Pregnancy Duration")]
		public float ScaledPregnancyDuration { get; set; } = 0.75f;

		[SettingPropertyBool("Adjust In-Progress Pregnancy Due Dates", HintText = AdjustPregnancyDueDates_Hint, RequireRestart = false, Order = 2)]
		[SettingPropertyGroup("Pregnancy Duration")]
		public bool AdjustPregnancyDueDates { get; set; } = true;

		public List<string> ToStringLines(uint indentSize = 0)
		{
			string prefix = string.Empty;

			for (uint i = 0; i < indentSize; ++i)
				prefix += " ";

			return new List<string>
			{
				$"{prefix}{nameof(TimeMultiplier)}          = {TimeMultiplier}",
				$"{prefix}{nameof(DaysPerSeason)}           = {DaysPerSeason}",
				$"{prefix}{nameof(EnablePregnancyTweaks)}   = {EnablePregnancyTweaks}",
				$"{prefix}{nameof(ScaledPregnancyDuration)} = {ScaledPregnancyDuration}",
				$"{prefix}{nameof(AdjustPregnancyDueDates)} = {AdjustPregnancyDueDates}",
			};
		}
	}
}
