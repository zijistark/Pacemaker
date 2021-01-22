using System.Collections.Generic;

namespace Pacemaker
{
    internal sealed class TimeParams
    {
        /* units */
        internal const int MsecPerSec = 1000;
        internal const int SecPerMin = 60;
        internal const int MinPerHour = 60;
        internal const int HourPerDay = 24;
        internal const int DayPerWeek = 7; // may not evenly divide a season (note lack of WeekPerSeason)
        internal const int SeasonPerYear = 4;

        /* special units */
        internal readonly int DayPerSeason; // independent configurable variable
        internal readonly int DayPerYear;

        /* ticks per unit */
        internal const long TickPerMsecL = 10L;
        internal const long TickPerSecL = TickPerMsecL * MsecPerSec;
        internal const long TickPerMinL = TickPerSecL * SecPerMin;
        internal const long TickPerHourL = TickPerMinL * MinPerHour;
        internal const long TickPerDayL = TickPerHourL * HourPerDay;
        internal const long TickPerWeekL = TickPerDayL * DayPerWeek;
        internal readonly long TickPerSeasonL;
        internal readonly long TickPerYearL;

        /* double-precision floating-point ticks per unit */
        internal const double TickPerMsecD = TickPerMsecL;
        internal const double TickPerSecD = TickPerSecL;
        internal const double TickPerMinD = TickPerMinL;
        internal const double TickPerHourD = TickPerHourL;
        internal const double TickPerDayD = TickPerDayL;
        internal const double TickPerWeekD = TickPerWeekL;
        internal readonly double TickPerSeasonD;
        internal readonly double TickPerYearD;

        /* single-precision floating-point ticks per unit */
        internal const float TickPerMsecF = TickPerMsecL;
        internal const float TickPerSecF = TickPerSecL;
        internal const float TickPerMinF = TickPerMinL;
        internal const float TickPerHourF = TickPerHourL;
        internal const float TickPerDayF = TickPerDayL;
        internal const float TickPerWeekF = TickPerWeekL;
        internal readonly float TickPerSeasonF;
        internal readonly float TickPerYearF;

        /* units (vanilla) */
        internal const int OldMsecPerSec = 1000;
        internal const int OldSecPerMin = 60;
        internal const int OldMinPerHour = 60;
        internal const int OldHourPerDay = 24;
        internal const int OldDayPerWeek = 7;
        internal const int OldWeekPerSeason = 3;
        internal const int OldSeasonPerYear = 4;

        /* special units (vanilla) */
        internal const int OldDayPerSeason = OldDayPerWeek * OldWeekPerSeason;
        internal const int OldDayPerYear = OldDayPerSeason * OldSeasonPerYear;

        /* ticks per unit (vanilla) */
        internal const long OldTickPerMsecL = 10L;
        internal const long OldTickPerSecL = OldMsecPerSec * OldTickPerMsecL;
        internal const long OldTickPerMinL = OldSecPerMin * OldTickPerSecL;
        internal const long OldTickPerHourL = OldMinPerHour * OldTickPerMinL;
        internal const long OldTickPerDayL = OldHourPerDay * OldTickPerHourL;
        internal const long OldTickPerWeekL = OldDayPerWeek * OldTickPerDayL;
        internal const long OldTickPerSeasonL = OldWeekPerSeason * OldTickPerWeekL;
        internal const long OldTickPerYearL = SeasonPerYear * OldTickPerSeasonL;

        /* double-precision floating-point ticks per unit (vanilla) */
        internal const double OldTickPerMsecD = OldTickPerMsecL;
        internal const double OldTickPerSecD = OldTickPerSecL;
        internal const double OldTickPerMinD = OldTickPerMinL;
        internal const double OldTickPerHourD = OldTickPerHourL;
        internal const double OldTickPerDayD = OldTickPerDayL;
        internal const double OldTickPerWeekD = OldTickPerWeekL;
        internal const double OldTickPerSeasonD = OldTickPerSeasonL;
        internal const double OldTickPerYearD = OldTickPerYearL;

        /* single-precision floating-point ticks per unit (vanilla) */
        internal const float OldTickPerMsecF = OldTickPerMsecL;
        internal const float OldTickPerSecF = OldTickPerSecL;
        internal const float OldTickPerMinF = OldTickPerMinL;
        internal const float OldTickPerHourF = OldTickPerHourL;
        internal const float OldTickPerDayF = OldTickPerDayL;
        internal const float OldTickPerWeekF = OldTickPerWeekL;
        internal const float OldTickPerSeasonF = OldTickPerSeasonL;
        internal const float OldTickPerYearF = OldTickPerYearL;

        /* ratios of old/vanilla to our ticks per unit (double-precision) */
        internal readonly double TickRatioSeasonD;
        internal readonly double TickRatioYearD;

        /* ratios of old/vanilla to our ticks per unit (single-precision) */
        internal readonly float TickRatioSeasonF;
        internal readonly float TickRatioYearF;

        internal TimeParams(int daysPerSeason = OldDayPerSeason)
        {
            // set appropriate days/week and weeks/season to match requested days/season
            DayPerSeason = daysPerSeason == 0 ? OldDayPerSeason : daysPerSeason;

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
            if (unit is null || level == 0)
                return lines;

            var indent = string.Empty;

            for (uint i = 0; i < level; ++i)
                indent += unit;

            List<string> results = new();

            foreach (var line in lines)
                results.Add(indent + line);

            return results;
        }

        private List<string> Indent(string unit, uint level, string line)
        {
            if (unit is null || level == 0)
                return new List<string> { line };

            var indent = string.Empty;

            for (uint i = 0; i < level; ++i)
                indent += unit;

            return new List<string> { indent + line };
        }

        public List<string> ToStringLines(uint indentSize = 0)
        {
            var indent = string.Empty;

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

            lines.AddRange(Indent(indent, --level, "Units (Vanilla):"));

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

            lines.AddRange(Indent(indent, --level, "Ticks per Unit (Vanilla):"));

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

            lines.AddRange(Indent(indent, --level, "Ticks per Unit Ratio (Vanilla to Pacemaker):"));

            lines.AddRange(Indent(indent, ++level, new List<string>
            {
                $"{nameof(TickRatioSeasonD)} = {TickRatioSeasonD}",
                $"{nameof(TickRatioYearD)}   = {TickRatioYearD}",
            }));

            return lines;
        }
    }
}
