﻿using Microsoft.Xna.Framework;
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

namespace MyGame.GameObjects
{
    internal class Enemy : MoveableObject, IGameObject
    {
        public float runAcc; //TODO all public?
        public float runSpeed;
        public float jumpPower;
        public float gravityWhenJumping;

        //public new RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }

        //private bool isJumping;

        public Vector2 targetDirection { get; private set; }
        public Vector2Int input { get { return new Vector2Int(Math.Sign(targetDirection.X), Math.Sign(targetDirection.Y));  } }
        public StationaryObject target; //change to Object or sometinhg

        public Action doBehaviour;
        
        public Enemy(Vector2 pos, float runAcc, float runSpeed, float jumpPower, float gravityWhenFalling, float gravityWhenJumping, float maxVerticalSpeed, AnimationHandler animationHandler, CollisionHandler collisionHandler, StationaryObject target)
            : base(pos, new Vector2(), new Vector2(), gravityWhenFalling, animationHandler, collisionHandler, null)
        {
            //new Enemy(pos, runAcc, runSpeed, jumpPower, gravityWhenFalling, gravityWhenJumping, animationHandler, collisionHandler, target, null, null);
            this.runAcc = runAcc;
            this.runSpeed = runSpeed;
            this.jumpPower = jumpPower;
            //this.gravityWhenFalling = gravityWhenFalling; is in base constructor
            this.gravityWhenJumping = gravityWhenJumping;

            this.target = target;

            this.maxVerticalSpeed = maxVerticalSpeed;
        }
        public Enemy(Vector2 pos, float runAcc, float runSpeed, float jumpPower, float gravityWhenFalling, float gravityWhenJumping, AnimationHandler animationHandler, CollisionHandler collisionHandler, StationaryObject target)
            : this(pos, runAcc, runSpeed, jumpPower, gravityWhenFalling, gravityWhenJumping, MaxVerticalSpeed, animationHandler, collisionHandler, target)
        { }

        public Enemy(Vector2 pos, float runAcc, float runSpeed, float jumpPower, float gravityWhenFalling, float gravityWhenJumping, AnimationHandler animationHandler, CollisionHandler collisionHandler, StationaryObject target, Action behaviour, OnTouch onTouch)
            : base(pos, new Vector2(), new Vector2(), gravityWhenFalling, animationHandler, collisionHandler, onTouch)
        { //TODO: what is this
        }
        public void StopMoving()
        {
            acc = new(0f, 0f);
            vel = new(0f, 0f);
            target = this;
        }
        public void FaceInputDirection()
        {
            if (input.X == 1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }
        }
        public new void Update()
        {
            if (target != null)
            {
                targetDirection = GetMiddleOfRect(target.CurrentCollisionBox) - GetMiddleOfRect(CurrentCollisionBox);
            }
            else
            {
                targetDirection = new(0f, 0f);
            }

            if (vel.Y >= 0)
            {
                acc.Y = gravityWhenFalling;
            }
            else
            {
                acc.Y = gravityWhenJumping;
            }
            
            doBehaviour();

            vel.X = Math.Clamp(vel.X + acc.X, -runSpeed, runSpeed);
            vel.Y = Math.Clamp(vel.Y + acc.Y, -maxVerticalSpeed, maxVerticalSpeed);
            
            if (collisionHandler != null)
            {
                (vel, acc, isGrounded) = collisionHandler.HandleCollisions(pos, vel, acc);
            }
            pos += vel;

            animationHandler.UpdatePartRectangle();
        }
    }
}

