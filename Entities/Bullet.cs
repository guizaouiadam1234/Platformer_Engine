using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNewEngine.Entities
{
    public class Bullet : Entity
    {
        public Vector2 velocity;
        public float LifeSpan = 2.0f; // Bullet will exist for 2 seconds
        
        public Bullet(Vector2 position, Vector2 velocity)
        {
            this.Position = position;
            this.velocity = velocity;
            this.Size = new Vector2(8, 8); // Smaller size for bullets
            this.tint = Color.Yellow; 
        }

        public override void Update(float dt, List<Entity> platforms)
        {
            Position += velocity * dt;
            foreach(var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    LifeSpan = 0;
                }

            }
        }
    }
}
