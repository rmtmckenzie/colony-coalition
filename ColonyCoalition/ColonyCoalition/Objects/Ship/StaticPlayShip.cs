using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition.Objects
{
    class StaticPlayShip : Ship
    {
        //public StaticPlayScreen Parent;

        public StaticPlayShip(
            GraphicsProperties gp,
            string model_s = GameConstants.content_m_ship, 
            float scale = 10f) 
                : base(gp, model_s, scale) 
        {
            //Parent = parent;
        }
    }
}
