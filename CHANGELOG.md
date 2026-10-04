## Spire Of Factorio
### Changelog
=====

### Current Version: 0.7.14

=====
## Roadmap

### 1.0 & Beyond
* Public Release?
  * This will more than likely need Art 2.0/3.0 to get enough away from pure Factorio art to be acceptable for public release 
* Additional Content
  * As StS2 continues, I will likely be able to integrate what Engineer currently has into the toolkit to keep him up to date and in line with the characters 

### 0.9+
* Art 2.0 
  * Re-do Engineer Character / Constructs to use pure 3D models instead of rendered 2D "effects"
  * Possibly make card art of my own instead of just all AI?
* Timeline / Epochs
  * A new "Chapter" will be added specifically for all the Factorio elements 
  * Factorio Events: 
	* Ch. 1. Complete Ascension 1 on all characters to unlock 'The Rail' event.
	* Ch. 2. Win runs with three Factorio Relics to unlock 'The Lost Engineer' event.
	* Ch. 3. Win runs with seven Factorio Relics to unlock 'The Engineer's Proposal' quest.
  * Engineer Quest from Lost Engineer Event
	* Ch. 4. As already present in the game, find the Engineer Event, take the quest card and win a run with the card.
	  * Winning a run with the quest card in hand will unlock the Engineer epoch, and The Engineer character
  * Relic, Card, Potions?
	* I will analyze the current suite of cards to pick which ones will go into the standard unlock requirements
	* The relics will follow the base game unlock proceedure 

### 0.8+
* Engineer Potions
* More Engineer Cards
* Maybe more constructs?
* Factorio Encounters
  * Biter / Pentapod Encounter Room (?)
  * Demolisher Boss (?)
  * Biter Egg Quest (?)
	* This was posed to me, could make it work.
  * Additional Events (?)
	* Not sure what I would do for additional Factorio themed events, but it's a possibility  

=====
## Version History
0.7.13 -> 0.7.14:
* Note:
  * I'm moving timeline to 0.9 and instead moving the next batch of mod content forward to 0.8, this will be the final 0.7 release and then I'll move to the 0.8 content.
* Misc
  * Shock now displayes the true stun percent chance in the text. 
  * Removed the Radar option from 'The Lost Engineer' from now, it will return in 0.9 with the Timeline update 
  * Updated Construct Targeting such that constructs can now be targeted by allies as well (See changes below in the Balance section) 
* Bugfixes:
  * Fixed the bug in the Scorch logic which prevented the Refined Flammables relic from properly applying.
  * Fixed the end-of-combat trigger heal for Gift of Gleba 
* Balance:
  * Adjustment / Buff: Speed Module
	* Store information on the first card played, each additional card played that turn adds one to the relic counter. At the start of your next turn add a copy of the original card to your hand with an increased cost, but Replay equal to the counter on the relic. Then reset the counter to 0.
	* Note: Speed module to me always felt like a "cheaper" version of Unceasing Top, if not less fun to use, so I wanted to give it a completely new gimmick.
  * Nerf: Efficiency Module
	* The relic now has a counter (2), which decreases by one when a reduced cost card is played. When the counter hits zero, all reduced cost cards in hand return to full cost. Restores to 2 each turn.
	* Note: Get your pitchforks out, but let's be real, this thing has a 100% take rate, it needs to be reigned in :)
  * Nerf / Adjustment: Power Armor
	* Base changed to +3 Titanium
	* Upgrade now moves to the original +5 Titanium
	* Note: With Speed Module 2.0 now a thing, the existing Power Armor can easily become busted. Adding more dexterity will push users more towards playing block instead of relying on the Titanium.
  * Buff: Not My Problem
	* Now gain scrap equal to the energy cost of the card(s) exhausted.
	* Note: Just wanted to give this card something to differentiate it a bit more from Purity, and this makes taking it a little more worth it.
  * Adjustment: The Nuclear Option
	* Now applies Shock based on the number of multiplayer players to guarantee a stun (e.g., 2 players = 110, 3 players = 120, etc.)
	* Note: This card is supposed to guarantee a Shock (As long as not already shocked), but in multiplayer, 100 shock is not 100%
  * Buff (Indirect): Grid Crash
	* The effect now applies to ALL allied constructs (Including other players)
  * Buff: Repair Kit
	* You can now target another player's construct and repair it
  * Buff: Overdrive
	* You can now target another player's construct and apply Overclock to it
  * Buff: Oil Slick
	* You can now target another player's construct and apply Slippery to it
  * Buff: Grid Redirection
	* You can now target another player's construct and remove Disabled from it

