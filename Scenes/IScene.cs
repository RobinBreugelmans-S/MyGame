using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Scenes
{
    internal interface IScene
    {
        public void LoadScene();
        public void Update();
        public void Draw(SpriteBatch spriteBatch);
    }
}
