using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KOSM.Windows
{
    public class ButtonBarWindow : Window
    {
        Action<string> onClicked;
        string[] buttons;

        public ButtonBarWindow(int index, float x, float y, float w, string title, string[] buttons, Action<string> onClicked) : base(index, x, y, w, title)
        {
            this.onClicked = onClicked;
            this.buttons = buttons;
        }

        protected override void buildLayout()
        {
            GUILayout.BeginHorizontal();

            foreach (string button in buttons)
                if (GUILayout.Button(button, GUI.skin.button, GUILayout.ExpandWidth(true)))
                    onClicked(button);

            GUILayout.EndHorizontal();
        }
    }
}
