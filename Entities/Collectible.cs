

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MyNewEngine.Entities
{
    public enum CollectibleType
    {
        Health,
        Mana,
        Coin
    }
    public class Collectible : Entity
    {
        public CollectibleType Type;
        private Texture2D _texture;

        private float _bobbingTimer = 0f;
        private float _startY;

        public Collectible(Vector2 position, CollectibleType type, Texture2D texture)
        {
            Position = position;
            _startY = position.Y; // On sauvegarde la position Y de départ pour la faire flotter autour
            Type = type;
            _texture = texture;
            Size = new Vector2(16, 16); // La taille de la boîte de collision
        }
        public void Update(float dt)
        {
            _bobbingTimer += dt;
            Position.Y = _startY + (float)Math.Sin(_bobbingTimer * 2) * 5;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }

        public void applyEffect(Player player)
        {
            switch (Type)
            {
                case CollectibleType.Health:
                    player.CurrentHealth = Math.Min(player.CurrentHealth + 20, player.MaxHealth);
                    break;
                case CollectibleType.Mana:
                    player.currentMana = Math.Min(player.currentMana + 20, player.maxMana);
                    break;
                case CollectibleType.Coin:
                    player.coins += 1;
                    break;
            }
        }

    }
}
