using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Reporting;

namespace KOSM.Reporting
{
    public class GraphicsOverlay : IDraw
    {
        private GameObject canvas = new GameObject("GraphicsOverlay");

        public void DrawLine(Transform parentTransform, Vector3 v1, Vector3 v2, Color color)
        {
        }

        public void DrawArrow(Transform parentTransform, Vector3 v1, Vector3 v2, Color color)
        {
        }

        public void DrawTriangle(Transform parentTransform, Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
        }

        public void FillTriangle(Transform parentTransform, Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
        }

        public void Clear()
        {
            canvas.DestroyGameObject();
            canvas = new GameObject("GraphicsOverlay");
        }
    }
}
