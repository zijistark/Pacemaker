using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Pacemaker
{
	class SavedValues
	{
		[SaveableProperty(1)]
		public int DaysPerSeason { get; set; }

		[SaveableProperty(2)]
		public float ScaledPregnancyDuration { get; set; }

		public SavedValues() { }

		internal void Snapshot()
		{
			if (DaysPerSeason == 0) // Only set this upon first save
				DaysPerSeason = Main.TimeParam.DayPerSeason;

			ScaledPregnancyDuration = Main.Settings.ScaledPregnancyDuration;

			Main.ExternalSavedValues.Set(
				Hero.MainHero.Name.ToString(),
				Clan.PlayerClan.Name.ToString(),
				this);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("{\n");
			builder.AppendFormat("  {0} = {1}\n", nameof(DaysPerSeason), DaysPerSeason);
			builder.AppendFormat("  {0} = {1}\n", nameof(ScaledPregnancyDuration), ScaledPregnancyDuration);
			builder.Append("}");
			return builder.ToString();
		}
	}
}
