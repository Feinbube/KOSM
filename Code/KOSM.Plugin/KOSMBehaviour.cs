using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Interfaces;
using KOSM.Game;
using KOSM.Examples;
using KOSM.Windows;

namespace KOSM
{
    public class KOSMBehaviour : MonoBehaviour
    {
        LogWindow persistentDebugWindow = null;
        LogWindow liveDebugWindow = null;

        LogWindow missionLogWindow = null;
        LogWindow missionPlanWindow = null;

        ButtonBarWindow controlBar = null;
        ButtonBarWindow scriptBar = null;

        private bool initialized = false;

        private string configName = "kosm.cfg";
        private ConfigNode config = null;

        private Texture2D launcherButtonTexture;

        private ApplicationLauncherButton applicationLauncherButton;

        IScript script = null;

        IWorld world = new World();

        public void Awake()
        {
            if (initialized) return;
            else initialized = true;

            config = System.IO.File.Exists(configName) ? ConfigNode.Load(configName) : new ConfigNode(configName);

            launcherButtonTexture = GameDatabase.Instance.GetTexture("KOSM/GFX/launcher-button", false);

            GameEvents.onGUIApplicationLauncherReady.Add(addAppLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(removeAppLauncher);
            GameEvents.onGameStateLoad.Add(gameReset);

            controlBar = new ButtonBarWindow(1, "KOSM Control", Screen.width / 3 - 100, 0, new string[] { "SB", "ML", "MP", "LD", "DL" }, a => onClicked(a));
            scriptBar = new ButtonBarWindow(2, "KOSM Scripts", Screen.width - 400, Screen.height - 80, new string[] { "None", "Present", "Ascend", "Land", "Maneuver", "Test" }, a => onClicked(a));

            missionLogWindow = new LogWindow(3, "KOSM Mission Log", 0.05, 0.15, 0.3, 0.3, 10, world.MissionLog);
            missionPlanWindow = new LogWindow(4, "KOSM Mission Plan", 0.05, 0.55, 0.3, 0.3, 10, world.MissionPlanLog);
            liveDebugWindow = new LogWindow(5, "KOSM Live Debug", 0.65, 0.15, 0.3, 0.3, 10, world.LiveDebugLog);
            persistentDebugWindow = new LogWindow(6, "KOSM Debug Log", 0.65, 0.55, 0.3, 0.3, 10, world.DebugLog);
        }

        bool first = true;

        public void FixedUpdate()
        {
            if (first)
            {
                first = false;
                world.StartGame("KOSM", "quicksave");
            }

            if (script != null)
            {
                script.Update(world);
                world.FinishUpdate();
            }

            world.LiveDebugLog.Add(String.Format("({0:0},{1:0})", Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            world.LiveDebugLog.Add(String.Format("({0:0},{1:0}) - ({2:0},{3:0})", liveDebugWindow.X, liveDebugWindow.Y, liveDebugWindow.Width + liveDebugWindow.X, liveDebugWindow.Height + liveDebugWindow.Y));
            world.LiveDebugLog.Add(Input.GetMouseButtonDown(0) ? "Mouse is down" : "Mouse is up");
            world.LiveDebugLog.Add(liveDebugWindow.mouse.ToString());

            checkGUI();
        }

        private void gameReset(ConfigNode game)
        {
            script = null;

            world.MissionPlanLog.Clear();
            world.MissionLog.Clear();
            world.DebugLog.Clear();
            world.LiveDebugLog.Clear();

            controlBar.Show();

            script = new PresentationScript();
        }

        private void addAppLauncher()
        {
            if (ApplicationLauncher.Instance == null || applicationLauncherButton != null)
                return;

            applicationLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                () => { controlBar.Show(); }, () => { controlBar.Hide(); }, () => { }, () => { }, () => { }, () => { },
                ApplicationLauncher.AppScenes.ALWAYS, launcherButtonTexture);
        }

        private void removeAppLauncher()
        {
            if (ApplicationLauncher.Instance == null || applicationLauncherButton == null)
                return;

            ApplicationLauncher.Instance.RemoveModApplication(applicationLauncherButton);
            applicationLauncherButton = null;
        }

        private void checkGUI()
        {
            missionLogWindow.Check();
            missionPlanWindow.Check();
            liveDebugWindow.Check();
            persistentDebugWindow.Check();
            controlBar.Check();
            scriptBar.Check();
        }

        private void onClicked(string button)
        {
            // controlBar
            if (button == "ML") missionLogWindow.ToggleShowHide();
            if (button == "MP") missionPlanWindow.ToggleShowHide();
            if (button == "LD") liveDebugWindow.ToggleShowHide();
            if (button == "DL") persistentDebugWindow.ToggleShowHide();
            if (button == "SB") scriptBar.ToggleShowHide();

            // scriptBar
            if (button == "None") script = null;
            if (button == "Present") script = new PresentationScript();
            if (button == "Ascend") script = new AscendToOrbitScript();
            if (button == "Land") script = new LandScript();
            if (button == "Maneuver") script = new ManeuverScript();
            if (button == "Test")
            {
                // test stuff here
            }
        }
    }
}
