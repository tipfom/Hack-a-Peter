using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hack_a_peter.Scenes.Strategy
{
    public class Weapon
    {
        public void Apply(Tile[,] map, int x, int y)
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
            Count = 3;
            MaxCount = 3;
        }
    }
}
