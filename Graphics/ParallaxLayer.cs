using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNewEngine.Graphics
{
    public class ParallaxLayer
    {
        public Texture2D Texture;
        public float ScrollFactor; // 0 = static, 1 = moves with camera, >1 = moves faster than camera
        public ParallaxLayer(Texture2D texture, float scrollFactor)
        {
            Texture = texture;
            ScrollFactor = scrollFactor;
        }

        public void Draw(SpriteBatch spriteBatch, Camera2D camera, int screenwidth, int screenheight)
        {
            Vector2 offset = camera.position * ScrollFactor;
            spriteBatch.Draw(Texture, new Rectangle(0, 0, screenwidth, screenheight), new Rectangle((int)offset.X, (int)offset.Y, screenwidth, screenheight), Color.White);


        }
    }
}
