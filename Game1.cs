using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyNewEngine.Entities;
using MyNewEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;
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
    List<Enemy> _enemies = new List<Enemy>();
    private Texture2D heartTexture;

    //collectibles
     List<Collectible> _collectibles = new List<Collectible>();
    private Texture2D _coinTexture;
    private Texture2D _manaTexture;

    //font for text
    SpriteFont font1;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        //platforms from ldtk
        levelManager = new LevelManager();
        levelManager.LoadLevel("Assets/World.ldtk");
        
        foreach(Rectangle solidBox in levelManager.SolidColliders)
        {
            Entity platform = new Entity();
            platform.Position = new Vector2(solidBox.X, solidBox.Y);
            platform.Size = new Vector2(solidBox.Width, solidBox.Height);
            _platforms.Add(platform);
        }

        //player
        _player = new Player();
        _player.Position = new Vector2(-2, 96);

        _target = new Entity();
        _target.Position = new Vector2(400, 300);
        _target.tint = Color.Red;

        //enemy
        
        Enemy premierEnnemi = new Enemy();
        premierEnnemi.Position = new Vector2(100, 100);
        _enemies.Add(premierEnnemi);
        

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
        heartTexture = Texture2D.FromFile(GraphicsDevice, "Assets/Heart.png");

        _player.LoadContent(GraphicsDevice);
        Art.Load(GraphicsDevice);
        Texture2D toasterTex = Texture2D.FromFile(GraphicsDevice, "Assets/Toaster.png");
        foreach (Enemy enemy in _enemies)
        {
            enemy.LoadContent(toasterTex);
        }
        // Charge tes images (ajoute les noms exacts de tes fichiers png)
        _coinTexture = Texture2D.FromFile(GraphicsDevice, "Assets/Coin.png");
        _manaTexture = Texture2D.FromFile(GraphicsDevice, "Assets/Mana.png");

        // Faisons apparaître un objet de chaque type pour tester :
        _collectibles.Add(new Collectible(new Vector2(190, 125), CollectibleType.Coin, _coinTexture));
        _collectibles.Add(new Collectible(new Vector2(240, 100), CollectibleType.Mana, _manaTexture));
        _collectibles.Add(new Collectible(new Vector2(300, 75), CollectibleType.Health, heartTexture));

        //font for text
        font1 = Content.Load<SpriteFont>("MyFont");
    }

    protected override void Update(GameTime gameTime)
    {
        // 1. Always update your input first!
        Input.Update();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _player.Update(dt, _platforms);
        foreach(Enemy enemy in _enemies)
        {
            enemy.Update(dt, _platforms);
        }
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
        // --- 3. COLLISIONS: BALLES vs ENNEMIS ---
        // On lit la liste des balles à l'envers (important pour pouvoir les supprimer sans faire crasher le jeu)
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            bool bulletHit = false;

            // On crée un faux rectangle pour la balle actuelle (car la classe Bullet n'a pas encore de Bounds)
            Rectangle bulletRect = new Rectangle((int)_bullets[i].Position.X, (int)_bullets[i].Position.Y, 10, 5);

            // On vérifie cette balle contre TOUS les ennemis
            for (int j = _enemies.Count - 1; j >= 0; j--)
            {
                if (bulletRect.Intersects(_enemies[j].Bounds))
                {
                    // BOOM ! La balle touche le grille-pain !
                    _enemies.RemoveAt(j);
                    bulletHit = true;     
                    break;                
                }
            }

            if (bulletHit)
            {
                _bullets.RemoveAt(i);
            }
        }
        //collisions with collectible
        for (int i = _collectibles.Count - 1; i >= 0; i--)
        {
            Rectangle collectibleRect = new Rectangle((int)_collectibles[i].Position.X, (int)_collectibles[i].Position.Y, 10, 5);
            if (_player.Bounds.Intersects(collectibleRect))
            {
                _collectibles[i].applyEffect(_player);
                _collectibles.RemoveAt(i);

            }

        }
        _camera.Follow(_player.Position, 800, 480);

        foreach(Enemy enemy in _enemies)
        {
            if (_player.Bounds.Intersects(enemy.Bounds)){
                _player.TakeDamage(1);
            }
        }
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
        System.Diagnostics.Debug.WriteLine(_player.Position);
        foreach (Enemy enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }
        foreach(Collectible collectible in _collectibles)   {
            collectible.Draw(_spriteBatch);
        }

        // 6. Push the drawing to the monitor
        _spriteBatch.End();

        _spriteBatch.Begin();
        for(int i = 0; i < _player.CurrentHealth; i++)
        {
            _spriteBatch.Draw(heartTexture, new Vector2(10 + i * 34, 10), Color.White);
        }

        Vector2 manaBarPosition = new Vector2(15, 50);
        int maxBarWidth = 200;
        int barHeight = 20;

        Rectangle manaBg = new Rectangle((int)manaBarPosition.X, (int)manaBarPosition.Y, (int)(maxBarWidth * (_player.currentMana / _player.maxMana)), barHeight);
        _spriteBatch.Draw(_debugTexture, manaBg, Color.Blue);
        int curretBarWidth = (int)(maxBarWidth * (_player.currentMana / _player.maxMana));
        Rectangle manaFg = new Rectangle((int)manaBarPosition.X, (int)manaBarPosition.Y, curretBarWidth, barHeight);
        _spriteBatch.Draw(_debugTexture, manaFg, Color.Cyan);


        Vector2 coinSymbolPosition = new Vector2(15, 80);
        _spriteBatch.Draw(_coinTexture, coinSymbolPosition, Color.White);
        _spriteBatch.DrawString(font1, "x " + _player.coins, new Vector2(50, 80), Color.White);


        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
}
