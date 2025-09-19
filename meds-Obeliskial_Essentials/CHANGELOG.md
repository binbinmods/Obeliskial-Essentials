# 1.5.2

Fixed card descriptions when multiple % based damage increases on an item.

Added "Heal Attacker" for Item.HealPercent and Item.HealQuantity.

Added description for Item.Energy when granting energy to someone other than self.

Fixed sprite export for Mac.

# 1.5.1

Prevented a bunch of things from being improperly turned into English when using non-English languages.

Fixed GetRandomCardIdByTypeAndRandomRarity

Added additional functions to the HeroFunctions

# 1.5.0

Updated for AtO version 1.6

# 1.4.9

Updated to allow proper NPC loading/upgrading in combats

Cards now properly display the Stealth Icon and should properly display all X values.

Added useful Custom Functions for Hero creation and more

AddTextToCardDescription now refreshes description when added - allows it to be used more widely

# 1.4.8

Fixed export issue of with IncreasedDirectDamageReceived

Fixed LootMisc

# 1.4.7

Temporary fix for not being able to load into Multiplayer games with "offscreen" heroes.

# 1.4.6

Fixed a text issue with "Dispel all" in the case where a card also has certain types of Special Values.

Fixed a text issue with transforming more than one aura/curses

Updated the adding text system

Custom text should appear properly in combat (assuming the text system is used). This is still not updated to use precalculated values.

# 1.4.5

Updated the Text system.

# 1.4.4

Alphabetized hero selection and added scroll. All modded heroes will now be present regardless of their original OrderInList property.

Multiple dispels are now displayed when special values are used.

Text is now properly combined for dual damage cards when damage is transformed

Card Descriptions can now handle "When you apply X Aura/Curse Apply Y Aura/Curse to All Heroes/Enemies"

Data export should now properly include descriptionIds.

Data export/import now includes Singularity cards

Profile Editor now can edit madness levels

# 1.4.3

Fixed a bug where the game would crash when completing a run.

# 1.4.2

Updated text on "Reduce cost by -1" to be "Increase Cost by 1"

# 1.4.1

Hopefully fixed the bug where the game would crash on Windows after completing a run.

If it does not work on Mac, roll back to v1.4.0

# 1.4.0

Initial update for AtO v1.5.0

This is an Initial update, so chances of bugs are relatively high.

# 1.3.0

Initial update for AtO v1.4.

# 1.2.6

New checksum method and Falo Rowi-specific checksums.

# 1.2.5

Card description fix for Tabula Rasa.

# 1.2.4

Fixed the same issue as 1.2.3, but for gold/experience!

# 1.2.3

Fixed a vanilla (!) issue where no hellmodemob could bug card rewards.

# 1.2.2

Fixed references to Mac rather than Windows!

# 1.2.1

Developer Tools (F2): Calculate Checksums.

# 1.2.0

Developer Tools (F2): change starting node, card image export, Tome of Knowledge Discord bot data export, activate event, activate combat and write Caravan shop to log.
Profile Editor (F2): change supplies, lock/unlock heroes, change hero XP/rank progression and lock/unlock cards.
Updated Consistency option to work for AtO v1.3.02.
New option to skip logos on startup (enabled by default).

# 1.1.0

Developer Tools (F2): +150 party XP; set enemy HP to 1; disable AtO buttons.
AddModVersionText has been deprecated and replaced with RegisterMod.
New properties from Queen DLC.
New option to remove Paradox integration and telemetry (enabled by default).

# 1.0.3

Change Hero Selection Manager spacing.

# 1.0.2

Updated BepInEx dependency zzzzz.

# 1.0.1

Added dependency to original BepInEx pack (now that it's updated - woo!).
Removed BepInEx pack from this mod.

# 1.0.0

Initial release.
