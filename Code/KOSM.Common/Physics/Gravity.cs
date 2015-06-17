using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public static class Gravity
    {
        public static double GravityAtSealevel(IBody body)
        {
            return GravityAtSealevel(body.GravityParameter, body.Radius);
        } 

        public static double GravityAtSealevel(double gravityParameter, double radius) 
        {
            return gravityParameter / Math.Pow(radius, 2); 
        } 
    }
}
