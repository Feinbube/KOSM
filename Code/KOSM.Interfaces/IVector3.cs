using System;

namespace KOSM.Interfaces
{
    public interface IVector3
    {
        double X { get; }
        double Y { get; }
        double Z { get; }

        double Dot(IVector3 other);

        IVector3 Cross(IVector3 other);

        double Magnitude { get; }
    }
}
