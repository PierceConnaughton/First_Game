using First_Game.Interfaces;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Game.Core
{
    public class Stairs : IDrawable
    {
        //added color and symbols too reprsent the stairs added X and Y positons 
        //for the stairs and a bool too check if the stairs is going up or down
        public RLColor Color { get; set; }

        public char Symbol { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool IsUp { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            //if the door has not been seen 
            if (!map.GetCell(X,Y).IsExplored)
            {
                return;
            }

            //change the stairs symbol from less than too greater than
            Symbol = IsUp ? '<' : '>';

            //if the door has been seen
            if (map.IsInFov(X,Y))
            {
                Color = Colors.Player;
            }
            //if the door has been found
            else
            {
                Color = Colors.Floor;
            }

            console.Set(X, Y, Color, null, Symbol);
        }
    }
}
