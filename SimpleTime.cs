using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	[SaveableClass(1)]
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
			fracDays -= (double)(Year * Main.TimeParam.DayPerYearL);

			Season = ct.GetSeasonOfYear;
			fracDays -= (double)(Season * Main.TimeParam.DayPerSeasonL);

			Day = ct.GetDayOfSeason;
			fracDays -= (double)Day;

			FractionalDay = (float)fracDays;

			if (FractionalDay >= 1f && FractionalDay < 1.0001f)
				FractionalDay = 0.9999f;

			if (FractionalDay < 0f && FractionalDay > -0.0001f)
				FractionalDay = 0f;
		}

		public CampaignTime ToCampaignTime()
		{
			return CampaignTime.Years((float)Year) + CampaignTime.Seasons((float)Season) + CampaignTime.Days((float)Day + FractionalDay);
		}

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
			var season = (Season < 0 || Season >= _seasonNames.Length) ? $"[BAD_SEASON: {Season}]" : _seasonNames[Season];

			return $"{season} {Day + 1}, {Year} at {hour:D2}:{min:D2}:{sec:D2} ({(100f * FractionalDay):F3}% of the day)";
		}

		private static readonly string[] _seasonNames = new[] { "Spring", "Summer", "Autumn", "Winter", };
	}
}
