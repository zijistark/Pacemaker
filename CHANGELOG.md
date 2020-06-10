## Changelog

### v0.8.0-alpha0

- Migrated from Bannerlord e1.4.0 to e1.4.1

- Collapsed *Days Per Week* and *Weeks Per Season* calendar settings into one *Days Per Season* setting.
  - Weeks play no actual role in this game, and users needn't think of them.
  - Code does attempt to factor the given days/season into reasonable values for weeks/season and days/week, largely for posterity.

- Altering time/calendar settings via the in-game menu should no longer require restarting the game to take effect.

- Calendar settings (days/season) are now also stored in savegames so that, if settings haven't changed since the last save, the saved calendar time doesn't need to be restored.
  - This was done because there can be a slight amount of floating-point precision error when restoring the calendar time, and this avoids it 99.99% of the time.

- When converting from a vanilla save, the converted time can never be earlier than the campaign start time (would be due to floating-point error).
