using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakingOut
{
    class Coin
    {
        Texture2D texture;
        Vector2 position;
        Vector2 velocity;
        Vector2 acceleration;
        Color tint;

        public Coin(Texture2D texture, Vector2 position, Color tint, Random random)
        {
            this.texture = texture;
            this.position = position;
            velocity = new Vector2((float)(random.NextDouble() * 2.0) - 1.0f, (float)random.NextDouble() * -1.0f);
            acceleration = new Vector2(0, 0.1f);
            this.tint = tint;
        }

        public void Update()
        {
            velocity += acceleration;
            position += velocity;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, position, tint);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }
    }
}
