using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public class EulerAngles3 : IEulerAngles3
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public EulerAngles3(float x, float y, float z)
        {
            this.X = x % 360;
            this.Y = y % 360;
            this.Z = z % 360;
        }
    }
}
