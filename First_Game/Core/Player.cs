using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using First_Game.Core;
using RLNET;


namespace First_Game.Core
{
    public class Player : Actor
    {
        #region Constructor
        //player is the actor you play as
        public Player()
        {
            

            //player starts with 2 dice rolls for attacking
            Attack = 2;

            //50 percent chance the die roll is a success
            AttackChance = 50;

            //player can see 15 cells
            Awareness = 15;

            //color of player
            Color = Colors.Player;

            //player starts with 2 dice rolls for defending
            Defense = 2;

            //40% chance that the defense roll is a success
            DefenseChance = 40;

            //start with 0 gold
            Gold = 0;

            //current health of player starts at 100
            Health = 100;

            //max health of player starts at 100
            MaxHealth = 100;

            //name of player
            Name = "Rogue";

            //speed of player starts at 10
            Speed = 10;

            //players symbol
            Symbol = '@';
        }

        #endregion Constructor

        #region Constructor

        //display all the players stats on the stat console with appropriate colors and in the right positions
        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print(1, 1, $"Name:      {Name}", Colors.TextHeading);
            statConsole.Print(1, 3, $"Health:    {Health}/{MaxHealth}", Colors.Text);
            statConsole.Print(1, 5, $"Attack:    {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense:   {Defense} ({DefenseChance}%)", Colors.Text);
            statConsole.Print(1, 9, $"Gold:      {Gold}", Colors.Gold);

        }

        #endregion Constructor
    }
}
