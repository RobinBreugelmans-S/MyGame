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
    internal class EntityFactory
    {
        //TODO: use Decorator pattern to define behaviours of the entities.

        private ContentManager content;
        private Action<Entity, int> remove;
        private Action<Entity> add;
        private Action<string> loadScene;
        private TileType[,] tileMapCollisions;
        private List<Entity> collidableEntities; //if entity is collidable, it will be added to this list
        public Player player;
        private Dictionary<string, Func<int, int, Entity>> getEntityMethods = new(); //TODO: change Object to Entity (statinaryEntity)
        
        public EntityFactory(Player player, TileType[,] tileMapCollisions, List<Entity> collidableEntities, ContentManager content, Action<Entity, int> remove, Action<Entity> add, Action<string> loadScene)
        {
            this.player = player;
            this.tileMapCollisions = tileMapCollisions;
            this.collidableEntities = collidableEntities;
            this.content = content;
            this.remove = remove;
            this.add = add;
            this.loadScene = loadScene;

            getEntityMethods.Add("player", (x, y) => getPlayer(x, y));
            getEntityMethods.Add("finish", (x, y) => getFinish(x, y));
            getEntityMethods.Add("coin", (x, y) => getCoin(x, y));
            getEntityMethods.Add("erik", (x, y) => getErik(x, y));
            getEntityMethods.Add("jellyfish", (x, y) => getJellyFish(x, y));
            getEntityMethods.Add("jones", (x, y) => getJones(x, y));
        }

        public Entity GetEntity(string entityName, int x, int y)
        {
            return getEntityMethods[entityName](x, y);
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

        //TODO: rename
        private static bool IsPlayer(Entity objectToCheck, out Player player)
        {
            if (objectToCheck is Player)
            {
                player = objectToCheck as Player;
                return true;
            }
            player = null;
            return false;
        }

        private Player getPlayer(int x, int y)
        {
            if (player == null)
            {
                playerTexture = content.Load<Texture2D>("PlayerSpriteSheet"); //TODO: name + spritesheet?

                player = new Player(new Vector2(x, y), playerTexture, new KeyboardReader(), tileMapCollisions, collidableEntities); //TODO: change input in settings
                return player;
            }
            throw new Exception("Cannot have 2 players!"); //TODO: fix ?
        }

        private Entity getCoin(int x, int y)
        {
            Texture2D texture = getTexture("CoinSpriteSheet");
            
            AnimationHandler animationHandler = new(texture, new Vector2Int(8, 8));
            animationHandler.AddAnimation(State.Idling, 0, 8, 6);
            animationHandler.ChangeState(State.Idling);

            Entity coin = new Entity(new Vector2(x, y), new RectangleF(0, 0, TileSize, TileSize), animationHandler);
           
            //change to have declare onTouch, like in GetErik
            coin.Touched = (collisionObject, normalVector) => 
            {
                if(collisionObject is Player)
                {
                    Player player = collisionObject as Player;
                    player.Score++;
                    remove(coin, 0);
                }
                return new();
            };
            
            return coin;
        }

        private Enemy getErik(int x, int y)
        {
            return getErik(x, y, player);
        }
        private Enemy getErik(int x, int y, Entity target)
        {
            //TODO: why does it dissappear sometimes when you hit it?
            Texture2D texture = getTexture("ErikSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(23,16));
            animationHandler.AnimationStates.Add(State.Idling, new AnimationState(0, 4, 16));
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(0, 4, 4));
            animationHandler.AnimationStates.Add(State.Jumping, new AnimationState(1, 1, 1)); //TODO add jumping anim to erik sprite sheet
            animationHandler.AnimationStates.Add(State.Attacking, new AnimationState(2, 1, 16)); //8 so spikes are there for atleast 8 frames
            animationHandler.ChangeState(State.Idling);
            
            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom), tileMapCollisions, collidableEntities);

            Enemy erik = new(new Vector2(x, y), .5f, 6f, 16f, 2f, 1f, animationHandler, collisionHandler, target);//, behaviour, onTouch);

            erik.doBehaviour = new(() =>
            {
                if (erik.targetDirection.Length() <= 24 * TileSize)
                {
                    erik.ChangeState(State.Walking);
                }
                //if is activated //TODO: bool activated
                if (erik.State == State.Walking && erik.targetDirection != new Vector2(0f, 0f))
                {
                    erik.FaceInputDirection();
                    
                    //TODO: put in method, is same as the player
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
                        List<Vector2Int> horizontalCollisions = erik.collisionHandler.GetTileMapCollisions(erik.CurrentCollisionBox.At(new(erik.input.X * 8, 0)));// erik.pos + new Vector2(erik.input.X * 8, 0));
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
                    if (erik.isGrounded)
                    {
                        erik.animationHandler.ChangeState(State.Walking);
                    }
                    else
                    {
                        erik.animationHandler.ChangeState(State.Jumping);
                    }
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
        
        private Enemy getJellyFish(int x, int y)
        {
            return getJellyFish(x, y, player);
        }
        private Enemy getJellyFish(int x, int y, Entity target)
        {
            Texture2D texture = getTexture("JellyFishSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(17, 24));
            animationHandler.AnimationStates.Add(State.Idling, new AnimationState(0, 5, 4));
            animationHandler.AnimationStates.Add(State.Walking, new AnimationState(1, 5, 4));
            animationHandler.AnimationStates.Add(State.Dying, new AnimationState(2, 6, 4));
            animationHandler.ChangeState(State.Idling);

            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 9f * Zoom, 7f * Zoom, 7f * Zoom), null, null);

            Enemy jellyFish = new(new Vector2(x, y), .5f, 2f, 0f, 0f, 0f, 2f, animationHandler, collisionHandler, target);

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
                        if (IsPlayer(collisionObject, out player))
                        {
                            player.Score += 8;

                            jellyFish.PlayAnimation(State.Dying);

                            jellyFish.StopMoving();
                            collisionHandler = null;
                            remove(jellyFish, animationHandler.GetAnimationTime(State.Dying)); //*3 //TODO: why not 4?
                            return new Vector2(0, -12f);
                        }
                    }
                    else
                    {
                        Player player;
                        if (IsPlayer(collisionObject, out player))
                        {
                            player.DamageIfNotImmune();
                        }
                    }
                }
                return new();
            });

            return jellyFish;
        }

        private Enemy getJones(int x, int y)
        {
            return getJones(x, y, player);
        }
        private Enemy getJones(int x, int y, Entity target)
        {
            //TODO: why does it dissappear sometimes when you hit it?
            Texture2D texture = getTexture("JonesSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(64, 32));
            animationHandler.AddAnimation(State.Idling, 0, 8, 8);
            animationHandler.AddAnimation(State.Walking, 1, 8, 4);
            animationHandler.AddAnimation(State.Jumping, 1, 1, 1);
            animationHandler.AddAnimation(State.Attacking, 2, 4, 4);
            animationHandler.AddAnimation(State.Dying, 3, 5, 6);
            animationHandler.ChangeState(State.Idling);

            CollisionHandler collisionHandler = new(new RectangleF(27f * Zoom, 14f * Zoom, 9f * Zoom, 18f * Zoom), tileMapCollisions, collidableEntities);

            Enemy jones = new(new Vector2(x, y), .5f, 5f, 20f, 2f, 1f, animationHandler, collisionHandler, target);//, behaviour, onTouch);

            jones.doBehaviour = new(() =>
            {
                jones.FaceInputDirection();

                if(jones.targetDirection.Length() <= 32 * TileSize
                && jones.targetDirection.Length() > 8 * TileSize)
                {
                    jones.ChangeState(State.Walking);
                }
                else if (jones.targetDirection.Length() <= 8 * TileSize)
                {
                    jones.ChangeState(State.Attacking);
                    jones.vel.X = 0;
                    jones.acc.X = 0;
                }
                else
                {
                    jones.ChangeState(State.Idling);
                    jones.vel.X = 0;
                    jones.acc.X = 0;
                }

                if (jones.State != State.Idling && jones.State != State.Attacking && jones.targetDirection != new Vector2(0f, 0f))
                {
                    if (jones.input.X == 0)
                    {
                        jones.acc.X = Math.Sign(jones.vel.X) * -1;
                    }
                    else
                    {
                        jones.acc.X = jones.input.X * jones.runAcc;
                    }

                    //jump
                    if (jones.isGrounded)
                    {
                        List<Vector2Int> horizontalCollisions = jones.collisionHandler.GetTileMapCollisions(jones.CurrentCollisionBox.At(new(jones.input.X * 8, 0)));
                        foreach (Vector2Int collision in horizontalCollisions) //TODO: colissions -> tiles
                        {
                            if (jones.collisionHandler.TileMapCollisions.tryGetValue(collision, out TileType tileType) && (tileType == TileType.Solid))
                            {
                                jones.vel.Y = -jones.jumpPower;
                                break;
                            }
                        }
                    }
                    
                    if (!jones.isGrounded)
                    {
                        jones.animationHandler.ChangeState(State.Jumping);
                    }
                }
                else
                {
                    jones.acc.X = Math.Sign(jones.vel.X) * -1;
                }

                if(jones.State == State.Attacking && jones.animationHandler.GetCurrentAnimationFrame() == 1 && jones.animationHandler.AnimationTimer % 4 == 0)
                {
                    add(getBullet((int)(jones.pos.X + (29.5f + jones.facingDirection * 12.5f) * Zoom), (int)jones.pos.Y + 19 * Zoom, jones.facingDirection));
                }
            });

            jones.Touched = new((collisionObject, touchNormal) =>
            {
                if (jones.State != State.Dying)
                {
                    if (touchNormal.Y == -1 && collisionObject.CurrentCollisionBox.Bottom <= jones.CurrentCollisionBox.Top) //hit on head
                    {
                        Player player;
                        if (IsPlayer(collisionObject, out player))
                        {
                            player.Score += 6;

                            jones.PlayAnimation(State.Dying);

                            collisionHandler = null;
                            remove(jones, animationHandler.GetAnimationTime(State.Dying));
                            return new Vector2(0, -12f);
                        }
                    }
                    else
                    {
                        Player player;
                        if (IsPlayer(collisionObject, out player))
                        {
                            player.DamageIfNotImmune();
                        }
                    }
                }
                return new();
            });

            return jones;
        }
        
        private MoveableEntity getBullet(int x, int y, int direction)
        {
            Texture2D texture = getTexture("BulletTexture");

            AnimationHandler animationHandler = new(texture, new(5, 3));
            animationHandler.AddAnimation(State.Idling, 0, 1, 1);
            animationHandler.AddAnimation(State.Dying, 1, 2, 4);

            CollisionHandler collisionHandler = new(new(0,0,5 * Zoom, 3 * Zoom), null, null);

            MoveableEntity bullet = new(new(x, y), new(direction * .1f * Zoom, 0), 0, animationHandler, collisionHandler, null);

            bullet.FaceDirection(direction);

            bullet.Touched = new((collisionObject, touchNormal) =>
            {
                if (bullet.State != State.Dying)
                {
                    if (touchNormal.Y == -1 && collisionObject.CurrentCollisionBox.Bottom <= bullet.CurrentCollisionBox.Top) //hit on head
                    {
                        Player player;
                        if (IsPlayer(collisionObject, out player))
                        {
                            bullet.PlayAnimation(State.Dying);
                            collisionHandler = null;
                            remove(bullet, animationHandler.GetAnimationTime(State.Dying));
                            return new Vector2(0, -12f);
                        }
                    }
                    else
                    {
                        Player player;
                        if (IsPlayer(collisionObject, out player))
                        {
                            player.DamageIfNotImmune();
                        }
                    }
                }
                return new();
            });

            return bullet;
        }

        private Finish getFinish(int x, int y)
        {
            Texture2D texture = getTexture("FinishSpriteSheet");

            AnimationHandler animationHandler = new(texture, new(16, 16));
            animationHandler.AddAnimation(State.Idling, 0, 4, 4);

            Finish finish = new(new(x, y), new(0, 0, 16 * Zoom, 16 * Zoom), animationHandler);

            finish.Touched = new((collisionObject, touchNormal) => {
                Player player;
                if (IsPlayer(collisionObject, out player))
                {
                    loadScene(finish.NextLevel);
                }
                return new();
            });

            return finish;
        }
    }
}
