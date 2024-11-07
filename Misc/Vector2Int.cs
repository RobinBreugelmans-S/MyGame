using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Misc
{
    internal struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }
        /* TODO
        public static Vector2Int Parse(Vector2 v)
        {
            return new Vector2Int((int)v.X, (int)v.Y);
        }*/
    }
}
