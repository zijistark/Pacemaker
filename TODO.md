## TODO

We should address these issues / features / tests for CampaignPacer (CP) in the relatively near future:

### Future:

#### High Priority

- Add setting *Prepare to Remove Campaign Pacer*
  - If enabled, on the next save it will convert the current time to vanilla units and clear our data from the savegame

- Optionally auto-adjust pregnancy duration to `3/4 * DaysInYear` (reality) or faster `36/84 * DaysInYear` (vanilla)
  - Use a dropdown selection to ensure mutual exclusivity
  - Dynamically target Harmony patch upon whatever pregnancy model is present at OnGameStart
  - Auto-adjust due dates of already in-progress pregnancies when converting from vanilla save

- Add settings preset for *Vanilla* (already have *Default* and the user will make *Custom*)


#### Normal Priority

- Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize when there may be a problem caused by patching (hash mismatch).

- Auto-adjust hero & troop/party healing rate if testing shows that it's faster with time multiplier > 1.


#### Low Priority

- Allow the user to configure a custom start date (my code already adjusts the start date due to a TW bug for alternative calendars)

- Consider usage of CP as a "library" mod for overhauls that want custom start dates and modified calendar properties

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter


### Testing:

- Test whether aging works correctly with time speed factors >= 1.5 and factors <= 0.667
  - FaceGen / aesthetic aging
  - Actual aging which can cause old age death probability to rise
    - Determine whether death due to old age even currently works in Bannerlord; if not, implement it.

- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly

- Test whether changing settings always properly triggers a save-triggered property change event in the debug log

- 

---

Please do add more items to the agenda if pertinent.
