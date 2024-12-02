using MyGame.Misc;

namespace MyGame.Interfaces
{
    internal interface IInputReader
    {
        public Vector2Int ReadInput();
        public bool ReadJumpInput();
    }
}
