# General Utility Mod, aka GUM

A [BepInEx](https://github.com/BepInEx/BepInEx/releases) plugin for [20 Minutes Till Dawn](https://store.steampowered.com/app/1966900/20_Minutes_Till_Dawn/).

## Features

GUM provides some quality of life configurable tweaks that make the game even better. These features are editable in-game via MTDUI or in BepInEx config file.

### Configurable features

#### Utility

- Menu starting point: can start the game directly in the Character select menu for instance (needs restart)
- Reverse Timer: If you prefer your timer to increase (like in endless) (needs restart)

#### Quality of life

- Skill Hold: Holding the right-click down during a reload will now trigger Abby's special upon finishing the reload !

#### Achievement Watcher

Display special achievements that should unlocked at the end of the run

Include the following : `Nimble`, `Pacifist`, `Reckless`, `On the edge` and `Gotta catch 'em ALL`

Configuration:
- Display mode: `Never`, `Pause` or `Always`
- Display currently inactive achievements: `true` or `false`

## Depedency

For the in-game configuration UI, rely on [MTDUI](https://github.com/legoandmars/MTDUI)

## Contributions

The code for the *Achievement Watcher* UI was based on @sloverlord [BetterUI](https://github.com/sloverlord/BetterUI) mod

## For modders

- Clone the [repo](https://github.com/NeoKaios/20MTD-GeneralUtilityMod)
- Open repo in VSCode
- Setup $GameDir variable in *GeneralUtilityMod.csproj*
- ```dotnet build``` to build and deploy mod
- ```dotnet publish``` to publish a .zip file