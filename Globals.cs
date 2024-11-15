﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    internal static class Globals
    {
        public const int Zoom = 4; //1px in sprite is 4px on ingame
        public const int OriginalTileSize = 8;
        public static int TileSize { get { return Zoom * OriginalTileSize; } }
        public enum TileType { None = -1, Solid, SemiUp, SemiRight, SemiDown, SemiLeft, Hazard, Water }


    }
}
