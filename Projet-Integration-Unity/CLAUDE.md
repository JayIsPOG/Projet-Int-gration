# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

2D top-down puzzle/adventure game built with Unity 2021.3.23f1, using URP (Universal Render Pipeline) and Physics2D. The game features enemy AI with A* pathfinding and light-based puzzle mechanics.

## Development Commands

All builds and scene testing are done through the **Unity Editor** — there is no CLI build script. Open the project in Unity 2021.3.23f1 and use the Editor's Play button to test.

For C# editing, open the `.sln` file with Visual Studio or Rider, or use VS Code with the C# extension (configured in `.vscode/settings.json`).

## Architecture

### Scenes
- `1.unity` — Main level (overworld with enemies and a door transition)
- `Puzzle 1.unity` — Light/mirror puzzle level
- `Puzzle 2.unity` — Second puzzle level
- `dice_test.unity` — Isolated test scene for `burger.cs` (3D dice roller)

Scene transitions are handled by `LevelDoor.cs` (E key to enter when door is open).

### Scripts (`Assets/Scripts/`)

**Movement & AI:**
- `PlayerMovement.cs` — WASD velocity-based movement, sprite flipping, animation
- `EnemyMovement.cs` — Enemy state machine (`Chasing` / `Attacking`) using A* pathfinding; enemies flank the player using a configurable `sideOffsetX`
- `Grid_Astar.cs` — `GridManager` singleton that builds a walkability grid from tilemap bounds (37×29 cells) and runs A* (orthogonal cost=10, diagonal cost=14)
- `Node.cs` — A* node data structure
- `Spawn.cs` — Spawns enemies at scene start

**Puzzle Elements (`Assets/Scripts/Puzzle Elements/`):**
- `Light.cs` — Emits Physics2D raycasts downward; chains to mirrors and receivers
- `Mirror.cs` — Reflects incoming light rays; can chain to other mirrors/receivers
- `LightReceiver.cs` — Boolean flag set when light hits it (used to gate doors/mechanisms)
- `Pushable.cs` — Player can push/drag this object using middle mouse button

**Rendering & Utilities:**
- `SpriteSorting.cs` — Sets `sortingOrder` each frame based on Y position for proper 2D depth
- `burger.cs` — Standalone 3D cube rolling simulation using custom quaternion math (not part of main game flow)

### Key Systems

**A* Pathfinding:**
`Grid_Astar` builds its grid from the `Tilemap` component on game start. A cell is walkable if no tile exists at that position (i.e., open floor). Enemies call `Grid_Astar.instance.FindPath()` to get a waypoint list.

**Light Puzzle Chain:**
`Light` → (optional) `Mirror` chain → `LightReceiver`. All interactions use `Physics2D.Raycast` with layer masks. Mirrors reflect using angle math between the incoming ray and the mirror's normal.

**Rendering Layers (order matters):**
`Ground` → `Default` → `Player` → `UI`

### Physics Layers
- Layer 3: Ground
- Layer 6: Player
- Layer 7: IgnorePlayer
- Layer 8: Air

### Key Packages
- `com.unity.feature.2d` — Full 2D support (Tilemaps, Physics2D)
- `com.unity.cinemachine` (2.8.9) — Camera
- `com.unity.render-pipelines.universal` (12.1.11) — URP
- `com.unity.visualscripting` (1.8.0) — Visual scripting (available but not the primary approach)
