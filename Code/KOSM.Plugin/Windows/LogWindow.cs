using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Interfaces;

namespace KOSM.Windows
{
    public class LogWindow : ResizableWindow
    {
        ILog log = null;
        int rows = 10;

        public LogWindow(int index, string title, double xRatio, double yRatio, int rows, ILog log)
            : base(index, title, xRatio, yRatio)
        {
            this.rows = rows;
            this.log = log;
        }

        public LogWindow(int index, string title, float x, float y, int rows, ILog log)
            : base(index, title, x, y)
        {
            this.rows = rows;
            this.log = log;
        }

        public LogWindow(int index, string title, double xRatio, double yRatio, double wRatio, double hRatio, int rows, ILog log)
            : base(index, title, xRatio, yRatio, wRatio, hRatio)
        {
            this.rows = rows;
            this.log = log;
        }

        public LogWindow(int index, string title, float x, float y, float w, float h, int rows, ILog log)
            : base(index, title, x, y, w, h)
        {
            this.rows = rows;
            this.log = log;
        }

        protected override void buildLayout()
        {            
            GUILayout.BeginVertical();

            for (int i = Math.Max(0, log.Messages.Count - rows); i < log.Messages.Count; i++)
                GUILayout.Label(log.Messages[i].ToString(), GUI.skin.label, GUILayout.ExpandWidth(true));
            for (int i = 0; i < rows - log.Messages.Count; i++)
                GUILayout.Label("", GUI.skin.label, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();
        }
    }
}
