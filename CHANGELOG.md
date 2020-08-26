## Pacemaker: Changelog

### v1.1.0

- Added export API for retrieving critical Pacemaker settings
  - You'll need to use reflection on the loaded Pacemaker assembly to access it
  - Supports useful things like `GetDaysPerHumanYear` which accounts for both the *Days Per Season* setting and the *Accelerated Aging Factor* to return the number of days which must pass for a character to age by a year

### v1.0.2

- Allowed *Days Per Season* to be configured to be as large as 90 days, effectively enabling an approximately 365-day year

- Made the hero and troop healing rate auto-calibration function of the *Time Multiplier* more liberal (instead of `sqrt(TM)`, it's now `pow(TM, 0.35)`)


### v1.0.1

- Adapted to changes in Bannerlord e1.4.3


### v1.0.0

- Added *Accelerated Aging Factor* setting that will accelerate character aging to be faster than actual calendar years elapsed. This complements the other two campaign time settings well, although it is disabled by default simply because one ought to understand how it works before they decide to use it.
  - NOTE: The setting to auto-calibrate pregnancy duration will still be calibrated to calendar years instead of the "human years" that result from accelerated aging.

- Synchronized cosmetic age to real age if *Accelerated Aging Factor* is enabled.

- Added party food consumption rate auto-calibration to the *Time Multiplier*.

- The default *Time Multiplier* is now 1.75 instead of 2.0.

- Patched a method in `HeroHelper` which assumed that years always consisted of 84 days.

- Upgraded Harmony to v2.0.2


### v0.11.0


#### v0.11.0-rc3

- The healing rate correction/calibration factor function of the *Time Multiplier* setting is now `1 / sqrt(TimeMultiplier)` for all values of the setting.

- The *Healing Rate Adjustment Factor* now applies before the auto-calibration due to *Time Multiplier* instead of the other way around, because this is more intuitive for those trying to use the setting.

- Stopped using MCM.Integrated and switched to MCM standalone (Pacemaker now depends upon the central MCM library mod)
  - Due to current game limitations with assembly resolving, this seems the safest overall in terms of any potential user headache, even though it means Pacemaker must depend explicitly upon MCM

- The default value for the *Time Multiplier* setting changed to 1.75 from 2.0

- Fixed the intended double line break in the dialog which pops up when loading a non-Pacemaker save

- Reversed the order of the *Time Multiplier* and *Days Per Season* settings so that the latter is the first setting. It's the most important.


#### v0.11.0-rc2

- Made Pacemaker's party healing rate auto-calibration to the *Time Multiplier* setting configurable
  - For users that feel the default auto-calibration isn't quite what they want, also added a slider for an additional, independent healing rate adjustment factor, predictably named the *Healing Rate Adjustment Factor*

- Hero healing rate and troop healing rate tooltips are now properly intuitive with the actual offset caused by auto-calibration to the *Time Multiplier* setting (and the new *Healing Rate Auto-Calibration Factor* setting) being shown rather than just redundantly showing the final result as if it were an offset.

- Addressed an issue with the method that actually heals heroes wherein it would always heal heroes at least 1 hit point per hour even if they should be healing much more slowly
  - When they should indeed be healing more slowly, the hourly HP value in `(0, 1)` is reconsidered as the probability to heal at all within that hour. A minimum of 5% healing chance per hour is enforced.

- Since healing rate auto-calibration is supposed to apply raw factors to the otherwise final result of healing rate calculations, ensured that the two related Harmony postfix patch priorities are minimally low (`Harmony.Priority.Last`) so that they definitely run last regardless of other possible patches.

- Separated the healing rate tooltips into separate offsets due to *Time Multiplier* auto-calibration and the user's [potentially] configured *Healing Rate Adjustment Factor*. Ensured that no zero-valued offsets would show up in the tooltip.

- Made the healing rate correction factor function of the *Time Multiplier* setting stop decreasing for multipliers above 4. Otherwise the correction factor function is `1 / TimeMultiplier`, so values above 4 simply use a correction factor of 0.25.

- Restructured things so that in-progress pregnancy due date adjustment will also work even when our own *Year-Scaled Pregnancy Duration* setting is not in effect due to another mod overriding it. This way, the player still benefits from the auto-adjustment upon configuration changes -- in whatever module.


#### v0.11.0-rc1

- Tick tracing has again been disabled due to verification of correct behavior

- When a non-Pacemaker save is loaded, a popup dialog reminds the player of the fact that their campaign will need to use an effective *Days Per Season* setting of 21 and why

- When the current setting for *Days Per Season* doesn't match the current campaign's locked-in setting, a short reminder text notification of the setting actually in effect is printed upon starting the session

- On game load, we now ensure that the current time parameters are calibrated for the saved *Days Per Season* setting (and similar for loading a vanilla save)
  - Additionally, we now use a savegame-external data store to allow us to predict the correct time parameters for a given campaign at the earliest point in the save loading process
    - This vastly reduces the likelihood of time incoherency at any game loading stage. Previously, we had to wait for the point at which we can extract our data from the save to correct anything, and now, that's just the backup plan.

- Removed all code related to adjusting the current time or campaign start time upon load of a save, as it was futile (*Days Per Season* can no longer change mid-game).

- Added a note in the *Days Per Season* setting's hint text about how the value is permanent for a campaign once you start it.

- Reverted tick auto-calibration system added in v0.9.0, intended for fluid support for changes in the *Days Per Season* setting, due to unforeseen and seemingly insurmountable issues

- Upgraded to MCM.Integrated v3.19


### v0.10.0

- Hero & troop health regeneration rate is now auto-calibrated to maintain vanilla pace. Examples:
  - If the *Time Multiplier* setting is 2, health regenerates half as quickly per day
  - If the *Time Multiplier* setting is 0.5, health regenerates twice as quickly per day

- Upgraded to MCM.Integrated v3.18.1 from v3.14

---

### v0.9.1

- Fixed several of the `CampaignTime` Harmony patches invoking, via reflection, `CampaignTime.CurrentTicks` as if it were an instance method rather than the static method it is. It's unclear whether this had any negative impact, but it's fixed regardless.

- Renamed submodule to _Pacemaker_

- Re-enabled tick tracing due to the major time overhaul

---

### v0.9.0

- Added a tick auto-calibration system that could've allowed Pacemaker to work with preexisting vanilla saves, change the *Days Per Season* setting mid-playthrough, and be easily removed without any correction necessary. By v0.11.0, we'd conclude that it couldn't reasonably be done without the unforeseen issues we encountered.

---

### v0.8.0

- Disabled tick tracing (confident that we're A+ on tick synchronization at this point).

- If the option is enabled in settings, we now auto-adjust due dates of already in-progress pregnancies when converting from vanilla saves, different calendar settings, or a different year-scaled pregnancy duration factor.
  - We now also check if our pregnancy duration is equal to the effective one supplied by the currently installed `PregnancyModel`, and if not, then we never auto-adjust in-progress pregnancy due dates.
  - **POSSIBLE ISSUE:** An anecdotal test that also involved campaign time adjustment (days/season changed) wherein, prior to calendar adjustment, a character definitely had conceived a child but after calendar adjustment, the code for auto-adjustment of in-progress pregnancy due dates [correctly] triggered but reported processing 0 pregnancies.
    - As the code looks fine and the huge outstanding issue with stored `CampaignTime` objects in the game (such as the `DueDate` for a pregnancy) not being adjusted upon calendar time conversion is still unresolved, I've decided to defer further digging into this possible issue until after that outstanding issue has been resolved in v0.9.0.

#### v0.8.0-alpha3

- Changed save format, use fresh saves (this won't be happening again-- at least not for a long time)

#### v0.8.0-alpha2

- Calendar time conversion from non-Pacemaker / vanilla savegames is now virtually exact in its precision.

- Fixed a bug where relevant settings would not be synchronized to/from savegames as intended.

- `v0.8.0-alpha2` is save-incompatible with the previous version, because the fractional component of the `SimpleTime` class (represents symbolic calendar time) is now double-precision instead of single-precision.

- Internally, weeks are now always 7 days for compatibility purposes, regardless of whether 7 evenly divides the season or year length. Since they mean nothing to the player, let them mean what they were intended to mean to code.

- Generalized `SavedTimeSettings` into `SavedSettings` for the savegame synchronization of non-time settings that still need to be stored in savegames in order for changes in them to trigger conversions / adjustments upon load.
  - Motivated by eventual need to detect changes in *Year-Scaled Pregnancy Duration Factor* to trigger in-progress pregnancies' due date auto-adjustment

#### v0.8.0-alpha1

- Fixed major issue with Harmony patching of patch class `Patches.CampaignPatch` that prevents loading the game

- Fixed a minor issue upon loading an older Pacemaker-enabled savegame with different configured days/season wherein Pacemaker could fail to restore the campaign time due to lack of saved configuration data.

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
