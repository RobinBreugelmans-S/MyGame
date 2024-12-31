using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameObjects.MenuObjects;
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
        Texture2D buttonSpriteSheet;
        public SceneFactory(ContentManager content, Action<string> loadScene, Action exit)
        {
            this.content = content;
            this.loadScene = loadScene;
            this.exit = exit;

            getSceneMethods.Add("main_menu", getMainMenuScene);
            getSceneMethods.Add("level_001", getLevel1Scene);
            getSceneMethods.Add("level_002", getLevel2Scene);
            getSceneMethods.Add("win", getWinScene);

            //error: content is null, but content isn't actually null??
            //moved to loadIfNeeded() for now
            //font = content.Load<SpriteFont>("PixelFont");
            //buttonSpriteSheet = content.Load<Texture2D>("ButtonSpriteSheet");
        }

        private void loadIfNeeded()
        {
            if(font == null)
            {
                font = content.Load<SpriteFont>("PixelFont");
            }
            if(buttonSpriteSheet == null)
            {
                buttonSpriteSheet = content.Load<Texture2D>("ButtonSpriteSheet");
            }
        }

        public IScene GetScene(string sceneName)
        {
            return getSceneMethods[sceneName]();
        }

        private IScene getMainMenuScene()
        {
            loadIfNeeded();
            Button[] buttons = {                //TODO: should be 900 ??
                new("Start", () => loadScene("level_001"), new(730,320), buttonSpriteSheet, font), //TODO: text first, then action, then rest
                new("Quit", exit, new(730,520), buttonSpriteSheet, font)
            }; 
            MenuScene mainMenuScene = new("MainMenuBG", buttons, content);

            return mainMenuScene;
        }

        private IScene getWinScene()
        {
            Button[] buttons = {                //TODO: should be 900 ??
                new("Play Again", () => loadScene("level_001"), new(730,320), buttonSpriteSheet, font), //TODO: text first, then action, then rest
                new("Main Menu", () => loadScene("main_menu"), new(730,520), buttonSpriteSheet, font),
                new("Quit", exit, new(730,720), buttonSpriteSheet, font)
            };
            MenuScene mainMenuScene = new("WinBG", buttons, content);

            return mainMenuScene;
        }
        
        private LevelScene getLevelScene(string level, string backgroundName, float[] paralaxStrengths, string nextLevel)
        {
            return new LevelScene(level, backgroundName, paralaxStrengths, nextLevel, content, loadScene);
        }

        private IScene getLevel1Scene()
        {
            return getLevelScene("level_001", "CloudsBG", new[] { 0f, -.05f, -.1f}, "level_002"); //loading in level data is in LevelScene.cs
        }

        private IScene getLevel2Scene()
        {
            return getLevelScene("level_002", "DesertBG", new[] { 0f, -.05f, -.1f, -.15f, -.2f }, "win");
        }

    }
}
