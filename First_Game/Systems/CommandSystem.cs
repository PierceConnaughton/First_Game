using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using First_Game.Core;

namespace First_Game.Systems
{
    public class CommandSystem
    {
        //this method allows rge okater too move or not
        //it returns true if the player was able to move
        //and returns false when the player cant move, eg:moving into a wall
        public bool MovePlayer(Direction direction)
        {
            //gets the players current postion
            int x = Game.player.X;
            int y = Game.player.Y;

            //depending on the direction the player chooses it will get the x or y value and add 1 or take away 1
            //from it and from that they will use the set actor position method too get the set the position of the actor
            switch(direction)
            {

                case Direction.Up:
                    {
                        y = Game.player.Y - 1;
                        break;
                    }

                case Direction.Down:
                    {
                        y = Game.player.Y + 1;
                        break;
                    }

                case Direction.Left:
                    {
                        x = Game.player.X - 1;
                        break;
                    }

                case Direction.Right:
                    {
                        x = Game.player.X + 1;
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            if (Game.DungeonMap.SetActorPosition(Game.player, x, y))
            {
                return true;
            }

            return false;

        }
    }
}
