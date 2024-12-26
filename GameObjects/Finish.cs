using Microsoft.Xna.Framework;
using MyGame.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameObjects
{
    internal class Finish : Entity
    {
        public string NextLevel;
        public Finish(Vector2 pos, RectangleF collisionBox, AnimationHandler animationHandler) : base(pos, collisionBox, animationHandler)
        {}
    }
}
