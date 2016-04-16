using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace hack_a_peter {
    /*
        Hallo Erik. :)
                                                                                    Tim was here ...

        Das Format einer InitFile ist : 
            assetname "filepath"
            assetname "filepath"
            ...
        wobei assetname immer das ist, womit du vom TextureManager am Ende die Texture bekommst
        In jedem Verweis auf ein Asset muss im Pfad nicht "Assets/*" stehen, sondern nur der Dateiname und möglicherweise vorher ein Unterverzeichnis.

        Die Implementationen von EndData (EndData/EndData.cs) sind dazu da, um den Scenen, die nach der vorherigen kamen Informationen zum anzeigen zu geben.
        Bsp.: Der Score auf dem EndScreen über die GameEndData.cs.
        Die EndData wird in der Begin()-Methode jeder Scene ausgewertet.

        VIEL SPAß, GL HAVE FUN.
         */
    class Game : Microsoft.Xna.Framework.Game {
        public const bool USE_VSYNC = true;
        public const bool USE_ANTIALISING = false;
        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 640;
        public const string WINDOW_TITLE = @"¯\_(ツ)_/¯          Hack-A-Peter the Game          (╯°□°）╯︵ ┻━┻";

        // visual variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SceneList sceneList;

        public Game () {
            // init graphics
            graphics = new GraphicsDeviceManager (this);
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferMultiSampling = USE_ANTIALISING;
            graphics.ApplyChanges ();

            // set content directory
            Content.RootDirectory = "Assets";

            // set game-settings
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = USE_VSYNC;
            this.Window.Title = WINDOW_TITLE;
        }

        protected override void LoadContent () {
            AssetsManager.LoadGameFont (this.Content);
            foreach (Scene scene in sceneList) {
                AssetsManager.Load (scene.InitFile, this.Content);
            }
        }

        protected override void Draw (GameTime gameTime) {
            // clear screen
            this.GraphicsDevice.Clear (sceneList.CurrentScene.BackColor);

            spriteBatch.Begin ();

            sceneList.CurrentScene.Draw (spriteBatch);

#if DEBUG
            // draw info
            spriteBatch.DrawString (AssetsManager.GameFont12, "fps : " + (1000f / gameTime.ElapsedGameTime.Milliseconds).ToString ("00.00") + " #hyperSpeed", new Vector2 (1, 1), Color.Black);
#endif
            spriteBatch.End ();
        }

        protected override void Initialize () {
            spriteBatch = new SpriteBatch (this.GraphicsDevice);
            sceneList = new SceneList (new Scenes.MainMenu ());

            base.Initialize ();
        }

        protected override void UnloadContent () {
            AssetsManager.DeleteAll ();
        }

        protected override void Update (GameTime gameTime) {
            sceneList.CurrentScene.Update (gameTime.ElapsedGameTime.Milliseconds);

            base.Update (gameTime);
        }
    }
}
