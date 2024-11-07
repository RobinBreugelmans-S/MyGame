using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Interfaces
{
    internal interface IMoveable
    {
        Vector2 pos { get; set; }
        Vector2 vel { get; set; }
        Vector2 acc { get; set; }
    }
}
