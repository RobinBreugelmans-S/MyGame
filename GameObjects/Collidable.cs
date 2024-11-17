using Microsoft.Xna.Framework;
using MyGame.Misc;
using System.Drawing;

namespace MyGame.GameObjects
{
    internal class Collidable
    {
        public Vector2 pos; //TODO: pos -> Pos   naming convenitons!8
        public RectangleF CollisionBox;
        public RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }
        public Collidable(Vector2 pos, RectangleF collisionBox)
        {
            this.pos = pos;
            CollisionBox = collisionBox;
        }

        public Collidable(RectangleF currentCollisionBox)
        {
            pos = new(currentCollisionBox.X, currentCollisionBox.Y);
            CollisionBox = new RectangleF(0f, 0f, currentCollisionBox.Width, currentCollisionBox.Height);
        }
    }
}
