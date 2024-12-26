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

    internal class Player : MoveableEntity, IGameObject //TODO: inherit from class only with collisions, pos, vel acc
    {
        private float runAcc = 2f;
        private float runSpeed = 8f;
        private float jumpPower = 22f;
        private float gravityWhenJumping = 1f;
        private new float gravityWhenFalling = 2f;
        private new float maxVerticalSpeed = 26f;

        private IInputReader inputReader;

        private Vector2Int input = new();
        private bool isJumping;
        private bool isJumpingPrevious = false;

        private bool isImmune { get { return immuneTimer > 0; } }
        private int immuneTimer = 0;
        
        public int Score = 0;
        public int HP = 3;

        private Rectangle levelBox;
        
        //TODO: change to factory method to get player 
        public Player(Vector2 pos, Texture2D spriteSheet, IInputReader inputReader, TileType[,] tileMapCollisions, List<Entity> collidableEntities)
            : base(pos, new Vector2(), new Vector2(), 0f, new AnimationHandler(spriteSheet, new Vector2Int(16, 16)), new CollisionHandler(new RectangleF(3f * Zoom, 10f * Zoom, 10f * Zoom, 6f * Zoom), tileMapCollisions, collidableEntities), null)
        {
            this.inputReader = inputReader;

            collisionHandler.Parent = this; //can't put this in the constructor :(

            levelBox = new(0, 0, tileMapCollisions.GetLength(0) * TileSize, tileMapCollisions.GetLength(1) * TileSize);

            //animationHandler = new AnimationHandler(spriteSheet, new Vector2Int(16,16));
            animationHandler.AddAnimation(State.Idling, 0, 4, 16);
            animationHandler.AddAnimation(State.Walking, 1, 4, 4);
            animationHandler.AddAnimation(State.Crouching, 7, 4, 24);
            animationHandler.AddAnimation(State.MidAir, 11, 1, 1);
        }

        public void DamageIfNotImmune()
        {
            if (!isImmune)
            {
                HP--;
                immuneTimer = 48;
            }
        }

        public new void Update() //TODO: new , fix ? ?
        {
            if (!levelBox.Contains(GetMiddleOfRect(CurrentCollisionBox))) //if middle of player is outside of the level area
            {
                HP = 0;
            }

            if (immuneTimer > 0)
            {
                immuneTimer--;
            }
            //TODO: better region names;
            #region input and Acceletaro
            //TODO: add animator class? to refactor animations and horizontalFlip
            input = inputReader.ReadInput();
            isJumpingPrevious = isJumping;
            //add jump timer and coyote time
            isJumping = inputReader.ReadJumpInput();
            
            if (input.X == 1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                animationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }

            if (vel.Y >= 0 || !isJumping)
            {
                acc.Y = gravityWhenFalling;
            }
            else
            {
                acc.Y = gravityWhenJumping;
            }

            if(vel.Y < 0 &&!isJumping && isJumpingPrevious)
            {
                vel.Y *= .5f;
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
            if (input.X != 0 && input.Y != 1)
            {
                //also changes y pos of partRect
                animationHandler.ChangeState(State.Walking);
            }
            else if(input.Y != 1)
            {
                animationHandler.ChangeState(State.Idling);
            }
            else
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
        public new void Draw(Vector2 offset,SpriteBatch spriteBatch)
        {
            if (immuneTimer/4 % 2 == 0)
            {
                animationHandler.Draw(spriteBatch, pos + offset);
            }
        }

    }
}
