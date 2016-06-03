using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hack_a_peter
{
    public static class Extensions
    {
        public static Vector2 XY(this Vector3 vec3)
        {
            return new Vector2(vec3.X, vec3.Y);
        }

        public static bool ContainsRange<T>(this HashSet<T> set, IEnumerable<T> collection)
        {
            foreach (T element in collection)
            {
                if (!set.Contains(element))
                    return false;
            }
            return true;
        }
    }
}
