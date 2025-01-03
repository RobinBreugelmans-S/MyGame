﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using MyGame.Animation;
using MyGame.Misc;
using static MyGame.Globals;
using System.Diagnostics;
using System.Net.Mime;
using MyGame.Input;


namespace MyGame.GameObjects.LevelObjects
{

    internal class Player : MoveableEntity, IGameObject //TODO: inherit from class only with collisions, pos, vel acc
    {
        private float runAcc = 2f;
        private float runSpeed = 8f;
        private float jumpPower = 22f;
        private float gravityWhenJumping = 1f;
        //gravityWhenFalling = 2f;
        //maxVerticalSpeed = 26f;

        private IInputReader inputReader;

        private Vector2Int input = new();
        private bool isJumping;
        private bool isJumpingPrevious = false;

        private bool isImmune { get { return immuneTimer > 0; } }
        private int immuneTimer = 0;

        public int Score = 0;
        public int HP = 3;

        private Rectangle levelBox;

        public Action<string> LoadScene { private get; set; }
        private int deathTimer = 60;

        //TODO: change to factory method to get player 
        public Player(Vector2 pos, Texture2D spriteSheet, IInputReader inputReader, TileType[,] tileMapCollisions, List<Entity> collidableEntities)
            : base(pos, new Vector2(), new Vector2(), 0f, new AnimationHandler(spriteSheet, new Vector2Int(16, 16)), new CollisionHandler(new RectangleF(3f * Zoom, 10f * Zoom, 10f * Zoom, 6f * Zoom), tileMapCollisions, collidableEntities), null)
        {
            gravityWhenFalling = 2f;
            maxVerticalSpeed = 26f;

            this.inputReader = inputReader;

            CollisionHandler.Parent = this; //can't put this in the constructor

            levelBox = new(0, 0, tileMapCollisions.GetLength(0) * TileSize, tileMapCollisions.GetLength(1) * TileSize);

            AnimationHandler.AddAnimation(State.Idling, 0, 4, 16);
            AnimationHandler.AddAnimation(State.Walking, 1, 4, 4);
            AnimationHandler.AddAnimation(State.Crouching, 7, 4, 24);
            AnimationHandler.AddAnimation(State.MidAir, 11, 1, 1);
        }

        public void DamageIfNotImmune()
        {
            if (!isImmune)
            {
                HP--;
                immuneTimer = 48;
            }
        }

        public override void Update() //TODO: new , fix ? ?
        {
            if (GetMiddleOfRect(CurrentCollisionBox).Y > levelBox.Bottom) //if middle of player is below 0
            {
                HP = 0;
            }

            if (immuneTimer > 0)
            {
                immuneTimer--;
            }

            if (HP <= 0)
            {
                deathTimer--;
            }
            if (deathTimer <= 0)
            {
                LoadScene("main_menu");
            }

            #region input and acceleration
            //TODO: add animator class? to refactor animations and horizontalFlip
            input = inputReader.ReadInput();
            isJumpingPrevious = isJumping;
            //add jump timer and coyote time
            isJumping = inputReader.ReadJumpInput();

            if (input.X == 1)
            {
                AnimationHandler.HorizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                AnimationHandler.HorizontalFlip = SpriteEffects.FlipHorizontally;
            }

            if (Vel.Y >= 0 || !isJumping)
            {
                Acc.Y = gravityWhenFalling;
            }
            else
            {
                Acc.Y = gravityWhenJumping;
            }

            if (Vel.Y < 0 && !isJumping && isJumpingPrevious)
            {
                Vel.Y *= .5f;
            }

            if (input.X == 0 || input.Y == 1 && IsGrounded)
            {
                Acc.X = Math.Sign(Vel.X) * -1; //friction
            }
            else
            {
                Acc.X = input.X * runAcc;
            }

            //jump
            if (isJumping && IsGrounded)
            {
                Vel.Y = -jumpPower;
            }

            //update velocity
            Vel.X = Math.Clamp(Vel.X + Acc.X, -runSpeed, runSpeed);
            Vel.Y = Math.Clamp(Vel.Y + Acc.Y, -maxVerticalSpeed, maxVerticalSpeed);

            #endregion

            //collissions
            (Vel, Acc, IsGrounded) = CollisionHandler.HandleCollisions(Pos, Vel, Acc);

            //update position
            Pos += Vel;


            #region states
            if (input.X != 0 && input.Y != 1)
            {
                //also changes y pos of partRect
                AnimationHandler.ChangeState(State.Walking);
            }
            else if (input.Y != 1)
            {
                AnimationHandler.ChangeState(State.Idling);
            }
            else
            {
                AnimationHandler.ChangeState(State.Crouching);
            }

            if (!IsGrounded) //TODO: change to is in air! (if water is added)
            {
                AnimationHandler.ChangeState(State.MidAir);
            }
            #endregion

            //animation
            AnimationHandler.Update();
        }
        public override void Draw(Vector2 offset, SpriteBatch spriteBatch)
        {
            if (immuneTimer / 4 % 2 == 0)
            {
                AnimationHandler.Draw(Pos + offset, spriteBatch);
            }
        }

    }
}
