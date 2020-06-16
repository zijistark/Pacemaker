using System;
using System.Collections.Generic;

namespace Pacemaker
{
	public class TimeParams
	{
		/* units */
		public const int MsecPerSec = 1000;
		public const int SecPerMin = 60;
		public const int MinPerHour = 60;
		public const int HourPerDay = 24;
		public const int DayPerWeek = 7; // may not evenly divide a season (note lack of WeekPerSeason)
		public const int SeasonPerYear = 4;

		/* special units */
		public readonly int DayPerSeason; // independent configurable variable
		public readonly int DayPerYear;

		/* ticks per unit */
		public const long TickPerMsecL = 10L;
		public const long TickPerSecL = TickPerMsecL * MsecPerSec;
		public const long TickPerMinL = TickPerSecL * SecPerMin;
		public const long TickPerHourL = TickPerMinL * MinPerHour;
		public const long TickPerDayL = TickPerHourL * HourPerDay;
		public const long TickPerWeekL = TickPerDayL * DayPerWeek;
		public readonly long TickPerSeasonL;
		public readonly long TickPerYearL;

		/* double-precision floating-point ticks per unit */
		public const double TickPerMsecD = TickPerMsecL;
		public const double TickPerSecD = TickPerSecL;
		public const double TickPerMinD = TickPerMinL;
		public const double TickPerHourD = TickPerHourL;
		public const double TickPerDayD = TickPerDayL;
		public const double TickPerWeekD = TickPerWeekL;
		public readonly double TickPerSeasonD;
		public readonly double TickPerYearD;

		/* single-precision floating-point ticks per unit */
		public const float TickPerMsecF = TickPerMsecL;
		public const float TickPerSecF = TickPerSecL;
		public const float TickPerMinF = TickPerMinL;
		public const float TickPerHourF = TickPerHourL;
		public const float TickPerDayF = TickPerDayL;
		public const float TickPerWeekF = TickPerWeekL;
		public readonly float TickPerSeasonF;
		public readonly float TickPerYearF;

		/* units (vanilla) */
		public const int OldMsecPerSec = 1000;
		public const int OldSecPerMin = 60;
		public const int OldMinPerHour = 60;
		public const int OldHourPerDay = 24;
		public const int OldDayPerWeek = 7;
		public const int OldWeekPerSeason = 3;
		public const int OldSeasonPerYear = 4;

		/* ticks per unit (vanilla) */
		public const long OldTickPerMsecL = 10L;
		public const long OldTickPerSecL = OldMsecPerSec * OldTickPerMsecL;
		public const long OldTickPerMinL = OldSecPerMin * OldTickPerSecL;
		public const long OldTickPerHourL = OldMinPerHour * OldTickPerMinL;
		public const long OldTickPerDayL = OldHourPerDay * OldTickPerHourL;
		public const long OldTickPerWeekL = OldDayPerWeek * OldTickPerDayL;
		public const long OldTickPerSeasonL = OldWeekPerSeason * OldTickPerWeekL;
		public const long OldTickPerYearL = SeasonPerYear * OldTickPerSeasonL;

		/* double-precision floating-point ticks per unit (vanilla) */
		public const double OldTickPerMsecD = OldTickPerMsecL;
		public const double OldTickPerSecD = OldTickPerSecL;
		public const double OldTickPerMinD = OldTickPerMinL;
		public const double OldTickPerHourD = OldTickPerHourL;
		public const double OldTickPerDayD = OldTickPerDayL;
		public const double OldTickPerWeekD = OldTickPerWeekL;
		public const double OldTickPerSeasonD = OldTickPerSeasonL;
		public const double OldTickPerYearD = OldTickPerYearL;

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
		public readonly double TickRatioSeasonD;
		public readonly double TickRatioYearD;

		/* ratios of old/vanilla to our ticks per unit (single-precision) */
		public readonly float TickRatioSeasonF;
		public readonly float TickRatioYearF;

