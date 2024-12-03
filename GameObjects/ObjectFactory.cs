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
        private Action<StationaryObject, int> remove;
        private TileType[,] tileMapCollisions;
        private List<StationaryObject> collidableEntities; //if entity is collidable, it will be added to this list
        public Player player;
        private Dictionary<string, Func<int, int, StationaryObject>> getEntityMethods = new(); //TODO: change Object to Entity (statinaryEntity)

        //todo: singleton ???
        public ObjectFactory(Player player, TileType[,] tileMapCollisions, List<StationaryObject> collidableEntities, ContentManager content, Action<StationaryObject, int> remove)
        {
            this.player = player;
            this.tileMapCollisions = tileMapCollisions;
            this.collidableEntities = collidableEntities;
            this.content = content;
            this.remove = remove;

            getEntityMethods.Add("player", (tileX, tileY) => getPlayer(tileX, tileY));
            getEntityMethods.Add("coin", (tileX, tileY) => getCoin(tileX, tileY));
            getEntityMethods.Add("red_coin", (tileX, tileY) => getRedCoin(tileX, tileY));
            getEntityMethods.Add("erik", (tileX, tileY) => getErik(tileX, tileY));
            getEntityMethods.Add("jellyfish", (tileX, tileY) => getJellyFish(tileX, tileY));
        }

        public StationaryObject GetEntity(string entityName, int tileX, int tileY)
        {
            return getEntityMethods[entityName](tileX, tileY);
        }

        private Dictionary<string, Texture2D> textureStore = new();
        private Texture2D playerTexture; //TODO: is needed?

        private Texture2D getTexture(string textureFileName)
        {
            //TODO: refactor to not use try catch
            try
            {
                return textureStore[textureFileName];
            }
            catch
            {
                textureStore.Add(textureFileName, content.Load<Texture2D>(textureFileName));
                return textureStore[textureFileName];
            }
        }

        private static bool IfPlayer(StationaryObject objectToCheck, out Player player)
        {
            if (objectToCheck is Player)
            {
                player = objectToCheck as Player;
                return true;
            }
            player = null;
            return false;
        }

        private Player getPlayer(int tileX, int tileY)
        {
            if (player == null)
            {
                playerTexture = content.Load<Texture2D>("PlayerSpriteSheet"); //TODO: name + spritesheet?

                player = new Player(new Vector2(tileX * TileSize, tileY * TileSize), playerTexture, new KeyboardReader(), tileMapCollisions, collidableEntities); //TODO: change input in settings
                return player;
            }
            throw new Exception("Cannot have 2 players!"); //TODO: fix ?
        }

        private StationaryObject getCoin(int tileX, int tileY)
        {
            Texture2D texture = getTexture("CoinSpriteSheet");
            
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
                    remove.Invoke(coin, 0);
                }
                return new();
            };
            
            return coin;
        }

        private StationaryObject getRedCoin(int tileX, int tileY)
        {
            //TODO make red coin texture!!!
            Texture2D texture = getTexture("RedCoinSpriteSheet");

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
                    remove.Invoke(coin, 0);
                }
                return new();
            };

            return coin;
        }

        private Enemy getErik(int tileX, int tileY)
        {
            return getErik(tileX, tileY, player);
        }
        private Enemy getErik(int tileX, int tileY, StationaryObject target)
        {
            //TODO: why does it dissappear sometimes when you hit it?
            Texture2D texture = getTexture("ErikSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(23,16));
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(0, 4, 4));
            animationHandler.AnimationStates.Add(State.Jumping, new AnimationState(1, 1, 1)); //TODO add jumping anim to erik sprite sheet
            animationHandler.AnimationStates.Add(State.Attacking, new AnimationState(2, 1, 16)); //8 so spikes are there for atleast 8 frames
            animationHandler.ChangeState(State.Walking);
            
            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom), tileMapCollisions, collidableEntities);

            Enemy erik = new(new Vector2(tileX * TileSize, tileY * TileSize), .5f, 6f, 16f, 2f, 1f, animationHandler, collisionHandler, target);//, behaviour, onTouch);

            erik.doBehaviour = new(() =>
            {
                erik.FaceInputDirection();
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
                    List<Vector2Int> horizontalCollisions = erik.collisionHandler.GetTileMapCollisions(erik.CurrentCollisionBox.At(new(erik.input.X * 8,0)));// erik.pos + new Vector2(erik.input.X * 8, 0));
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
            
            erik.Touched = new((collisionObject, touchNormal) =>
                {
                    if (collisionObject is Player)
                    {
                        Player player = collisionObject as Player;
                        player.DamageIfNotImmune();
                        animationHandler.PlayAnimation(State.Attacking);
                        //TODO: return velocity 
                    }
                    return new();
                }
            );
            
            return erik;
        }
        /*
        private Enemy getSnowballer(int tileX, int tileY)
        {
            return getSnowballer(tileX, tileY, player);
        }
        //TODO: enemy that throws projectiles
        private Enemy getSnowballer(int tileX, int tileY, StationaryObject target)
        {
            Texture2D texture = getTexture("SnowballerSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(23, 16));
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(0, 4, 4));
            animationHandler.ChangeState(State.Walking);

            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom), tileMapCollisions, collidableEntities);

            Enemy snowballer = new(new Vector2(tileX * TileSize, tileY * TileSize), .5f, 6f, 16f, 2f, 1f, animationHandler, collisionHandler, target);//, behaviour, onTouch);

            snowballer.doBehaviour = new(() =>
            {
                //movement    
                if (snowballer.input.X == 0)
                {
                    snowballer.acc.X = Math.Sign(snowballer.vel.X) * -1; //friction
                }
                else
                {
                    snowballer.acc.X = snowballer.input.X * snowballer.runAcc;
                }
                
                //jump
                if (snowballer.isGrounded)
                {
                    List<Vector2Int> horizontalCollisions = snowballer.collisionHandler.GetTileMapCollisions(snowballer.pos + new Vector2(snowballer.input.X * 8, 0));
                    foreach (Vector2Int collision in horizontalCollisions) //TODO: colissions -> tiles
                    {
                        if (snowballer.collisionHandler.TileMapCollisions.tryGetValue(collision, out TileType tileType) && (tileType == TileType.Solid)) //0 = air //|| tileType == TileType.SemiRight
                        {
                            snowballer.vel.Y = -snowballer.jumpPower;
                            break;
                        }
                    }
                }

                //animation
                if (snowballer.isGrounded)
                {
                    snowballer.animationHandler.ChangeState(State.Walking);
                }
                else
                {
                    snowballer.animationHandler.ChangeState(State.Jumping);
                }
            });

            snowballer.Touched = new((collisionObject, normalVector) =>
            {
                if (collisionObject is Player)
                {
                    Player player = collisionObject as Player;
                    player.DamageIfNotImmune();
                    animationHandler.PlayAnimation(State.Attacking);
                }
            }
            );

            return snowballer;
        }*/

        private Enemy getJellyFish(int tileX, int tileY)
        {
            return getJellyFish(tileX, tileY, player);
        }
        private Enemy getJellyFish(int tileX, int tileY, StationaryObject target)
        {
            Texture2D texture = getTexture("JellyFishSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(17, 24));
            animationHandler.AnimationStates.Add(State.Idling, new AnimationState(0, 5, 4));
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(1, 5, 4));
            animationHandler.AnimationStates.Add(State.Dying, new AnimationState(2, 6, 4));
            animationHandler.ChangeState(State.Idling);

            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 9f * Zoom, 7f * Zoom, 7f * Zoom), null, null);

            Enemy jellyFish = new(new Vector2(tileX * TileSize, tileY * TileSize), .5f, 2f, 0f, 0f, 0f, 2f, animationHandler, collisionHandler, target);

            jellyFish.doBehaviour = new(() =>
            {
                if (jellyFish.targetDirection.Length() <= 12 * TileSize && jellyFish.State != State.Dying)
                {
                    jellyFish.ChangeState(State.Walking);
                }

                if(jellyFish.State == State.Walking && jellyFish.targetDirection != new Vector2(0f,0f))
                {
                    jellyFish.acc = jellyFish.targetDirection.Normalized() * jellyFish.runAcc;
                }
            });

            jellyFish.Touched = new((collisionObject, touchNormal) =>
            {
                if (jellyFish.State != State.Dying)
                {
                    if (touchNormal.Y == -1 && collisionObject.CurrentCollisionBox.Bottom <= jellyFish.CurrentCollisionBox.Top) //hit on head
                    {
                        Player player;
                        if (IfPlayer(collisionObject, out player))
                        {
                            jellyFish.ChangeState(State.Dying);
                            //so the animation starts from 0
                            //TODO: isn't changing state but also is ??? weird.
                            //animationHandler.ChangeState(State.Dying); //TODO: use this function for erik too!

                            jellyFish.StopMoving();
                            collisionHandler = null;
                            remove.Invoke(jellyFish, 6 * 3); //TODO: why not 4??   getanimation length method!
                            return new Vector2(0, -12f); //TODO: for some reason player instantly goes back to previous velocity
                                                         //IS BECAUSE speed change happens during HandleCollisions, but this still returns the previous speed
                                                         //fix: have collisionhandler remember this velocity and return that ?? or use for collision
                                                         //remove after animation has played
                        }
                    }
                    else
                    {
                        Player player;
                        if (IfPlayer(collisionObject, out player))
                        {
                            player.DamageIfNotImmune();
                        }
                    }
                }
                return new();
            });

            return jellyFish;
        }
        /* floating enemy:
         * https://elthen.itch.io/2d-pixel-art-jellyfish-sprites
         * https://elthen.itch.io/2d-pixel-art-flying-eye-monster
         */
    }
}
