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
        
        public static Vector2Int operator *(Vector2Int vector, int num)
        {
            vector.X *= num;
            vector.Y *= num;
            return vector;
        }
        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            v1.X += v2.X;
            v1.Y += v2.Y;
            return v1;
        }
    }
}
