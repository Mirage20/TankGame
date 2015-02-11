using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace lk.ac.mrt.cse.pc11.bean
{
    public class MapItem
    {
        private Point position;
        private int lifeTime = -1;
        private int appearTime = -1;
        private int disappearTime = -1;
        private int disappearBalance = -1;

        public int DisappearBalance
        {
            get { return disappearBalance; }
            set { disappearBalance = value; }
        }

        public MapItem(int x, int y)
        {
            Position = new Point(x, y);
        }


        public Point Position
        {
            get { return position; }
            set { position = value; }
        }


        public int LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }

        public int AppearTime
        {
            get { return appearTime; }
            set { appearTime = value; }
        }

        public int DisappearTime
        {
            get { return disappearTime; }
            set { disappearTime = value; }
        }

    }
}
