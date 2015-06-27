using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public class BodyProgradePoint : PointInOrbit
    {
        public BodyProgradePoint(World world, Orbit orbit)
            : base(world, orbit)
        {
        }

        protected override double degreesFromPeriapsis
        {
            get {
                if (orbit.raw.referenceBody.orbit == null || orbit.raw.referenceBody.orbit.vel == null)
                    return 0;

                IVectorXYZ relativePositionRocket = orbit.Orbiter.Position.Minus(orbit.Body.Position).ReorderedToXZY;
                double angle = relativePositionRocket.AngleTo(orbit.Body.Orbit.VelocityVector);

                angle = isMovingTowards ? 360 - angle : angle;
                return checkAngle(orbit.raw.trueAnomaly - angle);
            }
        }

        private bool isMovingTowards
        {
            get { return Vector3d.Dot(orbit.raw.vel.normalized, orbit.raw.referenceBody.orbit.vel.normalized) >= 0; }
        }
    }
}
