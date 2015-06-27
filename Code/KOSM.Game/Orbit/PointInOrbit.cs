using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Game
{
    public abstract class PointInOrbit : IPointInOrbit
    {
        static double degreeToRand = Math.PI / 180.0;

        protected World world = null;
        protected Orbit orbit = null;

        public PointInOrbit(World world, Orbit orbit)
        {
            this.world = world;
            this.orbit = orbit;
        }

        public double Radius
        {
            get { return orbit.raw.RadiusAtTrueAnomaly(degreesFromPeriapsis); }
        }

        public double Altitude
        {
            get { return Radius - orbit.Body.Radius; }
        }

        public double Velocity
        {
            get { return KOSM.Common.Velocity.OrbitVelocity(orbit, this.Radius); }
        }

        public bool MovingTowards
        {
            get { return DegreesFrom > 180; }
        }

        public double DegreesFrom
        {
            get { return checkAngle(orbit.raw.trueAnomaly - degreesFromPeriapsis); }
        }

        public double DegreesTo
        {
            get { return 360 - DegreesFrom; }
        }

        public double TimeTill
        {
            get { return orbit.raw.GetDTforTrueAnomaly(degreesFromPeriapsis * degreeToRand, 0); }
        }

        public double TimeOf
        {
            get { return world.PointInTime + TimeTill; }
        }

        public double TimeTillDegreesToEquals(double degreesTo)
        {
            return orbit.raw.GetDTforTrueAnomaly(checkAngle(degreesFromPeriapsis - degreesTo) * degreeToRand, 0);
        }

        public double TimeTillDegreesFromEquals(double degreesFrom)
        {
            return orbit.raw.GetDTforTrueAnomaly(checkAngle(degreesFrom - degreesFromPeriapsis) * degreeToRand, 0);
        }

        protected double checkAngle(double angle)
        {
            while (angle < 0) angle += 360;
            while (angle > 360) angle -= 360;
            return angle != 360 ? angle : 0;
        }


        protected abstract double degreesFromPeriapsis { get; }

        public override string ToString()
        {
            return "Radius: " + Format.Distance(Radius) + " Altitude: " + Format.Distance(Altitude) + " Velocity: " + Format.Speed(Velocity) + " MovingTowards: " + MovingTowards
                + " degreesFromPeriapsis: " + Format.Degree(degreesFromPeriapsis) + " DegreesFrom: " + Format.Degree(DegreesFrom) + " DegreesTo: " + Format.Degree(DegreesTo)
                + " TimeTill: " + Format.KerbalTime(TimeTill) + " TimeOf: " + Format.KerbalTime(TimeOf)
                + " TimeTillDegreesToEquals: " + Format.KerbalTime(TimeTillDegreesToEquals(90)) + " TimeTillDegreesFromEquals: " + Format.KerbalTime(TimeTillDegreesFromEquals(90));
        }
    }
}
