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
        private Dictionary<State, AnimationState> animationStates = new();
        private Rectangle partRectangle;
        public State CurrentState;
        private Texture2D spriteSheet;
        private Vector2Int spriteSize;
        public SpriteEffects HorizontalFlip = SpriteEffects.None;
        public int AnimationTimer = 0;
        private int alternateAnimationTimer = 0;

        public AnimationHandler(Texture2D spriteSheet, Vector2Int spriteSize)
        {
            this.spriteSheet = spriteSheet;
            this.spriteSize = spriteSize;
            partRectangle = new Rectangle(0, 0, spriteSize.X, spriteSize.Y);
        }

        public void AddAnimation(State state, int location, int length, int time)
        {
            animationStates.Add(state, new AnimationState(location, length, time));
        }
        public int GetAnimationTime(State state)
        {
            return animationStates[state].Length * animationStates[state].Time;
        }
        public int GetCurrentAnimationFrame()
        {
            return partRectangle.X / spriteSize.X;
        }
        public void ChangeState(State state) //todo change to 
        {
            if(alternateAnimationTimer <= 0 && CurrentState != state) //aka animation is not playing
            {
                CurrentState = state;
                partRectangle.Y = animationStates[state].Location * spriteSize.Y;
            }
        }

        public void Update()
        {
            if(alternateAnimationTimer > 0)
            {
                alternateAnimationTimer--;
            }

            AnimationTimer = (AnimationTimer + 1) % animationStates[CurrentState].Time;
            if (AnimationTimer == 0)
            {
                partRectangle.X = (partRectangle.X + spriteSize.X) % (spriteSize.X * animationStates[CurrentState].Length);
            }
        }

        public void PlayAnimation(State state)
        {
            partRectangle.X = 0;
            ChangeState(state);
            alternateAnimationTimer = animationStates[state].Length * animationStates[state].Time;
        }

        public void Draw(Vector2 pos, SpriteBatch spriteBatch)
        {
            //first rect = destination, second rect = source in image
            spriteBatch.Draw(spriteSheet, new Rectangle((int)(Math.Round(pos.X / Zoom) * Zoom),
                (int)(Math.Round(pos.Y / Zoom) * Zoom), partRectangle.Width * Zoom, partRectangle.Height * Zoom), // <- dest rect
                partRectangle, Color.White, 0f, new Vector2(0, 0), HorizontalFlip, 0f);
        }
    }
}
