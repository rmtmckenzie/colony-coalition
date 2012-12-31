using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace ColonyCoalition
{
    class LightBloom
    {
        Effect gaussianBlurEffect;
        Effect bloomCombineEffect;

        RenderTarget2D sceneRenderTarget;
        RenderTarget2D renderTarget1;
        RenderTarget2D renderTarget2;

        Texture2D previous;

        float bloomValue, bloomSat;
        float baseValue, baseSat;

        SpriteBatch spriteBatch;

        public LightBloom()
        {
            bloomValue = 1;
            bloomSat = 2f;
            baseValue = 1;
            baseSat = 1;

        }

        public void LoadContent(SpriteBatch spriteBatch, ContentManager Content, GraphicsDevice device, RenderTarget2D sceneRenderTarget)
        {
            this.spriteBatch = spriteBatch;

            gaussianBlurEffect = Content.Load<Effect>("RenderAssets/GaussianBlur");
            bloomCombineEffect = Content.Load<Effect>("RenderAssets/BloomCombine");

            previous = new Texture2D(device, 1337, 1337);

            PresentationParameters pp = device.PresentationParameters;
            // renderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, true, device.DisplayMode.Format, DepthFormat.Depth24);

            int height = pp.BackBufferHeight;
            int width = pp.BackBufferWidth;
            SurfaceFormat format = pp.BackBufferFormat;

            this.sceneRenderTarget = sceneRenderTarget;

            height /= 2;
            width /= 2;
            renderTarget1 = new RenderTarget2D(device, width, height, false, format, pp.DepthStencilFormat);
            renderTarget2 = new RenderTarget2D(device, width, height, false, format, DepthFormat.None);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);


            spriteBatch.Draw(previous, new Rectangle(0, 0, width, height), new Color(240, 240, 240));
            spriteBatch.End();

        }

        void SetBlurEffectParameters(float dx, float dy)
        {


            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            // int sampleCount = weightsParameter.Elements.Count;

            int sampleCount = 3;

            //bloomsize = sampleCount;
            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        public void DrawLightBloomBegin(GraphicsDevice device)
        {
            device.SamplerStates[1] = SamplerState.LinearClamp;


            //Draws Lights to renderTarget1
            device.SetRenderTarget(renderTarget1);
            device.Clear(Color.Black);
            
            device.BlendState = BlendState.Additive;
            
            device.DepthStencilState = DepthStencilState.DepthRead;
            device.BlendState = BlendState.Additive;

        }

        public void DrawLightBloomEnd(GraphicsDevice device)
        {

            int width = renderTarget1.Width;
            int height = renderTarget1.Height;


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatch.Draw(previous, new Rectangle(0, 0, width, height), new Color(200, 200, 200));
            spriteBatch.End();

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0);

            device.SetRenderTarget(renderTarget2);
            DrawFullscreenQuad(renderTarget1, width, height, gaussianBlurEffect);

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height);

            device.SetRenderTarget(renderTarget1);
            DrawFullscreenQuad(renderTarget2, width, height, gaussianBlurEffect);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            device.SetRenderTarget(null);
            previous = renderTarget2;

            EffectParameterCollection parameters = bloomCombineEffect.Parameters;
            parameters["BloomIntensity"].SetValue(bloomValue);
            parameters["BaseIntensity"].SetValue(baseValue);
            parameters["BloomSaturation"].SetValue(bloomSat);
            parameters["BaseSaturation"].SetValue(baseSat);

            device.Textures[1] = sceneRenderTarget;
            Viewport viewport = device.Viewport;

            DrawFullscreenQuad(renderTarget1, viewport.Width, viewport.Height, bloomCombineEffect);
        }




        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect)
        {
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }
        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            float theta = 4;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }



    }
}
