<p align="center">
	<img src="https://tokei.rs/b1/github/zijistark/Pacemaker?category=code" alt="Lines of Code"/>
	<a href="https://www.codefactor.io/repository/github/zijistark/pacemaker"><img src="https://www.codefactor.io/repository/github/zijistark/pacemaker/badge" alt="CodeFactor"/></a>
</p>

## Pacemaker

Pacemaker is a game mod for [Mount &amp; Blade II Bannerlord](https://www.taleworlds.com/en/Games/Bannerlord) whose goal is, by default, to increase the pace of the game's "campaign time" (also known as "map time"). The intention is to make it reasonable for decades of campaign time to pass during normal playthroughs so that nobles may actually age, die, have children, undergo succession, and so on while also adding a more realistic perspective upon the amount of real time which would be taken by all the fun things we do in the Bannerlord universe.

Note that this increased pace is the default behavior. Pacemaker is actually fully configurable and can even be used to slow the pace of campaign time or, somewhat paradoxically but an example nevertheless, slow the passage of time but still reduce the number of days in a year. Care has been taken to ensure the time and calendar settings may be changed mid-playthrough without negative side effects. Pacemaker may also be added to a preexisting save without issue; it will adapt the vanilla notion of time to its time and calendar settings automatically.

It accomplishes this through redefining, at runtime, the game's core time implementation via the magic of dynamic method patching via Harmony. This is both to change the speed of campaign time itself (i.e., what is reflected by the frequency of the visual day/night cycle) as well as to alter the structure of the game's calendar with the default goal of making years in Calradia shorter.

Due to Pacemaker's approach, it doesn't need to perform hacks like timeshifting birthdays in order to age characters faster than calendar years permit or mess with any other jank: everything in the game reflects the altered notion of time naturally, because everything relies upon the modified core APIs for date/time calculations. There *are* a few things that appreciate further adjustment in order to work better with the configured time-scale, however. Pacemaker aims to take care of all of those things for you in a tightly-integrated fashion.

I welcome any contributions in the form of GitHub pull requests, testing, and feedback. Shoot me an email at zijistark@gmail.com or connect with me on Discord as `zijistark#3021`. I have a Slack workspace specifically for discussing Bannerlord modding and mods-- mainly for Pacemaker at this time, but Slack workspaces are invite-only, so you'll need to first express interest.

---

Author: Matthew D. Hall (**zijistark**)

License: [MIT License](LICENSE)
