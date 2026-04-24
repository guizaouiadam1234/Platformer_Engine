# MyNewEngine

A small 2D platformer project built with **MonoGame DesktopGL** and **.NET 9**. The current game setup includes a player with movement, jumping, dashing, shooting, a following camera, LDtk-based level loading, a simple enemy, and a basic HUD for health and mana.

---

## Current Features

- **Player controller**
  - Left/right movement
  - Gravity-based jumping
  - Jump buffering and coyote time
  - Dash ability with mana cost and cooldown
- **Combat**
  - Bullets fire in the last faced horizontal direction
  - Bullets are removed on wall hit or enemy hit
- **Enemy**
  - Simple walking enemy that patrols and turns around on collision
  - Contact damage with temporary player invincibility
- **HUD**
  - Heart-based health display
  - Mana bar with regeneration
- **Level loading**
  - Loads `Assets\World.ldtk`
  - Reads the first LDtk level in the file
  - Uses the `Collisions` layer for solid tiles
  - Uses the `Visuals` layer for rendered tiles
- **Rendering helpers**
  - `Camera2D` follow camera
  - Sprite animation helper
  - Base `Entity` class with bounds-based collision

---

## Tech Stack

- **.NET 9**
- **MonoGame.Framework.DesktopGL 3.8.\***
- **MonoGame.Content.Builder.Task 3.8.\***
- **MonoGame.Extended 5.4.0**

---

## Project Structure

```text
MyNewEngine/
|-- Assets/                    # PNG sprites, LDtk world, source art files
|-- Content/
|   `-- Content.mgcb           # MonoGame content project
|-- Entities/
|   |-- Bullet.cs
|   |-- Enemy.cs
|   |-- Entity.cs
|   `-- Player.cs
|-- Graphics/
|   |-- Animation.cs
|   |-- Art.cs
|   |-- Camera2D.cs
|   `-- ParallaxLayer.cs
|-- Input/
|   `-- Input.cs
|-- Levels/
|   |-- LdtkData.cs
|   `-- LevelManager.cs
|-- Game1.cs
|-- Program.cs
`-- MyNewEngine.csproj
```

> `ParallaxLayer.cs` exists in the project, but it is not currently used by `Game1`.

---

## Assets Used at Runtime

The game currently loads textures directly from the `Assets\` folder with `Texture2D.FromFile(...)`.

- `Assets\BananaIdle.png`
- `Assets\BananaRun.png`
- `Assets\BananaDash.png`
- `Assets\Toaster.png`
- `Assets\Grass.png`
- `Assets\Heart.png`
- `Assets\World.ldtk`

These files are configured in the project file to copy to the output directory.

---

## Getting Started

1. Clone the repository.
2. Restore packages:

   ```powershell
   dotnet restore
   ```

3. Run the game:

   ```powershell
   dotnet run --project .\MyNewEngine.csproj
   ```

---

## Controls

| Key | Action |
| --- | --- |
| `Left Arrow` | Move left |
| `Right Arrow` | Move right |
| `C` | Jump |
| `X` | Dash |
| `W` | Shoot |

---

## LDtk Notes

- The level file path is hardcoded in `Game1.cs` as `Assets/World.ldtk`.
- `LevelManager` currently loads only the first level in the LDtk world.
- Collision tiles come from the `Collisions` int grid.
- Rendered tiles come from the `Visuals` layer and use `Assets\Grass.png`.

If you rename the LDtk file or change layer names, you will need to update the code.

---

## Current Gameplay Notes

- The player starts at `(-2, 96)`.
- One enemy is spawned near the player at startup.
- If the player loses all health, they are reset to full health and moved back to `(-2, 96)`.

---

## License

There is currently no license file in this repository.
