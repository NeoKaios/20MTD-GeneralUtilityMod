# General Utility Mod, aka GUM
A [BepInEx](https://github.com/BepInEx/BepInEx/releases) plugin for [20 Minutes Till Dawn](https://store.steampowered.com/app/1966900/20_Minutes_Till_Dawn/).

## Features

GUM provides some quality of life configurable tweaks that make the game even better. These features are editable in-game via MTDUI or in BepInEx config file.

### Configurable features
#### Utility
- Menu starting point: can start the game directly in the Loadout menu for instance (needs restart)
- Reverse Timer: If you prefer your timer to increase (like in endless) (needs restart)
#### Gameplay
- No rune: Standard and Quickplay with no rune for a harder challange
## Depedency

For the in-game UI, rely on [MTDUI](https://github.com/legoandmars/MTDUI)

## For modders

- Clone the [repo](https://github.com/NeoKaios/20MTD-GeneralUtilityMod)
- Open repo in VSCode
- Setup $GameDir variable in *GeneralUtilityMod.csproj*
- ```dotnet build``` to build and deploy mod
- ```dotnet publish``` to publish a .zip file