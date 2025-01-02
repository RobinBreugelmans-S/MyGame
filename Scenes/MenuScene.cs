using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.GameObjects.MenuObjects;
using MyGame.Input;
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
        private Vector2Int inputPrevious;
        private Vector2Int input;
        private bool enterInputPrevious;
        private bool enterInput;
        
        private bool isFirstFrame = true;
        
        private IInputReader inputReader;

        public MenuScene(string backgroundName, Button[] buttons, ContentManager content, IInputReader inputReader)
        {
            this.backgroundName = backgroundName;
            this.buttons = buttons;
            this.content = content;
            this.inputReader = inputReader;
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
        public void Update()
        {
            enterInputPrevious = enterInput;
            enterInput = inputReader.ReadEnterInput();
            inputPrevious = input;
            input = inputReader.ReadInput();

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
