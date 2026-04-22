# MyNewEngine

A 2D game engine and platformer built with **MonoGame** and **.NET 9**, featuring physics-based player movement, tile-based level loading via **LDtk**, a 2D camera system, parallax backgrounds, and a bullet/shooting system.

---

## Features

- **Entity System** — Base `Entity` class with position, tint, size and bounds. Easily extendable for any game object.
- **Player Controller** — Smooth platformer movement with:
  - Horizontal movement & gravity
  - Jump buffering (0.15s) and coyote time (0.15s)
  - Axis-separated AABB collision resolution
- **Bullet System** — Shootable projectiles managed as entities.
- **LDtk Level Loading** — Loads `.ldtk` world files, parses `Collisions` (IntGrid) and `Tiles` layers to generate solid colliders and visual tiles at runtime.
- **Camera2D** — Smooth follow camera with zoom support and a `Matrix` transform for `SpriteBatch`.
- **Parallax Layers** — Multi-layer parallax scrolling background renderer with configurable scroll factors.
- **Input Manager** — Centralized keyboard input with `IsKeyDown` and `HasBeenPressed` helpers.

---

## Project Structure

```
MyNewEngine/
├── Assets/                  # Game assets (LDtk world, tilesets, sprites)
│   ├── World.ldtk
│   └── Grass.png
├── Entities/
│   ├── Entity.cs            # Base entity class
│   ├── Player.cs            # Platformer player controller
│   └── Bullet.cs            # Projectile entity
├── Graphics/
│   ├── Art.cs               # Centralized texture/asset loader
│   ├── Camera2D.cs          # 2D follow camera
│   └── ParallaxLayer.cs     # Parallax background layer
├── Input/
│   └── Input.cs             # Keyboard input manager
├── Levels/
│   ├── LdtkData.cs          # LDtk JSON deserialization models
│   └── LevelManager.cs      # Level loader (colliders + visual tiles)
├── Game1.cs                 # Main game loop
├── Program.cs               # Entry point
└── MyNewEngine.csproj
```

---

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MonoGame 3.8+](https://www.monogame.net/)
- [LDtk](https://ldtk.io/) *(for editing levels)*

---

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd MyNewEngine
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the project**
   ```bash
   dotnet run
   ```

---

## Controls

| Key         | Action        |
|-------------|---------------|
| `Left`      | Move left     |
| `Right`     | Move right    |
| `C`         | Jump          |
| `F`         | Shoot         |

---

## Level Editing

Levels are created with [LDtk](https://ldtk.io/) and saved as `.ldtk` files under `Assets/`.

The `LevelManager` reads:
- **`Collisions`** IntGrid layer → generates solid `Rectangle` colliders used for physics.
- **`Tiles`** layer → generates `LevelTile` structs (source + destination rects) rendered against the tileset texture (`Grass.png`).

To add a new level, create a level in LDtk and update the path passed to `levelManager.LoadLevel(...)` in `Game1.cs`.

---

## License

This project is open source. Feel free to use it as a learning resource or a starting point for your own MonoGame projects.
