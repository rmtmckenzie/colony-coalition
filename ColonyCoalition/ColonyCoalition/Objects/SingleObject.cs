using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace ColonyCoalition.Objects
{
    class SingleObject : Object
    {
        #region members
        //Velocity of the model, applied each frame to the model's position
        private Vector3 velocity;
        public Matrix RotationMatrix;
        public Matrix world;
        private float rotation;
        protected float rotationdelta;
        protected float alpha;

        
        #endregion

        #region Constructor
        public SingleObject() { }

        public SingleObject(
            GraphicsProperties gp, string model_s, float scale = 1)
                : base(gp, model_s, scale)
        {
            RotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
            velocity = Vector3.Zero;
            alpha = 1;
            world = new Matrix();
        }
        #endregion


        public float Rotation
        {
            get { return rotation; }
            set
            {
                float newVal = value;
                while (newVal >= MathHelper.TwoPi)
                {
                    newVal -= MathHelper.TwoPi;
                }
                while (newVal < 0)
                {
                    newVal += MathHelper.TwoPi;
                }

                if (rotation != value)
                {
                    rotation = value;
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                return world.Translation;
            }
            set
            {
                world.Translation = value;
            }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
            protected set { velocity = value; }
        }
        public override void Draw(GameTime gameTime)
        {
            DrawModel();
        }

        public void DrawModel()
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in Model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        Transforms[mesh.ParentBone.Index] *
                        world;
                    effect.Alpha = alpha;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

    }
}
