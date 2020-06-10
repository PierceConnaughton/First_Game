using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using First_Game.Core;
using First_Game.Interfaces;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace First_Game.Systems
{
    public class CommandSystem
    {
        public bool IsPlayerTurn { get; set; }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        //activate monsters puts monsters into a scheduled list
        public void ActivateMonsters()
        {
            IScheduleable scheduleable = Game.SchedulingSystem.Get();
            //if the object being scheduled is a player
            //set player the player turn too true and add the player too the scheduling system
            if (scheduleable is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSystem.Add(Game.player);
            }
            else
            {
                //if the object being scheduled is a monster
                Monster monster = scheduleable as Monster;

                if (monster != null)
                {
                    monster.PerformAction(this);
                    Game.SchedulingSystem.Add(monster);
                   
                }
                ActivateMonsters();


            }
        }

        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!Game.DungeonMap.SetActorPosition(monster,cell.X,cell.Y))
            {
                if (Game.player.X == cell.X && Game.player.Y == cell.Y)
                {
                    Attack(monster, Game.player);
                }
            }
        }

        //this method allows the player too move or not
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

            //checks too see if there is a monster where you are trying to go
            Monster monster = Game.DungeonMap.GetMonsterAt(x, y);

            //if there is a monster at that position use the attack command
            if (monster != null)
            {
                Attack(Game.player, monster);
                return true;
            }

            return false;

        }

        //attack method for when an actor attacks againsta a defender
        public void Attack (Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            //check too see how many hits the attacker got on the defender and displays message
            int hits = ResolveAttack(attacker, defender, attackMessage);

            //checks how much the defender blocked those hits from the attacker and displays a message
            int blocks = ResolveDefense(defender, hits, attackMessage,defenseMessage);

            //adds attack message too messagelog
            Game.messageLog.Add(attackMessage.ToString());

            //if the defense message is not there display it
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                Game.messageLog.Add(defenseMessage.ToString());
            }

            //get the amount of damage gotten from how much the defender blocked the attackers hits
            int damage = hits - blocks;

            //apply the damage done by the attacker too the defender
            ResolveDamage(defender, damage);
        }

        //applys any damage that wasn't blocked to the defender
        private static void ResolveDamage(Actor defender, int damage)
        {
            //if damage was more than 0 apply the damage
           if(damage > 0)
            { 
                //get the defenders health and take away the damage from it
                defender.Health = defender.Health - damage;

                //display how much damage was done too defender
                Game.messageLog.Add($"{defender.Name} was hit for {damage} damage");

                //if defenders health is now below or = 0 resolve there death depending on what they were
                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                //if no damage was done display this message
                Game.messageLog.Add($"   {defender.Name} blocked all damage");
            }
        }

        //resolves actors deaths
        private static void ResolveDeath(Actor defender)
        {
            //if the defender was a player display you are dead
            if (defender is Player)
            {
                Game.messageLog.Add($"  YOU ARE DEAD");

            }

            //if defender was a monster remove the monster from the game and display there name and how much they dropped
            else if (defender is Monster)
            {
                Game.DungeonMap.RemoveMonster((Monster)defender);
                Game.messageLog.Add($"  {defender.Name} Died and dropped {defender.Gold} gold");
            }
        }

        // The defender rolls based on his stats to see if he blocks any of the hits from the attacker
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            
            int blocks = 0;

            //if hits were over 0
            if (hits > 0)
            {
                //display how many hits
                attackMessage.AppendFormat("scoring {0} hits", hits);
                defenseMessage.AppendFormat("   {0} defends and rolls:", defender.Name);

                //Roll a number of 100-sided dice equal to the defense value of the defending actor
                //gets the dice for that defender
                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                //Looks at the face value of each single die that was rolled
                foreach (TermResult termResult in defenseRoll.Results)
                {
                    defenseMessage.Append(termResult.Value + ", ");

                    //if the current dice result was more than or equal too 100 - the defense chance than it was a success
                    //and we add a block
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }
                //display how many blocks the defender made
                defenseMessage.AppendFormat("resulting in {0} blocks", blocks);
            }
            //if hits were less thanor equal to 0 display this message
            else
            {
                attackMessage.Append("and misses completely.");
            }

            return blocks;
        }

        //check too see how many hits the attacker got on the defender and displays message
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;

            //displays message of who is attacking which defender
            attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);

            //Roll a number 100-dice equal to the attack value of the attacking actor
            //gets the dice for that attacker
            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            //rolls the attackers dice and gets the dice result
            DiceResult attackResult = attackDice.Roll();

            //foreach dice roll in the attackers results
            foreach (TermResult termResult in attackResult.Results)
            {
                //appends the attack message adding the current attack dice value
                attackMessage.Append(termResult.Value + ", ");

                //if the current attack dice roll result was more than 100 - the attackers attack chance
                //it was a success and a hit is added
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            return hits;
        }
    }
}
