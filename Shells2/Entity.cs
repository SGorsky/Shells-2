/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Entity.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Entity is the parent class for every Bullet and Unit/AI in the game
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shells2
{
    public class Entity
    {
        //Position is the position of the entity on the screen
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //Rotation is how the entity's texture will be rotated in radians
        float rotation;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        //TeamNum is the team number of the entity
        int teamNum;
        public int TeamNum
        {
            get { return teamNum; }
            set { teamNum = value; }
        }
    }
}