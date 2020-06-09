using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using First_Game.Core;


namespace First_Game.Core
{
    public class Player : Actor
    {
        //player is the actor you play as
        public Player()
        {
            //player can see 15 cells
            Awareness = 15;

            //name of player
            Name = "Rogue";

            //color of player
            Color = Colors.Player;

            //players symbol
            Symbol = '@';
            
            //start position on the map
            X = 10;
            Y = 10;
        }
    }
}
