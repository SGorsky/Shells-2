/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Particle.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Particle is the class for each blood particle
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shells2
{
    class Particle
    {
        //Position is the position of the particle on the screen
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //Velocity is the velocity of the particle
        Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //LifeSpan is the amount of time left that the particle will remain onscreen in milliseconds
        int lifeSpan;
        public int LifeSpan
        {
            get { return lifeSpan; }
            set { lifeSpan = value; }
        }

        public Particle(int life)
        {
            lifeSpan = life;
        }
    }
}
