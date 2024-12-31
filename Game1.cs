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

        private SceneFactory sceneFactory;
        private IScene currentScene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            //_graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = ViewPortSize.X;
            _graphics.PreferredBackBufferHeight = ViewPortSize.Y;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60fps

            sceneFactory = new(Content,
                (string sceneName) => { currentScene = sceneFactory.GetScene(sceneName); currentScene.LoadScene(); },
                Exit
            );
        }

        protected override void Initialize()
        {
            currentScene = sceneFactory.GetScene("main_menu");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitializeGameObjects();
        }

        private void InitializeGameObjects()
        {
            currentScene.LoadScene();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            { 
                Exit();
            }

            currentScene.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            currentScene.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
