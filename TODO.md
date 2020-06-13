## TODO

We should address these issues / features / tests for CampaignPacer (CP) in the relatively near future:

### Now:

- Scan decompiled vanilla code for all `CampaignTime` methods which use weeks.
  - They might need a patch due to assumptions about a week's relation to other time units.


### Future:

#### High Priority

- Add setting *Prepare to Remove Campaign Pacer*
  - If enabled, on the next save it will convert the current time to vanilla units and clear our data from the savegame


#### Normal Priority

- Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize when there may be a problem caused by patching (hash mismatch).

- Auto-adjust hero & troop/party healing rate if testing shows that it's faster with time multiplier > 1.

- Add settings preset for *Vanilla* (already have *Default* and the user will make *Custom*)


#### Low Priority

- Use string keys everywhere to enable others to do translations (Spanish, French, Chinese, Polish, Russian, etc.)
  - Really, it's just translation of our settings menu

- Create wrapper type for the `List<string>` objects currently being constructed all over the place for tracing
  - Hopefully a friendlier interface (does C# have variadic methods?)
  - Prevents actual `List<string>` construction if `Util.EnableTracer == false`

- Overhaul means of storing & loading save-synchronized settings so that adding/removing settings in the future will be gracefully handled without invalidation of any previously-stored settings (i.e., allow partial settings to be loaded)

- Allow the user to configure a custom start date (my code already adjusts and resets the start date)

- Consider alternative presentation of CP as a silent, integrated library DLL for overhauls that want custom start dates and modified calendar properties but do not want the user to see CP

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter


### Testing:

- Test decreasing/increasing the effective pregnancy duration (via the days/season or year-scaled pregnancy duration-- or both) on an existing save where it was first set to a known value (36 coming from vanilla and 75% of a year in CP). Can you force most pregnancies to immediately deliver all at once with a severely decreased effective duration?

- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly (applies to NPCs and party/troops)

- Test whether aging works correctly with time speed factors >= 1.5 and factors <= 0.667
  - FaceGen / aesthetic aging
  - Old age death probabilities 

---

Please do add more items to the agenda if pertinent.
