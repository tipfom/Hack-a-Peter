using hack_a_peter.Scenes.Strategy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hack_a_peter.Scenes {
    public class StrategyScene : Scene {
        public const string NAME = "game_strategy";

        public override Color BackColor {
            get {
                return Color.Pink;
            }
        }
        public override string InitFile {
            get {
                return "strategy.init";
            }
        }
        public override bool IsMouseVisible {
            get {
                return true;
            }
        }
        public override string Name {
            get {
                return NAME;
            }
        }

        //0 = Player 1 = AI
        private int ActivePlayer;
        private int InactivePlayer;
        private Tile[ , ] Field = new Strategy.Tile[1, 1];
        private Point Camera;
        private Point SelectedTile = new Point(0, 0);
        private MouseState PreviousState;
        private List<Point> FramesAt = new List<Point>( );
        private List<Button> Buttons = new List<Button>( );
        private List<Unit> AllUnits = new List<Unit>( );
        private ButtonTexture SelectedButton;
        private Point CurrentPos;
        private Unit CurrentUnit;

        public override void Begin(EndData.EndData lastSceneEndData) {
            AllUnits.Clear( );
            Field = new Strategy.Tile[32, 32];
            for (int x = 0; x < Field.GetLength(0); x++) {
                for (int y = 0; y < Field.GetLength(1); y++) {
                    Field[x, y] = new Strategy.Tile( );
                }
            }
            this.LoadMap(Environment.CurrentDirectory + "\\Assets\\main.hpmap");
            Unit OpponentUnit = new Unit( );
            OpponentUnit.Health = 20;
            OpponentUnit.MovementLeft = 5;
            OpponentUnit.MoveSpeed = 5;
            OpponentUnit.Weapons.Add(new Gun( ));
            OpponentUnit.Texture = UnitTexture.Bug1;
            OpponentUnit.Owner = 1;
            OpponentUnit.Position = new Point(15, 13);
            AllUnits.Add(OpponentUnit);

            Unit CoolerTyp = new Unit( );
            CoolerTyp.Health = 20;
            CoolerTyp.MovementLeft = 5;
            CoolerTyp.MoveSpeed = 5;
            CoolerTyp.Weapons.Add(new Gun( ));
            CoolerTyp.Texture = UnitTexture.Hero2;
            CoolerTyp.Owner = 0;
            CoolerTyp.Position = new Point(15, 9);
            AllUnits.Add(CoolerTyp);

            Button TurnEndButton = new Button(ButtonTexture.EndTurn);
            TurnEndButton.OnClick += EndTurn_OnClick;
            Buttons.Add(TurnEndButton);

            ActivePlayer = 0;
            InactivePlayer = 1;
            SelectedTile = new Point(0, 0);
            Camera = new Point(0, 0);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            //Drawing the Gamefield
            for (int x = 0; x < Field.GetLength(0); x++) {
                for (int y = 0; y < Field.GetLength(1); y++) {
                    switch (Field[x, y].Type) {
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
                }
            }

            //Drawing Units
            foreach (Unit OneUnit in AllUnits) {
                switch (OneUnit.Texture) {
                    case UnitTexture.Bug1:
                        spriteBatch.Draw(Assets.Textures.Get("enemy1"), new Rectangle(new Point(OneUnit.Position.X * 40 - Camera.X, OneUnit.Position.Y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                        break;
                    case UnitTexture.Bug2:
                        spriteBatch.Draw(Assets.Textures.Get("enemy2"), new Rectangle(new Point(OneUnit.Position.X * 40 - Camera.X, OneUnit.Position.Y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                        break;
                    case UnitTexture.Hero1:
                        spriteBatch.Draw(Assets.Textures.Get("hero1"), new Rectangle(new Point(OneUnit.Position.X * 40 - Camera.X, OneUnit.Position.Y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                        break;
                    case UnitTexture.Hero2:
                        spriteBatch.Draw(Assets.Textures.Get("hero2"), new Rectangle(new Point(OneUnit.Position.X * 40 - Camera.X, OneUnit.Position.Y * 40 - Camera.Y), new Point(40, 40)), Color.White);
                        break;
                    default:
                        break;
                }
            }

            //Drawing Frames
            foreach (Point OnePoint in FramesAt) {
                spriteBatch.Draw(Assets.Textures.Get("frame"), new Rectangle(((OnePoint) * new Point(40, 40) - Camera), new Point(40, 40)), Color.White);
            }

            //Drawing the selected cursor
            spriteBatch.Draw(Assets.Textures.Get("selected"), new Rectangle(new Point(SelectedTile.X * 40 - Camera.X, SelectedTile.Y * 40 - Camera.Y), new Point(40, 40)), Color.White);

            string TileInfo = "Tile Info";
            try {
                switch (Field[SelectedTile.X, SelectedTile.Y].Type) {
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
            } catch (IndexOutOfRangeException) { }
            spriteBatch.DrawString(Assets.Fonts.Get("14px"), TileInfo + " " + SelectedTile.ToString( ), new Vector2(0, Game.WINDOW_HEIGHT - 30), Color.Black);

            //Drawing Buttons
            for (int i = 0; i < Buttons.Count; i++) {
                Buttons[i].Rectangle = new Rectangle(240 + i * 100, Game.WINDOW_HEIGHT - 40, 80, 40);
                switch (Buttons[i].Texture) {
                    case ButtonTexture.Walk:
                        spriteBatch.Draw(Assets.Textures.Get("button_walk"), Buttons[i].Rectangle, Color.White);
                        break;
                    case ButtonTexture.Reload:
                        break;
                    case ButtonTexture.Gun:
                        spriteBatch.Draw(Assets.Textures.Get("button_mg"), Buttons[i].Rectangle, Color.White);
                        break;
                    case ButtonTexture.Greande:
                        break;
                    case ButtonTexture.Smoke:
                        break;
                    case ButtonTexture.EndTurn:
                        Buttons[i].Rectangle = new Rectangle(Game.WINDOW_WIDTH - 100, Game.WINDOW_HEIGHT - 40, 80, 40);
                        spriteBatch.Draw(Assets.Textures.Get("button_endturn"), Buttons[i].Rectangle, Color.White);
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Update(int dt, KeyboardState keyboard, MouseState mouse) {
            //Camera
            if (keyboard.IsKeyDown(Keys.Left) && Camera.X - 5 >= 0) {
                Camera.X -= 5;
            }
            if (keyboard.IsKeyDown(Keys.Right) && Camera.X + 5 <= Field.GetLength(0) * 40 - Game.WINDOW_WIDTH) {
                Camera.X += 5;
            }
            if (keyboard.IsKeyDown(Keys.Up) && Camera.Y - 5 >= 0) {
                Camera.Y -= 5;
            }
            if (keyboard.IsKeyDown(Keys.Down) && Camera.Y + 5 <= Field.GetLength(1) * 40 - Game.WINDOW_HEIGHT) {
                Camera.Y += 5;
            }
            //Selected Tile
            SelectedTile = (mouse.Position + Camera) / new Point(40, 40);
            SelectedTile.X = (SelectedTile.X < 0) ? 0 : SelectedTile.X;
            SelectedTile.Y = (SelectedTile.Y < 0) ? 0 : SelectedTile.Y;
            SelectedTile.X = (SelectedTile.X > this.Field.GetLength(0) - 1) ? this.Field.GetLength(0) - 1 : SelectedTile.X;
            SelectedTile.Y = (SelectedTile.Y > this.Field.GetLength(1) - 1) ? this.Field.GetLength(1) - 1 : SelectedTile.Y;
            //MouseInput
            if (mouse.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released && ActivePlayer == 0) {
                //Click auf Button
                bool Handled = false;
                foreach (Button OneButton in Buttons) {
                    if (OneButton.IsInShape(mouse.Position.X, mouse.Position.Y)) {
                        OneButton.TriggerOnClick( );
                        Handled = true;
                    }
                }

                //Click auf Feld mit Rahmen
                if (!Handled & FramesAt.Contains(SelectedTile)) {
                    switch (SelectedButton) {
                        case ButtonTexture.Walk:
                            CurrentUnit.Position = SelectedTile;
                            int Moved = Math.Abs(SelectedTile.X - CurrentPos.X) + Math.Abs(SelectedTile.Y - CurrentPos.Y);
                            CurrentUnit.MovementLeft -= Moved;
                            FramesAt.Clear( );
                            break;
                        case ButtonTexture.Reload:
                            break;
                        case ButtonTexture.Gun:
                            new Gun( ).Apply(this.Field, this.AllUnits, SelectedTile.X, SelectedTile.Y);
                            //AllUnits.RemoveAll(u => u.Position == SelectedTile);
                            FramesAt.Clear( );
                            break;
                        case ButtonTexture.Greande:
                            break;
                        case ButtonTexture.Smoke:
                            break;
                        case ButtonTexture.EndTurn:
                            break;
                        default:
                            break;
                    }
                    SelectedButton = ButtonTexture.None;
                }

                Buttons.Clear( );

                //Click auf Spielfeld
                if (AllUnits.Exists(u => u.Position == SelectedTile) & !Handled && AllUnits.First(u => u.Position == SelectedTile).Owner == 0) {
                    CurrentUnit = AllUnits.First(u => u.Position == SelectedTile);
                    CurrentPos = new Point(SelectedTile.X, SelectedTile.Y);
                    if (CurrentUnit.MovementLeft > 0) {
                        Button NewButton = new Button(ButtonTexture.Walk);
                        NewButton.OnClick += Walk_OnClick;
                        Buttons.Add(NewButton);
                    }
                    foreach (Weapon OneWeapon in CurrentUnit.Weapons) {
                        if (OneWeapon.GetType( ) == typeof(Gun)) {
                            Button NewButton = new Button(ButtonTexture.Gun);
                            NewButton.OnClick += Gun_OnClick;
                            Buttons.Add(NewButton);
                        }
                    }
                }

                Button TurnEndButton = new Button(ButtonTexture.EndTurn);
                TurnEndButton.OnClick += EndTurn_OnClick;
                Buttons.Add(TurnEndButton);
            }
            PreviousState = mouse;
        }

        private void Walk_OnClick(object sender, EventArgs e) {
            List<Point> Cache = new List<Point>( );
            FramesAt.Clear( );
            this.ExpandWalk(CurrentPos, CurrentUnit.MovementLeft);
            SelectedButton = ButtonTexture.Walk;
        }

        private Point[ ] ExpandWalk(Point pos, int count) {
            List<Point> ForReturn = new List<Point>( );
            if (this.Field[pos.X, pos.Y].Type != 3 & !AllUnits.Exists(u => u.Position == pos)) {
                AddToFrameList(pos);
            }
            if (count != 0) {
                if (pos.X + 1 < this.Field.GetLength(0) & this.Field[pos.X + 1, pos.Y].Type != 3) {
                    this.ExpandWalk(pos + new Point(1, 0), count - 1);
                }
                if (pos.X - 1 >= 0 & this.Field[pos.X - 1, pos.Y].Type != 3) {
                    this.ExpandWalk(pos + new Point(-1, 0), count - 1);
                }
                if (pos.Y + 1 < this.Field.GetLength(1) & this.Field[pos.X, pos.Y + 1].Type != 3) {
                    this.ExpandWalk(pos + new Point(0, 1), count - 1);
                }
                if (pos.Y - 1 >= 0 & this.Field[pos.X, pos.Y - 1].Type != 3) {
                    this.ExpandWalk(pos + new Point(0, -1), count - 1);
                }
            }
            return ForReturn.ToArray( );
        }

        private void Gun_OnClick(object sender, EventArgs e) {
            SelectedButton = ButtonTexture.Gun;
            foreach (Unit OneUnit in AllUnits) {
                int Distance = Math.Abs(OneUnit.Position.X - CurrentPos.X) + Math.Abs(OneUnit.Position.Y - CurrentPos.Y);
                if (Distance <= new Gun( ).Range & Distance != 0 & OneUnit.Owner == InactivePlayer && Visible(OneUnit.Position.ToVector2( ) + new Vector2(0.5f, 0.5f), CurrentPos.ToVector2( ) + new Vector2(0.5f, 0.5f))) {
                    AddToFrameList(OneUnit.Position);
                }
            }
        }

        private bool Visible(Vector2 p, Vector2 q) {
            if (p.X - q.X == 0) {
                float min = 0;
                float max = 0;
                if (p.Y < q.Y) {
                    min = p.Y;
                    max = q.Y;
                } else {
                    min = q.Y;
                    max = p.Y;
                }
                for (float i = min; i < max; i++) {
                    if (this.Field[Convert.ToInt32(Math.Abs(p.X)), Convert.ToInt32(Math.Abs(i))].Type == 3) {
                        return false;
                    }
                }
            } else {
                //Schlechtestes RayTracing aller Zeiten :O
                float m = ((float)p.Y - (float)q.Y) / ((float)p.X - (float)q.X);
                float n = (float)p.Y - m * (float)p.X;
                float min = 0;
                float max = 0;
                if (p.X < q.X) {
                    min = p.X;
                    max = q.X;
                } else {
                    min = q.X;
                    max = p.X;
                }
                for (float i = min; i < max; i += 0.1f) {
                    float y = m * i + n;
                    if (this.Field[Convert.ToInt32(Math.Floor((double)i)), Convert.ToInt32(Math.Floor((double)y))].Type == 3) {
                        return false;
                    }
                }
            }
            return true;
        }

        private void EndTurn_OnClick(object sender, EventArgs e) {
            if (ActivePlayer == 0) {
                ActivePlayer = 1;
                InactivePlayer = 0;
            } else {
                ActivePlayer = 0;
                InactivePlayer = 1;
            }

            foreach (Unit OneUnit in this.AllUnits.Where(u => u.Owner == ActivePlayer)) {
                OneUnit.MovementLeft = OneUnit.MoveSpeed;
            }

            if (ActivePlayer == 1) {
                //Execute Turn for AI
                this.EndTurn_OnClick(sender, e);
            }
        }

        private void AddToFrameList(Point p) {
            if (!FramesAt.Contains(p)) {
                FramesAt.Add(p);
            }
        }

        private void LoadMap(string path) {
            bool there = File.Exists(path);
            StreamReader MyReader = new StreamReader(File.OpenRead(path));
            int Line = 0;
            while (!MyReader.EndOfStream) {
                string Content = MyReader.ReadLine( );
                for (int i = 0; i < Content.Length; i++) {
                    int Type = 0;
                    switch (Content[i]) {
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