0.7.12 -> 0.7.13:
* Bugfixes:
  * Overload from Overclock now properly applies and is not immediately removed on turn change (Specifically for constructs that attack at the end of the turn)
  * Disabled the on add function for Utility Science Pack while I investigate why this is not working.

0.7.11 -> 0.7.12:
* Balance
  * Redesign: Overclock (2.0)
	* Note: Even with the continued adjustments to Overclock, I still don't like where it sits so I'm going to try something new.
	* New Overclock:
	  * Each stack of Overclock grants 50% output bonus to the Construct (Damage, Block, Status, etc)
	  * Whenever an 'Overclocked' construct performs an action, apply 1 stack of 'Overload' to the owning player, then the constuct loses one stack of 'Overclock'.
	* Overload:
	  * Half of the stacks of Overload are now lost when you end your turn, one stack is lost per card played
	  * The effect of Overload is unchanged (Extra cost on all cards) 
    * Disabled:
	  * This effect is now reserved for specific card actions (e.g., Hunker Down) 
	* Additional changes as a result of this adjustment:
	  * Fusion Reactor: Constructs no longer lose Overclock when performing an action, and you do not gain Overload from their actions. 
	  * Efficient Powering Network: At the start of your turn, lose (3, 5 - Upgrade) stacks of Overload
	  * Supercapacitor: Constructs with two or fewer stacks of Overclock do not apply Overload to you when acting 

0.7.10 -> 0.7.11:
* Misc
  * Bumped minimum BaseLib to 3.4.7 for 'X' cost card support. 
  * The rest of the changes in this update are compatibility updates to align with the new BaseLib 3.4.7 features that I originally wrote workarounds for. 

0.7.9 -> 0.7.10
* Bugfixes
  * Fixed the Entomancer (again, lol) 
  * Fixed the comical Scrap duplication bug from the Pristine Gear relic
  * Fixed a bug with the Utility Science pack when large quantities (Read 8+) of status cards would be added at once leading to a race condition with the AddedCard Hook. 
* Misc
  * Fixed the lovely spelling error on the 'Titanium' power. 
 
0.7.8 -> 0.7.9
* Decided to do another update before the Timeline stuff
* Bugfixes
  * Fixed the issue where the Entomancer's Dazed card would try to pass to Constructs, causing a softlock 
  * Shock Resist now lasts for the intended two turns instead of three. 
  * The Reduced Scrap Cost power no longer flashes excessively on the player. 
* New Content:
  * Card: Hellfire
	* Apply 10 (14) Scorch to a target. Scorch amount is increased by 20% (33%) for each stack of Vulnerable on the enemy.
  * Card: Security Detail
	* Multiplayer Card
	* All players assemble a Gun Turret (Gun Turret+) construct
  * Card: Pinpoint Regimen
	* Multiplayer Card
	* All players assemble a Laser Turret (Laser Turret+) construct
  * Slow Grenade
	* The target enemy loses 2 (3) Strength and Dexterity. Apply 10 (15) Shock to all enemies
  * Power Impact
	* Deal 13 (16) damage. Damage is increased by 1 (2) for each Block, Plating, and Titanium you have
  * Gunsmith
	* Multiplayer Card
	* All players select a Rare Engineer Attack card and add it to their hand. If upgraded, the added card is free to play this turn
  * Voltaic Inferno
	* Whenever you or your constructs apply Shock to an enemy, apply 5 (10) Scorch to that enemy
