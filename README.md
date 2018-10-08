# FYLS

"F*#% Your Log S&^$" is a mod to clean up the unity logs from HBS's BattleTech game. It is aimed primarily at mod developers(' sanity) and is a part of [RogueTech](https://www.nexusmods.com/battletech/mods/79).

Instead of digging for the Unity log (especially on mac/linux), this puts a new file directly in your mods directory called `cleaned_output_log.txt`. Also it adds timestamps to the logging as well as the logger level.

Uses  [BTML](https://github.com/janxious/BattleTechModLoader) and [ModTek](https://github.com/janxious/ModTek).

## Why

The primary reason I wrote this is the unity logs for the game are full of spam. This is a [bug in Unity from **2009**](https://forum.unity.com/threads/debug-log-and-needless-spam.38720/). The shape of those is:

```
(Filename: C:/buildslave/unity/build/artifacts/generated/common/runtime/DebugBindings.gen.cpp Line: 51)
```

### Demo

Original:

```
Using Wide Video for Background
 
(Filename: /Users/builduser/buildslave/unity/build/artifacts/generated/common/runtime/DebugBindings.gen.cpp Line: 51)

Counted 3 save slots
 
(Filename: /Users/builduser/buildslave/unity/build/artifacts/generated/common/runtime/DebugBindings.gen.cpp Line: 51)

Counted 8 save slots
 
(Filename: /Users/builduser/buildslave/unity/build/artifacts/generated/common/runtime/DebugBindings.gen.cpp Line: 51)

```

New:

```
2018-10-03T20:11:52 - Log - Using Wide Video for Background
2018-10-03T20:11:52 - Log - Counted 3 save slots
2018-10-03T20:11:52 - Log - Counted 8 save slots
```

### Options

In [`mod.json`](mod.json), you can enter prefix strings that will not be logged in the new log with the option `PrefixesToIgnore`. By default some of CustomComponents logging is ignored.

### Caveats

The logging code for this lives inside the game code so it misses two kinds of logging:

1. This will not capture the very first parts of the unity log from game startup. If there is an error there you will still need to dig for the log.
2. This will not capture the very end of output from segfault and other crash-to-desktop errors.


### LICENSE

[MIT © Joel Meador 2018](LICENSE)
