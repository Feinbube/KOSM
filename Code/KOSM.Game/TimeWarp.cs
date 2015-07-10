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

        private double warpFactor;
        private double persistentTimeToWarpTo = 0;

        public TimeWarp(World world)
        {
            this.world = world;
        }

        #region ITimeWarp

        public bool IsTimeWarping
        {
            get { return warpFactor > 1; }
        }

        public void PreventTimeWarping()
        {
            warpFactor = 1;
        }

        public bool OneTickWarpTimeBy(double factor)
        {
            if (factor < 0)
                return false;

            if (factor < warpFactor)
                warpFactor = factor;

            return true;
        }

        public bool OneTickWarpTime(double timespan)
        {
            if (timespan < 0)
                return false;

            return OneTickWarpTimeBy(warpRate(timespan));
        }

        public bool OneTickWarpTimeTo(double timeToWarpTo)
        {
            if (timeToWarpTo < world.PointInTime)
                return false;

            return OneTickWarpTimeBy(warpRate(timeToWarpTo - world.PointInTime));
        }

        public bool PersistentWarpTimeTo(double timeToWarpTo)
        {
            if (timeToWarpTo <= world.PointInTime)
                return false;

            if (timeToWarpTo > this.persistentTimeToWarpTo)
                this.persistentTimeToWarpTo = timeToWarpTo;

            return true;
        }

        #endregion ITimeWarp

        private int gameWarpRate { get { return global::TimeWarp.fetch.current_rate_index; } }

        public void ApplyTimeWarp()
        {
            OneTickWarpTimeTo(this.persistentTimeToWarpTo);

            if (global::TimeWarp.WarpMode == global::TimeWarp.Modes.LOW || warpFactor == double.MaxValue)
                warpFactor = 1;

            warpFactor = Math.Min(warpFactor, warpRate(world.TimeOfNextManeuver - world.PointInTime));

            if (warpFactor > 1)
                global::TimeWarp.SetRate(warpIndex(warpFactor), false);
            else if (gameWarpRate > 0)
                global::TimeWarp.SetRate(gameWarpRate - 1, false);

            warpFactor = double.MaxValue;
        }

        private double warpRate(double timeDelta)
        {
            float[] rates = global::TimeWarp.fetch.warpRates;
            for (int index = 0; index < rates.Length; index++)
                if (rates[index] > timeDelta * 5)
                    return rates[index];

            return rates[rates.Length - 1];
        }

        private int warpIndex(double rate)
        {
            float[] rates = global::TimeWarp.fetch.warpRates;
            for (int index = 0; index < rates.Length; index++)
                if (rates[index] > rate)
                    return Math.Max(0, index - 1);

            return rates.Length - 1;
        }
    }
}
