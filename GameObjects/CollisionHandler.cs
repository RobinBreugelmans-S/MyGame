﻿using Microsoft.Xna.Framework;
using MyGame.GameObjects.LevelObjects;
using MyGame.Misc;
using MyGame.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static MyGame.Globals;

namespace MyGame.GameObjects
{
    internal class CollisionHandler//TODO: erf over van iets met public pos enz.
    {
        public MoveableEntity Parent { private get; set; }

        public RectangleF CollisionBox;
        public TileType[,] TileMapCollisions;
        public List<Entity> CollidableEntities;
        //protected List<Entity> entities; //TODO make entity class, IGameObject with pos vel acc, collision rectangle, and stuff
        //TODO: gravity in globals
        private Vector2 contactNormal;
        private Vector2 contactPoint;
        private float timeHitNear;

        //protected bool isGrounded;
        public CollisionHandler(RectangleF collisionBox)
        {
            CollisionBox = collisionBox;
        }
        public CollisionHandler(/*MoveableObject parent, */RectangleF collisionBox, TileType[,] tileMapCollisions, List<Entity> collidableEntities)
        {
            //this.parent = parent;
            CollisionBox = collisionBox;
            TileMapCollisions = tileMapCollisions;
            CollidableEntities = collidableEntities;
        }
        
        private void swap<T>(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }
        private RectangleF getBoundingBox(params RectangleF[] rects)
        {
            float xMin = rects.Min(s => s.Left);
            float yMin = rects.Min(s => s.Top);
            float xMax = rects.Max(s => s.Right);
            float yMax = rects.Max(s => s.Bottom);
            RectangleF boundingRect = new(xMin, yMin, xMax - xMin, yMax - yMin);
            return boundingRect;
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
            if (near.X >= near.Y)
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
                if (rayDir.Y <= 0)
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

            if (rayVsRect(GetMiddleOfRect(dynRect), vel, expandedTargetRect, out contactPoint, out contactNormal, out timeHitNear))
            {
                if (timeHitNear <= 1f)
                {
                    return true;
                }
            }

            return false;
        }
        
        public List<Vector2Int> GetTileMapCollisions(RectangleF rectangle)
        {
            //calculate amount of tiles to check for
            Vector2Int tilesToCheck = new(
                (int)(Math.Ceiling(rectangle.Right / TileSize) - Math.Floor(rectangle.Left / TileSize)),
                (int)(Math.Ceiling(rectangle.Bottom / TileSize) - Math.Floor(rectangle.Top / TileSize))
            );

            //get a list of tiles the player intersects
            List<Vector2Int> collisions = new(); //then check tiletype (int) in tileMap dictionary
            for (int x = 0; x < tilesToCheck.X; x++)
            {
                for (int y = 0; y < tilesToCheck.Y; y++)
                {
                    collisions.Add(new Vector2Int( //tilespace not worldspace
                        (int)((rectangle.Left + x * TileSize) / TileSize),
                        (int)((rectangle.Top + y * TileSize) / TileSize)
                    ));
                }
            }

            //filter out tiles that are outside of the map area (else it will cause lag) TODO: may be because of the errors being logged in Debug
            collisions = collisions.Where(c => c.X >= 0 && c.Y >= 0 && c.X < TileMapCollisions.GetLength(0) && c.Y < TileMapCollisions.GetLength(1)).ToList();
            
            return collisions;
        }

        public List<Entity> GetEntityCollisions(RectangleF rectangle)
        {
            //TODO: problem collision is checked for object itself
            //temp fix:
            if (!(Parent is Player))
            {
                return new();
            }

            List<Entity> entities = new();
            
            foreach (Entity entity in CollidableEntities)
            {
                if (rectangle.IntersectsWith(entity.CurrentCollisionBox) && entity != Parent)//.At(entity.pos)))
                {
                    entities.Add(entity);
                }
            }
            
            return entities;
        }

