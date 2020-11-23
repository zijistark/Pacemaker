using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Pacemaker
{
	internal sealed class SimpleTime
	{
		[SaveableProperty(1)]
		public int Year { get; set; } = 0;

		[SaveableProperty(2)]
		public int Season { get; set; } = 0;

		[SaveableProperty(3)]
		public int Day { get; set; } = 0;

		[SaveableProperty(4)]
		public double FractionalDay { get; set; } = 0;

		public SimpleTime(CampaignTime ct)
		{
			double fracDays = ct.ToDays;

			Year = ct.GetYear;
			fracDays -= Year * Main.TimeParam.DayPerYear;

			Season = ct.GetSeasonOfYear;
			fracDays -= Season * Main.TimeParam.DayPerSeason;

			Day = ct.GetDayOfSeason;
			fracDays -= Day;

			FractionalDay = Math.Min(0.999999, Math.Max(+0.0, fracDays)); // clamp to [+0, 0.999999]
		}

		private bool IsSeasonValid => Season >= 0 && Season < TimeParams.SeasonPerYear;

		public override string ToString()
		{
			// only intended for debugging
			var ct = CampaignTimeExt.DaysD(FractionalDay);
			var hour = (int)ct.ToHours;
			var min = (int)ct.ToMinutes % TimeParams.MinPerHour;
			var sec = (int)ct.ToSeconds % TimeParams.SecPerMin;
			var season = !IsSeasonValid ? $"[BAD_SEASON: {Season}]" : _seasonNames[Season];

			return $"{season} {Day + 1}, {Year} at {hour:D2}:{min:D2}:{sec:D2} ({(100.0 * FractionalDay):F2}% of the day)";
		}

		private static readonly string[] _seasonNames = new[] { "Spring", "Summer", "Autumn", "Winter", };
	}
}
