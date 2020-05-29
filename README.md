## QuickTime

*No, it's not Apple's same-named [video codec](https://support.apple.com/downloads/quicktime), **for the last time**!*

QuickTime is a game mod for [Mount &amp; Blade II Bannerlord](https://www.taleworlds.com/en/Games/Bannerlord) whose sole goal is to increase the pace of the game's "campaign time" (also known as "map time"). The intention is to make it reasonable for decades of campaign time to pass during normal playthroughs so that nobles may actually age, die, have children, undergo succession, and so on while also adding a more realistic perspective upon the amount of real time which would be taken by all the fun things we do in the Bannerlord universe.

It accomplishes this through redefining at runtime the game's core campaign time implementation with help from Harmony's dynamic method patching services and the C# language's reflection intrinsics. This is both to directly speed up campaign time itself (i.e., what is reflected in the increased frequency of the visual day/night cycle) as well as to alter the structure of the game's calendar a bit with the goal of making years in Calradia shorter.

Due to QuickTime's approach, it doesn't need to perform hacks like timeshifting birthdays in order to age characters faster than calendar years permit or mess with old age death probability models: everything in the game reflects the quicker notion of time naturally, since everything relies upon the altered core API to inform it regarding time. There are a few things that appreciate further adjustment to work better with the new scale, however. QuickTime aims to take care of all of those things.

---
Author: **zijistark** // Matthew D. Hall

QuickTime, the mod, is licensed under the highly-permissive open source [MIT License](LICENSE) and all pull requests are welcome if you'd like to contribute.
