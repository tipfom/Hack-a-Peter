using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace hack_a_peter.Scenes
{
    public class WinScene : Scene
    {
        public const string NAME = "win";

        public override string InitFile
        {
            get
            {
                return "win.init";
            }
        }
        public override string Name
        {
            get
            {
                return WinScene.NAME;
            }
        }
        public override Color BackColor
        {
            get
            {
                return Game.GB2;
            }
        }
        public override bool IsMouseVisible
        {
            get
            {
                return false;
            }
        }
        public override string Intro
        {
            get
            {
                return "You sccusessfully hacked Peter's Computer.\nHis mum is angry now.";
            }
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            OnFinished(MainMenu.NAME, new EndData.EndData(WinScene.NAME));
        }
    }
}
