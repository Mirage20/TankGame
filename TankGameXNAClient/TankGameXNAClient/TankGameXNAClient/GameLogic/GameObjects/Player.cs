using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    abstract class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        protected int OldX { get; set; }
        protected int OldY { get; set; }
        protected float SmoothX { get; set; }
        protected float SmoothY { get; set; }
        public int PlayerNumber { get; set; }
        public Direction PlayerDirection { get; set; }
        protected Texture2D playerTexture { get; set; }
        protected SpriteFont fontPlayer { get; set; }
        public bool IsShooted { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentCoins { get; set; }
        public int CurrentPoints { get; set; }
        public abstract void Draw();
    }
}
