using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyNewEngine.Graphics;
using System;
using System.Collections.Generic;

namespace MyNewEngine.Entities
{
    public class Player : Entity

    {
        public float speed = 400f;
        public Vector2 Velocity;
        public float Speed = 300f;
        public float Gravity = 1200f; // Pixels per second squared
        public float JumpForce = -600f; // Negative because UP is smaller Y
        private bool _isGrounded = false;
        public float DashSpeed = 1000f;
        private bool _isDashing = false;
        private float _dashTimer = 0f;
        private float _dashCooldownTimer = 0f;
        private const float DASH_DURATION = 0.15f; // Le dash dure 0.15 secondes
        private const float DASH_COOLDOWN = 0.2f; // Il faut attendre 0.5 seconde avant de recommencer
        private int _facingDirection = 1;

        //mana
        public float maxMana = 100f;
        public float currentMana = 100f;
        public float dashManaCost = 35f;
        public int manaRegen = 15;

        //money
        public int coins = 0;

        //jump buffer
        private float _jumpBufferTimer = 0f;
        private float _coyoteTimer = 0f;
        private const float JUMP_BUFFER_TIME = 0.15f;
        private const float COYOTE_TIME = 0.15f;

        //animation
        private Texture2D _idleTexture;
        private Animation _idleAnim;
        private Animation _runAnim;
        private Animation _currentAnim;
        private Animation _dashAnim;
        private SpriteEffects _flip = SpriteEffects.None;

        //health and invincibility
        public int MaxHealth = 5;
        public int CurrentHealth;
        private float _invincibleTimer = 0f;
        private float INVINCIBLE_DURATION = 1.0f;

