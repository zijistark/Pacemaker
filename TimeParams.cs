using System;
using System.Collections.Generic;

namespace CampaignPacer
{
	public class TimeParams
	{
		/* units */
		public const long MsecPerSecL = 1000L;
		public const long SecPerMinL = 60L;
		public const long MinPerHourL = 60L;
		public const long HourPerDayL = 24L;
		public readonly long DayPerWeekL; // depends upon DayPerSeasonL
		public readonly long WeekPerSeasonL; // depends upon DayPerSeasonL
		public const long SeasonPerYearL = 4L;

		/* special units */
		public readonly long DayPerSeasonL; // independent configurable variable
		public readonly long DayPerYearL;

		/* ticks per unit */
		public const long TickPerMsecL = 10L;
		public const long TickPerSecL = TickPerMsecL * MsecPerSecL;
		public const long TickPerMinL = TickPerSecL * SecPerMinL;
		public const long TickPerHourL = TickPerMinL * MinPerHourL;
		public const long TickPerDayL = TickPerHourL * HourPerDayL;
		public readonly long TickPerWeekL;
		public readonly long TickPerSeasonL;
		public readonly long TickPerYearL;

		/* double-precision floating-point ticks per unit */
		public const double TickPerMsec = TickPerMsecL;
		public const double TickPerSec = TickPerSecL;
		public const double TickPerMin = TickPerMinL;
		public const double TickPerHour = TickPerHourL;
		public const double TickPerDay = TickPerDayL;
		public readonly double TickPerWeek;
		public readonly double TickPerSeason;
		public readonly double TickPerYear;

		/* single-precision floating-point ticks per unit */
		public const float TickPerMsecF = TickPerMsecL;
		public const float TickPerSecF = TickPerSecL;
		public const float TickPerMinF = TickPerMinL;
		public const float TickPerHourF = TickPerHourL;
		public const float TickPerDayF = TickPerDayL;
		public readonly float TickPerWeekF;
		public readonly float TickPerSeasonF;
		public readonly float TickPerYearF;

		/* units (vanilla) */
		public const long OldMsecPerSecL = 1000L;
		public const long OldSecPerMinL = 60L;
		public const long OldMinPerHourL = 60L;
		public const long OldHourPerDayL = 24L;
		public const long OldDayPerWeekL = 7L;
		public const long OldWeekPerSeasonL = 3L;
		public const long OldSeasonPerYearL = 4L;

		/* ticks per unit (vanilla) */
		public const long OldTickPerMsecL = 10L;
		public const long OldTickPerSecL = OldMsecPerSecL * OldTickPerMsecL;
		public const long OldTickPerMinL = OldSecPerMinL * OldTickPerSecL;
		public const long OldTickPerHourL = OldMinPerHourL * OldTickPerMinL;
		public const long OldTickPerDayL = OldHourPerDayL * OldTickPerHourL;
		public const long OldTickPerWeekL = OldDayPerWeekL * OldTickPerDayL;
		public const long OldTickPerSeasonL = OldWeekPerSeasonL * OldTickPerWeekL;
		public const long OldTickPerYearL = SeasonPerYearL * OldTickPerSeasonL;

		/* double-precision floating-point ticks per unit (vanilla) */
		public const double OldTickPerMsec = OldTickPerMsecL;
		public const double OldTickPerSec = OldTickPerSecL;
		public const double OldTickPerMin = OldTickPerMinL;
		public const double OldTickPerHour = OldTickPerHourL;
		public const double OldTickPerDay = OldTickPerDayL;
		public const double OldTickPerWeek = OldTickPerWeekL;
		public const double OldTickPerSeason = OldTickPerSeasonL;
		public const double OldTickPerYear = OldTickPerYearL;

		/* single-precision floating-point ticks per unit (vanilla) */
		public const float OldTickPerMsecF = OldTickPerMsecL;
		public const float OldTickPerSecF = OldTickPerSecL;
		public const float OldTickPerMinF = OldTickPerMinL;
		public const float OldTickPerHourF = OldTickPerHourL;
		public const float OldTickPerDayF = OldTickPerDayL;
		public const float OldTickPerWeekF = OldTickPerWeekL;
		public const float OldTickPerSeasonF = OldTickPerSeasonL;
		public const float OldTickPerYearF = OldTickPerYearL;

		/* ratios of old/vanilla to our ticks per unit (double-precision) */
		public readonly double TickRatioWeek;
		public readonly double TickRatioSeason;
		public readonly double TickRatioYear;

		/* ratios of old/vanilla to our ticks per unit (single-precision) */
		public readonly float TickRatioWeekF;
		public readonly float TickRatioSeasonF;
		public readonly float TickRatioYearF;