        public (Vector2 vel, Vector2 acc, bool isGrounded) HandleCollisions(Vector2 pos, Vector2 vel, Vector2 acc)
        {
            if(TileMapCollisions == null || CollidableEntities == null)
            {
                return (vel, acc, false);
            }
            bool isGrounded = false;
            contactNormal = new Vector2(0, 0);

            RectangleF currentCollisionBox = CollisionBox.At(pos);
            RectangleF projectedCollisionBox = CollisionBox.At(pos + vel);
            RectangleF boundingBox = getBoundingBox(currentCollisionBox, projectedCollisionBox);
            
            List<Vector2Int> tileMapCollisions = GetTileMapCollisions(boundingBox);

            //TODO: change tileRect to Collidable, make new Collidbale for the tiles, and ofr entities just add
            List<Entity> entityCollisions = GetEntityCollisions(boundingBox);

            //sort collisions based on contactTime
            List<(Collidable collider, float TimeHitNear, TileType TileType)> collisionsSorted = new();
            
            //check tilemap collisions
            foreach (Vector2Int collision in tileMapCollisions) //TODO: colissions -> tiles
            {
                if (TileMapCollisions.tryGetValue(collision, out TileType tileType) && tileType != TileType.None) //0 = air
                {
                    //raycast
                    RectangleF tileRect = new(collision.X * TileSize, collision.Y * TileSize, TileSize, TileSize);

                    Collidable tile = new(tileRect);
                    if (dynamicRectVsRect(currentCollisionBox, vel, tile.CurrentCollisionBox, out contactPoint, out contactNormal, out timeHitNear))
                    {
                        collisionsSorted.Add((tile, timeHitNear, tileType));
                    }
                }
            }

            //check entity collisions
            foreach(Entity entity in entityCollisions)
            {
                if (dynamicRectVsRect(currentCollisionBox, vel, entity.CurrentCollisionBox, out contactPoint, out contactNormal, out timeHitNear))
                {
                    collisionsSorted.Add((entity, timeHitNear, TileType.None)); //TODO: make it so touchable objects can exist
                    //TODO: TileType -> CollisionType
                }
            }//TODO: collisionsSorted -> collisions

            collisionsSorted = collisionsSorted.OrderBy(c => c.TimeHitNear).ThenBy(c => (GetMiddleOfRect(c.collider.CurrentCollisionBox) - GetMiddleOfRect(currentCollisionBox)).Length()).ToList();
            
            foreach ((Collidable collider, float TimeHitNear, TileType TileType) collision in collisionsSorted)
            {
                if (dynamicRectVsRect(currentCollisionBox, vel, collision.collider.CurrentCollisionBox, out contactPoint, out contactNormal, out timeHitNear))
                {
                    //first check if the collidable is tile or entity
                    if (collision.collider is Entity)
                    {
                        Entity entity = collision.collider as Entity;
                        Vector2 velAdded = entity.OnTouch(Parent, contactNormal);

                        //fix this
                        float NumberIfNotZero(float num1, float num2)
                        {
                            if(num2 == 0)
                            {
                                return num1;
                            }
                            else
                            {
                                return num2;
                            }
                        }
                         
                        vel = new(NumberIfNotZero(vel.X, velAdded.X), NumberIfNotZero(vel.Y, velAdded.Y));
                    }
                    else
                    {
                        //defined here so it has access to vel and acc
                        void applyCollisionResult()
                        {
                            vel += contactNormal * new Vector2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1f - timeHitNear);
                            acc -= acc * contactNormal;
                        }

                        switch (collision.TileType)
                        {
                            case TileType.Solid:
                                applyCollisionResult();
                                if (contactNormal == new Vector2(0, -1))
                                {
                                    isGrounded = true;
                                }
                                break;
                            case TileType.SemiUp:
                                if (contactNormal == new Vector2(0, -1) && currentCollisionBox.Bottom <= collision.collider.CurrentCollisionBox.Top) //check if player is above (normal vector)
                                {
                                    applyCollisionResult();
                                    isGrounded = true;
                                }
                                break;
                            case TileType.SemiRight:
                                if (contactNormal == new Vector2(1, 0) && currentCollisionBox.Left >= collision.collider.CurrentCollisionBox.Right) //check if player is above (normal vector)
                                {
                                    applyCollisionResult();
                                }
                                break;
                            case TileType.SemiDown:
                                if (contactNormal == new Vector2(0, 1) && currentCollisionBox.Top >= collision.collider.CurrentCollisionBox.Bottom) //check if player is above (normal vector)
                                {
                                    applyCollisionResult();
                                }
                                break;
                            case TileType.SemiLeft:
                                if (contactNormal == new Vector2(-1, 0) && currentCollisionBox.Right <= collision.collider.CurrentCollisionBox.Left) //check if player is above (normal vector)
                                {
                                    applyCollisionResult();
                                }
                                break;
                            case TileType.Hazard:
                                //same as solid
                                applyCollisionResult();
                                if (contactNormal == new Vector2(0, -1))
                                {
                                    isGrounded = true;
                                }
                                //damage
                                if (Parent is Player)
                                {
                                    Player player = Parent as Player;
                                    player.DamageIfNotImmune();
                                }
                                break;
                        }
                    }
                }
            }

            return (vel,  acc, isGrounded);
        }
    }
}
