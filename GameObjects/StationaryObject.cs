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
    internal class StationaryObject : IGameObject
    {
        public Vector2 pos; //TODO: naming conventions!
        public List<Vector2> Chunks = new(); //list of chunks this entity intersects with

        public RectangleF CollisionBox { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        public CollisionHandler collisionHandler;
        public delegate void OnTouch(MoveableObject collisionObject, Vector2 normalVector);
        public OnTouch Touched; // (collisionObject, normalVector) => stuff

        protected Texture2D texture;
        protected AnimationHandler animationHandler;

        public StationaryObject(Vector2 pos, RectangleF collisionBox, AnimationHandler animationHandler, OnTouch touched)
        {
            this.pos = pos;
            collisionHandler = new(collisionBox, null, null);
            this.animationHandler = animationHandler;
            Touched = touched;

            //set once since it never has to be updated
            UpdateChunks();
        }

        protected StationaryObject(Vector2 pos, AnimationHandler animationHandler, OnTouch touched)
        {
            this.pos = pos;
            this.animationHandler = animationHandler;
            Touched = touched;
        }

        protected void UpdateChunks()
        {
            Debug.WriteLine("-----updateChunks-----");
            Chunks = new();
            
            RectangleF currentCollisionBox = CollisionBox.At(pos);

            Vector2 topLeftChunk = new Vector2((float)Math.Floor(currentCollisionBox.X / ChunkSize) * ChunkSize, (float)Math.Floor(currentCollisionBox.Y / ChunkSize) * ChunkSize);
            Debug.WriteLine($"pos: {pos.X}, {pos.Y}");
            Debug.WriteLine($"topLeftChunk: {topLeftChunk.X}, {topLeftChunk.Y}");
            Vector2Int intersectingChunks = new(
                (int)(Math.Ceiling(currentCollisionBox.Right / ChunkSize) - Math.Floor(currentCollisionBox.Left / ChunkSize)),
                (int)(Math.Ceiling(currentCollisionBox.Bottom / ChunkSize) - Math.Floor(currentCollisionBox.Top / ChunkSize))
            );
            Debug.WriteLine($"{(int)Math.Ceiling(currentCollisionBox.Right / ChunkSize)} - {(int)Math.Floor(currentCollisionBox.Left / ChunkSize)}");
            Debug.WriteLine($"{(int)Math.Ceiling(currentCollisionBox.Bottom / ChunkSize)} - {(int)Math.Floor(currentCollisionBox.Top / ChunkSize)}");
            for (int x = 0; x < intersectingChunks.X; x++)
            {
                for (int y = 0; y < intersectingChunks.Y; y++)
                {
                    var chnk = topLeftChunk + new Vector2(x * ChunkSize, y * ChunkSize);
                    Debug.WriteLine($"chunk: {chnk.X}, {chnk.Y}");
                    Chunks.Add(chnk);
                }
            }
        }

        public void Update()
        {
            animationHandler.UpdatePartRectangle();
        }

        public void Draw(SpriteBatch spriteBatch)
        {//draw chunks
            foreach (Vector2 chunk in Chunks)
            {
                Debug.WriteLine("chunked");
                spriteBatch.Draw(DebugImage, new Microsoft.Xna.Framework.Rectangle((int)chunk.X, (int)chunk.Y, ChunkSize, ChunkSize), Microsoft.Xna.Framework.Color.Red);
            }



            animationHandler.Draw(spriteBatch, pos);
        }
    }
}
