using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Game.Interfaces
{
    //make sure too use interface instead of class
    public interface IActor
    {
        #region explanationOfStats
        /* 1.  Attack        -   Number of dice to roll when performing an attack also represents 
         *                       max number of damage that can be inflicted in a single attack.
         * 
         * 2.  AttackChance  -   Percentage chance that each die rolled is a success. A success for 
         *                       1 point of damage was inflicted .
         *                       
         * 3.  Awarness      -   This determines how far their field of view extends. For other actors or monsters
         *                       this is the distance they can see and hear. If the player gets within that distance
         *                       then it is likely the moster will be aware of the players presence.
         *                       
         * 4.  Defense       -   Number of dice to roll when defending against an attack. Also represents the 
         *                       max amount of damage that can be blocked or dodged from an incoming attack.
         *                       
         * 5.  DefenseChance -   Percentage chance that each die rolled is a success. 
         *                       A success for a die that means 1 point of damage was blocked.
         *                       
         * 6.  Gold          -   How much money the actor has. Most monsters drop gold upon death.  
         *                       
         * 7.  Health        -   The current health total of the actor. If this value is 0 or less the actor is killed.
         * 
         * 8.  Max Health    -   How much health the actor has when fully healed.
         * 
         * 9.  Name          -   Name of the actor.
         * 
         * 10. Speed         -   How fast the actor is. This determines the rate at which they perform actions.
         *                       A lower number is faster. An actor with a speed of 10 will perform actions twice
         *                       As quick as an actor with a speed of 20.
         */
        #endregion  explanationOfStats

        int Attack { get; set; }
        int AttackChance { get; set; }
        int Awareness { get; set; }
        int Defense { get; set; }
        int DefenseChance { get; set; }
        int Gold { get; set; }
        int Health { get; set; }
        int MaxHealth { get; set; }
        string Name { get; set; }
        int Speed { get; set; }


    }
}
