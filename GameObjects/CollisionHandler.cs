using Microsoft.Xna.Framework;
using MyGame.Misc;
using MyGame.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static MyGame.Globals;
using static MyGame.Misc.Methods;

namespace MyGame.GameObjects
{
    internal class CollisionHandler//TODO: erf over van iets met public pos enz.
    {
        public RectangleF CollisionBox;
        public TileType[,] TileMapCollisions; //TODO: change to CollisionTileMap !!
        public List<StationaryObject> CollidableEntities; 
        //protected List<Entity> entities; //TODO make entity class, IGameObject with pos vel acc, collision rectangle, and stuff
        //TODO: gravity in globals
        private Vector2 contactNormal;
        private Vector2 contactPoint;
        private float timeHitNear;

       // protected bool isGrounded;

        public CollisionHandler(RectangleF collisionBox , TileType[,] tileMapCollisions, List<StationaryObject> collidableEntities)
        {
            CollisionBox = collisionBox;
            TileMapCollisions = tileMapCollisions;
            CollidableEntities = collidableEntities;
        }

        protected Vector2 getMiddleOfRect(RectangleF rect)
        {
            return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
        private void swap<T>(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        //raycast interserct recangle
        private bool rayVsRect(Vector2 rayOrigin, Vector2 rayDir, RectangleF targetRect, out Vector2 contactPoint, out Vector2 contactNormal, out float timeHitNear)
        {
            //TODO: remove if unesecary
            contactPoint = this.contactPoint;
            contactNormal = this.contactNormal;
            timeHitNear = this.timeHitNear;

            Vector2 targetRectPos = new(targetRect.X, targetRect.Y);
            Vector2 targetRectSize = new(targetRect.Width, targetRect.Height);

            Vector2 near = (targetRectPos - rayOrigin) / rayDir;
            Vector2 far = (targetRectPos + targetRectSize - rayOrigin) / rayDir;

            if (float.IsNaN(near.X) || float.IsNaN(near.Y) || float.IsNaN(far.X) || float.IsNaN(far.Y))
            {
                return false;
            }

            //if far is closer than near
            if (near.X > far.X) { swap(ref near.X, ref far.X); }
            if (near.Y > far.Y) { swap(ref near.Y, ref far.Y); }

            //TODO: remove if unnecesarry
            if (near.X > far.Y || near.Y > far.X) { return false; }

            //TODO: remove when fixed calculation of timeHitNear
            timeHitNear = Math.Max(near.X, near.Y);
            float timeHitFar = Math.Min(far.X, far.Y); //TODO: remove if unused

            //TODO: remove if unecssecerya
            if (timeHitFar < 0) { return false; }

            contactPoint = rayOrigin + timeHitNear * rayDir;

            //calculate normal
            if (near.X > near.Y)
            {
                if (rayDir.X <= 0)
                {
                    contactNormal = new Vector2(1, 0);
                }
                else
                {
                    contactNormal = new Vector2(-1, 0);
                }
            }
            else
            {
                if (rayDir.Y < 0)
                {
                    contactNormal = new Vector2(0, 1);
                }
                else
                {
                    contactNormal = new Vector2(0, -1);
                }
            }
            return true;
        }
        private bool dynamicRectVsRect(RectangleF dynRect, Vector2 vel, RectangleF targetRect, out Vector2 contactPoint, out Vector2 contactNormal, out float timeHitNear) //TODO: contact_time? 
        {
            //TODO: remove if unesecary
            contactPoint = this.contactPoint;
            contactNormal = this.contactNormal;
            timeHitNear = this.timeHitNear;

            if (vel.X == 0 && vel.Y == 0)
            {
                return false;
            }

            RectangleF expandedTargetRect = new(targetRect.X - dynRect.Width / 2, targetRect.Y - dynRect.Height / 2,
                                                targetRect.Width + dynRect.Width, targetRect.Height + dynRect.Height);

            if (rayVsRect(getMiddleOfRect(dynRect), vel, expandedTargetRect, out contactPoint, out contactNormal, out timeHitNear))
            {
                if (timeHitNear <= 1f)
                {
                    return true;
                }
            }

            return false;
        }
        
        public List<Vector2Int> GetCollisions(Vector2 position)
        {
            //TODO: check for more
            //RectangleF currentCollisionBox = CollisionBox.At(pos);
            RectangleF projectedCollisionBox = CollisionBox.At(position);
            #region tilemap collisions
            //calculate amount of tiles to check for
            Vector2Int tilesToCheck = new(
                (int)(Math.Ceiling(projectedCollisionBox.Right / TileSize) - Math.Floor(projectedCollisionBox.Left / TileSize)),
                (int)(Math.Ceiling(projectedCollisionBox.Bottom / TileSize) - Math.Floor(projectedCollisionBox.Top / TileSize))
            );

            //get a list of tiles the player intersects
            List<Vector2Int> collisions = new(); //then check tiletype (int) in tileMap dictionary
            for (int x = 0; x < tilesToCheck.X; x++)
            {
                for (int y = 0; y < tilesToCheck.Y; y++)
                {
                    collisions.Add(new Vector2Int( //tilespace not worldspace
                        (int)((projectedCollisionBox.Left + x * TileSize) / TileSize),
                        (int)((projectedCollisionBox.Top + y * TileSize) / TileSize)
                    ));
                }
            }

            //filter out tiles that are outside of the map area (else it will cause lag)
            collisions = collisions.Where(c => c.X >= 0 && c.Y >= 0 && c.X < TileMapCollisions.GetLength(0) && c.Y < TileMapCollisions.GetLength(1)).ToList();
            #endregion

            #region entity collisions
            Vector2Int chunksToCheck = new(
                (int)(Math.Ceiling(projectedCollisionBox.Right / ChunkSize) - Math.Floor(projectedCollisionBox.Left / ChunkSize)),
                (int)(Math.Ceiling(projectedCollisionBox.Bottom / ChunkSize) - Math.Floor(projectedCollisionBox.Top / ChunkSize))
            );

            
            #endregion
            return collisions;
        }

        public (Vector2 vel, Vector2 acc, bool isGrounded) HandleCollisions(Vector2 pos, Vector2 vel, Vector2 acc)
        {
            bool isGrounded = false;
            contactNormal = new Vector2(0, 0);

            RectangleF currentCollisionBox = CollisionBox.At(pos);
            RectangleF projectedCollisionBox = CollisionBox.At(pos + vel);

            List<Vector2Int> collisions = GetCollisions(pos + vel);

            //sort collisions based on contactTime
            List<(RectangleF TileRect, float TimeHitNear, TileType TileType)> collisionsSorted = new();
            
            //check collisions and update velocity
            foreach (Vector2Int collision in collisions) //TODO: colissions -> tiles
            {
                if (TileMapCollisions.tryGetValue(collision, out TileType tileType) && tileType != TileType.None) //0 = air
                {
                    //raycast
                    RectangleF tileRect = new(collision.X * TileSize, collision.Y * TileSize, TileSize, TileSize);
                    if (dynamicRectVsRect(currentCollisionBox, vel, tileRect, out contactPoint, out contactNormal, out timeHitNear))
                    {
                        collisionsSorted.Add((tileRect, timeHitNear, tileType));
                    }
                }
            }

            //check entity collisions


            collisionsSorted = collisionsSorted.OrderBy(c => c.TimeHitNear).ThenBy(c => (getMiddleOfRect(c.TileRect) - getMiddleOfRect(currentCollisionBox)).Length()).ToList();
           
            foreach ((RectangleF TileRect, float TimeHitNear, TileType TileType) collision in collisionsSorted)
            {
                if (dynamicRectVsRect(currentCollisionBox, vel, collision.TileRect, out contactPoint, out contactNormal, out timeHitNear))
                {
                    //Action<Vector2> collisionMethod; TODO
                    //resolve collisions
                    switch (collision.TileType)
                    {
                        case TileType.Solid:
                            vel += contactNormal * new Vector2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1f - timeHitNear);
                            acc -= acc * contactNormal;
                            if(contactNormal == new Vector2(0, -1))
                            {
                                isGrounded = true;
                            }
                            //TODO: put these 2 in a method?
                            break;
                        case TileType.SemiUp:
                            if (contactNormal == new Vector2(0, -1) && currentCollisionBox.Bottom <= collision.TileRect.Top) //check if player is above (normal vector)
                            {
                                vel += contactNormal * new Vector2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1 - timeHitNear);
                                acc -= acc * contactNormal;
                                isGrounded = true;
                            }
                            break;
                        case TileType.SemiRight:
                            if (contactNormal == new Vector2(1, 0) && currentCollisionBox.Left >= collision.TileRect.Right) //check if player is above (normal vector)
                            {
                                vel += contactNormal * new Vector2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1 - timeHitNear);
                                acc -= acc * contactNormal;
                            }
                            break;
                        case TileType.SemiDown:
                            if (contactNormal == new Vector2(0, 1) && currentCollisionBox.Top >= collision.TileRect.Bottom) //check if player is above (normal vector)
                            {
                                vel += contactNormal * new Vector2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1 - timeHitNear);
                                acc -= acc * contactNormal;
                            }
                            break;
                        case TileType.SemiLeft:
                            if (contactNormal == new Vector2(-1, 0) && currentCollisionBox.Right <= collision.TileRect.Left) //check if player is above (normal vector)
                            {
                                vel += contactNormal * new Vector2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1 - timeHitNear);
                                acc -= acc * contactNormal;
                            }
                            break;
                    }
                }
            }

            return (vel,  acc, isGrounded);
        }
    }
}
