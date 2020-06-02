## TODO

We need to address these issues / features / tests for CampaignPacer (CP) in the relatively near future:


### Coded but not tested at all:
- Ability to add CP to an existing savegame (without warping the current date)
  - Needs savegame serialization of calendar time instead of just ticks
- Ability to change the CP calendar settings mid-playthrough (without warping the current date)
  - Needs savegame serialization of calendar of time


### Not yet started


#### High Priority

- Synchronize daily tick -- and ideally weekly tick, except keep the multiplier of 7 to the daily tick instead of using custom days/week -- to CP's scaled map time


#### Normal Priority

- Auto-adjust pregnancy duration to `0.75 * DaysInYear`
  - Dynamically target Harmony patch upon whatever pregnancy model is present at OnGameStart

- Auto-adjust party healing rate if testing shows that it's faster with time multiplier > 1.


#### Lower Priority

- Allow the user to configure a custom start date (my code already adjusts the start date due to a TW bug for alternative calendars)

- Consider usage of CP as a "library" mod for overhauls that want custom start dates and modified calendar properties

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter

- Consider alternative calendar mode wherein a year is actually only 2 seasons (cycling between warm years and cool years but configurable where the line is drawn)
  - Not as lore-friendly but totally believable. Automatically gives another natural 2x year rate increase.
  - Requires special mode to code (currently the code assumes that seasonal units must be fully-contained within a year), but it'd be pretty easy
  - Could also have a 3-season/year or 1-season/year mode (former seems kind of confusing, latter actually makes sense to me)
  - An optional, non-default setting, of course


### Necessary Testing
- Test whether aging works correctly with time speed factors >= 1.5 and factors <= 0.667
  - FaceGen / aesthetic aging
  - Actual aging which can cause old age death probability to rise
    - Determine whether death due to old age even currently works in Bannerlord; if not, implement it.
- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly
- Test using values of `DaysPerWeek = 1` and/or `WeeksPerSeason = 1`, as using `WeeksPerSeason = 1` in particular could provide a very flexible way to specify your season length (e.g., you want a prime number of days/season like 7 or 5)

---

Please do add more items to the agenda if pertinent.
