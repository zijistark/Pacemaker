## TODO

We should address these issues / features / tests for CampaignPacer (CP) in the relatively near future:


### Fix Now:

- Ability to add CP to an existing savegame without warping the current date


### Do Now:

- Restore the proper campaign start time upon loading saves, in case calendar settings changed or loading a vanilla save


### In the Future:

#### High Priority

- Migrate from Bannerlord e1.4.0 to e1.4.1

- When time/calendar settings change, do not require restarting the game
  - Need MCM to notify us when a settings save triggers so that we can recalculate our 'runtime constants'

- Simplify the *Days Per Week* and *Weeks Per Season* settings into one *Days Per Season* setting


#### Normal Priority

- Add setting *Prepare to Remove Campaign Pacer*
  - If enabled, on the next save it will convert the current time to vanilla units (and clear our data from the savegame)

- Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize when there may be a problem caused by patching (hash mismatch).

- Auto-adjust pregnancy duration to `0.75 * DaysInYear`
  - Dynamically target Harmony patch upon whatever pregnancy model is present at OnGameStart
  - Auto-adjust due dates of already in-progress pregnancies

- Auto-adjust hero & troop/party healing rate if testing shows that it's faster with time multiplier > 1.


#### Low Priority

- Allow the user to configure a custom start date (my code already adjusts the start date due to a TW bug for alternative calendars)

- Consider usage of CP as a "library" mod for overhauls that want custom start dates and modified calendar properties

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter


### Necessary Testing

- Test whether aging works correctly with time speed factors >= 1.5 and factors <= 0.667
  - FaceGen / aesthetic aging
  - Actual aging which can cause old age death probability to rise
    - Determine whether death due to old age even currently works in Bannerlord; if not, implement it.

- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly

---

Please do add more items to the agenda if pertinent.
