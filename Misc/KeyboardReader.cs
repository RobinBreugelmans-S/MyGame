using MyGame.Misc;
using MyGame.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame.Misc
{
    internal class KeyboardReader : IInputReader
    {
        Vector2Int IInputReader.ReadInput()
        {
            Vector2Int input = new(0,0);
            if (!Keyboard.GetState().IsKeyDown(Keys.Q) && Keyboard.GetState().IsKeyDown(Keys.D)) //TODO: add custom controls
            {
                input.X = 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Q) && !Keyboard.GetState().IsKeyDown(Keys.D))
            {
                input.X = -1;
            }
            else
            {
                input.X = 0;
            }

            if (!Keyboard.GetState().IsKeyDown(Keys.Z) && Keyboard.GetState().IsKeyDown(Keys.S)) //TODO: add custom controls
            {
                input.Y = 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Z) && !Keyboard.GetState().IsKeyDown(Keys.S))
            {
                input.Y = -1;
            }
            else
            {
                input.Y = 0;
            }
            return input;
        }
        bool IInputReader.ReadJumpInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
