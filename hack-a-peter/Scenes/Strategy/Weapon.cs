using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hack_a_peter.Scenes.Strategy
{
    public class Weapon
    {
        public void Apply(Tile[,] map)
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

    }
}
