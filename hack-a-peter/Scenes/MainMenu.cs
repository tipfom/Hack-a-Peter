using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hack_a_peter.Scenes {
    class MainMenu : Scene {
        public override string Name { get { return SceneList.MAIN_MENU_NAME; } }
        public override Color BackColor { get { return Color.Beige; } }
        public override string InitFile { get { return "mainmenu.init"; } }
        public override bool IsMouseVisible { get { return true; } }

        private int rotation = 420;

        public MainMenu () : base () {

        }

        public override void Begin (EndData.EndData lastSceneEndData) {
            base.Begin (lastSceneEndData);
        }

        public override void Draw (SpriteBatch spriteBatch) {
            spriteBatch.Draw (Assets.Textures.Get ("illuminati"), new Rectangle (0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT), Color.White);
            spriteBatch.DrawString (Assets.Fonts.Get ("72px"), "HACK-A-PETER",
                new Vector2 ((Game.WINDOW_WIDTH - Assets.Fonts.Get ("72px").MeasureString ("HACK-A-PETER").X) / 2, 10),
                Color.Orange);
            spriteBatch.DrawString (Assets.Fonts.Get ("48px"), "MAIN MENU (BETA)",
                new Vector2 (Game.WINDOW_WIDTH - Assets.Fonts.Get ("48px").MeasureString ("MAIN MENU (BETA)").X,
                Game.WINDOW_HEIGHT - Assets.Fonts.Get ("48px").MeasureString ("MAIN MENU (BETA)").Y) / 2,
                Color.Blue);
            spriteBatch.Draw (Assets.Textures.Get ("mlgbrille"), new Rectangle (Game.WINDOW_WIDTH * 3 / 4, Game.WINDOW_HEIGHT * 2 / 3, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT), null, Color.White,
                rotation, new Vector2 (Assets.Textures.Get ("mlgbrille").Width / 2, Assets.Textures.Get ("mlgbrille").Height / 2),
                SpriteEffects.FlipHorizontally, 1);
        }

        public override void Update (int dt, KeyboardState keyboard, MouseState mouse) {
            rotation += dt;

            OnFinished ("scene_labyrinth", new EndData.GameEndData (1337, this.Name));
        }
    }
}
