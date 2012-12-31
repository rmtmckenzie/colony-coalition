using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition.Objects
{
    class Bullets : MultiObject
    {
        public Bullets(GraphicsProperties gp,
                       int max, 
                       string model = GameConstants.content_m_bullet, 
                       float scale = 1) :
                base(gp, max, model, scale) { }

        //DON'T FORGET TO SET PREPROCESSING FOR MODELS!
        public override void Update(GameTime gameTime) { int i = Max;  }
        public override void HandleInput(InputState input) { int j = Max; }
        public override void Draw(GameTime gameTime) { int l = Max; }
        public override void New(int i)
        {
            throw new NotImplementedException();
        }
    }
}
