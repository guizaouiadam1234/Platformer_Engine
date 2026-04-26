using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyNewEngine.Entities;
using MyNewEngine.Graphics;
using System;
using System.Collections.Generic;
namespace MyNewEngine;
using ImGuiNET;
using ImGuiNET.SampleProgram.XNA;
using MyNewEngine.Graphics;

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

    //gui
    private ImGuiRenderer _imGuiRenderer;
    private RenderTarget2D _sceneRenderTarget;
    private IntPtr _sceneTextureId;
    private Entity _selectedEntity = null;

    //font for text
    SpriteFont font1;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1280; 
        _graphics.PreferredBackBufferHeight = 720; 
        _graphics.ApplyChanges(); 
    }

    protected override void Initialize()
    {
        //scene render target for imGui
        _sceneRenderTarget = new RenderTarget2D(GraphicsDevice, 1280, 720);
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
        //gui
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();

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
        // scene view
        GraphicsDevice.SetRenderTarget(_sceneRenderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // --- DESSIN DU MONDE (AVEC CAMERA) ---
        _spriteBatch.Begin(transformMatrix: _camera.Transform);

        if (levelManager != null && levelManager.VisualTiles != null)
        {
            foreach (LevelTile tile in levelManager.VisualTiles)
            {
                _spriteBatch.Draw(_tilesetTexture, tile.DestinationRect, tile.SourceRect, Color.White);
            }
        }
        foreach (Bullet bullet in _bullets)
        {
            Rectangle bulletRect = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, 10, 5);
            _spriteBatch.Draw(_debugTexture, bulletRect, Color.Yellow);
        }
        _player.Draw(_spriteBatch);
        foreach (Enemy enemy in _enemies) { enemy.Draw(_spriteBatch); }
        foreach (Collectible collectible in _collectibles) { collectible.Draw(_spriteBatch); }

        // highlight selected entity
        if (_selectedEntity != null)
        {
            Rectangle highlight = new Rectangle(
                (int)_selectedEntity.Position.X - 2,
                (int)_selectedEntity.Position.Y - 2,
                (int)_selectedEntity.Size.X + 4,
                (int)_selectedEntity.Size.Y + 4);
            _spriteBatch.Draw(_debugTexture, new Rectangle(highlight.X, highlight.Y, highlight.Width, 2), Color.Yellow);
            _spriteBatch.Draw(_debugTexture, new Rectangle(highlight.X, highlight.Bottom - 2, highlight.Width, 2), Color.Yellow);
            _spriteBatch.Draw(_debugTexture, new Rectangle(highlight.X, highlight.Y, 2, highlight.Height), Color.Yellow);
            _spriteBatch.Draw(_debugTexture, new Rectangle(highlight.Right - 2, highlight.Y, 2, highlight.Height), Color.Yellow);
        }
        _spriteBatch.End();

        _spriteBatch.Begin();
        for (int i = 0; i < _player.CurrentHealth; i++)
        {
            _spriteBatch.Draw(heartTexture, new Vector2(10 + i * 34, 10), Color.White);
        }
        Vector2 manaBarPosition = new Vector2(15, 50);
        int maxBarWidth = 200; int barHeight = 20;
        Rectangle manaBg = new Rectangle((int)manaBarPosition.X, (int)manaBarPosition.Y, (int)(maxBarWidth * (_player.currentMana / _player.maxMana)), barHeight);
        _spriteBatch.Draw(_debugTexture, manaBg, Color.Blue);
        int curretBarWidth = (int)(maxBarWidth * (_player.currentMana / _player.maxMana));
        Rectangle manaFg = new Rectangle((int)manaBarPosition.X, (int)manaBarPosition.Y, curretBarWidth, barHeight);
        _spriteBatch.Draw(_debugTexture, manaFg, Color.Cyan);

        Vector2 coinSymbolPosition = new Vector2(15, 80);
        _spriteBatch.Draw(_coinTexture, coinSymbolPosition, Color.White);
        _spriteBatch.DrawString(font1, "x " + _player.coins, new Vector2(50, 80), Color.White);
        _spriteBatch.End();


        // gui draw
        GraphicsDevice.SetRenderTarget(null); 
        GraphicsDevice.Clear(new Color(40, 44, 52)); // gray background for the GUI

        _imGuiRenderer.BeforeLayout(gameTime);
        ImGui.DockSpaceOverViewport();

        //window 1 : inspector
        ImGui.Begin("Inspecteur");
        ImGui.SliderFloat("Camera Zoom", ref _camera.zoom, 0.25f, 3f);
        ImGui.Separator();
        if (_selectedEntity == null)
        {
            ImGui.TextDisabled("Nothing selected");
        }
        else if (_selectedEntity == _player)
        {
            ImGui.Text("--- Player ---");
            ImGui.SliderFloat("Vitesse", ref _player.Speed, 50f, 1000f);
            ImGui.SliderFloat("Force de Saut", ref _player.JumpForce, -1500f, -200f);
            ImGui.SliderFloat("Gravite", ref _player.Gravity, 100f, 3000f);
            ImGui.Separator();
            ImGui.Text("Pieces recoltees : " + _player.coins);
            if (ImGui.Button("Soigner")) { _player.CurrentHealth = _player.MaxHealth; }
        }
        else if (_selectedEntity is Enemy selectedEnemy)
        {
            ImGui.Text("--- Enemy ---");
            ImGui.SliderFloat("Speed", ref selectedEnemy.Speed, 0f, 500f);
            ImGui.SliderFloat("Gravity", ref selectedEnemy.Gravity, 100f, 3000f);
            ImGui.Text("Position: (" + (int)selectedEnemy.Position.X + ", " + (int)selectedEnemy.Position.Y + ")");
            ImGui.Separator();
            if (ImGui.Button("Remove Enemy"))
            {
                _enemies.Remove(selectedEnemy);
                _selectedEntity = null;
            }
        }
        else if (_selectedEntity is Collectible selectedCollectible)
        {
            ImGui.Text("--- Collectible ---");
            ImGui.Text("Type: " + selectedCollectible.Type.ToString());
            ImGui.Text("Position: (" + (int)selectedCollectible.Position.X + ", " + (int)selectedCollectible.Position.Y + ")");
        }
        ImGui.Separator();
        if (ImGui.Button("Add Enemy"))
        {
            Enemy newEnemy = new Enemy();
            newEnemy.Position = new Vector2(_player.Position.X + 100, _player.Position.Y);
            Texture2D toasterTex = Texture2D.FromFile(GraphicsDevice, "Assets/Toaster.png");
            newEnemy.LoadContent(toasterTex);
            _enemies.Add(newEnemy);
        }
        ImGui.End();

        // window 2 : scene view
        ImGui.Begin("Scene View");
        _sceneTextureId = _imGuiRenderer.BindTexture(_sceneRenderTarget);
        ImGui.Image(_sceneTextureId, new System.Numerics.Vector2(800, 480));
        ImGui.End();

        //window 3 : scene hierarchy
        ImGui.Begin("Scene Hierarchy");

        if (ImGui.Selectable("Player", _selectedEntity == _player))
            _selectedEntity = _player;

        ImGui.Separator();

        if (ImGui.TreeNode("Enemies (" + _enemies.Count + ")"))
        {
            foreach (Enemy enemy in _enemies)
            {
                string label = "Enemy (" + (int)enemy.Position.X + ", " + (int)enemy.Position.Y + ")";
                if (ImGui.Selectable(label, _selectedEntity == enemy))
                    _selectedEntity = enemy;
            }
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Collectibles (" + _collectibles.Count + ")"))
        {
            for (int i = 0; i < _collectibles.Count; i++)
            {
                string label = _collectibles[i].Type.ToString() + " #" + i;
                if (ImGui.Selectable(label, _selectedEntity == _collectibles[i]))
                    _selectedEntity = _collectibles[i];
            }
            ImGui.TreePop();
        }

        ImGui.End();

        _imGuiRenderer.AfterLayout();
        base.Draw(gameTime);
    }

}
