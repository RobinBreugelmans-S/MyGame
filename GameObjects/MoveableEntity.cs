using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;

namespace MyGame.GameObjects
{
    internal class MoveableEntity : Entity, IGameObject
    {
        public Vector2 vel;
        public Vector2 acc;
        
        protected float maxHorizontalSpeed;
        protected float maxVerticalSpeed;

        public float gravityWhenFalling;

        //public CollisionHandler collisionHandler;
        //public new RectangleF CollisionBox { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        //public new RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }
        public bool isGrounded;
        public int facingDirection { get 
            {
                if (animationHandler.HorizontalFlip == SpriteEffects.None)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            } 
        }

        public MoveableEntity(Vector2 pos, Vector2 vel, Vector2 acc, float gravity, AnimationHandler animationHandler, CollisionHandler collisionHandler, OnTouch onTouch) : base(pos, animationHandler, onTouch)
        {
            this.vel = vel;
            this.acc = acc;
            gravityWhenFalling = gravity;
            this.collisionHandler = collisionHandler;
        }

        public MoveableEntity(Vector2 pos, Vector2 vel, float gravity, AnimationHandler animationHandler, CollisionHandler collisionHandler, OnTouch onTouch)
            : this(pos, vel, new Vector2(), gravity, animationHandler, collisionHandler, onTouch)
        { }

        public void FaceDirection(int dir)
        {
            if (dir == 1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (dir == -1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }
        }

        public new void Update()
        {
            vel.Y += gravityWhenFalling;

            vel.X = vel.X + acc.X;// Math.Clamp(, -maxHorizontalSpeed, maxHorizontalSpeed);
            vel.Y = Math.Clamp(vel.Y + acc.Y, -maxVerticalSpeed, maxVerticalSpeed);
            
            if(collisionHandler != null)
            {
                (vel, acc, isGrounded) = collisionHandler.HandleCollisions(pos, vel, acc);
            }

            pos += vel;

            base.Update();
        }
    }
}
