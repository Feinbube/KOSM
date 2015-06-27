using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface ITransferWindow
    {
        IBody Origin { get; }

        IBody Destination { get; }

        double TimeTill { get; }
        double TimeOf { get; }

        IVectorXYZ EjectionBurnVector { get; }

        double EjectionAngle { get; }
    }
}
