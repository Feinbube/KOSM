using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Reporting;

namespace KOSM.Windows
{
    public class LogWindow
    {
        ILog log = null;
        int index;
        float x = 0;
        float y = 0;
        float w = 500;
        string title = "Log Window";

        protected Rect windowPos;

        public float Height { get { return windowPos.height; } }

        public LogWindow(int index, float x, float y, float w, string title, ILog log)
        {
            this.index = index;
            this.x = x;
            this.y = y;
            this.w = w;
            this.title = title;
            this.log = log;
        }

        public void Show()
        {
            RenderingManager.AddToPostDrawQueue(3, new Callback(drawGUI));
        }

        public void Hide()
        {
            RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGUI));
        }

        private void drawGUI()
        {
            GUI.skin = HighLogic.Skin;
            windowPos = GUILayout.Window(index, new Rect(x, y, 50, 50), drawWindow, title, GUILayout.MinWidth(w));
        }

        private void drawWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUI.skin.label.fontSize = 11;
            foreach (string message in log.Messages)
                GUILayout.Label(message, GUI.skin.label, GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}
