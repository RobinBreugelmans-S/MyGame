using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;

namespace MyGame.GameObjects.LevelObjects
{
    internal class MoveableEntity : Entity, IGameObject
    {
        public Vector2 Vel;
        public Vector2 Acc;

        protected float maxHorizontalSpeed;
        protected float maxVerticalSpeed;

        protected float gravityWhenFalling;

        //public CollisionHandler collisionHandler;
        //public new RectangleF CollisionBox { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        //public new RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }
        public bool IsGrounded;
        public int FacingDirection
        {
            get
            {
                if (AnimationHandler.HorizontalFlip == SpriteEffects.None)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public MoveableEntity(Vector2 pos, Vector2 vel, Vector2 acc, float gravity, AnimationHandler animationHandler, CollisionHandler collisionHandler, Touch onTouch) : base(pos, animationHandler, onTouch)
        {
            this.Vel = vel;
            this.Acc = acc;
            gravityWhenFalling = gravity;
            this.CollisionHandler = collisionHandler;
        }

        public MoveableEntity(Vector2 pos, Vector2 vel, float gravity, AnimationHandler animationHandler, CollisionHandler collisionHandler, Touch onTouch)
            : this(pos, vel, new Vector2(), gravity, animationHandler, collisionHandler, onTouch)
        { }

        public void FaceDirection(int dir)
        {
            if (dir == 1)
            {
                AnimationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (dir == -1)
            {
                AnimationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }
        }

        public override void Update()
        {
            Vel.Y += gravityWhenFalling;

            Vel.X = Vel.X + Acc.X;// Math.Clamp(, -maxHorizontalSpeed, maxHorizontalSpeed);
            Vel.Y = Math.Clamp(Vel.Y + Acc.Y, -maxVerticalSpeed, maxVerticalSpeed);

            if (CollisionHandler != null)
            {
                (Vel, Acc, IsGrounded) = CollisionHandler.HandleCollisions(Pos, Vel, Acc);
            }

            Pos += Vel;

            base.Update();
        }
    }
}
