## TODO

We should address these issues / features / tests for Pacemaker in the relatively near future:

### Now:

- When loading a vanilla save for the first time, add a special popup reminder with explanatory text that *Days Per Season* will still be 21 if the setting isn't already set to 21.
  - Currently, they just receive the same, brief orange text that always shows when there is such a mismatch, but I think it'd be more intuitive to display something the user can't ignore when first loading a vanilla game into Pacemaker.

- Add a test Harmony patch for `Campaign.OnLoad` to see whether the player's clan name is available at that point

- Create a `GameSpecificSettings` class which encapsulates a mapping between player character + clan names (our best approximation of a campaign identifier) and a set of settings
  - Must save & load to formatted JSON (or custom) text format
  - Settings should be key-value and support data types of `long`, `decimal`, `bool`, and `string` (eventually-- first support `long` for `DaysPerSeason`)
  - Load upon submodule initialization

### Future:

#### High Priority


- In `SaveBehavior`:
  - Add a `BeforeSaveEvent` listener which updates the `GameSpecificSettings` structure/file with the current player character + player clan names and the save's configured `DaysPerSeason`

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
