using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace lk.ac.mrt.cse.pc11.bean
{
    /// <summary>
    /// Represents a CoinPile
    /// </summary>
    public class CoinPile : MapItem
    {
        #region "Variables"
        
        private int price = 0;
       
        #endregion

        public CoinPile(int x, int y): base(x,y)
        {
            
        }

        #region "Properties"
       

        public int Price
        {
            get { return price; }
            set { price = value; }
        }

       
        #endregion
    }
}
