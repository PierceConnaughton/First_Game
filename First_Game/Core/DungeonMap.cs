using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using First_Game.Core;
using RLNET;
using RogueSharp;
using First_Game.Systems;
using First_Game.Interfaces;
//make sure too include rogue sharp this time

namespace First_Game.Core
{
    //DungeonMap is our sub class we created of the base class that was created by rouge sharp
    public class DungeonMap : Map
    {
        #region Prop
        public Stairs StairsUp { get; set; }
        public Stairs StairsDown { get; set; }

        //create a list of rooms
        public List<Rectangle> Rooms { get; set; }

        private readonly List<Monster> _monsters;

        private readonly List<TreasurePile> _treasurePiles;

        public List<Door> Doors { get; set; }

        #endregion Prop

        #region Constructor
        public DungeonMap()
        {
            //clear the schedule for monsters everytime you change levels
            Game.SchedulingSystem.Clear();

            //everytime we create a dungeon map we have a list of rooms created and monsters created
            Rooms = new List<Rectangle>();

            _monsters = new List<Monster>();

            _treasurePiles = new List<TreasurePile>();

            Doors = new List<Door>();
        }

        #endregion Constructor



        #region Methods

        //The Draw method will be called each time the map is updated
        //It will render all of the symbols or colors for each cell to the map sub console we created earlier
        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            //ineffecient too redraw code everytime
            //clear what was previously on the map console
            //mapConsole.Clear();

            

            //foreach of the cells in the 
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }

            foreach (Door door in Doors)
            {
                door.Draw(mapConsole, this);
            }

            StairsUp.Draw(mapConsole, this);
            StairsDown.Draw(mapConsole, this);

            //keep an index so we know which position to draw monster stats at
            int i = 0;

            foreach (TreasurePile treasurePile in _treasurePiles)
            {
                IDrawable drawableTreasure = treasurePile.Treasure as IDrawable;
                drawableTreasure?.Draw(mapConsole, this);
            }

            foreach (Monster monster in _monsters)
            {
                monster.Draw( mapConsole,this);

                //if a monster is in field of view draw there stats too the stat console
                if (IsInFov(monster.X,monster.Y))
                {
                    monster.DrawStats(statConsole, i);
                    i++;
                }
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
                PiclUpTreasure(actor, x, y);

                //sets the previous actor's cell positon too walkable
                SetIsWalkable(actor.X, actor.Y, true);

                //Update the actors position
                actor.X = x;
                actor.Y = y;

                //makes the current cellthe actor is on too not walkable
                SetIsWalkable(actor.X, actor.Y, false);

                //try to open door if a door exists in this new position
                OpenDoor(actor, x, y);

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

        //whenever we create a map we use this method after too add a player too the map
        public void AddPlayer( Player player)
        {
            Game.player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game.SchedulingSystem.Add(player);
        }

        //whenever we create a map we add monsters too it after
        public void AddMonster( Monster monster)
        {
            //after adding a monster too the map we check that the cell is not walkable that they are now on
            _monsters.Add(monster);

            SetIsWalkable(monster.X, monster.Y, false);

            Game.SchedulingSystem.Add(monster);

        }

        //removes monster from map and makes space available
        public void RemoveMonster (Monster monster)
        {
            //remove the monster 
            _monsters.Remove(monster);

            //after removing the monster from the map, make sure the cell is available again
            SetIsWalkable(monster.X, monster.Y, true);

            Game.SchedulingSystem.Remove(monster);

        }

        //gets the monster at a particular locaton using coordinates
        public Monster GetMonsterAt(int x,int y)
        {    
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        //we use this method too find a random cell in the map that is walkable for the monster too spawn in
        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.random.Next(1, room.Width - 2) + room.X;
                    int y = Game.random.Next(1, room.Width - 2) + room.Y;

                    //glitch here
                    if (IsWalkable(x,y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            //if a cell couldnt be found return back its default point value which would be null
            return default(Point);
        }

        //go through each cell in the room and return true if it finds a cell that is walkable otherwise return false
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x < room.Width - 2; x++)
            {
                for (int y = 1; y < room.Height - 2; y++)
                {
                    if (IsWalkable (x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Returns the door at the x,y position or null if one is not found.
        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        //actor opens the door at the x y position
        private void OpenDoor(Actor actor, int x, int y)
        {
            //check if door is at that positon
            Door door = GetDoor(x, y);

            //if door is found and is not open
            if (door != null && !door.IsOpen)
            {
                //opens the door
                door.IsOpen = true;

                //gets the cell of the door position
                var cell = GetCell(x, y);

                //once the door is open it should be transparent and not block field of view
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);

                Game.messageLog.Add($"{actor.Name} opened a door");
            }
        }

        //check if player is on a stairs going down cell
        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.player;
            return StairsDown.X == player.X && StairsDown.Y == player.Y;
        }

        public void AddTreasure(int x, int y, ITreasure treasure)
        {
            _treasurePiles.Add(new TreasurePile(x, y, treasure));
        }

        public void AddGold(int x, int y, int amount)
        {
            if (amount > 0)
            {
                AddTreasure(x, y, new Gold(amount));
            }
        }

        private void PiclUpTreasure(Actor actor, int x, int y)
        {
            List<TreasurePile> treasureAtLocation = _treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
            foreach (TreasurePile treasurePile in treasureAtLocation)
            {
                if (treasurePile.Treasure.PickUp(actor))
                {
                    _treasurePiles.Remove(treasurePile);
                }
            }
            
        }

        #endregion Methods




    }
}
