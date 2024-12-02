using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Scenes
{
    internal interface IScene
    {
        //public IScene(string level, ContentManager content);
        public void LoadScene();
        public void Update();
        public void Draw(SpriteBatch spriteBatch);
    }
}
