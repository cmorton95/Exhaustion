# Exhaustion
A highly configurable plugin for Valheim intending to make stamina management more engaging, among various other changes to make gameplay more interesting. 

The plugin comes pre-configured with settings that make gameplay more interesting without harming the vanilla difficulty. If you find the balance isn't to your liking, the configuration allows almost everything to be customised to your needs.

## Main Features
* Exhaustion stamina system overhaul which applies debuffs and effects to punish stamina mis-management in more interesting ways than simply requiring you to kite enemies while your stamina regenerates
  * Allow for negative stamina values which incur negative (and/or possitive) status effects at configurable thresholds, becoming "Exhausted" at the final threshold
  * Allow sprinting with negative stamina (called "Pushing") to reach these thresholds - note that *only* sprinting is available with negative stamina
  * Apply additional stamina usage to attacks based on the weapon's weight
* Modify base player attributes, including
  * Health, stamina and carry weight
  * Stamina regeneration and delay
  * Jump, dodge and encumbrance stamina usage
* Alternative Encumbrance system to make carry weight less binary
  * Make movement speed scale with carry weight
  * Move "Encumbered" debuff threshold

## Additional Features
* Food value multipliers, modify health, stamina and time taken to burn
* Apply "Warmed Up" debuff when Pushing, temporarily removing cold and reducing the duration of the Wet debuff
* Customisation of Parry timing to allow for more or less time to parry (parry time is halved by default)
* Refund a portion of your stamina on a successful parry
* Movement speed and acceleration modifications
* No stamina requirement for building 

## Requirements
* [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
* [ValheimLib](https://valheim.thunderstore.io/package/ValheimModding/ValheimLib/)

## Installation
Install like any other BepInEx plugin, drop the dll into the BepInEx folder in the Valheim game directory. Configuration is done through the 7dd.dev.exhaustion.cfg file under BepInEx/config.

## Notes
Completely untested in multiplayer. If every client (and presumably the server) has the mod it might work, but I may not support issues with multiplayer.

Please report any issues on Github.

## Sources
[Github](https://github.com/cmorton95/Exhaustion)
[Thunderstore](https://valheim.thunderstore.io/package/etaks/Exhaustion/)
[Nexus Mods](https://www.nexusmods.com/valheim/mods/297)
