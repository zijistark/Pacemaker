## CampaignPacer

CampaignPacer is a game mod for [Mount &amp; Blade II Bannerlord](https://www.taleworlds.com/en/Games/Bannerlord) whose sole goal is to increase the pace of the game's "campaign time" (also known as "map time"). The intention is to make it reasonable for decades of campaign time to pass during normal playthroughs so that nobles may actually age, die, have children, undergo succession, and so on while also adding a more realistic perspective upon the amount of real time which would be taken by all the fun things we do in the Bannerlord universe. It will also be fully configurable soon, so the way in which it speeds (or slows) campaign time is under the user's control.

It accomplishes this through redefining at runtime the game's core campaign time implementation with help from Harmony's dynamic method patching services and the C# language's reflection intrinsics. This is both to directly speed up campaign time itself (i.e., what is reflected by the increased frequency of the visual day/night cycle) as well as to alter the structure of the game's calendar a bit with the goal of making years in Calradia shorter.

Due to CampaignPacer's approach, it doesn't need to perform hacks like timeshifting birthdays in order to age characters faster than calendar years permit or mess with any other jank: everything in the game reflects the quicker notion of time naturally, because everything relies upon the altered core API for campaign date/time calculations. There *are* a few things that appreciate further adjustment in order to work better with the configured time-scale, however. CampaignPacer aims to take care of all of those things for you in a tightly-integrated fashion.

I welcome any contributions in the form of GitHub pull requests, testing, and feedback. Shoot me an email at zijistark@gmail.com or connect with me on Discord as `zijistark#3021`. I have a Slack workspace specifically for discussing Bannerlord modding and mods, but Slack workspaces are invite-only, so you'll need to express interest first.

---

Author: Matthew D. Hall (**zijistark**)

License: [MIT License](LICENSE)
