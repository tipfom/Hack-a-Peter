using hack_a_peter.EndData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Timers;

namespace hack_a_peter.Scenes {
    class ScreenOfDeath : Scene {
        public const string NAME = "menu_death";

        private const string ENDSCREEN_TEXT =
            "A problem has been detected and Windows has been shut down to prevent damage\n" +
            "to your computer.\n" +
            "\n" +
            "The problem seems to be caused by the following file: {0}.exe\n" +
            "\n" +
            "If this is the first time you've seen this stop error sreen,\n" +
            "restart your computer. If this screen appears again, follow\n" +
            "these steps:\n" +
            "\n" +
            "Check to make sure any new hardware or software is properly installed.\n" +
            "If this is a new installation, ask your hardware or software manufacturer\n" +
            "for any Windows updates you might need.\n" +
            "\n" +
            "If problems continue, disable or remove any newly installed hardware\n" +
            "or software. Disable BIOS memory options such as caching or shadowing.\n" +
            "If you need to use safe mode to remove or disable components, restart\n" +
            "your computer, press F8 to select Advanced Startup Options, and then\n" +
            "select Safe Mode.\n" +
            "\n" +
            "Technical Information:\n" +
            "\n" +
            "*** STOP : SCORE = 0x{1:X8}\n" + // line with score in hex 
            "\n" +
            "*** {0}.exe - Adress 0x805376ba base at 0x804d7000 after {2} seconds\n" +
            "\n" +
            "*** PRESS ANY KEY TO REBOOT"; // time played

        public override bool IsMouseVisible { get { return false; } }
        public override Color BackColor { get { return new Color(0, 0, 128); } }
        public override string InitFile { get { return "deathscreen.init"; } }
        public override string Name { get { return NAME; } }

        private int score;
        private string screenText;
        private bool acceptsKeys = false;

        public ScreenOfDeath( ) : base( ) {

        }

        public override void Begin(EndData.EndData lastSceneEndData) {
            if (lastSceneEndData.GetType( ) == typeof(GameEndData)) {
                score = ((GameEndData)lastSceneEndData).FinishedScore;
            } else {
                Console.WriteLine("end screen got called without the lastSceneData of a Game :(");
                score = 0;
            }
            screenText = String.Format(ENDSCREEN_TEXT, lastSceneEndData.LastScene, score, (lastSceneEndData.TimePlayed / 1000f).ToString( ));

            acceptsKeys = false;
            Timer inputEnableTimer = new Timer(4000);
            inputEnableTimer.Elapsed += (sender, e) => { acceptsKeys = true; inputEnableTimer.Stop( ); };
            inputEnableTimer.Start( );
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Assets.Fonts.Get("bluescreenfont"), screenText, new Vector2(0, 10), Color.White);
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse) {
            if (keyboard.GetPressedKeys( ).Length > 0 && acceptsKeys)
                OnFinished(MainMenu.NAME, new EndData.EndData(Name));
        }
    }
}
