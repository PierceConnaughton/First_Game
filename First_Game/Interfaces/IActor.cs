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
        string Name { get; set; }

        //Awarness will calculate field of view
        int Awareness { get; set; }
    }
}
