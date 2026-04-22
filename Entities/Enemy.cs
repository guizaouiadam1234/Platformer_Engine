using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;



namespace MyNewEngine.Entities
{
    public class Enemy : Entity
    {
        public float speed = 100f;
        public float gravity = 1200f;
        public Vector2 velocity;
        public int direction = 1;


        private Texture2D _texture;

        public void LoadContent(Texture2D texture)
        {
            _texture = texture;
            Size = new Vector2(32, 32);


        }
        public override void Update(float dt, List<Entity> platforms)
        {
            velocity.Y += gravity * dt;
            velocity.X = speed * direction;

            Position.Y += velocity.Y * dt;
            foreach (var platform in platforms)
            {
                if (Bounds.Intersects(platform.Bounds))
                {
                    Position.Y = platform.Bounds.Top - Size.Y;
                    velocity.Y = 0;
                }
            }

            Position.X += velocity.X * dt;
            foreach (var platform in platforms)
            {
                if (Bounds.Intersects(platform.Bounds))
                {
                    if (velocity.X > 0)
                    {
                        Position.X = platform.Bounds.Left - Size.X;
                        direction = -1;
                    }
                    else
                    {
                        Position.X = platform.Bounds.Right;
                        direction = 1;
                    }
                }

            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                // On le dessine en rouge pour bien le voir !
                spriteBatch.Draw(_texture, this.Bounds, Color.Red);
            }
        }
    }
}
