using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Misc;
using System;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;


namespace MyGame.GameObjects
{
    internal class ObjectFactory
    {
        private ContentManager content;
        private Action<StationaryObject> remove;

        //todo: singleton ???
        public ObjectFactory(ContentManager content, Action<StationaryObject> remove)
        {
            this.content = content;
            this.remove = remove;
        }
        
        private Texture2D coinTexture;

        public StationaryObject CreateCoin(int tileX, int tileY)
        {
            if (coinTexture == null)
            {
                coinTexture = content.Load<Texture2D>("CoinSpriteSheet");
            }

            AnimationHandler animationHandler = new(coinTexture, new Vector2Int(8, 8));
            animationHandler.AddAnimation(State.Idling, 0, 8, 6);
            animationHandler.ChangeState(State.Idling);

            StationaryObject coin = new StationaryObject(new Vector2(tileX, tileY) * TileSize, new RectangleF(0, 0, TileSize, TileSize), animationHandler);
            coin.Touched = (collisionObject, normalVector) => 
            {
                if(collisionObject is Player)
                {
                    Player player = collisionObject as Player;
                    player.Score++;
                    remove.Invoke(coin);
                }
            };
            
            return coin;
        }

        public StationaryObject CreateRedCoin(int tileX, int tileY)
        {
            //TODO make red coin texture!!!
            if (coinTexture == null)
            {
                coinTexture = content.Load<Texture2D>("CoinSpriteSheet");
            }

            AnimationHandler animationHandler = new(coinTexture, new Vector2Int(8, 8));
            animationHandler.AddAnimation(State.Idling, 0, 8, 6);
            animationHandler.ChangeState(State.Idling);

            StationaryObject coin = new StationaryObject(new Vector2(tileX, tileY) * TileSize, new RectangleF(0, 0, TileSize, TileSize), animationHandler);
            coin.Touched = (collisionObject, normalVector) =>
            {
                if (collisionObject is Player)
                {
                    Player player = collisionObject as Player;
                    player.Score += 8;
                    remove.Invoke(coin);
                    Debug.WriteLine(player.Score);
                }
            };

            return coin;
        }
    }
}
