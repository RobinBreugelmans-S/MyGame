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

/* TODO:
 * comments where i used a coding structure thing (factory, command, any more used?)
 * 
 * https://anokolisa.itch.io/action
 */

namespace MyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private IScene scene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //_graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = BufferSize.X;
            _graphics.PreferredBackBufferHeight = BufferSize.Y;
            IsFixedTimeStep = true; //60fps
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            scene = new LevelScene("TestLevel1", Content); //TODO: refactor with scene factory/manager

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: remove?
            //DebugImage = Content.Load<Texture2D>("DebugImage");

            InitializeGameObjects();
        }

        private void InitializeGameObjects()
        {
            scene.LoadScene();
            //TODO: move to scene
            /*for (int i = 0; i < scene.tileMapsDecoration.Length; i++) //load tileset images from tileset names
            {
                scene.tileMapsDecoration[i].tileset = Content.Load<Texture2D>(scene.tileMapsDecoration[i].tilesetName);
            }*/
            //currently collisions and tileMap are the same
            //scene.Player = player;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            { 
                Exit();
            }

            scene.Update();
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
