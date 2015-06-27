using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public class VectorXYZ : IVectorXYZ
    {
        private static double randToDegree = 180.0 / Math.PI;

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public double Magnitude { get; private set; }

        public bool IsNormalized { get; private set; }
        private IVectorXYZ normalized = null;

        public VectorXYZ(double x, double y, double z)
            : this(x, y, z, Math.Sqrt(x * x + y * y + z * z))
        {
        }

        public VectorXYZ(double x, double y, double z, double magnitude)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            this.Magnitude = magnitude;
            this.IsNormalized = (magnitude == 1.0);

            if (!IsNormalized)
            {
                double inverseMagnitude = 1 / Magnitude;
                this.normalized = new VectorXYZ(X * inverseMagnitude, Y * inverseMagnitude, Z * inverseMagnitude, 1.0);
            }
        }

        public IVectorXYZ Normalized { get { return IsNormalized ? this : normalized; } }

        public IVectorXYZ ReorderedToXZY { get { return new VectorXYZ(X, Z, Y, Magnitude); } }

        public IVectorXYZ Plus(IVectorXYZ other)
        {
            return new VectorXYZ(X + other.X, Y + other.Y, Z + other.Z);
        }

        public IVectorXYZ Minus(IVectorXYZ other)
        {
            return new VectorXYZ(X - other.X, Y - other.Y, Z - other.Z);
        }

        public double Dot(IVectorXYZ other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public IVectorXYZ Cross(IVectorXYZ other)
        {
            return new VectorXYZ(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
        }

        public double AngleTo(IVectorXYZ other)
        {
            return randToDegree * Math.Acos(this.Normalized.Dot(other.Normalized));
        }

        public override string ToString()
        {
            return String.Format("(X: {0:0.00}; Y: {1:0.00}; Z: {2:0.00}; )", X, Y, Z);
        }
    }
}
