using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hack_a_peter {
    public abstract class Scene {
        // this part needs to be overwritten with scene-specific values (e.g. Scenes/MainMenu.cs)
        public virtual string InitFile { get { return null; } }
        public virtual Color BackColor { get { return new Color (0, 0, 170); } } // Windows XP BSOD-Color
        public virtual bool IsMouseVisible { get { return false; } } // can change while the game is running
        public virtual string Name { get { return null; } }

        public int Score;

        public delegate void OnSceneFinished (string nextScene, EndData.EndData endData);
        public event OnSceneFinished Finished;

        public virtual void Begin (EndData.EndData lastSceneEndData) {

        }

        public virtual void Draw (SpriteBatch spriteBatch) {

        }

        // deltatime in milliseconds
        public virtual void Update (int dt, KeyboardState keyboard, MouseState mouse ) {

        }

        protected void OnFinished (string nextScene, EndData.EndData endData) {
            this.Finished (nextScene, endData);
        }
    }
}
