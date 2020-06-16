using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using First_Game.Core;
using RogueSharp;
using RLNET;
using RogueSharp.DiceNotation;

namespace First_Game.Monsters
{
    //the drowner monster
    //too get alot of stats I used the dnd dice notation from rogue sharp

    public class Drowner : Monster
    {
        public static Drowner Create(int level)
        {
            #region Prop
            int health = Dice.Roll("2D5");

            #endregion Prop

            #region Constructor
            return new Drowner
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.KoboldColor,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Drowner",
                Speed = 50,
                Symbol = 'D'
            };

            #endregion Constructor
        }

    }
}
