## TODO

We should address these issues / features / tests for Pacemaker in the relatively near future:

### Now:

- Update setting description for *Days Per Season* to note that a new game is required for changes to take effect, and campaigns will always use the value with which they were started.

- In `SaveBehavior`:
  - Drop `AdjustTimeOnLoad()` (was futile / only mattered if `DaysPerSeason` could change)
  - In `SyncData`, stop synchronizing `SavedTime` and indeed remove it altogether
  - `OnLoad`, ensure that `Main.TimeParam` is calibrated for the saved `DaysPerSeason`
  - `OnLoad`, display warning/reminder text if savegame uses different `DaysPerSeason` than the current value in `Settings`

- Remove `SimpleTime` from the `CustomSaveableTypeDefiner`

### Future:

#### High Priority


- In `SaveBehavior`:
  - Add a `BeforeSaveEvent` listener which updates the `GameSpecificSettings` structure/file with the current player character + player clan names and the save's configured `DaysPerSeason`
- Create a `GameSpecificSettings` class which encapsulates a mapping between player character + clan names (our best approximation of a campaign identifier) and a set of settings
  - Must save & load to formatted JSON (or custom) text format
  - Settings should be key-value and support data types of `long`, `decimal`, `bool`, and `string` (eventually-- first support `long` for `DaysPerSeason`)
  - Load upon submodule initialization

- Patch `Campaign.OnLoad` to lookup the player character ID in `GameSpecificSettings` and, if the current time parameters in effect don't match the saved `DaysPerSeason` for this campaign, then switch the time parameters appropriately 

#### Normal Priority

- Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize when there may be a problem caused by patching (hash mismatch).


- Add settings preset for *Vanilla* (already have *Default* and the user will make *Custom*)


#### Low Priority

- Use string keys everywhere to enable others to do translations (Spanish, French, Chinese, Polish, Russian, etc.)
  - Really, it's just translation of our settings menu

- Create wrapper type for the `List<string>` objects currently being constructed all over the place for tracing
  - Hopefully a friendlier interface (does C# have variadic methods?)
  - Prevents actual `List<string>` construction if `Util.EnableTracer == false`


- Allow the user to configure a custom start date (my code already adjusts and resets the start date)?


- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter


### Testing:


- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly (applies to NPCs and party/troops)

- Test whether aging works correctly
  - FaceGen / aesthetic aging

---

Please do add more items to the agenda if pertinent.
