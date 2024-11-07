using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Drawing;
using MyGame.GameObjects;
using MyGame.Misc;
using System.Collections.Generic;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using static MyGame.Globals;

namespace MyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _playerTexture;
        private Player player;

        private Dictionary<Vector2Int, int> tilemap;
        //private Dictionary<Vector2Int, int> tilemapCollisions;
        private List<Rectangle> textureStore;
        private Texture2D textureAtlas;
        
        private Dictionary<Vector2Int, int> loadMap(string filePath)
        {
            Dictionary<Vector2Int, int> map = new();

            StreamReader reader = new(filePath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value)) {
                        if (value > 0)
                        {
                            map[new Vector2Int(x, y)] = value;
                        }
                    }
                }
                y++;
            }

            reader.Close();
            return map;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            this.IsFixedTimeStep = true; //60fps
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

            //load testmap
            tilemap = loadMap("../../../Maps/TestMap.csv");
            textureStore = new() //TODO: make this automatic
            {
                new Rectangle(0,0,16,16), //texture 0 //TODO: 16 -> OriginalTileSize ??
                new Rectangle(16,0,16,16),
                new Rectangle(32,0,16,16)
            };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _playerTexture = Content.Load<Texture2D>("catSpriteSheetFixed"); //TODO: naming conventions!!
            textureAtlas = Content.Load<Texture2D>("TilesTest");

            InitializeGameObjects();
        }

        private void InitializeGameObjects()
        {
            player = new Player(new Vector2(0,0), _playerTexture, new KeyboardReader(), tilemap); //change input in settings
                                                                                      //currently collisions and tilemap are the same
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            { 
                Exit();
            }

            // TODO: Add your update logic here
            player.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var item in tilemap)
            {
                Rectangle dest = new(item.Key.X * TileSize, item.Key.Y * TileSize, TileSize, TileSize); //refactor 64 
                Rectangle src = textureStore[item.Value - 1]; //try catch, so game doesn't crash if invalid number in map
                _spriteBatch.Draw(textureAtlas, dest, src, Color.White);
            }
            player.Draw(_spriteBatch);
            //_spriteBatch.Draw(textureAtlas, new Rectangle((int)player.currentCollisionBox.X, (int)player.currentCollisionBox.Y, (int)player.currentCollisionBox.Width, (int)player.currentCollisionBox.Height), textureStore[0], Color.Red);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
