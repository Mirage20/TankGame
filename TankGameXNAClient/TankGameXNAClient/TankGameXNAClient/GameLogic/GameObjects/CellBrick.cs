using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    class CellBrick : Cell
    {

        private BrickDamageLevel damageLevel = BrickDamageLevel.NoDamage;
        private Color brickColorMask = Color.White;
        public BrickDamageLevel DamageLevel
        {
            get
            {
                return damageLevel;
            }
            set
            {
                switch (value)
                {
                    case BrickDamageLevel.NoDamage:
                        break;
                    case BrickDamageLevel.LightDamage:
                        cellTexture = Global.Content.Load<Texture2D>("brick_25");
                        break;
                    case BrickDamageLevel.MediumDamage:
                        cellTexture = Global.Content.Load<Texture2D>("brick_50");
                        break;
                    case BrickDamageLevel.HeavyDamage:
                        cellTexture = Global.Content.Load<Texture2D>("brick_75");
                        break;
                    case BrickDamageLevel.FullyDamage:
                        brickColorMask = Color.White;
                        IsBlockedMove = false;
                        cellTexture = Global.Content.Load<Texture2D>("sand");
                        break;
                    default:
                        break;
                }
                damageLevel = value;
            }
        }
        public CellBrick()
        {
            IsBlockedMove = true;
            cellTexture = Global.Content.Load<Texture2D>("brick");
        }
        public override void Draw()
        {
            Global.SpriteBatch.Draw(cellTexture, new Vector2(10 + (X * Global.ScaleFactor * 64), 10 + (Y * Global.ScaleFactor * 64)), null, brickColorMask, 0.0f, new Vector2(0, 0), Global.ScaleFactor, SpriteEffects.None, 0);

        }
    }
}
