using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Interfaces;

namespace KOSM.Windows
{
    public class WindowManager
    {
        IWorld world = null;

        private Callback drawCallback;

        LogWindow persistentDebugWindow = null;
        LogWindow liveDebugWindow = null;

        LogWindow missionLogWindow = null;
        LogWindow missionPlanWindow = null;

        ButtonBarWindow controlBar = null;
        ButtonBarWindow scriptBar = null;

        Action<string> onClickedCallback = null;

        public WindowManager(IWorld world, Action<string> onClickedAction)
        {
            controlBar = new ButtonBarWindow(1, "KOSM Control", Screen.width / 3 - 100, 0, new string[] { "SB", "ML", "MP", "LD", "DL" }, a => onClicked(a));
            scriptBar = new ButtonBarWindow(2, "KOSM Scripts", Screen.width - 400, Screen.height - 80, new string[] { "None", "Present", "Ascend", "Land", "Maneuver", "Test" }, a => onClicked(a));

            missionLogWindow = new LogWindow(3, "KOSM Mission Log", 0.05, 0.15, 0.3, 0.3, 10, world.MissionLog);
            missionPlanWindow = new LogWindow(4, "KOSM Mission Plan", 0.05, 0.55, 0.3, 0.3, 10, world.MissionPlanLog);
            liveDebugWindow = new LogWindow(5, "KOSM Live Debug", 0.65, 0.15, 0.3, 0.3, 30, world.LiveDebugLog);
            persistentDebugWindow = new LogWindow(6, "KOSM Debug Log", 0.65, 0.55, 0.3, 0.3, 10, world.PersistentDebugLog);

            this.drawCallback = drawWindows;
            this.onClickedCallback = onClickedAction;
            this.world = world;
        }

        public WindowManager(ConfigNode node, IWorld world, Action<string> onClickedAction)
        {
            controlBar = new ButtonBarWindow(node.GetNode(windowIdentifier("KOSM Control", 1)), new string[] { "SB", "ML", "MP", "LD", "DL" }, a => onClicked(a));
            scriptBar = new ButtonBarWindow(node.GetNode(windowIdentifier("KOSM Scripts", 2)), new string[] { "None", "Present", "Ascend", "Land", "Maneuver", "Test" }, a => onClicked(a));

            missionLogWindow = new LogWindow(node.GetNode(windowIdentifier("KOSM Mission Log", 3)), world.MissionLog);
            missionPlanWindow = new LogWindow(node.GetNode(windowIdentifier("KOSM Mission Plan", 4)), world.MissionPlanLog);
            liveDebugWindow = new LogWindow(node.GetNode(windowIdentifier("KOSM Live Debug", 5)), world.LiveDebugLog);
            persistentDebugWindow = new LogWindow(node.GetNode(windowIdentifier("KOSM Debug Log", 6)), world.PersistentDebugLog);

            this.drawCallback = drawWindows;
            this.onClickedCallback = onClickedAction;
            this.world = world;
        }

        public void Reset()
        {
            RenderingManager.AddToPostDrawQueue(3, drawCallback);
            controlBar.IsVisible = true;
        }


        public void ToggleVisibility()
        {
            controlBar.ToggleVisibility();
        }

        private void drawWindows()
        {
            buildSkin();

            missionPlanWindow.Draw();
            missionLogWindow.Draw();
            persistentDebugWindow.Draw();
            liveDebugWindow.Draw();

            controlBar.Draw();
            scriptBar.Draw();
        }

        private void buildSkin()
        {
            GUI.skin = HighLogic.Skin;
            GUI.skin.label.fontSize = 11;
            GUI.skin.label.margin = new RectOffset(1, 1, 1, 1);
            GUI.skin.label.padding = new RectOffset(0, 0, 2, 2);
        }

        public ConfigNode AsConfigNode()
        {
            ConfigNode node = new ConfigNode("Windows");

            node.SetNode(windowIdentifier(missionLogWindow), missionLogWindow.AsConfigNode(), true);
            node.SetNode(windowIdentifier(missionPlanWindow), missionPlanWindow.AsConfigNode(), true);
            node.SetNode(windowIdentifier(liveDebugWindow), liveDebugWindow.AsConfigNode(), true);
            node.SetNode(windowIdentifier(persistentDebugWindow), persistentDebugWindow.AsConfigNode(), true);

            node.SetNode(windowIdentifier(controlBar), controlBar.AsConfigNode(), true);
            node.SetNode(windowIdentifier(scriptBar), scriptBar.AsConfigNode(), true);

            return node;
        }

        private string windowIdentifier(Window window)
        {
            return window.Title.Replace(" ", "_") + "_" + window.Index;
        }

        private string windowIdentifier(string title, int index)
        {
            return title.Replace(" ", "_") + "_" + index;
        }

        private void onClicked(string button)
        {
            // controlBar
            if (button == "ML") missionLogWindow.ToggleVisibility();
            if (button == "MP") missionPlanWindow.ToggleVisibility();
            if (button == "LD") liveDebugWindow.ToggleVisibility();
            if (button == "DL") persistentDebugWindow.ToggleVisibility();
            if (button == "SB") scriptBar.ToggleVisibility();

            onClickedCallback(button);
        }
    }
}
