using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public class Vector3 : IVector3
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double Dot(IVector3 other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public IVector3 Cross(IVector3 other)
        {
            return new Vector3(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
        }

        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }
    }
}
