## CampaignPacer: Changelog

### v0.8.0

#### v0.8.0-alpha4

- Disabled tick tracing (confident that we're A+ on tick synchronization at this point).

- If the option is enabled in settings, we now auto-adjust due dates of already in-progress pregnancies when converting from vanilla saves, different calendar settings, or a different year-scaled pregnancy duration factor.
  - We now also check if our pregnancy duration is equal to the effective one supplied by the currently installed `PregnancyModel`, and if not, then we never auto-adjust in-progress pregnancy due dates.
  - **POSSIBLE ISSUE:** An anecdotal test that also involved campaign time adjustment (days/season changed) wherein, prior to calendar adjustment, a character definitely had conceived a child but after calendar adjustment, the code for auto-adjustment of in-progress pregnancy due dates [correctly] triggered but reported processing 0 pregnancies.
    - As the code looks fine and the huge outstanding issue with stored `CampaignTime` objects in the game (such as the `DueDate` for a pregnancy) not being adjusted upon calendar time conversion is still unresolved, I've decided to defer further digging into this possible issue until after that outstanding issue has been resolved in v0.9.0.

#### v0.8.0-alpha3

- Changed save format, use fresh saves (this won't be happening again-- at least not for a long time)

#### v0.8.0-alpha2

- Calendar time conversion from non-CP / vanilla savegames is now virtually exact in its precision.

- Fixed a bug where relevant settings would not be synchronized to/from savegames as intended.

- `v0.8.0-alpha2` is save-incompatible with the previous version, because the fractional component of the `SimpleTime` class (represents symbolic calendar time) is now double-precision instead of single-precision.

- Internally, weeks are now always 7 days for compatibility purposes, regardless of whether 7 evenly divides the season or year length. Since they mean nothing to the player, let them mean what they were intended to mean to code.

- Generalized `SavedTimeSettings` into `SavedSettings` for the savegame synchronization of non-time settings that still need to be stored in savegames in order for changes in them to trigger conversions / adjustments upon load.
  - Motivated by eventual need to detect changes in *Year-Scaled Pregnancy Duration Factor* to trigger in-progress pregnancies' due date auto-adjustment

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
