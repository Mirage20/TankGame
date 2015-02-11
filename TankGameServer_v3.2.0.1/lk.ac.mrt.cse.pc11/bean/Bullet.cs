using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace lk.ac.mrt.cse.pc11.bean
{
    class Bullet
    {
        Contestant shooter;
        int[] dirData = new Int32[2];
        int id = 0;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int[] DirData
        {
            get { return dirData; }
            set { dirData = value; }
        }

        public Contestant Shooter
        {
            get { return shooter; }
            set { shooter = value; }
        }
        Point pos;

        public Point Pos
        {
            get { return pos; }
            set { pos = value; }
        }
        int direction;

        public int Direction
        {
            get { return direction; }
            set { 
                direction = value;

                switch (direction)
                {
                    case 0:
                        dirData[0] = 0;
                        dirData[1] = -1;
                        break;
                    case 1:
                        dirData[0] = 1;
                        dirData[1] = 0;
                        break;
                    case 2:
                        dirData[0] = 0;
                        dirData[1] = 1;
                        break;
                    case 3:
                        dirData[0] = -1;
                        dirData[1] = 0;
                        break;
                    default:
                        break;
                }


            }
        }

        public Bullet(Contestant con, Point origin, int dir)
        {
            shooter = con;
            pos = origin;
            Direction = dir;
            Console.WriteLine("Bullet shot at " + pos.X + "," + pos.Y + "," + direction);
             RandomGen r = new RandomGen();
             id = r.randomD(1000);
        }
    }
}
