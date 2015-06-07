using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    class TankShell
    {
        private Texture2D shellTexture;
        private float RotationAngle;
        private bool hasMoved = false;

        public bool HasMoved
        {
            get { return hasMoved; }
            set { hasMoved = value; }
        }
        public int X { get; set; }
        public int Y { get; set; }
        public Direction ShellDirection { get; set; }

        private float shellTimer = 250;         
        
        public TankShell()
        {
            shellTexture = Global.Content.Load<Texture2D>("tank_shell");
        }

        public void Draw()
        {
            RotationAngle = (float)ShellDirection * 90;
            RotationAngle = RotationAngle * MathHelper.Pi / 180;
            
            float elapsed = (float)Global.GameTime.ElapsedGameTime.TotalMilliseconds;
            shellTimer -= elapsed;
            if (shellTimer < 0)
            {                
                Y += (int)Math.Round(-Math.Cos(RotationAngle), 0, MidpointRounding.AwayFromZero);
                X += (int)Math.Round(Math.Sin(RotationAngle), 0, MidpointRounding.AwayFromZero);
                shellTimer = 250;
                hasMoved = true;
            }
                       
            Global.SpriteBatch.Draw(shellTexture, new Vector2(10 + (32 * Global.ScaleFactor) + (X * Global.ScaleFactor * 64), 10 + (32 * Global.ScaleFactor) + (Y * Global.ScaleFactor * 64)), null, Color.White, RotationAngle, new Vector2(32, 32), Global.ScaleFactor, SpriteEffects.None, 0);
        }
    }
}
