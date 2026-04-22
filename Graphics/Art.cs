using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyNewEngine.Graphics
{
    public static class Art
    {
        public static Texture2D Pixel { get; private set; }

        public static void Load(GraphicsDevice graphicsDevice)
        {
            Pixel = new Texture2D(graphicsDevice, 1, 1);

            Color[] data = new Color[] { Color.White };
            Pixel.SetData(data);
        }
    }
}