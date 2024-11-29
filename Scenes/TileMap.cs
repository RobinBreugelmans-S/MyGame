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

        public int[,] map; //TODO: naming conventions
        public Texture2D tileset;
        public string tilesetName;
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
                        map[x, y] = layer.data2D[y][x]; //TODO: if has data2D or has entities
                    }
                }
            }
            // else {} //do entities
            tilesetName = layer.tileset;
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
            return new Rectangle(id * OriginalTileSize % tileset.Width,
                                 (int)Math.Floor((float)id * OriginalTileSize / tileset.Width) * OriginalTileSize,
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
                        Rectangle src = getTextureRect(map[x, y]); //TODO: remove -1 //try catch, so game doesn't crash if invalid number in map
                        spriteBatch.Draw(tileset, dest, src, Color.White);
                    }
                }
            }
        }
    }
}
