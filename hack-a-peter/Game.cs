using hack_a_peter.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        public const bool USE_FULLSCREEN =
#if DEBUG
            false;
#else
            true;
#endif
        public const int WINDOW_WIDTH = 1024;
        public const int WINDOW_HEIGHT = 768;
        public const string WINDOW_TITLE = @"¯\_(ツ)_/¯          Hack-A-Peter the Game          (╯°□°）╯︵ ┻━┻";

        private const int BACKGROUND_OFFSET_X = 646;
        private const int BACKGROUND_OFFSET_Y = 268;
        private const int BACKGROUND_SIZE_X = 2316;
        private const int BACKGROUND_SIZE_Y = 1305;

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
        private Texture2D background;
        private VideoPlayer videoPlayer;
        private bool introShown = false;

        public static Viewport GameViewport { get; private set; }
        public static Viewport GlobalViewport { get; private set; }

        public Game ( ) {
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

        private void Window_ClientSizeChanged (object sender, EventArgs e) {
            UpdateGraphics( );
        }

        protected override void LoadContent ( ) {
            Assets.Load("main.init", this.Content);
            foreach (Scene scene in sceneList) {
                Assets.Load(scene.InitFile, this.Content);
            }
            sceneList.CurrentScene.Begin(new EndData.EndData(null));

            background = Assets.Textures.Get("main_background");
        }

        protected override void Draw (GameTime gameTime) {

            // clear screen
            this.GraphicsDevice.Clear(sceneList.CurrentScene.BackColor);

            globalSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            if (videoPlayer.State == MediaState.Playing) {
                globalSpriteBatch.Draw(videoPlayer.GetTexture( ), GlobalViewport.Bounds, Color.White);
                globalSpriteBatch.End( );
                base.Draw(gameTime);
                return;
            }

            GraphicsDevice.Viewport = GlobalViewport;
            // globaldrawing here
            Rectangle backgroundRectangle = new Rectangle(GameViewport.X - BACKGROUND_OFFSET_X, GameViewport.Y - BACKGROUND_OFFSET_Y, BACKGROUND_SIZE_X, BACKGROUND_SIZE_Y);
            globalSpriteBatch.Draw(background, backgroundRectangle, Color.White);
            globalSpriteBatch.End( );

            globalSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            GraphicsDevice.Viewport = GameViewport;
            if (currentlyShowsIntro)
                sceneList.CurrentScene.DrawIntro(globalSpriteBatch);
            else
                sceneList.CurrentScene.Draw(globalSpriteBatch);

#if DEBUG
            // draw info
            globalSpriteBatch.DrawString(Assets.Fonts.Get("12px"), "fps : " + (1000f / gameTime.ElapsedGameTime.Milliseconds).ToString("00.00") + " #hyperSpeed", new Vector2(1, 1), Color.Black);
#endif
            globalSpriteBatch.End( );
            base.Draw(gameTime);
        }

        protected override void Initialize ( ) {
            GenerateRandom( );
            globalSpriteBatch = new SpriteBatch(this.GraphicsDevice);
            videoPlayer = new VideoPlayer( );

            sceneList = new SceneList(OnFinished,
                new MainMenu(random.Next(int.MinValue, int.MaxValue)),
                new ScreenOfDeath( ),
                new SpaceShooterScene(random.Next(int.MinValue, int.MaxValue)),
                new MinesweeperScene(random.Next(int.MinValue, int.MaxValue)),
                new LabyrinthScene(random.Next(int.MinValue, int.MaxValue), globalSpriteBatch.GraphicsDevice),
                new StrategyScene( ),
                new SiderunnerScene(random.Next(int.MinValue, int.MaxValue)),
                new WinScene( ));

            UpdateViewports( );
            base.Initialize( );
        }

        private void UpdateGraphics ( ) {
            if (globalGraphics.PreferredBackBufferWidth == Window.ClientBounds.Width &&
                globalGraphics.PreferredBackBufferHeight == Window.ClientBounds.Height)
                return;

            globalGraphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            globalGraphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            globalGraphics.ApplyChanges( );

            UpdateViewports( );
        }

        private void UpdateViewports ( ) {
            Viewport defaultViewport = GraphicsDevice.Viewport;
            GlobalViewport = new Viewport(0, 0, globalGraphics.PreferredBackBufferWidth, globalGraphics.PreferredBackBufferHeight);
            Point gamePosition = new Point(
                (globalGraphics.PreferredBackBufferWidth - WINDOW_WIDTH) / 2,
                (globalGraphics.PreferredBackBufferHeight - WINDOW_HEIGHT) / 2
                );
            GameViewport = new Viewport(gamePosition.X, gamePosition.Y, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT);
        }

        protected override void UnloadContent ( ) {
            Content.Unload( );
        }

        protected override void Update (GameTime gameTime) {
            if (!introShown) {
                videoPlayer.Play(Assets.Videos.Get("intro"));
                introShown = true;
            } else if (videoPlayer.State == MediaState.Playing) {
                if (videoPlayer.PlayPosition > Assets.Videos.Get("intro").Duration)
                    videoPlayer.Stop( );
                base.Update(gameTime);
                return;
            }
            timePlayedTotal += gameTime.ElapsedGameTime.Milliseconds;

            if (!GameViewport.Bounds.Intersects(new Rectangle(Mouse.GetState( ).Position, new Point(0, 0)))) {
                Point mousePosition = Mouse.GetState( ).Position;
                mousePosition.X = Math.Min(GameViewport.Bounds.Right, Math.Max(GameViewport.Bounds.Left, mousePosition.X));
                mousePosition.Y = Math.Min(GameViewport.Bounds.Bottom, Math.Max(GameViewport.Bounds.Top, mousePosition.Y));
                Mouse.SetPosition(mousePosition.X, mousePosition.Y);
                Console.WriteLine(Mouse.GetState( ).Position);
            }

            if (Keyboard.GetState( ).IsKeyDown(Keys.Space))
                currentlyShowsIntro = false;
            if (Keyboard.GetState( ).IsKeyDown(Keys.Escape))
                Application.Exit( );
            if (!currentlyShowsIntro)
                sceneList.CurrentScene.Update(gameTime.ElapsedGameTime.Milliseconds, Keyboard.GetState( ), Mouse.GetState( ));

            this.IsMouseVisible = sceneList.CurrentScene.IsMouseVisible;
            base.Update(gameTime);
        }

        private void GenerateRandom ( ) {
            random = new Random( );
            int seed = random.Next(int.MinValue, int.MaxValue);
            Console.WriteLine(seed);
            random = new Random(seed);
        }

        private void OnFinished (string nextScene, EndData.EndData endData) {
            if ((endData?.LastScene ?? "") == MainMenu.NAME)
                timePlayedTotal = 0;
            else if ((endData?.LastScene ?? "") == ScreenOfDeath.NAME)
                GenerateRandom( );

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
