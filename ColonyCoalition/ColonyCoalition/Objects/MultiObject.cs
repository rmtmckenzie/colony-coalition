using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition.Objects
{
    struct ObjectSingleton
    {
        //one thing to check that may speed it up
        //is using vectors and simply putting calculated things
        //into matrix.. takes slightly more room but may be faster
        public void initialize()
        {
            world = Matrix.Identity;
            velocity = Vector3.Zero;
            spinAxis = Vector3.Zero;
            spinAngle = 0f;
            spinSpeed = 0f;
            size = 0f;
            health = 0f;
            isActive = false;
        }

        public void set(Model model,
                        Vector3 position,
                        Vector3 velocity,
                        Vector3 spinAxis,
                        float spinSpeed,
                        float spinAngle,
                        float size,
                        float health,
                        bool isActive = true)
        {
            this.model = model;
            this.world = Matrix.CreateScale(size) *
                        Matrix.CreateFromAxisAngle(spinAxis, MathHelper.ToRadians(spinAngle)) *
                        Matrix.CreateTranslation(position + velocity);
            this.velocity = velocity;
            this.spinAxis = spinAxis;
            this.spinSpeed = spinSpeed;
            this.spinAngle = spinAngle;
            this.size = size;
            this.health = health;
            this.isActive = isActive;
        }

        public Model model;
        public Matrix world;
        public Vector3 velocity;
        public Vector3 spinAxis;
        public float spinAngle;
        public float spinSpeed;
        public float size;
        public float health;
        public bool isActive;



        public void Update(float elapsedTime)
        {
            spinAngle += spinSpeed * elapsedTime;
            world = Matrix.CreateScale(size) *
                    Matrix.CreateFromAxisAngle(spinAxis, MathHelper.ToRadians(spinAngle)) *
                    Matrix.CreateTranslation(world.Translation + velocity * elapsedTime);
        }
        public void Draw(Matrix[] Transforms)
        {
            DrawModel(Transforms);
        }

        public bool isInHorizBounds(float max_x)
        {
            if (Math.Abs(world.Translation.X) > max_x)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool isInVertBounds(float max_y)
        {
            if (Math.Abs(world.Translation.Y) > max_y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public void DrawModel(Matrix[] Transforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        Transforms[mesh.ParentBone.Index] *
                        world;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public void flipX()
        {
            world.Translation = world.Translation * new Vector3(-0.95f,1,0);
        }

        public void flipY()
        {
            world.Translation = world.Translation * new Vector3(1f,-.95f,0f);
        }

        public void delete()
        {
            isActive = false;
        }
    }


    abstract class MultiObject : Object
    {
        protected static ObjectSingleton[] objs;
        public delegate void ObjsRefDelegate(ref ObjectSingleton obj);

        public MultiObject(
                GraphicsProperties gp, int max, string model, float scale = 1)
            : base(gp, model, scale)
        {
            Max = max;
            objs = new ObjectSingleton[max];
            for (int i = 0; i < Max; i++)
            {
                objs[i].initialize();
            }
        }
        protected int max;
        public int Max
        {
            get { return max; }
            protected set { max = value; }
        }

        public void New()
        {
            New(1);
        }

        public abstract void New(int i);

        

        public void New(Vector3 position,
                        Vector3 velocity,
                        Vector3 spinAxis,
                        float spinAngle,
                        float spinSpeed,
                        float size,
                        float health,
                        out bool full)
        {
            full = true;
            for (int i = 0; i < max; i++)
            {
                if (!objs[i].isActive)
                {
                    objs[i].set(Model,position, velocity, spinAxis, spinAngle, spinSpeed, size, health);
                    full = false;
                    break;
                }
            }
        }
        
        public virtual void Update(GameTime gameTime,ObjsRefDelegate outX = null, ObjsRefDelegate outY = null,ObjsRefDelegate del = null)
        {
            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds/1000f + 
                                   gameTime.ElapsedGameTime.Seconds;
            for (int i = 0; i < Max; i++)
            {
                if (objs[i].isActive)
                {
                    objs[i].Update(elapsedSeconds);
                    if (outX != null && 
                        !objs[i].isInHorizBounds(GameVariables.playfieldSizeX))
                    {
                        outX(ref objs[i]);
                    }
                    if (outY != null &&
                        !objs[i].isInVertBounds(GameVariables.playfieldSizeY))
                    {
                        outY(ref objs[i]);
                    }
                    if (del != null)
                    {
                        del(ref objs[i]);
                    }
                }

                //collisions are checked elsewhere.

            }
        }

        public void Delete(ref ObjectSingleton obj)
        {
            obj.isActive = false;
        }

        public void Delete(int i)
        {
            objs[i % Max].delete();
        }

        public void FlipX(ref ObjectSingleton obj)
        {
            obj.flipX();
        }
        public void FlipY(ref ObjectSingleton obj)
        {
            obj.flipY();
        }

        //todo: add some functions here that will need to be implemented
        //by any subclasses

        //for example, draw, update, etc.

        //Note: one thing that should be done is add switch for draw : 
        //      instance vs hardware-instanced (See demo from microsoft)
        //Note that must make sure that use the .fx file in the demo to compile .fbx files.

        public override void Draw(GameTime gameTime) 
        {
            for (int i = 0; i < Max; i++)
            {
                if (objs[i].isActive)
                {
                    objs[i].Draw(Transforms);
                }
            }
        }

    }
}