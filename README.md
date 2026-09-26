# Barn Busters

You're a farmer whose rapscallions have escaped, and your job is to harvest your herd as fast as you can. It's a physics based tower defense where you place traps and obstacles to knock the animals off the map in as few rounds as possible.

- Play: [itch.io](https://unitedfailures.itch.io/barn-busters)
- Made: January 2023 for Ludum Dare 52
- Team: [@mfclinton](https://github.com/mfclinton) (programming), [@DavidKoleczek](https://github.com/DavidKoleczek) (programming)
- Engine: Unity, C#

This is an export of a private repo with only the code we wrote. Art, audio, the Unity project files, and third party plugins aren't included. The history is squashed into one commit.

## What I built

- The ragdolls. Animals switch to ragdoll physics when a trap hits them, and the impact effects come from a pool.
- The round loop. Each round the animals path around your traps to a goal area using the [A* Pathfinding Project](https://arongranberg.com/astar/). Knock one off the map and it goes back to the start after a delay. Animals still on the field when time runs out get harvested, the rest come back next round, and the game ends when none are left.
- Cash and the shop. Your cash changes each round and whenever an animal falls, reaches the goal, or gets harvested, with every amount set in one config asset. You spend it on traps.
- Digging. You can delete up to 14 grass tiles per game to open holes, a limit I added as a custom rule for the [Easy Build System](https://assetstore.unity.com/packages/templates/systems/easy-build-system-modular-building-system-45394) asset.
- A performance pass for the browser build. Each floor tile hides its sides while all four neighbors are there and shows them again when one is deleted. I also added LODs, baked occlusion culling, and a low graphics setting that turns off the grass.
- A map generator tool for development, in `Scripts/Tools/`. One button builds the tile map from weighted tile prefabs, then fits the Cinemachine camera track and the pathfinding grid to it. I also used Cinemachine to shoot a gameplay trailer from several angles.
