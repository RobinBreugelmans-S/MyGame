using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
//TODO: remove unused imports

namespace MyGame.Scenes
{
    internal class MenuScene : IScene
    {
        private ContentManager content;
        private SpriteFont font;

        private Texture2D background;
        private Button[] buttons;
        private Button selectedButton
        { 
            get { return buttons[selectedIndex]; }
            set { selectedButton = value; }
        }
        private int selectedIndex;
        
        private KeyboardReader keyboardReader = new KeyboardReader();

        public MenuScene(Texture2D background, Button[] buttons)
        {
            this.background = background;
            this.buttons = buttons;
            select(this.buttons[0]);
        }
        
        public void select(Button button)
        {
            selectedButton.IsSelected = false;
            selectedButton = button;
            selectedButton.IsSelected = true;
        }

        public void LoadScene()
        {
            foreach (Button button in buttons)
            {
                button.texture = content.Load<Texture2D>(button.textureName);
            }
        }

        public void Update()
        {
            foreach(Button button in buttons)
            {
                button.Update();
            }
            if (keyboardReader.ReadEnterInput())
            {
                selectedButton.OnClick.Invoke();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw bg
            //draw button
            throw new NotImplementedException();
        }

    }
}
