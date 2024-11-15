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
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace MyGame.Scenes
{
    internal class Scene
    {
        //TODO: 
        //factory ? to return Scene object which has all of these loaded, methods go into SceneManager
        
        //public TileType[,] tilemapCollisions { get; private set; } //TODO: naming conventions!
        public TileMap tilemapCollisions { get; private set; } //TODO: tileMapCol..
        public TileMap[] tilemapsDecoration { get; private set; } //array of 2d arrays //TODO: naming conventions!
        //public Texture2D[] tilesets { get; private set; }
        //public string[] tilesetNames { get; private set; }
        public List<IGameObject> entities { get; private set; } = new(); //TODO: public or private?
        private Texture2D background; //Background class with pos, paralax, Texture2D, Draw()
        //public List<Rectangle> textureStore { get; private set; }
        //public Texture2D textureAtlas { get; set; }

        public Scene(string level)
        {
            //mapObject = loadMap($".././../../Maps/{level}.json")
            loadScene($".././../../Maps/{level}.json");//($".././../../Maps/{level}.csv"); //

        }

        private void loadScene(string filePath)
        {
            //parse ogmo json            
            StreamReader reader = new(filePath);
            string jsonData = reader.ReadToEnd();

            LevelJson levelJson = JsonSerializer.Deserialize<LevelJson>(jsonData);

            //refacotr with tilemap class
            tilemapCollisions = new TileMap(levelJson.layers[0]);         //TODO: make it so tilemapsDeco doesn't get entity, than make list withb entity layers
            tilemapsDecoration = new TileMap[levelJson.layers.Count - 2]; //-2: first layer is collisios, last is entities //TODO: add checks for if there are not enough layers
            for (int i = 0; i < levelJson.layers.Count - 2; i++)
            {
                tilemapsDecoration[i] = new TileMap(levelJson.layers[i + 1]); // i+1 cause layer 0 is collisions
            }
        }

        

        public void Draw(SpriteBatch spriteBatch)
        {
            //tilemapCollisions.Draw(spriteBatch);
            foreach(TileMap tileMap in tilemapsDecoration)
            {
                tileMap.Draw(spriteBatch);
            }
            
            foreach (IGameObject entity in entities)
            {
                entity.Draw(spriteBatch);
            }
        }
    }
}
