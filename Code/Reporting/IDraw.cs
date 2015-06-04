using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KOSM.Reporting
{
    public interface IDraw
    {
        void DrawLine(Transform parentTransform, Vector3 v1, Vector3 v2, Color color);

        void DrawArrow(Transform parentTransform, Vector3 v1, Vector3 v2, Color color);

        void DrawTriangle(Transform parentTransform, Vector3 v1, Vector3 v2, Vector3 v3, Color color);

        void FillTriangle(Transform parentTransform, Vector3 v1, Vector3 v2, Vector3 v3, Color color);

        void Clear();
    }
}
