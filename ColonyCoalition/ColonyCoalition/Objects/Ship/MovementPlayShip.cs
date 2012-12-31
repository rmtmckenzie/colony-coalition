using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition.Objects
{
    class MovementPlayShip : Ship
    {

        public MovementPlayShip(
            GraphicsProperties gp,
            string model_s = GameConstants.content_m_ship, 
            float scale = GameConstants.mov_ship_scale)
                : base(gp,model_s,scale) 
        {
            
        
        }        

    }
}
