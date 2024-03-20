# circloO-patcher
 
circloO patcher is a tool and set of mods for the game [circloO by Florian van Strien](https://store.steampowered.com/app/2195630/circloO/) that currently takes the form of a script for [UndertaleModTool bleeding edge](https://github.com/UnderminersTeam/UndertaleModTool/releases/tag/bleeding-edge).

## Running the tool/modding your game
1. Download [UndertaleModTool's bleeding edge version](https://github.com/UnderminersTeam/UndertaleModTool/releases/tag/bleeding-edge). You _must_ use bleeding edge as the latest stable release won't open circloO.
2. Download the script called `cirQOL.csx` from the latest release on the [releases page](https://github.com/DT-is-not-available/circloO-patcher/releases).
3. Open UndertaleModTool and go to "Scripts" > "Run other script..." and select `cirQOL.csx`. The script will try to automatically find and open the game, but if it can't you may need to open the `data.win` for your game manually.

The rest of the process should be fairly self explanatory.

## Adding custom patches/mods

Currently the only patches you can apply are a collection of patches called cirQOL made specifically for circloO. If you want to add your own patches to the game, you will need to edit the source code and build the tool yourself.

## Building the tool

Since this is essentially just a script for UndertaleModTool, the build process is fairly simple. You will need [Node.js](https://nodejs.org/en/download) (The minimum tested version is v20.11.1 but the build script should in theory run just fine on v16.20.2 or later). Building the script is as simple as running `node build` in the main directory. A few files to take note of:
- `build.js` is the build script, essentially a custom written preprocessor I wrote in an afternoon that just preprocesses C# script `#load` directives.
- `src/` is where the source for the actual `cirQOL.csx` script is located.
- `project.json` contains configuration for `build.js`. It is extremely simple, and there is no reason to complicate it further.

## Contributing

If you would like to contribute, please open a pull request. Additions to `patches.csx` are welcome, 

## TODOS

I currently have 2 separate todo lists: one for the tool and one for cirQOL. Each todo list is in order of highest priority to lowest priority, and will be updated as things get completed. Each TODO list also comes with a wishlist, which is simply a list of things I would like to do with the tool, but given the amount of time they would take right now I'm not expecting to complete. Help on these items would be greatly appreciated if I ever get to them.

### Patcher TODO list

- [ ] Self contain all logic for the tool inside of the Form class like a real C# application
- [ ] Clean up the source files to make them managable
- [ ] Update the patch configuration checkbox list to use custom components and a custom renderer, so that collapsible categories can be added
- [ ] Replace the 'required patch' option with patch dependancies (for when loading one patch requires loading another)
- [ ] Create a simple file format for creating and storing custom patches and categories
- [ ] Remove the built-in patches and put them in their own repository, and rename the tool (at this point cirQOL will inherit the version number, and the new tool will start at v1.0.0)

### cirQOL TODO list

- [ ] Modify the patches that replace assembly to use the matching patterns instead of actual label numbers, that way the chance of patches breaking on a game update is minimal
- [ ] Figure out how to automatically define locals for code entries so that local variables can be used
- [ ] Remove as many global variables from the patches as possible
- [ ] Move the 'run level from clipboard', 'copy level from clipboard', and 'open level in editor' to Ctrl+Shift+V (in menus), Ctrl+Shift+C (in a level), and F5 (in a level) respectively
- [ ] Make the 'open level in editor' hotkey automatically load it in the correct editor save if it is a local level so that you dont wind up with hundreds of saves
- [ ] Make Ctrl+Shift+V (in the editor) load the level from your clipboard and Ctrl+Shift+C (in the editor) export to clipboard
- [ ] Make it possible to set values to nan and inf again, as well as `5e-324` (the smallest floating point value possible without being 0)
- [ ] Change the display of 
- [ ] Get rid of the currently required `cirQOL` 'patch', move the startup notification to the F1 menu patch
- [ ] Fix square level drawing when 'Start Full' is enabled
- [ ] Make level shape and gravity control mode import correctly back into the editor (requires editing the switch-case in assembly, it's probably easy I'm just putting it off)
- [ ] Add a menu ingame for changing mod settings like skipping menu animations, and cut down on the number of hotkeys involved so things that need to be hotkeys can be added without clashing
- [ ] Draw the circle 'load zone' since, regardless of the level shape, the game automatically loads and draws things based on the level radius
- [ ] Increase the precision for all values in the editor
- [ ] Fix the incorrect text offset in the 'give text inputs a flashing underscore' patch

### Patcher wishlist

- [ ] Make the tool an actual application instead of an UndertaleModTool script, use UndertaleModLib instead.
- [ ] Create an UndertaleModTool script that will be able to record changes made to the `data.win` and generate a patch file automatically to help ease the modding process.
- [ ] Make the tool capable of undoing patches based on the patch file format without having to create and load backups of the vanilla game

### cirQOL wishlist

- [ ] Make heart shaped levels draw correctly in the editor
- [ ] Change outside of the level to be the same color as the background and draw a level border instead (so that you can see collectables and other things outside the level border)
- [ ] Add UI ingame for setting gravity control mode and level shape, remove those hotkeys
- [ ] Add a restitution option to the player
- [ ] Add the option to make the player square
- [ ] Add the 'make object use improved physics' option to all moving objects, not just the player
- [ ] Make whether the collectable activates on touching an object a toggleable option
- [ ] Add a position editor for rope offsets
- [ ] Add a new 'no collision' connection (disables collision between the two objects connected)
- [ ] Add a new 'weld objects' connection (similar to glue but not as rigid)
- [ ] Add a property to every level editor object called a 'tag', which is visible on hover or when ALT is pressed
- [ ] Add a menu to locate objects by tag
