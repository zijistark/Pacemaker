using System;
using TaleWorlds.CampaignSystem;

namespace Pacemaker
{
	public class SimpleTime
	{
		public int Year { get; set; }

		public int Season { get; set; }

		public int Day { get; set; }

		public double FractionalDay { get; set; }

		public SimpleTime()
		{
			Year = -1;
			Season = -1;
			Day = -1;
			FractionalDay = -1f;
		}

		public SimpleTime(CampaignTime ct)
		{
			double fracDays = ct.ToDays;

			Year = ct.GetYear;
			fracDays -= Year * Main.TimeParam.DayPerYear;

			Season = ct.GetSeasonOfYear;
			fracDays -= Season * Main.TimeParam.DayPerSeason;

			Day = ct.GetDayOfSeason;
			fracDays -= Day;

			FractionalDay = Math.Min(0.99999, Math.Max(+0.0, fracDays)); // clamp to [+0, 0.99999]
		}

		public CampaignTime ToCampaignTime() => CampaignTimeExt.YearsD(Year) +
			CampaignTimeExt.SeasonsD(Season) +
			CampaignTimeExt.DaysD(Day + FractionalDay);

		public bool IsValid => Year >= 0 &&
			IsSeasonValid &&
			Day >= 0 &&
			FractionalDay >= 0f &&
			FractionalDay < 1f;

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
