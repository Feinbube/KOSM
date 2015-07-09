using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using KOSM.Common;
using KOSM.Interfaces;
using KOSM.Utility;

namespace KOSM.Game
{
    public class Rocket : WorldObject, IRocket
    {
        internal RocketControlSAS control = null;
        internal Vessel raw = null;

        public Rocket(World world, Vessel vessel)
            : base(world)
        {
            this.raw = vessel;
            this.control = new RocketControlSAS(world, this);
        }

        #region WorldObject

        public override string Identifier
        {
            get { return raw.id.ToString(); }
        }

        #endregion WorldObject

        #region IRocket

        public string Name
        {
            get { return raw.GetName(); }
        }

        public IBody Body
        {
            get { return world.FindBodyByName(raw.mainBody.GetName()); }
        }

        public IOrbit Orbit
        {
            get { return new Orbit(world, raw.orbit, this, this.Body); }
        }

        public double RocketVerticalHeight
        {
            get
            {
                if (raw.rigidbody == null)
                    return 0;

                double altitudeOfPart = double.MaxValue;
                foreach (Part part in raw.Parts)
                {
                    if (part.collider == null)
                        continue;

                    Vector3 extents = part.collider.bounds.extents;
                    float partRadius = Math.Max(extents[0], Math.Max(extents[1], extents[2]));
                    double partAltitudeBottom = Math.Max(0, raw.mainBody.GetAltitude(part.collider.bounds.center) - partRadius);
                    if (partAltitudeBottom < altitudeOfPart)
                        altitudeOfPart = partAltitudeBottom;
                }

                return Altitude - altitudeOfPart;
            }
        }

        public double Mass
        {
            get { return this.raw.GetTotalMass(); }
        }

        public List<IManeuver> Maneuvers
        {
            get { return raw.patchedConicSolver.maneuverNodes.Select(a => new Maneuver(world, this, raw.patchedConicSolver, a) as IManeuver).ToList(); }
        }

        public IManeuver NextManeuver
        {
            get
            {
                if (Maneuvers.Count == 0)
                    return null;

                var timeOfNextManeuver = Maneuvers.Where(a => a.TimeWhenDue > world.PointInTime).Min(a => a.TimeWhenDue);
                return Maneuvers.Where(a => a.TimeWhenDue == timeOfNextManeuver).First();
            }
        }

        public bool HasManeuver
        {
            get { return raw.patchedConicSolver != null && raw.patchedConicSolver.maneuverNodes != null && raw.patchedConicSolver.maneuverNodes.Count > 0; }
        }

        public void AddApoapsisManeuver(double targetRadius)
        {
            AddManeuver(this.Orbit.Apoapsis.TimeOf, DeltaV.ApoapsisManeuver(this.Orbit, targetRadius));
        }

        public void AddPeriapsisManeuver(double targetRadius)
        {
            AddManeuver(this.Orbit.Periapsis.TimeOf, DeltaV.PeriapsisManeuver(this.Orbit, targetRadius));
        }

        public void AddHohmannManeuver(IBody targetBody)
        {
            throw new NotImplementedException();

            // this.MainBody.raw.Orbit;
            // this.MainBody.raw.OrbitDriver.OnRailsSOITransition();
            // OrbitUtil.GetTransferTime(o.referenceBody.orbit, destination);
        }

        public void AddInclinationChangeManeuver(IBody targetBody)
        {
            throw new NotImplementedException();
        }

        public void AddManeuver(double pointInTime, IVectorXYZ burnVector)
        {
            ManeuverNode node = raw.patchedConicSolver.AddManeuverNode(pointInTime);
            node.DeltaV = v3d(burnVector); // X:radialOut, Y:normal, Z:prograde
            raw.patchedConicSolver.UpdateFlightPlan();
        }

        public double Throttle
        {
            get { return control.Throttle; }
            set { control.Throttle = value; }
        }

        public void LockHeading(IEulerAngles3 newHeading)
        {
            control.LockHeading(newHeading);
        }

        /// <summary>
        /// Wish for a target heading, pitch and roll. We see what we can do.
        /// </summary>
        /// <param name="pitchAboveHorizon">-90° (down) to 90° up</param>
        /// <param name="degreesFromNorth">0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)</param>
        /// <param name="roll">--180° to 180°</param>
        public void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll)
        {
            control.SetCompassSteering(pitchAboveHorizon, degreesFromNorth, roll);
        }

