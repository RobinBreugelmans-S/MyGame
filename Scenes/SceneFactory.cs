using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Scenes
{
    internal class SceneFactory
    {
        private ContentManager content;
        private Action<string> loadScene;
        private Action exit;
        private Dictionary<string, Func<IScene>> getSceneMethods = new();
        private Vector2Int standardButtonSize = new(260, 130);
        SpriteFont font;
        public SceneFactory(ContentManager content, Action<string> loadScene, Action exit)
        {
            this.content = content;
            this.loadScene = loadScene;
            this.exit = exit;
            
            getSceneMethods.Add("main_menu", getMainMenuScene);
            getSceneMethods.Add("level1", getLevel1Scene);
        }

        public IScene GetScene(string sceneName)
        {
            return getSceneMethods[sceneName]();
        }

        private IScene getMainMenuScene()
        {
            //TODO: move this
            font = content.Load<SpriteFont>("PixelFont");
            var buttonSpriteSheet = content.Load<Texture2D>("ButtonSpriteSheet");
            //TODO: fix the level loaidng!!
            Button[] buttons = {//TODO: make getBuytton  //TODO: should be 900 ??
                new(() => loadScene("level1"), new(730,320), buttonSpriteSheet, font, "Start"), //TODO: text first, then action, then rest
                new(() => loadScene("level1"), new(730,520), buttonSpriteSheet, font, "button"),
                new(exit, new(730,720), buttonSpriteSheet, font, "Quit")
            }; 
            MenuScene mainMenuScene = new("MainMenuBG", buttons, content);

            return mainMenuScene;
        }

        private IScene getLevel1Scene()
        {//TestLevel1   level_001
            return new LevelScene("level_001", "CloudsBG", new[] { 0f, -.05f, -.1f}, content); //loading in level data is in LevelScene.cs
        }
    }
}
