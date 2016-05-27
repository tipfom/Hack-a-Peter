using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using hack_a_peter.EndData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using hack_a_peter.Scenes.Strategy;

namespace hack_a_peter.Scenes
{
    public class StrategyScene : Scene
    {
        public override Color BackColor
        {
            get
            {
                return Color.Pink;
            }
        }
        public override string InitFile
        {
            get
            {
                return "strategy.init";
            }
        }
        public override bool IsMouseVisible
        {
            get
            {
                return true;
            }
        }
        public override string Name
        {
            get
            {
                return "strategy";
            }
        }

        private Strategy.Tile[,] Field = new Strategy.Tile[1,1];
        private Point Camera;
        private Point SelectedTile = new Point(0, 0);
        private MouseState PreviousState;
        private HashSet<Point> FramesAt = new HashSet<Point>();
        private List<Button> Buttons = new List<Button>();
        private Point CurrentPos;
        private Unit CurrentUnit;

        public override void Begin(EndData.EndData lastSceneEndData)
        {
            Field = new Strategy.Tile[32, 32];
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    Field[x, y] = new Strategy.Tile();
                }
            }
            this.LoadMap(Environment.CurrentDirectory + "\\Assets\\main.hpmap");
            Unit Opponent = new Unit();
            Opponent.MovementLeft = 5;
            Opponent.MoveSpeed = 5;
            Opponent.Weapons.Add(new Gun());
            Opponent.Texture = UnitTexture.Hero2;
            Field[15, 9].Unit = Opponent;
            SelectedTile = new Point(0, 0);
            Camera = new Point(0, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Drawing the Gamefield
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    switch (Field[x, y].Type)
                    {
                        case 0:
                            spriteBatch.Draw(Assets.Textures.Get("tile_default"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                            break;
                        case 1:
                            spriteBatch.Draw(Assets.Textures.Get("tile_smallcover"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(Assets.Textures.Get("tile_bigcover"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                            break;
                        case 3:
                            spriteBatch.Draw(Assets.Textures.Get("tile_impossible"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                            break;
                    }
                    if (Field[x, y].Unit != null)
                    {
                        switch (Field[x,y].Unit.Texture)
                        {
                            case UnitTexture.Bug1:
                                spriteBatch.Draw(Assets.Textures.Get("enemy1"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                                break;
                            case UnitTexture.Bug2:
                                spriteBatch.Draw(Assets.Textures.Get("enemy2"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                                break;
                            case UnitTexture.Hero1:
                                spriteBatch.Draw(Assets.Textures.Get("hero1"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                                break;
                            case UnitTexture.Hero2:
                                spriteBatch.Draw(Assets.Textures.Get("hero2"), new Rectangle(new Point(x * 40 - Camera.X, y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            foreach (Point OnePoint in FramesAt)
            {
                spriteBatch.Draw(Assets.Textures.Get("frame"), new Rectangle(((OnePoint) * new Point(40, 40) - Camera), new Point(40, 40)), Color.White);
            }

            //Drawing the selected cursor
            spriteBatch.Draw(Assets.Textures.Get("selected"), new Rectangle(new Point(SelectedTile.X * 40 - Camera.X, SelectedTile.Y * 40 - Camera.Y), new Point(40, 40)), Color.White);

            string TileInfo= "Tile Info";
            try
            {
                switch (Field[SelectedTile.X, SelectedTile.Y].Type)
                {
                    case 0:
                        TileInfo = "Flat";
                        break;
                    case 1:
                        TileInfo = "Medium Cover";
                        break;
                    case 2:
                        TileInfo = "Full Cover";
                        break;
                    case 3:
                        TileInfo = "Unpassable";
                        break;
                    default:
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            { }
            spriteBatch.DrawString(Assets.Fonts.Get("14px"), TileInfo + " " + SelectedTile.ToString(), new Vector2(0, Game.WINDOW_HEIGHT - 30),Color.Black);

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].Rectangle = new Rectangle(240 + i * 100, Game.WINDOW_HEIGHT - 40, 80, 40);
                switch (Buttons[i].Texture)
                {
                    case UnitButton.Walk:
                        spriteBatch.Draw(Assets.Textures.Get("button_walk"), Buttons[i].Rectangle, Color.White);
                        break;
                    case UnitButton.Reload:
                        break;
                    case UnitButton.Gun:
                        spriteBatch.Draw(Assets.Textures.Get("button_mg"), Buttons[i].Rectangle, Color.White);
                        break;
                    case UnitButton.Greande:
                        break;
                    case UnitButton.Smoke:
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse)
        {
            //Camera
            if (keyboard.IsKeyDown(Keys.Left) && Camera.X - 5 >= 0)
            {
                Camera.X -= 5;
            }
            if (keyboard.IsKeyDown(Keys.Right) && Camera.X + 5 <= Field.GetLength(0) * 40 - Game.WINDOW_WIDTH)
            {
                Camera.X += 5;
            }
            if (keyboard.IsKeyDown(Keys.Up) && Camera.Y - 5 >= 0)
            {
                Camera.Y -= 5;
            }
            if (keyboard.IsKeyDown(Keys.Down) && Camera.Y + 5 <= Field.GetLength(1) * 40 - Game.WINDOW_HEIGHT)
            {
                Camera.Y += 5;
            }
            //Selected Tile
            SelectedTile = (mouse.Position + Camera) / new Point(40, 40);
            SelectedTile.X = (SelectedTile.X < 0) ? 0 : SelectedTile.X;
            SelectedTile.Y = (SelectedTile.Y < 0) ? 0 : SelectedTile.Y;
            SelectedTile.X = (SelectedTile.X > this.Field.GetLength(0) - 1) ? this.Field.GetLength(0) - 1 : SelectedTile.X;
            SelectedTile.Y = (SelectedTile.Y > this.Field.GetLength(1) - 1) ? this.Field.GetLength(1) - 1 : SelectedTile.Y;
            //MouseInput
            if (mouse.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released)
            {
                bool Handled = false;
                foreach (Button OneButton in Buttons)
                {
                    if (OneButton.IsInShape(mouse.Position.X, mouse.Position.Y))
                    {
                        OneButton.TriggerOnClick();
                        Handled = true;
                    }
                }

                Buttons.Clear();

                if (Field[SelectedTile.X, SelectedTile.Y].Unit != null & !Handled)
                {
                    CurrentUnit = Field[SelectedTile.X, SelectedTile.Y].Unit;
                    CurrentPos = new Point(SelectedTile.X, SelectedTile.Y);
                    if (CurrentUnit.MovementLeft > 0)
                    {
                        Button NewButton = new Button(UnitButton.Walk);
                        NewButton.OnClick += Walk_OnClick;
                        Buttons.Add(NewButton);
                    }
                    foreach (Weapon OneWeapon in CurrentUnit.Weapons)
                    {
                        if (OneWeapon.GetType() == typeof(Gun))
                        {
                            Buttons.Add(new Button(UnitButton.Gun));
                        }
                    }
                }
            }
            PreviousState = mouse;
        }

        private void Walk_OnClick(object sender, EventArgs e)
        {
            List<Point> Cache = new List<Point>();
            FramesAt.Clear();
            Cache.AddRange(this.Expand(CurrentPos, CurrentUnit.MovementLeft));
            FramesAt = new HashSet<Point>(Cache);
        }

        private Point[] Expand(Point pos, int count)
        {
            List<Point> ForReturn = new List<Point>();
            if (count == 1)
            {
                if (this.Field[pos.X + 1, pos.Y].Type != 3)
                {
                    ForReturn.Add(pos + new Point(1, 0));
                }
                if (this.Field[pos.X - 1, pos.Y].Type != 3)
                {
                    ForReturn.Add(pos + new Point(-1, 0));
                }
                if (this.Field[pos.X, pos.Y + 1].Type != 3)
                {
                    ForReturn.Add(pos + new Point(0, 1));
                }
                if (this.Field[pos.X, pos.Y - 1].Type != 3)
                {
                    ForReturn.Add(pos + new Point(0, -1));
                }
            }
            else
            {
                if (this.Field[pos.X + 1, pos.Y].Type != 3)
                {
                    ForReturn.AddRange(this.Expand(pos + new Point(1, 0), count - 1));
                }
                if (this.Field[pos.X - 1, pos.Y].Type != 3)
                {
                    ForReturn.AddRange(this.Expand(pos + new Point(-1, 0), count - 1));
                }
                if (this.Field[pos.X, pos.Y + 1].Type != 3)
                {
                    ForReturn.AddRange(this.Expand(pos + new Point(0, 1), count - 1));
                }
                if (this.Field[pos.X, pos.Y - 1].Type != 3)
                {
                    ForReturn.AddRange(this.Expand(pos + new Point(0, -1), count - 1));
                }
            }
            return ForReturn.ToArray();
        }

        private void LoadMap(string path)
        {
            bool there = File.Exists(path); 
            StreamReader MyReader = new StreamReader(File.OpenRead(path));
            int Line = 0;
            while (!MyReader.EndOfStream)
            {
                string Content = MyReader.ReadLine();
                for (int i = 0; i < Content.Length; i++)
                {
                    int Type = 0;
                    switch (Content[i])
                    {
                        case '0':
                            Type = 0;
                            break;
                        case 'H':
                            Type = 1;
                            break;
                        case '#':
                            Type = 2;
                            break;
                        case 'X':
                            Type = 3;
                            break;
                    }
                    Field[i, Line].Type = Type;
                }
                Line++;
            }
        }
    }
}
