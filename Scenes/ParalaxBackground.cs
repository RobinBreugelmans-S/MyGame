using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Globals;

namespace MyGame.Scenes
{
    internal class ParalaxBackground
    {
        Texture2D backgroundTexture;
        float paralaxStrength;
        public ParalaxBackground(Texture2D texture, float paralaxStrength)
        {
            backgroundTexture = texture;
            this.paralaxStrength = paralaxStrength;
        }
        public void Draw(Vector2 cameraPos, SpriteBatch spritebatch)
        {
            //for infinite tiling of moving bg
            int posX = (int)(cameraPos.X * paralaxStrength) % BufferSize.X;
            spritebatch.Draw(backgroundTexture, new Rectangle(posX, 0, BufferSize.X, BufferSize.Y), Color.White);
            spritebatch.Draw(backgroundTexture, new Rectangle(posX + BufferSize.X * Math.Sign(-paralaxStrength), 0, BufferSize.X, BufferSize.Y), Color.White);
                                                                //TODO: sign from paralax stregghthng
        }
    }
}
