using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hack_a_peter.EndData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Timers;

namespace hack_a_peter.Scenes
{
    public class MinesweeperScene : Scene
    {
        const int FIELD_SIZE = 13;
        const int MINE_COUNT = 1;
        const int END_PAUSE = 5 * 1000;

        public override string InitFile { get { return "minesweeper.init"; } }
        public override string Name { get { return "scene_minesweeper"; } }
        public override Color BackColor { get { return Game.GB4; } }
        public override bool IsMouseVisible { get { return true; } }

        private bool previouslyClicked;
        private bool fieldGenerated;
        private bool[,] mines;
        private bool[,] visible;
        private int[,] hints;
        private Random random;
        private int mineSize;
        private int shownTiles;

        private Texture2D mineTexture;
        private Texture2D hideTexture;
        private Texture2D[] numberTextures;

        public MinesweeperScene(int seed)
        {
            random = new Random(seed);
        }

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            mines = new bool[FIELD_SIZE, FIELD_SIZE];
            visible = new bool[FIELD_SIZE, FIELD_SIZE];
            hints = new int[FIELD_SIZE, FIELD_SIZE];

            shownTiles = 0;
            mineSize = Math.Min(Game.WINDOW_HEIGHT, Game.WINDOW_WIDTH) / FIELD_SIZE;
            fieldGenerated = false;
            mineTexture = Assets.Textures.Get("minesweeper_bomb");
            hideTexture = Assets.Textures.Get("minesweeper_hide");
            numberTextures = new Texture2D[9];
            for (int i = 0; i < 9; i++)
            {
                numberTextures[i] = Assets.Textures.Get($"minesweeper_{i}");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < FIELD_SIZE; x++)
            {
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    Rectangle drawRectangle = new Rectangle(x * mineSize, y * mineSize, mineSize, mineSize);
                    if(!visible[x,y])
                        spriteBatch.Draw(hideTexture, drawRectangle, Color.White);
                    else if (mines[x, y])
                        spriteBatch.Draw(mineTexture, drawRectangle, Color.White);
                    else if (visible[x, y])
                        spriteBatch.Draw(numberTextures[hints[x, y]], drawRectangle, Color.White);
                 }
            }
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            if(FIELD_SIZE * FIELD_SIZE - MINE_COUNT == shownTiles)
            {
                // user completed
                throw new IndexOutOfRangeException("hurra!!!11!!!elf du hast es geschafft!");
            }

            if (previouslyClicked && mouse.LeftButton == ButtonState.Released)
                previouslyClicked = false;
            else if (!previouslyClicked && mouse.LeftButton == ButtonState.Pressed)
            {
                Point clickedMine = GetMine(mouse.Position);
                if (clickedMine.X < 0 || clickedMine.X >= FIELD_SIZE || clickedMine.Y < 0 || clickedMine.Y >= FIELD_SIZE)
                    return;
                Console.WriteLine(clickedMine);
                if (!fieldGenerated)
                    GenerateField(clickedMine);
                else
                    HandleClick(clickedMine);

                previouslyClicked = true;
            }
        }

        private void GenerateField(Point exclude)
        {
            fieldGenerated = true;

            List<Point> availablePoints = new List<Point>();
            for (int x = 0; x < FIELD_SIZE; x++)
            {
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    availablePoints.Add(new Point(x, y));
                }
            }
            availablePoints.Remove(exclude);

            for (int i = 0; i < MINE_COUNT; i++)
            {
                int mineindex = random.Next(0, availablePoints.Count);
                Point mine = availablePoints[mineindex];
                availablePoints.RemoveAt(mineindex);
                mines[mine.X, mine.Y] = true;
            }

            HandleClick(exclude);
        }

        private Point GetMine(Point pointOnScreen)
        {
            return new Point(pointOnScreen.X / mineSize, pointOnScreen.Y / mineSize);
        }

        private void HandleClick(Point point)
        {
            if (visible[point.X, point.Y])
                return;
            if (mines[point.X, point.Y])
                HandleEnding();

            // expand visible bounds
            Stack<Point> stack = new Stack<Point>();
            stack.Push(point);
            while (stack.Count > 0)
            {
                Point plate = stack.Pop();
                if (!visible[plate.X, plate.Y])
                {
                    visible[plate.X, plate.Y] = true;
                    shownTiles++;

                    List<Point> neighbours = GetNeighbours(plate);
                    int mineNeighbours = neighbours.Sum(p => (mines[p.X, p.Y] ? 1 : 0));
                    hints[plate.X, plate.Y] = mineNeighbours;
                    if (mineNeighbours == 0)
                    {
                        neighbours.ForEach(item => stack.Push(item));
                    }
                }
            }
        }

        private void HandleEnding()
        {
            for(int x = 0; x < FIELD_SIZE; x++)
            {
                for(int y = 0; y < FIELD_SIZE; y++)
                {
                    visible[x, y] = true;
                }
            }

            Timer timer = new Timer(END_PAUSE);
            timer.Elapsed += (sender, e) => { timer.Close(); OnFinished("death_screen", new GameEndData(shownTiles, Name)); };
            timer.Start();
        }

        private List<Point> GetNeighbours(Point point)
        {
            List<Point> neighbours = new List<Point>();
            if (point.X > 0)
            {
                neighbours.Add(new Point(point.X - 1, point.Y));
                if (point.Y < FIELD_SIZE - 1)
                    neighbours.Add(new Point(point.X - 1, point.Y + 1));
                if (point.Y > 0)
                    neighbours.Add(new Point(point.X - 1, point.Y - 1));
            }
            if (point.X < FIELD_SIZE - 1)
            {
                neighbours.Add(new Point(point.X + 1, point.Y));
                if (point.Y < FIELD_SIZE - 1)
                    neighbours.Add(new Point(point.X + 1, point.Y + 1));
                if (point.Y > 0)
                    neighbours.Add(new Point(point.X + 1, point.Y - 1));
            }
            if (point.Y > 0)
                neighbours.Add(new Point(point.X, point.Y - 1));
            if (point.Y < FIELD_SIZE - 1)
                neighbours.Add(new Point(point.X, point.Y + 1));

            return neighbours;
        }
    }
}
