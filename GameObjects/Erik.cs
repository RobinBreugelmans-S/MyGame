using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MyGame.Animation;
using MyGame.Interfaces;
using MyGame.Misc;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;
using System.Collections.Generic;

namespace MyGame.GameObjects
{
    internal class Erik : MoveableObject, IGameObject //refactor to have enemy class that already has target
    {
        private float runAcc = 1f;
        private float runSpeed = 6f;
        private float jumpPower = 16f;
        private float gravityWhenJumping = 1f;

        //private bool isJumping;

        private Vector2Int input;
        private StationaryObject target; //change to Object or sometinhg
        
        public Erik(Vector2 pos, Texture2D spriteSheet, StationaryObject target, TileType[,] tileMapCollisions, List<StationaryObject> collidableEntities) : base(pos, new Vector2(), new Vector2(), 0f, new AnimationHandler(spriteSheet, new Vector2Int(23, 16)), new CollisionHandler(new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom), tileMapCollisions, collidableEntities), null)
        {
            this.target = target;

            //CollisionBox = new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom); //TODO: is not centered!

            gravityWhenFalling = 2f;
            maxVerticalSpeed = 26f;

            //animationHandler = new AnimationHandler(spriteSheet, new Vector2Int(23, 16));

            // .AnimationStates.Add() -> .AddAnimationState()
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(0,4,4));
            animationHandler.AnimationStates.Add(State.Jumping, new AnimationState(1, 1, 1));

            animationHandler.ChangeState(State.Walking);
        }
        
        public void Update()
        {
            #region input & acceleration
            Vector2 targetDirection = target.pos - pos;
            input = new Vector2Int(Math.Sign(targetDirection.X), Math.Sign(targetDirection.Y));

            if (input.X == 1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }

            if (vel.Y > 0)
            {
                acc.Y = gravityWhenFalling;
            }
            else
            {
                acc.Y = gravityWhenJumping;
            }

            if (input.X == 0)
            {
                acc.X = Math.Sign(vel.X) * -1; //friction
            }
            else
            {
                acc.X = input.X * runAcc;
            }

            vel.X = Math.Clamp(vel.X + acc.X, -runSpeed, runSpeed);
            vel.Y = Math.Clamp(vel.Y + acc.Y, -maxVerticalSpeed, maxVerticalSpeed);

            //jump if wall is infront
            if (isGrounded)
            {
                List<Vector2Int> horizontalCollisions = collisionHandler.GetCollisions(pos + new Vector2(vel.X, 0));
                foreach (Vector2Int collision in horizontalCollisions) //TODO: colissions -> tiles
                {
                    if (collisionHandler.TileMapCollisions.tryGetValue(collision, out TileType tileType) && (tileType == TileType.Solid)) //0 = air //|| tileType == TileType.SemiRight
                    {
                        vel.Y = -jumpPower;
                        break;
                    }
                }
            }
            #endregion

            (vel, acc, isGrounded) = collisionHandler.HandleCollisions(pos, vel, acc);

            pos += vel;

            UpdateChunks();

            #region animation states
            /*if (isAttacking)
            {

            }*/
            
            #endregion

            animationHandler.UpdatePartRectangle();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            animationHandler.Draw(spriteBatch, pos);
        }
    }
}
