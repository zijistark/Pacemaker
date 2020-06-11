## TODO

We should address these issues / features / tests for CampaignPacer (CP) in the relatively near future:

### Now:

- Improve accuracy of vanilla date conversion logic

- Optionally auto-adjust pregnancy duration to `F * DaysInYear` where F is configurable
  - Dynamically target Harmony patch upon whatever pregnancy model is present at OnGameStart

### Future:

#### High Priority

- Whenever converting between time configurations, do a sanity pass over data structures and ensure things like:
  - No Hero.BirthDate is in the future
  - No Pregnanacy.DueDate is in the past or way too large
  - No Hero death dates are in the future or way too far from their birthdate

- Add setting *Prepare to Remove Campaign Pacer*
  - If enabled, on the next save it will convert the current time to vanilla units and clear our data from the savegame

- Auto-adjust due dates of already in-progress pregnancies when converting from vanilla save (assumes vanilla pregnancy duration)


#### Normal Priority

- Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize when there may be a problem caused by patching (hash mismatch).

- Auto-adjust hero & troop/party healing rate if testing shows that it's faster with time multiplier > 1.

- Look at compatibility issues which may arise when running alongside:
  - An enabled Bannerlord Tweaks's pregnancy model
  - LifeIsShort

- Add settings preset for *Vanilla* (already have *Default* and the user will make *Custom*)


#### Low Priority

- Use string keys everywhere & enable translations

- Create wrapper type for the `List<string>` objects currently being constructed all over the place for tracing
  - Hopefully a friendlier interface (does C# have variadic methods?)
  - Prevents actual `List<string>` construction if `Util.EnableTracer == false`

- Allow the user to configure a custom start date (my code already adjusts the start date due to a TW bug for alternative calendars)

- Consider usage of CP as a "library" mod for overhauls that want custom start dates and modified calendar properties

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter


### Testing:

- Test whether v0.8.0-alpha0 loads a vanilla campaign that's run for a bit
  - Take special note of time when saved vs. time when loaded

- Test loading a CP-enabled save game with & without different calendar settings
  - First create a fresh CP-enabled game.
  - (Do or do not modify calendar settings, both variants)
  - Is it the correct campaign time upon load (i.e., the campaign time that it was when saved)?

- Verify that equivalent `DaysPerWeekL` and `WeeksPerSeasonL` values are chosen when using a *Days Per Season* value greater than 9 (check debug log for time parameters)

- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly (applies to NPCs and party/troops)

- Test whether aging works correctly with time speed factors >= 1.5 and factors <= 0.667
  - FaceGen / aesthetic aging
  - Actual aging which can cause old age death probability to rise
    - Probably need some debug tracing of these probabilities for the player character to make it clear
    - Determine whether death due to old age even currently works in Bannerlord (w/ "Enable Death," I assume); if not, implement it.

---

Please do add more items to the agenda if pertinent.
