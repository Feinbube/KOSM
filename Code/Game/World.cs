using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Reporting;

namespace KOSM.Game
{
    public class World
    {
        public ILog MissionLog = new Log();
        public ILog MissionPlanLog = new Log();
        public ILog DebugLog = new Log();
        public ILog LiveDebugLog = new Log();

        public IDraw Canvas = new GraphicsOverlay();

        private List<Body> bodies = null;

        public List<Body> Bodies
        {
            get
            {
                if (bodies == null)
                    bodies = FlightGlobals.Bodies.Select(a => new Body(this, a)).ToList();
                return bodies;
            }
        }

        public List<Rocket> Rockets { get { return FlightGlobals.Vessels.Where(a => a.IsControllable).Select(a => new Rocket(this, a)).ToList(); } }

        public Body FindBodyByName(string bodyName)
        {
            return this.Bodies.Where(a => a.Name == bodyName).FirstOrDefault();
        }

        public void FinishUpdate()
        {
            applyTimeWarp();
        }

        #region Time and Warping

        public double PointInTime { get { return Planetarium.GetUniversalTime(); } }

        private double newWarpingTo = 0;
        private double warpingTo = 0;
        private double warpingStarted = 0;

        public bool IsTimeWarping { get { return isTimeWarping || PointInTime <= newWarpingTo; } }
        private bool isTimeWarping { get { return PointInTime <= warpingTo || TimeWarp.fetch.current_rate_index > 0; } }
        private bool GameCompliedToTimeWarpingRequest { get { return !(PointInTime - warpingStarted > 3 && warpingTo > PointInTime + 10 && TimeWarp.fetch.current_rate_index == 0); } }

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

        public void PreventTimeWarping()
        {
            newWarpingTo = 0;
        }

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

        private bool warpTimeNeedsUpdate { get { return isTimeWarping && (TimeOfNextManeuver < warpingTo || newWarpingTo < warpingTo || !GameCompliedToTimeWarpingRequest); } }

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

        #endregion Time and Warping

        #region Save and Load

        public void SaveGame(string filename)
        {
            global::Game game = HighLogic.CurrentGame.Updated();
            game.startScene = GameScenes.FLIGHT;
            GamePersistence.SaveGame(game, filename, HighLogic.SaveFolder, SaveMode.OVERWRITE);
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

        public bool QuickSave()
        {
            if (!HighLogic.CurrentGame.Parameters.Flight.CanQuickSave) return false;
            QuickSaveLoad.QuickSave();
            return true;
        }

        public bool QuickLoad()
        {
            if (!HighLogic.CurrentGame.Parameters.Flight.CanQuickLoad) return false;
            LoadGame("quicksave");
            return true;
        }

        #endregion Save and Load
    }
}
