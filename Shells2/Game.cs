/* Author: Ori Almog, Sean Gorsky, Gil Kvint, David Minin
 * File Name: Game.cs
 * Project Name: Game
 * Creation Date: December 12, 2013
 * Modified Date: January 20, 2014
 * Description: The is a top down shooter game where you are trying to kill enemy AI while trying to stay alive
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Shells2
{
    public class Game : Microsoft.Xna.Framework.Game
    {

        #region Basic Variable Requirements

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont defaultFont;
        SpriteFont damageFont;

        //Holds the current state of the keyboard and the mouse
        KeyboardState kb;
        MouseState mouse;

        Random rand = new Random();

        #endregion

        #region Game Textures

        //The textures for tile cracking
        Texture2D smallCrack;
        Texture2D midCrack;
        Texture2D bigCrack;
        
        //Aura for the player
        Texture2D aura;

        //Holds an array of textures that the player can choose to use represent the player. Each texture is like another costume for the player
        Texture2D[] playerTexture = new Texture2D[15];

        //Holds an array of textures for each gun that the player can use
        Texture2D[] gunTexture = new Texture2D[6];

        //The textures for the status bars above each player
        Texture2D statusBar;
        Texture2D healthBar;
        Texture2D ammoBar;

        //The texutre for the bullet being fired from a gun
        Texture2D bullet;

        //The texture for the blood that comes out of a player once they have been shot
        Texture2D blood;

        //The texture for the crosshair which represents the location of the player's mouse
        Texture2D crossHair;

        //The textures for the tiles on the map
        Texture2D brickTile;
        Texture2D floorTile;
        Texture2D grassTile;
        Texture2D sandTile;
        Texture2D planksTile;
        Texture2D greyBricksTile;

        #endregion

        #region Menu Textures

        //The textures for the backgrounds and the buttons used in the different menus that the user can interact with
        Texture2D backgroundMain;
        Texture2D backgroundGeneral;
        Texture2D start;
        Texture2D back;
        Texture2D options;
        Texture2D quit;
        Texture2D rightArrow;
        Texture2D leftArrow;
        Texture2D plus;
        Texture2D minus;

        #endregion

        #region Lists

        //The lists of particles, bullets, players and menus used in the game
        List<Particle> allParticles = new List<Particle>();
        List<Bullet> bulletList = new List<Bullet>();
        List<Unit> players = new List<Unit>();
        List<Menu> menus = new List<Menu>();

        #endregion

        #region Map Related

        //The dimmensions (in pixels) of each tile
        const int TILE_SIZE = 30;

        //The index of the current map being chosen to play
        public byte currentMap = 0;

        //The filename to be read in that contains the map
        string[] mapFileNames = new string[4] { "TownMap.txt", "PalaceMap.txt", "MazeMap.txt", "RaidMap.txt" };

        //The array of textures that show what each map looks like
        Texture2D[] mapPreviews = new Texture2D[5];

        //The height of the map in terms of number of tiles
        int height;

        //The width of the map in terms of number of tiles
        int width;

        //The array of tiles that represents the map
        Tile[,] World;

        #endregion

        #region Sounds

        //The sound effects that are played when a player fires a gun
        SoundEffect largeGun;
        SoundEffect mediumGun;
        SoundEffect smallGun;
        SoundEffect ak47Sound;

        #endregion

        #region Constants and Semi-Variables

        //playerIndex is the index of the user's unit in the player list
        int playerIndex;

        //The timeModifier that allows the user to slow down the game and cause a bullet time effect while playing
        float timeModifier = 1f;

        //The volume of the sounds in terms of percent
        float volume = 100f;

        //The current menu that is currently being displayed
        byte currentMenu = 0;

        //The index of the texture to use in reference to the playerTexture array
        byte team0TextureID = 0;
        byte team1TextureID = 1;

        //The number of players on each team when the game starts
        public byte teamSize = 1;

        //A boolean that checks whether the left mouse button is being clicked or not
        bool down = false;

        //A boolean that checks whether the escape key is being pressed or not
        bool escape = false;

        //An integer to keep track of how much time remains until bullet time is over
        int bulletTimeLeft = 0;

        //The distance that an AI and an enemy teammate (in line of sight) must be less than in order to run the A Star Pathfinding Algorithm
        const int AI_DISTANCE_MAXIMUM = 500;

        //The size of the units being displayed
        const int UNIT_SIZE = 24;

        //The default health for each unit
        const int UNIT_HEALTH = 100;

        //The amount of time a unit will need to wait before respawning after dying
        const int RESPAWN_TIME = 5000;

        //The maximum number of people there can be on one team
        const int TEAM_SIZE_LIMIT = 5;

        //The number of bullets that come out of a shotgun
        const int SHOTGUN_BULLETS = 8;

        //The factor that affects how quickly bullets move
        const float BULLET_SPEED_MULTIPLIER = 15;

        //How much time is slowed down to when bullet Time is activated
        const float BULLET_TIME_FACTOR = 0.3f;

        //The amount of time that bullet time occurs after a player dies (in milliseconds)
        const int BULLET_TIME_LENGTH = 750;

        //The width and height of each bullet texture
        const int BULLET_WIDTH = 10;
        const int BULLET_HEIGHT = 5;

        //The amount of blood that is created after a player is shot and the size of each blood particle
        const int BLOOD_PER_HIT = 100;
        const int BLOOD_SIZE = 3;

        //The minimum and maximum lifespan of a single blood particle
        const int PARTICLE_MIN_LIFESPAN = 0;
        const int PARTICLE_MAX_LIFESPAN = 1000;

        //The explosion factor that affects how far/fast the blood moves when generated
        const float PARTICLE_EXPLOSION = 3f;

        //The index of the different menus in the menus list
        const int MAIN_MENU_INDEX = 0;
        const int OPTIONS_INDEX = 1;
        const int START_SCREEN_INDEX = 2;
        const int CHOOSE_SKIN_SCREEN_INDEX = 3;
        const int CHOOSE_MAP_SCREEN_INDEX = 4;
        const int GAME_SCREEN_INDEX = 5;
        const int END_INDEX = 6;

        //The gun ID of a shotgun
        const int SHOTGUN_ID = 5;

        #endregion

        #region Goal Management

        //The number of kills each team has achieved
        int team0Score = 0;
        int team1Score = 0;

        //The index of which scoreLimit to use from the scoreLimit array
        int scoreLimitIndex = 0;

        //The different score limits the game can have. If it is -1, there is no score limit
        int[] scoreLimit = new int[5] { 5, 10, 20, 50, -1 };

        //The index of the scoreLimit array where it is equal to -1
        const int UNLIMITED_SCORE_LIMIT = 4;

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Set the width and height of the window
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1350;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            defaultFont = Content.Load<SpriteFont>("defaultFont");
            damageFont = Content.Load<SpriteFont>("damageFont");

            #region Load Textures

            //Read in the entity textures
            playerTexture[0] = Content.Load<Texture2D>("Entities\\Player One");
            playerTexture[1] = Content.Load<Texture2D>("Entities\\Player Two");
            playerTexture[2] = Content.Load<Texture2D>("Entities\\Player Three");
            playerTexture[3] = Content.Load<Texture2D>("Entities\\Player Four");
            playerTexture[4] = Content.Load<Texture2D>("Entities\\Soldier");
            playerTexture[5] = Content.Load<Texture2D>("Entities\\BlueSoldier");
            playerTexture[6] = Content.Load<Texture2D>("Entities\\RedSoldier");
            playerTexture[7] = Content.Load<Texture2D>("Entities\\YellowSoldier");
            playerTexture[8] = Content.Load<Texture2D>("Entities\\Captain");
            playerTexture[9] = Content.Load<Texture2D>("Entities\\ConstructionWorker");
            playerTexture[10] = Content.Load<Texture2D>("Entities\\Link Skin");
            playerTexture[11] = Content.Load<Texture2D>("Entities\\Mario Skin");
            playerTexture[12] = Content.Load<Texture2D>("Entities\\Ninja");
            playerTexture[13] = Content.Load<Texture2D>("Entities\\Pirate");
            playerTexture[14] = Content.Load<Texture2D>("Entities\\Politician");

            //Read in the gun textures
            gunTexture[0] = Content.Load<Texture2D>("Guns\\AK47");
            gunTexture[1] = Content.Load<Texture2D>("Guns\\Desert Eagle");
            gunTexture[2] = Content.Load<Texture2D>("Guns\\Dragunov");
            gunTexture[3] = Content.Load<Texture2D>("Guns\\LSAT");
            gunTexture[4] = Content.Load<Texture2D>("Guns\\MP5");
            gunTexture[5] = Content.Load<Texture2D>("Guns\\SPAS");

            //Read in the tile textures
            brickTile = Content.Load<Texture2D>("Tiles\\Brick");
            floorTile = Content.Load<Texture2D>("Tiles\\Floor");
            grassTile = Content.Load<Texture2D>("Tiles\\Grass");
            sandTile = Content.Load<Texture2D>("Tiles\\Sand");
            greyBricksTile = Content.Load<Texture2D>("Tiles\\GreyBricks");
            planksTile = Content.Load<Texture2D>("Tiles\\Planks");

            //Read in the map previews
            //{ TownMap, PalaceMap, OpenMap, MazeMap, RaidMap };
            mapPreviews[0] = Content.Load<Texture2D>("Previews\\TownMap");
            mapPreviews[1] = Content.Load<Texture2D>("Previews\\PalaceMap");
            mapPreviews[2] = Content.Load<Texture2D>("Previews\\MazeMap");
            mapPreviews[3] = Content.Load<Texture2D>("Previews\\RaidMap");

            //Read in the player's health and ammo textures
            statusBar = Content.Load<Texture2D>("Player Bar");
            ammoBar = Content.Load<Texture2D>("Ammo Bar");
            healthBar = Content.Load<Texture2D>("Health Bar");

            //Read in the blood and bullet textures
            bullet = Content.Load<Texture2D>("Bullet1");
            blood = Content.Load<Texture2D>("blood");

            //Read in the crosshair texture and the player's aura texture
            crossHair = Content.Load<Texture2D>("Crosshair");
            aura = Content.Load<Texture2D>("Aura");

            //Read in crack textures
            smallCrack = Content.Load<Texture2D>("CrackV1");
            midCrack = Content.Load<Texture2D>("CrackV2");
            bigCrack = Content.Load<Texture2D>("CrackV3");

            #endregion

            #region Load Sounds

            //Read in the sounds for the guns
            largeGun = Content.Load<SoundEffect>("Sounds\\Largegun");
            mediumGun = Content.Load<SoundEffect>("Sounds\\Mediumgun");
            smallGun = Content.Load<SoundEffect>("Sounds\\Smallgun");
            ak47Sound = Content.Load<SoundEffect>("Sounds\\AK47");

            #endregion

            #region Load Menu Textures

            //Read in the textures for the guns
            backgroundMain = Content.Load<Texture2D>("Controls\\BackgroundMain");
            backgroundGeneral = Content.Load<Texture2D>("Controls\\Background2");
            start = Content.Load<Texture2D>("Controls\\Start Button1");
            back = Content.Load<Texture2D>("Controls\\Back Button");
            options = Content.Load<Texture2D>("Controls\\Options Button");
            quit = Content.Load<Texture2D>("Controls\\Quit Button");
            rightArrow = Content.Load<Texture2D>("Controls\\Right Button");
            leftArrow = Content.Load<Texture2D>("Controls\\Left Button");
            plus = Content.Load<Texture2D>("Controls\\Plus Button");
            minus = Content.Load<Texture2D>("Controls\\Subtract Button");

            #endregion

            SetupMenus();
        }

        //Pre: none
        //Post: Each menu is created and stored in the menus list
        //Description: Set up the different menus to be used throughout the game
        private void SetupMenus()
        {
            //Define menus
            menus = new List<Menu>();

            #region Main Menu

            //Add all the control features into the main menu such as the start, options and quit buttons
            Menu menu = new Menu(backgroundMain);
            menu.Buttons.Add(new Button(START_SCREEN_INDEX, start, new Rectangle(100, 100, start.Width, start.Height), "startScreen"));
            menu.Buttons.Add(new Button(OPTIONS_INDEX, options, new Rectangle(100, 250, options.Width, options.Height), "optionsScreen"));
            menu.Buttons.Add(new Button(66, quit, new Rectangle(100, 400, quit.Width, quit.Height), "quit"));
            menus.Add(menu);

            #endregion

            #region Options

            //Add all the control features into the options menu such as the increase and decrease volume and the return to main menu button
            menu = new Menu(backgroundGeneral);
            menu.Buttons.Add(new Button(MAIN_MENU_INDEX, back, new Rectangle(100, 100, back.Width, back.Height), "mainMenu"));
            menu.Buttons.Add(new Button(66, plus, new Rectangle(100, 400, plus.Width, plus.Height), "volumeUp"));
            menu.Buttons.Add(new Button(66, minus, new Rectangle(100, 550, minus.Width, minus.Height), "volumeDown"));

            //Add the text that shows the volume of the sounds being played
            menu.Texts.Add(new FloatingText("Volume: " + volume + "%", Color.White, new Vector2(100, 380), defaultFont));
            menus.Add(menu);

            #endregion

            #region Start Screen

            //Add all the control features into the start screen such as the main menu, choose skin and volume buttons
            //Add the text that is used to show how many members are on each team and the score's limit
            menu = new Menu(backgroundGeneral);
            menu.Buttons.Add(new Button(MAIN_MENU_INDEX, back, new Rectangle(100, 250, back.Width, back.Height), "mainMenu"));
            menu.Buttons.Add(new Button(CHOOSE_SKIN_SCREEN_INDEX, start, new Rectangle(100, 100, start.Width, start.Height), "chooseSkinScreen"));

            menu.Buttons.Add(new Button(66, plus, new Rectangle(100, 400, plus.Width, plus.Height), "teamsizeUp"));
            menu.Buttons.Add(new Button(66, minus, new Rectangle(100, 550, minus.Width, minus.Height), "teamsizeDown"));
            menu.Texts.Add(new FloatingText("Team Size: " + teamSize.ToString(), Color.White, new Vector2(100, 380), defaultFont));

            menu.Buttons.Add(new Button(66, plus, new Rectangle(400, 400, plus.Width, plus.Height), "scoreUp"));
            menu.Buttons.Add(new Button(66, minus, new Rectangle(400, 550, minus.Width, minus.Height), "scoreDown"));
            menu.Texts.Add(new FloatingText("Score Limit: " + scoreLimit[scoreLimitIndex], Color.White, new Vector2(400, 380), defaultFont));

            menus.Add(menu);

            #endregion

            #region Choose Skin Screen

            //Add all the control features into the skin screen such as the game screen button, the start screen button and the 
            //button for you to view the skins for your team and the enemy team
            menu = new Menu(backgroundGeneral);
            menu.Buttons.Add(new Button(66, leftArrow, new Rectangle(100, 600, plus.Width, plus.Height), "previousTeam0Texture"));
            menu.Buttons.Add(new Button(66, rightArrow, new Rectangle(400, 600, minus.Width, minus.Height), "nextTeam0Texture"));

            menu.Buttons.Add(new Button(66, leftArrow, new Rectangle(700, 600, plus.Width, plus.Height), "previousTeam1Texture"));
            menu.Buttons.Add(new Button(66, rightArrow, new Rectangle(1000, 600, minus.Width, minus.Height), "nextTeam1Texture"));

            menu.Buttons.Add(new Button(CHOOSE_MAP_SCREEN_INDEX, start, new Rectangle(725, 800, start.Width, start.Height), "chooseMapScreen"));
            menu.Buttons.Add(new Button(START_SCREEN_INDEX, back, new Rectangle(395, 800, back.Width, back.Height), "startScreen"));

            menus.Add(menu);

            #endregion

            #region Choose Map Screen

            menu = new Menu(backgroundGeneral);
            menu.Buttons.Add(new Button(66, leftArrow, new Rectangle(395, 600, plus.Width, plus.Height), "previousMap"));
            menu.Buttons.Add(new Button(66, rightArrow, new Rectangle(725, 600, minus.Width, minus.Height), "nextMap"));
            menu.Buttons.Add(new Button(GAME_SCREEN_INDEX, start, new Rectangle(725, 800, start.Width, start.Height), "gameScreen"));
            menu.Buttons.Add(new Button(CHOOSE_SKIN_SCREEN_INDEX, back, new Rectangle(395, 800, back.Width, back.Height), "chooseSkinScreen"));

            menus.Add(menu);

            #endregion

            #region Game Screen

            //Add the text that shows the score of both teams
            menu = new Menu(backgroundGeneral);
            menu.Texts.Add(new FloatingText("Team 1 score: " + team0Score, Color.Yellow, new Vector2(20, 10), defaultFont));
            menu.Texts.Add(new FloatingText("Team 2 score: " + team1Score, Color.Yellow, new Vector2(1172, 10), defaultFont));
            menus.Add(menu);

            #endregion

            #region End Screen

            //Add the button that allows the user to go back to the main menu and the text that shows whether you won or lost
            menu = new Menu(backgroundMain);
            menu.Texts.Add(new FloatingText("Team X Wins!", Color.White, Vector2.Zero, defaultFont));
            menu.Buttons.Add(new Button(MAIN_MENU_INDEX, back, new Rectangle(50, 100, back.Width, back.Height), "mainMenu"));
            menus.Add(menu);

            #endregion
        }

        //Pre: none
        //Post: The units for both teams have been created
        //Description: Create the user's unit and the units for the user's teammates and the enemy team
        private void SpawnEntities()
        {
            //GunName - Type - GunID
            //AK47 - Assault Rifle - 0
            //Desert Eagle - Handgun - 1
            //Dragunov - Sniper Rifle - 2
            //LSAT - Light Machine Gun - 3
            //MP5 - Submachine Gun - 4
            //SPAS - Shotgun  - 5

            //Create the user's unit
            players.Add(new Unit(UNIT_HEALTH, ReadInGun(4), team0TextureID, 0, new Vector2(World[1, 1].Rectangle.Center.X, World[1, 1].Rectangle.Center.Y)));

            //Call upon the CreateTeam subprogram to create the two teams
            CreateTeam(UNIT_HEALTH, ReadInGun(rand.Next(0, 6)), team0TextureID, new Vector2(World[1, 1].Rectangle.Center.X, World[1, 1].Rectangle.Center.Y), teamSize - 1, 0);
            CreateTeam(UNIT_HEALTH, ReadInGun(rand.Next(0, 6)), team1TextureID, new Vector2(World[28, 43].Rectangle.Center.X, World[28, 43].Rectangle.Center.Y), teamSize, 1);
        }

        //Pre: health is a valid integer number greater than 0, Gun is a valid gun with all its properties already set
        //imageId is a valid integer that corresponds to a certain index in the playerTextures array
        //spawnLocation is a valid Vector2 on the screen that shows where the player will spawn during the game
        //amount is a valid integer number greater than or equal to 0
        //team is a valid integer
        //Post: A list of players are added to the players list
        //Description: Use the parameters to create a blueprint for a certain amount of players
        private void CreateTeam(int health, Gun weapon, int imageID, Vector2 spawnLocation, int amount, int team)
        {
            //For amount number of times, add a new player using the parameters to the players list
            for (int i = 0; i < amount; ++i)
            {
                players.Add(new AI(health, weapon, imageID, team, new Vector2(spawnLocation.X, spawnLocation.Y)));
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (true)//IsActive)
            {
                #region Keyboard and Mouse Updates

                //Get the state of the keyboard and the mouse
                mouse = Mouse.GetState();
                kb = Keyboard.GetState();

                // Allows the game to exit
                if (kb.IsKeyDown(Keys.Escape) && !escape)
                {
                    //If the current state of the game is in the gameScreen, set escape to true and the currentMenu to the MAIN_MENU_INDEX
                    if (currentMenu == GAME_SCREEN_INDEX)
                    {
                        escape = true;
                        currentMenu = MAIN_MENU_INDEX;
                        players = new List<Unit>();
                        World = null;
                    }
                    //Else, exit the program
                    else
                    {
                        this.Exit();
                    }
                }
                //If the escape key is up and escape is true, set escape to false
                else if (kb.IsKeyUp(Keys.Escape) && escape)
                {
                    escape = false;
                }

                #endregion

                //If the current gameState is the gameScreen
                if (currentMenu == GAME_SCREEN_INDEX)
                {
                    #region Game Screen

                    #region Update Entities

                    //Set playerIndex to be the index of the first Unit inside the players list that is not an AI
                    playerIndex = players.FindIndex(entity => entity is AI == false);

                    //If playerIndex is not -1 (player exists), set the user's target to the mouse
                    if (playerIndex != -1)
                    {
                        players[playerIndex].Target = new Vector2(mouse.X, mouse.Y);
                    }

                    //Call upon the Unit's Update function for each unit
                    for (int i = 0; i < players.Count; ++i)
                    {
                        players[i].Update(Convert.ToInt32(gameTime.ElapsedGameTime.Milliseconds * timeModifier));
                    }

                    //Call upon the RespawnCheckSubprogram
                    RespawnCheck();

                    #endregion

                    UpdateBulletTime(gameTime.ElapsedGameTime.Milliseconds);
 
                    #region Player Movement

                    //If playerIndex is not -1 (player exists)
                    if (playerIndex != -1)
                    {
                        //If the user presses the A or Left Arrow Key
                        if ((kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)))
                        {
                            CheckMovement(new Vector2(-1 * players[playerIndex].Gun.Speed * timeModifier, 0));
                        }
                        //If the user presses the D or Right Arrow Key
                        if ((kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)))
                        {
                            CheckMovement(new Vector2(players[playerIndex].Gun.Speed * timeModifier, 0));
                        }
                        //If the user presses the W or Up Arrow Key
                        if ((kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)))
                        {
                            CheckMovement(new Vector2(0, -1 * players[playerIndex].Gun.Speed * timeModifier));
                        }
                        //If the user presses the S or Down Arrow Key
                        if ((kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)))
                        {
                            CheckMovement(new Vector2(0, players[playerIndex].Gun.Speed * timeModifier));
                        }
                    }

                    #endregion

                    #region Shooting

                    //If playerIndex is not -1
                    if (playerIndex != -1)
                    {
                        //If the player presses the R key and the player is does not have a full clip and the player is not currently reloading
                        if (kb.IsKeyDown(Keys.R) && players[playerIndex].Gun.Ammo != players[playerIndex].Gun.ClipCapacity && !players[playerIndex].Gun.Reloading)
                        {
                            //Set reloading to true and set the timePassed so that the player only needs to reload an amount of time based on how much
                            //ammo they need to get
                            players[playerIndex].Gun.Reloading = true;
                            players[playerIndex].Gun.TimePassed = Convert.ToInt32(((float)(players[playerIndex].Gun.Ammo) / (float)players[playerIndex].Gun.ClipCapacity) * players[playerIndex].Gun.ReloadTime);
                        }

                        //If the left mouse button is pressed
                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            Shoot(Convert.ToInt32(gameTime.ElapsedGameTime.Milliseconds * timeModifier), playerIndex);
                        }
                        //Else set shootTime to 1
                        else
                        {
                            players[playerIndex].ShootTime = 1;
                        }
                    }

                    #endregion

                    #region Bullet Management

                    //Do the following for each bullet in bulletList
                    foreach (Bullet b in bulletList)
                    {
                        if (b.Damage > b.DamageFallOff)
                        {
                            b.Damage -= b.DamageFallOff;
                        }

                        //Add the bullets direction multiplied by the bullet speed multiplier multiplied by the timeModifier to the bullets position
                        b.Position += Vector2.Multiply(b.Direction, BULLET_SPEED_MULTIPLIER * timeModifier);

                        //Loop through each player in the players array
                        for (int j = 0; j < players.Count; ++j)
                        {
                            //Check if the player at index j is not on the same team as the unit that shot the bullet and if it has more than 0 health
                            if (players[j].Health > 0 && players[j].TeamNum != b.TeamNum)
                            {
                                //Check if the player intersects with the rectangle
                                if (new Rectangle((int)players[j].Position.X - UNIT_SIZE / 2, (int)players[j].Position.Y - UNIT_SIZE / 2, UNIT_SIZE, UNIT_SIZE).Intersects(new Rectangle((int)b.Position.X, (int)b.Position.Y, BULLET_WIDTH, BULLET_HEIGHT)))
                                {
                                    //Decrease the players health by the damage done by the bullet
                                    players[j].Health -= b.Damage;

                                    menus[currentMenu].Texts.Add(new FloatingText(Math.Round(b.Damage, 0).ToString(), Color.Red, new Vector2(players[j].Position.X + rand.Next(-50, +50), players[j].Position.Y + rand.Next(-54, -10)), damageFont));

                                    //Set the players idleTime to 0
                                    players[j].IdleTime = 0;

                                    //Set the bullets hit value to true
                                    b.Hit = true;

                                    //If the player is not dead after being shot
                                    if (players[j].Health > 0)
                                    {
                                        //BLOOD_PER_HIT is the amount of blood particles that are created per shot
                                        for (int i = 0; i < BLOOD_PER_HIT; ++i)
                                        {
                                            //Create a blood particle
                                            Particle p = new Particle(rand.Next(PARTICLE_MIN_LIFESPAN, PARTICLE_MAX_LIFESPAN));
                                            p.Position = new Vector2((float)players[j].Position.X + UNIT_SIZE / 2, (float)players[j].Position.Y);

                                            p.Velocity = b.Direction * PARTICLE_EXPLOSION * 2;
                                            p.Velocity += new Vector2((float)rand.NextDouble() * PARTICLE_EXPLOSION, (float)rand.NextDouble() * PARTICLE_EXPLOSION);
                                            p.Velocity -= new Vector2((float)rand.NextDouble() * PARTICLE_EXPLOSION, (float)rand.NextDouble() * PARTICLE_EXPLOSION);
                                            p.Velocity.Normalize();
                                            p.Velocity *= PARTICLE_EXPLOSION;
                                            allParticles.Add(p);
                                        }
                                    }
                                    else
                                    {
                                        //Create 5 times the normal amount of blood when a unit dies
                                        for (int i = 0; i < BLOOD_PER_HIT * 10; ++i)
                                        {
                                            Particle p = new Particle(rand.Next(PARTICLE_MIN_LIFESPAN, PARTICLE_MAX_LIFESPAN));
                                            p.Position = new Vector2(players[j].Position.X, players[j].Position.Y);

                                            p.Velocity = b.Direction * PARTICLE_EXPLOSION;
                                            p.Velocity += new Vector2((float)rand.NextDouble() * PARTICLE_EXPLOSION, (float)rand.NextDouble() * PARTICLE_EXPLOSION);
                                            p.Velocity -= new Vector2((float)rand.NextDouble() * PARTICLE_EXPLOSION, (float)rand.NextDouble() * PARTICLE_EXPLOSION);
                                            p.Velocity.Normalize();
                                            p.Velocity *= PARTICLE_EXPLOSION;
                                            allParticles.Add(p);
                                        }

                                        bulletTimeLeft = BULLET_TIME_LENGTH;
                                        timeModifier = BULLET_TIME_FACTOR;

                                        //Set the player's position to {0, 0}
                                        players[j].Position = new Vector2(-1000, -1000);

                                        #region Score Management

                                        //Based on the team of the unit that died, increase and update the opposite team's score
                                        switch (players[j].TeamNum)
                                        {
                                            case 0:
                                                ++team1Score;
                                                menus[currentMenu].Texts[1].Text = "Team 2 Score: " + team1Score;

                                                //If there is a scoreLimit
                                                if (scoreLimitIndex != UNLIMITED_SCORE_LIMIT)
                                                {
                                                    //If team0Score is greater than or equal to the ScoreLimit
                                                    if (team1Score >= scoreLimit[scoreLimitIndex])
                                                    {
                                                        //Calculate where the message would go if it is meant to be centered in the middle of the screen
                                                        Vector2 centerText = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 2 -
                                                            defaultFont.MeasureString("YOU HAVE BEEN DEFEATED!") / 2;

                                                        //Set the currentMenu to END_INDEX
                                                        currentMenu = END_INDEX;

                                                        //Change the end screen text to display the appropriate outcome of the game and give it a new position
                                                        menus[currentMenu].Texts[0].Text = "YOU HAVE BEEN DEFEATED!";
                                                        menus[currentMenu].Texts[0].Position = centerText;
                                                        players = new List<Unit>();
                                                        bulletList = new List<Bullet>();
                                                        bulletTimeLeft = 0;
                                                    }
                                                }

                                                break;

                                            case 1:
                                                ++team0Score;
                                                menus[currentMenu].Texts[0].Text = "Team 1 Score: " + team0Score;

                                                //If there is a scoreLimit
                                                if (scoreLimitIndex != UNLIMITED_SCORE_LIMIT)
                                                {
                                                    //If team1Score is greater than or equal to the ScoreLimit
                                                    if (team0Score >= scoreLimit[scoreLimitIndex])
                                                    {
                                                        //Calculate where the message would go if it is meant to be centered in the middle of the screen
                                                        Vector2 centerText = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 2 -
                                                            defaultFont.MeasureString("YOU ARE VICTORIOUS!") / 2;

                                                        //Set the currentMenu to END_INDEX
                                                        currentMenu = END_INDEX;

                                                        //Change the end screen text to display the appropriate outcome of the game and give it a new position
                                                        menus[currentMenu].Texts[0].Text = "YOU ARE VICTORIOUS!";
                                                        menus[currentMenu].Texts[0].Position = centerText;
                                                        players = new List<Unit>();
                                                        bulletList = new List<Bullet>();
                                                        bulletTimeLeft = 0;
                                                    }
                                                }
                                                break;
                                        }

                                        #endregion
                                    }
                                    break;
                                }
                            }
                        }
                        Tile bTile = GetTileAtLocation(b.Position);
                        //If the bullet hits a tile that is not passable, set hit to true
                        if (!bTile.Passable)
                        {
                            b.Hit = true;

                            if (!(bTile.GridLocation.X == 0 || bTile.GridLocation.X == height - 1 ||
                                bTile.GridLocation.Y == 0 || bTile.GridLocation.Y == width - 1))
                            {
                                if (bTile.Type != TileType.GreyBricks)//Grey bricks are indestructible
                                {
                                    //Reduce from the tile's health
                                    bTile.TileHealth -= b.Damage;

                                    //If the tile health is 0 or less
                                    if (GetTileAtLocation(b.Position).TileHealth <= 0)
                                    {
                                        //Change the tile's type
                                        switch (currentMap)
                                        {
                                            case 0:
                                            case 1:
                                            case 2:
                                                GetTileAtLocation(b.Position).Type = TileType.Floor;
                                                break;
                                            default:
                                                GetTileAtLocation(b.Position).Type = TileType.Grass;
                                                break;
                                        }

                                        //Make tile passable
                                        GetTileAtLocation(b.Position).Passable = true;
                                    }
                                }
                            }
                        }
                    }

                    //If the bullet goes outside the screen or its Hit property is true, delete it
                    bulletList.RemoveAll(b => b.Position.X < 0 || b.Position.X > GraphicsDevice.Viewport.Width - TILE_SIZE || b.Position.Y < 0 || b.Position.Y > GraphicsDevice.Viewport.Height - TILE_SIZE || b.Hit);

                    #endregion

                    #region AI

                    //Go through the players list and for each unit that is an AI and is not dead, call upon AIShooting and AIMovement for that AI
                    for (int i = 0; i < players.Count; ++i)
                    {
                        if (players[i].Health > 0 && players[i] is AI)
                        {
                            AIShooting(Convert.ToInt32(gameTime.ElapsedGameTime.Milliseconds * timeModifier), i);
                            AIMovement(i);
                        }
                    }

                    #endregion

                    #region Particles

                    allParticles.RemoveAll(p => p.LifeSpan <= 0 || p.Position.X < TILE_SIZE || p.Position.X > GraphicsDevice.Viewport.Width - TILE_SIZE ||
                        p.Position.Y < TILE_SIZE || p.Position.Y > GraphicsDevice.Viewport.Height - TILE_SIZE);

                    foreach (Particle p in allParticles)
                    {
                        p.Position += p.Velocity * timeModifier;
                        Tile pTile = GetTileAtLocation(p.Position);
                        if (pTile == null || !pTile.Passable)
                        {
                            p.LifeSpan = 0;
                            //float leftCheck = Math.Abs(p.Position.X - pTile.rect.Left);
                            //float rightCheck = Math.Abs(p.Position.X - pTile.rect.Right);
                            //float upCheck = Math.Abs(p.Position.Y - pTile.rect.Top);
                            //float downCheck = Math.Abs(p.Position.Y - pTile.rect.Bottom);

                            //if ((leftCheck < upCheck && leftCheck < downCheck) || (rightCheck < upCheck && rightCheck < downCheck))
                            //{
                            //    p.Velocity *= new Vector2(-1, 1);
                            //}
                            //else
                            //{
                            //    p.Velocity *= new Vector2(1, -1);
                            //}
                        }

                        p.Velocity *= 0.98f;
                        p.LifeSpan -= Convert.ToInt32(gameTime.ElapsedGameTime.Milliseconds * timeModifier);
                    }

                    #endregion

                    #region Damages Management

                    foreach (FloatingText f in menus[currentMenu].Texts)
                    {
                        if (f.Font == damageFont)
                        {
                            Color c = f.Color;
                            c.A -= 3;
                            f.Color = c;
                            f.Position = new Vector2(f.Position.X, f.Position.Y - 2);
                        }
                    }

                    menus[currentMenu].Texts.RemoveAll(f => f.Color.A <= 20 && f.Font == damageFont);

                    #endregion

                    #endregion
                }

                else
                {
                    #region Button Presses

                    //Goes through each button in the current menu screen
                    foreach (Button b in menus[currentMenu].Buttons)
                    {
                        //Check if the mouse is hovering over the button
                        if (b.Bounds.Contains(mouse.X, mouse.Y))
                        {
                            //Set the buttons color modulation to gray
                            b.Color = Color.Gray;

                            //Check the user clicks the left mouse button
                            if (mouse.LeftButton == ButtonState.Pressed && !down)
                            {
                                //Set down to true
                                down = true;

                                //Based on the current menu and the button's ID, follow through with a certain action that pertains to the button
                                switch (currentMenu)
                                {
                                    #region Main Menu

                                    case MAIN_MENU_INDEX:

                                        switch (b.ID)
                                        {
                                            case "quit":
                                                //Quit the game
                                                this.Exit();
                                                break;

                                            default:
                                                //Set the currentMenu to button's target property
                                                currentMenu = b.Target;
                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region Options

                                    case OPTIONS_INDEX:

                                        switch (b.ID)
                                        {
                                            case "volumeUp":
                                                //Increase the volume if it is less than the maximum
                                                if (volume < 100)
                                                {
                                                    volume += 10;
                                                    menus[currentMenu].Texts[0].Text = "Volume: " + volume + "%";
                                                }
                                                break;

                                            case "volumeDown":
                                                //Decrease the volume if it is greater than the minimum
                                                if (volume > 0)
                                                {
                                                    volume -= 10;
                                                    menus[currentMenu].Texts[0].Text = "Volume: " + volume + "%";
                                                }
                                                break;

                                            default:
                                                //Else, set the currentMenu to button's target property
                                                currentMenu = b.Target;
                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region Start Screen

                                    case START_SCREEN_INDEX:

                                        switch (b.ID)
                                        {
                                            case "teamsizeUp":
                                                //Increase the teamSize if it is less than the maximum
                                                if (teamSize < TEAM_SIZE_LIMIT)
                                                {
                                                    ++teamSize;
                                                    menus[currentMenu].Texts[0].Text = "Team Size: " + teamSize;
                                                }
                                                break;

                                            case "teamsizeDown":
                                                //Decrease the teamSize if it is greater than the minimum
                                                if (teamSize > 1)
                                                {
                                                    --teamSize;
                                                    menus[currentMenu].Texts[0].Text = "Team Size: " + teamSize;
                                                }
                                                break;

                                            case "scoreUp":
                                                //Change the scoreLimitIndex and update the text decribing the score limit
                                                if (scoreLimitIndex < 4)
                                                    ++scoreLimitIndex;

                                                if (scoreLimitIndex == 4)
                                                    menus[currentMenu].Texts[1].Text = "Score Limit: Infinity";

                                                else
                                                    menus[currentMenu].Texts[1].Text = "Score Limit: " + scoreLimit[scoreLimitIndex].ToString();

                                                break;

                                            case "scoreDown":
                                                //Change the scoreLimitIndex and update the text decribing the score limit
                                                if (scoreLimitIndex > 0)
                                                    --scoreLimitIndex;

                                                if (scoreLimitIndex == 4)
                                                    menus[currentMenu].Texts[1].Text = "Score Limit: Infinity";

                                                else
                                                    menus[currentMenu].Texts[1].Text = "Score Limit: " + scoreLimit[scoreLimitIndex].ToString();

                                                break;

                                            default:
                                                //Else, set the currentMenu to button's target property
                                                currentMenu = b.Target;
                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region Choose Skin Screen

                                    case CHOOSE_SKIN_SCREEN_INDEX:

                                        switch (b.ID)
                                        {
                                            case "previousTeam0Texture":
                                                //Go to the previous texture in the playerTexture array
                                                if (team0TextureID > 0)
                                                {
                                                    --team0TextureID;
                                                }
                                                else
                                                {
                                                    team0TextureID = Convert.ToByte(playerTexture.Length - 1);
                                                }
                                                break;
                                            case "nextTeam0Texture":
                                                //Go to the next texture in the playerTexture array
                                                if (team0TextureID < playerTexture.Length - 1)
                                                {
                                                    ++team0TextureID;
                                                }
                                                else
                                                {
                                                    team0TextureID = 0;
                                                }
                                                break;
                                            case "previousTeam1Texture":
                                                //Go to the previous texture in the playerTexture array
                                                if (team1TextureID > 0)
                                                {
                                                    --team1TextureID;
                                                }
                                                else
                                                {
                                                    team1TextureID = Convert.ToByte(playerTexture.Length - 1);
                                                }
                                                break;
                                            case "nextTeam1Texture":
                                                //Go to the next texture in the playerTexture array
                                                if (team1TextureID < playerTexture.Length - 1)
                                                {
                                                    ++team1TextureID;
                                                }
                                                else
                                                {
                                                    team1TextureID = 0;
                                                }
                                                break;

                                            default:
                                                //Go to the button's target menu
                                                currentMenu = b.Target;
                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region Choose Map Screen

                                    case CHOOSE_MAP_SCREEN_INDEX:
                                        switch (b.ID)
                                        {
                                            case "gameScreen":
                                                //Reset the scores for both teams
                                                team0Score = 0;
                                                team1Score = 0;

                                                menus[GAME_SCREEN_INDEX].Texts[0].Text = "Team 1 Score: " + team0Score;
                                                menus[GAME_SCREEN_INDEX].Texts[1].Text = "Team 2 Score: " + team1Score;
                                                menus[GAME_SCREEN_INDEX].Texts.RemoveAll(f => f.Font == damageFont);

                                                //Clear the player, particle and bullet lists
                                                players.Clear();
                                                allParticles.Clear();
                                                bulletList.Clear();
                                                
                                                //Create the map, generate the players and change the current menu so the game can begin
                                                World = GenerateMap(mapFileNames[currentMap]);
                                                SpawnEntities();
                                                foreach (Unit u in players)
                                                {
                                                    u.Respawn(ReadInGun(rand.Next(0, 6)));
                                                }

                                                currentMenu = b.Target;
                                                break;

                                            case "previousMap":
                                                if (currentMap == 0)
                                                    currentMap = 3;
                                                else
                                                    --currentMap;
                                                break;

                                            case "nextMap":
                                                if (currentMap == 3)
                                                    currentMap = 0;
                                                else
                                                    ++currentMap;
                                                break;

                                            case "chooseSkinScreen":
                                                currentMenu = b.Target;
                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region End Screen

                                    case END_INDEX:
                                        //Reset the scores for both teams
                                        team0Score = 0;
                                        team1Score = 0;

                                        //Clear the player, particle and bullet lists
                                        players.Clear();
                                        allParticles.Clear();
                                        bulletList.Clear();

                                        //Go to the button's target menu
                                        currentMenu = b.Target;
                                        break;

                                    #endregion
                                }
                            }
                            //If the left mouse button is not being pressed and down is true, set down to false
                            else if (mouse.LeftButton == ButtonState.Released && down)
                                down = false;
                        }
                        //Set the button's color modulation to white
                        else
                            b.Color = Color.White;
                    }

                    #endregion
                }
            }

            base.Update(gameTime);
        }

        //Pre: none
        //Post: Ends bullet time if it is over
        //Description: Stops bullet time and modifies all it's properties appropriately if bullet time is over
        public void UpdateBulletTime(int elapsedMilliseconds)
        {
            //If bulletTimeLeft is greater than 0, decrease bulletTimeLeft set the timeModifier to the BULLET_TIME_FACTOR if it isn't set
            if (bulletTimeLeft > 0)
            {
                bulletTimeLeft -= elapsedMilliseconds;
            }
            else
            {
                timeModifier = 1.0f;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (currentMenu == GAME_SCREEN_INDEX)
            {
                #region Game Draw

                //tileLocation is the location of the tile to be drawn
                Rectangle tileLocation = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

                //Loop through the World array and draw all the tiles
                for (int y = 0; y < width; ++y)
                {
                    for (int x = 0; x < height; ++x)
                    {
                        switch (World[y, x].Type)
                        {
                            case TileType.Floor:
                                spriteBatch.Draw(floorTile, tileLocation, Color.White);
                                break;
                            case TileType.Grass:
                                spriteBatch.Draw(grassTile, tileLocation, Color.White);
                                break;
                            case TileType.Sand:
                                spriteBatch.Draw(sandTile, tileLocation, Color.White);
                                break;
                            case TileType.Bricks:
                                spriteBatch.Draw(brickTile, tileLocation, Color.White);
                                if (World[y, x].TileHealth >= (World[y, x].defaultHealth * 0.85) && World[y, x].TileHealth <= (World[y, x].defaultHealth * 0.94))
                                {
                                    spriteBatch.Draw(smallCrack, tileLocation, Color.White);
                                }
                                else if (World[y, x].TileHealth >= (World[y, x].defaultHealth * 0.4) && World[y, x].TileHealth <= (World[y, x].defaultHealth * 0.84))
                                {
                                    spriteBatch.Draw(midCrack, tileLocation, Color.White);
                                }
                                else if (World[y, x].TileHealth > 0 && World[y, x].TileHealth <= (World[y, x].defaultHealth * 0.39))
                                {
                                    spriteBatch.Draw(bigCrack, tileLocation, Color.White);
                                }
                                break;
                            case TileType.GreyBricks:
                                spriteBatch.Draw(greyBricksTile, tileLocation, Color.White);
                                break;
                            case TileType.Planks:
                                spriteBatch.Draw(planksTile, tileLocation, Color.White);
                                break;
                        }

                        tileLocation.X += TILE_SIZE;
                    }
                    tileLocation.X = 0;
                    tileLocation.Y += TILE_SIZE;
                }

                //Draw the aura around the player so the user knows who they are
                spriteBatch.Draw(aura, new Rectangle((int)players[playerIndex].Position.X - UNIT_SIZE / 2 - 4, (int)players[playerIndex].Position.Y - UNIT_SIZE / 2 - 4, UNIT_SIZE + 8, UNIT_SIZE + 8), new Color(255, 255, 255, 166));

                //Draw all the bullets
                foreach (Bullet b in bulletList)
                {
                    spriteBatch.Draw(bullet, new Rectangle((int)b.Position.X, (int)b.Position.Y, BULLET_WIDTH, BULLET_HEIGHT), null, Color.White,
                        b.Rotation, new Vector2(bullet.Width / 2, bullet.Height / 2), SpriteEffects.None, 1.0f);
                }

                //Draw all the players and their health and ammo bars if they are alive
                foreach (Unit unit in players)
                {
                    //Draw the players if they are alive
                    if (unit.Health > 0)
                    {
                        unit.Draw(spriteBatch, playerTexture[unit.TextureID], gunTexture[unit.Gun.GunID],
                            UNIT_SIZE, gunTexture[unit.Gun.GunID].Width / 2, gunTexture[unit.Gun.GunID].Height / 2);

                        //Draws their status bar
                        spriteBatch.Draw(statusBar, new Rectangle((int)(unit.Position.X - playerTexture[unit.TextureID].Width / 2), (int)(unit.Position.Y - playerTexture[unit.TextureID].Height / 2), (int)(statusBar.Width), statusBar.Height), Color.White);
                        spriteBatch.Draw(healthBar, new Rectangle((int)(unit.Position.X - playerTexture[unit.TextureID].Width / 2), (int)(unit.Position.Y + 1 - playerTexture[unit.TextureID].Height / 2), (int)(healthBar.Width * unit.Health / unit.MaxHealth), healthBar.Height), Color.White);
                        if (unit.Gun.Reloading)
                        {
                            spriteBatch.Draw(ammoBar, new Rectangle((int)(unit.Position.X - playerTexture[unit.TextureID].Width / 2), (int)(unit.Position.Y + 5 - playerTexture[unit.TextureID].Height / 2), (int)(ammoBar.Width * ((float)unit.Gun.TimePassed / (float)unit.Gun.ReloadTime)), ammoBar.Height), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(ammoBar, new Rectangle((int)(unit.Position.X - playerTexture[unit.TextureID].Width / 2), (int)(unit.Position.Y + 5 - playerTexture[unit.TextureID].Height / 2), (int)(ammoBar.Width * ((float)unit.Gun.Ammo / (float)unit.Gun.ClipCapacity)), ammoBar.Height), Color.White);
                        }
                    }
                }

                //Draw all the blood particles
                foreach (Particle p in allParticles)
                {
                    spriteBatch.Draw(blood, new Rectangle((int)p.Position.X, (int)p.Position.Y, BLOOD_SIZE, BLOOD_SIZE), Color.White);
                }

                #endregion
            }
            else
            {
                //Draw the background
                spriteBatch.Draw(menus[currentMenu].Background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

                #region Menu Draws

                //Draw all the buttons in the current menu
                foreach (Button b in menus[currentMenu].Buttons)
                {
                    spriteBatch.Draw(b.Texture, b.Bounds, b.Color);
                }

                switch (currentMenu)
                {
                    case CHOOSE_SKIN_SCREEN_INDEX:
                        spriteBatch.Draw(playerTexture[team0TextureID], new Rectangle(265, 300, playerTexture[team0TextureID].Width * 3, playerTexture[team0TextureID].Height * 3), Color.White);
                        spriteBatch.Draw(playerTexture[team1TextureID], new Rectangle(940, 300, playerTexture[team1TextureID].Width * 3, playerTexture[team1TextureID].Height * 3), Color.White);
                        break;
                    case CHOOSE_MAP_SCREEN_INDEX:
                        spriteBatch.Draw(mapPreviews[currentMap], new Rectangle(337, 50, mapPreviews[currentMap].Width / 2, mapPreviews[currentMap].Height / 2), Color.White);
                        break;
                }

                #endregion
            }

            //Draw all the text in the current menu
            foreach (FloatingText s in menus[currentMenu].Texts)
            {
                spriteBatch.DrawString(defaultFont, s.Text, s.Position, s.Color);
            }

            //Draw the crossHair so that the sights are positioned at the mouse's position
            spriteBatch.Draw(crossHair, new Rectangle(mouse.X - TILE_SIZE / 2, mouse.Y - TILE_SIZE / 2, TILE_SIZE + TILE_SIZE / 2, TILE_SIZE + TILE_SIZE / 2), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: gunIndex is a valid integer greater than or equal to 0
        //Post: Return the resulting Gun 
        //Description: Read in a gun from a txt file based on the gunIndex parameter
        public Gun ReadInGun(int gunIndex)
        {
            //GunName - Type - GunID
            //AK47 - Assault Rifle - 0
            //Desert Eagle - Handgun - 1
            //Dragunov - Sniper Rifle - 2
            //LSAT - Light Machine Gun - 3
            //MP5 - Submachine Gun - 4
            //SPAS - Shotgun  - 5

            //Based on gunIndex, choose the appropriate string for gunName
            string gunName = "";

            switch (gunIndex)
            {
                case 0:
                    gunName = "AK47"; //Assault Rifle
                    break;
                case 1:
                    gunName = "Desert Eagle"; //Handgun
                    break;
                case 2:
                    gunName = "Dragunov"; //Sniper Rifle
                    break;
                case 3:
                    gunName = "LSAT"; //Light Machine Gun
                    break;
                case 4:
                    gunName = "MP5"; //Submachine Gun
                    break;
                case 5:
                    gunName = "SPAS"; //Shotgun
                    break;
            }

            //Read in the appropriate text file and read in the properties for the gun
            System.IO.StreamReader reader = new System.IO.StreamReader("Guns\\" + gunName + ".txt");

            Gun gun = new Gun();
            gun.Damage = Convert.ToInt32(reader.ReadLine());
            gun.RateOfFire = Convert.ToInt32(reader.ReadLine());

            if (gunIndex == SHOTGUN_ID)
            {
                gun.ClipCapacity = Convert.ToInt32(reader.ReadLine()) * SHOTGUN_BULLETS;
            }
            else
            {
                gun.ClipCapacity = Convert.ToInt32(reader.ReadLine());
            }

            gun.ReloadTime = Convert.ToInt32(reader.ReadLine());
            gun.Speed = Convert.ToSingle(reader.ReadLine());
            gun.Spread = Convert.ToSingle(reader.ReadLine());
            gun.DamageFallOff = Convert.ToSingle(reader.ReadLine());

            gun.Ammo = gun.ClipCapacity;
            gun.GunID = gunIndex;
            reader.Close();

            return gun;
        }

        //Pre: elapsedMilliseconds is a valid number greater than 0, 
        //aiIndex is a valid integer that refers to the index of an AI unit in the players array
        //Post: The AI will shoot at the closest enemy that they have a clear line of sight to
        //Description: For the AI at aiIndex, see which enemy it can shoot at and if necessary go towards that enemy
        public void AIShooting(int elapsedMilliseconds, int aiIndex)
        {
            //bulletShot checks whether the AI shot a bullet
            bool bulletShot = false;

            //enemies contains the list of all alive enemy units order by distance from the AI unit
            List<Unit> enemies = new List<Unit>();
            enemies = players.FindAll(p => p.TeamNum != players[aiIndex].TeamNum);
            enemies = enemies.OrderBy(e => Vector2.Distance(players[aiIndex].Position, e.Position)).ToList(); ;
            enemies.RemoveAll(unit => unit.Health <= 0);

            //Continue to loop while enemies is not empty
            while (enemies.Count > 0)
            {
                //Shoot a test bullet at the first enemy in the enemies list
                Vector2 bulletDirection = new Vector2(enemies[0].Position.X - players[aiIndex].Position.X,
                     enemies[0].Position.Y - players[aiIndex].Position.Y);
                bulletDirection.Normalize();

                Vector2 bulletPosition = new Vector2(players[aiIndex].Position.X, players[aiIndex].Position.Y);

                //Shoot Test Bullet
                //0 - Still moving test bullet
                //1 - Hit Wall
                //2 - Hit Enemy
                byte hit = 0;

                //Continue to loop while hit until hit is not 0
                //While hit is 0, the test bullet is still travelling towards the target and has not hit them
                while (hit == 0)
                {
                    //Add the bullet's Direction to the bullet's position and check
                    bulletPosition += bulletDirection;

                    //Check if the bullet hit the target enemy
                    if (new Rectangle((int)enemies[0].Position.X - UNIT_SIZE / 2, (int)enemies[0].Position.Y - UNIT_SIZE / 2, 
                        UNIT_SIZE, UNIT_SIZE).Intersects(new Rectangle((int)bulletPosition.X, (int)bulletPosition.Y, 
                            BULLET_WIDTH, BULLET_HEIGHT)))
                    {
                        //Set hit to true, set the current AI's target to the enemy's position
                        hit = 2;
                        players[aiIndex].Target = enemies[0].Position;

                        //Call upon the Shoot subprogram
                        bulletShot = Shoot(elapsedMilliseconds, aiIndex);

                        //If the path list is empty or if the distance from the AI to the enemy is less than AI_DISTANCE_MAXIMIM
                        //using A Star Pathfinding Algorithm, find the path from the AI to the enemy
                        if (players[aiIndex].path.Count == 0 || GetDistance(new Point((int)enemies[0].Position.X, (int)enemies[0].Position.Y),
                            players[aiIndex].path.First().Rectangle.Center) < AI_DISTANCE_MAXIMUM)
                        {
                            FindPath(aiIndex, enemies[0].Position);
                            //players[aiIndex].isBusy = true;
                        }
                        break;
                    }

                    //Get the tile at the bullets position
                    Tile bulletTile = GetTileAtLocation(bulletPosition);

                    //If the tile is not passable (wall), set hit to 1 and break out of the loop
                    if (!bulletTile.Passable)
                    {
                        hit = 1;
                        break;
                    }
                }

                //If hit is either 0 or 1, remove the enemy from the enemies list
                if (hit == 0 || hit == 1)
                {
                    enemies.RemoveAt(0);
                }
                //Else, break out of the loop
                else
                {
                    break;
                }
            }

            //If bulletShot is false, set the AI's shootTime to 1 and the AI's target to { 0, 0 }
            if (!bulletShot)
            {
                players[aiIndex].ShootTime = 1;
                players[aiIndex].Target = Vector2.Zero;
            }
        }

        //Pre: a is a valid Point, b is a valid Point
        //Post: The AI will move based on its path
        //Description: For the AI at aiIndex, move the AI to the next tile in the path or generate a path for the AI to follow
        public float GetDistance(Point a, Point b)
        {
            return (float)Convert.ToDouble(Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y * (b.Y - a.Y))));
        }

        //Pre: aiIndex is a valid integer that refers to the index of an AI unit in the players array
        //Post: The AI will move towards the center of the next tile in the path
        //Description: Based on the AI's position relative to the center of the next tile in the path, move the AI towards that location
        public void AIMovement(int aiIndex)
        {
            //Check If the AI's path is not empty and the distance from the AI and the distance from first tile in the 
            //path is less that the size of a tile
            if (players[aiIndex].path.Count > 0 && Vector2.Distance(players[aiIndex].Position,
                new Vector2(players[aiIndex].path[0].Rectangle.Center.X, players[aiIndex].path[0].Rectangle.Center.Y)) > TILE_SIZE)
            {
                //If the distance from the AI's position to the last tile in the path is less than its movement speed, 
                //remove the tile from the list
                if (Vector2.Distance(players[aiIndex].Position, new Vector2(players[aiIndex].path.Last().Rectangle.Center.X, 
                    players[aiIndex].path.Last().Rectangle.Center.Y)) < players[aiIndex].Gun.Speed * timeModifier)
                {
                    players[aiIndex].path.Remove(players[aiIndex].path.Last());
                }
                //Else, move the player towards the center of the last tile
                else
                {
                    //Calculate the direcion the AI must move to get to the center of the last tile in the path
                    Vector2 dir = new Vector2(players[aiIndex].path[players[aiIndex].path.Count - 1].Rectangle.Center.X - players[aiIndex].Position.X,
                        players[aiIndex].path[players[aiIndex].path.Count - 1].Rectangle.Center.Y - players[aiIndex].Position.Y);

                    //If the absolute value of the Y direction is greater than the absolute value of the X direction
                    if (Math.Abs(dir.Y) > Math.Abs(dir.X))
                    {
                        //AI Move Down
                        if (dir.Y > 0)
                        {
                            players[aiIndex].Position += new Vector2(0, players[aiIndex].Gun.Speed * timeModifier);
                            if (players[aiIndex].Gun.TimePassed > players[aiIndex].Gun.RateOfFire)
                            {
                                players[aiIndex].Rotation = MathHelper.Pi;
                            }
                        }
                        //AI Move Up
                        else if (dir.Y < 0)
                        {
                            players[aiIndex].Position += new Vector2(0, -players[aiIndex].Gun.Speed * timeModifier);
                            if (players[aiIndex].Gun.TimePassed > players[aiIndex].Gun.RateOfFire)
                            {
                                players[aiIndex].Rotation = 0;
                            }
                        }
                    }
                    else
                    {
                        //AI Move Right
                        if (dir.X > 0)
                        {
                            players[aiIndex].Position += new Vector2(players[aiIndex].Gun.Speed * timeModifier, 0);
                            if (players[aiIndex].Gun.TimePassed > players[aiIndex].Gun.RateOfFire)
                            {
                                players[aiIndex].Rotation = MathHelper.PiOver2;
                            }
                        }
                        //AI Move Left
                        else if (dir.X < 0)
                        {
                            players[aiIndex].Position += new Vector2(-players[aiIndex].Gun.Speed * timeModifier, 0);
                            if (players[aiIndex].Gun.TimePassed > players[aiIndex].Gun.RateOfFire)
                            {
                                players[aiIndex].Rotation = 3f * MathHelper.PiOver2;
                            }
                        }
                    }
                }
            }
            else
            {
                //Set the AI's target to 0,0 and find the path to a random tile within a certain range that is passable
                players[aiIndex].Target = Vector2.Zero;
                while (true)
                {
                    Tile randTile = World[rand.Next(0, width), rand.Next(0, height)];
                    if (randTile.Passable && Vector2.Distance(players[aiIndex].Position, new Vector2(randTile.Rectangle.Center.X, randTile.Rectangle.Center.Y)) < GraphicsDevice.Viewport.Width / 3)
                    {
                        FindPath(aiIndex, new Vector2(randTile.Rectangle.Center.X, randTile.Rectangle.Center.Y));
                        //players[aiIndex].isBusy = false;
                        break;
                    }
                }
            }
        }

        //Pre: file
        //Post: The World will be returned as a 2D array of tiles
        //Description: Based on the AI's position relative to the center of the next tile in the path, move the AI towards that location
        private Tile[,] GenerateMap(string file)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(file);

            Tile[,] map;

            //Read in the height and the width of the map
            height = Convert.ToInt32(reader.ReadLine());
            width = Convert.ToInt32(reader.ReadLine());

            map = new Tile[width, height];

            //Read in the RGB values and create a new tile at a certain location based on thh read in values
            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < height; ++x)
                {
                    byte red = Convert.ToByte(reader.ReadLine());
                    byte green = Convert.ToByte(reader.ReadLine());
                    byte blue = Convert.ToByte(reader.ReadLine());

                    map[y, x] = new Tile(red, green, y, x, TILE_SIZE);
                }
            }
            
            //Return the map
            return map;
        }

        //Pre: none
        //Post: The path from the enemy AI to the end location is stored in the List<Tile> path
        //Description: Calculate the path from the enemy AI to the end location using A Star Pathfinding Algorithm
        private void FindPath(int aiIndex, Vector2 endPosition)
        {
            //Set the Previous of all tiles to null
            foreach (Tile t in World)
            {
                t.Previous = null;
            }

            players[aiIndex].path = new List<Tile>();
            List<Tile> openList = new List<Tile>();
            List<Tile> closedList = new List<Tile>();
            bool pathFound = false;

            //Add the Start Tile to the openList
            openList.Add(GetTileAtLocation(players[aiIndex].Position));

            //Continue to loop while the openList contains more than one element and the last element in the closedList is not the End Tile
            while (openList.Count > 0 || closedList.Last() == GetTileAtLocation(players[aiIndex].Position))
            {
                Tile current = openList[0];

                // If current tile is the goal tile, the path was found
                if (current == GetTileAtLocation(endPosition))
                {
                    pathFound = true;
                    break;
                }
                else
                {
                    //Remove current from the openList and add it to the closedList
                    openList.Remove(current);
                    closedList.Add(current);

                    //neighbours holds a list of adjacent tiles to the current tile
                    List<Tile> neighbours = GetNeighbours(current);

                    //Go through each tile
                    foreach (Tile t in neighbours)
                    {
                        //If the tile is not a wall and isn't in the closedList
                        if (t.Passable == true && !closedList.Contains(t))
                        {
                            //If the tile isn't in the openList
                            if (!openList.Contains(t))
                            {
                                //Calculate the F and H Scores (HScore was already calculated) and add it to the openList
                                t.CalculateHScore(GetTileAtLocation(endPosition).GridLocation);
                                openList.Add(t);
                            }
                            //If the tile is in the openList
                            else
                            {
                                //Find the index of the matching tile in openList
                                int index = openList.IndexOf(t);

                                //If the currentTile has a lower GScore than the GScore in the  openList, set the Previous tile of the tile in the openList to current
                                if (t.GScore < openList[index].GScore)
                                {
                                    openList[index].Previous = current;
                                }
                            }
                            //Calculate the GScore of the currentTile
                            t.CalculateGScore(GetTileAtLocation(players[aiIndex].Position).GridLocation, current);

                            //Set the tile's previous Tile to current
                            t.Previous = current;
                        }
                    }

                    //Sort the tiles in openList by their FScore
                    openList.OrderBy(tile => tile.FScore);
                }
            }
            //If a pathFound is true
            if (pathFound)
            {
                //Add the tiles along the path to the path list
                Tile currentTile = GetTileAtLocation(endPosition);
                players[aiIndex].path.Clear();
                while (currentTile.Previous != null)
                {
                    players[aiIndex].path.Add(currentTile);
                    currentTile = currentTile.Previous;
                }
            }
        }

        //Pre: tile is a proper Vector2
        //Post: The Tile in World[,] at tile location is returned
        //Description: Find out what Tile is at tile location in the World and return it
        private Tile GetTileAtLocation(Vector2 tile)
        {
            try
            {
                return World[(int)tile.Y / TILE_SIZE, (int)tile.X / TILE_SIZE];
            }
            catch (IndexOutOfRangeException)
            {
                return World[0, 0];
            }
        }

        //Pre: tile is a proper Vector2
        //Post: The Tile in World[,] at tile location is returned
        //Description: Find out what Tile is at tile location in the World and return it
        private void CheckMovement(Vector2 positionChange)
        {
            //newPlayerRect is the player's new location if their move is valid
            Rectangle newPlayerRect = new Rectangle(Convert.ToInt32(players[playerIndex].Position.X + positionChange.X) - (UNIT_SIZE / 2),
                Convert.ToInt32(players[playerIndex].Position.Y + positionChange.Y) - (UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE);
            
            //destTile is the tile which the player will end up at if their move is valid
            Tile destTile = GetTileAtLocation(players[playerIndex].Position + positionChange);

            //neighbours is the list of the four adjacent tiles around the destTile
            List<Tile> neighbours = GetNeighbours(destTile);

            //count is the number of adjacent tiles that are passable or don't intersect with the player
            int count = 0;

            //Go through each tile in neighbours and see if each tile is either passable or doesn't intersect with the player
            //For each tile that satisfies this criteria, increment count
            foreach (Tile t in neighbours)
            {
                if (t.Passable || newPlayerRect.Intersects(t.Rectangle) == false)
                {
                    ++count;
                }
            }

            //If all the tiles in neighbours satisfy the condition, the move is valid
            if (count == neighbours.Count)
            {
                players[playerIndex].Position += positionChange;
            }
        }

        //Pre: current is the current tile whose adjacent tiles will be returned
        //Post: Return the adjacent tiles in a List<Tile>
        //Description: Finds all the adjacent tiles to the current tile
        private List<Tile> GetNeighbours(Tile current)
        {
            //neighbours is the list of adjacent tiles to the current tile
            List<Tile> neighbours = new List<Tile>();

            //currentLoc holds the grid location of the current tile
            Point currentLoc = current.GridLocation;

            //Adds the adjacent tiles while checking if the current tile is against a wall
            if (currentLoc.X > 0)
            {
                neighbours.Add(World[(int)currentLoc.Y, (int)currentLoc.X - 1]);
            }
            if (currentLoc.X < height - 1)
            {
                neighbours.Add(World[(int)currentLoc.Y, (int)currentLoc.X + 1]);
            }
            if (currentLoc.Y > 0)
            {
                neighbours.Add(World[(int)currentLoc.Y - 1, (int)currentLoc.X]);
            }
            if (currentLoc.Y < width - 1)
            {
                neighbours.Add(World[(int)currentLoc.Y + 1, (int)currentLoc.X]);
            }

            //Return the list of adjacent tiles
            return neighbours;
        }

        //Pre: none
        //Post: Respawns all units that have been "dead" for more than the RESPAWN_TIME
        //Description: Goes through all the "dead" players and respawns those who have been "dead" for more than the RESPAWN_TIME
        private void RespawnCheck()
        {
            //Checks if any players are dead
            if (players.Any(p => p.Health <= 0))
            {
                //Goes through each dead player and respawns them if their idleTime is  greater than or equal to RESPAWN_TIME
                foreach (Unit u in players.FindAll(u => u.Health <= 0))
                {
                    if (u.IdleTime >= RESPAWN_TIME)
                    {
                        u.Respawn(ReadInGun(rand.Next(0, 6)));
                    }
                }
            }
        }

        //Pre: elapsedMilliseconds is a valid integer greater than 0, unitIndex is the index of the unit in the players array
        //Post: A boolean is returned showing whether a shot has actually been fired
        //Description: Shoots a bullet from the current Unit
        private bool Shoot(int elapsedMilliseconds, int unitIndex)
        {
            //shoot checks whether a bullet has been fired
            bool shoot = false;

            //numOfBullets is the number of bullets being fired from the gun
            byte numOfBullets = 1;

            //If the gun is a shotgun, change numOfBullets to SHOTGUN_BULLETS
            if (players[unitIndex].Gun.GunID == SHOTGUN_ID)
                numOfBullets = SHOTGUN_BULLETS;

            //For each bullet being fired, shoot the bullet and add it to the bulletList
            for (int i = 0; i < numOfBullets; ++i)
            {
                Bullet b = players[unitIndex].Shoot(elapsedMilliseconds, gunTexture[players[unitIndex].Gun.GunID]);
                if (b != null)
                {
                    shoot = true;
                    bulletList.Add(b);
                    if (i != numOfBullets - 1 && players[unitIndex].Gun.GunID == SHOTGUN_ID)
                        players[unitIndex].Gun.TimePassed = players[unitIndex].Gun.RateOfFire;
                }
                else
                {
                    break;
                }
            }

            //If a bullet was shot, play a sound that corresponds to that gun
            if (shoot)
            {
                switch (players[unitIndex].Gun.GunID)
                {
                    case 0:
                        ak47Sound.Play(volume / 100f, 0f, 0f);
                        break;
                    case 4:
                        mediumGun.Play(volume / 100f, 0f, 0f);
                        break;
                    case 1:
                    case 3:
                        smallGun.Play(volume / 100f, 0f, 0f);
                        break;
                    case 2:
                    case 5:
                        largeGun.Play(volume / 100f, 0f, 0f);
                        break;
                }
            }

            //Return the shoot boolean
            return shoot;
        }
    }
}