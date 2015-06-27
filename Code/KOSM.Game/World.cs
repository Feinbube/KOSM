using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Game
{
    public class World : IWorld
    {
        private List<IBody> bodies = null;

        // time warping
        private double newWarpingTo = 0;
        private double warpingTo = 0;
        private double warpingStarted = 0;

        public World()
        {
            MissionLog = new Log();
            MissionPlanLog = new Log();
            PersistentDebugLog = new Log();
            LiveDebugLog = new Log();

            PersistentDebugLog.MessageAdded = a => ToGameLog(a);

            Camera = new Camera(this);
        }

        #region IWorld

        public ILog MissionLog { get; private set; }
        public ILog MissionPlanLog { get; private set; }
        public ILog PersistentDebugLog { get; private set; }
        public ILog LiveDebugLog { get; private set; }

        public void ClearLogs()
        {
            MissionPlanLog.Clear();
            MissionLog.Clear();
            PersistentDebugLog.Clear();
            LiveDebugLog.Clear();   
        }

        public void ToGameLog(object message)
        {
            Debug.Log("KOSM: " + message);
        }

        public ICamera Camera { get; private set; }

        public IRocket ActiveRocket
        {
            get { return new Rocket(this, FlightGlobals.ActiveVessel); }
        }

        public List<IRocket> Rockets
        {
            get { return FlightGlobals.Vessels.Where(a => a.IsControllable && a.state != Vessel.State.DEAD).Select(a => new Rocket(this, a) as IRocket).ToList(); }
        }

        public List<IBody> Bodies
        {
            get
            {
                if (bodies == null)
                    bodies = FlightGlobals.Bodies.Select(a => new Body(this, a) as IBody).ToList();
                return bodies;
            }
        }

        public IBody FindBodyByName(string bodyName)
        {
            return this.Bodies.Where(a => a.Name == bodyName).FirstOrDefault();
        }

        public void FinishUpdate()
        {
            applyTimeWarp();
        }

        public void StartGame(string profile, string filename)
        {
            HighLogic.SaveFolder = profile;
            LoadGame(filename);
        }

        public void LoadGame(string filename)
        {
            global::Game game = GamePersistence.LoadGame(filename, HighLogic.SaveFolder, true, false);

            if (game == null) throw new Exception("Game " + filename + " could not be loaded!");
            if (!game.compatible) throw new Exception("Save file " + filename + " is not compatible with your version of KSP!");

            if (game.flightState == null) return; // throw new Exception("Flightstate " + filename + " could not be loaded!");

            FlightDriver.StartAndFocusVessel(game, game.flightState.activeVesselIdx);
        }

        public void SaveGame(string filename)
        {
            global::Game game = HighLogic.CurrentGame.Updated();
            game.startScene = GameScenes.FLIGHT;
            GamePersistence.SaveGame(game, filename, HighLogic.SaveFolder, SaveMode.OVERWRITE);
        }

        public bool QuickLoad()
        {
            if (!HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad)
                return false;
            LoadGame("quicksave");
            return true;
        }

        public bool QuickSave()
        {
            if (!HighLogic.CurrentGame.Parameters.Flight.CanQuickSave)
                return false;
            QuickSaveLoad.QuickSave();
            return true;
        }

        public List<string> RocketDesigns
        {
            get
            {
                return Directory.GetFiles(vabPath).Select(a => Path.GetFileNameWithoutExtension(a)).ToList();
            }
        }

        public void Launch(string vabName)
        {
            FlightDriver.StartWithNewLaunch(vabPath + vabName + ".craft", HighLogic.CurrentGame.flagURL, "LaunchPad", new VesselCrewManifest());
        }

        public double PointInTime
        {
            get { return Planetarium.GetUniversalTime(); }
        }

        public double TimeOfNextManeuver
        {
            get
            {
                double result = double.MaxValue;
                foreach (Rocket rocket in Rockets)
                {
                    if (rocket.NextManeuver == null)
                        continue;

                    if (rocket.NextManeuver.TimeOfBurn < result)
                        result = rocket.NextManeuver.TimeOfBurn;

                    if (rocket.NextManeuver.TimeOfTurn > PointInTime && rocket.NextManeuver.TimeOfTurn < result)
                        result = rocket.NextManeuver.TimeOfTurn;
                }
                return result;
            }
        }

        public bool IsTimeWarping
        {
            get { return isTimeWarping || PointInTime <= newWarpingTo; }
        }

        public bool WarpTime(double timespan)
        {
            return WarpTimeTo(PointInTime + timespan);
        }

        public bool WarpTimeTo(double timeToWarpTo)
        {
            if (timeToWarpTo <= PointInTime + 1)
                return false;

            if (IsTimeWarping && (timeToWarpTo >= warpingTo || timeToWarpTo >= newWarpingTo))
                return true;

            newWarpingTo = timeToWarpTo;
            return true;
        }

        public void PreventTimeWarping()
        {
            newWarpingTo = 0;
        }

        #endregion IWorld

        private bool isTimeWarping { get { return PointInTime <= warpingTo || TimeWarp.fetch.current_rate_index > 0; } }
        private bool gameCompliedToTimeWarpingRequest { get { return !(PointInTime - warpingStarted > 3 && warpingTo > PointInTime + 10 && TimeWarp.fetch.current_rate_index == 0); } }

        private void applyTimeWarp()
        {
            if ((isTimeWarping && newWarpingTo == 0) || TimeWarp.WarpMode == TimeWarp.Modes.LOW)
            {
                applyTimeWarp(0);
                return;
            }

            if (warpingNeedsStarting || warpTimeNeedsUpdate)
                applyTimeWarp(Math.Min(newWarpingTo, TimeOfNextManeuver));
        }

        private bool warpingNeedsStarting { get { return !isTimeWarping && newWarpingTo > PointInTime; } }

        private bool warpTimeNeedsUpdate { get { return isTimeWarping && (TimeOfNextManeuver < warpingTo || newWarpingTo < warpingTo || !gameCompliedToTimeWarpingRequest); } }

        private void applyTimeWarp(double timeToWarpTo)
        {
            warpingStarted = PointInTime;

            newWarpingTo = timeToWarpTo;
            warpingTo = timeToWarpTo;

            if (timeToWarpTo > 0)
                TimeWarp.fetch.WarpTo(timeToWarpTo);
            else if (TimeWarp.fetch.current_rate_index > 0)
                TimeWarp.fetch.WarpTo(PointInTime);
        }

        private string vabPath
        {
            get
            {
                return KSPUtil.ApplicationRootPath + "saves/" +  HighLogic.SaveFolder + "/Ships/VAB/";
            }
        }
    }
}
