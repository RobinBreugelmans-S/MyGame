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
using static System.Net.Mime.MediaTypeNames;


namespace MyGame.Scenes
{
    internal class Scene
    {
        SpriteFont font;

        //TODO: naming conventions
        private Player player;
        //public Player Player { get { return player; } set { player = value; objectFactory.player = value; } }
        private string filePath;
        private ContentManager content;
        //TODO: 
        //factory ? to return Scene object which has all of these loaded, methods go into SceneManager
        
        //public TileType[,] tileMapCollisions { get; private set; } //TODO: naming conventions!
        public TileMap tileMapCollisions { get; private set; } //TODO: is get and private set needed?
        public TileMap[] tileMapsDecoration { get; private set; } //array of 2d arrays //TODO: naming conventions!
        //public Texture2D[] tilesets { get; private set; }
        //public string[] tilesetNames { get; private set; }
        //public List<IGameObject> entities { get; private set; } = new(); //TODO: public or private?
        private ObjectFactory objectFactory;
        public List<StationaryObject> entities = new(); //TODO naming convetions
        public List<StationaryObject> collidableEntities = new();
        public List<StationaryObject> entitiesToBeRemoved = new();
        private Texture2D background; //Background class with pos, paralax, Texture2D, Draw()
        
        public Scene(string level, ContentManager content)
        {
            font = content.Load<SpriteFont>("PixelFont");

            filePath = $".././../../Maps/{level}.json";
            this.content = content;
        }

        private void addEntity(StationaryObject entity)
        {   if(entity is Player)
            {
                Player _player = (Player)entity; //add check for adding 2 players
                objectFactory.player = _player;
                player = _player;
            }
            entities.Add(entity);
            if (entity.Touched != null) //TODO: Touched -> OnTouch //TODO: use isCollidable
            {
                collidableEntities.Add(entity);
            }
        }

        public void LoadScene()
        {
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
            objectFactory = new(player, tileMapCollisions.AsTileTypeMap(), collidableEntities, content, new Action<StationaryObject>(entity => entitiesToBeRemoved.Add(entity)));
            //foreach over the list, big case switch in seperate method, add into collision entity if the entity is collidable?
            addEntity(objectFactory.GetPlayer(0, 6));
            addEntity(objectFactory.GetCoin(5, 12));
            addEntity(objectFactory.GetCoin(7, 12));
            addEntity(objectFactory.GetCoin(9, 12));
            addEntity(objectFactory.GetRedCoin(8, 16));
            addEntity(objectFactory.GetErik(0,12));
        }

        private void  removeEntity(StationaryObject entity)
        {
            entities.Remove(entity);
            collidableEntities.Remove(entity);
        }

        public void Update()
        {
            foreach(IGameObject entity in entities)
            {
                entity.Update();
            }
            foreach(StationaryObject entity in entitiesToBeRemoved)
            {
                removeEntity(entity);
            }
            entitiesToBeRemoved = new();
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

            if(player != null) //TODO: put in player class or scene?
            {
                //HP
                Vector2 textMiddlePoint = Vector2.Zero;
                Vector2 position = new Vector2(16, 0); //TODO: use viewport
                spriteBatch.DrawString(font, $"HP: {player.HP}", position, Color.Black, 0, textMiddlePoint, 4f, SpriteEffects.None, 0.5f);
                
                //Death screen
                if (player.HP <= 0)
                {
                    spriteBatch.DrawString(font, "epic fail", new(1920/2,1080/2), Color.Black, 0, font.MeasureString("epic fail")/2, 12f, SpriteEffects.None, 0.5f);
                }

                //Score
                string scoreText = $"Score: {player.Score.ToString("D8")}";
                //scoreText = "Score: 00001234";
                spriteBatch.DrawString(font, scoreText, new Vector2(1920 - font.MeasureString(scoreText).X * 2 - 16, 32) , Color.Black, 0, font.MeasureString(scoreText) / 2, 4f, SpriteEffects.None, 0.5f);

            }
        }

    }
}
