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
        public ILog DebugLog = new Log();

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

        public List<Rocket> Rockets { get { return FlightGlobals.Vessels.Where(a => a.HasControlSources()).Select(a => new Rocket(this, a)).ToList(); } }

        public Body FindBodyByName(string bodyName)
        {
            return this.Bodies.Where(a => a.Name == bodyName).FirstOrDefault();
        }

        public double PointInTime { get { return Planetarium.GetUniversalTime(); } }

        public double WarpingTill = 0;

        public bool IsTimeWarping { get { return PointInTime <= WarpingTill; } }
        public bool WarpTime(double timespan)
        {
            if (timespan <= 0)
                return false;

            if (!IsTimeWarping)
            {
                WarpingTill = PointInTime + timespan;
                TimeWarp.fetch.WarpTo(WarpingTill);
            }

            return true;
        }

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
    }
}
