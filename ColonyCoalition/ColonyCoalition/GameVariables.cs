using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace ColonyCoalition
{
    class GameVariables
    {
        //Assumption:
        // for any particular game these are set then not changed!
        #region Screen
        public static int win_Width = 853;
        public static int win_Height = 600;


        public static float playfieldSizeX = 16000f;
        public static float playfieldSizeY = 12500f;

        public static float asteroidSpeed = 500f;

        #endregion

        public static float nextAsteroidAdjust = 5f;
    }
}
