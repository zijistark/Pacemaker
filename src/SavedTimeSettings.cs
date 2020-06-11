using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	public class SavedTimeSettings
	{
		[SaveableProperty(1)]
		public float TimeMultiplier { get; set; }

		[SaveableProperty(2)]
		public int DaysPerSeason { get; set; }

		public SavedTimeSettings()
		{
			TimeMultiplier = -1f;
			DaysPerSeason = -1;
		}

		public SavedTimeSettings(Settings settings)
		{
			TimeMultiplier = settings.TimeMultiplier;
			DaysPerSeason = settings.DaysPerSeason;
		}

		public bool IsNull => TimeMultiplier < -0.99 && TimeMultiplier > -1.01 && DaysPerSeason == -1;
		public bool IsValid => IsNull || (TimeMultiplier >= 0.01f && DaysPerSeason > 0);

		public override string ToString() => $"{{TimeMultiplier = {TimeMultiplier}, DaysPerSeason = {DaysPerSeason}}}";
	}
}
