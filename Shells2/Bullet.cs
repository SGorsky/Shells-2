/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Bullet.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Bullet is a single projectile object that causes damage to tiles and players
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shells2
{
    public class Bullet : Entity
    {
        //Direction is the normalized direction that the bullet is travelling
        Vector2 direction;
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        //Damage is the amount of damage that the bullet will cause once it hits
        float damage;
        public float Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        //Hit is a boolean that checks whether the bullet has hit an unpassable tile or an entity
        bool hit;
        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }

        //DamageFallOff is the amount of damage that is added/subtracted from the bullets damage as it travels
        float damageFallOff;
        public float DamageFallOff
        {
            get { return damageFallOff; }
            set { value = damageFallOff; }
        }


        public Bullet(Vector2 pos, Vector2 dir, float bulletRotation, float bulletDamage, float thisDamageFallOff, int teamNum)
        {
            Position = pos;
            direction = dir;
            Rotation = bulletRotation;
            damage = bulletDamage;
            damageFallOff = thisDamageFallOff;
            base.TeamNum = teamNum;

            direction.Normalize();
        }
    }
}