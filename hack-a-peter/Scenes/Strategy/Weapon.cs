﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace hack_a_peter.Scenes.Strategy
{
    public class Weapon
    {
        public virtual void Apply(Tile[,] map, List<Unit> units, int x, int y)
        {

        }

        private int _Range;
        public int Range
        {
            get { return _Range; }
            set { _Range = value; }
        }

        private bool _Reloadable;
        public bool Reloadable
        {
            get { return _Reloadable; }
            set { _Reloadable = value; }
        }

        private int _Count;
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        private int _MaxCount;
        public int MaxCount
        {
            get { return _MaxCount; }
            set { _MaxCount = value; }
        }
    }
    public class Gun : Weapon
    {
        public Gun()
        {
            Range = 5;
            Reloadable = true;
            Count = 1;
            MaxCount = 1;
        }

        public override void Apply(Tile[,] map, List<Unit> units, int x, int y)
        {
            int BaseDamage = 1;
            Random Rng = new Random();
            switch (map[x,y].Type)
            {
                case 0:
                    BaseDamage += Rng.Next(1, 4);
                    break;
                case 1:
                    BaseDamage += Rng.Next(1, 3);
                    break;
                case 2:
                    BaseDamage += Rng.Next(0, 2);
                    break;
                default:
                    break;
            }
            Unit Damaged = units.First(u => u.Position == new Microsoft.Xna.Framework.Point(x, y));
            Damaged.Health -= BaseDamage;
        }
    }
    public class Grenade : Weapon
    {
        public Grenade()
        {
            Range = 5;
            Reloadable = false;
            Count = 1;
            MaxCount = 1;
        }

        public override void Apply(Tile[,] map, List<Unit> units, int x, int y)
        {
            Point[] Affected = new Point[5];
            Affected[0] = new Point(x, y);
            Affected[1] = new Point(x + 1, y);
            Affected[2] = new Point(x - 1, y);
            Affected[3] = new Point(x, y + 1);
            Affected[4] = new Point(x, y - 1);

            foreach (Point OnePoint in Affected)
            {
                if (map[OnePoint.X, OnePoint.Y].Type > 0)
                {
                    map[OnePoint.X, OnePoint.Y].Type -= 1;
                }
                if(units.Exists(u => u.Position == OnePoint))
                {
                    units.First(u => u.Position == OnePoint).Health -= 2;
                }
            }
        }
    }
}
