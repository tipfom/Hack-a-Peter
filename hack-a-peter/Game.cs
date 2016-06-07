using hack_a_peter.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MainMenu = hack_a_peter.Scenes.MainMenu;

namespace hack_a_peter {
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
    class Game : Microsoft.Xna.Framework.Game {
        public const bool USE_VSYNC = true;
        public const bool USE_ANTIALISING = false;
        public const bool USE_FULLSCREEN = false;
        public const int WINDOW_WIDTH = 1024;
        public const int WINDOW_HEIGHT = 768;
        public const string WINDOW_TITLE = @"¯\_(ツ)_/¯          Hack-A-Peter the Game          (╯°□°）╯︵ ┻━┻";
        public const int CURRENT_SEED = 1337;

        public static Color GB1 { get { return new Color(156, 189, 15); } }
        public static Color GB2 { get { return new Color(140, 173, 15); } }
        public static Color GB3 { get { return new Color(48, 98, 48); } }
        public static Color GB4 { get { return new Color(15, 56, 15); } }

        // visual variables
        private GraphicsDeviceManager globalGraphics;
        private SpriteBatch globalSpriteBatch;

        private SceneList sceneList;
        private int timePlayedTotal;
        private bool currentlyShowsIntro;
        private Random random;

        private Viewport gameViewport;
        private Viewport globalViewport;

        public Game( ) {
            // init graphics
            globalGraphics = new GraphicsDeviceManager(this);
            globalGraphics.IsFullScreen = USE_FULLSCREEN;
            globalGraphics.PreferMultiSampling = USE_ANTIALISING;
            globalGraphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            globalGraphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            globalGraphics.ApplyChanges( );

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            // set content directory
            Content.RootDirectory = "Assets";

            // set game-settings
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = USE_VSYNC;
            this.Window.Title = WINDOW_TITLE;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e) {
            UpdateGraphics( );
        }

        protected override void LoadContent( ) {
            Assets.Load("main.init", this.Content);
            foreach (Scene scene in sceneList) {
                Assets.Load(scene.InitFile, this.Content);
            }
        }

        protected override void Draw(GameTime gameTime) {
            // clear screen
            this.GraphicsDevice.Clear(sceneList.CurrentScene.BackColor);

            globalSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            GraphicsDevice.Viewport = globalViewport;
            // globaldrawing here

            GraphicsDevice.Viewport = gameViewport;
            if (currentlyShowsIntro)
                sceneList.CurrentScene.DrawIntro(globalSpriteBatch);
            else
                sceneList.CurrentScene.Draw(globalSpriteBatch);

#if DEBUG
            // draw info
            globalSpriteBatch.DrawString(Assets.Fonts.Get("12px"), "fps : " + (1000f / gameTime.ElapsedGameTime.Milliseconds).ToString("00.00") + " #hyperSpeed", new Vector2(1, 1), Color.Black);
#endif
            globalSpriteBatch.End( );
        }

        protected override void Initialize( ) {
            random = new Random(CURRENT_SEED);
            globalSpriteBatch = new SpriteBatch(this.GraphicsDevice);

            sceneList = new SceneList(OnFinished,
                new MainMenu( ),
                new ScreenOfDeath( ),
                new SpaceShooterScene(random.Next(int.MinValue, int.MaxValue)),
                new MinesweeperScene(random.Next(int.MinValue, int.MaxValue)),
                new LabyrinthScene(random.Next(int.MinValue, int.MaxValue), globalSpriteBatch.GraphicsDevice),
                new StrategyScene( ));

            UpdateViewports( );
            base.Initialize( );
        }

        private void UpdateGraphics( ) {
            if (globalGraphics.PreferredBackBufferWidth == Window.ClientBounds.Width &&
                globalGraphics.PreferredBackBufferHeight == Window.ClientBounds.Height)
                return;

            globalGraphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            globalGraphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            globalGraphics.ApplyChanges( );

            UpdateViewports( );
        }

        private void UpdateViewports( ) {
            globalViewport = new Viewport(0, 0, globalGraphics.PreferredBackBufferHeight, globalGraphics.PreferredBackBufferWidth);
            Point gamePosition = new Point(
                (globalGraphics.PreferredBackBufferWidth - WINDOW_WIDTH) / 2,
                (globalGraphics.PreferredBackBufferHeight - WINDOW_HEIGHT) / 2
                );
            gameViewport = new Viewport(gamePosition.X, gamePosition.Y, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT);
        }

        protected override void UnloadContent( ) {
            Content.Unload( );
        }

        protected override void Update(GameTime gameTime) {
            timePlayedTotal += gameTime.ElapsedGameTime.Milliseconds;

            if (Keyboard.GetState( ).IsKeyDown(Keys.Space))
                currentlyShowsIntro = false;
            if (!currentlyShowsIntro)
                sceneList.CurrentScene.Update(gameTime.ElapsedGameTime.Milliseconds, Keyboard.GetState( ), Mouse.GetState( ));

            this.IsMouseVisible = sceneList.CurrentScene.IsMouseVisible;
            base.Update(gameTime);
        }

        private void OnFinished(string nextScene, EndData.EndData endData) {
            if (endData.LastScene == MainMenu.NAME)
                timePlayedTotal = 0;
            endData.TimePlayed = timePlayedTotal;

            if (sceneList.SetScene(nextScene)) {
                Console.WriteLine("set scene to " + nextScene);
                sceneList.CurrentScene.Begin(endData);
            } else {
                Console.WriteLine("failed to set scene " + nextScene + ", returning to main menu");
                sceneList.SetScene(MainMenu.NAME);
            }
            sceneList.CurrentScene.Begin(endData);

            currentlyShowsIntro = sceneList.CurrentScene.HasIntro;
        }
    }
}
