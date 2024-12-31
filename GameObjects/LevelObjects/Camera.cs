using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static MyGame.Globals;
using System.Drawing;
using MyGame.Misc;
using System.Diagnostics;
using System;

namespace MyGame.GameObjects.LevelObjects
{
    internal class Camera
    {
        //TODO: refactor so that the pos is the top left ! ! ! ! ! ! !

        //TODO: use Vector2Int?
        //public Vector2 pos { get; private set; } //pos is the middle of the viewport here! //TODO: naming conventions
        public Vector2 PosMiddle
        {
            get { return Vector2.Floor(GetMiddleOfRect(Viewport) / Zoom) * Zoom; }
            private set
            {
                Viewport.X = value.X - ViewPortSize.X / 2;
                Viewport.Y = value.Y - ViewPortSize.Y / 2;
            }
        }
        public Vector2 Pos
        {
            get { return Vector2.Floor(new Vector2(Viewport.X, Viewport.Y) / Zoom) * Zoom; }
            private set
            {
                Viewport.X = value.X;
                Viewport.Y = value.Y;
            }
        }
        private MoveableEntity target;
        public RectangleF Viewport = new(0f, 0f, ViewPortSize.X, ViewPortSize.Y);
        private Vector2Int bounds;//boundary of the level. so that the camera can't go outside
        //top left is always 0, 0
        public Camera(MoveableEntity target, Vector2Int bounds)
        {
            this.target = target;
            PosMiddle = GetMiddleOfRect(target.CurrentCollisionBox);
            this.bounds = bounds;
        }

        public void Update()
        {
            PosMiddle += (GetMiddleOfRect(target.CurrentCollisionBox) + target.vel - PosMiddle) / 8;

            if (Viewport.Right > bounds.X)
            {
                Viewport.X = bounds.X - Viewport.Width;
            }
            if (Viewport.Bottom > bounds.Y)
            {
                Viewport.Y = bounds.Y - Viewport.Height;
            }

            if (Viewport.Left < 0)
            {
                Viewport.X = 0;
            }
            if (Viewport.Top < 0)
            {
                Viewport.Y = 0;
            }
        }

    }
}
