using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//always include RLNET
using RLNET;

namespace First_Game.Core
{
    public class Colors
    {
        //color of floors when in view and out of view
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        //color of walls when in view and out of view
        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        //color of player 
        public static RLColor Player = Swatch.DbLight;

        //color of text
        public static RLColor TextHeading = RLColor.White;

        
        public static RLColor Text = Swatch.DbLight;

        public static RLColor Gold = Swatch.DbSun;

        //color of first monster of the game
        public static RLColor KoboldColor = RLColor.Red;
    }
}
