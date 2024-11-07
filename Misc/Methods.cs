using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Misc
{
    static class Methods
    {
        //TODO: make not extension function
        public static RectangleF At(this RectangleF a, Vector2 b)
        {
            return new RectangleF(a.X + b.X, a.Y + b.Y, a.Width, a.Height);
        }
    }
}
