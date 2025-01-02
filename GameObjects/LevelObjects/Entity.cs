using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;


namespace MyGame.GameObjects.LevelObjects
{
    internal class Entity : Collidable, IGameObject
    {
        //public RectangleF CollisionBox;// { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        //public int priority; TODO: use this (sort entity list by this), //lower values will get updated and drawed first
        public Touch OnTouch; //(collisionObject, normalVector) => stuff

        protected Texture2D texture;
        public State State { get { return AnimationHandler.CurrentState; } }
        public AnimationHandler AnimationHandler;

        public Entity(Vector2 pos, RectangleF collisionBox, AnimationHandler animationHandler) : base(pos, collisionBox)
        {
            //this.pos = pos;
            //collisionHandler = new(collisionBox, null, null);
            //CollisionBox = collisionBox;
            AnimationHandler = animationHandler;
            //Touched = touched;
        }

        protected Entity(Vector2 pos, AnimationHandler animationHandler, Touch touched) : base(pos, new())
        {
            AnimationHandler = animationHandler;
            OnTouch = touched;
        }

        public void PlayAnimation(State animationState)
        {
            AnimationHandler.PlayAnimation(animationState);
        }

        public void ChangeState(State state)
        {
            AnimationHandler.ChangeState(state);
        }

        public virtual void Update()
        {
            AnimationHandler.Update();
        }

        public virtual void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            AnimationHandler.Draw(Pos + offset, spriteBatch); //TODO: first offset, then spritebatch
        }
    }
}
