using Microsoft.Xna.Framework.Graphics;
using MyNewEngine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MyNewEngine.Entities
{
    public class Entity
    {
        public Vector2 Position;
        public Vector2 Size = new Vector2(32, 32); // Default size
        public Color tint = Color.White;

        // Update Bounds to use the new Size
        public virtual Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y); }
        }

        public virtual void Update(float dt, List<Entity> platforms) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Use the Bounds (which now accounts for Size)
            spriteBatch.Draw(Art.Pixel, Bounds, tint);
        }
    }
}
