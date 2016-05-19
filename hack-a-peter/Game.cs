using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hack_a_peter
{
    /*
        Hallo Erik. :)
                                                                                    Tim was here ...

        Das Format einer InitFile ist : 
            format assetname "filepath"
            format assetname "filepath"
            ...
        wobei   assetname immer das ist, womit du vom TextureManager am Ende die Texture bekommst und 
                format jeweils für den Datentyp steht {
                    A = SoundEffect aka Audio
                    S = Song
                    T = Texture
                    V = Video
                    F = Font    
                }
        In jedem Verweis auf ein Asset muss im Pfad nicht "Assets/*" stehen, sondern nur der Dateiname und möglicherweise vorher ein Unterverzeichnis.

        Die Implementationen von EndData (EndData/EndData.cs) sind dazu da, um den Scenen, die nach der vorherigen kamen Informationen zum anzeigen zu geben.
        Bsp.: Der Score auf dem EndScreen über die GameEndData.cs.
        Die EndData wird in der Begin()-Methode jeder Scene ausgewertet.

        VIEL SPAß, GL HAVE FUN.
         */
    class Game : Microsoft.Xna.Framework.Game
    {
        public const bool USE_VSYNC = true;
        public const bool USE_ANTIALISING = false;
        public const int WINDOW_WIDTH = 1024;
        public const int WINDOW_HEIGHT = 768;
        public const string WINDOW_TITLE = @"¯\_(ツ)_/¯          Hack-A-Peter the Game          (╯°□°）╯︵ ┻━┻";
        public const int CURRENT_SEED = 1337;

        // visual variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        private SceneList sceneList;
        private int timePlayedTotal;
        private Random random;

        public Game()
        {
            // init graphics
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferMultiSampling = USE_ANTIALISING;
            graphics.ApplyChanges();

            // set content directory
            Content.RootDirectory = "Assets";

            // set game-settings
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = USE_VSYNC;
            this.Window.Title = WINDOW_TITLE;
        }

        protected override void LoadContent()
        {
            Assets.Load("main.init", this.Content);
            foreach (Scene scene in sceneList)
            {
                Assets.Load(scene.InitFile, this.Content);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // clear screen
            this.GraphicsDevice.Clear(sceneList.CurrentScene.BackColor);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            sceneList.CurrentScene.Draw(spriteBatch);

#if DEBUG
            // draw info
            spriteBatch.DrawString(Assets.Fonts.Get("12px"), "fps : " + (1000f / gameTime.ElapsedGameTime.Milliseconds).ToString("00.00") + " #hyperSpeed", new Vector2(1, 1), Color.Black);
#endif
            spriteBatch.End();
        }

        protected override void Initialize()
        {
            random = new Random(CURRENT_SEED);
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            sceneList = new SceneList(OnFinished,
                new Scenes.MainMenu(),
                new Scenes.ScreenOfDeath(),
                new Scenes.SpaceShooterScene(random.Next(int.MinValue,int.MaxValue)));

            base.Initialize();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            timePlayedTotal += gameTime.ElapsedGameTime.Milliseconds;

            sceneList.CurrentScene.Update(gameTime.ElapsedGameTime.Milliseconds, Keyboard.GetState(), Mouse.GetState());

            this.IsMouseVisible = sceneList.CurrentScene.IsMouseVisible;
            base.Update(gameTime);
        }

        private void OnFinished(string nextScene, EndData.EndData endData)
        {
            if (endData.LastScene == SceneList.MAIN_MENU_NAME)
                timePlayedTotal = 0;
            endData.TimePlayed = timePlayedTotal;

            if (sceneList.SetScene(nextScene))
            {
                Console.WriteLine("set scene to " + nextScene);
                sceneList.CurrentScene.Begin(endData);
            }
            else
            {
                Console.WriteLine("failed to set scene " + nextScene + ", returning to main menu");
                sceneList.SetScene(SceneList.MAIN_MENU_NAME);
            }
            sceneList.CurrentScene.Begin(endData);
        }
    }
}
