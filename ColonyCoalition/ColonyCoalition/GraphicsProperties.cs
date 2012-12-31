using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonyCoalition
{
    class GraphicsProperties
    {
        private Matrix pm;
        private Matrix vm;
        private MyContentManager cm;
        private GraphicsDevice gd;
        private SpriteBatch sb;
        private RenderTarget2D rt;

        public Matrix ProjectionMatrix{get{return pm;}}
        public Matrix ViewMatrix{get{return vm;}}
        public MyContentManager ContentManager{get{return cm;}}
        public GraphicsDevice GraphicsDevice{get{return gd;}}
        public SpriteBatch SpriteBatch{get{return sb;}}
        public RenderTarget2D RenderTarget { get { return rt; } }

        public GraphicsProperties(Matrix projectionMatrix,
                                  Matrix viewMatrix,
                                  MyContentManager contentManager,
                                  GraphicsDevice graphicsDevice,
                                  SpriteBatch spriteBatch,
                                  RenderTarget2D renderTarget)
        {
            pm = projectionMatrix;
            vm = viewMatrix;
            cm = contentManager;
            gd = graphicsDevice;
            sb = spriteBatch;
            rt = renderTarget;
        }
    }
}
