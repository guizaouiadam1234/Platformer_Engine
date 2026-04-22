using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MyNewEngine.Graphics
{
    public class Animation
    {
        private Texture2D _texture;
        private int _frameCount;
        private int _currentFrame;
        private float _timer;
        private int _framewidth;
        private int _frameheight;
        private float _frameTime; // <-- On ajoute cette variable

        public Animation(Texture2D texture, int frameCount, float v)
        {
            _texture = texture;
            _frameCount = frameCount;
            _currentFrame = 0;
            _timer = 0f;
            _frameTime = v; // <-- On enregistre la vitesse ici
            _framewidth = _texture.Width / _frameCount;
            _frameheight = _texture.Height;
        }

        public void Update(float dt)
        {
            _timer += dt;

            if (_timer >= _frameTime)
            {
                _timer = 0;
                _currentFrame++;

                if (_currentFrame >= _frameCount)
                {
                    _currentFrame = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects flip)
        {
            Rectangle sourceRect = new Rectangle(_currentFrame * _framewidth, 0, _framewidth, _frameheight);
            spriteBatch.Draw(_texture, position, sourceRect, Color.White, 0f, Vector2.Zero, 1f, flip, 0f);
        }
    }
}