using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition.Objects
{
    abstract class Object
    {
        #region Members
        public delegate void EffectSetupDelegate(ref BasicEffect effect);
        protected string model_s;
        private Model model;
        private Matrix[] transforms;
        protected float scale;
        protected GraphicsProperties gp;
        public string Model_s
        {
            get { return model_s; }
            set { model_s = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public Matrix[] Transforms
        {
            get { return transforms; }
            set { transforms = value; }
        }
        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        #endregion

        #region Constructor
        public Object() { }

        public Object(GraphicsProperties gp, string model, float scale)
        {
            this.gp = gp;
            Model_s = model;
            Model = gp.ContentManager.Load<Model>(model);
            Scale = scale;
            Transforms = SetupEffectDefaults(gp.ProjectionMatrix,gp.ViewMatrix);
        }
        #endregion

        private Matrix[] SetupEffectDefaults(Matrix projectionMatrix, Matrix viewMatrix, float alpha = 1)
        {
            Matrix[] absoluteTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            for (int i = 0; i < Model.Bones.Count; i++)
            {
                absoluteTransforms[i] = absoluteTransforms[i] * Matrix.CreateScale(Scale);
            }

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                    effect.Alpha = alpha;
                }
            }
            return absoluteTransforms;
        }

        protected Matrix[] SetupEffectDefaults(EffectSetupDelegate effectdel)
        {            

            Matrix[] absoluteTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            for (int i = 0; i < Model.Bones.Count; i++)
            {
                absoluteTransforms[i] = absoluteTransforms[i] * Matrix.CreateScale(Scale);
            }

            foreach (ModelMesh mesh in Model.Meshes)
            {
                for (int i = 0; i < mesh.Effects.Count; i++ )
                {
                    BasicEffect e = (BasicEffect)mesh.Effects[i];
                    effectdel(ref e);
                }
            }
            return absoluteTransforms;
        }



        public virtual void Update(GameTime gameTime) { }
        public virtual void HandleInput(InputState input) { }
        public virtual void Draw(GameTime gameTime) { }



        //add functions here that will need to be implemented by any object
        // (multi or not)
        


    }
}
