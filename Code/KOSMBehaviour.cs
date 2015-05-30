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
        LogWindow debugWindow = null;
        LogWindow missionWindow = null;

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

            debugWindow = new LogWindow(1, 10, 50, 600, "KOSM Debug UI", world.DebugLog);
            missionWindow = new LogWindow(2, 620, 50, 300, "KOSM Mission Log", world.MissionLog);
        }

        public void Update()
        {
            if (executingScript)
                script.Update(world);
        }
    
        private void gameReset(ConfigNode game)
        {
            executingScript = false;
            script.Reset(world);
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
            debugWindow.Show();
            missionWindow.Show();
        }

        private void toggleLauncherButtonToFalse()
        {
            executingScript = false;
            debugWindow.Hide();
            missionWindow.Hide();
        }
    }
}
