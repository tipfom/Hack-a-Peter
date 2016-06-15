using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Application = System.Windows.Forms.Application;

namespace hack_a_peter.Scenes
{
    class MainMenu : Scene
    {
        public const string NAME = "menu_main";

        private const int IMAGE_WIDTH = 200;
        private const int IMAGE_HEIGHT = 150;

        private const float PLAY_BUTTON_REL_X = 57f / IMAGE_WIDTH;
        private const float PLAY_BUTTON_REL_Y = 56f / IMAGE_HEIGHT;
        private const float PLAY_BUTTON_REL_WIDTH = 85f / IMAGE_WIDTH;
        private const float PLAY_BUTTON_REL_HEIGHT = 13f / IMAGE_HEIGHT;

        private const float EXIT_BUTTON_REL_X = 57f / IMAGE_WIDTH;
        private const float EXIT_BUTTON_REL_Y = 86f / IMAGE_HEIGHT;
        private const float EXIT_BUTTON_REL_WIDTH = 85f / IMAGE_WIDTH;
        private const float EXIT_BUTTON_REL_HEIGHT = 13f / IMAGE_HEIGHT;

        public override string Name { get { return NAME; } }
        public override Color BackColor { get { return Game.GB1; } }
        public override string InitFile { get { return "mainmenu.init"; } }
        public override bool IsMouseVisible { get { return true; } }

        private Texture2D texture;
        private Rectangle playButton;
        private Rectangle exitButton;
        private Random random;

        public MainMenu(int seed) : base()
        {
            random = new Random(seed);
        }

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            texture = random.Next(0, 100) == 0 ? Assets.Textures.Get("main_menu_1") : Assets.Textures.Get("main_menu");
            playButton = new Rectangle(
                (int)(PLAY_BUTTON_REL_X * Game.WINDOW_WIDTH),
                (int)(PLAY_BUTTON_REL_Y * Game.WINDOW_HEIGHT),
                (int)(PLAY_BUTTON_REL_WIDTH * Game.WINDOW_WIDTH),
                (int)(PLAY_BUTTON_REL_HEIGHT * Game.WINDOW_HEIGHT)
                );
            exitButton = new Rectangle(
                (int)(EXIT_BUTTON_REL_X * Game.WINDOW_WIDTH),
                (int)(EXIT_BUTTON_REL_Y * Game.WINDOW_HEIGHT),
                (int)(EXIT_BUTTON_REL_WIDTH * Game.WINDOW_WIDTH),
                (int)(EXIT_BUTTON_REL_HEIGHT * Game.WINDOW_HEIGHT)
                );
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT), Color.White);
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Point position = new Point(mouse.Position.X - Game.GameViewport.X, mouse.Position.Y - Game.GameViewport.Y);
                Rectangle clickRect = new Rectangle(position, new Point(0, 0));
                if (clickRect.Intersects(exitButton))
                {
                    Application.Exit();
                }
                else if (clickRect.Intersects(playButton))
                {
                    OnFinished(SpaceShooterScene.NAME, new EndData.EndData(NAME));
                }
            }
        }
    }
}
