using System;

namespace KOSM.Interfaces
{
    public interface IVectorXYZ
    {
        double X { get; }
        double Y { get; }
        double Z { get; }

        double Magnitude { get; }
        bool IsNormalized { get; }

        IVectorXYZ Normalized { get; }

        IVectorXYZ ReorderedToXZY { get; }

        IVectorXYZ Plus(IVectorXYZ other);

        IVectorXYZ Minus(IVectorXYZ other);

        double Dot(IVectorXYZ other);

        IVectorXYZ Cross(IVectorXYZ other);

        double AngleTo(IVectorXYZ other);
    }
}
