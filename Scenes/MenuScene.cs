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
        
        public void select(int index)
        {
            selectedButton.IsSelected = false;
            selectedIndex = index;
            selectedButton.IsSelected = true;
        }

        public void LoadScene()
        {
            background = content.Load<Texture2D>(backgroundName);
            foreach (Button button in buttons)
            {
                button.textureSelected = content.Load<Texture2D>(button.textureSelectedName);
                button.textureUnselected = content.Load<Texture2D>(button.textureUnselectedName);
            }
            select(0);
        }
        
        public void Update()
        {
            inputPrevious = input;
            input = keyboardReader.ReadInput();

            if(input.X != 0 && input.X != inputPrevious.X)
            {//TODO: fix index out of bounds
                select((selectedIndex + input.X) % buttons.Length);
            }/* TODO: FIX!! (use input vector2int to get directino or something,v  
            if (input.Y != 0 && input.Y != inputPrevious.Y)
            {
                select((selectedIndex + input.Y) % buttons.Length);
            }*/
            Debug.WriteLine(selectedIndex);
            foreach (Button button in buttons)
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
            spriteBatch.Draw(background, new Rectangle(0, 0, BufferSize.X, BufferSize.Y), Color.White);

            //draw buttons
            foreach(Button button in buttons)
            {
                button.Draw(new(), spriteBatch);
            }
        }

    }
}
