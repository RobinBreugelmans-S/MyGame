using Microsoft.Xna.Framework;
using MyGame.Misc;
using System.Drawing;

namespace MyGame.GameObjects
{
    internal class Collidable
    {
        public Vector2 Pos; //TODO: pos -> Pos   naming convenitons!8
        /*public virtual RectangleF CollisionBox { get; protected set; }
        public virtual RectangleF CurrentCollisionBox { get { return CollisionBox.At(pos); } }*/
        //TODO: refactor to not use CollisionHandler for Collidable and (Stationary)Entity, override in MoveableEntity
        public CollisionHandler collisionHandler;
        public RectangleF CollisionBox { get { return collisionHandler.CollisionBox; } protected set { collisionHandler.CollisionBox = value; } }
        public RectangleF CurrentCollisionBox { get { return CollisionBox.At(Pos); } }

        public Collidable(Vector2 pos, RectangleF collisionBox)
        {
            collisionHandler = new(collisionBox);
            Pos = pos;
            //CollisionBox = collisionBox;
        }

        public Collidable(RectangleF currentCollisionBox)
        {
            Pos = new(currentCollisionBox.X, currentCollisionBox.Y);
            collisionHandler = new(new RectangleF(0f, 0f, currentCollisionBox.Width, currentCollisionBox.Height));
            //TODO remove all comments!!! pleas esee 
            //CollisionBox = new RectangleF(0f, 0f, currentCollisionBox.Width, currentCollisionBox.Height);
        }
    }
}