* Balance
  * The Nuclear Option
	* Reduced card cost to 4 (Upgrade reduces to 3)
	* Note: I originally designed this card around the Beacon, but getting two rare cards together in a deck is very hard, so this should make it easier to add to a deck and use.
  * Buff: Logistic Robot
	* The card you take is now free to play on that turn
	* If upgraded, the card you take is upgraded and also upgraded for the player you took it from.
	* Note: In the prior play session, I would say ~80% of the time, I could not play the card I took on the same turn and it felt counterproductive.
  * Buff: Robotics Assembly Routine
	* The upgraded card now adds upgraded robot cards to your hand.
  * Buff / Adjustment: Cargo Delivery
	* The "Weak Damage Adjustment" is now a 10% damage increase PER STACK of Weak on the target
	* Note: I want to try something different so I'm taking a wild swing at this card. This could prove to be an exceptionally lethal card with the right build.
  * Buff: Conductor
	* Each time you play Conductor during combat, its damage will now increase by 5 (7).
	* Note: In the spirit of building into more card longevity, this should help with looping builds
  * Buff: Railgun Shot
	* Now properly triggers block removal effects (e.g., Tunneler's Burrowed Buff)
	* Now removes all block when fired
	* Note: I wanted to give this card just a little more power behind it, this should put it in a perfect spot.
  * Buff: Overclock
	* At the end of the turn, only half of the amount of Overclock now becomes Disabled
	  * Note: I'm not sure I'm planning on keeping Disabled from Overclock much longer with how it feels, it may need a more significant rework, but for now this will reduce the impact of it.
	* Increased the base Overclock Percentage from 25% to 50%
	  * Note: Overclock still feels like it can do so much more, especially given the limited sources to build it up quickly on one turn. This should give it a more impactful hit. 
* Misc
  * When scrap with a value of 4 or more is printed on a card / relic it now shows a number and a single scrap icon instead of X scrap icons, following the behavior of other icon elements in the game.
  * Wall Block Gain now does so under the 'Fast' flag, which should make the end-of-turn block stacking much more quick 

0.7.7 -> 0.7.8
* Prior playtest showed that a round of balance adjustments would be nice to have right now since 0.8 will take a bit of time to finish.
* Bugfixes
  * Added an additional conditional check to ensure Utility Science Pack transforms status cards generated into your hand
	* Note: It should have been fine, but this is to 100% "zeke proof" things :)
  * Created a BaseLib Local Patch for 'X' cost CustomResource Cards not Working
	* Locally, this fixes BaseLib Issue #386 in the mod (https://github.com/Alchyr/BaseLib-StS2/issues/386)
	* This fix addresses "Rain of Trash" being unplayable and Softlocking the game
* New Content:
  * Card: Limitless Production
	* Convert all cards energy cost in your hand to scrap cost for that turn only
* Balance:
  * Adjustment: The Rail
	* Train Strike damage is now fixed at 8 and no longer increases on subsequent failures.
	* Note: See the last playtest, this should make things a little less punishing. I am thinking about making this a HP percentage instead of a fixed number, but let's see how this plays out first.
  * Buff: Scorch
	* Adjusted Scorch logic so the buff is removed if stacks are less than 5 before the reduction instead of after. 
	* Note: A handful of early game scorch appliers weren't able to stack up on multiple turns, this should make it much easier to build Scorch stacks effectively
  * Buff: Shock (Multiplayer)
	* The Shock per-player increase is now only 10 instead of 50 (e.g., Two Players: 110 = 100%, Three Players: 120 = 100%)
	* Note: The pendulum swing on shock was a bit too hard and with four players, it didn't fire off a single time, this should pull it back a bit.
  * Buff: All Constructs
	* Block passed from cards increased to 75%
	* Note: Prior playtest on higher ascension made this a "pain point" of the run, losing constructs on the same turn due to not having enough block. I still want players to need to play around lower block on them, but this should feel less punishing.
  * Buff: Gun Turret
	* Damage increased from 4 (6) to 5 (8)
	* Initial HP increased from 15 (22) to 20 (26)
	* Note: I felt like the gun turrets don't really pull enough slack in the later parts of Act 1, so this should make them feel a little better.
  * Buff: Quality Gun Turret
	* Damage increased from 5 (7) to 7 (10)
	* Initial HP increased from 20 (30) to 27 (34)
	* Note: Same as the regular gun turret, especially given this is an ancient upgrade card.
  * Buff: Laser Turret
	* Damage increased from 8 (10) to 10 (13)
	* Initial HP on the unupgraded laser turret increased to 27 (Upgraded unchanged from 35)
	* Note: The laser turret needed a little bump, while the upgraded turret is fine health wise, some more damage was needed.
  * Buff: Flamethrower Turret
	* Damage increased from 6 (8) to 8 (10)
	* Increased the HP on the unupgraded flamethrower turret to 30 (from 25)
	* Note: Similar to the laser turret, the base turret needed a little help, the upgrade was fine. Also wanted to slightly bump up damage.
  * Buff: Wall
	* Initial HP increased from 20 (27) to 30 (36)
	* Note: The wall is supposed to be your defensive construct, so it makes sense for it to be a little beefier.
  * Buff / Adjustment: Cargo Delivery
	* Swapped the energy / scrap cost (Now 2 Energy, 1 Scrap)
	* The upgrade now reduces the energy cost to 1, and also increases the Weak Damage bonus from 25% to 33%.
  * Buff: Harvest
	* Now gives 2 scrap at base value (Up from 1)
	* Note: Lining things up with Regent a bit more.
  * Buff: Electric Furnace
	* Increased Scrap gained from 3 (5) to 4 (7)
	* Note: This is meant to be a stronger ugprade compared to Harvest, and with Harvest getting a small bump, its only natural that this card also gets a bump.
  * Buff: Power Armor
	* Now grants 'Titanium' instead of 'Plating', which is 'Plating' that does not decrease each turn.
	* Power Armor+ now gives 2 extra Titanium (Up to 7)
  * Buff: Flashbang Grenade
	* Now gives 2 Vulnerable at base instead of 1.
	* Note: Buffing this up slightly to line up with the other direct application card (Suppressing Fire)
  * Buff: Reinforced Plating
	* Energy cost set to 0 (Was 1)
	* Scrap cost set to 1 (Was 2)
	* HP Increase has been increase from 5 (10) to 7 (14).
	* Note: Conceptually, I like this idea for a card, but I never once took it or played it. It just felt way too weak and too much of a cost for a small HP bump on the construct. This should make it a more competitive choice.
  * Buff: Repait Kit
	* Energy cost set to 0 (Was 1)
	* Scrap cost set to 1 (Was 1)
	* Similar to the above, it felt "too expensive" of a card. With the upgrade removing the scrap cost, this should be a really powerful mid to late game card in a construct focused deck.
  * Buff: Fusion Reactor
	* Energy cost reduced to 2
	* Note: Lines up with Fission Reactor now, technically the improved version from Dusty Tome, which is already rare enough of a find; Should make it easier to play this card
  * Buff: Gift of Gleba
	* Base HP Restore increased to 15 (Was 10)
	* Energy cost reduced to 2
	* Heal will now also trigger if combat ends before the turn count runs out.
	* Note: One bad turn can ruin this card's effect immediately, so making it a little more beefer of a heal and easier to play should make the risk/reward payout better.
