using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using hack_a_peter.EndData;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Timers;

namespace hack_a_peter.Scenes
{
    public class SpaceShooterScene : Scene
    {
        const double PLAYER_SCALE = 1.5d;
        const int DEFAULT_PLAYER_HEIGHT = (int)(40 * PLAYER_SCALE);
        const int DEFAULT_PLAYER_WIDTH = (int)(24 * PLAYER_SCALE);
        const int PLAYER_SPEED = 10;
        const int BUG_COUNT = 17;
        const int BUGS_TO_FINISH = 42 + 13;
        const float ROTATION_PER_SECOND = (float)(Math.PI * 4);
        const float ROTATION_TOLERANCE = (float)(Math.PI / 16);
        const double SPAWN_RATE = 0.5 * 1000;

        public override Color BackColor { get { return Color.DarkGray; } }
        public override string InitFile { get { return "spaceshooter.init"; } }
        public override bool IsMouseVisible { get { return false; } }
        public override string Name { get { return "scene_spaceshooter"; } }

        private List<Bug> bugs = new List<Bug>();
        private Queue<Bug> diedBugs = new Queue<Bug>();
        private Vector3 playerPosition;
        private Vector2 playerSize;
        private Vector2 drawSize;
        private Texture2D playerTexture;
        private Texture2D bugTexture;
        private Texture2D backgroundTexture;
        private SpriteFont font;
        private Color textColor;
        private Random random;
        private int diedBugsCount = 0;

        public SpaceShooterScene(int seed)
        {
            random = new Random(seed);
            Timer spawnTimer = new Timer(SPAWN_RATE);
            spawnTimer.Elapsed += SpawnTimer_Elapsed;
            spawnTimer.Start();
        }

