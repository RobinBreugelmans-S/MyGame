using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameObjects
{
    internal class StationaryObject : IGameObject
    {
        public Vector2 pos; //TODO: naming conventions!

        public RectangleF CollisionBox;
        public delegate void OnTouch(MoveableObject collisionObject, Vector2 normalVector);
        public OnTouch onTouch;

        protected Texture2D texture;
        protected AnimationHandler animationHandler;

        public StationaryObject(Vector2 pos, AnimationHandler animationHandler, OnTouch onTouch)
        {
            this.pos = pos;
            this.animationHandler = animationHandler;
            this.onTouch = onTouch;
        }

        public void Update()
        {
            animationHandler.UpdatePartRectangle();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animationHandler.Draw(spriteBatch, pos);
        }
    }
}
