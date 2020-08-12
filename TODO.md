## TODO

We should address these issues / features / tests for Pacemaker in the relatively near future:

### Now:

- Add support for the Bannerlord e1.4.3 beta branch

- Depend upon Aragas's Bannerlord.Harmony mod library (though this may be a soft dependency, still distributing the Harmony DLL)

### Future:


#### High Priority

- Enable others to do translations / localisations of our [pretty small amount of] text
  - Use string keys / localisation keys for all text
  - Move English text to an XML file & provide a template for another translation (if not another translation itself -- e.g., Spanish)


#### Normal Priority

- More robust Harmony patching:
  - Like Community Patch, add acceptable hashes of our Harmony patch target method bodies so that we can recognize at runtime when there may be a problem caused by patching (mismatch with acceptable/confirmed hashes).
    - Requires adding a program to our build toolchain to actually generate the acceptable hashes
  - Also like Community Patch, check whether other mods have already patched one of our target methods in a conflicting way


---

Please do add more items to the agenda if pertinent.
