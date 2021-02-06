# FYLS

"F*#% Your Log S&^$" is a mod to clean up the unity logs from HBS's BattleTech game. It is aimed primarily at mod developers(' sanity) and is a part of [RogueTech](https://www.nexusmods.com/battletech/mods/79).

Instead of digging for the Unity log (especially on mac/linux), this puts a new file directly in your mods directory called `cleaned_output_log.txt`. Also it adds timestamps to the logging as well as the logger level. It will also halt logging to the normal output log and put a raw log out in the mods folder.

Uses  [ModTek](https://github.com/BattletechModders/ModTek)

## Why

The primary reason I wrote this is the unity logs for the game are full of spam. This is a [bug in Unity from **2009**](https://forum.unity.com/threads/debug-log-and-needless-spam.38720/). The shape of those is:

```
(Filename: C:/buildslave/unity/build/artifacts/generated/common/runtime/DebugBindings.gen.cpp Line: 51)
```

Flipping 2009!!@#! A decade-old bug.

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
2018-10-03T20:11:52 FYLS [Log] - Using Wide Video for Background
2018-10-03T20:11:52 FYLS [Log] - Counted 3 save slots
2018-10-03T20:11:52 FYLS [Log] - Counted 8 save slots
```

### Options

In [`mod.json`](mod.json), you can enter prefix strings that will not be logged in the new log with the option `PrefixesToIgnore`. By default a set of info that identifies a computer's capabilities ignored. The prefix entry for that looks like `FYLS [LOG] SystemInfo.`.

Another option is `preserveFullLogging`. If this is enabled then an additional file will be created in your Mods folder: `output_log.txt`. It will be similar to what the vanilla game logs in full but without some of the annoying shit, and with timestamps.

### Setup Development

Copy FYLS.csproj.user.template as FYLS.csproj.user and modify the ReferencePath to point to your Battletech' "Managed" folder that contains all required DLL depednencies.

### Special Super Thanks!

@m22spencer for some extra good logging code and PR
@LadyAlekto for putting the mod through its paces

### LICENSE

[MIT © Joel Meador 2018](LICENSE)
