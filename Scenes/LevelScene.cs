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
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using System.Linq;


namespace MyGame.Scenes
{
    internal class LevelScene : IScene
    {
        private SpriteFont font;

        //TODO: naming conventions
        private Player player;
        private Camera camera;
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
        private ObjectFactory objectFactory;
        //TODO: has to be public???
        public List<StationaryObject> entities = new(); //TODO naming convetions
        public List<StationaryObject> collidableEntities = new();
        public List<StationaryObject> entitiesToBeRemoved = new();
        public List<(StationaryObject entity, int timer)> entitiesToBeRemovedTimer = new();
        private Texture2D background; //Background class with pos, paralax, Texture2D, Draw()
        
        public LevelScene(string level, ContentManager content)
        {
            filePath = $".././../../Maps/{level}.json";
            this.content = content;
        }

        public void LoadScene()
        {
            //TODO refactor
            font = content.Load<SpriteFont>("PixelFont");
            
            //parse ogmo json            
            StreamReader reader = new(filePath);
            string jsonData = reader.ReadToEnd();

            LevelJson levelJson = JsonSerializer.Deserialize<LevelJson>(jsonData);

            //refactor with tileMap class
            tileMapCollisions = new TileMap(levelJson.layers[0]);         //TODO: make it so tileMapsDeco doesn't get entity, than make list withb entity layers
            tileMapsDecoration = new TileMap[levelJson.layers.Count - 2]; //-2: first layer is collisios, last is entities //TODO: add checks for if there are not enough layers
            for (int i = 0; i < levelJson.layers.Count - 2; i++)
            {
                tileMapsDecoration[i] = new TileMap(levelJson.layers[i + 1]); // i+1 cause layer 0 is collisions
            }
            for (int i = 0; i < tileMapsDecoration.Length; i++) //load tileset images from tileset names
            {
                tileMapsDecoration[i].tileset = content.Load<Texture2D>(tileMapsDecoration[i].tilesetName);
            }
            //load entities
            objectFactory = new(player, tileMapCollisions.AsTileTypeMap(), collidableEntities, content, 
                new Action<StationaryObject, int>((entity, timer) => entitiesToBeRemovedTimer.Add((entity, timer)))
            );
                                                                                                        //TODO: add way to remove entity in some time
            
            Layer entityLayer = levelJson.layers[levelJson.layers.Count - 1];
            foreach(EntityData entityData in entityLayer.entities)
            {
                addEntity(entityData.name, entityData.x, entityData.y);
            }
        }
        private void addEntity(string entityName, int tileX, int tileY)
        {
            StationaryObject entity = objectFactory.GetEntity(entityName, tileX, tileY);

            if (entity is Player)
            {
                Player _player = (Player)entity; //add check for adding 2 players
                objectFactory.player = _player;
                player = _player;
                camera = new(player, new(tileMapCollisions.Width, tileMapCollisions.Height));
            }
            entities.Add(entity);
            if (entity.Touched != null) //TODO: Touched -> OnTouch //TODO: use isCollidable
                //TODO: also make it so if you can stand on entity
            {
                collidableEntities.Add(entity);
            }
        }
        private void  removeEntity(StationaryObject entity)
        {
            entities.Remove(entity); //TODO: what if entity is not collidable?
            collidableEntities.Remove(entity);
        }

        public void Update()
        {
            foreach(IGameObject entity in entities)
            {
                entity.Update();
            }
            camera.Update();
            
            Debug.WriteLine("--");
            for (int i = 0; i < entitiesToBeRemovedTimer.Count; i++)
            {
                Debug.WriteLine(entitiesToBeRemovedTimer[i]);
                //decrement timer by 1
                entitiesToBeRemovedTimer[i] = (entitiesToBeRemovedTimer[i].entity, entitiesToBeRemovedTimer[i].timer - 1);
            }
            foreach (var item in entitiesToBeRemovedTimer)
            {
                Debug.WriteLine(item);
                if(item.timer <= 0)
                {
                    Debug.WriteLine("DELETED!!");
                    removeEntity(item.entity);
                }
            }
            entitiesToBeRemovedTimer = entitiesToBeRemovedTimer.Where(i => i.timer > 0).ToList();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 offset = -camera.Pos;
            foreach (TileMap tileMap in tileMapsDecoration)
            {
                tileMap.Draw(offset, spriteBatch);
            }
            
            foreach (IGameObject entity in entities)
            {
                entity.Draw(offset, spriteBatch);
            }

            if(player != null) //TODO: put in player class or scene? put in camera!
            {
                //HP
                Vector2 textMiddlePoint = Vector2.Zero;
                Vector2 position = new Vector2(16, 0); //TODO: use viewport
                spriteBatch.DrawString(font, $"HP: {player.HP}", position, Color.Black, 0, textMiddlePoint, 4f, SpriteEffects.None, 0.5f);
                
                //TODO
                //Death screen
                if (player.HP <= 0)
                {
                    spriteBatch.DrawString(font, "epic fail", new(BufferSize.X / 2, BufferSize.Y / 2), Color.Black, 0, font.MeasureString("epic fail")/2, 12f, SpriteEffects.None, 0.5f);
                }

                //Score
                string scoreText = $"Score: {player.Score.ToString("D8")}";
                spriteBatch.DrawString(font, scoreText, new Vector2(BufferSize.X- font.MeasureString(scoreText).X * 2 - 16, 32), Color.Black, 0, font.MeasureString(scoreText) / 2, 4f, SpriteEffects.None, 0.5f);
            }
        }

    }
}
