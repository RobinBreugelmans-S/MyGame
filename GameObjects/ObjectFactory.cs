using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.Misc;
using MyGame.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static MyGame.Globals;


namespace MyGame.GameObjects
{
    internal class ObjectFactory
    {
        private ContentManager content;
        private Action<StationaryObject> remove;
        private TileType[,] tileMapCollisions;
        private List<StationaryObject> collidableEntities; //if entity is collidable, it will be added to this list
        public Player player;

        //todo: singleton ???
        public ObjectFactory(Player player, TileType[,] tileMapCollisions, List<StationaryObject> collidableEntities, ContentManager content, Action<StationaryObject> remove)
        {
            this.player = player;
            this.tileMapCollisions = tileMapCollisions;
            this.collidableEntities = collidableEntities;
            this.content = content;
            this.remove = remove;
        }

        //TODO: make dictionary with texture name and texture2D
        private Dictionary<string, Texture2D> textureStore = new();
        private Texture2D redCoinTexture;
        private Texture2D erikTexture;
        private Texture2D playerTexture;

        private void getTexture(string textureFileName, out Texture2D texture)
        {
            try
            {
                texture = textureStore[textureFileName];
            }
            catch
            {
                textureStore.Add(textureFileName, content.Load<Texture2D>(textureFileName));
                texture = textureStore[textureFileName];
            }
        }

        public Player GetPlayer(int tileX, int tileY)
        {
            if (player == null)
            {
                playerTexture = content.Load<Texture2D>("PlayerSpriteSheet"); //TODO: name + spritesheet?

                player = new Player(new Vector2(tileX * TileSize, tileY * TileSize), playerTexture, new KeyboardReader(), tileMapCollisions, collidableEntities); //TODO: change input in settings
                Debug.WriteLine("GetPlayer");
                Debug.WriteLine(player);
                return player;
            }
            throw new Exception("Cannot have 2 players!"); //TODO: fix ?
        }

        public StationaryObject GetCoin(int tileX, int tileY) //change to GetCoin
        {
            Texture2D texture;
            getTexture("CoinSpriteSheet", out texture);
            
            AnimationHandler animationHandler = new(texture, new Vector2Int(8, 8));
            animationHandler.AddAnimation(State.Idling, 0, 8, 6);
            animationHandler.ChangeState(State.Idling);

            StationaryObject coin = new StationaryObject(new Vector2(tileX, tileY) * TileSize, new RectangleF(0, 0, TileSize, TileSize), animationHandler);
           
            //change to have declare onTouch, like in GetErik
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

        public StationaryObject GetRedCoin(int tileX, int tileY)
        {
            //TODO make red coin texture!!!
            Texture2D texture;
            getTexture("RedCoinSpriteSheet", out texture);

            AnimationHandler animationHandler = new(texture, new Vector2Int(8, 8));
            animationHandler.AddAnimation(State.Idling, 0, 8, 6);
            animationHandler.ChangeState(State.Idling);

            StationaryObject coin = new StationaryObject(new Vector2(tileX, tileY) * TileSize, new RectangleF(0, 0, TileSize, TileSize), animationHandler);
            coin.Touched = (collisionObject, normalVector) =>
            {
                if (collisionObject is Player)
                {
                    Player player = collisionObject as Player;
                    player.Score += 675034;
                    remove.Invoke(coin);
                }
            };

            return coin;
        }
        
        public Enemy GetErik(int tileX, int tileY)
        {
            return GetErik(tileX, tileY, player);
        }

        public Enemy GetErik(int tileX, int tileY, StationaryObject target)//, StationaryObject target)
        {
            Texture2D texture;
            getTexture("ErikSpriteSheet", out texture);

            AnimationHandler animationHandler = new(texture, new Vector2Int(23,16));
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(0, 4, 4));
            animationHandler.AnimationStates.Add(State.Jumping, new AnimationState(1, 1, 1)); //TODO add jumping anim to erik sprite sheet
            animationHandler.AnimationStates.Add(State.Attacking, new AnimationState(2, 1, 16)); //8 so spikes are there for atleast 8 frames
            animationHandler.ChangeState(State.Walking);
            
            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom), tileMapCollisions, collidableEntities);

            Enemy erik = new(new Vector2(tileX * TileSize, tileY * TileSize), 1f, 6f, 16f, 2f, 1f, animationHandler, collisionHandler, target);//, behaviour, onTouch);
            
            erik.doBehaviour = new(() =>
            {
                //movement    
                if (erik.input.X == 0)
                {
                    erik.acc.X = Math.Sign(erik.vel.X) * -1; //friction
                }
                else
                {
                    erik.acc.X = erik.input.X * erik.runAcc;
                }

                //jump
                if (erik.isGrounded)
                {
                    List<Vector2Int> horizontalCollisions = erik.collisionHandler.GetTileMapCollisions(erik.pos + new Vector2(erik.input.X * 8, 0));
                    foreach (Vector2Int collision in horizontalCollisions) //TODO: colissions -> tiles
                    {
                        if (erik.collisionHandler.TileMapCollisions.tryGetValue(collision, out TileType tileType) && (tileType == TileType.Solid)) //0 = air //|| tileType == TileType.SemiRight
                        {
                            erik.vel.Y = -erik.jumpPower;
                            break;
                        }
                    }
                }

                //animation
                if(erik.isGrounded)
                {
                    erik.animationHandler.ChangeState(State.Walking);
                }
                else
                {
                    erik.animationHandler.ChangeState(State.Jumping);
                }
            });
            
            erik.Touched = new((collisionObject, normalVector) =>
                {
                    if (collisionObject is Player)
                    {
                        Player player = collisionObject as Player;
                        player.DamageIfNotImmune();
                        animationHandler.PlayAnimation(State.Attacking);
                    }
                }
            );
            
            return erik;
        }
    }
}
