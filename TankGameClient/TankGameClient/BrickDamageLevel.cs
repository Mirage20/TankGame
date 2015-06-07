using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGameClient
{
    public enum BrickDamageLevel : byte
    {
        NoDamage = 0,
        LightDamage = 1,
        MediumDamage = 2,
        HeavyDamage = 3,
        FullyDamage = 4
    }
}
