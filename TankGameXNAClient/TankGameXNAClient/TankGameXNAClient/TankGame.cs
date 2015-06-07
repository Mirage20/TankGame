using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TankGameXNAClient
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TankGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        KeyboardState oldState;
        //SpriteFont fontMain;
        GameManager gameManager;
        public TankGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            graphics.PreferredBackBufferWidth = Global.ViewportWidth;
            graphics.PreferredBackBufferHeight = Global.ViewportHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Tank Game Client Decepticons";
            gameManager = GameManager.GetInstance();
            Global.Content = Content;           
            base.Initialize();
            oldState = Keyboard.GetState();
        }


        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;
            //fontMain = Content.Load<SpriteFont>("SpriteFontMain");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Global.SpriteBatch = spriteBatch;
            gameManager.Start();
            
            // TODO: use this.Content to load your game content here
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            gameManager.Stop();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateInput();
            //if (Keyboard.GetState().IsKeyDown(Keys.A))
            //    Communicator.GetInstance().SendData("JOIN#");

            //if (Keyboard.GetState().IsKeyDown(Keys.Left))
                

            //if (Keyboard.GetState().IsKeyDown(Keys.Right))
            //    Communicator.GetInstance().SendData("RIGHT#");

            //if (Keyboard.GetState().IsKeyDown(Keys.Up))
            //    Communicator.GetInstance().SendData("UP#");

            //if (Keyboard.GetState().IsKeyDown(Keys.Down))
            //    Communicator.GetInstance().SendData("DOWN#");

            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
                

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.J))
            {
                if (!oldState.IsKeyDown(Keys.J))
                    Communicator.GetInstance().SendData("JOIN#");
            }

            // Is the SPACE key down?
            if (newState.IsKeyDown(Keys.Space))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    Communicator.GetInstance().SendData("SHOOT#");
                }
            }
            else if (oldState.IsKeyDown(Keys.Space))
            {
                // Key was down last update, but not down now, so
                // it has just been released.
            }


            if (newState.IsKeyDown(Keys.Left))
            {                
                if (!oldState.IsKeyDown(Keys.Left))              
                    Communicator.GetInstance().SendData("LEFT#");               
            }

            if (newState.IsKeyDown(Keys.Right))
            {
                if (!oldState.IsKeyDown(Keys.Right))
                    Communicator.GetInstance().SendData("RIGHT#");
            }

            if (newState.IsKeyDown(Keys.Up))
            {
                if (!oldState.IsKeyDown(Keys.Up))
                    Communicator.GetInstance().SendData("UP#");
            }

            if (newState.IsKeyDown(Keys.Down))
            {
                if (!oldState.IsKeyDown(Keys.Down))
                    Communicator.GetInstance().SendData("DOWN#");
            }

            // Update saved state.
            oldState = newState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Global.GameTime = gameTime;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            DrawText();
            gameManager.Draw();
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawText()
        {
            //spriteBatch.DrawString(fontMain, gameManager.GetText(), new Vector2(20, 45), Color.White);
        }
    }
}
