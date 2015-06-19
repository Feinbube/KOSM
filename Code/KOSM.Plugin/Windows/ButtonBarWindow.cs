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

        public ButtonBarWindow(int index, string title, double xRatio, double yRatio, string[] buttons, Action<string> onClicked)
            : base(index, title, xRatio, yRatio)
        {
            this.onClicked = onClicked;
            this.buttons = buttons;
        }

        public ButtonBarWindow(int index, string title, float x, float y, string[] buttons, Action<string> onClicked)
            : base(index, title, x, y)
        {
            this.onClicked = onClicked;
            this.buttons = buttons;
        }

        protected override void buildLayout()
        {
            GUILayout.BeginHorizontal();

            foreach (string button in buttons)
                if (GUILayout.Button(button, GUI.skin.button))
                    onClicked(button);

            GUILayout.EndHorizontal();
        }
    }
}
