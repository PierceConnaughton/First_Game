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

namespace First_Game
{
    public class Game
    {
        // set the screen height and width
        //make sure the subconsole sizes you create fit appropriatley with the main screen
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;

        private static RLRootConsole _rootConsole;

        //The console for the map that we are using that will take most of the mainscreen
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        //This console is for displaying attack rolls and other information
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        //this console is too display the player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        //This console is too display your current inventory
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;

        public static DungeonMap DungeonMap { get; private set; }


        static void Main(string[] args)
        {
            //name of bitmap font file
            string fontFileName = "Images/terminal8x8.png";

            // The title will appear at the top of the console window
            string consoleTitle = "RougeSharp V3 Tutorial -  Level 1";

            //Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);

            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight);
            DungeonMap = mapGenerator.CreateMap();

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
            RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);

            RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);

            RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight);

            RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);

            DungeonMap.Draw(_mapConsole);

            _rootConsole.Draw();


        }

        private static void OnRootConsoleUpdate (object sender, UpdateEventArgs e)
        {
            // draw the console that we setup

            //Write this code at the start too check if your console update method has actually worked first
            //_rootConsole.Print(10, 10, "It worked", RLColor.Green);

            //set the background color and text for each console

            _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, RLColor.Black);
            _mapConsole.Print(1, 1, "", Colors.TextHeading);

            _messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, RLColor.Gray);
            _messageConsole.Print(1, 1, "Messages", Colors.TextHeading);

            _statConsole.SetBackColor(0, 0, _statWidth, _statHeight, RLColor.Red);
            _statConsole.Print(1, 1, "Stats", Colors.TextHeading);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, RLColor.Cyan);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

        }
    }
}
