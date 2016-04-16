using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            spriteBatch.Draw (AssetsManager.Get ("illuminati"), new Rectangle (0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT), Color.White);
            spriteBatch.DrawString (AssetsManager.GameFont72, "HACK-A-PETER",
                new Vector2 ((Game.WINDOW_WIDTH - AssetsManager.GameFont72.MeasureString ("HACK-A-PETER").X) / 2, 10),
                Color.Orange);
            spriteBatch.DrawString (AssetsManager.GameFont48, "MAIN MENU (BETA)",
                new Vector2 (Game.WINDOW_WIDTH - AssetsManager.GameFont48.MeasureString ("MAIN MENU (BETA)").X,
                Game.WINDOW_HEIGHT - AssetsManager.GameFont48.MeasureString ("MAIN MENU (BETA)").Y) / 2,
                Color.Blue);
            spriteBatch.Draw (AssetsManager.Get ("mlgbrille"), new Rectangle (Game.WINDOW_WIDTH *3/4, Game.WINDOW_HEIGHT *2/3, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT), null, Color.White,
                rotation, new Vector2 (AssetsManager.Get ("mlgbrille").Width / 2, AssetsManager.Get ("mlgbrille").Height / 2),
                SpriteEffects.FlipHorizontally, 1);
        }

        public override void Update (int dt) {
            rotation += dt;
        }
    }
}
