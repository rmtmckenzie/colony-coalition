#region File Description
//-----------------------------------------------------------------------------
// MovementScreen.cs
//
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion


namespace ColonyCoalition
{
    class MovementPlayScreen : PlayScreen
    {
        #region Fields

        MyContentManager content;
        float pauseAlpha;
        float cameraHeight;

        System.TimeSpan PlayTime;
        float timeToNextAsteroid;
        Random random;
        GraphicsProperties gp;
        SpriteBatch spriteBatch;
        Rectangle ViewRectangle;
        Stars stars;
        RenderTarget2D sceneRenderTarget;
        

        #region Camera
        Vector3 cameraPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        float aspectRatio;
        #endregion

        #region Objects
        Objects.MovementPlayShip ship;
        Objects.Asteroids asteroids;
        #endregion

        #region GUI Assets
        //Load GUI assets
        SpriteFont font;
        Texture2D background, whiteBox, greyBox, shipdisplay;
        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public MovementPlayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load content (graphics) for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new MyContentManager(ScreenManager.Game.Services, "Content");
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            //todo: implement asteroid, ship, and bullet. They should contain a model
            //      string saying which model to use
            //  Note: It is believed that the most efficient way of doing this is to
            //      have a class for each type - i.e. Asteroid, Bullet
            //      that handles all of the functions for the set of 
            //      individual objects (structs). This way, each structure
            //      can simply contain the information needed (such as ID,
            //      size, rotation, model, etc) and have the drawing or moving
            //      handled by a loop in the parent class. Note that the structures
            //      can contain individual functions, but since structs are not 
            //      inheritable this might not make sense. Instead, the classes
            //      will inherit from a 'MultiObject' class.


            //possible something to do: seed all randoms so that
            // each gameplay is the same for starting difficulties.
            random = new Random();

            #region Setup Camera & Playfield
            //initialize camera information.
            // note: MUST Be done before initializing models
            cameraHeight = GameConstants.cameraHeight_o * ScreenManager.GraphicsDevice.Viewport.Width / GameConstants.windowWidth_o;
            cameraPosition = new Vector3(0.0f, 0.0f, cameraHeight);
            aspectRatio = ScreenManager.GraphicsDevice.Viewport.AspectRatio;
            GameVariables.playfieldSizeX = GameConstants.playFieldSizeX_o;
            GameVariables.playfieldSizeY = GameConstants.playFieldSizeX_o / aspectRatio;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio,
                cameraHeight - cameraHeight * .6f,
                cameraHeight + cameraHeight * .6f);
            viewMatrix = Matrix.CreateLookAt(cameraPosition,
                Vector3.Zero, Vector3.Up);

            #endregion
            sceneRenderTarget = new RenderTarget2D(
                ScreenManager.GraphicsDevice,
                GameVariables.win_Width,
                GameVariables.win_Height,
                false,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                ScreenManager.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                ScreenManager.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);


            gp = new GraphicsProperties(
                    projectionMatrix,
                    viewMatrix,
                    content,
                    ScreenManager.GraphicsDevice,
                    spriteBatch,
                    sceneRenderTarget);

            #region Initialize Object Classes + models + gui assets

            //note that models for the objects are loaded into the contentmanager
            // which is passed into the objects.
            ship = new Objects.MovementPlayShip(gp);
            asteroids = new Objects.Asteroids(gp, GameConstants.mov_maxAsteroids);

            //Load GUI assets
            font = content.Load<SpriteFont>("Fonts/SpriteFont1");
            background = content.Load<Texture2D>("Textures/spacebackground");
            whiteBox = content.Load<Texture2D>("Textures/whiteBox");
            greyBox = content.Load<Texture2D>("Textures/greyBox");
            shipdisplay = content.Load<Texture2D>("Textures/shipDiagram");

            ViewRectangle = new Rectangle(0, 0, GameVariables.win_Width, GameVariables.win_Height);

            stars = new Stars(content);



            #endregion

            //tell garbage collector that loading is done!
            GC.Collect();

            //could be long load - tell game not to try to catch up
            ScreenManager.Game.ResetElapsedTime();
            PlayTime = TimeSpan.Zero;
            timeToNextAsteroid = 0f;



        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update, Input and Draw

        /// <summary>
        /// Perform Draw
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime,
                                    bool otherScreenHasFocus,
                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);


            // Gradually fade in or out depending on whether we are covered
            // by the pause screen
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                PlayTime += gameTime.ElapsedGameTime;
                timeToNextAsteroid -= gameTime.ElapsedGameTime.Milliseconds / 1000f;
                if (timeToNextAsteroid < 0)
                {
                    timeToNextAsteroid = (float)random.NextDouble() * GameVariables.nextAsteroidAdjust;
                    asteroids.New();
                }

                ship.Update(gameTime);
                asteroids.Update(gameTime);
                stars.Update(gameTime);
            }

        }

        /// <summary>
        /// Handles Input
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //add more for keypad disconnect, etc.
            // Right now this works on platform of 'controllingplayer'... this should be
            // changed eventually since input will be coming from two players.
            if (input.IsPauseGame(null))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                ship.HandleInput(input);
            }
            if (input.CurrentKeyboardStates[1].IsKeyDown(Keys.OemPlus))
            {
                GameVariables.nextAsteroidAdjust *= .99f;
                GameVariables.asteroidSpeed *= 1.01f;

            }

        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {

            gp.GraphicsDevice.SetRenderTarget(sceneRenderTarget);
            gp.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(background, ViewRectangle,Color.White);

            stars.Draw(spriteBatch);

            spriteBatch.End();
            
            gp.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            asteroids.Draw(gameTime);
            //draw ship (bullets) and asteroids
            ship.Draw(gameTime);
            

            

            ///////DEBUG
            //SpriteBatch n = new SpriteBatch(ScreenManager.GraphicsDevice);
            //n.Begin();
            //n.DrawString(ScreenManager.Font,
            //            ship.Position.ToString(),
            //            new Vector2(200, 200),
            //            Color.LightGreen,
            //            0, 
            //            new Vector2(0f,0f), 
            //            0.8f, 
            //            SpriteEffects.None, 
            //            0.5f);
            //n.DrawString(ScreenManager.Font,
            //    "PlayX: " + GameVariables.playfieldSizeX.ToString() + "PlayY:" + GameVariables.playfieldSizeY.ToString(),
            //    new Vector2(200, 300),
            //    Color.Wheat);

            //n.End();
            /////END DEBUG

            // This could be put in?
            base.Draw(gameTime); 
        }



        #endregion

    }
}
