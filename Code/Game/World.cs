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
    }
}
