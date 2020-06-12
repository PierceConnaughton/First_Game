using First_Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using RogueSharp;

namespace First_Game.Core
{
    //Inherits from Idrawable because we want to draw the doors on the map console
    public class Door : IDrawable
    {
        #region Prop
        public bool IsOpen { get; set; }

        public RLColor Color { get; set; }
        public RLColor BackgroundColor { get; set; }
        public char Symbol { get; set; }

        //coordinates we want the door to be
        public int X { get; set; }
        public int Y { get; set; }

        #endregion Prop

        #region Constructors

        //creates door
        public Door()
        {
            //reprsents the colors and symbol that we want too represent the door
            Symbol = '+';
            Color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        #endregion Constructors

        #region Methods

        public void Draw(RLConsole console, IMap map)
        {
            //! reprsents that what is inside is null

            //if the door has not been seen 
            if (!map.GetCell(X,Y).IsExplored)
            {
                return;
            }

            //when door is opened change the symbol
            Symbol = IsOpen ? '-' : '+';

            //if door is in view
            if (map.IsInFov(X,Y))
            {
                Color = Colors.DoorFov;
                BackgroundColor = Colors.DoorBackgroundFov;
            }

            //if the door has just been found
            else
            {
                Color = Colors.Door;
                BackgroundColor = Colors.DoorBackground;
            }

            console.Set(X, Y, Color, BackgroundColor, Symbol);
        }

        #endregion Methods


    }
}
