using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Globals;

namespace MyGame.Scenes
{
    internal class TileMap
    {

        private int[,] map;
        public Texture2D Tileset;
        public string TilesetName;
        public int Width { get { return map.GetLength(0); } }
        public int Height { get { return map.GetLength(1); } }

        public TileMap(Layer layer)
        {
            if(layer.data2D != null)
            {
                map = new int[layer.gridCellsX, layer.gridCellsY];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        map[x, y] = layer.data2D[y][x];
                    }
                }
            }
            
            TilesetName = layer.tileset;
        }
        
        public TileType[,] AsTileTypeMap() //for collisions
        {
            TileType[,] tileMap = new TileType[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tileMap[x, y] = (TileType)map[x,y];
                }
            }

            return tileMap;
        }
        public Rectangle getTextureRect(int id)
        {
            return new Rectangle(id * OriginalTileSize % Tileset.Width,
                                 (int)Math.Floor((float)id * OriginalTileSize / Tileset.Width) * OriginalTileSize,
                                 OriginalTileSize,
                                 OriginalTileSize);
        }
        
        public void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[x, y] != -1)
                    {                                       //TODO: maybe do Math.Round()
                        Rectangle dest = new(x * TileSize + (int)offset.X, y * TileSize + (int)offset.Y, TileSize, TileSize);
                        Rectangle src = getTextureRect(map[x, y]); //TODO: try catch, so game doesn't crash if invalid number in map
                        spriteBatch.Draw(Tileset, dest, src, Color.White);
                    }
                }
            }
        }
    }
}
