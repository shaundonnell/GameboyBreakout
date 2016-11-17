using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BreakingOut.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;

namespace BreakingOut
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Paddle paddle;
        private Ball ball;
        private Rectangle screenRectangle;

        private int bricksWide = 8;
        private int bricksHigh = 5;
        private Texture2D brickImage;
        private Texture2D coinImage;
        private static Brick[,] bricks;
        private List<Coin> coins;

        private List<SoundEffect> explosions;
        private SoundEffect bounce;
        private SoundEffect bump;
        private SoundEffect bump2;

        private RenderTarget2D renderTarget;
        private ScreenShake screenShake;

        private int worldHeight = 144;
        private int worldWidth = 160;
        
        private Color color1 = new Color(0.1f, 0.11f, 0.1f, 1);
        private Color color2 = new Color(0.8f, 1, 0.9f, 1);
        private Color color3 = new Color(0.5f, 0.8f, 0.5f, 1);
        private Color color4 = new Color(0.2f, 0.5f, 0.3f, 1);

        private List<Coin> coinsToRemove;

        private bool gameStarted = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 576;

            Window.AllowUserResizing = true;

            screenRectangle = new Rectangle(0, 0, worldWidth, worldHeight);

            coins = new List<Coin>();
            coinsToRemove = new List<Coin>();
            explosions = new List<SoundEffect>();

            screenShake = new ScreenShake();

            bricks = new Brick[bricksWide, bricksHigh];
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                worldWidth,
                worldHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D tempTexture = Content.Load<Texture2D>("paddle");

            brickImage = Content.Load<Texture2D>("brick");
            coinImage = Content.Load<Texture2D>("coin");

            bounce = Content.Load<SoundEffect>("bounce");
            bump = Content.Load<SoundEffect>("bump3");
            bump2 = Content.Load<SoundEffect>("bump1");

            for (int i = 1; i <= 4; i++)
            {
                //explosions.Add(Content.Load<SoundEffect>("explosion" + i));
            }

            explosions.Add(Content.Load<SoundEffect>("explosion2"));

            paddle = new Paddle(tempTexture, screenRectangle, color3, color2, bump, bump2);

            tempTexture = Content.Load<Texture2D>("ball");
            ball = new Ball(tempTexture, screenRectangle, color2, bounce);

            StartGame();
        }

        private void StartGame()
        {
            gameStarted = false;

            paddle.SetInStartPosition();
            ball.SetInStartPosition(paddle.GetBounds());

            int offset = 1;
            int xPadding = (worldWidth - ((offset + brickImage.Width) * bricksWide)) / 2;
            int YPadding = 6;

            for(int y = 0; y < bricksHigh; y++)
            {
                Color tint = Color.White;

                switch (y)
                {
                    case 0: tint = color2; break;
                    case 1: tint = color3; break;
                    case 2: tint = color4; break;
                    case 3: tint = color3; break;
                    case 4: tint = color2; break;
                }

                for (int x = 0; x < bricksWide; x++)
                {
                    Vector2 brickPosition = new Vector2((x * offset) + xPadding + x * brickImage.Width,
                        YPadding + (y * offset) + y * brickImage.Height);
                    bricks[x, y] = new Brick(brickImage, brickPosition, tint, BrickExplosion);
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                gameStarted = true;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var coin in coins)
            {
                coin.Update();

                if (coin.GetBounds().Y > screenRectangle.Height)
                {
                    coinsToRemove.Add(coin);
                }
            }

            if (!gameStarted)
                return;

            // TODO: Add your update logic here

            screenShake.Update();

            paddle.Update();
            ball.Update();

            foreach (var coin in coinsToRemove)
            {
                coins.Remove(coin);
            }

            ball.PaddleCollision(paddle);

            if(ball.OffBottom())
                StartGame();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw the scene
            GraphicsDevice.Clear(color1);

            foreach (var brick in bricks)
            {
                brick.Draw(spriteBatch);
            }

            paddle.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            foreach (var coin in coins)
            {
                coin.Draw(spriteBatch);
            }

            spriteBatch.End();

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            GraphicsDevice.Clear(Color.Black);

            float ratio = worldWidth / worldHeight;
            float currentWidth = Window.ClientBounds.Height * ratio;
            int offset = (int)((Window.ClientBounds.Width - currentWidth) / 2.0f);
            spriteBatch.Draw(renderTarget, new Rectangle(offset + (int)screenShake.GetPosition().X, (int)screenShake.GetPosition().Y, (int)currentWidth, Window.ClientBounds.Height), Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }

        public void BrickExplosion(Brick brick)
        {
            screenShake.Shake(Window.ClientBounds.Width * 0.02f);
            Random random = new Random();

            int explosionIndex = random.Next(0, explosions.Count);
            explosions[explosionIndex].Play();

            for (int i = 0; i < 10; i++)
            {
                float randomX = (float)random.NextDouble() * brickImage.Width;
                float randomY = (float)random.NextDouble() * brickImage.Height;
                Coin coin = new Coin(coinImage, new Vector2(brick.location.X + randomX, brick.location.Y + randomY), brick.tint, random);
                coins.Add(coin);
            }
        }

        public static Brick CheckCollision(Rectangle bounds)
        {
            foreach (var brick in bricks)
            {
                if (brick != null)
                {
                    if (brick.alive && brick.GetBounds().Intersects(bounds))
                    {
                        return brick;
                    }
                }

            }

            return null;
        }
    }
}
