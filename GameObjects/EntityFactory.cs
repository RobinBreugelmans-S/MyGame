using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Animation;
using MyGame.GameObjects.LevelObjects;
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
        public Player Player;
        private Dictionary<string, Func<int, int, Entity>> getEntityMethods = new();
        
        public EntityFactory(Player player, TileType[,] tileMapCollisions, List<Entity> collidableEntities, ContentManager content, Action<Entity, int> remove, Action<Entity> add, Action<string> loadScene)
        {
            this.Player = player;
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
            if (Player == null)
            {
                playerTexture = content.Load<Texture2D>("PlayerSpriteSheet"); //TODO: name + spritesheet?

                Player = new Player(new Vector2(x, y), playerTexture, new KeyboardReader(), tileMapCollisions, collidableEntities); //TODO: change input in settings
                return Player;
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
            
            coin.OnTouch = (collisionObject, normalVector) => 
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
            return getErik(x, y, Player);
        }
        private Enemy getErik(int x, int y, Entity target)
        {
            Texture2D texture = getTexture("ErikSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(23,16));
            animationHandler.AddAnimation(State.Idling, 0, 4, 16);
            animationHandler.AddAnimation(State.Walking,0, 4, 4);
            animationHandler.AddAnimation(State.Jumping,1, 1, 1); //TODO add jumping anim to erik sprite sheet
            animationHandler.AddAnimation(State.Attacking, 2, 1, 16); //8 so spikes are there for atleast 8 frames
            animationHandler.ChangeState(State.Idling);
            
            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 8f * Zoom, 12f * Zoom, 8f * Zoom), tileMapCollisions, collidableEntities);

            Enemy erik = new(new Vector2(x, y), .5f, 6f, 16f, 2f, 1f, animationHandler, collisionHandler, target);//, behaviour, onTouch);

            erik.DoBehaviour = new(() =>
            {
                if (erik.targetDirection.Length() <= 24 * TileSize)
                {
                    erik.ChangeState(State.Walking);
                }
                //if is activated //TODO: bool activated
                if (erik.State != State.Idling && erik.targetDirection != new Vector2(0f, 0f))
                {
                    erik.FaceInputDirection();
                    
                    //TODO: put in method, is same as the player
                    //movement    
                    if (erik.input.X == 0)
                    {
                        erik.Acc.X = Math.Sign(erik.Vel.X) * -1; //friction
                    }
                    else
                    {
                        erik.Acc.X = erik.input.X * erik.RunAcc;
                    }

                    //jump
                    if (erik.IsGrounded)
                    {
                        List<Vector2Int> horizontalCollisions = erik.CollisionHandler.GetTileMapCollisions(erik.CurrentCollisionBox.At(new(erik.input.X * 8, 0)));// erik.pos + new Vector2(erik.input.X * 8, 0));
                        foreach (Vector2Int collision in horizontalCollisions) //TODO: colissions -> tiles
                        {
                            if (erik.CollisionHandler.TileMapCollisions.tryGetValue(collision, out TileType tileType) && (tileType == TileType.Solid)) //0 = air //|| tileType == TileType.SemiRight
                            {
                                erik.Vel.Y = -erik.JumpPower;
                                break;
                            }
                        }
                    }

                    //animation
                    if (erik.IsGrounded)
                    {
                        erik.AnimationHandler.ChangeState(State.Walking);
                    }
                    else
                    {
                        erik.AnimationHandler.ChangeState(State.Jumping);
                    }
                }
            });
            
            erik.OnTouch = new((collisionObject, touchNormal) =>
                {
                    if (collisionObject is Player)
                    {
                        Player player = collisionObject as Player;
                        player.DamageIfNotImmune();
                        animationHandler.PlayAnimation(State.Attacking);
                    }
                    return new();
                }
            );
            
            return erik;
        }
        
        private Enemy getJellyFish(int x, int y)
        {
            return getJellyFish(x, y, Player);
        }
        private Enemy getJellyFish(int x, int y, Entity target)
        {
            Texture2D texture = getTexture("JellyFishSpriteSheet");

            AnimationHandler animationHandler = new(texture, new Vector2Int(17, 24));
            animationHandler.AddAnimation(State.Idling, 0, 5, 4);
            animationHandler.AddAnimation(State.Walking, 1, 5, 4);
            animationHandler.AddAnimation(State.Dying, 2, 6, 4);
            animationHandler.ChangeState(State.Idling);

            CollisionHandler collisionHandler = new(new RectangleF(5f * Zoom, 9f * Zoom, 7f * Zoom, 7f * Zoom), null, null);

            Enemy jellyFish = new(new Vector2(x, y), .5f, 2f, 0f, 0f, 0f, 2f, animationHandler, collisionHandler, target);

            jellyFish.DoBehaviour = new(() =>
            {
                if (jellyFish.targetDirection.Length() <= 12 * TileSize && jellyFish.State != State.Dying)
                {
                    jellyFish.ChangeState(State.Walking);
                }

                if(jellyFish.State == State.Walking && jellyFish.targetDirection != new Vector2(0f,0f))
                {
                    jellyFish.Acc = jellyFish.targetDirection.Normalized() * jellyFish.RunAcc;
                }
            });

            jellyFish.OnTouch = new((collisionObject, touchNormal) =>
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
            return getJones(x, y, Player);
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

            jones.DoBehaviour = new(() =>
            {
                jones.FaceInputDirection();

                if(jones.targetDirection.Length() <= 24 * TileSize
                && jones.targetDirection.Length() > 8 * TileSize)
                {
                    jones.ChangeState(State.Walking);
                }
                else if (jones.targetDirection.Length() <= 8 * TileSize)
                {
                    jones.ChangeState(State.Attacking);
                    jones.Vel.X = 0;
                    jones.Acc.X = 0;
                }
                else
                {
                    jones.ChangeState(State.Idling);
                    jones.Vel.X = 0;
                    jones.Acc.X = 0;
                }

                if (jones.State != State.Idling && jones.State != State.Attacking && jones.targetDirection != new Vector2(0f, 0f))
                {
                    if (jones.input.X == 0)
                    {
                        jones.Acc.X = Math.Sign(jones.Vel.X) * -1;
                    }
                    else
                    {
                        jones.Acc.X = jones.input.X * jones.RunAcc;
                    }

                    //jump
                    if (jones.IsGrounded)
                    {
                        List<Vector2Int> horizontalCollisions = jones.CollisionHandler.GetTileMapCollisions(jones.CurrentCollisionBox.At(new(jones.input.X * 8, 0)));
                        foreach (Vector2Int collision in horizontalCollisions) //TODO: colissions -> tiles
                        {
                            if (jones.CollisionHandler.TileMapCollisions.tryGetValue(collision, out TileType tileType) && (tileType == TileType.Solid))
                            {
                                jones.Vel.Y = -jones.JumpPower;
                                break;
                            }
                        }
                    }
                    
                    if (!jones.IsGrounded)
                    {
                        jones.AnimationHandler.ChangeState(State.Jumping);
                    }
                }
                else
                {
                    jones.Acc.X = Math.Sign(jones.Vel.X) * -1;
                }

                if(jones.State == State.Attacking && jones.AnimationHandler.GetCurrentAnimationFrame() == 1 && jones.AnimationHandler.AnimationTimer % 4 == 0)
                {
                    add(getBullet((int)(jones.Pos.X + (29.5f + jones.FacingDirection * 12.5f) * Zoom), (int)jones.Pos.Y + 19 * Zoom, jones.FacingDirection));
                }
            });

            jones.OnTouch = new((collisionObject, touchNormal) =>
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

            bullet.OnTouch = new((collisionObject, touchNormal) =>
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
                            //TODO: bullet doesn't get removed for some reason
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

            finish.OnTouch = new((collisionObject, touchNormal) => {
                Player player;
                if (IsPlayer(collisionObject, out player) && player.HP > 0)
                {
                    loadScene(finish.NextLevel);
                }
                return new();
            });

            return finish;
        }
    }
}
