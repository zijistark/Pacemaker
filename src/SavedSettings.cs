using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	public class SavedSettings
	{
		[SaveableProperty(1)]
		public float TimeMultiplier { get; set; }

		[SaveableProperty(2)]
		public int DaysPerSeason { get; set; }

		[SaveableProperty(3)]
		public float ScaledPregnancyDuration { get; set; }

		public SavedSettings()
		{
			TimeMultiplier = -1f;
			DaysPerSeason = -1;
			ScaledPregnancyDuration = -1f;
		}

		public SavedSettings(Settings settings)
		{
			TimeMultiplier = settings.TimeMultiplier;
			DaysPerSeason = settings.DaysPerSeason;
			ScaledPregnancyDuration = settings.ScaledPregnancyDuration;
		}

		public bool IsNull => TimeMultiplier < -0.99 && TimeMultiplier > -1.01 &&
			ScaledPregnancyDuration < -0.99 && ScaledPregnancyDuration > -1.01 && DaysPerSeason == -1;

		public bool IsValid => IsNull || (TimeMultiplier > 0.009f && ScaledPregnancyDuration > 0.009f && DaysPerSeason > 0);

		public override string ToString() => "{\n" +
			$"{Tab}{nameof(TimeMultiplier)} = {TimeMultiplier:F2}\n" +
			$"{Tab}{nameof(DaysPerSeason)} = {DaysPerSeason}\n" +
			$"{Tab}{nameof(ScaledPregnancyDuration)} = {ScaledPregnancyDuration:F2}\n" +
			"}";

		private const string Tab = "    ";
	}
}
