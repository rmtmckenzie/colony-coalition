using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition.Objects
{
    enum bulletloc{frontleft,frontright,backleft,backright}

    class Ship : SingleObject
    {

        //TODO: Move this!
        private const float VelocityScale = 0.5f;
        private const float RotationScale = 0.023f;

        private Bullets bulletsfront;
        private Bullets bulletsback;
        public float health;
        public float temp_health;

        private KeyboardState keystate;
        private GamePadState gpStateOne;
        private GamePadState gpStateTwo;

        private DrawingComponents drawingComponents;


        TestSphere shipfront;
        TestSphere shipback;
        

        private float movedamping = 1f;
        private float shielddamping = 0.1f;

        //this this the array of bounding spheres for collision detection
        //rather than one big sphere, we are going to use 5 small spheres
        public ShipNode[] shipBounds = new ShipNode[5];

        public Vector3 p1DirectionVector = Vector3.Zero;
        public Vector3 p2DirectionVector = Vector3.Zero;
        public Vector3 p1ShieldVector = Vector3.Zero;
        public Vector3 p2ShieldVector = Vector3.Zero;

        
        public Ship(
            GraphicsProperties gp,string model_s, float scale = 1)
                : base(gp,model_s, scale)
        {
            this.bulletsfront = new Objects.Bullets(gp,GameConstants.mov_maxBullets);
            this.bulletsback = new Objects.Bullets(gp, GameConstants.mov_maxBullets);
            this.drawingComponents = new DrawingComponents(gp);
            this.shipfront = new Objects.TestSphere(gp,Color.Blue);
            this.shipback = new Objects.TestSphere(gp,Color.Red);
        }

        //TODO: Add sound handlers
        public override void Update(GameTime gameTime) 
        {

            //add velocity to current position
            Position += Velocity * gameTime.ElapsedGameTime.Milliseconds * VelocityScale;

            //add rotation
            Rotation += rotationdelta * gameTime.ElapsedGameTime.Milliseconds * RotationScale;

            //make velocity 'bleed' off over time
            Velocity *= 0.99f;
            rotationdelta *= 0.9f;

            world = GameConstants.rotationX * Matrix.CreateRotationZ(Rotation) * Matrix.CreateTranslation(Position);

            bulletsfront.Update(gameTime);
            bulletsback.Update(gameTime);

            temp_health = 0;
            for (int i = 0; i < shipBounds.Length; i++)
            {
                //move spheres
                shipBounds[i].bounds.Center = world.Translation + world.Up * shipBounds[i].nodeoffset;

                if (shipBounds[i].health < 0)
                    shipBounds[i].health = 0;

                temp_health += shipBounds[i].health;
            }

            health = temp_health / shipBounds.Length;

            drawingComponents.Update(p1ShieldVector,p2ShieldVector,world);
            shipfront.Update(gameTime, Matrix.CreateTranslation(world.Translation + world.Up*500f + world.Backward*2500f + p1DirectionVector * 400f));
            shipback.Update(gameTime, Matrix.CreateTranslation(world.Translation + world.Up*500f + world.Forward * 2500f + p2DirectionVector * 400f));
        }

        public override void HandleInput(InputState input) 
        {

            //TEMP - to fix so that works for players
            keystate = input.CurrentKeyboardStates[1];
            gpStateOne = input.CurrentGamePadStates[0];
            gpStateTwo = input.CurrentGamePadStates[1];

            p1DirectionVector = new Vector3(gpStateOne.ThumbSticks.Left,0);
            p2DirectionVector = new Vector3(gpStateTwo.ThumbSticks.Left,0);
            p1DirectionVector.X += (keystate.IsKeyDown(KeyTable.p1_right ) ? movedamping : 0)
                                 + (keystate.IsKeyDown(KeyTable.p1_left) ? -movedamping : 0);
            p1DirectionVector.Y += (keystate.IsKeyDown(KeyTable.p1_up) ? movedamping : 0)
                                 + (keystate.IsKeyDown(KeyTable.p1_down) ? -movedamping : 0);

            p2DirectionVector.X += (keystate.IsKeyDown(KeyTable.p2_right) ? movedamping : 0)
                                 + (keystate.IsKeyDown(KeyTable.p2_left) ? -movedamping : 0);
            p2DirectionVector.Y += (keystate.IsKeyDown(KeyTable.p2_up) ? movedamping : 0)
                                 + (keystate.IsKeyDown(KeyTable.p2_down) ? -movedamping : 0);

            p1ShieldVector += new Vector3(gpStateOne.ThumbSticks.Right,0);
            p2ShieldVector += new Vector3(gpStateTwo.ThumbSticks.Right,0);
            p1ShieldVector.X += (keystate.IsKeyDown(KeyTable.p1_shield_right) ? shielddamping : 0)
                                    + (keystate.IsKeyDown(KeyTable.p1_shield_left) ? -shielddamping : 0);
            p1ShieldVector.Y += (keystate.IsKeyDown(KeyTable.p1_shield_up) ? shielddamping : 0)
                                    + (keystate.IsKeyDown(KeyTable.p1_shield_down) ? -shielddamping : 0);

            p2ShieldVector.X += (keystate.IsKeyDown(KeyTable.p2_shield_right) ? shielddamping : 0)
                                    + (keystate.IsKeyDown(KeyTable.p2_shield_left) ? -shielddamping : 0);
            p2ShieldVector.Y += (keystate.IsKeyDown(KeyTable.p2_shield_up) ? shielddamping : 0)
                                    + (keystate.IsKeyDown(KeyTable.p2_shield_down) ? -shielddamping : 0);


            Velocity += p1DirectionVector + p2DirectionVector;
            
            rotationdelta += RotationScale*(Vector3.Dot(p1DirectionVector, world.Right) + Vector3.Dot(p2DirectionVector, world.Left));
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            bulletsfront.Draw(gameTime);
            bulletsback.Draw(gameTime);
            shipfront.Draw(gameTime);
            shipback.Draw(gameTime);
            drawingComponents.Draw(gameTime);
        }
        
    }

    public struct ShipNode
    {
        public BoundingSphere bounds;
        public float health;
        public int nodesize;
        public int nodeoffset;

        //armor value, critical systems?

        public ShipNode(int nodesize, int nodeoffset, Vector3 updirection, float health = 1)
        {
            this.bounds = new BoundingSphere(updirection * nodeoffset, nodesize);
            this.health = health;
            this.nodesize = nodesize;
            this.nodeoffset = nodeoffset;
        }

    }
}
