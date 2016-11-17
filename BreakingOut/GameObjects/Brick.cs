using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakingOut
{
    public class Brick
    {
        public Vector2 location;

        private Texture2D texture;
        public Color tint;
        public bool alive;

        private Action<Brick> explosionCallback;

        public Brick(Texture2D texture, Vector2 location, Color tint, Action<Brick> explosionCallback)
        {
            this.texture = texture;
            this.location = location;
            this.tint = tint;
            this.alive = true;

            this.explosionCallback = explosionCallback;
        }

        public bool CheckCollision(Ball ball)
        {
            if (alive && ball.Bounds.Intersects(GetBounds()))
            {
                alive = false;
                ball.Deflection(this);
                return true;
            }

            return false;
        }

        public void OnColision()
        {
            alive = false;
            explosionCallback(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(alive)
                spriteBatch.Draw(texture, location, tint);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height);
        }
    }
}
