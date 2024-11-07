using MyGame.Misc;

namespace MyGame.Interfaces
{
    internal interface IInputReader
    {
        Vector2Int ReadInput();
        bool ReadJumpInput();
    }
}
