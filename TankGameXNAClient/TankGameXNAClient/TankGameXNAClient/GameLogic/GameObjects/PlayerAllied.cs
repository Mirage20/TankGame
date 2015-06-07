using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    class PlayerAllied : Player
    {
        float RotationAngle;
        public PlayerAllied()
        {
            playerTexture = Global.Content.Load<Texture2D>("tank");
            fontPlayer = Global.Content.Load<SpriteFont>("SpriteFontPlayer");
        }
        public override void Draw()
        {
            RotationAngle = (float)PlayerDirection * 90;
            RotationAngle = RotationAngle * MathHelper.Pi / 180;

            if (OldX == X && OldY == Y)
            {
                SmoothX = 0;
                SmoothY = 0;
                Global.SpriteBatch.Draw(playerTexture, new Vector2(10 + (32 * Global.ScaleFactor) + (X * Global.ScaleFactor * 64), 10 + (32 * Global.ScaleFactor) + (Y * Global.ScaleFactor * 64)), null, Color.White, RotationAngle, new Vector2(32, 32), Global.ScaleFactor, SpriteEffects.None, 0);
                Global.SpriteBatch.DrawString(fontPlayer, PlayerNumber.ToString(), new Vector2(5 + (32 * Global.ScaleFactor) + (X * Global.ScaleFactor * 64), 5 + (32 * Global.ScaleFactor) + (Y * Global.ScaleFactor * 64)), Color.White);
            }
            else
            {
                float speed = .85f;
                SmoothX += speed * (float)Global.GameTime.ElapsedGameTime.TotalSeconds * (X - OldX);
                SmoothY += speed * (float)Global.GameTime.ElapsedGameTime.TotalSeconds * (Y - OldY);
                
                Global.SpriteBatch.Draw(playerTexture, new Vector2(10 + (32 * Global.ScaleFactor) + ((OldX + SmoothX) * Global.ScaleFactor * 64), 10 + (32 * Global.ScaleFactor) + ((OldY + SmoothY) * Global.ScaleFactor * 64)), null, Color.White, RotationAngle, new Vector2(32, 32), Global.ScaleFactor, SpriteEffects.None, 0);
                Global.SpriteBatch.DrawString(fontPlayer, PlayerNumber.ToString(), new Vector2(5 + (32 * Global.ScaleFactor) + ((OldX + SmoothX) * Global.ScaleFactor * 64), 5 + (32 * Global.ScaleFactor) + ((OldY + SmoothY) * Global.ScaleFactor * 64)), Color.White);

                if ((OldX + (int)SmoothX) == X)
                    OldX = X;
                if ((OldY + (int)SmoothY) == Y)
                    OldY = Y;
            }
        }
    }
}
