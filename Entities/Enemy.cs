using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyNewEngine.Graphics; // On ajoute ça pour accéder à la classe Animation
using System.Collections.Generic;

namespace MyNewEngine.Entities
{
    public class Enemy : Entity
    {
        public float Speed = 100f;
        public float Gravity = 1200f;
        public Vector2 Velocity;
        public int Direction = 1; // 1 = droite, -1 = gauche

        // --- Nouvelles variables d'animation ---
        private Animation _walkAnim;
        private SpriteEffects _flip = SpriteEffects.None;

        public void LoadContent(Texture2D texture)
        {
            // On découpe ton image en 2 frames !
            _walkAnim = new Animation(texture, 2, 0.15f);

            // On calcule la taille physique (Size) de l'ennemi en fonction d'UNE SEULE frame
            Size = new Vector2(texture.Width / 2, texture.Height);
        }

        public override void Update(float dt, List<Entity> platforms)
        {
            
            Velocity.Y += Gravity * dt;
            Velocity.X = Speed * Direction;

            
            _walkAnim.Update(dt);
            
            _flip = Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // --- COLLISIONS Y ---
            Position.Y += Velocity.Y * dt;
            foreach (var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    if (Velocity.Y > 0)
                    {
                        Position.Y = platform.Bounds.Top - this.Bounds.Height;
                        Velocity.Y = 0;
                    }
                }
            }

            // --- COLLISIONS X ---
            Position.X += Velocity.X * dt;
            foreach (var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    if (Velocity.X > 0)
                    {
                        Position.X = platform.Bounds.Left - this.Bounds.Width;
                        Direction = -1; 
                    }
                    else if (Velocity.X < 0)
                    {
                        Position.X = platform.Bounds.Right;
                        Direction = 1; 
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_walkAnim != null)
            {
                _walkAnim.Draw(spriteBatch, Position, _flip, Color.White);
            }
        }
    }
}