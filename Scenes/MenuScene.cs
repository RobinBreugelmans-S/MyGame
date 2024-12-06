using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Globals;
//TODO: remove unused imports

namespace MyGame.Scenes
{
    internal class MenuScene : IScene
    {
        private ContentManager content;
        private SpriteFont font;

        private string backgroundName;
        private Texture2D background;
        private Button[] buttons;
        
        private Button selectedButton
        { 
            get { return buttons[selectedIndex]; }
            set { selectedButton = value; }
        }
        private int selectedIndex = 0;
        Vector2Int input;
        Vector2Int inputPrevious;
        
        private KeyboardReader keyboardReader = new KeyboardReader();

        public MenuScene(string backgroundName, Button[] buttons, ContentManager content)
        {
            this.backgroundName = backgroundName;
            this.buttons = buttons;
            this.content = content;
        }
        
        private void select(int index)
        {
            selectedButton.Unselect();//selectedButton.IsSelected = false;
            selectedIndex = index;
            selectedButton.Select();
        }

        public void LoadScene()
        {
            background = content.Load<Texture2D>(backgroundName);
            /*foreach (Button button in buttons)
            {
                //TODO: refactor into button
                button.SpriteSheet = content.Load<Texture2D>(button.textureName);
            }*/
            select(0);
        }
        int mod(int x, int m) //because negative module in c# is bad //TODO: phrase all comments better lol
        {//TODO: move to globals
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        public void Update()
        {
            inputPrevious = input;
            input = keyboardReader.ReadInput();

            if(input.X != 0 && input.X != inputPrevious.X)
            {//TODO: fix index out of bounds
                select(mod(selectedIndex + input.X, buttons.Length));
            }
            if (input.Y != 0 && input.Y != inputPrevious.Y)
            {
                select(mod(selectedIndex + input.Y, buttons.Length));
            }

            foreach (Button button in buttons)
            {
                button.Update();
            }
            
            if (keyboardReader.ReadEnterInput()) //command pattern
            {
                selectedButton.OnClick();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw bg
            spriteBatch.Draw(background, new Rectangle(0, 0, BufferSize.X, BufferSize.Y), Color.White);
            
            //draw buttons
            foreach(Button button in buttons)
            {
                button.Draw(new(), spriteBatch);
            }
        }

    }
}
