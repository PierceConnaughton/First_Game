using First_Game.Core;
using First_Game.Interfaces;
using First_Game.Systems;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Game.Behaviors
{
    public class StandardMoveAndAttack : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.player;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);

            //if the monster has not been alerted, compute a field of view
            //use the monsters awarness value for the distance in fov check
            //if the player is in the monsters field of view they alert it
            //when it is alerted send a mesage
            if (!monster.TurnsAlerted.HasValue)
            {
                
                monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
                if (monsterFov.IsInFov(player.X,player.Y))
                {
                    Game.messageLog.Add($"{monster.Name} is eager to fight {player.Name}");
                    monster.TurnsAlerted = 1;
                }
            }

            if (monster.TurnsAlerted.HasValue)
            {
                //make the monster and player cells walkable
                dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(dungeonMap);

                Path path = null;

                try
                {
                    //find the shortest path between where the player is and where the monster is
                    path = pathFinder.ShortestPath(dungeonMap.GetCell(monster.X, monster.Y),
                                                   dungeonMap.GetCell(player.X, player.Y));
                }
                catch (PathNotFoundException)
                {
                    //if the monster sees the player but cant get too him send this message
                    Game.messageLog.Add($"{monster.Name} waits for a turn");
                }

                //set walkable status back too false
                dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
                dungeonMap.SetIsWalkable(player.X, player.Y, false);

                //if there is a path tell the command system too move that monster
                if (path != null)
                {
                    try
                    {
                        Cell cell = (Cell)path.StepForward();
                        commandSystem.MoveMonster(monster, cell);
                    }
                    catch (NoMoreStepsException)
                    {

                        Game.messageLog.Add($"{monster.Name} growls in frustration");
                    }
                }

                monster.TurnsAlerted++;

                //after 15 turns lose alert status as long as the player is still un fov the monster will stay alert
                //otherwise the monster will quit chasing the player
                if (monster.TurnsAlerted > 15)
                {
                    monster.TurnsAlerted = null;
                }

               
            }
            return true;
        }
    }
}
