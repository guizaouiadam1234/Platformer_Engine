using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyNewEngine.Entities;
using MyNewEngine.Graphics;
using System.Collections.Generic;
namespace MyNewEngine;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Vector2 _playerPosition;
    private Texture2D _whiteTexture;
    Player _player;
    Entity _target;
    List<Entity> _platforms = new List<Entity>();
    List <Bullet> _bullets = new List<Bullet> ();
    float _lastDirection = 1f; // Default to facing right
    Camera2D _camera;
    private Texture2D _debugTexture;
    private LevelManager levelManager;
    private Texture2D _tilesetTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Create a big floor
        levelManager = new LevelManager();
        levelManager.LoadLevel("Assets/World.ldtk");
        
        foreach(Rectangle solidBox in levelManager.SolidColliders)
        {
            Entity platform = new Entity();
            platform.Position = new Vector2(solidBox.X, solidBox.Y);
            platform.Size = new Vector2(solidBox.Width, solidBox.Height);
            _platforms.Add(platform);
        }

        _player = new Player();
        _player.Position = new Vector2(100, 100);

        _target = new Entity();
        _target.Position = new Vector2(400, 300);
        _target.tint = Color.Red;

        //add camera
        _camera = new Camera2D();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        _debugTexture.SetData(new[] { Color.White });
        _tilesetTexture = Texture2D.FromFile(GraphicsDevice, "Assets/Grass.png");
        Art.Load(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        // 1. Always update your input first!
        Input.Update();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _player.Update(dt, _platforms);
        // 1. Shooting Logic (Press F to fire)
        if (Input.IsKeyDown(Keys.Left))
        {
            _lastDirection = -1f;
        }
        else if (Input.IsKeyDown(Keys.Right))
        {
            _lastDirection = 1f;
        }
        if (Input.HasBeenPressed(Keys.W))
        {
            // Determine direction based on player movement or a default
            float bulletSpeed = 600f * _lastDirection;
            Vector2 bulletVel = new Vector2(bulletSpeed, 0); // Shoots right

            // If you want to shoot left when moving left:
            // if (playerFacingLeft) bulletVel.X = -600f;

            _bullets.Add(new Bullet(_player.Position, bulletVel));
        }

        // 2. Update Bullets
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            _bullets[i].Update(dt, _platforms);

            // Remove bullet if it's "dead"
            if (_bullets[i].LifeSpan <= 0)
            {
                _bullets.RemoveAt(i);
            }
        }
        _camera.Follow(_player.Position, 800, 480);
    }

    protected override void Draw(GameTime gameTime)
    {
        // 1. Wipe the screen clean every frame
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // 2. Open the paintbrush (NO CAMERA YET - just raw drawing)
        _spriteBatch.Begin(transformMatrix : _camera.Transform);

        // --- DRAW THE ART ---
        if (levelManager != null && levelManager.VisualTiles != null)
        {
            foreach (LevelTile tile in levelManager.VisualTiles)
            {
               //draw grass tile
                _spriteBatch.Draw(_tilesetTexture, tile.DestinationRect, tile.SourceRect, Color.White);
            }
        }

        

        
        foreach(Bullet bullet in _bullets){
            Rectangle bulletRect  = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, 10, 5);
            _spriteBatch.Draw(_debugTexture, bulletRect, Color.Yellow);
        }
        _player.Draw(_spriteBatch);

        // 6. Push the drawing to the monitor
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
