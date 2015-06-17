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
        protected float x = 0;
        protected float y = 0;
        protected float w = 500;
        protected string title = "Window Title";

        protected bool hidden = true;

        protected Rect windowPos;

        public float Height { get { return windowPos.height; } }

        public Window(int index, float x, float y, float w, string title)
        {
            this.index = index;
            this.x = x;
            this.y = y;
            this.w = w;
            this.title = title;
        }

        public void Show()
        {
            if (hidden)
                RenderingManager.AddToPostDrawQueue(3, new Callback(drawGUI));
            hidden = false;
        }

        public void Hide()
        {
            if (!hidden)
                RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGUI));
            hidden = true;
        }

        public void ToggleShowHide()
        {
            if (hidden) Show();
            else Hide();
        }

        private void drawGUI()
        {
            GUI.skin = HighLogic.Skin;
            windowPos = GUILayout.Window(index, new Rect(x, y, 50, 50), drawWindow, title, GUILayout.MinWidth(w));
        }

        private void drawWindow(int windowID)
        {
            if (hidden)
                return;

            GUI.skin.label.fontSize = 11;
            GUI.skin.label.margin = new RectOffset(1, 1, 1, 1);
            GUI.skin.label.padding = new RectOffset(0, 0, 2, 2);

            buildLayout();

            GUI.DragWindow();
        }

        protected abstract void buildLayout();
    }
}
