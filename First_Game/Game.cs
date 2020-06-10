using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//install RLNET and RogueSharp in Nuget Extensions
using RLNET;

using First_Game.Systems;
//include new core folder
using First_Game.Core;
using RogueSharp.Random;

namespace First_Game
{
    public class Game
    {
        // set the screen height and width
        //make sure the subconsole sizes you create fit appropriatley with the main screen
        private static readonly int _screenWidth = 120;
        private static readonly int _screenHeight = 70;

        private static RLRootConsole _rootConsole;

        //The console for the map that we are using that will take most of the mainscreen
        private static readonly int _mapWidth = 100;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        //This console is for displaying attack rolls and other information
        private static readonly int _messageWidth = 100;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        //this console is too display the player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        //This console is too display your current inventory
        private static readonly int _inventoryWidth = 100;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;

        private static bool _renderRequired = true;

        //private static int _steps = 0;

        public static CommandSystem commandSystem { get; private set; }

        public static IRandom random { get; private set; }

        //add player to the game
        public static Player player { get; set; }

        //add dungeon map too the game
        public static DungeonMap DungeonMap { get; private set; }

        public static MessageLog messageLog { get; private set; }

        public static SchedulingSystem SchedulingSystem { get; private set; }


        static void Main(string[] args)
        {
            //name of bitmap font file
            string fontFileName = "Images/terminal8x8.png";

            //UTCNOW is the current time on this computer
            //Ticks are the number of nanoseconds since the start of year 0001 too now
            int seed = (int)DateTime.UtcNow.Ticks;

            //DotNetRandom is a random number generator that is part of .Net
            random = new DotNetRandom(seed);

            // The title will appear at the top of the console window including the seed used too generate the level
            string consoleTitle = $"RougeSharp V3 Tutorial -  Level 1 - Seed {seed}";

            SchedulingSystem = new SchedulingSystem();

            messageLog = new MessageLog();

            //adds these 2 lines of messages too the message log
            messageLog.Add("The rogue arrives at level 1");
            messageLog.Add($"Level created with seed '{seed}'");


            //Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);

            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            commandSystem = new CommandSystem();

            

            //construct a new dungeon map with the size of the map the number of rooms you want and the max and min sizes of those rooms
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7 );
            DungeonMap = mapGenerator.CreateMap();

            //add the players field of view
            DungeonMap.UpdatePlayerFieldOfView();



            //Method for RNL Update
            _rootConsole.Update += OnRootConsoleUpdate;

            //Method for RNL Update
            _rootConsole.Render += OnRootConsoleRender;

            //begin RLNET's game loop
            _rootConsole.Run();

        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            /*Blit the sub consoles that I just created too the correct locations on the app
            The parameters of a blit are 
            1. the source console
            2. the x position 3. the y position
            4. The width 5. the height 
            6. a destination console to blit to
            7. the blit destination of the top left corner of where we will blit to in the destination console
            */

            //if a render is required draw everything otherwise don't
            if (_renderRequired)
            {
                _mapConsole.Clear();
                _statConsole.Clear();
                _messageConsole.Clear();

                DungeonMap.Draw(_mapConsole,_statConsole);

                //draw the player into the map
                player.Draw(_mapConsole, DungeonMap);

                player.DrawStats(_statConsole);

                messageLog.Draw(_messageConsole);

                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);

                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);

                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight);

                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);


                _rootConsole.Draw();

                _renderRequired = false;
            }

           


        }

        private static void OnRootConsoleUpdate (object sender, UpdateEventArgs e)
        {
            // draw the console that we setup

            //Write this code at the start too check if your console update method has actually worked first
            //_rootConsole.Print(10, 10, "It worked", RLColor.Green);

            bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (commandSystem.IsPlayerTurn)
            {

                //if you have pressed a key
                if (keyPress != null)
                {
                    //checks what key you have pressed if it was up down left or right and depending on the key
                    //uses the command system class too find you new position on the map
                    if (keyPress.Key == RLKey.Up)
                    {
                        didPlayerAct = commandSystem.MovePlayer(Direction.Up);
                    }
                    else if (keyPress.Key == RLKey.Down)
                    {
                        didPlayerAct = commandSystem.MovePlayer(Direction.Down);
                    }
                    else if (keyPress.Key == RLKey.Left)
                    {
                        didPlayerAct = commandSystem.MovePlayer(Direction.Left);
                    }
                    else if (keyPress.Key == RLKey.Right)
                    {
                        didPlayerAct = commandSystem.MovePlayer(Direction.Right);
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                }
            }
            if (didPlayerAct)
            {
                //counts the steps player took
                //messageLog.Add($"Step # {++_steps}");

                _renderRequired = true;
                commandSystem.EndPlayerTurn();
            }
            else
            {
                commandSystem.ActivateMonsters();
                _renderRequired = true;
            }


            //set the background color and text for each console except the map and message consoles
            //_messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, RLColor.Gray);

            //_statConsole.SetBackColor(0, 0, _statWidth, _statHeight, RLColor.Red);
            //_statConsole.Print(1, 1, "Stats", Colors.TextHeading);
            
            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, RLColor.Cyan);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

        }
    }
}
