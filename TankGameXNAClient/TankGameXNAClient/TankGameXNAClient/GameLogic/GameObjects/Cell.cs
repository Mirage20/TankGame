using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    abstract class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int FScore { get; set; }
        public int GScore { get; set; }
        public bool IsBlockedMove { get; set; }
        protected Texture2D cellTexture { get; set; }
        public abstract void Draw();
    }
}
