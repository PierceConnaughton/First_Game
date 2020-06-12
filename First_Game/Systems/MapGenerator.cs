using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RLNET;
using RogueSharp;
using First_Game.Core;
using RogueSharp.DiceNotation;
using First_Game.Monsters;

namespace First_Game.Systems
{
    public class MapGenerator
    {
        #region Prop
        private readonly int _width;
        private readonly int _height;

        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly DungeonMap _map;

        #endregion Prop

        #region Constructor
        //Constructing a new map generator requires the dimensions of the maps it will create
        //as well as the sizes of rooms and maximum number of rooms
        public MapGenerator( int width, int height, int maxRooms, int roomMaxSize, int roomMinSize,int mapLevel)
        {       
             _width = width;
             _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        #endregion Constructor

        #region Methods
        //Generate a new map that places rooms randomly around the map
        public DungeonMap CreateMap()
        {
            // Initialize every cell in the map 
            _map.Initialize(_width, _height);

            //go through creating each room for the particular map 
            //until you reach that maps max number of rooms needed
            for (int r = 0; r < _maxRooms; r++)
            {

                //determine the sizes and position of the room randomly
                int roomWidth = Game.random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.random.Next(_roomMinSize, _roomMaxSize);

                int roomXPosition = Game.random.Next(0,_width - roomWidth -1);
                int roomYPosition = Game.random.Next(0, _height - roomHeight - 1);

                //all of our rooms that we are generating are gonna be represented by rectangles
                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                //checks too see if rooms intersect with others
                bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                //if room doesn't intersect add the room too the list of rooms
                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }

                //go through each room we want and use create room method 
                //too make that particular room
                foreach (Rectangle room in _map.Rooms)
                {
                    CreateRoom(room);
                    
                }

            }

            //go through all rooms that were generated except for the first room
            for (int r = 1; r < _map.Rooms.Count; r++)
            {

                //find the x and y axis of the room we are currently in and
                //the x and y axis of the room previous
                int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X;
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                //sets up a 50/50 chance of which tunnel we should make
                if (Game.random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            foreach (Rectangle room in _map.Rooms)
            {
                CreateDoors(room);
            }

            CreateStairs();

            //once rooms and tunnels are generated we add the player too the map
            PlacePlayer();

            PlaceMonsters();

            return _map;



            #region OldMap
            //used this at the beging too make sure the map works by implementing a big empty room on the map
            /*foreach (Cell cell in _map.GetAllCells())
            {
                _map.SetCellProperties(cell.X, cell.Y, true, true, true);
            }

            //Set the first and last rows in the map to not be transparent or walkable
            foreach (Cell cell in _map.GetCellsInRows(0, _height - 1))
            {
                _map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }

            //set the first and last coloumns in the map to not be transparent or walkable
            foreach (Cell cell in _map.GetCellsInColumns(0, _width - 1))
            {
                _map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }
            

            return _map;
            */
            #endregion OldMap
        }

        //set the cell properties of the room we are creating too true 
        //so that we can walk on them
        private void CreateRoom(Rectangle room)
        {
            //+1 too make sure there are space between rooms
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    //set is explored too false when rooms are created so you can't first see them when you start the game
                    _map.SetCellProperties(x, y, true, true, false);
                }
            }
        }

        //finds the center of the first room we created on the map and places that player there
        private void PlacePlayer()
        {
            Player player = Game.player;
            if (player == null)
            {
                player = new Player();
            }

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        //this method goes through each room and rolls a 10-sided die.
        //if the result of thr roll is 1 - 7 we'll roll 4 sided die and 
        //add that many monsters too that particular room in any open cells
        private void PlaceMonsters()
        {

            foreach (var room in _map.Rooms)
            {

                if (Dice.Roll("1D10") < 8)
                {
                    var numberOfMonsters = Dice.Roll("1D3");

                    //start i at 1 because we dont want monsters in first room we enter
                    for (int i = 1; i < numberOfMonsters; i++)
                    {
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);

                        if (randomRoomLocation != null)
                        {
                            var monster = Kobold.Create(1);
                            monster.X = randomRoomLocation.X;
                            monster.Y = randomRoomLocation.Y;
                            _map.AddMonster(monster);
                        }
                    }
                }



            }
        }

        //creates a tunnel out of the map parallel too the x-axis too the next room
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart,xEnd); x < Math.Max(xStart,xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        //creates a tunnel out of the map parrallel too the y-axis too the next room
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y < Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }

        //Create Doors
        private void CreateDoors(Rectangle room)
        {
            //the boundaries of the room
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            //put the room border cells into a list
            List<ICell> borderCells = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            //Go through each room of the border cells and look for locations too place doors
            foreach (Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    //if door is closed it must block te field of view
                    //done at the start because all doors will start off closed 
                    _map.SetCellProperties(cell.X, cell.Y, false, true);

                    //add door at this position and check that the door is closed
                    _map.Doors.Add(new Door
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpen = false
                    }); ;

                }
            }

        }

        //Checks to see if a cell is a good place for a door
        private bool IsPotentialDoor(ICell cell)
        {
            //If the cell is not walkable
            //then it is a wall and not a good place for a door
            if (!cell.IsWalkable)
            {
                return false;
            }

            //check all of the neighbouring cells
            ICell right = _map.GetCell(cell.X + 1, cell.Y);
            ICell left = _map.GetCell(cell.X - 1, cell.Y);
            ICell top = _map.GetCell(cell.X, cell.Y - 1);
            ICell bottom = _map.GetCell(cell.X + 1, cell.Y +1);

            //make sure that a door is not already there
            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null )
            {
                return false;
            }

            //good place for a door on the right or left of the room
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }

            //good place for a door on the top or bottom of the room
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;

            }
            return false;

        }
        
        //creates the stairs
        private void CreateStairs()
        {
            //creates stairs going up in the first room made next too the player
            _map.StairsUp = new Stairs
            {
                X = _map.Rooms.First().Center.X + 1,
                Y = _map.Rooms.First().Center.Y,
                IsUp = true
            };

            //creates stairs going down in the last room made in the center of the room
            _map.StairsDown = new Stairs
            {
                X = _map.Rooms.Last().Center.X,
                Y = _map.Rooms.Last().Center.Y,
                IsUp = false
            };
        }

        #endregion Methods
    }
}
