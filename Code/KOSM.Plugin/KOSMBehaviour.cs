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

        private DateTime configSaveTime = DateTime.Now;
        private string configName = "KOSM.cfg";
        private ConfigNode config = null;

        private Texture2D launcherButtonTexture;

        private ApplicationLauncherButton applicationLauncherButton;

        IScript script = null;

        IWorld world = new World();

        public void Awake()
        {
            if (initialized) return;
            else initialized = true;

            launcherButtonTexture = GameDatabase.Instance.GetTexture("KOSM/GFX/launcher-button", false);

            GameEvents.onGUIApplicationLauncherReady.Add(addAppLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(removeAppLauncher);
            GameEvents.onGameStateLoad.Add(gameReset);

            loadConfig();
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

            checkGUI();
            checkConfigSave();
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

        private void checkConfigSave()
        {
            if (DateTime.Now < configSaveTime)
                return;

            config.SetNode(windowIdentifier(missionLogWindow), missionLogWindow.AsConfigNode(), true);
            config.SetNode(windowIdentifier(missionPlanWindow), missionPlanWindow.AsConfigNode(), true);
            config.SetNode(windowIdentifier(liveDebugWindow), liveDebugWindow.AsConfigNode(), true);
            config.SetNode(windowIdentifier(persistentDebugWindow), persistentDebugWindow.AsConfigNode(), true);

            config.SetNode(windowIdentifier(controlBar), controlBar.AsConfigNode(), true);
            config.SetNode(windowIdentifier(scriptBar), scriptBar.AsConfigNode(), true);

            config.Save(configName);
            configSaveTime = DateTime.Now.AddSeconds(5);
        }

        private void loadConfig()
        {
            if (!System.IO.File.Exists(configName))
            {
                config = new ConfigNode(configName);

                controlBar = new ButtonBarWindow(1, "KOSM Control", Screen.width / 3 - 100, 0, new string[] { "SB", "ML", "MP", "LD", "DL" }, a => onClicked(a));
                scriptBar = new ButtonBarWindow(2, "KOSM Scripts", Screen.width - 400, Screen.height - 80, new string[] { "None", "Present", "Ascend", "Land", "Maneuver", "Test" }, a => onClicked(a));

                missionLogWindow = new LogWindow(3, "KOSM Mission Log", 0.05, 0.15, 0.3, 0.3, 10, world.MissionLog);
                missionPlanWindow = new LogWindow(4, "KOSM Mission Plan", 0.05, 0.55, 0.3, 0.3, 10, world.MissionPlanLog);
                liveDebugWindow = new LogWindow(5, "KOSM Live Debug", 0.65, 0.15, 0.3, 0.3, 10, world.LiveDebugLog);
                persistentDebugWindow = new LogWindow(6, "KOSM Debug Log", 0.65, 0.55, 0.3, 0.3, 10, world.DebugLog);
            }
            else
            {
                config = ConfigNode.Load(configName);

                controlBar = new ButtonBarWindow(config.GetNode(windowIdentifier("KOSM Control", 1)), new string[] { "SB", "ML", "MP", "LD", "DL" }, a => onClicked(a));
                scriptBar = new ButtonBarWindow(config.GetNode(windowIdentifier("KOSM Scripts", 2)), new string[] { "None", "Present", "Ascend", "Land", "Maneuver", "Test" }, a => onClicked(a));

                missionLogWindow = new LogWindow(config.GetNode(windowIdentifier("KOSM Mission Log", 3)), world.MissionLog);
                missionPlanWindow = new LogWindow(config.GetNode(windowIdentifier("KOSM Mission Plan", 4)), world.MissionPlanLog);
                liveDebugWindow = new LogWindow(config.GetNode(windowIdentifier("KOSM Live Debug", 5)), world.LiveDebugLog);
                persistentDebugWindow = new LogWindow(config.GetNode(windowIdentifier("KOSM Debug Log", 6)), world.DebugLog);
            }
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
