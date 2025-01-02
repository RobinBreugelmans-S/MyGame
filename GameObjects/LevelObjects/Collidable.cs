using Microsoft.Xna.Framework;
using MyGame.Misc;
using System.Drawing;

namespace MyGame.GameObjects.LevelObjects
{
    internal class Collidable
    {
        public Vector2 Pos;
        /*public virtual RectangleF CollisionBox { get; protected set; }
        public virtual RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }*/
        //TODO: refactor to not use CollisionHandler for Collidable and (Stationary)Entity, override in MoveableEntity
        public CollisionHandler CollisionHandler;
        public RectangleF CollisionBox { get { return CollisionHandler.CollisionBox; } protected set { CollisionHandler.CollisionBox = value; } }
        public RectangleF CurrentCollisionBox { get { return CollisionBox.At(Pos); } }

        public Collidable(Vector2 pos, RectangleF collisionBox)
        {
            CollisionHandler = new(collisionBox);
            Pos = pos;
            //CollisionBox = collisionBox;
        }

        public Collidable(RectangleF currentCollisionBox)
        {
            Pos = new(currentCollisionBox.X, currentCollisionBox.Y);
            CollisionHandler = new(new RectangleF(0f, 0f, currentCollisionBox.Width, currentCollisionBox.Height));
            //CollisionBox = new RectangleF(0f, 0f, currentCollisionBox.Width, currentCollisionBox.Height);
        }
    }
}
