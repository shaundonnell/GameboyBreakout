using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakingOut
{
    public class Paddle
    {
        Vector2 position;
        Vector2 motion;
        float paddleSpeed = 4.0f;

        KeyboardState keyboardState;
        GamePadState gamePadState;

        Texture2D texture;
        Rectangle screenBounds;
        private Color tint;
        private Color tint2;
        private Color currentTint;

        private SoundEffect bump;
        private SoundEffect bump2;

        private bool hit;
        private float hitTime;

        private float rotation;

        private int scale = 2;

        public Paddle(Texture2D texture, Rectangle screenBounds, Color tint, Color tint2, SoundEffect bump, SoundEffect bump2)
        {
            this.texture = texture;
            this.screenBounds = screenBounds;
            this.tint = tint;
            this.tint2 = tint2;
            this.bump = bump;
            this.bump2 = bump2;

            currentTint = tint;

            SetInStartPosition();
        }

        public void Update()
        {
            position.Y = screenBounds.Height - 6;

            if (hit)
            {
                if (hitTime < 0.1f)
                {
                    hitTime += 1.0f / 60.0f;
                    position.Y += 2;
                    currentTint = tint2;
                }
                else
                {
                    bump2.Play();
                    currentTint = tint;
                    hit = false;
                }
            }

            motion = Vector2.Zero;
            rotation = 0;

            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                motion.X = -1;
                rotation = -0.05f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                motion.X = 1;
                rotation = 0.05f;
            }

            motion.X *= paddleSpeed;
            position += motion;
            LockPaddle();
        }

        public void Hit()
        {
            hit = true;
            hitTime = 0;
            bump.Play();
        }

        public void LockPaddle()
        {
            if (GetBounds().X < 0)
                position.X = (texture.Width * scale) / 2.0f;
            if (GetBounds().X + GetBounds().Width > screenBounds.Width)
                position.X = screenBounds.Width - ((texture.Width * scale) / 2.0f);
        }

        public void SetInStartPosition()
        {
            rotation = 0;
            position.X = screenBounds.Width / 2.0f;
            position.Y = screenBounds.Height - 6;
        }

        public void Draw(SpriteBatch batch)
        {
            Rectangle locationRect = new Rectangle((int)position.X, (int)position.Y, texture.Width * scale, texture.Height);
            //batch.Draw(texture, locationRect, currentTint);
            batch.Draw(texture, position, null, currentTint, rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), new Vector2(scale, 1), SpriteEffects.None, 0);

        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)(position.X - ((texture.Width * scale) / 2.0f)), (int)(position.Y - (texture.Height / 2.0f)), texture.Width * scale, texture.Height);
        }
    }
}
