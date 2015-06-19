using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KOSM
{
    public class Mouse
    {
        public float X { get { return Input.mousePosition.x; } }
        public float Y { get { return Screen.height - Input.mousePosition.y; } }

        public bool LeftDown { get; private set; }
        public bool RightDown { get; private set; }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
                LeftDown = true;
            if (Input.GetMouseButtonUp(0))
                LeftDown = false;

            if (Input.GetMouseButtonDown(1))
                RightDown = true;
            if (Input.GetMouseButtonUp(1))
                RightDown = false;
        }

        public bool Hovering(Rect rect)
        {
            return rect.Contains(new Vector2(X, Y));
        }

        public override string ToString()
        {
            return String.Format("Mouse: ({0:0}, {1:0}}", X, Y) + "Left is " + (LeftDown ? "down" : "up") + ". Right is " + (RightDown ? "down" : "up") + ".";
        }
    }
}
