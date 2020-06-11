## CampaignPacer: Changelog

### v0.8.0

#### v0.8.0-alpha1

- Fixed major issue with Harmony patching of patch class `Patches.CampaignPatch` that prevented loading the game

- Fixed a minor issue upon loading an older CP-enabled savegame with different configured days/season wherein CP could fail to restore the campaign time due to lack of saved configuration data.

- Add settings menu options for pregnancy duration (but not yet a full implementation)
  - *Year-Scaled Pregnancy Length Factor* is implemented, and as long as Bannerlord Tweaks doesn't explicitly override this particular duration, it should be in full effect

- Fixed a hard-coded assumption in `AgingCampaignBehavior` regarding the number of weeks in a year. Death probabilities will now always globally update every year.

#### v0.8.0-alpha0

- Migrated from Bannerlord e1.4.0 to e1.4.1

- Collapsed *Days Per Week* and *Weeks Per Season* calendar settings into one *Days Per Season* setting.
  - Weeks play no actual role in this game, and users needn't think of them.
  - Code does attempt to factor the given days/season into reasonable values for weeks/season and days/week, largely for posterity.

- Altering time/calendar settings via the in-game menu should no longer require restarting the game to take effect.

- Calendar settings (days/season) are now also stored in savegames so that, if the setting hasn't changed since the last save, the saved calendar time doesn't need to be restored.
  - This was done because there can be a slight amount of floating-point precision error when restoring the calendar time, and this avoids it 99.99% of the time.

- When converting from a vanilla save, the converted time can never be earlier than the campaign start time (would be due to floating-point error).
