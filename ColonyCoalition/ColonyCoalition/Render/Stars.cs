using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonyCoalition
{
    /// <summary>
    /// All your stars.
    /// </summary>
    public class Stars
    {
        //todo - move most of this to gameconstants/gamevariables
        float LevelSpeed = 220;

        private float MAX_POSITION = 1024f;  // How far to travel before looping to the top
        private float MIN_POSITION = -200f;
        private int TOP_LEVEL_STARS = 50;    // How many top level start to make.
        private int MIDDLE_LEVEL_STARS = 25; // How many middle level stars to make.
        private int BOTTOM_LEVEL_STARS = 25; // How many bottom level stars to make.
        private int LEFT_BORDER = 0;     // Furthest left area on the screen.
        private int RIGHT_BORDER = 1280; // Furthest right area on the screen. 
        private float TWINKLE_TIME = .05f;
        private float FADE_SPEED = .15f;
        private float FADE_TRANSPARENCY = .05f;
        private float FADE_TRANSPARENCY_FUN_MODE = .5f;

        private Random random;
        private Vector2 topSpeed;
        private Vector2 middleSpeed;
        private Vector2 bottomSpeed;
        private Texture2D starCover; // 1x1 black pixel
        private int twinkleStar1;
        private int twinkleStar2;
        private int twinkleStar3;
        private int twinkleStar4;
        private int twinkleStar5;
        private float twinkleTimer;
        private Color twinkleColor;
        private Color overlayColor;
        private ColorOverlayType colorOverlayType;
        private float red;
        private float blue;
        private float green;

        Star[] ListOfStars;

        //public List<Star> ListOfStars
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Create our paralax stars.
        /// </summary>
        public Stars(MyContentManager content)
        {

            this.ListOfStars = new Star[TOP_LEVEL_STARS + BOTTOM_LEVEL_STARS + MIDDLE_LEVEL_STARS];
                //new List<Star>();
            random = new Random();
            this.random = new Random();
            this.starCover = content.Load<Texture2D>("Stars\\starCover");

            // Start game as  0, 1, 0
            this.overlayColor = new Color(0, 1f, 0) * FADE_TRANSPARENCY;

            this.twinkleTimer = TWINKLE_TIME;
            this.twinkleColor = new Color(1f, 1f, 1f) * .5f;
            this.colorOverlayType = ColorOverlayType.Red;

            // Add different layers of stars. Add in reverse order so bottom stars are drawn first.
            for (int i = 0; i < BOTTOM_LEVEL_STARS; i++)
            {
                this.ListOfStars[i] = new Star(
                        content,
                        (StarType)random.Next(1, 2), //star type
                        StarLayer.Bottom,   //layer
                        new Vector2(random.Next(LEFT_BORDER, RIGHT_BORDER), random.Next(-200, 850))); //pos
            }

            for (int i = BOTTOM_LEVEL_STARS; i < MIDDLE_LEVEL_STARS+BOTTOM_LEVEL_STARS; i++)
            {
                this.ListOfStars[i] = new Star(
                        content,
                        (StarType)random.Next(3, 4),
                        StarLayer.Middle,
                        new Vector2(random.Next(LEFT_BORDER, RIGHT_BORDER), random.Next(-200, 850)));
            }

            for (int i = BOTTOM_LEVEL_STARS+MIDDLE_LEVEL_STARS; i < BOTTOM_LEVEL_STARS+MIDDLE_LEVEL_STARS+TOP_LEVEL_STARS; i++)
            {
                this.ListOfStars[i] = new Star(
                        content,
                        (StarType)random.Next(5, 7),
                        StarLayer.Top,
                        new Vector2(random.Next(LEFT_BORDER, RIGHT_BORDER), random.Next(-200, 850)));
            }
        }

        public void Update(GameTime gameTime)
        {
            float ElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.topSpeed = new Vector2(0, (LevelSpeed / 2) * ElapsedTime);
            this.middleSpeed = new Vector2(0, (LevelSpeed / 4) * ElapsedTime);
            this.bottomSpeed = new Vector2(0, (LevelSpeed / 8) * ElapsedTime);
            this.twinkleTimer -= ElapsedTime;

            this.UpdateStarFading(ElapsedTime);

            for (int i = 0; i < this.ListOfStars.Length; i++)
            {
                switch (this.ListOfStars[i].CurrentStarLayer)
                {
                    case StarLayer.Top:
                        {
                            this.ListOfStars[i].Position += this.topSpeed;
                            break;
                        }
                    case StarLayer.Middle:
                        {
                            this.ListOfStars[i].Position += this.middleSpeed;
                            break;
                        }
                    case StarLayer.Bottom:
                        {
                            this.ListOfStars[i].Position += this.bottomSpeed;
                            break;
                        }
                }

                // Move star back to the top of the screen.
                if (this.ListOfStars[i].Position.Y > MAX_POSITION)
                {
                    this.ListOfStars[i].Position = new Vector2(this.ListOfStars[i].Position.X, MIN_POSITION);
                }
            }
        }


        private void UpdateStarFading(float ElapsedTime)
        {
            // Red to blue to green.
            switch (colorOverlayType)
            {
                case ColorOverlayType.Red:
                    // 1, 0, 0
                    this.red -= ElapsedTime * FADE_SPEED;
                    this.blue += ElapsedTime * FADE_SPEED;

                    if (this.blue > 1f)
                    {
                        this.red = 0f;
                        this.blue = 1f;
                        this.colorOverlayType = ColorOverlayType.Blue;
                    }
                    break;
                case ColorOverlayType.Blue:
                    // 0, 0, 1
                    this.blue -= ElapsedTime * FADE_SPEED;
                    this.green += ElapsedTime * FADE_SPEED;

                    if (this.green > 1f)
                    {
                        this.green = 1f;
                        this.blue = 0f;
                        this.colorOverlayType = ColorOverlayType.Green;
                    }
                    break;
                case ColorOverlayType.Green:
                    // 0, 1, 0
                    this.green -= ElapsedTime * FADE_SPEED;
                    this.red += ElapsedTime * FADE_SPEED;

                    if (this.red > 1f)
                    {
                        this.green = 0f;
                        this.red = 1f;
                        this.colorOverlayType = ColorOverlayType.Red;
                    }
                    break;
            }

            this.overlayColor = new Color(this.red, this.green, this.blue) * FADE_TRANSPARENCY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.twinkleTimer < 0)
            {
                this.twinkleTimer = TWINKLE_TIME;
                this.twinkleStar1 = this.random.Next(1, 99);
                this.twinkleStar2 = this.random.Next(1, 99);
                this.twinkleStar3 = this.random.Next(1, 99);
                this.twinkleStar4 = this.random.Next(1, 99);
                this.twinkleStar5 = this.random.Next(1, 99);
            }

            for (int i = 0; i < this.ListOfStars.Length; i++)
            {
                this.ListOfStars[i].Draw(spriteBatch, false);
            }

            // Draw black over stars to make them twinkle.
            spriteBatch.Draw(this.starCover, this.ListOfStars[this.twinkleStar1].DrawRectangle(false), this.twinkleColor);
            spriteBatch.Draw(this.starCover, this.ListOfStars[this.twinkleStar2].DrawRectangle(false), this.twinkleColor);
            spriteBatch.Draw(this.starCover, this.ListOfStars[this.twinkleStar3].DrawRectangle(false), this.twinkleColor);
            spriteBatch.Draw(this.starCover, this.ListOfStars[this.twinkleStar4].DrawRectangle(false), this.twinkleColor);
            spriteBatch.Draw(this.starCover, this.ListOfStars[this.twinkleStar5].DrawRectangle(false), this.twinkleColor);
        }
    }

    public enum ColorOverlayType
    {
        Red,
        Blue,
        Green
    }

    public enum StarLayer
    {
        Top,
        Middle,
        Bottom
    }

    public enum StarType
    {
        Star1 = 1, // Dark
        Star2 = 2, // Dark
        Star3 = 3, // Medium
        Star4 = 4, // Medium
        Star5 = 5, // Light
        Star6 = 6, // Light
        Star7 = 7  // Light
    }

    /// <summary>
    /// A single star.
    /// </summary>
    public struct Star
    {
        private Vector2 direction;
        private Color drawColor;
        private StarType currentStarType;
        private Texture2D texture;
        
        public StarLayer CurrentStarLayer;
        public Vector2 Speed;
        public Vector2 Position;
        public int Width;
        public int Height;

        public Rectangle DrawRectangle(bool maxSpeed)
        {
            return new Rectangle(Convert.ToInt32(this.Position.X), Convert.ToInt32(this.Position.Y), this.Width, this.Height);
        }

        public Star(MyContentManager content, StarType initialStarType, StarLayer initialStarLayer, Vector2 position)
        {
            this.direction = new Vector2(0, 1);
            this.Speed = new Vector2(0, 50);
            this.currentStarType = initialStarType;
            this.drawColor = Color.White;
            this.CurrentStarLayer = initialStarLayer;
            this.Position = position;
            this.texture = null;

            // You must create textures named Star1 to Star7 in the Background folder of your
            // content project. This gives the stars a more varried look.
            switch (initialStarType)
            {
                case StarType.Star1:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star3");
                        break;
                    }
                case StarType.Star2:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star3");
                        break;
                    }
                case StarType.Star3:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star2");
                        break;
                    }
                case StarType.Star4:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star2");
                        break;
                    }
                case StarType.Star5:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star");
                        break;
                    }
                case StarType.Star6:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star");
                        break;
                    }
                case StarType.Star7:
                    {
                        this.texture = content.Load<Texture2D>("Stars\\star");
                        break;
                    }
            }

            this.Width = this.texture.Width;
            this.Height = this.texture.Height;
        }

        public void Draw(SpriteBatch spriteBatch, bool MaxSpeed)
        {
            // This should really be all on one line but I needed to make it two to fit nicely on this page.
            spriteBatch.Draw(this.texture, new Rectangle(Convert.ToInt32(this.Position.X), Convert.ToInt32(this.Position.Y),
                this.Width, this.Height), this.drawColor);
        }
    }
}