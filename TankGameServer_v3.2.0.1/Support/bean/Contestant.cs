using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using lk.ac.mrt.cse.pc11.util;

namespace lk.ac.mrt.cse.pc11.bean
{
    /// <summary>
    /// Represents a Contestant
    /// </summary>
    public class Contestant
    {
        #region "Variables"
        private string name = "";
        private string ip = "";
        private int port = -1;                
        private Point startP;
        private Point currentP;
        private int direction = 0;
        private Boolean shot = false;
        private int pointsEarned = 0;
        private int coins = 0;
        private int health = Constant.PLAYER_HEALTH;   
        private bool isAlive = true;
        private bool invalidCell = false;
        private DateTime updatedTime;
        private int index = -1;


        public int Coins
        {
            get { return coins; }
            set { coins = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }


        public Boolean Shot
        {
            get { return shot; }
            set { shot = value; }
        }

        public int Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        
        #endregion

        public Contestant(string cName, string ipAdd,int cPort)
        {
            name = cName;
            ip = ipAdd;
            port = cPort;
        }

        #region "Properties"
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public Point StartP
        {
            get { return startP; }
            set { startP = value; }
        }

        public Point CurrentP
        {
            get { return currentP; }
            set { currentP = value; }
        }

        public DateTime UpdatedTime
        {
            get { return updatedTime; }
            set { updatedTime = value; }
        }

        public int PointsEarned
        {
            get { return pointsEarned; }
            set { pointsEarned = value; }
        }

        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public bool InvalidCell
        {
            get { return invalidCell; }
            set { invalidCell = value; }
        }
        #endregion
    }
}
