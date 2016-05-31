using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hack_a_peter.Scenes.Strategy
{
    public class Tile
    {
        private int _Cover;
        public int Cover
        {
            get { return _Cover; }
            set { _Cover = value; }
        }

        /// <summary>
        /// 0 = default
        /// 1 = half cover
        /// 2 = full cover
        /// 3 = unpassable
        /// </summary>

        private int _Type;
        public int Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        //private Unit _Unit;
        //public Unit Unit
        //{
        //    get { return _Unit; }
        //    set { _Unit = value; }
        //}
    }
}
