﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    class CellEmpty : Cell
    {
        

        public CellEmpty()
        {
            cellTexture = Global.Content.Load<Texture2D>("sand");
            IsBlockedMove = false;
        }

        public override void Draw()
        {          
            Global.SpriteBatch.Draw(cellTexture, new Vector2(10 + (X * Global.ScaleFactor * 64), 10 + (Y * Global.ScaleFactor * 64)), null, Color.White, 0.0f, new Vector2(0, 0), Global.ScaleFactor, SpriteEffects.None, 0);
        }
    }
}
