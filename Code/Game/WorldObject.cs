﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Game
{
    public class WorldObject
    {
        protected World world = null;

        protected WorldObject(World world)
        {
            this.world = world;
        }

        protected double clamp(double value, double min, double max)
        {
            return Math.Min(max, Math.Max(min, value));
        }
    }
}