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
    public class TimeWarp : ITimeWarp
    {
        private World world;

        private double warpingTo = 0;

        public TimeWarp(World world)
        {
            this.world = world;
        }

        #region ITimeWarp

        public bool IsTimeWarping
        {
            get { return warpingTo > world.PointInTime; }
        }        

        public bool WarpTime(double timespan)
        {
            return WarpTimeTo(world.PointInTime + timespan);
        }

        public bool WarpTimeTo(double timeToWarpTo)
        {
            if (timeToWarpTo <= world.PointInTime + 1)
                return false;

            if (IsTimeWarping && timeToWarpTo >= warpingTo)
                return true;

            warpingTo = timeToWarpTo;
            return true;
        }

        public void PreventTimeWarping()
        {
            warpingTo = 0;
        }

        #endregion ITimeWarp

        private int gameWarpRate { get { return global::TimeWarp.fetch.current_rate_index; } }

        public void ApplyTimeWarp()
        {
            if (global::TimeWarp.WarpMode == global::TimeWarp.Modes.LOW)
                warpingTo = 0;

            warpingTo = Math.Min(warpingTo, world.TimeOfNextManeuver);

            if (warpingTo > world.PointInTime)
                global::TimeWarp.SetRate(warpRate(warpingTo), false);
            else if (gameWarpRate > 0)
                global::TimeWarp.SetRate(gameWarpRate - 1, false);
        }

        private int warpRate(double timeToWarpTo)
        {
            double delta = timeToWarpTo - world.PointInTime;

            float[] rates = global::TimeWarp.fetch.warpRates;
            for (int index = 0; index < rates.Length; index++)
                if (rates[index] > delta * 5)
                    return Math.Max(0, index - 1);

            return rates.Length - 1;
        }
    }
}