* Misc
  * Fixed some grammatical issues in the localization text. 

0.7.6 -> 0.7.7:
* Small bugfix update prior to 0.8
* Bugfixes
  * Fixed Shock Resist applying to all enemies on the field
  * Fixed Shock Resist not decreasing correctly 

0.7.5 -> 0.7.6:
* Bugfixes
  * Fixed a logic error in the 'Complex Recipe' card, it now properly gives 2 block for each additional copy in your hand instead of a flat 4 block. 
* New Content
  * Scorch now has a "Health Bar Forecast" which displays expected damage at the end of a turn, similar to Poison
  * Added Localization strings for all of the Ancients for The Engineer 
* Balance
  * Buff: Utility Science Pack
	* Status cards are now transformed when added to your hand
	* Note: Some status effects were getting around Utility Science Pack, this should now make sure ALL of them are properly countered.
  * Nerf: Scorch
	* Removed the 'Unblockable' flag from Scorch damage, enemy block will now affect final output damage 
	* In conjunction with the Flamethrower Turret adjustment, this should make Scorch a bit more fair
  * Nerf: Flamethrower Turret
	* Reduced output Scorch to 6 (9) from 10 (15).
	* Note: Our last playtest is all the evidence needed here ( You haven't even found the 'Scorch Relic' yet Zeke ;) )
  * Adjustment / Nerf: Shock
	* Shock now scales based on the number of players in a session.
	* The calculation is based on a numerical factor between 0 - 100, which increasingly scales 50 each additional player.
	  * e.g., Two players bases the stack count up to 150 = 100% instead of 100 = 100%
	* When an enemy is stunned by Shock, they now gain 2 turns of 'Shock Resist'
	  * Internally you will see 3 appear initially, but one stack will be lost as it counts the stun as the first "turn" 
	  * All shock applications will be cut to 25% and a creature cannot be Stunned by Shock while this buff is active
    * Note: Shock on paper was a support tool for Engineer, but recent playtests have turned it into an offensive juggernaut, especially when multiple Tesla Turrets hit the field. I still want Shock to be "good", just not completely dominant as it was.
  * Misc
	* Adjusted the internal code structure in preparation for 0.8
	* Moved the Libraries out of the SpireOfFactorioCode namespace and into their own 'Libraries' namespace for cleaner differentiation within code files
	* Cleaned up unused Using statements from all files

0.7.4 -> 0.7.5:
* Bugfixes
  * Fixed a comical bug that allowed Fission Reactor and Fusion Reactor to endlessly grant more construct slots every turn
	* This addresses the bug with the Mining Drill power giving "exuberant" amounts of scrap each turn
  * Scrap Shield: Fixed the broken Localization Tag and it only giving one block instead of the intended block equal to the number of scrap held
  * Railgun / The Nuclear Option: Fixed the bug that prevented the damage being dealt from being 'Unblockable'. These attacks will now penetrate block correctly
  * Fixed the Logistics Robot Card
* New Content 
  * Card: Oil Slick
	* Add 'Slippery' to the targeted construct
  * Card: Blueprint
	* Your next 2 (3) scrap cost cards have reduced cost.
  * Card: Hunker Down
	* All constructs gain 20 (25) block, but are disabled for 2 (1) turn(s).
  * Card: Emergency Recall
	* Deconstruct the selected construct. Add its card to your hand with Retain and set its scrap cost to free until replayed. If the Construct is Disabled you gain Overloaded 
  * Card / Power: Discharge Defense
	* Whenever an enemy attacks you, apply 10 (15) shock to it.
  * Card: Junk to Riches
	* Convert 2 Scrap to 2 (3) Energy. Exhaust (Remove Exhaust).
  * Card / Power: Gift of Gleba
	* Adds a "Crop Growth" power to you that has 10 (15) stacks. Stacks are lost when taking damage. After 3 (2) turns, heal equal to the amount of stacks.
* Balance
  * Nerf: Rocket Fuel
	* The energy gained each turn is now only half the amount on the relic
	* Note: A recent playtest I did with Necrobinder combo'd this relic with the Ivory Tile to generate absolutely comical levels of energy. This needs to be toned down... quite a bit (I may need to do more here)
  * Misc: Railgun
	* Added 'Burrowed' to the list of powers railgun strips.
  * Buff: Incinerate
	* Increased Scorch Damage to 7 (10)
	* Note: While Incinerate is an early card pick option, for 2 energy it needed to be more "worth it", this gives some looping capability on the Scorch debuff.
  * Buff: Covering Fire
	* Buffed values to 25% (Was 10%) -> 35% (Was 15%).
	* Note: In the existing form this card was effectively useless for everything except the Artillery Turret. I'll need to monitor in case this is stupid OP, but this should help the lower tier turrets.
  * Buff: Temporary Barricades
 	* Reduced base scrap cost to 1 (Was 2)
	* Increased upgraded block from 17 to 20.
	* Note: I was pushing for a similar card to Regent's Glitterstream minus the block next turn, but this card felt a little too expensive for a one-off block card.

0.7.3 -> 0.7.4:
* Bugfixes
  * Addressed the softlock with the Scorch Power incorrectly trying to adjust strength / dexterity when the applying player did not have the Plasma Fuel relic
  * Addressed the bug where existing constructs would lose interactability when a new construct was spawned
  * Moved the Laser Turret position to 'Front' such that when multiple turrets are spanwed you can still interact with them
  * Adjusted the spawn positioning logic to make constructs have tighter positioning around the Engineer 
* New Content:
  * Card: Logistics Robot
	* First multiplayer card for the mod!
	* Select another player, add one card from that player's discard pile to your hand. When upgraded, a copy of the selected card is added to the top of the original player's draw pile.
  * Card: Robotics Assembly Routine
	* Add two robot cards (Construction Robot / Logistics Robot) to your hand. They are free to play this turn.
* Balance:
  * Nerf / Adjustment: Beacon Construct
	* Beacon will now only give 25% of scrap as energy. 
	* Only Beacon+ will now translate energy to allies and only 25% of the amount you gain will be passed to allies.
	* Note: Our last play session had me passing 5+ energy to everyone with only 12 Scrap, which is very easily gained. Given the existing of Mining Drill, this needed to be reigned in.
  * Nerf: Wall Construct
	* Block gained is now 7 (10) down from 10 (15)
	* Note: As hilarious as it is to loop this with Beacon of Hope and gain 100+ block each turn, that's clearly busted. :)
  * Buff: Overclock Power
	* Now grants 25% per stack up from 20%
	* Note: Overclock takes a bit to build into, so having it ramp up a little faster would be nice.
  * Buff / Adjustment: Railgun Strike
	* Now, if a target has 'Plating', 'Artifact', 'Slippery', 'Hardened Shell', or 'Hard to Kill', deals 15 extra damage and strips that respective buff immediately.
	* The card upgrade now also grants 5 extra damage to the base and extra bonus damage values.
	* Note: There, happy now Zeke?

0.7.2 -> 0.7.3:
* Bugfixes
  * Fixed the NCard UI to now show the correct scrap costs on cards.
	* BaseLib has an error in NCard duplication which must be fixed for X cost scrap cards to work on the UI (See: https://github.com/Alchyr/BaseLib-StS2/issues/386)
* New Content:
  * Added 'Covering Fire' Card / Power
	* 10% (15% Upgraded) of construct damage dealt is applied to you as block.
  * Added 'Efficient Powering Network' Card / Power
	* Constructs now lose 2 (3) stacks of 'Disabled' each turn.
  * Added 'Prompt Critical' Card
	* Select a construct, it gains 10 (15) stacks of 'Overclock'. Upon performing an action, it explodes, dealing 10 (15) damage to all enemies, then dies.
* Balance:
  * Adjustment / Buff: Flamethrower Turret
	* Flipped the direct damage / scorch debuff values.
	* Notes: The flamethrower turret should be a great early find to build up stacks of scorch. Right now it struggles to do so and whenever 2+ enemies are on the field, it just takes one turn to lose the debuff if the turret switches targets.
  * Buff: Laser Turret (Card)
	* Reduced Scrap Cost From 3 > 2.
	* Note: Laser Turret is meant to be a very common Construct card to find, so it doesn't make sense to swipe the entire base load of scrap to spawn one in.
  * Buff: Overdrive
	* Energy cost reduced (2 > 1), Now Grants 2 (3) Overclock instead of 1.
	* Note: Overdrive is supposed to be the early pick for Overclock, but 2 energy for a 20% buff to a construct for one turn felt very bad, so I took a big swing at this in the positive direction.
  * Buff: Defensive Perimeter
	* Reduced the scrap cost from 3 > 2
	* Note: This is the ancient upgrade to Gun Turret, and while the three Quality Gun Turrets are quite strong, losing all of your initial scrap to fire this off on an early turn doesn't feel like an "upgrade".
  * Buff: Power Armor
	* Reduced the scrap cost from 2 > 1
	* Power armor itself is already in a "good" spot on what it does, but I felt like it was a tad too expensive on the scrap side.
  * Buff: Grenade Toss
	* Added 'Apply 1 Weak'
	* Notes: In all of our playtests, Grenade Toss was only ever used as a "5% lols Shock" card. Suppressing Fire is a great one-off for weak (See below), so I felt Engineer needed a more direct damage route to Weak.
  * Buff: Suppressing Fire
	* Now applies 2 (3) Weak.
	* Note: With grenade toss doing damage + weak, it felt like the direct Weak card needed a little more "omph" to it.
  * Buff: Cargo Delivery
	* Reduced energy and scrap cost (Energy: 1 (Was 2), Scrap: 2 (Was 3)). 
	* Note: This card felt only slightly more powerful than the missile launcher card, but was so much more expensive due to the 3 scrap cost. This is meant to be a card to build with Suppressing Fire, so it makes sense to have it so both can be played on the same turn.
  * Buff: Rain of Trash
	* Now Deals 9 (13) Damage (Up from 7 (10)).
	* Note: Regent's 'Heavenly Drill' deals 8 (10) and gets doubled when X is more than 4. Rain of Trash doesn't get this second upgrade, so I felt it needed to do more damage at its base level.
  * Buff: Grid Redirection
	* Energy Cost Reduced to 1 (From 2)
	* Added 'Retain' to the Upgraded Card
  * Buff: Grid Crash 
	* Now gives 3(5) stacks of Overclock to all constructs

0.7.1 -> 0.7.2:
* Bugfixes:
  * Fixed Rain of Trash being unplayable
	* 0.7.5 Note: This fixed the incorrect scrap cost logic, however another bug was found that still makes this unplayable.
  * Shock now correctly persists for a set number of turns following application.
* New Content:
  * Redid all the existing Factorio Relic Art so it's not just copied from Factorio.
  * Added seven new relics to the mod for the Engineer, Go find them and break the game in more comical ways! :)
	* Scrap Shield 
	* Lightning Rod
	* Supercapacitor
	* Refined Flammables
	* Fulgoran Tesla Cell
	* Recovery Manual
	* Plasma Fuel

0.7.0 -> 0.7.1:
* Bugfixes:
  * Addressed a race condition in construct targeting logic that could lead to a softlock if an enemy died as a construct targeted it.
* New Content:
  * New Constructs:
	* Flamethrower Turret: Damages a random enemy 10 (15) and applies scorch 6 (8) to it.
	* Tesla Turret: Damages a random enemy 15 (20) and applies shock 25 (35) to it.
	* Artillery Turret: Damages the enemy with the highest health 30 (35) and applies shock 10 (20) to it.
  * New Cards: 
	* Flamethrower Turret: Spawns a flamethrower turret construct
	* Tesla Turret: Spawns a tesla turret construct
	* Shock and Awe: Spawns an artillery turret construct

0.6.7 -> 0.7.0:
* Bugfixes:
  * Fixed the invalid targeting condition in Shotgun Strike 
* New Content:
  * New Powers:
 	* Overclock: Provides a stacking bonus (20% * Stack) to construct output, then flips to an equal number of stacks of Disabled next turn
	* Disabled: The construct is unable to act for a number of turns equal to the stack count. One stack lost each turn.
	* Overload: Increase the cost required to play cards by 1. Each card played or turn passed reduces the stack count. Given when trying to Recycle a Disabled construct.
  * New Cards:
	* Overdrive
      * Gives 1(2) stacks of Overclock to the selected construct
	* Grid Redirection
	  * Removes 1(2) stacks of Disabled from the selected construct 
	* Grid Crash
	  * Gives 2(3) stacks of Overclock to all constructs. Exhaust
	* Fusion Reactor
	  * Note: This is the other Ancient Card, given by the Dusty Tome from Darv.
	  * Grants two construct slots. All constructs spawn with one overclock and can no longer be Disabled. 

0.6.6 -> 0.6.7:
* Bugfixes:
  * Fixed a display bug with some of the scrap cost elements on cards
	* Note: There is something going on with X cost right now, will investigate
* New Content: 
  * New Construct: Beacon
	* At the start of your turn, half of your current scrap number is added to you as energy. Half of that number is added to all allies.
  * New Card: Not My Problem
	* Exhaust 2(3) cards from your hand
  * New Card: Shotgun Strike
	* Deal damage to all enemies
  * New Card: Rain of Trash
	* First X card for scrap
	* Deal damage to all enemies X times
  * New Card: Mining Drill
	* Gain scrap for each open construct slot at the start of your turn
  * New Card: Fission Reactor
	* Gain 1 (2) construct slot(s)

0.6.5 -> 0.6.6:
* Bugfixes:
  * Addressed the softlock caused when scorch was being removed triggering a null object exception
  * Addressed the null object exception on targeting from the Laser Turret when multiple enemies were flagged as the "weakest" target 
* New Content:
  * Took an initial swing at putting Scrap cost on the cards
  * New Card: Resource Discovery
	* Gain scrap this turn and additional scrap the following turn
	* Note: Zeke, you can stop whining about scrap now, I have planned cards coming to fix this problem already :|

0.6.1 -> 0.6.5:
* Bugfixes:
  * Constructs are now in a "playable" state
    * Addressed the interaction bug on constructs
    * Addressed the bug preventing constructs from taking damage
* New Content:
  * Added a Scrap UI element next to the energy meter to show your current Scrap counter 
  * Added 8 more engineer cards
	* I forgot to document this. 
  * New Construct: Wall
	* Gives 10 (15) Block to all allies at the end of your turn.
	* Note: Wall Block cannot apply to itself or other wall(s).
  * New Construct: Laser Turret
	* Targets the weakest enemy and damages it and applies Scorch 
* Misc:
  * Added the 'Construct' keywork to Engineer cards to indicate which ones are used for spawning / interacting with constructs
  * Constructs now correctly gain 50% of Block from played cards 

0.6.0 -> 0.6.1:
* Bugfixes:
  * Addressed a bug with Efficiency Module softlocking the game 
  * ConstructLib:
	* Fixed the bug causing Constructs to be one with the darkness :)
