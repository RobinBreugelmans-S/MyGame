using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Scenes
{
    internal class Button : IGameObject
    {
        public Texture2D texture { private get; set; }
        public string textureName { get; }
        private Rectangle collisionBox;
        private SpriteFont font;
        private string text;
        public Action OnClick { get; }

        public bool IsSelected;

        public Button(Action onclick, Rectangle collisionBox, Texture2D texture, SpriteFont font, string text)
        {
            OnClick = onclick;
            this.collisionBox = collisionBox;
            this.texture = texture;
            this.font = font;
            this.text = text;
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            if (IsSelected)
            {
                //draw stuff
            }
            else
            {

            }
        }
    }
}
