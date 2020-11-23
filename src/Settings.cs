using System.Collections.Generic;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace Pacemaker
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => $"{Main.Name}_v1";
        public override string DisplayName => Main.DisplayName;
        public override string FolderName => Main.Name;
        public override string FormatType => "json";

        private const string DaysPerSeason_Hint = "Alters the length of a season (and a year). Vanilla uses " +
            "21. NOTE: Once you start a game, this value is permanently set for that campaign. [ Default: 7 ]";

        private const string TimeMultiplier_Hint = "Multiplies the rate at which campaign time passes. " +
            "Note that the same general pace is maintained: days simply pass more quickly/slowly. [ Default: 1.75 ]";

        private const string AgeFactor_Hint = "Multiplies the number of effective \"human years\" characters age " +
            "in one calendar year. It's disabled at 100%, but at, e.g., 200%, all characters age twice as fast. " +
            "[ Default: 200% ]";

        private const string EnablePregnancyTweaks_Hint = "Adjust the duration of pregnancies. [ Default: ON ]";

        private const string ScaledPregnancyDuration_Hint = "Scale pregnancy duration to this proportion of a " +
            "year. [ Default: 75% ]";

        private const string AdjustPregnancyDueDates_Hint = "Auto-adjust in-progress pregnancies' due dates to " +
            "match settings upon load of a game. Still works correctly if another mod is overriding " +
            "this mod's pregnancy duration setting. [ Default: ON ]";

        private const string EnableHealingTweaks_Hint = "Auto-calibrate hero & troop healing rate to the Time " +
            "Multiplier in order to maintain vanilla pacing. [ Default: ON ]";

        private const string HealingRateAdjustmentFactor_Hint = "Additional factor to apply to healing rates " +
            "if the default auto-calibration isn't quite right for you. Higher than 100% causes faster healing; " +
            "lower will cause slower. [ Default: 100% ]";

        private const string EnableFoodTweaks_Hint = "Auto-calibrate party food consumption rate to the Time " +
            "Multiplier in order to maintain vanilla pacing. When the Time Multiplier is more than 1.0, as usual, " +
            "parties can otherwise run out of food too quickly. [ Default: ON ]";

        [SettingPropertyInteger("Days Per Season", 1, 90, HintText = DaysPerSeason_Hint, RequireRestart = false, Order = 0)]
        [SettingPropertyGroup("General Settings", GroupOrder = 0)]
        public int DaysPerSeason { get; set; } = 7;

        [SettingPropertyFloatingInteger("Time Multiplier", 0.25f, 4f, HintText = TimeMultiplier_Hint, RequireRestart = false, Order = 1)]
        [SettingPropertyGroup("General Settings")]
        public float TimeMultiplier { get; set; } = 1.75f;

        [SettingPropertyFloatingInteger("Accelerated Aging Factor", 1f, 15f, "#0%", HintText = AgeFactor_Hint, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("General Settings")]
        public float AgeFactor { get; set; } = 2f;

        [SettingPropertyBool("Pregnancy Duration", HintText = EnablePregnancyTweaks_Hint, RequireRestart = false, IsToggle = true, Order = 0)]
        [SettingPropertyGroup("Pregnancy Duration", GroupOrder = 1)]
        public bool EnablePregnancyTweaks { get; set; } = true;

        [SettingPropertyFloatingInteger("Year-Scaled Pregnancy Duration Factor", 0.2f, 4f, "#0%", HintText = ScaledPregnancyDuration_Hint, RequireRestart = false, Order = 1)]
        [SettingPropertyGroup("Pregnancy Duration")]
        public float ScaledPregnancyDuration { get; set; } = 0.75f;

        [SettingPropertyBool("Adjust In-Progress Pregnancy Due Dates", HintText = AdjustPregnancyDueDates_Hint, RequireRestart = false, Order = 2)]
        [SettingPropertyGroup("Pregnancy Duration")]
        public bool AdjustPregnancyDueDates { get; set; } = true;

        [SettingPropertyBool("Healing Rate Auto-Calibration", HintText = EnableHealingTweaks_Hint, RequireRestart = false, IsToggle = true, Order = 0)]
        [SettingPropertyGroup("Healing Rate Auto-Calibration", GroupOrder = 2)]
        public bool EnableHealingTweaks { get; set; } = true;

        [SettingPropertyFloatingInteger("Healing Rate Adjustment Factor", 0.25f, 4f, "#0%", HintText = HealingRateAdjustmentFactor_Hint, RequireRestart = false, Order = 1)]
        [SettingPropertyGroup("Healing Rate Auto-Calibration")]
        public float HealingRateFactor { get; set; } = 1f;

        [SettingPropertyBool("Food Consumption Auto-Calibration", HintText = EnableFoodTweaks_Hint, RequireRestart = false, IsToggle = true, Order = 0)]
        [SettingPropertyGroup("Food Consumption Auto-Calibration", GroupOrder = 3)]
        public bool EnableFoodTweaks { get; set; } = true;

        public List<string> ToStringLines(uint indentSize = 0)
        {
            string prefix = string.Empty;

            for (uint i = 0; i < indentSize; ++i)
                prefix += " ";

            return new List<string>
            {
                $"{prefix}{nameof(DaysPerSeason)}           = {DaysPerSeason}",
                $"{prefix}{nameof(TimeMultiplier)}          = {TimeMultiplier}",
                $"{prefix}{nameof(AgeFactor)}               = {AgeFactor}",
                $"{prefix}{nameof(EnablePregnancyTweaks)}   = {EnablePregnancyTweaks}",
                $"{prefix}{nameof(ScaledPregnancyDuration)} = {ScaledPregnancyDuration}",
                $"{prefix}{nameof(AdjustPregnancyDueDates)} = {AdjustPregnancyDueDates}",
                $"{prefix}{nameof(EnableHealingTweaks)}     = {EnableHealingTweaks}",
                $"{prefix}{nameof(HealingRateFactor)}       = {HealingRateFactor}",
                $"{prefix}{nameof(EnableFoodTweaks)}        = {EnableFoodTweaks}",
            };
        }
    }
}
