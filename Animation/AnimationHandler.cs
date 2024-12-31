using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.GameObjects;
using MyGame.Interfaces;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MyGame.Globals;

namespace MyGame.Animation
{
    internal class AnimationHandler : IGameObject
    {
        //TODO which can be private, or get/set?
        public Dictionary<State, AnimationState> AnimationStates = new();
        public Rectangle PartRectangle;
        public State State; //TODO: naming convention enums!! States State //TODO change to currentState
        public Texture2D SpriteSheet;
        public Vector2Int SpriteSize;
        public SpriteEffects HorizontalFlip = SpriteEffects.None;
        public int AnimationTimer = 0; //TODO: private?
        private int alternateAnimationTimer = 0;

        public AnimationHandler(Texture2D spriteSheet, Vector2Int spriteSize)
        {
            SpriteSheet = spriteSheet;
            SpriteSize = spriteSize;
            PartRectangle = new Rectangle(0, 0, spriteSize.X, spriteSize.Y);
        }

        public void AddAnimation(State state, int location, int length, int time)
        {
            AnimationStates.Add(state, new AnimationState(location, length, time));
        }
        public int GetAnimationTime(State state)
        {
            return AnimationStates[state].Length * AnimationStates[state].Time;
        }
        public int GetCurrentAnimationFrame()
        {
            return PartRectangle.X / SpriteSize.X;
        }
        public void ChangeState(State state) //todo change to 
        {
            if(alternateAnimationTimer <= 0 && State != state) //aka animation is not playing
            {
                State = state;
                PartRectangle.Y = AnimationStates[state].Location * SpriteSize.Y;
            }
        }

        public void Update()
        {
            if(alternateAnimationTimer > 0)
            {
                alternateAnimationTimer--;
            }

            AnimationTimer = (AnimationTimer + 1) % AnimationStates[State].Time;
            if (AnimationTimer == 0)
            {
                PartRectangle.X = (PartRectangle.X + SpriteSize.X) % (SpriteSize.X * AnimationStates[State].Length);
            }
        }

        public void PlayAnimation(State state)
        {
            PartRectangle.X = 0;
            ChangeState(state);
            alternateAnimationTimer = AnimationStates[state].Length * AnimationStates[state].Time;
        }

        public void Draw(Vector2 pos, SpriteBatch spriteBatch)
        {
            //first rect = destination, second rect = source in image
            spriteBatch.Draw(SpriteSheet, new Rectangle((int)(Math.Round(pos.X / Zoom) * Zoom),
                (int)(Math.Round(pos.Y / Zoom) * Zoom), PartRectangle.Width * Zoom, PartRectangle.Height * Zoom), // <- dest rect
                PartRectangle, Color.White, 0f, new Vector2(0, 0), HorizontalFlip, 0f);
        }
    }
}
