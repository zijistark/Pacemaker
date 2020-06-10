using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	public class SimpleTime
	{
		[SaveableProperty(1)]
		public int Year { get; set; }

		[SaveableProperty(2)]
		public int Season { get; set; }

		[SaveableProperty(3)]
		public int Day { get; set; }

		[SaveableProperty(4)]
		public float FractionalDay { get; set; }

		[SaveableProperty(5)]
		public int DaysPerSeason { get; set; }

		public SimpleTime()
		{
			Year = -1;
			Season = -1;
			Day = -1;
			FractionalDay = -1f;
			DaysPerSeason = -1;
		}

		public SimpleTime(CampaignTime ct)
		{
			DaysPerSeason = (int)Main.TimeParam.DayPerSeasonL;
			double fracDays = ct.ToDays;

			Year = ct.GetYear;
			fracDays -= Year * Main.TimeParam.DayPerYearL;

			Season = ct.GetSeasonOfYear;
			fracDays -= Season * Main.TimeParam.DayPerSeasonL;

			Day = ct.GetDayOfSeason;
			fracDays -= Day;

			FractionalDay = MathF.Clamp((float)fracDays, 0f, 0.99999f);
		}

		public CampaignTime ToCampaignTime() => CampaignTime.Years(Year) + CampaignTime.Seasons(Season) + CampaignTime.Days(Day + FractionalDay);

		public bool IsNull => Year == -1 && Season == -1 && Day == -1 && FractionalDay == -1f;
		public bool IsValid => IsNull || (Year >= 0 && IsSeasonValid && Day >= 0 && IsFractionalDayValid);
		public bool IsValidWithCurrentCalendar => IsValid && Day < Main.TimeParam.DayPerSeasonL;
		private bool IsFractionalDayValid => FractionalDay >= 0f && FractionalDay < 1f;
		private bool IsSeasonValid => Season >= 0 && Season < TimeParams.SeasonPerYearL;

		public override string ToString()
		{
			// only intended for debugging
			var ct = CampaignTime.Days(FractionalDay);
			var hour = (int)ct.ToHours;
			var min = (int)ct.ToMinutes % TimeParams.MinPerHourL;
			var sec = (int)ct.ToSeconds % TimeParams.SecPerMinL;
			var season = !IsSeasonValid ? $"[BAD_SEASON: {Season}]" : _seasonNames[Season];

			return $"{season} {Day + 1}, {Year} at {hour:D2}:{min:D2}:{sec:D2} ({(100f * FractionalDay):F3}% of the day)";
		}

		private static readonly string[] _seasonNames = new[] { "Spring", "Summer", "Autumn", "Winter", };
	}
}
