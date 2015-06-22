using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KOSM.Windows
{
    public abstract class ResizableWindow : Window
    {
        public Mouse mouse = new Mouse();
        protected Vector2 scrollPosition = Vector2.zero;

        protected bool currentlyResizingRight = false;
        protected bool currentlyResizingBottom = false;

        protected bool currentlyResizing { get { return currentlyResizingRight || currentlyResizingBottom; } }

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

        public ResizableWindow(int index, string title, float x, float y, float w, float h)
            : base(index, title, x, y, w, h)
        {
            AutoWidth = false;
            AutoHeight = false;
        }

        public ResizableWindow(ConfigNode node) : base(node)
        {
            AutoWidth = bool.Parse(node.GetValue("AutoWidth"));
            AutoHeight = bool.Parse(node.GetValue("AutoHeight"));
        }

        protected override GUILayoutOption[] layoutOptions()
        {
            List<GUILayoutOption> result = new List<GUILayoutOption>();
            if (AutoWidth) result.Add(GUILayout.ExpandWidth(true));
            if (AutoHeight) result.Add(GUILayout.ExpandHeight(true));
            return result.ToArray();
        }

        public override ConfigNode AsConfigNode()
        {
            ConfigNode result = base.AsConfigNode();
            result.AddValue("AutoWidth", AutoWidth);
            result.AddValue("AutoHeight", AutoHeight);
            return result;
        }

        protected override void drawWindow(int windowID)
        {
            checkResizing();

            if (AutoHeight && AutoWidth)
                buildLayout();
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                buildLayout();
                GUILayout.EndScrollView();
            }

            if (Draggable && !currentlyResizing)
                GUI.DragWindow();
        }

        private void checkResizing()
        {
            mouse.Update();

            if (currentlyResizingRight)
            {
                rect.width = mouse.X - rect.x;

                if (!mouse.LeftDown)
                    currentlyResizingRight = false;
            }

            if (currentlyResizingBottom)
            {
                rect.height = mouse.Y - rect.y;

                if (!mouse.LeftDown)
                    currentlyResizingBottom = false;
            }

            if (!currentlyResizing)
            {
                if (!mouse.LeftDown || !mouse.Hovering(rect))
                    return;

                if (mouseOnRightBorder) currentlyResizingRight = true;
                if (mouseOnBottomBorder) currentlyResizingBottom = true;

                if (currentlyResizingRight)
                    AutoWidth = false;
                if (currentlyResizingBottom)
                    AutoHeight = false;
            }
        }

        bool mouseOnRightBorder { get { return mouse.X > rect.x + rect.width - 8; } }
        bool mouseOnBottomBorder { get { return mouse.Y > rect.y + rect.height - 8; } }
    }
}
