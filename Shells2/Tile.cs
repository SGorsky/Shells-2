/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Tile.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Tile is a single space on the map
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

public enum TileType
{
    Floor,
    Grass,
    Sand,
    Bricks,
    GreyBricks,
    Planks
}

namespace Shells2
{
    public class Tile
    {
        //Type is the TileType for the tile and describes its texture, health and whether it is passable
        TileType type;

        public float defaultHealth = 50f;
        public TileType Type
        {
            get { return type; }
            set { type = value; }
        }

        //Passable is a boolean that controls whether units can pass through the tile
        bool passable;
        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }

        //TileHealth is the amount of health tiles have before they are destroyed
        float tileHealth;
        public float TileHealth
        {
            get { return tileHealth; }
            set { tileHealth = value; }
        }

        //Rectangle holds the x, y, width and height of the tile
        Rectangle rectangle;
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        //The GScore is the distance to move from the starting point to the tile following the path
        double gScore;
        public double GScore
        {
            get { return gScore; }
        }

        //The HScore is the heuristic cost to move from the current tile to the end tile (Manhattan Method used)
        double hScore;
        public double HScore
        {
            get { return hScore; }
        }

        //The FScore is the sum of the GScore and HScore
        public double FScore
        {
            get { return gScore + hScore; }
        }

        //GridLocation is the tile's location on the grid
        Point gridLocation;
        public Point GridLocation
        {
            get { return gridLocation; }
        }

        //Previous is the tile before this tile in the path
        Tile previous;
        public Tile Previous
        {
            get { return previous; }
            set { previous = value; }
        }

        //Assigns tile properties based on inputted values
        public Tile(byte redVal, byte greenVal, int y, int x, int tileDimmensions)
        {
            switch (redVal)
            {
                case 30:
                    type = TileType.GreyBricks;
                    passable = false;
                    tileHealth = 100;
                    break;

                case 50:
                    type = TileType.Grass;
                    passable = true;
                    tileHealth = 0;
                    break;

                case 140:
                    type = TileType.Floor;
                    passable = true;
                    tileHealth = 0;
                    break;

                case 180:
                    type = TileType.Sand;
                    passable = true;
                    tileHealth = 0;
                    break;

                case 255:
                    switch (greenVal)
                    {
                        case 0:
                            type = TileType.Bricks;
                            passable = false;
                            tileHealth = defaultHealth;
                            break;

                        case 143:
                            type = TileType.Planks;
                            passable = true;
                            tileHealth = 0;
                            break;
                    }
                    break;

            }
            rectangle = new Rectangle(x * tileDimmensions, y * tileDimmensions, tileDimmensions, tileDimmensions);
            gridLocation = new Point(x, y);
        }

        //Pre: The start is a valid Vector, prevTile is a valid tile
        //Post: The GScore for the tile is calculated and stored in GScore
        //Description: The distance from the Start tile to the current tile is calculated using only up, down, left and right movements
        public void CalculateGScore(Point start, Tile prevTile)
        {
            if (prevTile != null)
            {
                gScore = prevTile.GScore + 10;
            }
            else
            {
                gScore = 10;
            }
        }

        //Pre: The end is a valid Vector
        //Post: The HScore for the tile is calculated and stored in HScore
        //Description: The distance from the End tile to the current tile is calculated using only up, down, left and right movements
        public void CalculateHScore(Point end)
        {
            float xDistance = Math.Abs(GridLocation.X - end.X);
            float yDistance = Math.Abs(GridLocation.Y - end.Y);
            hScore = 10.0 * (xDistance + yDistance);
        }
    }
}
