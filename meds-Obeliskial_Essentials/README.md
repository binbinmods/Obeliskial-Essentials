# Obeliskial Essentials

This is an **Across the Obelisk** mod that enables my other mods to work and communicate with each other. It includes a copy of sinai-dev's excellent [Configuration Manager](https://github.com/sinai-dev/BepInExConfigManager/). It also contains classes and methods that may be useful for other developers.

## Installation (manual)

1. Install the [BepInEx pack](https://across-the-obelisk.thunderstore.io/package/BepInEx/BepInExPack_AcrossTheObelisk/).
2. Click _Manual Download_ at the top of the page.
3. In Steam, right-click Across the Obelisk and select _Manage_->_Browse local files_.
4. Extract the archive into the game folder. 
5. Run the game. If everything runs correctly, you will see a list of registered mods on the main menu.
6. Press F5 to open/close the Config Manager and F1 to show/hide mod version information.

## Installation (automatic)

1. Download and install [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager) or [r2modman](https://across-the-obelisk.thunderstore.io/package/ebkr/r2modman/).
2. Click **Install with Mod Manager** button on top of the page.
3. Run the game via the mod manager.

## Options

| Option                          | Default | Description                                                                      |
|:--------------------------------|:-------:|:---------------------------------------------------------------------------------|
| **Export JSON**                 | false   | Export AtO class data to JSON files that are compatible with Obeliskial Content. |
| **Export Sprites**              | true    | Export sprites when exporting vanilla content.                                   |
| **Show At Start**               | true    | Show the mod version window when the game loads.                                 |
| **Disable Paradox Integration** | true    | Disable Paradox integration and telemetry (does not include launcher).           |
| **Skip Logos**                  | true    | Skip logos on startup.                                                           |

## Developer Tools

Press F2 to access the Developer Tools:
* mouse position lookup (useful for placing nodes/roads when [developing custom content](https://code.secretsisters.gay/AtO_How_To));
* toggle to disable AtO buttons (so you don't accidentally click on them while doing things!);
* +150 party XP;
* set enemy HP to 1;
* change starting node (for testing custom Act 1);
* card image export;
* [Tome of Knowledge Discord bot](https://github.com/stiffmeds/Tome-of-Knowledge) data export;
* activate event;
* activate combat;
* write Caravan shop to log (just a demo; expect this to be added to the [Seed Checker](https://code.secretsisters.gay/AtO_Seeds) at some point!); 
* a checksum calculator to confirm if you have the same content as someone else (work- in progress!); and
* a profile editor (work in progress!):
  * change supplies;
  * lock/unlock heroes;
  * change hero XP/rank progression; and
  * lock/unlock cards (including items).

## Support

Open a github issue or make a post in the **modding #support-and-requests** channel of the [official Across the Obelisk Discord](https://discord.gg/across-the-obelisk-679706811108163701).

## Donations

I make mods because I love it, not because I want to make money from it. If you really, really want to donate to me, my preferred non-profit organisations are [Greyhound Adoptions WA](https://greyhoundadoptionswa.com.au/donation/) ([paypal](https://www.paypal.com/donate?token=m8DwEGGEH0FFsS6PS-5p4MX9_5g8_ocMMrNFjaELN-xcG6Ok-KCFabu5xtB-57QBiOM7QLSuKVUepvL_)) or [Headache Australia](https://headacheaustralia.org.au/donate/).