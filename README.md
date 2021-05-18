# PSTest_LilyRaeburn_Unity2019_4_26f1
 My coding test for Pineapple Studios.
 
 Copyright 2021. Lily Raeburn. All rights reserved.

 ===== HOW TO PLAY =====
 Start the game by tapping the button labelled "Start" in the menu;
 Play by using your finger to move the player left and right.
 Swipe up to jump over obstacles.
 Collect gems to beat the high score!

 ===== ADDITIONAL FEATURES =====
 -Loading screens
 -Menu
 -Pickup animations
 -Custom shader
 -Run speed multiplier
 
  ===== DEVELOPMENT ISSUES =====
  -The world generation led to a lot of memory leaks and Unity crashing.
  -Initially when creating the world curve shader I didn't realise there was a lighting pre-pass before the vertex stage. This created very bad looking shadows on most objects. Adding the "addshadow" identifier to the surface shader declaration fixed this.
  -It took me a while to decide on what method to use to map player input to character movement, after some fighting with the maths I decided to settle on the pixel distance between the mouse position and the player screen position.
  
  ===== WHAT WOULD I HAVE DONE DIFFERENTLY =====
  -Make the level generation more modular. The tile based system is quick to put together but if I had more time I would like to have put some more complexity and fine tuning into it. Currently the tile based system is quite inflexible.
  -Add corners and turns to the level generation. Although this is something I briefly attempted, due to the time constraints I wasn't able to fully implement this feature.
  -Add powerups and more pickups. The game is in dire need of health pickups, powerups (such as bonus gems, take no damage for a period of time, move faster, move slower, jump higher, etc.), and maybe even some incentive to spend gems in game to unlock paths or interact with NPCs.
  -Extra lives for watching an ad. When the player runs out of lives they are sent to the menu screen, however it would be a better idea to give the player the choice of watching an ad for a second chance and a few extra lives.
  -Permanant upgrades. It could be interesting to add a shop where the player could spend in-game or premium currency for permanant upgrades such as more lives, bonus gems, or even cosmetics like skins and accessories.
  -Sound effects. This demo has no sound which is a shame. Obviously it would liven things up quite a lot with some music and sound effects.
  
  ===== ASSET CREDITS =====
  https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153
  https://assetstore.unity.com/packages/3d/characters/humanoids/character-pack-free-sample-79870
  https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410
  https://assetstore.unity.com/packages/3d/environments/landscapes/simple-low-poly-nature-pack-157552
  https://assetstore.unity.com/packages/2d/textures-materials/sky/farland-skies-cloudy-crown-60004
  https://assetstore.unity.com/packages/3d/props/sets-gems-19902
  https://www.dafont.com/reglisse.font
  
  ===== TIME SPENT ON PROJECT =====
  I spent roughly 16 hours on this project, mostly in increments of 3-4 hours.