using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hack_a_peter.EndData;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.IO;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace hack_a_peter.Scenes
{
    public class SiderunnerScene : Scene,ContactListener
    {
        public const string NAME = "game_siderunner";


        public override string InitFile { get { return "siderunner.init"; } }
        public override string Name { get { return NAME; } }
        public override string Intro { get { return "HALLO WELT!"; } }
        public override bool IsMouseVisible { get { return false; } }

        private Texture2D playerTexture;
        private Texture2D platformTexture;
        private bool[,] level;
        private int tileSize;
        private int drawLength;
        private Vector2 playerPosition;
        private World world;
        private Body playerBody;

        public SiderunnerScene(int seed)
        {

        }

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            playerTexture = Assets.Textures.Get("siderunner_player");
            platformTexture = Assets.Textures.Get("siderunner_platform");

            // load level
            string[] lines = File.ReadAllLines("Assets/siderunner_level.lvl");
            level = new bool[lines[0].Length, lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                char[] content = lines[i].ToCharArray();
                for (int c = 0; c < content.Length; c++)
                {
                    level[c, i] = content[c] == ' ' ? false : true;
                }
            }

            tileSize = Game.WINDOW_HEIGHT / lines.Length;
            drawLength = Game.WINDOW_WIDTH / tileSize + 1;

            // create world
            world = new World(new AABB() { LowerBound = new Vec2(0, 0), UpperBound = new Vec2(level.GetLength(0), level.GetLength(1)) }, new Vec2(1, 3), true);
            world.SetContinuousPhysics(true);
            world.SetContactListener(this);

            // create player
            BodyDef playerDef = new BodyDef();
            playerDef.Position = new Vec2(drawLength / 2f, level.GetLength(1) / 2f);
            playerDef.UserData = true;
            playerDef.FixedRotation = true;
            playerBody = world.CreateBody(playerDef);
            PolygonDef playerFix = new PolygonDef();
            playerFix.SetAsBox(0.25f, 0.5f);
            playerBody.CreateFixture(playerFix);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < drawLength; x++)
            {
                for (int y = 0; y < level.GetLength(1); y++)
                {
                    if (level[x, y])
                    {
                        Rectangle drawRectangle = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                        spriteBatch.Draw(platformTexture, drawRectangle, Microsoft.Xna.Framework.Color.White);
                    }
                }
            }
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            //Vector2 playerTargetPosition = playerPosition + currentVelocity * dt / 1000f;
            //if (currentVelocity.Y < 0)
            //{
            //    for (int y = (int)(playerPosition.Y - playerSize.; y > (int)playerTargetPosition.Y; y--)
            //    {
            //        if (level[(int)playerPosition.X, y])
            //        {

            //        }
            //    }
            //}
            //else
            //{
            //    for (int y = (int)playerPosition.Y; y < (int)playerTargetPosition.Y; y++)
            //    {

            //    }
            //}
        }

        void ContactListener.BeginContact(Contact contact)
        {
        }

        void ContactListener.EndContact(Contact contact)
        {
        }

        void ContactListener.PreSolve(Contact contact, Manifold oldManifold)
        {
        }

        void ContactListener.PostSolve(Contact contact, ContactImpulse impulse)
        {
        }
    }
}
