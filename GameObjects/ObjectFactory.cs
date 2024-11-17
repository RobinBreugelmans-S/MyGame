using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Misc;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;


namespace MyGame.GameObjects
{
    internal class ObjectFactory
    {
        private ContentManager content;

        //todo: singleton ???
        public ObjectFactory(ContentManager content)
        {
            this.content = content;
        }
        
        private Texture2D coinTexture;
        
        public StationaryObject CreateCoin(int tileX, int tileY)
        {
            if(coinTexture == null)
            {
                coinTexture = content.Load<Texture2D>("CoinSpriteSheet");
            }

            AnimationHandler animationHandler = new(coinTexture, new Vector2Int(8, 8));
            animationHandler.AddAnimation(State.Idling, 0, 8, 6);
            animationHandler.ChangeState(State.Idling);

            return new StationaryObject(new Vector2(tileX, tileY) * TileSize, new RectangleF(0, 0, TileSize, TileSize), animationHandler, (collisionObject, normalVector) => Debug.WriteLine("COIN HIT!!"));
        }
    }
}
