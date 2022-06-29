# General Utility Mod, aka GUM
A [BepInEx](https://github.com/BepInEx/BepInEx/releases) plugin for [20 Minutes Till Dawn](https://store.steampowered.com/app/1966900/20_Minutes_Till_Dawn/).

## Features

GUM provides some quality of life configurable tweaks that make the game even better.

### Configurable features
- Menu starting point: can start the game directly in the Loadout menu for instance
- No rune: Standard and Quickplay with no rune for a harder challange
- Reverse Timer: If you prefere your timer to increase (like in endless)
## Depedency

For the in-game UI, rely on [MTDUI](https://github.com/legoandmars/MTDUI)

## For modders

- Clone the project
- Open repo in VSCode
- Setup $GameDir variable in *GeneralUtilityMod.csproj*
- ```dotnet build``` to build and deploy mod
- ```dotnet publish``` to publish a .zip file
