using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNewEngine.Graphics
{
    public class Camera2D
    {
        public Vector2 position = new Vector2();
        public float zoom = 1f;
        public Matrix Transform { get; private set; }

        public void Follow(Vector2 target, int screenwidth, int screenheight)
        {
            position = target - new Vector2(screenwidth / 2, screenheight / 2);
            Transform = Matrix.CreateTranslation(-position.X, -position.Y, 0) * Matrix.CreateScale(zoom);
        }



    }
}
