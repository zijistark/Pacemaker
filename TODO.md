## Pacemaker: TODO

We should address these issues / features / tests for Pacemaker in the relatively near future:

### Now:

Nothing for now.

### Future:

#### High Priority

- Auto-adjust hero & troop/party healing rate if testing shows that it's faster with time multiplier > 1.


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

- Consider alternative presentation of Pacemaker as a silent, integrated library DLL for overhauls that want custom start dates and modified calendar properties but do not want the user to see Pacemaker

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter


### Testing:

- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly (applies to NPCs and party/troops)

- Test whether aging works correctly
  - FaceGen / aesthetic aging

---

Please do add more items to the agenda if pertinent.
