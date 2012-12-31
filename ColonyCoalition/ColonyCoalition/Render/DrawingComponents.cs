using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ColonyCoalition
{

    class DrawingComponents
    {
        Effect renderEffects;

        Vector3 P1shieldDirection;
        Vector3 P2shieldDirection;
        float shieldWeight;
        Model shieldModel;
        private RenderTarget2D sceneRenderTarget;
        GraphicsProperties gp;
        SpriteBatch spriteBatch;
        float shieldmax = 2f;

        Matrix shipWorld;

        //pointers to game1 vars
        Matrix viewMatrix, projectionMatrix;
        GraphicsDevice device;

        LightBloom lightBloom;

        MyContentManager content;


        public DrawingComponents(GraphicsProperties gp)
        {
            this.viewMatrix = gp.ViewMatrix;
            this.projectionMatrix = gp.ViewMatrix;
            this.device = gp.GraphicsDevice;
            this.content = gp.ContentManager;
            this.spriteBatch = gp.SpriteBatch;

            //** set up scene Render taget, main target for all object drawing **/
            PresentationParameters pp = device.PresentationParameters;

            int height = pp.BackBufferHeight;
            int width = pp.BackBufferWidth;
            SurfaceFormat format = pp.BackBufferFormat;

            sceneRenderTarget = gp.RenderTarget;


            renderEffects = content.Load<Effect>("RenderAssets/RenderEffects");
            shieldModel = LoadModel("RenderAssets/cruiserShield", content);

            lightBloom = new LightBloom();
            lightBloom.LoadContent(spriteBatch, content, device, sceneRenderTarget);
        }


        private Model LoadModel(string assetName, MyContentManager Content)
        {

            Model newModel = Content.Load<Model>(assetName);

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = renderEffects.Clone();
                }
            return newModel;
        }

        public void Update(Vector3 p1shield, Vector3 p2shield, Matrix shipWorld)
        {
            P1shieldDirection = p1shield;
            P2shieldDirection = p2shield;
            this.shipWorld = shipWorld;
        }

        public void Draw(GameTime gametime)
        {
            lightBloom.DrawLightBloomBegin(device);
            DrawShield(shipWorld);
            lightBloom.DrawLightBloomEnd(device);

        }

        public void DrawShield(Matrix world)
        {
            Vector3 scale, translation;
            Quaternion rotation;

            //should actually be up... 
            Vector3 side = world.Right;
            Vector3 forward = world.Up;

            world.Decompose(out scale, out rotation, out translation);

            scale.Z *= .8f;
            scale *= 1.5f;
            // scale.X *= 3f;
            //translation -= world.Right;

            rotation = Quaternion.Concatenate(rotation, Quaternion.CreateFromAxisAngle(world.Right, MathHelper.ToRadians(90)));


            Matrix.CreateFromQuaternion(ref rotation, out world);
            world = Matrix.CreateScale(scale) * world;
            world *= Matrix.CreateTranslation(translation);


            // world = Matrix.CreateScale(scale*2) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateRotationZ((float)(Math.PI / 2)) * Matrix.CreateTranslation(translation);


            Matrix[] ModelTransforms = new Matrix[shieldModel.Bones.Count];
            shieldModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);


            foreach (ModelMesh mesh in shieldModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                    currentEffect.Parameters["xWorld"].SetValue(ModelTransforms[mesh.ParentBone.Index] * world);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);


                    Vector3 lightDirection = new Vector3(3, -1, 0);
                    lightDirection.Normalize();

                    currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                    currentEffect.Parameters["xShieldP1"].SetValue(P1shieldDirection);
                    currentEffect.Parameters["xShieldP2"].SetValue(P2shieldDirection);
                    currentEffect.Parameters["xShipSide"].SetValue(side);
                    currentEffect.Parameters["xShipForward"].SetValue(forward);
                    //   currentEffect.Parameters["xTexture"].SetValue(texture);

                }
                mesh.Draw();
            }
        }













    }
}
