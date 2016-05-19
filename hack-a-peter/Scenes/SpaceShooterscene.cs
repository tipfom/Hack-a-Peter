using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hack_a_peter.EndData;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hack_a_peter.Scenes
{
    public class SpaceShooterScene : Scene
    {
        const int PLAYER_SIZE = 50;
        const int PLAYER_SPEED = 10;

        public override Color BackColor { get { return Color.DarkGray; } }
        public override string InitFile { get { return "spaceshooter.init"; } }
        public override bool IsMouseVisible { get { return false; } }
        public override string Name { get { return "scene_spaceshooter"; } }

        private List<Bug> bugs = new List<Bug>();
        private Vector3 playerPosition;
        private Vector2 playerSize;
        private Texture2D playerTexture;
        private Texture2D bugTexture;
        private Random random;

        public SpaceShooterScene(int seed)
        {
            random = new Random(seed);
        }

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            playerPosition = new Vector3(Game.WINDOW_WIDTH / 2, Game.WINDOW_HEIGHT / 2, 0);
            playerSize = new Vector2(PLAYER_SIZE, PLAYER_SIZE);
            playerTexture = Assets.Textures.Get("spacecraft");
            bugTexture = Assets.Textures.Get("bug");

            for(int i = 0; i < 10; i++)
            {
                bugs.Add(new Bug(random, playerPosition));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw player
            spriteBatch.Draw(playerTexture, new Rectangle(playerPosition.XY().ToPoint(), playerSize.ToPoint()), null, Color.White, playerPosition.Z, playerTexture.Bounds.Size.ToVector2() / 2, SpriteEffects.None, 1f);
            // draw bugs
            foreach (Bug bug in bugs)
            {
                spriteBatch.Draw(bugTexture, new Rectangle(bug.Transform.XY().ToPoint(), bug.Size.ToPoint()), null, Color.White, bug.Transform.Z, bugTexture.Bounds.Size.ToVector2() / 2, SpriteEffects.None, 1f);
            }
            Console.WriteLine(bugs[0].Transform.ToString());
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            if (keyboard.IsKeyDown(Keys.Up))
                playerPosition.Y -= PLAYER_SPEED;
            if (keyboard.IsKeyDown(Keys.Down))
                playerPosition.Y += PLAYER_SPEED;
            if (keyboard.IsKeyDown(Keys.Left))
                playerPosition.X -= PLAYER_SPEED;
            if (keyboard.IsKeyDown(Keys.Right))
                playerPosition.X += PLAYER_SPEED;

            playerPosition.X = Math.Min(Game.WINDOW_WIDTH - PLAYER_SIZE / 2, Math.Max(PLAYER_SIZE / 2, playerPosition.X));
            playerPosition.Y = Math.Min(Game.WINDOW_HEIGHT - PLAYER_SIZE / 2, Math.Max(PLAYER_SIZE / 2, playerPosition.Y));

            foreach(Bug bug in bugs)
            {
                bug.Update(dt);
            }
        }

        public class Bug
        {
            const int MIN_SIZE = 20;
            const int MAX_SIZE = 30;
            const int BUG_SPEED = 30;

            public Vector3 Transform;
            public Vector2 Size;
            private Vector2 Movement;

            public Bug(Random random, Vector3 playerPosition)
            {
                Size = new Vector2(random.Next(MIN_SIZE, MAX_SIZE), random.Next(MIN_SIZE, MAX_SIZE));
                switch (random.Next(0, 4))
                {
                    case 0:
                        // top
                        Transform = new Vector3(random.Next(0, Game.WINDOW_WIDTH), 0, 0);
                        break;
                    case 1:
                        // bottom
                        Transform = new Vector3(random.Next(0, Game.WINDOW_WIDTH), Game.WINDOW_HEIGHT, 0);
                        break;
                    case 2:
                        // left
                        Transform = new Vector3(0, random.Next(0, Game.WINDOW_HEIGHT), 0);
                        break;
                    case 3:
                        // right 
                        Transform = new Vector3(Game.WINDOW_WIDTH, random.Next(0, Game.WINDOW_HEIGHT), 0);
                        break;
                }
                float increase = (playerPosition.X - Transform.X) / (playerPosition.Y - Transform.Y);
                Movement = new Vector2((Transform.X < playerPosition.X) ? 1f : -1f, increase);
                Transform.Z = (float)Math.Tan(increase);
            }

            public void Update(float dt)
            {
                Transform = new Vector3(Transform.XY() + Movement * BUG_SPEED* (dt/1000f), Transform.Z);
            }
        }
    }
}
