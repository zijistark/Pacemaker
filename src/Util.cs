using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using TaleWorlds.CampaignSystem;

namespace Pacemaker
{
    internal static class Util
    {
        internal static bool EnableLog
        {
            get => Log is GameLog; // GameLog, derived from GameLogBase, provides thread-safe, async logging & in-game text display
            set
            {
                if (Log is GameLog && !value)
                    Log = new GameLogBase();
                else if (!(Log is GameLog) && value)
                    Log = new GameLog(Main.Name, truncate: true, logName: "debug");
            }
        }

        internal static bool EnableTracer { get; set; } = false;

        internal static GameLogBase Log = new GameLogBase(); // GameLogBase, parent of GameLog, implements do-nothing virtual output methods

        // Slightly out of place, but meh:
        internal static bool NearEqual(float v1, float v2, float epsilon = 1e-5f) => Math.Abs(v1 - v2) < epsilon;
        internal static bool NearEqual(double v1, double v2, double epsilon = 1e-5f) => Math.Abs(v1 - v2) < epsilon;

        internal static class EventTracer
        {
            private static readonly ConcurrentDictionary<string, bool> _stackTraceMap = new ConcurrentDictionary<string, bool>();

            [MethodImpl(MethodImplOptions.NoInlining)]
            internal static void Trace(string extraInfo, int framesToSkip = 1) => Trace(new List<string> { extraInfo }, framesToSkip + 1);

            [MethodImpl(MethodImplOptions.NoInlining)]
            internal static void Trace(List<string>? extraInfo = null, int framesToSkip = 1)
            {
                if (!EnableTracer || !EnableLog)
                    return;

                var st = new StackTrace(framesToSkip, true);
                var frames = st.GetFrames();
                var evtMethod = frames[0].GetMethod();

                var msg = new List<string>
                {
                    $"Code Event Invoked: {evtMethod.DeclaringType}.{evtMethod.Name}",
                    $"Real Timestamp:     {DateTime.Now:MM/dd H:mm:ss.fff}",
                };

                if (Campaign.Current is not null)
                {
                    msg.AddRange(new List<string>
                    {
                        $"Campaign Time:      {new SimpleTime(CampaignTime.Now)}",
                        $"  Elapsed Years:    {Campaign.Current.CampaignStartTime.ElapsedYearsUntilNow:F3}",
                        $"  Elapsed Seasons:  {Campaign.Current.CampaignStartTime.ElapsedSeasonsUntilNow:F3}",
                        $"  Elapsed Days:     {Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow:F2}",
                        $"  Elapsed Hours:    {Campaign.Current.CampaignStartTime.ElapsedHoursUntilNow:F2}",
                        $"  Day of Season:    {CampaignTime.Now.GetDayOfSeason + 1}",
                        $"  Hour of Day:      {CampaignTime.Now.GetHourOfDay}",
                    });
                }

                var stStr = st.ToString();

                if (stStr.Length > 2)
                {
                    // ensure we're using Unix-style EOLs in the stack trace & remove extra newline at end
                    stStr = stStr.Replace("\r\n", "\n");
                    stStr = stStr.Remove(stStr.Length - 1, 1);

                    // only show a distinct stack trace once per event traced
                    if (_stackTraceMap.TryAdd(stStr, true))
                    {
                        msg.AddRange(new List<string>
                        {
                            string.Empty,
                            "Stack Trace:",
                            stStr,
                        });
                    }
                }

                if (extraInfo is not null && extraInfo.Count > 0)
                {
                    msg.AddRange(new List<string>
                    {
                        string.Empty,
                        "Extra Information:",
                    });

                    if (extraInfo.Count > 1)
                        msg.Add(string.Empty);

                    msg.AddRange(extraInfo);
                }

                Log.ToFile(msg, true);
            }
        }
    }
}
