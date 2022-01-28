# Speen Phone
 Sound customization mod
 
## How to set-up (v1.1)
 - Download the mod and put it in your `plugins` folder
 - Start the game to generate the config file
 - Close the game
 - Head to `Documents\Speen Mods\SpeenPhoneConfig.ini`
 - Change `/path/to/hitsound/folder` to the path of whatever hitsound folder you want to use
 - Restart the game. If no errors or warnings are logged before the main menu loads, you should be good to go!
 
 (More details about the hitsound folder format [here](#hitsound-folder-info))

## Roadmap
 - [x] Play sound on note hits
 - [x] Play sound on misses
 - [x] Randomize sound (~~weight system~~)
 - [ ] Hold sound
 - [ ] Better sound configuration (possibly in-game?)

## Hitsound folder info
 Since version 1.1.0, the configuration file now points to a folder rather than a single audio file. This folder must be structured like follows:
 ```
 HITSOUNDS FOLDER
 ├───Fail
 ├───Hit
 ├───Miss
 └───Win
 ```
 You don't have to create all the folders above, the mod will automatically detect if one is missing and skip it.
 
 The game will pick up every sound in every directory and play it according to the folders they are placed in, and randomly (e.g: if you have 2 hitsounds, each one of them has a 50% chance of being played on a note tick)
