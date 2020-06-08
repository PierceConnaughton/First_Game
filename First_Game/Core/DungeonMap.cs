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

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            //When a cell hasnt been explored yet we will return
            if (!cell.IsExplored)
            {
                return;
            }

            //when a cell is in the field of view it should be drawn in lighter colors
            if (IsInFov(cell.X, cell.Y))
            {

                //choose the symbol to draw based on if the cell is walkable or not
                // '.' for floor and # for walls
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');

                }

            }
            //when cell is outside view use darker colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');  
                }
            }
            

        }
    }
}
