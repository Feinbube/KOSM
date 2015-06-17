using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Interfaces;

namespace KOSM.Windows
{
    public class LogWindow : Window
    {
        ILog log = null;
        int rows = 10;

        public LogWindow(int index, float x, float y, float w, string title, int rows, ILog log) : base(index, x, y, w, title)
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