        public override void Update(float dt, List<Entity> platforms)
        {
            // --- 1. TIMERS ---
            _jumpBufferTimer -= dt;
            _coyoteTimer -= dt;
            _dashCooldownTimer -= dt; // On diminue le temps de recharge du dash
            if (_invincibleTimer > 0) _invincibleTimer -= dt;

            if (_isGrounded) _coyoteTimer = COYOTE_TIME;

            //mana regen
            if (currentMana < maxMana)
            {
                currentMana += manaRegen * dt;
                if (currentMana > maxMana) currentMana = maxMana;
            }

            //direction faced
            if (Input.IsKeyDown(Keys.Left)) _facingDirection = -1;
            if (Input.IsKeyDown(Keys.Right)) _facingDirection = 1;

            // x for dash, c for jump, z for bullets
            if (Input.HasBeenPressed(Keys.X) && _dashCooldownTimer <= 0 && !_isDashing && currentMana >= dashManaCost)
            {
                _isDashing = true;
                _dashTimer = DASH_DURATION;
                currentMana -= dashManaCost;
            }

            
            if (_isDashing)
            {
                // PENDANT LE DASH : On va super vite, et on annule la gravité !
                Velocity.X = DashSpeed * _facingDirection;
                Velocity.Y = 0;

                _dashTimer -= dt;
                if (_dashTimer <= 0)
                {
                    _isDashing = false; // Fin du dash
                    _dashCooldownTimer = DASH_COOLDOWN; // Lancement du temps de recharge
                }
            }
            else
            {
                // MOUVEMENT NORMAL (Quand on ne dash pas)
                Velocity.X = 0;
                if (Input.IsKeyDown(Keys.Left)) Velocity.X = -Speed;
                if (Input.IsKeyDown(Keys.Right)) Velocity.X = Speed;

                // GRAVITÉ NORMALE
                Velocity.Y += Gravity * dt;

                // JUMP NORMAL
                if (Input.HasBeenPressed(Keys.C)) _jumpBufferTimer = JUMP_BUFFER_TIME;

                if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
                {
                    Velocity.Y = JumpForce;
                    _isGrounded = false;
                    _jumpBufferTimer = 0;
                    _coyoteTimer = 0;
                }
            }

            // --- 5. GESTION DE L'ANIMATION ---
            if (_isDashing)
            {
                _currentAnim = _dashAnim; // Priorité au Dash !
            }
            else if (Math.Abs(Velocity.X) > 0.1f)
            {
                _currentAnim = _runAnim;
                // Le sprite se tourne en fonction de la direction
                _flip = _facingDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            else
            {
                _currentAnim = _idleAnim;
            }

            _currentAnim.Update(dt);

            // --- 6. Y-AXIS MOVEMENT & COLLISION ---
            Position.Y += Velocity.Y * dt;
            _isGrounded = false;

            foreach (var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    if (Velocity.Y > 0)
                    {
                        Position.Y = platform.Bounds.Top - this.Bounds.Height;
                        Velocity.Y = 0;
                        _isGrounded = true;
                    }
                    else if (Velocity.Y < 0)
                    {
                        Position.Y = platform.Bounds.Bottom;
                        Velocity.Y = 0;
                    }
                }
            }

            // --- 7. X-AXIS MOVEMENT & COLLISION ---
            Position.X += Velocity.X * dt;

            foreach (var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    // Même en dashant, si on tape un mur, on s'arrête !
                    if (Velocity.X > 0)
                    {
                        Position.X = platform.Bounds.Left - this.Bounds.Width;
                    }
                    else if (Velocity.X < 0)
                    {
                        Position.X = platform.Bounds.Right;
                    }
                }
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // 1. On charge et on crée l'animation IDLE
            _idleTexture = Texture2D.FromFile(graphicsDevice, "Assets/BananaIdle.png");
            _idleAnim = new Animation(_idleTexture, 2, 0.5f); // Tu pourras ajuster la vitesse (0.5 c'est un peu lent)

            // 2. On charge et on CRÉE la vraie animation de COURSE
            Texture2D runTexture = Texture2D.FromFile(graphicsDevice, "Assets/BananaRun.png");
            _runAnim = new Animation(runTexture, 2, 0.08f); // <-- C'est cette ligne qui manquait !

            // 3. On initialise l'animation par défaut pour éviter un crash au démarrage
            _currentAnim = _idleAnim;
            Texture2D dashTexture = Texture2D.FromFile(graphicsDevice, "Assets/BananaDash.png");
            // On met 1 pour le frameCount. Le temps (0.1f) n'a pas d'importance puisqu'il n'y a qu'une image !
            _dashAnim = new Animation(dashTexture, 1, 0.1f);

            CurrentHealth=MaxHealth;


        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_currentAnim != null)
            {
                Color tintColor = Color.White;
                if (_invincibleTimer > 0)
                {
                    tintColor = Color.Red;
                    if ((int)(_invincibleTimer * 15) % 2 == 0)
                    {
                        _currentAnim.Draw(spriteBatch, Position, _flip, tintColor);
                    }
                }
                else
                {
                    tintColor= Color.White;
                    _currentAnim.Draw(spriteBatch, Position, _flip, tintColor);
                }
            }
        }
        public void TakeDamage(int damage)
        {
            if (_invincibleTimer <= 0)
            {
                CurrentHealth -= damage;
                if (CurrentHealth < 0) CurrentHealth = 0;
                // Lancement de l'invincibilité
                _invincibleTimer = INVINCIBLE_DURATION;
                //knockback animation
                Velocity.X = -JumpForce * _facingDirection * 0.5f; // Recul horizontal
                Velocity.Y = JumpForce*0.5f; // Recul horizontal
                                             // Si on n'a plus de vie, on réapparaît au point de départ (pour le moment)
                if (CurrentHealth <= 0)
                {
                    CurrentHealth = MaxHealth;
                    Position = new Vector2(100, 100);
                    Velocity = Vector2.Zero;
                }
            }
            else
            {
                return; // Le joueur est invincible, on ignore les dégâts
            }
        }
    }     
}
