using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KOSM.Windows
{
    public abstract class Window
    {
        protected int index;
        protected string title = "Window Title";

        protected Rect rect;

        private Callback callback;

        public double X { get { return rect.x; } set { rect.x = (float)value; } }
        public double Y { get { return rect.y; } set { rect.y = (float)value; } }
        public double Width { get { return rect.width; } }
        public double Height { get { return rect.height; } }

        protected bool shouldBeVisible = false;
        protected bool isReallyVisible { get { return RenderingManager.fetch.postDrawQueue.Contains(callback); } }

        public bool Draggable { get; set; }

        public Window(int index, string title, double xRatio, double yRatio)
            : this(index, title, xRatio * Screen.width, yRatio * Screen.height, 0.0, 0.0)
        {
        }

        public Window(int index, string title, float x, float y)
            : this(index, title, x, y, 0, 0)
        {
        }

        public Window(int index, string title, double xRatio, double yRatio, double wRatio, double hRatio)
            : this(index, title, (float)(xRatio * Screen.width), (float)(yRatio * Screen.height), (float)(wRatio * Screen.width), (float)(hRatio * Screen.height))
        {
        }

        public Window(int index, string title, float x, float y, float w, float h)
        {
            this.index = index;
            this.title = title;

            this.rect = new Rect(x, y, w, h);
            this.callback = drawGUI;

            Draggable = true;
        }

        public void Show()
        {
            shouldBeVisible = true;

            if (!isReallyVisible)
                RenderingManager.AddToPostDrawQueue(3, callback);
        }

        public void Hide()
        {
            shouldBeVisible = false;

            if (isReallyVisible)
                RenderingManager.RemoveFromPostDrawQueue(3, callback);
        }

        public void Check()
        {
            if (shouldBeVisible) Show();
            else Hide();
        }

        public void ToggleShowHide()
        {
            if (!shouldBeVisible) Show();
            else Hide();
        }

        private void drawGUI()
        {
            buildSkin();

            rect = GUILayout.Window(index, rect, drawWindow, title, layoutOptions());
        }        

        protected virtual GUILayoutOption[] layoutOptions()
        {
            return new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
        }

        protected virtual void drawWindow(int windowID)
        {
            if (!shouldBeVisible)
                return;

            buildLayout();

            if (Draggable)
                GUI.DragWindow();
        }

        private void buildSkin()
        {
            GUI.skin = HighLogic.Skin;
            GUI.skin.label.fontSize = 11;
            GUI.skin.label.margin = new RectOffset(1, 1, 1, 1);
            GUI.skin.label.padding = new RectOffset(0, 0, 2, 2);
        }

        protected abstract void buildLayout();

        public override string ToString()
        {
            return title;
        }
    }
}