		public TimeParams(int daysPerSeason, List<string> trace = null)
		{
			// set appropriate days/week and weeks/season to match requested days/season
			DayPerWeekL = daysPerSeason;
			WeekPerSeasonL = 1;

			// we COULD just leave weeks per season == 1, but if the days per season can be evenly divided
			// into multiple weeks of any of these sizes, we will use the first days per week value that
			// qualifies in this list. Note that maximum days/season settable via MCM settings is 30.
			int[] tryDaysPerWeek = new[]
			{
				5,
				6,
				7,
				8,
				9,
				11,
				13,
			};

			foreach (int dpw in tryDaysPerWeek)
			{
				if (daysPerSeason > dpw && daysPerSeason % dpw == 0)
				{
					DayPerWeekL = dpw;
					WeekPerSeasonL = daysPerSeason / dpw;
					break;
				}
			}

			trace?.Add($"{nameof(TimeParams)}: Given {daysPerSeason} days/season, chose {DayPerWeekL} days/week and {WeekPerSeasonL} weeks/season");

			/* special units */
			DayPerSeasonL = DayPerWeekL * WeekPerSeasonL;
			DayPerYearL = DayPerSeasonL * SeasonPerYearL;

			/* ticks per unit */
			TickPerWeekL = TickPerDayL * DayPerWeekL;
			TickPerSeasonL = TickPerWeekL * WeekPerSeasonL;
			TickPerYearL = TickPerSeasonL * SeasonPerYearL;

			/* ticks per unit, double-precision floating point */
			TickPerWeek = TickPerWeekL;
			TickPerSeason = TickPerSeasonL;
			TickPerYear = TickPerYearL;

			/* ticks per unit, single-precision floating point */
			TickPerWeekF = TickPerWeekL;
			TickPerSeasonF = TickPerSeasonL;
			TickPerYearF = TickPerYearL;

			/* ratios of vanilla units to new units, double-precision floating-point */
			TickRatioWeek = OldTickPerWeek / TickPerWeek;
			TickRatioSeason = OldTickPerSeason / TickPerSeason;
			TickRatioYear = OldTickPerYear / TickPerYear;

			/* ratios of vanilla units to new units, single-precision floating-point */
			TickRatioWeekF = (float)TickRatioWeek;
			TickRatioSeasonF = (float)TickRatioSeason;
			TickRatioYearF = (float)TickRatioYear;
		}

		private List<string> Indent(string unit, uint level, List<string> lines)
		{
			if (unit == null || level == 0) return lines;

			var indent = String.Empty;
			for (uint i = 0; i < level; ++i)
				indent += unit;

			List<string> results = new List<string>();

			foreach (var line in lines)
				results.Add(indent + line);

			return results;
		}

		private List<string> Indent(string unit, uint level, string line)
		{
			if (unit == null || level == 0) return new List<string> { line };

			var indent = String.Empty;
			for (uint i = 0; i < level; ++i)
				indent += unit;

			return new List<string> { indent + line };
		}

		public List<string> ToStringLines(uint indentSize = 0)
		{
			var indent = String.Empty;

			if (indentSize > 0)
				for (int i = 0; i < indentSize; ++i)
					indent += " ";

			uint level = 0;
			var lines = new List<string> { "Time Parameters:" };

			lines.AddRange(Indent(indent, ++level, "Units:"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"MsecPerSecL    = {MsecPerSecL}",
				$"SecPerMinL     = {SecPerMinL}",
				$"MinPerHourL    = {MinPerHourL}",
				$"HourPerDayL    = {HourPerDayL}",
				$"DayPerWeekL    = {DayPerWeekL}",
				$"WeekPerSeasonL = {WeekPerSeasonL}",
				$"SeasonPerYearL = {SeasonPerYearL}",
				$"DayPerSeasonL  = {DayPerSeasonL}",
				$"DayPerYearL    = {DayPerYearL}",
			}));

			lines.AddRange(Indent(indent, --level, "Ticks per Unit:"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"TickPerMsecL   = {TickPerMsecL}",
				$"TickPerSecL    = {TickPerSecL}",
				$"TickPerMinL    = {TickPerMinL}",
				$"TickPerHourL   = {TickPerHourL}",
				$"TickPerDayL    = {TickPerDayL}",
				$"TickPerWeekL   = {TickPerWeekL}",
				$"TickPerSeasonL = {TickPerSeasonL}",
				$"TickPerYearL   = {TickPerYearL}",
			}));

			lines.AddRange(Indent(indent, --level, "Units (vanilla):"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"OldMsecPerSecL    = {OldMsecPerSecL}",
				$"OldSecPerMinL     = {OldSecPerMinL}",
				$"OldMinPerHourL    = {OldMinPerHourL}",
				$"OldHourPerDayL    = {OldHourPerDayL}",
				$"OldDayPerWeekL    = {OldDayPerWeekL}",
				$"OldWeekPerSeasonL = {OldWeekPerSeasonL}",
				$"OldSeasonPerYearL = {OldSeasonPerYearL}",
			}));

			lines.AddRange(Indent(indent, --level, "Ticks per Unit (vanilla):"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"OldTickPerMsecL   = {OldTickPerMsecL}",
				$"OldTickPerSecL    = {OldTickPerSecL}",
				$"OldTickPerMinL    = {OldTickPerMinL}",
				$"OldTickPerHourL   = {OldTickPerHourL}",
				$"OldTickPerDayL    = {OldTickPerDayL}",
				$"OldTickPerWeekL   = {OldTickPerWeekL}",
				$"OldTickPerSeasonL = {OldTickPerSeasonL}",
				$"OldTickPerYearL   = {OldTickPerYearL}",
			}));

			lines.AddRange(Indent(indent, --level, "Ticks per Unit Ratio (Vanilla / Us):"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"TickRatioWeek   = {TickRatioWeek}",
				$"TickRatioSeason = {TickRatioSeason}",
				$"TickRatioYear   = {TickRatioYear}",
			}));

			return lines;
		}
	}
}