* New Content:
  * Added about 10 new Engineer cards, but work for now is focused on the ConstructLib stuff.

0.5.0 -> 0.6.0:
* New Content:
  * New Character: The Engineer
	* This is a very (WIP) early alpha version
	* The Engineer is a hybrid between The Regent and The Necrobinder, focusing on the deployment of "Constructs" to the field.
	* His strength will primarily be focused in support capabilities, and will lack in Deck management tools (Draw / Discard style cards)
  * New Power: Scorch
	* This is an offensive debuff applied by The Engineer, similar to Poison, however it stacks much more quickly and therefore drains faster
  * New Power: Shock
	* This is a support debuff applied by The Engineer. It is a 0-100 counter which denotes a chance. When rolled, the target is stunned. 
  * New Construct: Gun Turret
	* Deals damage to a random enemy at the end of your turn
  * New Construct: Quality Gun Turret
	* Deals damage to all enemies at the end of your turn
	* Note: This construct will only come from the Ancient Upgrade

0.4.1 -> 0.5.0:
* Note: This is a small update, primarily again just prep work for what is to come.
  * Internally: Finished the alpha version of ConstructLib 
* New Content:
  * The Lost Engineer and The Rail now have event art 

0.4.0 -> 0.4.1:
* Note: Most of this update was behind the scenes work in preparation for the mod's new Character
* Bugfixes:
  * Addressed the bug where The Rail's bottom and top selections were flipped in the text prompt.
  * Addressed the bug where The Lost Engineer event could be soft locked
  * 'Frantic Escape' is now blacklisted from Utility Science Pack to prevent you from dooming yourself in that boss fight ;) 

