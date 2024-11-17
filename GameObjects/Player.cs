using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using MyGame.Interfaces;
using MyGame.Animation;
using MyGame.Misc;
using static MyGame.Globals;
using System.Diagnostics;
using System.Net.Mime;


namespace MyGame.GameObjects
{

    internal class Player : MoveableObject //TODO: inherit from class only with collisions, pos, vel acc
    {
        private float runAcc = 2f;
        private float runSpeed = 8f;
        private float jumpPower = 22f;
        private float gravityWhenJumping = 1f;

        private IInputReader inputReader;

        private Vector2Int input = new();
        private bool isJumping = false;

        public int Score = 0;

        //TODO: change to factory method to get player 
        public Player(Vector2 pos, Texture2D spriteSheet, IInputReader inputReader, TileType[,] tileMapCollisions, List<StationaryObject> collidableEntities)
            : base(pos, new Vector2(), new Vector2(), 0f, new AnimationHandler(spriteSheet, new Vector2Int(16, 16)), new CollisionHandler(new RectangleF(3f * Zoom, 10f * Zoom, 10f * Zoom, 6f * Zoom), tileMapCollisions, collidableEntities), null)
        {
            this.inputReader = inputReader;

            collisionHandler.Parent = this; //can't put this in the constructor :(

            gravityWhenFalling = 2f;
            maxVerticalSpeed = 26f;

            //animationHandler = new AnimationHandler(spriteSheet, new Vector2Int(16,16));

            animationHandler.AddAnimation(State.Idling, 0, 4, 16);
            animationHandler.AddAnimation(State.Walking, 1, 4, 4);
            animationHandler.AddAnimation(State.Crouching, 7, 4, 24);
            animationHandler.AddAnimation(State.MidAir, 11, 1, 1);
        }

        public new void Update() //TODO: new , fix ? ?
        {
            //TODO: better region names;
            #region input and Acceletaro
            //TODO: add animator class? to refactor animations and horizontalFlip
            input = inputReader.ReadInput();
            isJumping = inputReader.ReadJumpInput();

            if (input.X == 1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }

            if (vel.Y > 0)//refactor, move to collision detection
            {
                acc.Y = gravityWhenFalling;
            }
            else
            {
                acc.Y = gravityWhenJumping;
            }
            
            if (input.X == 0 || (input.Y == 1 && isGrounded))
            {
                acc.X = Math.Sign(vel.X) * -1; //friction
            }
            else
            {
                acc.X = input.X * runAcc;
            }

            //jump
            if (isJumping && isGrounded)
            {
                vel.Y = -jumpPower;
            }

            //update velocity
            vel.X = Math.Clamp(vel.X + acc.X, -runSpeed, runSpeed);
            vel.Y = Math.Clamp(vel.Y + acc.Y, -maxVerticalSpeed, maxVerticalSpeed);

            #endregion

            //collissions
            (vel, acc, isGrounded) = collisionHandler.HandleCollisions(pos, vel, acc);
            
            //update position
            pos += vel;

            //UpdateChunks();

            #region states
            if (input.X != 0)
            {
                //also changes y pos of partRect
                animationHandler.ChangeState(State.Walking);
            }
            else
            {
                animationHandler.ChangeState(State.Idling);
            }

            if (input.Y == 1)
            {
                animationHandler.ChangeState(State.Crouching);
            }

            if(!isGrounded) //TODO: change to is in air! (if water is added)
            {
                animationHandler.ChangeState(State.MidAir);
            }
            #endregion

            //animation
            animationHandler.UpdatePartRectangle();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            animationHandler.Draw(spriteBatch, pos);
        }
    }
}