        private void SpawnTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // called on main thread
            if (bugs.Count < BUG_COUNT)
                CreateBug();
        }

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            playerPosition = new Vector3(Game.WINDOW_WIDTH / 2, Game.WINDOW_HEIGHT / 2, 0);
            drawSize = new Vector2(DEFAULT_PLAYER_WIDTH, DEFAULT_PLAYER_HEIGHT);
            playerSize = new Vector2(DEFAULT_PLAYER_WIDTH, DEFAULT_PLAYER_HEIGHT);
            playerTexture = Assets.Textures.Get("space_craft");
            bugTexture = Assets.Textures.Get("space_bug");
            backgroundTexture = Assets.Textures.Get("space_background");
            font = Assets.Fonts.Get("14px");
            textColor = new Color(15, 56, 15);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw background
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT), Color.White);
            // draw player
            spriteBatch.Draw(playerTexture, new Rectangle(playerPosition.XY().ToPoint(), drawSize.ToPoint()), null, Color.White, playerPosition.Z, playerTexture.Bounds.Size.ToVector2() / 2, SpriteEffects.None, 1f);
            // draw bugs
            foreach (Bug bug in bugs)
            {
                spriteBatch.Draw(bugTexture, new Rectangle(bug.Transform.XY().ToPoint(), bug.Size.ToPoint()), null, Color.White, bug.Transform.Z, bugTexture.Bounds.Size.ToVector2() / 2, SpriteEffects.None, 1f);
            }
            // draw remaining bug count
            spriteBatch.DrawString(font, $"UEBERLEBE NOCH {BUGS_TO_FINISH - diedBugsCount:X2} BUGS", new Vector2(5, 5), textColor);
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            if (diedBugsCount >= BUGS_TO_FINISH)
            {
                throw new IndexOutOfRangeException("hurra!");
            }
            
            if (keyboard.IsKeyDown(Keys.Up))
            {
                playerPosition.Y -= PLAYER_SPEED;
                if (playerPosition.Z < 2 * Math.PI - ROTATION_TOLERANCE && playerPosition.Z > ROTATION_TOLERANCE)
                {
                    playerPosition.Z += ((playerPosition.Z > Math.PI) ? 1 : -1) * ROTATION_PER_SECOND * (dt / 1000f);
                }
                else
                {
                    playerPosition.Z = 0;
                    playerSize = new Vector2(DEFAULT_PLAYER_WIDTH, DEFAULT_PLAYER_HEIGHT);
                }
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                playerPosition.Y += PLAYER_SPEED;
                if (playerPosition.Z < Math.PI - ROTATION_TOLERANCE)
                {
                    playerPosition.Z += ROTATION_PER_SECOND * (dt / 1000f);
                }
                else if (playerPosition.Z > Math.PI + ROTATION_TOLERANCE)
                {
                    playerPosition.Z -= ROTATION_PER_SECOND * (dt / 1000f);
                }
                else
                {
                    playerPosition.Z = (float)Math.PI;
                    playerSize = new Vector2(DEFAULT_PLAYER_WIDTH, DEFAULT_PLAYER_HEIGHT);
                }
            }
            else            if (keyboard.IsKeyDown(Keys.Left))
            {
                playerPosition.X -= PLAYER_SPEED;
                if (playerPosition.Z > 1.5 * Math.PI + ROTATION_TOLERANCE || playerPosition.Z < 0.5 * Math.PI)
                {
                    if (playerPosition.Z < ROTATION_TOLERANCE)
                        playerPosition.Z = (float)(2 * Math.PI);
                    playerPosition.Z -= ROTATION_PER_SECOND * (dt / 1000f);
                }
                else if (playerPosition.Z < 1.5 * Math.PI - ROTATION_TOLERANCE)
                {
                    playerPosition.Z += ROTATION_PER_SECOND * (dt / 1000f);
                }
                else
                {
                    playerPosition.Z = (float)(1.5 * Math.PI);
                    playerSize = new Vector2(DEFAULT_PLAYER_HEIGHT, DEFAULT_PLAYER_WIDTH);
                }
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                playerPosition.X += PLAYER_SPEED;
                if (playerPosition.Z < 0.5 * Math.PI - ROTATION_TOLERANCE || playerPosition.Z > 1.5 * Math.PI)
                {
                    if (playerPosition.Z > 2 * Math.PI - ROTATION_TOLERANCE)
                        playerPosition.Z = 0f;
                    playerPosition.Z += ROTATION_PER_SECOND * (dt / 1000f);
                }
                else if (playerPosition.Z > 0.5 * Math.PI + ROTATION_TOLERANCE)
                {
                    playerPosition.Z -= ROTATION_PER_SECOND * (dt / 1000f);
                }
                else
                {
                    playerPosition.Z = (float)(0.5 * Math.PI);
                    playerSize = new Vector2(DEFAULT_PLAYER_HEIGHT, DEFAULT_PLAYER_WIDTH);
                }
            }

            playerPosition.X = Math.Min(Game.WINDOW_WIDTH - playerSize.X / 2, Math.Max(playerSize.X / 2, playerPosition.X));
            playerPosition.Y = Math.Min(Game.WINDOW_HEIGHT - playerSize.Y / 2, Math.Max(playerSize.Y / 2, playerPosition.Y));
            playerPosition.Z = Math.Max(0,playerPosition.Z % (float)(2 * Math.PI));

            while (diedBugs.Count > 0)
            {
                bugs.Remove(diedBugs.Dequeue());
            }
            
            foreach (Bug bug in bugs)
            {
                bug.Update(dt);
                if (bug.Collides(playerPosition, playerSize))
                    OnFinished("death_screen", new GameEndData(diedBugsCount, Name));
            }
        }

        private void CreateBug()
        {
            Bug createdBug = new Bug(random, playerPosition);
            createdBug.Died += (sender, bug) => { diedBugs.Enqueue(bug); diedBugsCount++; };
            bugs.Add(createdBug);
        }

        public class Bug
        {
            const int MIN_SIZE = 20;
            const int MAX_SIZE = 40;
            const int MIN_BUG_SPEED = 50;
            const int MAX_BUG_SPEED = 200;
            const float MIN_ROTATION_PER_SECOND = (float)(Math.PI * 1d);
            const float MAX_ROTATION_PER_SECOND = (float)(Math.PI * 2.5d);

            public Vector3 Transform;
            public Vector2 Size;
            private float Increase;
            private float Direction;
            private int speed;
            private float rotation;

            public event EventHandler<Bug> Died;

            public Bug(Random random, Vector3 playerPosition)
            {
                Size = new Vector2(random.Next(MIN_SIZE, MAX_SIZE), random.Next(MIN_SIZE, MAX_SIZE));
                speed = random.Next(MIN_BUG_SPEED, MAX_BUG_SPEED);
                rotation = (float)( MIN_ROTATION_PER_SECOND + random.NextDouble() * (MAX_ROTATION_PER_SECOND - MIN_ROTATION_PER_SECOND));
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
                Increase = (playerPosition.Y - Transform.Y) / (playerPosition.X - Transform.X);
                Transform.Z = (float)Math.Tan(Increase);
                Direction = (playerPosition.X > Transform.X) ? 1f : -1f;
            }

            public void Update(float dt)
            {
                float movementX = (float)Math.Cos(Math.Atan(Increase)) * speed * (dt / 1000f) * Direction;
                Vector2 movement = new Vector2(movementX, movementX * Increase);
                Transform = new Vector3(Transform.XY() + movement, (Transform.Z + rotation * (dt / 1000f)) % (2f * (float)Math.PI));

                if (OutOfMap())
                    Died?.Invoke(null, this);
            }

            public bool Collides(Vector3 playerPosition, Vector2 playerSize)
            {
                return !(
                    playerPosition.X - playerSize.X / 2 > this.Transform.X + this.Size.X / 2 ||
                    playerPosition.X + playerSize.X / 2 < this.Transform.X - this.Size.X / 2 ||
                    playerPosition.Y - playerSize.Y / 2 > this.Transform.Y + this.Size.Y / 2 ||
                    playerPosition.Y + playerSize.Y / 2 < this.Transform.Y - this.Size.Y / 2
                    );
            }

            private bool OutOfMap()
            {
                return (
                    this.Transform.Y - this.Size.Y / 2 >= Game.WINDOW_HEIGHT ||
                    this.Transform.Y + this.Size.Y / 2 <= 0 ||
                    this.Transform.X - this.Size.X / 2 >= Game.WINDOW_WIDTH ||
                    this.Transform.X + this.Size.X / 2 <= 0
                    );
            }
        }
    }
}
