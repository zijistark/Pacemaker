## TODO

We should address these issues / features / tests for Pacemaker in the relatively near future:

### Now:

- Display warning text when another mod has overridden the pregnancy model in a way which conflicts with the *Year-Scaled Pregnancy Duration* setting.

- Make in-progress pregnancy due date auto-adjustment work regardless of whether our configured pregnancy duration patch is in effect (due to potential mod conflicts).
  - Add a `PregnancyDuration` field to our `SavedValues` and use that as the "old duration" while asking whatever pregnancy model is installed for the new duration directly instead of calculating it from the `ScaledPregnancyDuration` and the year length ourselves.
  - The due date adjustment math will continue to work correctly (it only relies upon the old duration, the new duration, and the old due date).

### Future:


#### High Priority

- Enable others to do translations / localisations of our [pretty small amount of] text
  - Use string keys / localisation keys for all text
    - ... and figure out how this works exactly:
      - How do we generate a new string key ID properly? (6-digit alphanumeric code)
      - Do we even need to use 6-digit alphanumeric string keys? We might be able to use longer, semantically named keys (e.g., `{=Pacemaker_SettingsHintText_TimeMultiplier}`
  - Move English text to an XML file & provide a template for another translation (if not another translation itself -- e.g., Spanish)


#### Normal Priority

- More robust Harmony patching:
  - Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize at runtime when there may be a problem caused by patching (mismatch with acceptable/confirmed hashes).
    - Requires adding a program to our build toolchain to actually generate the acceptable hashes
  - Also like Community Patch, check whether other mods have already patched one of our target methods in a conflicting way

#### Low Priority

- Create wrapper type for the `List<string>` objects currently being constructed all over the place for tracing
  - Hopefully a friendlier interface (does C# have variadic methods?)
  - Prevents wasting cycles and RAM upon actual `List<string>` construction if `Util.EnableTracer == false`
  - First few Nexus releases will keep tracing enabled for debugging purposes, so this is ultra low-priority

- Auto-marry nobles over a certain age, as even at vanilla time paces, they have trouble marrying, and it'll be far worse with years that are much shorter
  - While closely related to our scope of speeding up the passage of years, this is probably best done as a separate mod or left to the preexisting *Matchmaker* mod


### Testing:


- Test whether health regeneration rate is sped up by the time multiplier and adjust accordingly (applies to NPCs and party/troops)

- Test whether aging works correctly
  - FaceGen / aesthetic aging

---

Please do add more items to the agenda if pertinent.
