using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using First_Game.Core;
using First_Game.Systems;

namespace First_Game.Interfaces
{
    public interface IBehavior
    {
        //all future monsters will or wont act based on this
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
