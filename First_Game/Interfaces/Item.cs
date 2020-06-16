using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Game.Interfaces
{
    public interface Item
    {
        string Name { get; }
        int RemainingUses { get; }

        bool Use();
    }
}
