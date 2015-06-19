using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KOSM.Windows
{
    public abstract class ResizableWindow : Window
    {
        protected Vector2 scrollPosition = Vector2.zero;

        public bool AutoWidth { get; set; }
        public bool AutoHeight { get; set; }

        public new float Width { get { return rect.width; } set { rect.width = value; } }
        public new float Height { get { return rect.height; } set { rect.height = value; } }

        public ResizableWindow(int index, string title, double xRatio, double yRatio)
            : base(index, title, xRatio, yRatio)
        {
            AutoWidth = true;
            AutoHeight = true;
        }

        public ResizableWindow(int index, string title, float x, float y)
            : base(index, title, x, y)
        {
            AutoWidth = true;
            AutoHeight = true;
        }

        public ResizableWindow(int index, string title, double xRatio, double yRatio, double wRatio, double hRatio)
            : base(index, title, xRatio, yRatio, wRatio, hRatio)
        {
            AutoWidth = false;
            AutoHeight = false;
        }

        public ResizableWindow(int index, string title, float x, float y, float w, float h):base(index, title, x, y, w, h)
        {
            AutoWidth = false;
            AutoHeight = false;
        }

        protected override GUILayoutOption[] layoutOptions()
        {
            List<GUILayoutOption> result = new List<GUILayoutOption>();
            if (AutoWidth) result.Add(GUILayout.ExpandWidth(true));
            if (AutoHeight) result.Add(GUILayout.ExpandHeight(true));
            return result.ToArray();
        }
        
        protected override void drawWindow(int windowID)
        {
            if (!shouldBeVisible)
                return;

            if (AutoHeight && AutoWidth)
                buildLayout();
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                buildLayout();
                GUILayout.EndScrollView();
            }

            if (Draggable)
                GUI.DragWindow();
        }
    }
}
