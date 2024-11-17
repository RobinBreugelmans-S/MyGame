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
using Microsoft.Xna.Framework.Content;


namespace MyGame.Scenes
{
    internal class Scene
    {
        private string filePath;
        private ContentManager content;
        //TODO: 
        //factory ? to return Scene object which has all of these loaded, methods go into SceneManager
        
        //public TileType[,] tileMapCollisions { get; private set; } //TODO: naming conventions!
        public TileMap tileMapCollisions { get; private set; } //TODO: tileMapCol..
        public TileMap[] tileMapsDecoration { get; private set; } //array of 2d arrays //TODO: naming conventions!
        //public Texture2D[] tilesets { get; private set; }
        //public string[] tilesetNames { get; private set; }
        //public List<IGameObject> entities { get; private set; } = new(); //TODO: public or private?
        public List<StationaryObject> entities { get; private set; } = new(); //TODO naming convetions
        private Texture2D background; //Background class with pos, paralax, Texture2D, Draw()
        
        public Scene(string level, ContentManager content)
        {
            filePath = $".././../../Maps/{level}.json";
            this.content = content;
        }

        public void LoadScene()
        {
            ObjectFactory objectFactory = new(content, new Action<StationaryObject>(entity => entities.Remove(entity)));

            //parse ogmo json            
            StreamReader reader = new(filePath);
            string jsonData = reader.ReadToEnd();

            LevelJson levelJson = JsonSerializer.Deserialize<LevelJson>(jsonData);

            //refacotr with tileMap class
            tileMapCollisions = new TileMap(levelJson.layers[0]);         //TODO: make it so tileMapsDeco doesn't get entity, than make list withb entity layers
            tileMapsDecoration = new TileMap[levelJson.layers.Count - 2]; //-2: first layer is collisios, last is entities //TODO: add checks for if there are not enough layers
            for (int i = 0; i < levelJson.layers.Count - 2; i++)
            {
                tileMapsDecoration[i] = new TileMap(levelJson.layers[i + 1]); // i+1 cause layer 0 is collisions
            }
            //load entities
            //foreach over the list, big case switch in seperate method, add into collision entity if the entity is collidable?
            entities.Add(objectFactory.CreateCoin(5, 12));
            entities.Add(objectFactory.CreateCoin(7, 12));
            entities.Add(objectFactory.CreateCoin(9, 12));
            entities.Add(objectFactory.CreateRedCoin(8, 16));
        }

        public void Update()
        {
            foreach(IGameObject entity in entities)
            {
                entity.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //tileMapCollisions.Draw(spriteBatch);
            foreach(TileMap tileMap in tileMapsDecoration)
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
