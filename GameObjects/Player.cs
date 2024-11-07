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


namespace MyGame.GameObjects
{
    enum State {Idling, Walking, Jumping, Crouching, MidAir}

    internal class Player : CollisionObject, IGameObject //TODO: inherit from class only with collisions, pos, vel acc
    {
        private float runAcc = 2f;
        private float runSpeed = 8f;
        private float jumpPower = 22f;
        private float gravityWhenJumping = 1f;
        private float gravityWhenFalling = 2f;
        private float maxVerticalSpeed = 26f;

        private Dictionary<State, AnimationState> animationStates = new();
        private State state;
        private Texture2D texture;
        private Rectangle partRectangle;
        private Vector2Int spriteSize = new Vector2Int(16,16);
        private SpriteEffects horizontalFlip = SpriteEffects.None;
        private int animationTimer = 0;

        private IInputReader inputReader;

        private Vector2Int input = new();
        private bool isJumping = false;
        private bool isGrounded = false; //TODO: have no value at start, get calculated instantly
        
        public Player(Vector2 pos, Texture2D texture, IInputReader inputReader, Dictionary<Vector2Int, int> tilemapCollisions)
        {
            this.inputReader = inputReader;

            collisionBox = new(3f * Zoom, 10f * Zoom, 10f * Zoom, 6f * Zoom);

            this.texture = texture;
            partRectangle = new Rectangle(0, 0, spriteSize.X, spriteSize.Y);

            animationStates.Add(State.Idling, new AnimationState(0, 4, 16));
            animationStates.Add(State.Walking, new AnimationState(1, 4, 4));
            animationStates.Add(State.Crouching, new AnimationState(7, 4, 24));
            animationStates.Add(State.MidAir, new AnimationState(11, 1, 1));

            //TODO:
            //method will be executed when player hits a certain tile type
            /*Dictionary<int, Action<Vector2>> collisionMethods = new();
            collisionMethods.Add(1, contactNormal =>
            {
                if (contactNormal == new Vector2(0, -1))
                {
                    isGrounded = true;
                    acc.Y = 0f;
                }
            });
            collisionMethods.Add(2, contactNormal =>
            { //no need to check contactNormal since it will always be 0, -1 on this semi-solid
                isGrounded = true;
                acc.Y = 0f;
            });*/

            this.pos = pos;
            this.tilemapCollisions = tilemapCollisions;
        }

        public void Draw(SpriteBatch spriteBatch) // TODO: refactor 4 to Zoom
        {
            //TODO: put in seperate function
            //first rect = destination, second rect = source in image
            spriteBatch.Draw(texture, new Rectangle((int)(Math.Round(pos.X / Zoom) * Zoom),
                (int)(Math.Round(pos.Y / Zoom) * Zoom), partRectangle.Width * Zoom, partRectangle.Height * Zoom),
                partRectangle, Color.White, 0, new Vector2(0,0), horizontalFlip, 0f);
        }
        
        private void changeState(State state)
        {
            this.state = state;
            partRectangle.Y = animationStates[state].Location * spriteSize.Y;
        }

        public void Update()
        {
            //TODO: better region names;
            #region input and Acceletaro
            //TODO: add animator class? to refactor animations and horizontalFlip
            input = inputReader.ReadInput();
            isJumping = inputReader.ReadJumpInput();

            if (input.X == 1)
            {
                horizontalFlip = SpriteEffects.None;
            }
            else if (input.X == -1)
            {
                horizontalFlip = SpriteEffects.FlipHorizontally;
            }

            //check if is grounded //TODO: fix jumping against wall, problem is in CollisionObject.cs line 139
            List<Vector2Int> ground = getCollisions(pos + new Vector2(0, 2)); //half pixel on screen
            isGrounded = false;
            if (pos.Y % TileSize == 0f)
            {
                foreach (Vector2Int collision in ground)
                {
                    if(tilemapCollisions.TryGetValue(collision, out int tileType) && tileType == 1 || tileType == 2)
                    {
                        isGrounded = true;
                    }
                }
            }
            
            //TODO: add looking up IdleUp?
            //gravity
            /*if (isGrounded) //TODO: uncomment annd fix
            {
                acc.Y = 0;
            }
            else //TODO: instead of setting isGrounded during calculating collisions
            {*/
                if (!isJumping && vel.Y > 0)//refactor, move to collision detection
                {
                    acc.Y = gravityWhenFalling;
                }
                else
                {
                    acc.Y = gravityWhenJumping;
                }
            //}
            //calculate acceleration
            //figure out wtf this does
            if (input.X == 0 || (input.Y == 1 && isGrounded))
            {
                acc.X = Math.Sign(vel.X) * -1; //friction
            }
            else
            {
                acc.X = input.X * runAcc;
            }

            //jump
            if (isJumping && isGrounded) //TODO: && on ground
            {
                vel.Y = -jumpPower;
            }

            //update velocity
            vel.X = Math.Clamp(vel.X + acc.X, -runSpeed, runSpeed);
            vel.Y = Math.Clamp(vel.Y + acc.Y, -maxVerticalSpeed, maxVerticalSpeed);

            #endregion

            //collissions
            handleCollisions();
            
            //update position
            pos += vel;

            #region states
            if (input.X != 0)
            {
                //also changes y pos of partRect
                changeState(State.Walking);
            }
            else
            {
                changeState(State.Idling);
            }

            if (input.Y == 1)
            {
                changeState(State.Crouching);
            }

            if(!isGrounded) //TODO: change to is in air!
            {
                changeState(State.MidAir);
            }
            #endregion

            //animation
            animationTimer = (animationTimer + 1) % animationStates[state].Time;
            if (animationTimer == 0)
            {
                partRectangle.X = (partRectangle.X + spriteSize.X) % (spriteSize.X * animationStates[state].Length);
            }
        }
    }
}
