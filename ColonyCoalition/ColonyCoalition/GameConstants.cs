using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ColonyCoalition
{
    internal class GameConstants
    {

        #region MovementGame AsteroidSpawn
        public const int mov_maxAsteroids = 600;
        public const int mov_maxBullets = 400;

        //Local constants to determine asteroid spawn rate/position
        public const float spawnY = 40f; //spawn location Y (should be off screen)
        public const float spawnX = 20f; //spawn location X (amount left or right of center

        public const float spawnMax = 2f; //Max size will actually be spawnMax + spawnMin...
        public const float spawnMin = 5f; //these determine spawn rate of asteroid

        public const float sizeMax = 2f;  //Max size will actually be sizeMax + sizeMin...
        public const float sizeMin = 1f;     //these will determine size of new asteroid
        #endregion

        #region Object Stuff
        //Fixed arrays to define shipNodes, should be static to save space
        public static float[] ShipNodeOffsets = { 4f, 2.2f, 0f, -2.2f, -4f, };
        public static float[] ShipNodeSizes = { 1f, 1.1f, 1.8f, 1.1f, 1f };
        public const float mov_ship_scale = 10f;
        public const float asteroidMin = 5f;
        public const float asteroidMax = 15f;

        #endregion

        #region Content Names
        // Names of different content to be loaded

        #region Models
        public const string content_m_asteroid = "Models\\asteroid_lowpoly";
        public const string content_m_bullet = "Models\\bullet";
        public const string content_m_ship = "Models\\ship_e";
        public const string content_m_testsphere = "Models\\sphere";
        #endregion

        #region Textures
        public const string content_t_background = "Textures\\background";
        public const string content_t_blank = "Textures\\blank";
        public const string content_t_gradient = "Textures\\gradient";
        public const string content_t_greybox = "Textures\\greybox";
        #endregion

        #region Fonts
        public const string content_f_gamefont = "Fonts\\gamefont";
        public const string content_f_menufont = "Fonts\\menufont";
        #endregion

        #endregion

        #region Camera

        //todo: make so cameraheight is right!
        public const int cameraHeight_o = 25000;
        public const int playFieldSizeX_o = 16000;
        public const int windowWidth_o = 853;
        #endregion

        public static Matrix rotationX = Matrix.CreateRotationX(MathHelper.PiOver2);


    }
}
