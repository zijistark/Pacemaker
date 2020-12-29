namespace Pacemaker
{
    /* Export
     *
     * Public interface to other modules for querying Pacemaker's critical information.
     * Simply intended to be used via reflection upon the Pacemaker assembly.
     * Subject to semantic versioning.
     */
    public static class Export
    {
        public static int GetDaysPerSeason() => Main.TimeParam.DayPerSeason;

        public static int GetDaysPerYear() => Main.TimeParam.DayPerYear;

        public static float GetTimeMultiplier() => Main.Settings!.TimeMultiplier;

        /* GetDaysPerHumanYear
         *
         * Takes into account the Accelerated Aging Factor to return the number of days in
         * a human character's year of aging.
         *
         * Since children and adults can be configured to age at different rates, an 'isAdult'
         * boolean parameter must be supplied for the hero or set of heroes in question.
         * Pacemaker adulthood is defined at ages greater than or equal to the age at which
         * children come of age / reach maturity and become active characters.
         */
        public static float GetDaysPerHumanYear(bool isAdult)
        {
            var ageFactor = isAdult ? Main.Settings!.AdultAgeFactor : Main.Settings!.ChildAgeFactor;
            return Util.NearEqual(Main.Settings.AdultAgeFactor, 1f, 1e-2)
                ? Main.TimeParam.DayPerYear
                : Main.TimeParam.DayPerYear / ageFactor;
        }
    }
}