0.3.0 -> 0.4.0:
* Misc
  * The two Factorio events are now linked, so you won't get the same reward twice in a single run.
* Balance
  * Adjustment: The Rail Event
	* Success Chance adjusted to 60%
	* HP loss now starts at 10 instead of 15.
	* You now get to select between three relics instead of just being given one.
	* Note: It felt kind of bad to get the RNG luck only to get handed something you didn't want/need. Also felt the event was a little too unforgiving still, so I tweaked the numbers a bit.

0.2.0 -> 0.3.0:
* Note: All releases will now be published on Steam Workshop going forward.
* New Content:
  * Relic: Bioflix
	* Gain 4HP at the start of combat, if you lost any HP during combat, gain 3 HP at the end of combat.
  * Relic: Recycler
	* Whenever you discard a card, increase the relic counter. At 3 stacks the next discarded card is upgraded and returned to your hand.
* Balance:
  * Adjustment: Rocket Fuel
	* Now has a counter which increses when playing 3 Cost cards. Gain energy equal to the counter each turn.
	* Note: Needed a little differentiation from Ivory Tile, this will help
  * Adjustment: Portable Energy Shield
	* Now gain 3 Artifact (Down from 5) at the start of non-boss combat, but can gain more whenever you end a turn with 30+ block.
	* Note: I felt like having a way to get more Artifact would help this relic a bit.
  * Adjustment: Lost Engineer Event
	* Can now appear when all players have 50 gold, instead of 100.
	* Note: This event wasn't popping up during our test runs (Someone needs to stop spending money... Looking at you Jensen)
  * Adjustment: The Rail Event
 	* Success chance changed to 50% (Was 35%)
	* Note: Stop whining about getting murdered by Trains Zeke, they are OBVIOUSLY RIGGED tracks :)

