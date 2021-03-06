﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface IRocketManeuvers
    {
        List<IManeuver> Maneuvers { get; }
        bool HasManeuver { get; }
        IManeuver NextManeuver { get; }

        void AddManeuver(double pointInTime, IVectorXYZ burnVector);
        void AddApoapsisManeuver(double targetRadius);
        void AddPeriapsisManeuver(double targetRadius);
        void AddHohmannManeuver(IBody targetBody);
        void AddInclinationChangeManeuver(IBody targetBody);

        ITransferWindow NextTransferWindow(double earliestDepartureTime, IBody origin, double altitude, IBody destination, bool aerobraking);

        ITransferWindow BestTransferWindow(double earliestDepartureTime, IBody origin, double altitude, IBody destination, bool aerobraking);

        bool HasEncounter { get; }
        IOrbit NextEncounter { get; }
    }
}
