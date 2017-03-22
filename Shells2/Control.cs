/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Control.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: Control is not an actual class, it is just a file to store 3 other classes: Menu, Button, and FloatingText
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shells2
{
    //Desc: Menu is collection of buttons and texts that also has a GameState and a background
    class Menu
    {
        Texture2D background;
        public Texture2D Background
        {
            get { return background; }
            set { background = value; }
        }

        //List of all buttons in the menu
        List<Button> buttons;
        public List<Button> Buttons
        {
            get { return buttons; }
            set { buttons = value; }
        }

        //List of all "Floating Texts". Essentially just labels
        List<FloatingText> texts;
        public List<FloatingText> Texts
        {
            get { return texts; }
            set { texts = value; }
        }

        public Menu(Texture2D thisTexture)
        {
            buttons = new List<Button>();
            texts = new List<FloatingText>();
            background = thisTexture;
        }
    }

    //Desc: The button object is a texture contained within a rectangle, and has a specific target and ID which are used to identify itself and the menu to which it links
    class Button
    {
        //Texture to draw
        Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        //Color changes depending on if the player is mousing over the button
        Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        //The menu that is reached when the button is pressed
        byte target;
        public byte Target
        {
            get { return target; }
            set { target = value; }
        }

        //An ID used to identify the button (For buttons that do more than just access a menu)
        string id;
        public string ID
        {
            get { return id; }
        }

        //Bounds for the button to reside within
        Rectangle bounds;
        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public Button(byte thisTarget, Texture2D thisTexture, Rectangle thisBounds, string buttonID)
        {
            texture = thisTexture;
            target = thisTarget;
            bounds = thisBounds;
            color = Color.White;
            id = buttonID;
        }
    }

    //Desc: The FloatingText object is a way to write things to the screen. It resembles labels in form applications
    class FloatingText
    {
        //A text to display
        string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        //A location to display the text at
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //The color of the text
        Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        //The font to use to draw the text
        SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public FloatingText(string thisText, Color thisColor, Vector2 thisPosition, SpriteFont thisFont)
        {
            position = thisPosition;
            color = thisColor;
            text = thisText;
            font = thisFont;
        }
    }
}
