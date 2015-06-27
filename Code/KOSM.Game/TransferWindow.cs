using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;
using KOSM.RelatedWork.TransferWindowPlanner;

namespace KOSM.Game
{
    public class TransferWindow : WorldObject, ITransferWindow
    {
        TransferDetails raw = null;

        private TransferWindow(World world, IBody origin, IBody destination, TransferDetails raw)
            : base(world)
        {
            this.Origin = origin;
            this.Destination = destination;

            this.raw = raw;
        }

        public TransferWindow(World world, double earliestDepartureTime, IBody origin, IBody destination, bool aerobraking)
            : this(world, origin, destination, first(earliestDepartureTime, origin, destination, aerobraking && destination.HasAtmosphere))
        {
        }

        public override string ToString()
        {
            return raw.TransferDetailsText;
        }

        #region WorldObject

        public override string Identifier
        {
            get { return raw.DepartureTime + ": " + Origin.Name + "/" + Destination.Name; }
        }

        #endregion WorldObject

        #region ITransferWindow

        public IBody Origin { get; private set; }

        public IBody Destination { get; private set; }

        public double TimeTill
        {
            get { return raw.DepartureTime - world.PointInTime; }
        }

        public double TimeOf
        {
            get { return raw.DepartureTime; }
        }

        public IVectorXYZ EjectionBurnVector
        {
            get { return new VectorXYZ(0, raw.EjectionDVNormal, raw.EjectionDVPrograde); } // v3(raw.EjectionDeltaVector); }
        }

        public double EjectionAngle
        {
            get { return raw.EjectionAngle * 180.0d / Math.PI; }
        }

        #endregion ITransferWindow

        public static TransferWindow Scan(World world, double earliestDepartureTime, IBody origin, IBody destination, bool aerobraking)
        {
            return new TransferWindow(world, origin, destination, scan(earliestDepartureTime, origin, destination, aerobraking && destination.HasAtmosphere));
        }

        private static TransferDetails scan(double earliestDepartureTime, IBody origin, IBody destination, bool aerobraking)
        {
            int resolution = 50;

            double synodicPeriod = Math.Abs(1 / (1 / destination.Orbit.Period - 1 / origin.Orbit.Period));
            double latestDepartureTime = earliestDepartureTime + Math.Min(2 * synodicPeriod, 2 * origin.Orbit.Period);

            double hohmannTransferTime = LambertSolver.HohmannTimeOfFlight((origin.Orbit as Orbit).raw, (destination.Orbit as Orbit).raw);
            double earliestTransferTime = Math.Max(hohmannTransferTime - destination.Orbit.Period, hohmannTransferTime / 2);
            double latestTransferTime = earliestTransferTime + Math.Min(2 * destination.Orbit.Period, hohmannTransferTime);

            double minDeltaV = double.MaxValue;
            double minDepartureTime = double.MaxValue;
            double minTransferTime = double.MaxValue;

            for (double departureTime = earliestDepartureTime; departureTime < latestDepartureTime; departureTime += (latestDepartureTime - earliestDepartureTime) / resolution)
                for (double transferTime = earliestTransferTime; transferTime < latestTransferTime; transferTime += (latestTransferTime - earliestTransferTime) / resolution)
                {
                    double currentDeltaV = aerobraking
                        ? LambertSolver.TransferDeltaV((origin as Body).raw, (destination as Body).raw, departureTime, transferTime, origin.SafeLowOrbitAltitude, null)
                        : LambertSolver.TransferDeltaV((origin as Body).raw, (destination as Body).raw, departureTime, transferTime, origin.SafeLowOrbitAltitude, destination.SafeLowOrbitAltitude);

                    if (currentDeltaV < minDeltaV)
                    {
                        minDeltaV = currentDeltaV;
                        minDepartureTime = departureTime;
                        minTransferTime = transferTime;
                    }
                }

            TransferDetails transferSelected = null;
            double deltaV = aerobraking
                        ? LambertSolver.TransferDeltaV((origin as Body).raw, (destination as Body).raw, minDepartureTime, minTransferTime, origin.SafeLowOrbitAltitude, null, out transferSelected)
                        : LambertSolver.TransferDeltaV((origin as Body).raw, (destination as Body).raw, minDepartureTime, minTransferTime, origin.SafeLowOrbitAltitude, destination.SafeLowOrbitAltitude, out transferSelected);

            transferSelected.CalcEjectionValues();
            return transferSelected;
        }

        private static TransferDetails first(double earliestDepartureTime, IBody origin, IBody destination, bool aerobraking)
        {
            double hohmannTransferTime = LambertSolver.HohmannTimeOfFlight((origin.Orbit as Orbit).raw, (destination.Orbit as Orbit).raw);
            double shortestTransfertime = Math.Max(hohmannTransferTime - destination.Orbit.Period, hohmannTransferTime / 2);

            TransferDetails transferSelected = null;
            double deltaV = aerobraking
                        ? LambertSolver.TransferDeltaV((origin as Body).raw, (destination as Body).raw, earliestDepartureTime, shortestTransfertime, origin.SafeLowOrbitAltitude, null, out transferSelected)
                        : LambertSolver.TransferDeltaV((origin as Body).raw, (destination as Body).raw, earliestDepartureTime, shortestTransfertime, origin.SafeLowOrbitAltitude, destination.SafeLowOrbitAltitude, out transferSelected);

            transferSelected.CalcEjectionValues();
            return transferSelected;
        }
    }
}
