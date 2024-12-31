﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameObjects.MenuObjects;
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
        //private SpriteFont font; //TODO: give font to button in constructor instead of loading it in button

        private string backgroundName;
        private Texture2D background;
        private Button[] buttons;
        
        private Button selectedButton
        { 
            get { return buttons[selectedIndex]; }
        }
        private int selectedIndex = 0;
        Vector2Int inputPrevious;
        Vector2Int input;
        bool enterInputPrevious;
        bool enterInput;
        
        private KeyboardReader keyboardReader = new KeyboardReader();

        public MenuScene(string backgroundName, Button[] buttons, ContentManager content)
        {
            this.backgroundName = backgroundName;
            this.buttons = buttons;
            this.content = content;
        }
        
        private void select(int index)
        {
            selectedButton.Unselect();
            selectedIndex = index;
            selectedButton.Select();
        }

        public void LoadScene()
        {
            background = content.Load<Texture2D>(backgroundName);
            select(0);
        }
        bool isFirstFrame = true;
        public void Update()
        {
            enterInputPrevious = enterInput;
            enterInput = keyboardReader.ReadEnterInput();
            inputPrevious = input;
            input = keyboardReader.ReadInput();

            if(input.X != 0 && input.X != inputPrevious.X)
            {
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

            //don't execute input on first frame, else if user is still holding Enter from previous scene it will instantly press a button
            if (isFirstFrame)
            {
                isFirstFrame = false;
            }
            else if (enterInput && !enterInputPrevious)
            {
                selectedButton.OnClick();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw bg
            spriteBatch.Draw(background, new Rectangle(0, 0, ViewPortSize.X, ViewPortSize.Y), Color.White);
            
            //draw buttons
            foreach(Button button in buttons)
            {
                button.Draw(new(), spriteBatch);
            }
        }

    }
}
