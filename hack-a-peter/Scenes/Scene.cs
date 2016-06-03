using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hack_a_peter {
    public abstract class Scene {
        // this part needs to be overwritten with scene-specific values (e.g. Scenes/MainMenu.cs)
        public abstract string InitFile { get; }
        public abstract string Name { get; }
        public virtual string Intro { get { return null; } }
        public virtual Color BackColor { get { return Game.GB4; } } // Windows XP BSOD-Color
        public virtual bool IsMouseVisible { get { return false; } } // can change while the game is running

        public bool HasIntro { get { return Intro != null; } }
        public int Score;

        public delegate void OnSceneFinished(string nextScene, EndData.EndData endData);
        public event OnSceneFinished Finished;

        public virtual void Begin(EndData.EndData lastSceneEndData) {

        }

        public virtual void Draw(SpriteBatch spriteBatch) {

        }

        public void DrawIntro(SpriteBatch spriteBatch) {
            SpriteFont font = Assets.Fonts.Get("12px");
            string[ ] lines = (Intro + "\nPRESS SPACE TO CONTINUE!").Split('\n');
            Vector2[ ] sizes = new Vector2[lines.Length];
            Vector2 overallSize = new Vector2( );

            for (int i = 0; i < lines.Length; i++) {
                sizes[i] = font.MeasureString(lines[i]);
                overallSize.Y += sizes[i].Y;
                if (sizes[i].X > overallSize.X)
                    overallSize.X = sizes[i].X;
            }

            int yStartPoint = (Game.WINDOW_HEIGHT - (int)overallSize.Y) / 2;
            for (int i = 0; i < lines.Length; i++) {
                Vector2 position = new Vector2((Game.WINDOW_WIDTH - sizes[i].X) / 2, yStartPoint);
                spriteBatch.DrawString(font, lines[i], position, Game.GB3);
                yStartPoint += (int)sizes[i].Y;
            }
        }

        // deltatime in milliseconds
        public virtual void Update(int dt, KeyboardState keyboard, MouseState mouse) {

        }

        protected void OnFinished(string nextScene, EndData.EndData endData) {
            this.Finished(nextScene, endData);
        }
    }
}
