using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    class GameManager
    {
        private static GameManager gameManager = new GameManager();
        private MessageDecoder messageDecoder;
        private Communicator communicator;
        private GridManager gridManager;
        private PlayerManager playerManager;
        private ItemManager itemManager;
        private CollisionManager collisionManager;
        private AIManager aiManager;
        private bool isGameStarted = false;
        private SpriteFont fontMain;
        private Texture2D backgroundTexture;
        private Texture2D logoTexture;
        private Rectangle mainFrame;

        private GameManager()
        {
            messageDecoder = MessageDecoder.GetInstance();
            communicator = Communicator.GetInstance();
            gridManager = new GridManager();
            playerManager = new PlayerManager();
            itemManager = new ItemManager();
            collisionManager = new CollisionManager();
            aiManager = new AIManager();

            // Event subscribe
            messageDecoder.InitialMapReceived += messageDecoder_InitialMapReceived;
            messageDecoder.PlayerSetupReceived += messageDecoder_PlayerSetupReceived;
            messageDecoder.GameUpdateReceived += messageDecoder_GameUpdateReceived;
            messageDecoder.CoinUpdateReceived += messageDecoder_CoinUpdateReceived;
            messageDecoder.LifeUpdateReceived += messageDecoder_LifeUpdateReceived;
        }

        // Event when initial server map was recived
        private void messageDecoder_InitialMapReceived(int playerNumber, System.Drawing.Point[] brickPositions, System.Drawing.Point[] stonePositions, System.Drawing.Point[] waterPositions)
        {
            gridManager.SetStationaryGameObjects(brickPositions, stonePositions, waterPositions);
            playerManager.OwnPlayerNumber = playerNumber;
        }

        // Event when player setup was recived
        private void messageDecoder_PlayerSetupReceived(int numberOfPlayers, int[] playerNumbers, System.Drawing.Point[] playerPositions, Direction[] playerDirections)
        {
            playerManager.CreatePlayers(numberOfPlayers, playerNumbers, playerPositions, playerDirections);
        }

        // Event when global update was recived
        private void messageDecoder_GameUpdateReceived(int numberOfPlayers, int[] playerNumbers, System.Drawing.Point[] playerCurrentPositions, Direction[] playerCurrentDirections, bool[] playerIsShooted, int[] playerCurrentHealth, int[] playerCurrentCoins, int[] playerCurrentPoints, int numberOfBricks, System.Drawing.Point[] brickPositions, BrickDamageLevel[] currentBrickDamageLevel)
        {
            playerManager.UpdatePlayers(numberOfPlayers, playerNumbers, playerCurrentPositions, playerCurrentDirections, playerIsShooted, playerCurrentHealth, playerCurrentCoins, playerCurrentPoints);
            gridManager.UpdateBricks(numberOfBricks, brickPositions, currentBrickDamageLevel);
            
            // Low accuracy collisions
            collisionManager.CheckEventCollisions(gridManager, playerManager, itemManager);
            
            if (!isGameStarted)
                isGameStarted = true;
            communicator.SendData(aiManager.getNextCommand(gridManager, playerManager, itemManager));
        }

        // Event when coin pile was recived
        private void messageDecoder_CoinUpdateReceived(System.Drawing.Point coinPosition, int coinLifeTime, int coinValue)
        {
            itemManager.AddCoinPile(coinPosition, coinLifeTime, coinValue);
        }

        // Event when life pack was recived
        private void messageDecoder_LifeUpdateReceived(System.Drawing.Point lifePackPosition, int lifePackLifeTime)
        {
            itemManager.AddLifePack(lifePackPosition, lifePackLifeTime);
        }

        public static GameManager GetInstance()
        {
            return gameManager;
        }


        // Initialize the game sequence with listning network thread
        public void Start()
        {
            fontMain = Global.Content.Load<SpriteFont>("SpriteFontMain");
            backgroundTexture = Global.Content.Load<Texture2D>("background");
            logoTexture = Global.Content.Load<Texture2D>("logo");
            mainFrame = new Rectangle(0, 0, Global.ViewportWidth, Global.ViewportHeight);
            gridManager.BuildGrid();
            communicator.StartListening();
        }
        //Stop the network thread
        public void Stop()
        {
            communicator.StopListening();
        }


        // Main draw call which call required draw methords to draw the  user interface
        public void Draw()
        {
            Global.SpriteBatch.Draw(backgroundTexture, mainFrame, Color.White);
            Global.SpriteBatch.Draw(logoTexture, new Vector2(Global.Left + 100,Global.Top - 180), null, Color.White, 0.0f, new Vector2(0, 0), .2f, SpriteEffects.None, 0);
            gridManager.Draw();
            itemManager.Draw();
            playerManager.Draw();


            // Score board
            int top = Global.Top;
            int left = Global.Left;
            int hSpace1 = 130;
            int hSpace2 = 200;
            int hSpace3 = 260;
            int vSpace = 13;
            int vMargin = 20;
            Global.SpriteBatch.DrawString(fontMain, "Player Number", new Vector2(left, top), Color.White);
            Global.SpriteBatch.DrawString(fontMain, "Score", new Vector2(left + hSpace1, top), Color.White);
            Global.SpriteBatch.DrawString(fontMain, "Coins", new Vector2(left + hSpace2, top), Color.White);
            Global.SpriteBatch.DrawString(fontMain, "Health", new Vector2(left + hSpace3, top), Color.White);
            if (playerManager.GetAllPlayers() != null)
            {

                foreach (Player player in playerManager.GetAllPlayers())
                {
                    if (player != null)
                    {
                        Global.SpriteBatch.DrawString(fontMain, "P " + player.PlayerNumber.ToString(), new Vector2(left, top + vMargin + player.PlayerNumber * vSpace), Color.White);
                        Global.SpriteBatch.DrawString(fontMain, player.CurrentPoints.ToString(), new Vector2(left + hSpace1, top + vMargin + player.PlayerNumber * vSpace), Color.White);
                        Global.SpriteBatch.DrawString(fontMain, player.CurrentCoins.ToString(), new Vector2(left + hSpace2, top + vMargin + player.PlayerNumber * vSpace), Color.White);
                        Global.SpriteBatch.DrawString(fontMain, player.CurrentHealth.ToString() + "%", new Vector2(left + hSpace3, top + vMargin + player.PlayerNumber * vSpace), Color.White);
                    }
                }


            }
            Global.SpriteBatch.DrawString(fontMain, "Press \"J\" to join\r\nPress Escape to exit", new Vector2(left, 500), Color.White);
            // High Accuracy Collisions 
            if (isGameStarted)
                collisionManager.CheckCollisions(gridManager, playerManager, itemManager);
        }


    }
}
