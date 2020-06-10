using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RLNET;
using RogueSharp;
using First_Game.Core;

namespace First_Game.Systems
{
    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;

        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly DungeonMap _map;

        //Constructing a new map generator requires the dimensions of the maps it will create
        //as well as the sizes of rooms and maximum number of rooms
        public MapGenerator( int width, int height, int maxRooms, int roomMaxSize, int roomMinSize)
        {       
             _width = width;
             _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

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

            return _map;


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
        }

        //set the cell properties of the room we are creating too true 
        //so that we can walk on them
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, true);
                }
            }
        }
    }
}