        public void SetSteering(IVectorXYZ forward)
        {
            control.SetSteering(forward);
        }

        public void Stage()
        {
            control.Stage();
        }

        public void RaiseGear()
        {
            raw.ActionGroups.SetGroup(KSPActionGroup.Gear, false);
        }

        public void LowerGear()
        {
            raw.ActionGroups.SetGroup(KSPActionGroup.Gear, true);
        }

        public double MissionTime
        {
            get { return raw.missionTime; }
        }

        public bool Landed
        {
            get { return raw.situation == Vessel.Situations.LANDED || raw.situation == Vessel.Situations.PRELAUNCH || raw.situation == Vessel.Situations.SPLASHED; }
        }

        public bool InCircularOrbit
        {
            get { return raw.situation == Vessel.Situations.ORBITING && Orbit.IsCircular; }
        }

        public double Altitude
        {
            get { return raw.altitude; }
        }

        public double AltitudeOverGround
        {
            get { return Math.Max(raw.altitude, raw.GetHeightFromTerrain()) - RocketVerticalHeight; }
        }

        public bool Turned
        {
            get { return control.Turned; } // less than 0.03° deviation }
        }

        public double TurnDeviation
        {
            get { return control.TurnDeviation; }
        }

        /// <summary>
        /// 0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)
        /// </summary>
        public double Heading
        {
            get { return control.Heading; }
        }

        /// <summary>
        /// -90° (down) to 90° (up)
        /// </summary>
        public double Pitch
        {
            get { return control.Pitch; }
        }

        /// <summary>
        /// -180° to 180°
        /// </summary>
        public double Roll
        {
            get { return control.Roll; }
        }

        public IVectorXYZ Up
        {
            get { return control.Up; }
        }

        public IVectorXYZ OrbitRetrograde
        {
            get { return control.OrbitRetrograde; }
        }

        public IVectorXYZ SurfaceRetrograde
        {
            get { return control.SurfaceRetrograde; }
        }

        public double MaxAcceleration
        {
            get { return control.MaxAcceleration; }
        }

        public double CurrentThrust
        {
            get { return control.CurrentThrust; }
        }

        public double MaxThrust
        {
            get { return control.MaxThrust; }
        }

        public double VerticalSpeed
        {
            get { return raw.verticalSpeed; }
        }

        public double HorizontalSurfaceSpeed
        {
            get { return raw.horizontalSrfSpeed; }
        }

        public IVectorXYZ SurfaceVelocity
        {
            get { return vXYZ(raw.srf_velocity); }
        }

        public double TimeToMissionTime(double pointInTime)
        {
            return MissionTime + pointInTime - world.PointInTime;
        }

        public IVectorXYZ Position
        {
            get { return vXYZ(raw.transform.position); }
        }

        public ITransferWindow NextTransferWindow(double earliestDepartureTime, IBody origin, double altitude, IBody destination, bool aerobraking)
        {
            return new TransferWindow(world, earliestDepartureTime, origin, altitude, destination, aerobraking);
        }

        public ITransferWindow BestTransferWindow(double earliestDepartureTime, IBody origin, double altitude, IBody destination, bool aerobraking)
        {
            return TransferWindow.Scan(world, earliestDepartureTime, origin, altitude, destination, aerobraking);
        }

        public bool HasEncounter
        {
            get { return NextEncounter != null; }
        }

        public IBody NextEncounter
        {
            get
            {
                global::Orbit encounter = findNextOrbitPatch(global::Orbit.PatchTransitionType.ENCOUNTER);
                return encounter == null ? null : world.FindBodyByName(encounter.referenceBody.name);
            }
        }

        #endregion IRocket

        public override string ToString()
        {
            return Name;
        }

        private global::Orbit findNextOrbitPatch(global::Orbit.PatchTransitionType type)
        {
            var orbit = this.raw.orbit;
            while (orbit != null)
            {
                if (orbit.patchStartTransition == type)
                    return orbit;
                orbit = orbit.nextPatch;
            }

            return null;
        }
    }
}
