using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace BreakingOut
{
    public class Ball
    {
        private Vector2 position;
        private Vector2 velocity;

        private const float ballStartSpeed = 3.0f;

        private float ballRestartSpeed = ballStartSpeed;
        private float ballSpeed = ballStartSpeed;
        private float ballMinSpeed = 1.5f;
        private float damping = 0.015f;

        private Rectangle bounds;

        private Texture2D texture;
        private Rectangle screenBounds;

        private bool collided;
        private float rotation;

        private Color tint;

        private SoundEffect bounce;

        public Rectangle Bounds
        {
            get
            {
                bounds.X = (int) position.X;
                bounds.Y = (int) position.Y;
                return bounds;
            }
        }

        public Ball(Texture2D texture, Rectangle screenBounds, Color tint, SoundEffect bounce)
        {
            bounds = new Rectangle(0, 0, texture.Width, texture.Height);
            this.texture = texture;
            this.screenBounds = screenBounds;
            this.rotation = 0;
            this.tint = tint;
            this.bounce = bounce;

        }

        public void Update()
        {
            collided = false;

            float newX = position.X + (velocity.X * ballSpeed);
            Brick collidedBrick =  Game1.CheckCollision(new Rectangle((int) newX, (int) position.Y, texture.Width, texture.Height));
            if (collidedBrick != null)
            {
                collidedBrick.OnColision();
                velocity.X *= -1;
            }
            else
                position.X = newX;

            float newY = position.Y + (velocity.Y * ballSpeed);
            collidedBrick = Game1.CheckCollision(new Rectangle((int) position.X, (int) newY, texture.Width, texture.Height));
            if (collidedBrick != null)
            {
                collidedBrick.OnColision();
                velocity.Y *= -1;
            }
            else
                position.Y = newY;

            ballSpeed -= damping;
            if (ballSpeed < ballMinSpeed)
                ballSpeed = ballMinSpeed;

            //rotation += 0.1f;

            CheckWallCollisions();
        }

        private void CheckWallCollisions()
        {
            if (position.X < 0)
            {
                position.X = 0;
                velocity.X *= -1;
                bounce.Play();
            }
            if (position.X + texture.Width > screenBounds.Width)
            {
                position.X = screenBounds.Width - texture.Width;
                velocity.X *= -1;
                bounce.Play();
            }
            if (position.Y < 0)
            {
                position.Y = 0;
                velocity.Y *= -1;
                bounce.Play();
            }

            velocity.Normalize();
        }

        public void SetInStartPosition(Rectangle paddleLocation)
        {
            Random rand = new Random();
            velocity = new Vector2(((float)rand.NextDouble() * 2) - 0.5f, ((float)rand.NextDouble() / 2.0f) + 0.5f);
            velocity.Normalize();

            ballRestartSpeed = ballStartSpeed;
            ballSpeed = ballStartSpeed;

            position.Y = paddleLocation.Y - texture.Height;
            position.X = paddleLocation.X + (paddleLocation.Width - texture.Width) / 2;
        }

        public bool OffBottom()
        {
            return position.Y > screenBounds.Height;
        }

        public void PaddleCollision(Paddle paddle)
        {
            Rectangle paddleLocation = paddle.GetBounds();

            if (paddleLocation.Intersects(Bounds))
            {
                paddle.Hit();
                ballRestartSpeed += 0.05f;
                ballSpeed = ballRestartSpeed;

                position.Y = paddleLocation.Y - texture.Height;
                velocity.Y *= -1;
                velocity.Normalize();

                float positionDiff = position.X - paddleLocation.X;
                positionDiff /= paddleLocation.Width;
                positionDiff -= 0.5f;
                velocity.X += positionDiff;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            //batch.Draw(texture, position, null, tint, rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 0);
            batch.Draw(texture, position, Color.White);
        }

        public void Deflection(Brick brick)
        {
            return;

            if (!collided)
            {
                Rectangle collisionRect = Rectangle.Intersect(brick.GetBounds(), Bounds);
                if (collisionRect.Width > collisionRect.Height)
                {
                    bool isBottom = collisionRect.Y > brick.location.Y;
                    position.Y += isBottom ? collisionRect.Height : -collisionRect.Height;
                    velocity.Y = isBottom ? 1 : -1;
                }
                else
                {
                    bool isRight = collisionRect.X > brick.location.X;
                    position.X += isRight ? collisionRect.Width : -collisionRect.Width;
                    velocity.X = isRight ? 1 : -1;
                }

                velocity.Normalize();

                collided = true;
            }
        }
    }
}
