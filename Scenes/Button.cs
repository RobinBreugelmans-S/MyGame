using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Animation;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Globals;

namespace MyGame.Scenes
{
    internal class Button : IGameObject
    {
        private Texture2D spriteSheet;
        private AnimationHandler animationHandler;
        public string textureName { get; private set; }
        //private Rectangle buttonBox;
        private Vector2 pos;
        private Vector2Int size = new(120, 40);
        private SpriteFont font;
        private string text;
        public Action OnClick { get; }

        public bool IsSelected;

        public Button(string text, Action onclick, Vector2 pos, Texture2D spriteSheet, SpriteFont font)
        {
            //TODO: fix the stuff with buttonbox
            OnClick = onclick;
            //this.buttonBox = buttonBox;
            this.text = text;
            this.pos = pos;
            this.spriteSheet = spriteSheet;
            this.font = font;
            
            animationHandler = new(spriteSheet, size);
            animationHandler.AddAnimation(State.Unselected, 0, 2, 8); //so that if you move the color doesn't always reset to yellow of the selected button
            animationHandler.AddAnimation(State.Selected, 1, 2, 8);
            animationHandler.ChangeState(State.Unselected);
        }

        public void Unselect()
        {
            IsSelected = false;
            animationHandler.ChangeState(State.Unselected);
        }
        public void Select()
        {
            IsSelected = true;
            animationHandler.ChangeState(State.Selected);
        }

        public void Update()
        {
            animationHandler.Update();
        }

        public void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            animationHandler.Draw(pos, spriteBatch);
            spriteBatch.DrawString(font, text, GetMiddleOfRect(new Rectangle((int)pos.X, (int)pos.Y, size.X * Zoom, size.Y * Zoom)), Color.White, 0, font.MeasureString(text) / 2, 3f, SpriteEffects.None, 0.5f);
        }
    }
}
