using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.GameObjects;
using MyGame.Misc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    internal static class Globals
    {
        #region constants
        public static readonly Vector2Int ViewPortSize = new(1920, 1080);
        public const int Zoom = 4; //1px in sprite is 4px on ingame
        public const int OriginalTileSize = 8;
        public static int TileSize { get { return Zoom * OriginalTileSize; } }
        public enum TileType { None = -1, Solid, SemiUp, SemiRight, SemiDown, SemiLeft, Hazard, Water }
        public enum State { Idling, Walking, Jumping, Crouching, MidAir, Dying, Dead, Attacking, Selected, Unselected }

        public static readonly Texture2D DebugImage;
        public const float MaxVerticalSpeed = 26f;
        #endregion

        #region methods
        //for collision boxes
        public static RectangleF At(this RectangleF a, Vector2 b)
        {
            return new RectangleF(a.X + b.X, a.Y + b.Y, a.Width, a.Height);
        }
        public static bool tryGetValue(this TileType[,] tileMap, Vector2Int coords, out TileType tileType)
        {
            try
            {
                tileType = tileMap[coords.X, coords.Y];
                return true;
            }
            catch
            {
                tileType = TileType.None;
                return false;
            }
        }

        public static Vector2 GetMiddleOfRect(RectangleF rect)
        {
            return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
        public static Vector2 GetMiddleOfRect(Microsoft.Xna.Framework.Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
        public static int mod(int x, int m) //because negative modulo in c# is wrong
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        public static Vector2 Normalized(this Vector2 v)
        {
            v.Normalize();
            return v;
        }
        #endregion

        //returns velocity
        public delegate Vector2 Touch(Entity collisionObject, Vector2 normalVector);
    }
}
