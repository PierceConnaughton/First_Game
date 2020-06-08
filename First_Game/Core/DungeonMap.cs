using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RLNET;
using RogueSharp;
//make sure too include rogue sharp this time

namespace First_Game.Core
{
    //DungeonMap is our sub class we created of the base class that was created by rouge sharp
    public class DungeonMap : Map
    {
        //The Draw method will be called each time the map is updated
        //It will render all of the symbols or colors for each cell to the map sub console we created earlier

        public void Draw(RLConsole mapConsole)
        {
            //clear what was previously on the map console
            mapConsole.Clear();

            //foreach of the cells in the 
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        private void SetConsoleSymbolForCell(RLConsole mapConsole, Cell cell)
        {
            if (!cell.IsExplored)
            {

            }
        }
    }
}
