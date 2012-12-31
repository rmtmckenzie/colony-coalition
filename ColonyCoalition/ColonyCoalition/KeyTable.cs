using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ColonyCoalition
{
    class KeyTable
    {
        //movement
        public static Keys p1_up = Keys.F;
        public static Keys p1_down = Keys.V;
        public static Keys p1_left = Keys.C;
        public static Keys p1_right = Keys.B;
        public static Keys p2_up = Keys.Up;
        public static Keys p2_down = Keys.Down;
        public static Keys p2_left = Keys.Left;
        public static Keys p2_right = Keys.Right;
        //shield
        public static Keys p1_shield_up = Keys.W;
        public static Keys p1_shield_down = Keys.S;
        public static Keys p1_shield_left = Keys.A;
        public static Keys p1_shield_right = Keys.D;
        public static Keys p2_shield_up = Keys.P;
        public static Keys p2_shield_down = Keys.OemSemicolon;
        public static Keys p2_shield_left = Keys.L;
        public static Keys p2_shield_right = Keys.OemQuotes;
        //bullets
        public static Keys p1_bullet_left = Keys.Q;
        public static Keys p1_bullet_right = Keys.E;
        public static Keys p2_bullet_left = Keys.O;
        public static Keys p2_bullet_right = Keys.OemOpenBrackets;

    }
}
