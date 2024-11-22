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
        public const int Zoom = 4; //1px in sprite is 4px on ingame
        public const int OriginalTileSize = 8;
        public static int TileSize { get { return Zoom * OriginalTileSize; } }
        public enum TileType { None = -1, Solid, SemiUp, SemiRight, SemiDown, SemiLeft, Hazard, Water }
        public enum State { Idling, Walking, Jumping, Crouching, MidAir, Dying, Attacking }

        public static Texture2D DebugImage;
        #endregion

        #region methods
        //for collision boxes
        public static RectangleF At(this RectangleF a, Vector2 b)
        {/*
            Debug.WriteLine("~~~~");
            Debug.WriteLine($"box: {a.Left}, {a.Top} : {a.Right}, {a.Bottom}");
            Debug.WriteLine($"vector: {b.X}, {b.Y}");
            Debug.WriteLine($"result: {a.X + b.X}, {a.Y + b.Y}, {a.Width}, {a.Height}");
            Debug.WriteLine("~~~~");*/
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
        #endregion

        public delegate void OnTouch(StationaryObject collisionObject, Vector2 normalVector);
    }
}
