using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Game.Core
{
    //monster is a subclass of an actor
    public class Monster : Actor
    {
        public void DrawStats(RLConsole statConsole, int position)
        {
            //start position at Y = 13 which is located below the player stats.
            //Multiply the position by 2 to leave space between each stat
            int yPosition = 13 + (position * 2);

            //prints the symbol of the monster in the appropriate color
            statConsole.Print(1, yPosition, Symbol.ToString(), Color);

            //Figure out the width of the health bar by dividing the current health by the max health
            int width = Convert.ToInt32(((double)Health/ (double)MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            //set the background colors of the health bar to show how damaged the monster is
            //the background color is the max health
            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary);
            //the second background color is the current health
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest);

            //prints the monsters name over the top of there health bar
            statConsole.Print(2, yPosition, $": {Name}", Swatch.DbLight);

        }

        
    }
}
