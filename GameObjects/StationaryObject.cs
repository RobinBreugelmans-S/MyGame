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
        //public Vector2 pos; //TODO: naming conventions!
        //public List<Vector2> Chunks = new(); //list of chunks this entity intersects with

        //public RectangleF CollisionBox;// { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        //public CollisionHandler collisionHandler;
        public delegate void OnTouch(StationaryObject collisionObject, Vector2 normalVector);
        public OnTouch Touched; // (collisionObject, normalVector) => stuff

        protected Texture2D texture;
        protected AnimationHandler animationHandler;

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
        /*
        protected void UpdateChunks()
        {
            Chunks = new();
            
            RectangleF currentCollisionBox = CollisionBox.At(pos);

            Vector2 topLeftChunk = new Vector2((float)Math.Floor(currentCollisionBox.X / ChunkSize) * ChunkSize, (float)Math.Floor(currentCollisionBox.Y / ChunkSize) * ChunkSize);
            Vector2Int intersectingChunks = new(
                (int)(Math.Ceiling(currentCollisionBox.Right / ChunkSize) - Math.Floor(currentCollisionBox.Left / ChunkSize)),
                (int)(Math.Ceiling(currentCollisionBox.Bottom / ChunkSize) - Math.Floor(currentCollisionBox.Top / ChunkSize))
            );
            for (int x = 0; x < intersectingChunks.X; x++)
            {
                for (int y = 0; y < intersectingChunks.Y; y++)
                {
                    Chunks.Add(topLeftChunk + new Vector2(x * ChunkSize, y * ChunkSize));
                }
            }
        }*/

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
