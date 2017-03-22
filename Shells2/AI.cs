/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: AI.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: AI is one of the types of players in the game but it is controlled by the computer, not the user
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shells2
{
    class AI : Unit
    {
        /// <summary>
        /// The constructor for the AI class
        /// </summary>
        /// <param name="hp">The amount of health the player will have.</param>
        /// <param name="entityGun">The gun that the player will use.</param>
        /// <param name="imageID">The index of the texture in the playerTexture array to use when drawing this player.</param>
        /// <param name="teamNum">The team number. Players with different team numbers will be against this unit.</param>
        /// <param name="startingPosition">The initial position that the unit will spawn from</param>
        public AI(int hp, Gun entityGun, int imageID, int teamNum, Vector2 startingPosition)
            : base(hp, entityGun, imageID, teamNum, startingPosition)
        {
            path = new List<Tile>();
        }
    }
}
