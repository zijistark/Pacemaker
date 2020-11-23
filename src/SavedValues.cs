using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Pacemaker
{
    internal sealed class SavedValues
    {
        [SaveableProperty(1)]
        public int DaysPerSeason { get; set; }

        [SaveableProperty(3)]
        public float PregnancyDuration { get; set; }

        public SavedValues() { }

        internal void Snapshot()
        {
            if (DaysPerSeason == default) // Only set this upon first save
                DaysPerSeason = Main.TimeParam.DayPerSeason;

            PregnancyDuration = Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays;

            Main.ExternalSavedValues.Set(Hero.MainHero.Name.ToString(), Clan.PlayerClan.Name.ToString(), this);
        }

        public override string ToString()
        {
            StringBuilder builder = new("{\n");
            builder.AppendFormat("  {0} = {1}\n", nameof(DaysPerSeason), DaysPerSeason);
            builder.AppendFormat("  {0} = {1}\n", nameof(PregnancyDuration), PregnancyDuration);
            builder.Append("}");
            return builder.ToString();
        }
    }
}
