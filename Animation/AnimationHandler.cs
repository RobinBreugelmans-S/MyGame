using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.GameObjects;
using MyGame.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MyGame.Globals;

namespace MyGame.Animation
{
    internal class AnimationHandler
    {
        public Dictionary<State, AnimationState> AnimationStates = new();
        public Rectangle PartRectangle;
        public State State; //TODO: naming convention enums!! States State
        public Texture2D SpriteSheet;
        public Vector2Int SpriteSize;
        public SpriteEffects HorizontalFlip = SpriteEffects.None;
        public int AnimationTimer = 0;

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
        
        public void ChangeState(State state)
        {
            State = state;
            PartRectangle.Y = AnimationStates[state].Location * SpriteSize.Y;
        }
        public void UpdatePartRectangle()
        {
            AnimationTimer = (AnimationTimer + 1) % AnimationStates[State].Time;
            if (AnimationTimer == 0)
            {
                PartRectangle.X = (PartRectangle.X + SpriteSize.X) % (SpriteSize.X * AnimationStates[State].Length);
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            //first rect = destination, second rect = source in image
            spriteBatch.Draw(SpriteSheet, new Rectangle((int)(Math.Round(pos.X / Zoom) * Zoom),
                (int)(Math.Round(pos.Y / Zoom) * Zoom), PartRectangle.Width * Zoom, PartRectangle.Height * Zoom), // <- dest rect
                PartRectangle, Color.White, 0f, new Vector2(0, 0), HorizontalFlip, 0f);
        }
    }
}
