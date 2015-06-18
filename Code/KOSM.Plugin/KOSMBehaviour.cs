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

            controlBar = new ButtonBarWindow(1, 200, 0, 50, "KOSM Control", new string[] { "SB", "ML", "MP", "LD", "DL" }, a => onClicked(a));
            scriptBar = new ButtonBarWindow(2, 365, 0, 50, "KOSM Scripts", new string[] { "None", "Present", "Ascend", "Land", "Maneuver", "Test" }, a => onClicked(a));

            missionLogWindow = new LogWindow(3, 10, 70, 500, "KOSM Mission Log", 10, world.MissionLog);
            missionPlanWindow = new LogWindow(4, 10, 300, 500, "KOSM Mission Plan", 10, world.MissionPlanLog);
            liveDebugWindow = new LogWindow(5, Screen.width - 510, 70, 500, "KOSM Live Debug", 10, world.LiveDebugLog);
            persistentDebugWindow = new LogWindow(6, Screen.width - 510, 300, 500, "KOSM Debug Log", 10, world.DebugLog);
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
        }

        private void gameReset(ConfigNode game)
        {
            script = null;

            world.MissionPlanLog.Clear();
            world.MissionLog.Clear();
            world.DebugLog.Clear();
            world.LiveDebugLog.Clear();

            missionLogWindow.Hide();
            missionPlanWindow.Hide();
            liveDebugWindow.Hide();
            persistentDebugWindow.Hide();
            controlBar.Hide();
            scriptBar.Hide();

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
                foreach (string rocketDesign in world.RocketDesigns)
                {
                    world.DebugLog.Add(rocketDesign);
                    world.ToGameLog(rocketDesign);
                }
                world.Launch(world.RocketDesigns[1]);
            }
        }
    }
}
