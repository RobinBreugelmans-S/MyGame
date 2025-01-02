using MyGame.Misc;

namespace MyGame.Input
{
    internal interface IInputReader
    {
        public Vector2Int ReadInput();
        public bool ReadJumpInput();
        public bool ReadEnterInput();
    }
}
