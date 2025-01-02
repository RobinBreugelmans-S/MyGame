using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Globals;

namespace MyGame.GameObjects.LevelObjects
{
    internal class Enemy : MoveableEntity, IGameObject
    {
        public float RunAcc; //TODO all public?
        public float RunSpeed;
        public float JumpPower;
        public float GravityWhenJumping;

        //public new RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }

        //private bool isJumping;

        public Vector2 targetDirection { get; private set; } //TODO: change to targetVector or sometihgn
        public Vector2Int input { get { return new Vector2Int(Math.Sign(targetDirection.X), Math.Sign(targetDirection.Y)); } }
        public Entity Target;

        public Action DoBehaviour { protected get; set; }

        public Enemy(Vector2 pos, float runAcc, float runSpeed, float jumpPower, float gravityWhenFalling, float gravityWhenJumping, float maxVerticalSpeed, AnimationHandler animationHandler, CollisionHandler collisionHandler, Entity target)
            : base(pos, new Vector2(), new Vector2(), gravityWhenFalling, animationHandler, collisionHandler, null)
        {
            //new Enemy(pos, runAcc, runSpeed, jumpPower, gravityWhenFalling, gravityWhenJumping, animationHandler, collisionHandler, target, null, null);
            this.RunAcc = runAcc;
            this.RunSpeed = runSpeed;
            this.JumpPower = jumpPower;
            //this.gravityWhenFalling = gravityWhenFalling; is in base constructor
            this.GravityWhenJumping = gravityWhenJumping;

            this.Target = target;

            this.maxVerticalSpeed = maxVerticalSpeed;
        }
        public Enemy(Vector2 pos, float runAcc, float runSpeed, float jumpPower, float gravityWhenFalling, float gravityWhenJumping, AnimationHandler animationHandler, CollisionHandler collisionHandler, Entity target)
            : this(pos, runAcc, runSpeed, jumpPower, gravityWhenFalling, gravityWhenJumping, MaxVerticalSpeed, animationHandler, collisionHandler, target)
        { }

        public void StopMoving()
        {
            Acc = new(0f, 0f);
            Vel = new(0f, 0f);
            Target = this;
        }
        public void FaceInputDirection()
        {
            if (input.X == 1)
            {
                AnimationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                AnimationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }
        }

        public override void Update()
        {
            if (Target != null)
            {
                targetDirection = GetMiddleOfRect(Target.CurrentCollisionBox) - GetMiddleOfRect(CurrentCollisionBox);
            }
            else
            {
                targetDirection = new(0f, 0f);
            }

            if (Vel.Y >= 0)
            {
                Acc.Y = gravityWhenFalling;
            }
            else
            {
                Acc.Y = GravityWhenJumping;
            }

            DoBehaviour();

            Vel.X = Math.Clamp(Vel.X + Acc.X, -RunSpeed, RunSpeed);
            Vel.Y = Math.Clamp(Vel.Y + Acc.Y, -maxVerticalSpeed, maxVerticalSpeed);

            if (CollisionHandler != null)
            {
                (Vel, Acc, IsGrounded) = CollisionHandler.HandleCollisions(Pos, Vel, Acc);
            }
            Pos += Vel;

            AnimationHandler.Update();
        }
    }
}

