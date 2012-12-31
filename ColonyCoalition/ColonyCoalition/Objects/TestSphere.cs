using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonyCoalition.Objects
{
    /// <summary>
    /// Testsphere object.
    /// If autoupdate is True, it is expected that no matrix is
    /// passed to the object each time, but rather
    /// </summary>
    class TestSphere:SingleObject
    {
        Vector3 color;
        public TestSphere(
            GraphicsProperties gp,
            string model_s = GameConstants.content_m_testsphere,
            float scale = 5f) : base(gp,model_s,scale)
        {
            world = new Matrix();
            color = Vector3.One;
        }
        public TestSphere(
            GraphicsProperties gp,
            Color color,
            string model_s = GameConstants.content_m_testsphere,
            float scale = 5f):base()
        {
            world = new Matrix();
            Velocity = Vector3.Zero;
            this.color = color.ToVector3();
            alpha = 0.75f;
            this.gp = gp;
            Model_s = model_s;
            Model = gp.ContentManager.LoadCopy<Model>(model_s);
            Scale = scale;
            Transforms = SetupEffectDefaults(new EffectSetupDelegate(ColorDelegate));
        }

        public override void Update(GameTime gameTime)
        {
            Position += Velocity;
        }
        public void Update(GameTime gameTime, Matrix worldMatrix, float alpha = 1f)
        {
            this.alpha = alpha;
            world = worldMatrix;
        }

        public void ColorDelegate(ref BasicEffect effect)
        {
            effect.EnableDefaultLighting();
            effect.Projection = gp.ProjectionMatrix;
            effect.View = gp.ViewMatrix;
            effect.Alpha = alpha;
            //effect.AmbientLightColor = color;
            effect.DiffuseColor = color;
        }

    }
}
