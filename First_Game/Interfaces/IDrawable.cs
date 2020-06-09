using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RLNET;
using RogueSharp;
using First_Game.Interfaces;

namespace First_Game.Interfaces
{
    public interface IDrawable
    {
        //properties too draw a cell are a color symbol and x and y coordinates
        RLColor Color { get; set; }
        char Symbol { get; set; }

        int X { get; set; }

        int Y { get; set; }

        //Draw method that will take the RLConsole to draw to as well an IMap
        void Draw(RLConsole console, IMap map);
        
    }
}
