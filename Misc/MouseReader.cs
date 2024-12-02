using Microsoft.Xna.Framework.Input;
using MyGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Misc
{
    internal class MouseReader : IInputReader
    {
        public Vector2Int ReadInput()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                int mouseX;
                int mouseY;
                Mouse.GetState().Position.Deconstruct(out mouseX, out mouseY);
                return new Vector2Int(mouseX, mouseY);
            }
            return new(); // 0,0
        }

        public bool ReadJumpInput() //TODO: revmove ?
        {
            throw new NotImplementedException();
        }
    }
}
