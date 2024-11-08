using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Globals;

namespace MyGame.Misc
{
    static class Methods
    {
        //TODO: move to globals

        public static RectangleF At(this RectangleF a, Vector2 b)
        {
            return new RectangleF(a.X + b.X, a.Y + b.Y, a.Width, a.Height);
        }

        public static bool tryGetValue(this TileType[,] tilemap, Vector2Int coords, out TileType tileType)
        {
            try
            {
                tileType = tilemap[coords.X, coords.Y];
                return true;
            }
            catch
            {
                tileType = TileType.None;
                return false;
            }
        }
    }
}
