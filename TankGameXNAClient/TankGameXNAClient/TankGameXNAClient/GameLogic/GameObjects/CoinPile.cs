using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    class CoinPile
    {
        public int X { get; set; }
        public int Y { get; set; }
        private SpriteFont fontGameItem { get; set; }
        private Texture2D coinTexture { get; set; }
        public int LifeTime { get; set; }
        public int DisappearTime { get; set; }
        public int CoinValue { get; set; }
        public CoinPile()
        {
            coinTexture = Global.Content.Load<Texture2D>("coin");
            fontGameItem = Global.Content.Load<SpriteFont>("SpriteFontGameItem");
        }
        public void Draw()
        {
            
            Global.SpriteBatch.Draw(coinTexture, new Vector2(10 + (X * Global.ScaleFactor * 64), 10 + (Y * Global.ScaleFactor * 64)), null, Color.White, 0.0f, new Vector2(0, 0), Global.ScaleFactor, SpriteEffects.None, 0);
            Global.SpriteBatch.DrawString(fontGameItem,CoinValue.ToString(), new Vector2(5 + (20 * Global.ScaleFactor) + (X * Global.ScaleFactor * 64), 5 + (24 * Global.ScaleFactor) + (Y * Global.ScaleFactor * 64)), Color.Black);
            Global.SpriteBatch.DrawString(fontGameItem, ((DisappearTime-(int)Global.GameTime.TotalGameTime.TotalMilliseconds)/1000).ToString(), new Vector2(5 + (28 * Global.ScaleFactor) + (X * Global.ScaleFactor * 64), 5 + (40 * Global.ScaleFactor) + (Y * Global.ScaleFactor * 64)), Color.Black);

        }
    }
}
