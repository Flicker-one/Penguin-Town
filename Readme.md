# Penguin Town - Game README
## Basic Information
- **Game Name**: Penguin Town
- **Genre**: 2.5D Survival Real-Time Strategy Game
- **Platform**: Windows (PC)
- **Engine**: Unity(2D)
- **Author**: Cai Yiren (3036445763)

### Story Background
The rampant Beggar ticks (Bidens Pilosa) have invaded Penguin Town and trapped all residents. As the town’s greatest Magician, you must defeat the invasive Beggar ticks to save everyone.

### Game Map & Core Rules
- Map size: **4×4 tilemap**
- Each tile represents a building (purified / polluted state)

### Win Conditions
1. Purify **any full row or column** of buildings
2. Directly defeat the boss with weapons/items

### Lose Condition
Any row or column becomes **fully polluted** → Town destroyed → Game Over

### Main Tasks
- Collect items
- Evade enemy attacks
- Complete building missions to purify areas
- Defeat the boss (direct fight or row/column purification)

---

## Game Mechanics
### UI
- Start Game
- Start Tutorial
- Settings
- Dictionary

### View
- Isometric view

### Building System (Tilemap)
- Implemented via Unity Tilemap
- Each building has unique mission interface
- Purified/polluted state determines game result
[Building recipes](recipe_dictionary.png)

### Player System
- Health system
- Weapon & item system
- Movement with inertia
- Rolling skill

### Enemy System
- Two types: Melee / Remote attack
- Random spawn
- Pathfinding
- Drop items on death

### Boss System
- Summon enemies at intervals
- Attack when player approaches
- Randomly pollute buildings at intervals

### Audio
- Background music
- Sound effects

---

## Game Aesthetics & Technology
### Aesthetics
- Isometric projection
- Lively animations
- Rich sound effects

### Development Tools
- Unity 3D (core development)
- Blender (3D decorations)
- Nano Banana Pro (images)
- Adobe Photoshop (images)
- Mixamo (animations)
- Soundy (sound effects)
- Figma (UI design)

---

## Extendable Features
- Multiplayer mode
- Extendable maps & levels
- More memes in the game

---
