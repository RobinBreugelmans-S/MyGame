using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.GameObjects;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using static MyGame.Globals;
using MyGame.Interfaces;
using static System.Net.WebRequestMethods;


namespace MyGame.Scenes
{
    internal class Scene
    {
        //TODO: 
        //factory ? to return Scene object which has all of these loaded, methods go into SceneManager
        
        public TileType[,] tilemapForeground { get; private set; } //TODO: naming conventions!
        public TileType[,] tilemapCollisions { get; private set; }
        public List<IGameObject> entities { get; private set; } = new(); //TODO: public or private?
        private Texture2D background; //Background class with pos, paralax, Texture2D, Draw()
        //public List<Rectangle> textureStore { get; private set; }
        public Texture2D textureAtlas { get; set; }
        private string filePath;

        public Scene(string level)
        {
            //mapObject = loadMap($".././../../Maps/{level}.json")
            tilemapCollisions = loadMap($".././../../Maps/{level}.csv");
            
        }

        private TileType[,] loadMap(string filePath)
        {
            TileType[,] tilemap = new TileType[50,50]; //temp

            //parse ogmo json
            /*
             
            
            tileMapCollisions = new TileType[.gridCellsX,.gridCellsY];
            */

            StreamReader reader = new(filePath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        tilemap[x, y] = (TileType)value;
                    }
                }
                y++;
            }

            reader.Close();
            
            return tilemap;
        }

        public Rectangle getTextureRect(int id)
        {
            return new Rectangle(id * OriginalTileSize % textureAtlas.Width, (int)Math.Floor((float)id * OriginalTileSize / textureAtlas.Width) * OriginalTileSize, OriginalTileSize, OriginalTileSize);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < tilemapCollisions.GetLength(0); x++)
            {
                for (int y = 0; y < tilemapCollisions.GetLength(1); y++)
                {
                    if(tilemapCollisions[x, y] != TileType.None)
                    {
                        Rectangle dest = new(x * TileSize, y * TileSize, TileSize, TileSize);
                        Rectangle src = getTextureRect((int)tilemapCollisions[x,y] - 1); //TODO: remove -1 //try catch, so game doesn't crash if invalid number in map
                        spriteBatch.Draw(textureAtlas, dest, src, Color.White);
                    }
                }
            }/*
            foreach (var tile in tilemapCollisions) // change to tilemapForeground
            {
                Rectangle dest = new(tile.Key.X * TileSize, tile.Key.Y * TileSize, TileSize, TileSize);
                Rectangle src = getTextureRect((int)tile.Value - 1); //TODO: remove -1 //try catch, so game doesn't crash if invalid number in map
                spriteBatch.Draw(textureAtlas, dest, src, Color.White);
            }*/
            foreach(IGameObject entity in entities)
            {
                entity.Draw(spriteBatch);
            }
        }
    }
}
