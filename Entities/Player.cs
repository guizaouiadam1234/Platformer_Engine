using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private float _jumpBufferTimer = 0f;
        private float _coyoteTimer = 0f;
        private const float JUMP_BUFFER_TIME = 0.15f;
        private const float COYOTE_TIME = 0.15f;

        public override void Update(float dt, List<Entity> platforms)
        {
            // --- 1. TIMERS ---
            _jumpBufferTimer -= dt;
            _coyoteTimer -= dt;
            if (_isGrounded) _coyoteTimer = COYOTE_TIME;

            // --- 2. INPUT & HORIZONTAL VELOCITY ---
            Velocity.X = 0;
            if (Input.IsKeyDown(Keys.Left)) Velocity.X = -Speed;
            if (Input.IsKeyDown(Keys.Right)) Velocity.X = Speed;

            if (Input.HasBeenPressed(Keys.C)) _jumpBufferTimer = JUMP_BUFFER_TIME;

            // --- 3. GRAVITY ---
            Velocity.Y += Gravity * dt;

            // --- 4. JUMP EXECUTION ---
            if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
            {
                Velocity.Y = JumpForce;
                _isGrounded = false;
                _jumpBufferTimer = 0;
                _coyoteTimer = 0;
            }

            // --- 5. Y-AXIS MOVEMENT & COLLISION ---
            // Move first, then check. This prevents sticking.
            Position.Y += Velocity.Y * dt;
            _isGrounded = false;

            foreach (var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    if (Velocity.Y > 0) // Falling down
                    {
                        Position.Y = platform.Bounds.Top - this.Bounds.Height;
                        Velocity.Y = 0;
                        _isGrounded = true;
                    }
                    else if (Velocity.Y < 0) // Hitting ceiling (Fixes jumping through)
                    {
                        Position.Y = platform.Bounds.Bottom;
                        Velocity.Y = 0;
                    }
                }
            }

            // --- 6. X-AXIS MOVEMENT & COLLISION ---
            Position.X += Velocity.X * dt;

            foreach (var platform in platforms)
            {
                if (this.Bounds.Intersects(platform.Bounds))
                {
                    if (Velocity.X > 0) // Moving Right
                    {
                        Position.X = platform.Bounds.Left - this.Bounds.Width;
                    }
                    else if (Velocity.X < 0) // Moving Left
                    {
                        Position.X = platform.Bounds.Right;
                    }
                }
            }
        }
    }
}