		public TimeParams(int daysPerSeason)
		{
			// set appropriate days/week and weeks/season to match requested days/season
			DayPerSeason = daysPerSeason;

			/* special units */
			DayPerYear = DayPerSeason * SeasonPerYear;

			/* ticks per unit */
			TickPerSeasonL = TickPerDayL * DayPerSeason;
			TickPerYearL = TickPerSeasonL * SeasonPerYear;

			/* ticks per unit, double-precision floating point */
			TickPerSeasonD = TickPerSeasonL;
			TickPerYearD = TickPerYearL;

			/* ticks per unit, single-precision floating point */
			TickPerSeasonF = TickPerSeasonL;
			TickPerYearF = TickPerYearL;

			/* ratios of vanilla units to new units, double-precision floating-point */
			TickRatioSeasonD = OldTickPerSeasonD / TickPerSeasonD;
			TickRatioYearD = OldTickPerYearD / TickPerYearD;

			/* ratios of vanilla units to new units, single-precision floating-point */
			TickRatioSeasonF = (float)TickRatioSeasonD;
			TickRatioYearF = (float)TickRatioYearD;
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
				$"{nameof(MsecPerSec)}    = {MsecPerSec}",
				$"{nameof(SecPerMin)}     = {SecPerMin}",
				$"{nameof(MinPerHour)}    = {MinPerHour}",
				$"{nameof(HourPerDay)}    = {HourPerDay}",
				$"{nameof(DayPerSeason)}  = {DayPerSeason}",
				$"{nameof(SeasonPerYear)} = {SeasonPerYear}",
				$"{nameof(DayPerYear)}    = {DayPerYear}",
			}));

			lines.AddRange(Indent(indent, --level, "Ticks per Unit:"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"{nameof(TickPerMsecL)}   = {TickPerMsecL}",
				$"{nameof(TickPerSecL)}    = {TickPerSecL}",
				$"{nameof(TickPerMinL)}    = {TickPerMinL}",
				$"{nameof(TickPerHourL)}   = {TickPerHourL}",
				$"{nameof(TickPerDayL)}    = {TickPerDayL}",
				$"{nameof(TickPerWeekL)}   = {TickPerWeekL}",
				$"{nameof(TickPerSeasonL)} = {TickPerSeasonL}",
				$"{nameof(TickPerYearL)}   = {TickPerYearL}",
			}));

			lines.AddRange(Indent(indent, --level, "Units (vanilla):"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"{nameof(OldMsecPerSec)}    = {OldMsecPerSec}",
				$"{nameof(OldSecPerMin)}     = {OldSecPerMin}",
				$"{nameof(OldMinPerHour)}    = {OldMinPerHour}",
				$"{nameof(OldHourPerDay)}    = {OldHourPerDay}",
				$"{nameof(OldDayPerWeek)}    = {OldDayPerWeek}",
				$"{nameof(OldWeekPerSeason)} = {OldWeekPerSeason}",
				$"{nameof(OldSeasonPerYear)} = {OldSeasonPerYear}",
			}));

			lines.AddRange(Indent(indent, --level, "Ticks per Unit (vanilla):"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"{nameof(OldTickPerMsecL)}   = {OldTickPerMsecL}",
				$"{nameof(OldTickPerSecL)}    = {OldTickPerSecL}",
				$"{nameof(OldTickPerMinL)}    = {OldTickPerMinL}",
				$"{nameof(OldTickPerHourL)}   = {OldTickPerHourL}",
				$"{nameof(OldTickPerDayL)}    = {OldTickPerDayL}",
				$"{nameof(OldTickPerWeekL)}   = {OldTickPerWeekL}",
				$"{nameof(OldTickPerSeasonL)} = {OldTickPerSeasonL}",
				$"{nameof(OldTickPerYearL)}   = {OldTickPerYearL}",
			}));

			lines.AddRange(Indent(indent, --level, "Ticks per Unit Ratio (vanilla to CP):"));

			lines.AddRange(Indent(indent, ++level, new List<string>
			{
				$"{nameof(TickRatioSeasonD)} = {TickRatioSeasonD}",
				$"{nameof(TickRatioYearD)}   = {TickRatioYearD}",
			}));

			return lines;
		}
	}
}
