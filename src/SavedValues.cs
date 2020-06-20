using System.Text;
using TaleWorlds.SaveSystem;

namespace Pacemaker
{
	class SavedValues
	{
		[SaveableProperty(1)]
		internal int DaysPerSeason { get; set; } = 0;

		[SaveableProperty(2)]
		internal float ScaledPregnancyDuration { get; set; } = 0f;

		internal void Snapshot()
		{
			DaysPerSeason = Main.Settings.DaysPerSeason;
			ScaledPregnancyDuration = Main.Settings.ScaledPregnancyDuration;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("{\n");

			builder.AppendFormat("{0}{1} = {2}\n", Tab, nameof(DaysPerSeason), DaysPerSeason);
			builder.AppendFormat("{0}{1} = {2}\n", Tab, nameof(ScaledPregnancyDuration), ScaledPregnancyDuration);

			builder.Append("}");
			return builder.ToString();
		}

		private const string Tab = "    ";
	}
}
