using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private Dictionary<string, Func<IScene>> getSceneMethods = new();
        public SceneFactory(ContentManager content, Action<string> loadScene)
        {
            this.content = content;

            getSceneMethods.Add("main_menu", () => getMainMenuScene());
            getSceneMethods.Add("level1", () => getLevel1Scene());
        }

        public IScene GetScene(string sceneName)
        {
            return getSceneMethods[sceneName].Invoke();
        }

        private IScene getMainMenuScene()
        {
            Button[] buttons = {                                                                        //TODO: must be given from constructor
                new(() => loadScene("level1"), new(810,420,300,150), "ButtonSelected", "ButtonUnselected", content.Load<SpriteFont>("PixelFont"), "Start"),
                new(() => loadScene("level1"), new(810,720,300,150), "ButtonSelected", "ButtonUnselected", content.Load<SpriteFont>("PixelFont"), "Start"),
                new(() => /*exit game*/loadScene("level1"), new(810,920,300,150), "ButtonSelected", "ButtonUnselected", content.Load<SpriteFont>("PixelFont"), "Start")
            }; 
            MenuScene mainMenuScene = new("TilesetNature", buttons, content);

            return mainMenuScene;
        }

        private IScene getLevel1Scene()
        {
            return new LevelScene("TestLevel1", content); //loading in level data is in LevelScene.cs
        }
    }
}
