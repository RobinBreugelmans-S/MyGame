using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Interfaces
{
    internal interface IGameObject
    {
        void Update();
        void Draw(Vector2 offset, SpriteBatch spriteBatch);
    }
}
