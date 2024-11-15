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
using System.Diagnostics;

namespace MyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D playerTexture;
        private Player player;

        private Scene scene;
        private SceneManager sceneManager = new();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //_graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            this.IsFixedTimeStep = true; //60fps
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

            scene = new Scene("TestLevel1");
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("catSpriteSheetFixed"); //TODO: naming conventions!!
            for (int i = 0; i < scene.tilemapsDecoration.Length; i++) //load tileset images from tileset names
            {
                scene.tilemapsDecoration[i].tileset = Content.Load<Texture2D>(scene.tilemapsDecoration[i].tilesetName);
            }
            
            InitializeGameObjects();
        }

        private void InitializeGameObjects()
        {
            //TODO: factory for entities (so don't have to give playerTexture, keyboardReader, tilemapCollisions)
            player = new Player(new Vector2(0,0), playerTexture, new KeyboardReader(), scene.tilemapCollisions.AsTileTypeMap()); //change input in settings
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
            scene.Draw(_spriteBatch);
            player.Draw(_spriteBatch); //will be in scene when player is in entities list
            //_spriteBatch.Draw(textureAtlas, new Rectangle((int)player.currentCollisionBox.X, (int)player.currentCollisionBox.Y, (int)player.currentCollisionBox.Width, (int)player.currentCollisionBox.Height), textureStore[0], Color.Red);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
