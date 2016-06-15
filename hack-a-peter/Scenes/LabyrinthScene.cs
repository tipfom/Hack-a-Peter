using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace hack_a_peter.Scenes
{
    public class LabyrinthScene : Scene
    {
        public const string NAME = "game_labyrinth";

        const int LABYRINTH_SIZE = 42 - 8;
        const int LABYRINTH_DRAW_SIZE = 5;
        const int DEATH_TIME = 175;
        const int TEXT_FADE_TIME = 300;

        public override string Intro
        {
            get
            {
                return
                     $"YOU GOT {DEATH_TIME} SECONDS TO REACH THE BOTTOM LEFT CORNER OF THE LABYRINTH!\n" +
                      "GOOD LUCK\n";
            }
        }

        public override Color BackColor { get { return Game.GB4; } }
        public override string InitFile { get { return "labyrinth.init"; } }
        public override bool IsMouseVisible { get { return false; } }
        public override string Name { get { return NAME; } }

        private Connection[,] labyrinth;
        private Random random;
        private Color[][,] colors;
        private Texture2D rectTexture;
        private Texture2D playerTexture;
        private Point playerPosition;

        private Point playerPositionOnScreen;
        private Point scrollPosition;
        private Point drawBeginPosition;
        private int steps;
        private Timer deathTimer;
        private int remainingSeconds;
        private int textTime;

        public LabyrinthScene(int seed, GraphicsDevice g)
        {
            random = new Random(seed);
            rectTexture = new Texture2D(g, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });
        }

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            playerTexture = Assets.Textures.Get("labyrinth_player");

            colors = new Color[][,]
            {
                new Color[,] // 0
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 1
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 2
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] // 3
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] // 4
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 5
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 6
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] // 7
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] // 8
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 9
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 10
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] // 11
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB4},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] // 12
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 13
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB4,Game.GB4 }
                },
                new Color[,] // 14
                {
                    { Game.GB4,Game.GB4,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB1,Game.GB4 }
                },
                new Color[,] //15
                {
                    { Game.GB4,Game.GB1,Game.GB4},
                    { Game.GB1,Game.GB1,Game.GB1},
                    { Game.GB4,Game.GB1,Game.GB4 }
                }
            };

            playerPosition = new Point(0, 0);
            playerPositionOnScreen = new Point(0, 0);
            scrollPosition = new Point(0, 0);

            int drawOffset = (Math.Max(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT) - Math.Min(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT)) / 2;
            drawBeginPosition = new Point(
                (Game.WINDOW_WIDTH < Game.WINDOW_HEIGHT) ? 0 : drawOffset,
                (Game.WINDOW_HEIGHT < Game.WINDOW_WIDTH) ? 0 : drawOffset
                );

            remainingSeconds = DEATH_TIME;
            deathTimer = new Timer(1000);
            deathTimer.Elapsed += (object sender, ElapsedEventArgs e) => { remainingSeconds--; Console.WriteLine(remainingSeconds); textTime = TEXT_FADE_TIME; };

            GenerateLabyrinth();
        }

        private void GenerateLabyrinth()
        {
            // create and reset labyrinth
            labyrinth = new Connection[LABYRINTH_SIZE, LABYRINTH_SIZE];

            // build some connections \o/
            HashSet<Point> visited = new HashSet<Point>();
            Stack<Point> stack = new Stack<Point>();
            Point currentCell = new Point(0, 0);//new Point(random.Next(0, LABYRINTH_SIZE), random.Next(0, LABYRINTH_SIZE));
            visited.Add(currentCell);

            while (visited.Count < LABYRINTH_SIZE * LABYRINTH_SIZE)
            {
                List<Point> cellNeighbours = GetCellNeighbours(currentCell).Where(cell => !visited.Contains(cell)).ToList();
                if (cellNeighbours.Count > 0)
                {
                    stack.Push(currentCell);
                    int selectedNeighbour = random.Next(0, cellNeighbours.Count);
                    Connect(currentCell, cellNeighbours[selectedNeighbour]);
                    currentCell = cellNeighbours[selectedNeighbour];
                    visited.Add(cellNeighbours[selectedNeighbour]);
                }
                else if (stack.Count > 0)
                {
                    currentCell = stack.Pop();
                }
            }
        }

        private List<Point> GetCellNeighbours(Point cell)
        {
            List<Point> neighbours = new List<Point>();
            if (cell.X > 0)
                neighbours.Add(new Point(cell.X - 1, cell.Y));
            if (cell.X < LABYRINTH_SIZE - 1)
                neighbours.Add(new Point(cell.X + 1, cell.Y));
            if (cell.Y > 0)
                neighbours.Add(new Point(cell.X, cell.Y - 1));
            if (cell.Y < LABYRINTH_SIZE - 1)
                neighbours.Add(new Point(cell.X, cell.Y + 1));
            return neighbours;
        }

        private void Connect(Point cell1, Point cell2)
        {
            if (cell1.X > cell2.X)
            {
                labyrinth[cell1.X, cell1.Y] |= Connection.West;
                labyrinth[cell2.X, cell2.Y] |= Connection.East;
            }
            else if (cell1.X < cell2.X)
            {
                labyrinth[cell1.X, cell1.Y] |= Connection.East;
                labyrinth[cell2.X, cell2.Y] |= Connection.West;
            }
            else if (cell1.Y > cell2.Y)
            {
                labyrinth[cell1.X, cell1.Y] |= Connection.North;
                labyrinth[cell2.X, cell2.Y] |= Connection.South;
            }
            else if (cell1.Y < cell2.Y)
            {
                labyrinth[cell1.X, cell1.Y] |= Connection.South;
                labyrinth[cell2.X, cell2.Y] |= Connection.North;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int size = Game.WINDOW_HEIGHT / LABYRINTH_DRAW_SIZE;
            int squaresize = size / 3;
            for (int x = 0; x < LABYRINTH_DRAW_SIZE; x++)
            {
                for (int y = 0; y < LABYRINTH_DRAW_SIZE; y++)
                {
                    int index = (int)labyrinth[x + scrollPosition.X, y + scrollPosition.Y];
                    Color[,] rectColor = colors[index];
                    for (int xr = 0; xr < 3; xr++)
                    {
                        for (int yr = 0; yr < 3; yr++)
                        {
                            Rectangle drawRectangle = new Rectangle(
                                drawBeginPosition.X + x * size + xr * squaresize,
                                drawBeginPosition.Y + y * size + yr * squaresize,
                                (xr == 2) ? size - 2 * squaresize : squaresize,
                                (yr == 2) ? size - 2 * squaresize : squaresize);
                            Color color = rectColor[yr, xr];
                            spriteBatch.Draw(rectTexture, drawRectangle, color);
                        }
                    }
                }
            }

            Color playerColor = Game.GB4;
            playerColor.A = 100;
            spriteBatch.Draw(playerTexture, new Rectangle(drawBeginPosition.X + playerPositionOnScreen.X * size + squaresize, drawBeginPosition.Y + playerPositionOnScreen.Y * size + squaresize, squaresize, squaresize), Color.White);

            if (textTime > 0)
            {
                // draw time
                SpriteFont font = Assets.Fonts.Get("72px");
                Vector2 textSize = font.MeasureString(remainingSeconds.ToString());
                spriteBatch.DrawString(font, remainingSeconds.ToString(), new Vector2(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT) / 2 - textSize / 2, Game.GB3);
            }
        }


        bool goneUp, goneDown, goneLeft, goneRight;
        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            if (!deathTimer.Enabled)
                deathTimer.Start();
            if (remainingSeconds <= 0)
            {
                OnFinished(ScreenOfDeath.NAME, new EndData.GameEndData(0, NAME));
                deathTimer.Stop();
            }

            textTime -= dt;

            if (playerPosition.X == LABYRINTH_SIZE - 1 && playerPosition.Y == LABYRINTH_SIZE - 1)
            {
                deathTimer.Stop();
                OnFinished(MinesweeperScene.NAME, new EndData.EndData(NAME));
            }

            if ((keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S)) && !goneDown && labyrinth[playerPosition.X, playerPosition.Y].HasFlag(Connection.South))
            {
                playerPosition.Y = Math.Min(playerPosition.Y + 1, LABYRINTH_SIZE - 1);
                goneDown = true;
                steps++;
            }
            else if ((keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) && !goneUp && labyrinth[playerPosition.X, playerPosition.Y].HasFlag(Connection.North))
            {
                playerPosition.Y = Math.Max(playerPosition.Y - 1, 0);
                goneUp = true;
                steps++;
            }
            else if ((keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) && !goneLeft && labyrinth[playerPosition.X, playerPosition.Y].HasFlag(Connection.West))
            {
                playerPosition.X = Math.Max(playerPosition.X - 1, 0);
                goneLeft = true;
                steps++;
            }
            else if ((keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) && !goneRight && labyrinth[playerPosition.X, playerPosition.Y].HasFlag(Connection.East))
            {
                playerPosition.X = Math.Min(playerPosition.X + 1, LABYRINTH_SIZE - 1);
                goneRight = true;
                steps++;
            }

            if ((keyboard.IsKeyUp(Keys.Up) && keyboard.IsKeyUp(Keys.W)) && goneUp)
                goneUp = false;
            if ((keyboard.IsKeyUp(Keys.Down) && keyboard.IsKeyUp(Keys.S)) && goneDown)
                goneDown = false;
            if ((keyboard.IsKeyUp(Keys.Left) && keyboard.IsKeyUp(Keys.A)) && goneLeft)
                goneLeft = false;
            if ((keyboard.IsKeyUp(Keys.Right) && keyboard.IsKeyUp(Keys.D)) && goneRight)
                goneRight = false;

            scrollPosition.X = Math.Min(LABYRINTH_SIZE - LABYRINTH_DRAW_SIZE, Math.Max(0, playerPosition.X - LABYRINTH_DRAW_SIZE / 2));
            scrollPosition.Y = Math.Min(LABYRINTH_SIZE - LABYRINTH_DRAW_SIZE, Math.Max(0, playerPosition.Y - LABYRINTH_DRAW_SIZE / 2));

            bool onTheLeft = scrollPosition.X == 0;
            bool onTheRight = scrollPosition.X == LABYRINTH_SIZE - LABYRINTH_DRAW_SIZE;
            bool onTheTop = scrollPosition.Y == 0;
            bool onTheBottom = scrollPosition.Y == LABYRINTH_SIZE - LABYRINTH_DRAW_SIZE;

            playerPositionOnScreen.X =
                (!onTheLeft && !onTheRight) ? LABYRINTH_DRAW_SIZE / 2 :
                (onTheLeft) ? playerPosition.X :
                playerPosition.X - LABYRINTH_SIZE + LABYRINTH_DRAW_SIZE;
            playerPositionOnScreen.Y =
                (!onTheTop && !onTheBottom) ? LABYRINTH_DRAW_SIZE / 2 :
                (onTheTop) ? playerPosition.Y :
                playerPosition.Y - LABYRINTH_SIZE + LABYRINTH_DRAW_SIZE;
        }

        [Flags]
        public enum Connection
        {
            None = 0,
            North = 1,
            South = 2,
            East = 4,
            West = 8
        }
    }
}
