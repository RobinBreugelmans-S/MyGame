using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Scenes
{
    internal class Button : IGameObject
    {
        public Texture2D textureSelected { private get; set; }
        public Texture2D textureUnselected { private get; set; }

        public string textureSelectedName { get; private set; }
        public string textureUnselectedName { get; private set; }
        private Rectangle buttonBox;
        private SpriteFont font;
        private string text;
        public Action OnClick { get; }

        public bool IsSelected;

        public Button(Action onclick, Rectangle buttonBox, string textureSelectedName, string textureUnselectedName, SpriteFont font, string text)
        {
            OnClick = onclick;
            this.buttonBox = buttonBox;
            this.textureSelectedName = textureSelectedName;
            this.textureUnselectedName = textureUnselectedName;
            this.font = font;
            this.text = text;
        }

        public void Update()
        {
            //animate
        }

        public void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            Debug.WriteLine(IsSelected);
            if (IsSelected)
            {
                spriteBatch.Draw(textureSelected, buttonBox, Color.White);
            }
            else
            {
                spriteBatch.Draw(textureUnselected, buttonBox, Color.White);
            }
        }
    }
}
