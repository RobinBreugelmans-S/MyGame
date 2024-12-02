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


namespace MyGame.GameObjects
{
    internal class StationaryObject : Collidable, IGameObject
    {
        //public List<object> data; //used to store some data for certain entities 

        //public Vector2 pos; //TODO: naming conventions!
        //public List<Vector2> Chunks = new(); //list of chunks this entity intersects with

        //public RectangleF CollisionBox;// { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        public bool isCollidable; //TODO: add to constructors?
        //public int priority; TODO: use this (sort entity list by this), //lower values will get updated and drawed first
        public OnTouch Touched; // (collisionObject, normalVector) => stuff
        public Action Initialize;

        protected Texture2D texture;
        public State State { get { return animationHandler.State; } }
        public AnimationHandler animationHandler; //TODO: -> AnimationHandler

        public StationaryObject(Vector2 pos, RectangleF collisionBox, AnimationHandler animationHandler) : base(pos, collisionBox)
        {
            //this.pos = pos;
            //collisionHandler = new(collisionBox, null, null);
            //CollisionBox = collisionBox;
            this.animationHandler = animationHandler;
            //Touched = touched;

            //set once since it never has to be updated
            //UpdateChunks();
        }

        protected StationaryObject(Vector2 pos, AnimationHandler animationHandler, OnTouch touched) : base(pos, new())
        {
            this.animationHandler = animationHandler;
            Touched = touched;
        }

        public void PlayAnimation(State animationState)
        {
            animationHandler.PlayAnimation(animationState);
        }

        public void ChangeState(State state)
        {
            animationHandler.ChangeState(state);
        }

        public void Update()
        {
            animationHandler.UpdatePartRectangle();
        }

        public void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            animationHandler.Draw(spriteBatch, pos + offset); //TODO: first offset, then spritebatch
        }
    }
}
