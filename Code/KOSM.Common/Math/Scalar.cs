using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Utility
{
    public static class Scalar
    {
        public static double Interpolate(double from, double to, double progress)
        {
            return (to - from) * progress + from;
        }
    }
}