0.1.1 -> 0.2.0:
* New Content:
  * Event: The Rail
	* The Apex Predator of Factorio has arrived in Slay the Spire, do you accept inevit- risk it for the loot?
  * Relic: Productivity Module
	* Whenever you play a card, any copy of the same card in your hand becomes cheaper, but becomes 20% less effective.
  * Relic: Speed Module
	* Whenever you play a card, draw a card, but the cost of that card will be increased.
  * Card: Complex Recipe
	* Status Card
	* Gain 1 Dexterity this turn, gain 2 block for each copy of Complex Recipe ion your hand.
  * Relic: Utility Science Pack
	* Convert any status cards in your hand into Complex Recipe at the start of your turn.

0.1.0 -> 0.1.1:
* Balance
  * Nerf: Rocket Fuel
	* Changed to now only give one energy when playing a 3+ cost card.
	* Note: Yeah, this was absolutely busted.
  * Buff: Portable Fission Reactor
	* Reduced the strength and dexterity penalty to 2 from 4.
	* Note: This seems a little more fair.

0.0.2 -> 0.1.0:
* Bugfixes:
  * The Lost Engineer event can now be properly completed
* New Content:
  * Relic: Portable Energy Shield
	* Gain 5 Artifact at the start of all non-boss combat
  * Relic: Efficiency Module
	* Reduce the cost of all non-X cards by 1

0.0.1 -> 0.0.2:
* New Content:
  * Event: The Lost Engineer
	* Can randomly occur if all players have 100+ gold.
	* You will be offered a trade for three Factorio themed relics costing 50, 100, or 150 gold
  * Relic: Rocket Fuel
	* Whenever you play a card that costs three energy or more, gain two energy
  * Relic: Portable Fission Reactor
	* Powers no longer cost energy, lose four strength and dexterity at the start of combat 

0.0.1:
* First Release (Internal)
* BaseLib Intgration
* New Content:
  * Relic: Long-Handed Inserter  
	* A random card from your discard pile is returned to the top of your draw pile

=====
## Known Bugs

* Multiplayer Desyncs:
  * Recorded multiplayer desync when enemy turn began (Final boss - Queen) related to Power
* Rain of Trash Unplayable
  * Requires BaseLib to address the X card issue
* Shock appears to be rolling the same RNG instance for all creatures on the same turn instead of each creature rolling a unique RNG
* Incorrect Power Application
  * Something gave the 'Complex Recipe' power even though the relic was not present in the run. 