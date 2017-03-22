/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Gun.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Holds the framework for any gun that will be created in the game
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shells2
{
    public class Gun
    {
        //Holds the damage of the gun
        float damage;
        public float Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        //Holds the amount of damage that the bullet's damage is increased/decreased by as it travels
        float damageFallOff;
        public float DamageFallOff
        {
            get { return damageFallOff; }
            set { damageFallOff = value; }
        }

        //Holds how often the gun can be shot
        int rateOfFire;
        public int RateOfFire
        {
            get { return rateOfFire; }
            set { rateOfFire = value; }
        }

        //Holds the maximum amount of bullets the gun can hold
        int clipCapacity;
        public int ClipCapacity
        {
            get { return clipCapacity; }
            set { clipCapacity = value; }
        }

        //Holds the current amount of bullets in the gun
        int ammo;
        public int Ammo
        {
            get { return ammo; }
            set { ammo = value; }
        }

        //Holds the amount of time it takes for the gun to reload
        int reloadTime;
        public int ReloadTime
        {
            get { return reloadTime; }
            set { reloadTime = value; }
        }

        //Holds if the gun is reloading or not as either true or false
        bool reloading;
        public bool Reloading
        {
            get { return reloading; }
            set { reloading = value; }
        }

        //Holds the speed at which the character holding the gun can move at
        float speed;
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        //Holds the bullet spread of the gun
        float spread;
        public float Spread
        {
            get { return spread; }
            set { spread = value; }
        }

        //Holds the id of the gun as a number
        int gunID;
        public int GunID
        {
            get { return gunID; }
            set { gunID = value; }
        }

        //Holds the amount of time in milliseconds passed after last shooting the gun
        int timePassed;
        public int TimePassed
        {
            get { return timePassed; }
            set { timePassed = value; }
        }
    }
}