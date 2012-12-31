using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ColonyCoalition.Objects
{
    class Asteroids : MultiObject
    {
        public Asteroids(GraphicsProperties gp,
                         int max,
                         string model = GameConstants.content_m_asteroid,
                         float scale = 1) :
            base(gp, max, model, scale) { }
        ///DONT FORGET TO SET ALL ASTEROIDS TO BE PREPROCESSED BY THE PREPROCESSOR FROM MICROSOFT, IF DOING THIS WAY!!!!
        ///

        public override void HandleInput(InputState input) { }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime,new ObjsRefDelegate(FlipX),new ObjsRefDelegate(Delete));
        }

        public override void New(int number)
        {
            bool full = false;
            Random rand = new Random();
            //call the base class for each of the items to be created.
            //If the array is full, the full boolean is set to true, and
            //insertion stops.
            for (int i = 0; i < number && !full; i++)
            {
                //this is creating a new asteroid
                //parameters are:
                //  position: starts above playfield at random position
                //  velocity: starts aimed downwards at random angle
                //      doesn't worry about going out of screen because when goes out
                //      reappears on other side
                //  spinposition: zero
                //  spinspeed: 3 dimentional spin vector (random)
                //  size: between min and max set in constants
                //  health: same as size
                // out full: this is set if the object is full
                // so that can stop trying

                Vector3 position = new Vector3(
                        ((float)(rand.NextDouble()) - 0.5f) * GameVariables.playfieldSizeX*2,
                        GameVariables.playfieldSizeY * 1.10f,
                        0f);
                Vector3 velocity = new Vector3(
                        ((float)(rand.NextDouble()) - 0.5f) *10f * GameVariables.asteroidSpeed,
                        -(float)rand.NextDouble()*6f * GameVariables.asteroidSpeed,
                        0f);
                Vector3 spinAxis = new Vector3(
                        (float)rand.NextDouble(),
                        (float)rand.NextDouble(),
                        (float)rand.NextDouble());
                spinAxis.Normalize();
                float spinAngle = (float)rand.NextDouble() * 360f;
                float spinSpeed = ((float)rand.NextDouble() * .4f - .2f);
                float size = GameConstants.asteroidMin + (GameConstants.asteroidMax-GameConstants.asteroidMin)*(float)rand.NextDouble();
                float health = size;
                base.New(position, velocity, spinAxis, spinAngle, spinSpeed, size, health, out full);

            }
        }
    }
}
