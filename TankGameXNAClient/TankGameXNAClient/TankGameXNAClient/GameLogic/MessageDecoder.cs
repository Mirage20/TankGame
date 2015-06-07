using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TankGameXNAClient
{
    // delegates for events
    public delegate void InitialMapReceivedHandler(int playerNumber, Point[] brickPositions, Point[] stonePositions, Point[] waterPositions);
    public delegate void PlayerSetupReceivedHandler(int numberOfPlayers, int[] playerNumbers, Point[] playerPositions, Direction[] playerDirections);
    public delegate void GameUpdateReceivedHandler(int numberOfPlayers, int[] playerNumbers, Point[] playerCurrentPositions, Direction[] playerCurrentDirections, bool[] playerIsShooted, int[] playerCurrentHealth, int[] playerCurrentCoins, int[] playerCurrentPoints, int numberOfBricks, Point[] brickPositions, BrickDamageLevel[] currentBrickDamageLevel);
    public delegate void CoinUpdateReceivedHandler(Point coinPosition,int coinLifeTime,int coinValue);
    public delegate void LifeUpdateReceivedHandler(Point lifePackPosition, int lifePackLifeTime);

    //Decodes the incoming messages
    class MessageDecoder
    {
        // Main broadcast message events
        public event InitialMapReceivedHandler InitialMapReceived = delegate { };
        public event PlayerSetupReceivedHandler PlayerSetupReceived = delegate { };
        public event GameUpdateReceivedHandler GameUpdateReceived = delegate { };
        public event CoinUpdateReceivedHandler CoinUpdateReceived = delegate { };
        public event LifeUpdateReceivedHandler LifeUpdateReceived = delegate { };

        private static MessageDecoder msgDecInst = new MessageDecoder();

        private Communicator com;

        private MessageDecoder()
        {
            com = Communicator.GetInstance();
            com.MessageReceived += com_MessageReceived;
        }

        public static MessageDecoder GetInstance()
        {
            return msgDecInst;
        }

        //must be private
        private void com_MessageReceived(string message)
        {

            if (message[1] == ':')
            {
                //Internal variable message of the server
                DecodeInternalServerMessage(message);
            }
            else
            {
                //Direct server message
                DecodeServerMessage(message);
            }

        }

        // decode normal server massages
        private void DecodeServerMessage(string message)
        {
            if (message.Equals("CELL_OCCUPIED#"))
            {
                com.SendData("SHOOT#");
                return;
            }
            throw new NotImplementedException();
        }


        private void DecodeInternalServerMessage(string message)
        {
            switch (message[0])
            {
                case 'I':
                    DecodeInitialMapCondition(message);
                    break;
                case 'S':
                    DecodePlayerSetup(message);
                    break;
                case 'G':
                    DecodeGameUpdate(message);
                    break;
                case 'C':
                    DecodeCoinUpdate(message);
                    break;
                case 'L':
                    DecodeLifeUpdate(message);
                    break;
                default:
                    break;
            }
        }

        // converts string cordinate to point
        private Point CoordinateToPoint(string coordinate)
        {
            string[] pointString = coordinate.Split(',');
            int x = Convert.ToInt32(pointString[0]);
            int y = Convert.ToInt32(pointString[1]);
            return new Point(x,y);
        }

        //decode initial map 
        private void DecodeInitialMapCondition(string message)
        {
            string[] content = message.Split(new char[] { ':', '#' });

            int playerNumber = Convert.ToInt32(content[1][1].ToString());

            string brickString = content[2];
            string[] brickCoordinates = brickString.Split(';');
            Point[] brickPositions = new Point[brickCoordinates.Length];
            for (int i = 0; i < brickCoordinates.Length; i++)
            {
                brickPositions[i] = CoordinateToPoint(brickCoordinates[i]);
            }

            string stoneString = content[3];
            string[] stoneCoordinates = stoneString.Split(';');
            Point[] stonePositions = new Point[stoneCoordinates.Length];
            for (int i = 0; i < stoneCoordinates.Length; i++)
            {
                stonePositions[i] = CoordinateToPoint(stoneCoordinates[i]);
            }

            string waterString = content[4];
            string[] waterCoordinates = waterString.Split(';');
            Point[] waterPositions = new Point[waterCoordinates.Length];
            for (int i = 0; i < waterCoordinates.Length; i++)
            {
                waterPositions[i] = CoordinateToPoint(waterCoordinates[i]);
            }

            InitialMapReceived(playerNumber, brickPositions, stonePositions, waterPositions);
        }

        // decode player setup message
        private void DecodePlayerSetup(string message)
        {
            string[] content = message.Split(new char[] { ':', '#' });
            int numberOfplayers = content.Length - 2;

            int[]  playerNumbers=new int[numberOfplayers];
            Point[] playerPositions = new Point[numberOfplayers];
            Direction[] playerDirections = new Direction[numberOfplayers];

            for (int i = 0; i < numberOfplayers; i++)
            {
                string[] playerInitialData=content[i+1].Split(';');
                playerNumbers[i] = Convert.ToInt32(playerInitialData[0][1].ToString());
                playerPositions[i] = CoordinateToPoint(playerInitialData[1]);
                playerDirections[i] = (Direction) Convert.ToByte(playerInitialData[2]);
            }

            PlayerSetupReceived(numberOfplayers, playerNumbers, playerPositions, playerDirections);
        }

        // decode global game update
        private void DecodeGameUpdate(string message)
        {
            
            string[] content = message.Split(new char[] { ':', '#' });
            int numberOfplayers = content.Length - 3;

            //Decoding player updates
            int[]  playerNumbers=new int[numberOfplayers];
            Point[] playerCurrentPositions = new Point[numberOfplayers];
            Direction[] playerCurrentDirections = new Direction[numberOfplayers];
            bool [] playerShooted = new bool[numberOfplayers];            
            int [] playerCurrentHealth = new int[numberOfplayers];
            int [] playerCurrentCoins = new int[numberOfplayers];
            int [] playerCurrentPoints = new int[numberOfplayers];

            for (int i = 0; i < numberOfplayers; i++)
            {
                string[] currentPlayerData=content[i+1].Split(';');
                playerNumbers[i] = Convert.ToInt32(currentPlayerData[0][1].ToString());
                playerCurrentPositions[i] = CoordinateToPoint(currentPlayerData[1]);
                playerCurrentDirections[i] = (Direction) Convert.ToByte(currentPlayerData[2]);
                playerShooted[i] = Convert.ToBoolean(Convert.ToInt32(currentPlayerData[3]));
                playerCurrentHealth[i] = Convert.ToInt32(currentPlayerData[4]);
                playerCurrentCoins[i] = Convert.ToInt32(currentPlayerData[5]);
                playerCurrentPoints[i] = Convert.ToInt32(currentPlayerData[6]);
            }

            //Decoding brick updates
            string[] brickUpdates = content[numberOfplayers+1].Split(';');

            int numberOfBricks = brickUpdates.Length;
            Point[] brickPositions = new Point[numberOfBricks];
            BrickDamageLevel[] currentBrickDamageLevel = new BrickDamageLevel[numberOfBricks];
            for (int i = 0; i < numberOfBricks; i++)
            {
                string[] currentBrickData = brickUpdates[i].Split(',');
                string brickCoordinate = currentBrickData[0] +"," + currentBrickData[1];
                brickPositions[i] = CoordinateToPoint(brickCoordinate);
                currentBrickDamageLevel[i] = (BrickDamageLevel)Convert.ToByte(currentBrickData[2]);
            }

            GameUpdateReceived(numberOfplayers, playerNumbers, playerCurrentPositions, playerCurrentDirections, playerShooted, playerCurrentHealth, playerCurrentCoins, playerCurrentPoints, numberOfBricks, brickPositions, currentBrickDamageLevel);

        }

        // decode coin update
        private void DecodeCoinUpdate(string message)
        {
            string[] content = message.Split(new char[] { ':', '#' });
            Point coinPosition = CoordinateToPoint(content[1]);
            int coinLifeTime = Convert.ToInt32(content[2]);
            int coinValue = Convert.ToInt32(content[3]);
            CoinUpdateReceived(coinPosition, coinLifeTime, coinValue);
        }
        // decode life update
        private void DecodeLifeUpdate(string message)
        {
            string[] content = message.Split(new char[] { ':', '#' });
            Point lifePackPosition = CoordinateToPoint(content[1]);
            int lifePackLifeTime = Convert.ToInt32(content[2]);
            LifeUpdateReceived(lifePackPosition, lifePackLifeTime);
        }

    }
}
