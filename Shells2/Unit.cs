/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Unit.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Unit is the class for each player both user and AI. It is the parent of the AI class
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shells2
{
    public class Unit : Entity
    {
        //double to store how much time needs pass before shooting
        double shootTime;

        public double ShootTime
        {
            get { return shootTime; }
            set { shootTime = value; }
        }

        //Flaot to store health
        float health;
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        //Max health of unit
        int maxHealth;
        public int MaxHealth
        {
            get { return maxHealth; }
        }

        //Current gun of unit
        Gun gun;
        public Gun Gun
        {
            get { return gun; }
            set { gun = value; }
        }

        //The unit's texture
        int textureID;
        public int TextureID
        {
            get { return textureID; }
            set { textureID = value; }
        }

        //Time passed idling
        int idleTime;
        public int IdleTime
        {
            get { return idleTime; }
            set { idleTime = value; }
        }

        //Where unit is looking at
        Vector2 target;
        public Vector2 Target
        {
            get { return target; }
            set { target = value; }
        }

        //Constants
        const float GUN_ROTATION = 4.5f;
        const float IDLE_RELOAD_CHECK = 5000;
        const float IDLE_HEALTH_CHECK = 3000;
        const float HEALTH_REGEN_INTERVAL = 300;
        const float HEALTH_INCREASE = 3;

        //Random generator
        Random generator = new Random();

        //Spawn locaton
        Vector2 spawnPosition;

        //List of tiles to form the path
        public List<Tile> path;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hp">The amount of health the player will have.</param>
        /// <param name="entityGun">The gun that the player will use.</param>
        /// <param name="imageID">The index of the texture in the playerTexture array to use when drawing this player.</param>
        /// <param name="teamNum">The team number. Players with different team numbers will be against this unit.</param>
        /// <param name="startingPosition">The initial position that the unit will spawn from</param>
        public Unit(int hp, Gun entityGun, int imageID, int teamNum, Vector2 startingPosition)
        {
            maxHealth = hp;
            health = hp;
            gun = entityGun;
            textureID = imageID;
            shootTime = 1;
            spawnPosition = startingPosition;

            base.Position = new Vector2(startingPosition.X, startingPosition.Y);
            base.TeamNum = teamNum;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D playerTexture, Texture2D gunTexture, int UNIT_SIZE, int gunTextureWidth, int gunTextureHeight)
        {
            spriteBatch.Draw(playerTexture, new Rectangle((int)Position.X, (int)Position.Y, UNIT_SIZE, UNIT_SIZE), null, Color.White,
                Rotation, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), SpriteEffects.None, 1.0f);
            spriteBatch.Draw(gunTexture, new Rectangle((int)Position.X, (int)Position.Y, gunTextureWidth, gunTextureHeight), null, Color.White,
                Rotation, new Vector2(gunTexture.Width / 2, gunTexture.Height + playerTexture.Height / GUN_ROTATION), SpriteEffects.None, 1.0f);
        }

        public void Update(int elapsedMilliseconds)
        {
            idleTime += elapsedMilliseconds;
            if (health > 0)
            {
                gun.TimePassed += elapsedMilliseconds;
                if (target != Vector2.Zero)
                {
                    Rotation = Convert.ToSingle(Math.Atan2(target.Y - Position.Y, target.X - Position.X) + MathHelper.PiOver2);
                }

                if (gun.TimePassed >= IDLE_RELOAD_CHECK && gun.Ammo != gun.ClipCapacity && this is AI && !gun.Reloading)
                {
                    gun.Reloading = true;
                    gun.TimePassed = Convert.ToInt32(((float)(gun.Ammo) / (float)gun.ClipCapacity) * gun.ReloadTime);
                }

                if (((HEALTH_INCREASE / HEALTH_REGEN_INTERVAL) * (idleTime - IDLE_HEALTH_CHECK)) > HEALTH_INCREASE && health < maxHealth)
                {
                    idleTime = (int)IDLE_HEALTH_CHECK;
                    health += (int)HEALTH_INCREASE;
                }

                if (gun.TimePassed >= gun.ReloadTime && gun.Reloading)
                {
                    gun.Reloading = false;
                    gun.Ammo = gun.ClipCapacity;
                }
            }
        }

        //Pre: Unit needs to shoot a bullet
        //Post: Bullet is ejected
        //Desc: Uses math and vectors to calculate trajectory and speed. Returns a bullet object to MainGame
        public Bullet Shoot(int elapsedMilliseconds, Texture2D gunTexture)
        {
            //If unit can shoot
            if (!gun.Reloading && gun.TimePassed >= gun.RateOfFire)
            {
                //Reset variables
                shootTime += 0.05;
                gun.TimePassed = 0;
                idleTime = 0;
                --gun.Ammo;

                //Calculate direction
                float spread = Convert.ToSingle((generator.NextDouble() * 2.0f - 1.0f) * gun.Spread * shootTime);
                float bulletRotation = Convert.ToSingle(Math.Atan2(target.Y - Position.Y + spread, target.X - Position.X + spread));
                Vector2 bulletDirection = new Vector2(target.X - Position.X + spread, target.Y - Position.Y + spread);
                Vector2 bulletPosition = new Vector2(Position.X + (gunTexture.Height / 2 * (float)Math.Sin(Rotation)),
                    Position.Y - (gunTexture.Height / 2 * (float)Math.Cos(Rotation)));

                //if no ammo, reload
                if (gun.Ammo == 0)
                {
                    gun.Reloading = true;
                }

                //return
                return new Bullet(bulletPosition, bulletDirection, bulletRotation, gun.Damage, gun.DamageFallOff, base.TeamNum);
            }
            else if (gun.Reloading)
            {
                shootTime = 1;
            }
            return null;
        }

        //Pre: Unit has died and 5 seconds passed
        //Post: Unit is respawned
        //Desc: resets unit's stats
        public void Respawn(Gun newGun)
        {
            health = maxHealth;
            gun = newGun;
            shootTime = 1;
            idleTime = 0;

            if (this is AI)
            {
                path.Clear();
            }

            base.Position = new Vector2(spawnPosition.X, spawnPosition.Y);
        }
    }
}