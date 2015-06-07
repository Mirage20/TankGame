using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace TankGameXNAClient
{
    class Global
    {
        //XNA Common Objects
        public static ContentManager Content;
        public static SpriteBatch SpriteBatch;
        public static GameTime GameTime;

        // Score Board
        public static int Top = 200;
        public static int Left = 620;

        //Grid Size
        public static int GridSizeX = 20;
        public static int GridSizeY = 20;

        //Scaling
        public static float ScaleFactor = .45f;

        //Network
        //public static string ServerIP = "10.8.106.49";
        //public static string ClientIP = "10.8.99.49";
        public static string ServerIP = "127.0.0.1"; 
        public static string ClientIP = "127.0.0.1";
        public static int ServerPort = 6000;
        public static int ClientPort = 7000;

        //Graphics
        public static int ViewportWidth=1000;
        public static int ViewportHeight = 600;
        
        //AI Options
        public static bool IsPeaceful = false;
        public static int LineOfSight = 3;
        public static int ScanRange = 20;
        public static int MaxHealthTarget = 140;
        public static int CriticalHealth = 50;
        public static bool EnableOccupiedCellDetection = true;
        public static bool EnableKillSteal = true;
        public static int KillStealRange = 5;
        public static int KillStealHealth = 40;
    }
}
