using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameObjects.MenuObjects;
using MyGame.Input;
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
        private SpriteFont font;
        private Texture2D buttonSpriteSheet;

        private IInputReader inputReader;
        public SceneFactory(IInputReader inputReader, ContentManager content, Action<string> loadScene, Action exit)
        {
            this.inputReader = inputReader;
            this.content = content;
            this.loadScene = loadScene;
            this.exit = exit;

            getSceneMethods.Add("main_menu", getMainMenuScene);
            getSceneMethods.Add("level_001", getLevel1Scene);
            getSceneMethods.Add("level_002", getLevel2Scene);
            getSceneMethods.Add("win", getWinScene);

            //error: content is null, but content isn't actually null??
            //moved to loadContentIfNeeded() for now
            //font = content.Load<SpriteFont>("PixelFont");
            //buttonSpriteSheet = content.Load<Texture2D>("ButtonSpriteSheet");
        }

        private void loadContentIfNeeded()
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

        private MenuScene getMenuScene(string backgroundName, Button[] buttons)
        {
            return new(backgroundName, buttons, content, inputReader);
        }

        private Button getButton(string text, Action action, Vector2 pos)
        {
            return new(text, action, pos, buttonSpriteSheet, font);
        }

        private IScene getMainMenuScene()
        {
            loadContentIfNeeded();
            Button[] buttons = {                       //TODO: 730 should be 900 ??
                getButton("Start", () => loadScene("level_001"), new(730,320)), //TODO: text first, then action, then rest
                getButton("Quit", exit, new(730,520))
            }; 
            
            return getMenuScene("MainMenuBG", buttons);
        }

        private IScene getWinScene()
        {
            Button[] buttons = {
                getButton("Play Again", () => loadScene("level_001"), new(730,320)), //TODO: text first, then action, then rest
                getButton("Main Menu", () => loadScene("main_menu"), new(730,520)),
                getButton("Quit", exit, new(730,720))
            };

            return getMenuScene("WinBG", buttons);
        }

        
        private LevelScene getLevelScene(string level, string backgroundName, float[] paralaxStrengths, string nextLevel)
        {
            return new LevelScene(level, backgroundName, paralaxStrengths, nextLevel, content, loadScene, inputReader);
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
