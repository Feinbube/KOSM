using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KOSM.Game;

namespace KOSM.Scripts
{
    public interface IScript
    {
        void Update(World world);
    }
}
