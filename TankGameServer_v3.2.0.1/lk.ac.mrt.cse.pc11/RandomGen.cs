using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace lk.ac.mrt.cse.pc11
{
    class RandomGen
    {
        public int random(int limit)
        {            
            Random rand = new Random();
            return (rand.Next(-1, limit + 1));

        }

        public int randomD(int limit)//Random with delay
        {
            Thread.Sleep(10);
            Random rand = new Random();
            int temp = rand.Next(0, limit + 1);
            if (temp >= 0)
            {
                return (temp);
            }
            else
            {
                return (randomD(limit));
            }

        }

        public int randomDD(int limit)//Random with double delay
        {
            Thread.Sleep(10);
            Random rand = new Random(DateTime.Now.Millisecond + randomD(10000));
            int temp = rand.Next(0, limit + 1);
            if (temp >= 0)
            {
                return (temp);
            }
            else
            {
                return (randomDD(limit));
            }

        }

        public int randomDP(int limit,int size,int previous)//Random with delay and prevent 0 5 9
        {
            Thread.Sleep(10);
            Random rand = new Random();
            int result=rand.Next(0, limit + 1);
            if ((result != 0) & (result != size / 2) & (result != (size - 1)) & (result != previous) & (result != (size-previous)))
            {
                return (result);
            }
            else
            {
                return (randomDP(limit, size, previous));
            }

        }

        public int randomD(int begin, int end)//Random with delay and between numbers begin and end
        {
            Thread.Sleep(10);
            Random rand = new Random();
            int result = (rand.Next((begin*10), ((end + 1)*10)))/10;
            return (result);
        }
    }
}
