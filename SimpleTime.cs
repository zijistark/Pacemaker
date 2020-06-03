using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	[SaveableClass(2)]
	internal class SimpleTime
	{
		[SaveableProperty(1)]
		internal int Year { get; set; } = -1;

		[SaveableProperty(2)]
		internal int Season { get; set; } = -1;

		[SaveableProperty(3)]
		internal int Day { get; set; } = -1;

		[SaveableProperty(4)]
		internal float FractionalDay { get; set; } = -1f;

		internal SimpleTime(CampaignTime ct)
		{
			double fracDays = ct.ToDays;

			Year = ct.GetYear;
			fracDays -= (double)(Year * Main.TimeParam.DayPerYearL);

			Season = ct.GetSeasonOfYear;
			fracDays -= (double)(Season * Main.TimeParam.DayPerSeasonL);

			Day = ct.GetDayOfSeason;
			fracDays -= (double)Day;

			FractionalDay = (float)fracDays;

			if (!(FractionalDay < 1f) && FractionalDay < 1.0001f)
				FractionalDay = 0.9999f;

			if (!(FractionalDay < 0f) && FractionalDay > -0.0001f)
				FractionalDay = 0f;

			Debug.Assert( IsFractionalDayValid() );
			Debug.Assert( IsSeasonValid() );
			Debug.Assert( IsValid() );
			Debug.Assert( IsValidWithCurrentCalendar() );
		}

		internal CampaignTime CampaignTime
		{
			get
			{
				return CampaignTime.Years((float)Year) + CampaignTime.Seasons((float)Season) + CampaignTime.Days((float)Day + FractionalDay);
			}
		}

		internal bool IsNull() => Year == -1 && Season == -1 && Day == -1 && FractionalDay == -1f;
		internal bool IsValid() => IsNull() || (Year >= 0 && IsSeasonValid() && Day >= 0 && IsFractionalDayValid());
		internal bool IsValidWithCurrentCalendar() => IsValid() && Day < Main.TimeParam.DayPerSeasonL;
		private bool IsFractionalDayValid() => FractionalDay >= 0f && FractionalDay < 1f;
		private bool IsSeasonValid() => Season >= 0 && Season < TimeParams.SeasonPerYearL;

		public override string ToString()
		{
			/* not at all optimized, lol. however, that's because I've no need to use this but for debugging. */
			var ct = CampaignTime.Days(FractionalDay);
			var hour = (int)ct.ToHours;
			var min = (int)ct.ToMinutes % TimeParams.MinPerHourL;
			var sec = (int)ct.ToSeconds % TimeParams.SecPerMinL;
			var season = (Season < 0 || Season >= _seasonNames.Length) ? Season.ToString() : _seasonNames[Season];

			return $"{_seasonNames[Season]} {Day + 1}, {Year} at {hour:D2}:{min:D2}:{sec:D2} ({(100f * FractionalDay):F3}% of the day)";
		}

		private readonly string[] _seasonNames = new[] { "Spring", "Summer", "Autumn", "Winter", };
	}
}
