using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    //Manages items in the games such as coin piles and life packs
    class ItemManager
    {

        private List<CoinPile> coinPiles;
        private List<LifePack> lifePacks;

        public ItemManager()
        {
            coinPiles = new List<CoinPile>();
            lifePacks = new List<LifePack>();
        }


        // add a coin pile to the list
        public void AddCoinPile(System.Drawing.Point coinPosition, int coinLifeTime, int coinValue)
        {
            CoinPile coin = new CoinPile();
            coin.X = coinPosition.X;
            coin.Y = coinPosition.Y;
            coin.LifeTime = coinLifeTime;
            coin.DisappearTime = (int)Global.GameTime.TotalGameTime.TotalMilliseconds + coin.LifeTime;
            coin.CoinValue = coinValue;
            coinPiles.Add(coin);
        }
        // add a life pack to the list
        public void AddLifePack(System.Drawing.Point lifePackPosition, int lifePackLifeTime)
        {
            LifePack life = new LifePack();
            life.X = lifePackPosition.X;
            life.Y = lifePackPosition.Y;
            life.LifeTime = lifePackLifeTime;
            life.DisappearTime = (int)Global.GameTime.TotalGameTime.TotalMilliseconds + life.LifeTime;
            lifePacks.Add(life);
        }

        //return all coin piles
        public List<CoinPile> GetCoinPiles()
        {
            return coinPiles;
        }

        // remove selected coin pile from the list
        public void RemoveCoinPile(CoinPile coinPile)
        {
            coinPiles.Remove(coinPile);
        }
        //return all life packs
        public List<LifePack> GetLifePacks()
        {
            return lifePacks;
        }
        // remove selected life pack  from the list
        public void RemoveLifePack(LifePack lifePack)
        {
            lifePacks.Remove(lifePack);
        }

        //draw all the coin piles and life packs to the gui
        public void Draw()
        {

            for (int i = 0; i < coinPiles.Count; i++)
            {
                coinPiles[i].Draw();
                if ((Global.GameTime.TotalGameTime.TotalMilliseconds > coinPiles[i].DisappearTime))
                    coinPiles.Remove(coinPiles[i--]);
            }

            for (int i = 0; i < lifePacks.Count; i++)
            {
                lifePacks[i].Draw();
                if ((Global.GameTime.TotalGameTime.TotalMilliseconds > lifePacks[i].DisappearTime))
                    lifePacks.Remove(lifePacks[i]);
            }

        }
    }
}
