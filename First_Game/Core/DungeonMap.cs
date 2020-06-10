using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using First_Game.Core;
using RLNET;
using RogueSharp;
//make sure too include rogue sharp this time

namespace First_Game.Core
{
    //DungeonMap is our sub class we created of the base class that was created by rouge sharp
    public class DungeonMap : Map
    {
        //create a list of rooms
        public List<Rectangle> Rooms;

        public DungeonMap()
        {
            //everytime we create a dungeon map we have a list of rooms created
            Rooms = new List<Rectangle>();
        }

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

        //anytime the player moves we call this method
        public void UpdatePlayerFieldOfView()
        {
            //get the current player
            Player player = Game.player;

            //get the field of view for this player based on there current awarness
            ComputeFov(player.X, player.Y, player.Awareness, true);

            // Mark all cells of field of view as having been explored
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov( cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        //this method checks that the previous cell the actor was on is now alkable and the current cell
        //the actor is on is not walkable

        //returns true when able to place the Actor on the cell and returns false if it can't
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            //only allow actor too move too new position if the cell is walkable
            if (GetCell(x,y).IsWalkable)
            {
                //sets the previous actor's cell positon too walkable
                SetIsWalkable(actor.X, actor.Y, true);

                //Update the actors position
                actor.X = x;
                actor.Y = y;

                //makes the current cellthe actor is on too not walkable
                SetIsWalkable(actor.X, actor.Y, false);

                //if the actor is part of the player subclass update the players field of view
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }

                return true;
            }

            return false;
        }

        //this method sets the cell the actor is on too non walkable
        public void SetIsWalkable( int x, int y, bool isWalkable)
        {
            ICell cell = GetCell(x,y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }
    }
}
