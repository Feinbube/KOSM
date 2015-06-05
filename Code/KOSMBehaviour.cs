using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KOSM.Examples;
using KOSM.Game;
using KOSM.Windows;

namespace KOSM
{
    public class KOSMBehaviour : MonoBehaviour
    {
        LogWindow persistentDebugWindow = null;
        LogWindow liveDebugWindow = null;

        LogWindow missionLogWindow = null;
        LogWindow missionPlanWindow = null;

        private bool initialized = false;

        private string configName = "kosm.cfg";
        private ConfigNode config = null;

        private Texture2D launcherButtonTexture;

        private ApplicationLauncherButton applicationLauncherButton;

        bool executingScript = false;
        PresentationScript script = new PresentationScript();
        World world = new World();

        public void Awake()
        {
            if (initialized) return;
            else initialized = true;

            config = System.IO.File.Exists(configName) ? ConfigNode.Load(configName) : new ConfigNode(configName);

            launcherButtonTexture = GameDatabase.Instance.GetTexture("KOSM/GFX/launcher-button", false);

            GameEvents.onGUIApplicationLauncherReady.Add(addAppLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(removeAppLauncher);
            GameEvents.onGameStateLoad.Add(gameReset);

            missionLogWindow = new LogWindow(1, 10, 50, 500, "KOSM Mission Log", 10, world.MissionLog);
            missionPlanWindow = new LogWindow(2, 10, 350, 500, "KOSM Mission Plan", 10, world.MissionPlanLog);
            liveDebugWindow = new LogWindow(3, Screen.width - 510, 50, 500, "KOSM Live Debug", 10, world.LiveDebugLog);
            persistentDebugWindow = new LogWindow(4, Screen.width - 510, 350, 500, "KOSM Debug Log", 10, world.DebugLog);
            
        }

        bool first = true;

        public void FixedUpdate()
        {
            if (first)
            {
                first = false;
                world.StartGame("KOSM", "quicksave");
            }

            if (executingScript)
            {
                script.Update(world);
                world.FinishUpdate();
            }
        }

        private void gameReset(ConfigNode game)
        {
            executingScript = false;
            script.Reset(world);
            toggleLauncherButtonToTrue();
        }

        private void addAppLauncher()
        {
            if (ApplicationLauncher.Instance == null || applicationLauncherButton != null)
                return;

            applicationLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                () => { toggleLauncherButtonToTrue(); }, () => { toggleLauncherButtonToFalse(); },
                () => { }, () => { }, () => { }, () => { },
                ApplicationLauncher.AppScenes.ALWAYS, launcherButtonTexture);
        }

        private void removeAppLauncher()
        {
            if (ApplicationLauncher.Instance == null || applicationLauncherButton == null)
                return;

            ApplicationLauncher.Instance.RemoveModApplication(applicationLauncherButton);
            applicationLauncherButton = null;
        }

        private void toggleLauncherButtonToTrue()
        {
            executingScript = true;
            missionLogWindow.Show();
            missionPlanWindow.Show();
            persistentDebugWindow.Show();
            liveDebugWindow.Show();
        }

        private void toggleLauncherButtonToFalse()
        {
            executingScript = false;
            missionLogWindow.Hide();
            missionPlanWindow.Hide();
            persistentDebugWindow.Hide();
            liveDebugWindow.Hide();            
        }
    }
}
