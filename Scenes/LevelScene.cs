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
using System.Runtime.InteropServices;
using MyGame.GameObjects.LevelObjects;


namespace MyGame.Scenes
{
    internal class LevelScene : IScene
    {
        private SpriteFont font;

        //TODO: naming conventions
        private Player player;
        private Camera camera;
        private Entity finish;
        //public Player Player { get { return player; } set { player = value; objectFactory.player = value; } }
        private string filePath;
        private ContentManager content;
        private Texture2D blank;
        //TODO: 
        //factory ? to return Scene object which has all of these loaded, methods go into SceneManager

        //public TileType[,] tileMapCollisions { get; private set; } //TODO: naming conventions!
        public TileMap tileMapCollisions { get; private set; } //TODO: is get and private set needed?
        public TileMap[] tileMapsDecoration { get; private set; } //array of 2d arrays //TODO: naming conventions!
        //public Texture2D[] tilesets { get; private set; }
        //public string[] tilesetNames { get; private set; }
        private EntityFactory objectFactory;
        private List<Entity> entities = new(); //TODO naming convetions
        private List<Entity> collidableEntities = new();
        private List<Entity> entitiesToBeRemoved = new();
        private List<Entity> entitiesToBeAdded = new();
        private List<(Entity entity, int timer)> entitiesToBeRemovedTimer = new();

        private string backgroundName;
        private float[] backgroundParalaxStrengths;
        private ParalaxBackground[] backgroundLayers; //Background class with pos, paralax, Texture2D, Draw()

        private string nextLevel;
        private Action<string> loadScene;

        public LevelScene(string level, string backgroundName, float[] paralaxStrengths, string nextLevel, ContentManager content, Action<string> loadScene)
        {
            filePath = $".././../../Maps/{level}.json";
            this.backgroundName = backgroundName;
            backgroundParalaxStrengths = paralaxStrengths;
            this.content = content;
            this.loadScene = loadScene;
            this.nextLevel = nextLevel;
        }

        public void LoadScene()
        {
            blank = content.Load<Texture2D>("blank");

            //get backgrounds
            backgroundLayers = new ParalaxBackground[backgroundParalaxStrengths.Length];

            for (int i = 0; i < backgroundParalaxStrengths.Length; i++)
            {
                Texture2D texture = content.Load<Texture2D>($"{backgroundName}/{i + 1}");
                backgroundLayers[i] = new(texture, backgroundParalaxStrengths[i]);
            }

            font = content.Load<SpriteFont>("PixelFont");
            
            //parse json data from ogmo          
            StreamReader reader = new(filePath);
            string jsonData = reader.ReadToEnd();

            LevelJson levelJson = JsonSerializer.Deserialize<LevelJson>(jsonData);

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
                new Action<Entity, int>((entity, timer) => entitiesToBeRemovedTimer.Add((entity, timer))),
                new Action<Entity>((entity) => addEntity(entity)),
                new Action<string>(sceneName => loadScene(sceneName))
            );
            
            Layer entityLayer = levelJson.layers[levelJson.layers.Count - 1];
            foreach(EntityData entityData in entityLayer.entities)
            {
                addEntity(entityData.name, entityData.x * Zoom, entityData.y * Zoom);
            }
        }
        private void addEntity(string entityName, int x, int y)
        {
            Entity entity = objectFactory.GetEntity(entityName, x, y);

            if (entity is Player)
            {
                Player _player = (Player)entity;
                objectFactory.player = _player;
                player = _player;
                camera = new(player, new(tileMapCollisions.Width * TileSize, tileMapCollisions.Height * TileSize));
                player.LoadScene = loadScene;
            }
            else if(entity is Finish)
            {
                Finish finish = (Finish)entity;
                finish.NextLevel = nextLevel;
            }
            entities.Add(entity);
            if (entity.OnTouch != null) //TODO: Touched -> OnTouch //TODO: use isCollidable
                                        //TODO: also make it so you can stand on entities
            {
                collidableEntities.Add(entity);
            }
        }
        private void addEntity(Entity entity)
        {
            entitiesToBeAdded.Add(entity);
        }
        private void  removeEntity(Entity entity)
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
            
            for (int i = 0; i < entitiesToBeRemovedTimer.Count; i++)
            {
                //decrement timer by 1
                entitiesToBeRemovedTimer[i] = (entitiesToBeRemovedTimer[i].entity, entitiesToBeRemovedTimer[i].timer - 1);
            }
            foreach (var item in entitiesToBeRemovedTimer)
            {
                if (item.timer <= 0)
                {
                    removeEntity(item.entity);
                }
            }
            entitiesToBeRemovedTimer = entitiesToBeRemovedTimer.Where(i => i.timer > 0).ToList();
            
            foreach(Entity entity in entitiesToBeAdded)
            {
                entities.Add(entity);
                if (entity.OnTouch != null)
                {
                    collidableEntities.Add(entity);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 offset = -camera.Pos;

            foreach(ParalaxBackground background in backgroundLayers)
            {
                background.Draw(camera.Pos, spriteBatch);
            }

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
                    spriteBatch.Draw(blank, new Rectangle(0, 0, ViewPortSize.X, ViewPortSize.Y), Color.Black);
                    spriteBatch.DrawString(font, "epic fail", new(ViewPortSize.X / 2, ViewPortSize.Y / 2), Color.White, 0, font.MeasureString("epic fail")/2, 12f, SpriteEffects.None, 0.5f);
                }

                //Score
                string scoreText = $"Score: {player.Score.ToString("D8")}";
                spriteBatch.DrawString(font, scoreText, new Vector2(ViewPortSize.X- font.MeasureString(scoreText).X * 2 - 16, 32), Color.Black, 0, font.MeasureString(scoreText) / 2, 4f, SpriteEffects.None, 0.5f);
            }
        }

    }
}
