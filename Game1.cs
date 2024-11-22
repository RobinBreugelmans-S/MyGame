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
using MyGame.Scenes;


namespace MyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Player player; //TODO remove
        private Texture2D playerTexture;

        private Scene scene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //_graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            IsFixedTimeStep = true; //60fps
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            scene = new Scene("TestLevel1", Content); //TODO: refactor with scene factory

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("PlayerSpriteSheet"); //TODO: naming conventions!!
            

            //TODO: remove?
            DebugImage = Content.Load<Texture2D>("DebugImage");

            InitializeGameObjects();
        }

        private void InitializeGameObjects()
        {
            //TODO: factory for entities (so don't have to give playerTexture, keyboardReader, tileMapCollisions)
            //TODO: move to scene
            scene.LoadScene();
            for (int i = 0; i < scene.tileMapsDecoration.Length; i++) //load tileset images from tileset names
            {
                scene.tileMapsDecoration[i].tileset = Content.Load<Texture2D>(scene.tileMapsDecoration[i].tilesetName);
            }

            //player = new Player(new Vector2(0,0), playerTexture, new KeyboardReader(), scene.tileMapCollisions.AsTileTypeMap(), scene.entities); //change input in settings
                                                                                                                                                 //currently collisions and tileMap are the same
            //scene.Player = player;

            //erik = new Erik(new Vector2(18 * TileSize,17 * TileSize), erikTexture, player, scene.tileMapCollisions.AsTileTypeMap(), scene.entities);
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            { 
                Exit();
            }

            // TODO: Add your update logic here
            scene.Update();
            //TODO: move to scene
            //player.Update();
            //erik.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            scene.Draw(_spriteBatch);
            //TODO: V
            //_spriteBatch.Draw(textureAtlas, new Rectangle((int)player.currentCollisionBox.X, (int)player.currentCollisionBox.Y, (int)player.currentCollisionBox.Width, (int)player.currentCollisionBox.Height), textureStore[0], Color.Red);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
