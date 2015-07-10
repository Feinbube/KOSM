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
        IWorld world = new World();
        IScript script = null;

        WindowManager windowManager = null;

        private bool initialized = false;
        private double latestUpdate = -1;
        private DateTime latestReset;

        private DateTime configSaveTime = DateTime.Now;
        private string configName = "KOSM.cfg";
        private ConfigNode config = null;

        private Texture2D launcherButtonTexture;
        private ApplicationLauncherButton applicationLauncherButton;

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

        public void FixedUpdate()
        {
            if (latestUpdate == -1)
            {
                latestUpdate = 0;
                world.StartGame("KOSM", "quicksave");
                return;
            }

            if (latestUpdate == world.PointInTime) // just one update per tick (the game calls FixedUpdate way to often!)
            {
                //world.PersistentDebugLog.Add("Multi-Update: " + latestUpdate);
                return;
            }            

            world.LiveDebugLog.Clear();
            latestUpdate = world.PointInTime;

            if (latestReset == DateTime.MinValue)
                latestReset = DateTime.Now;

            if (latestReset > DateTime.Now.AddSeconds(-20)) // the game engine should have some time to load the scene and place the ship
            {
                world.LiveDebugLog.Add("Starting mission in T-" + (latestReset - DateTime.Now.AddSeconds(-20)).TotalSeconds + "s.");
                return;
            }

            if (script != null)
            {
                script.Update(world);
                world.FinishUpdate();
            }

            saveConfig();
        }

        private void gameReset(ConfigNode game)
        {
            world.ClearLogs();

            script = new PresentationScript();

            windowManager.Reset();

            latestReset = DateTime.MinValue;
        }

        private void addAppLauncher()
        {
            if (ApplicationLauncher.Instance == null || applicationLauncherButton != null)
                return;

            applicationLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                () => { windowManager.ToggleVisibility(); }, () => { windowManager.ToggleVisibility(); }, () => { }, () => { }, () => { }, () => { },
                ApplicationLauncher.AppScenes.ALWAYS, launcherButtonTexture);
        }

        private void removeAppLauncher()
        {
            if (ApplicationLauncher.Instance == null || applicationLauncherButton == null)
                return;

            ApplicationLauncher.Instance.RemoveModApplication(applicationLauncherButton);
            applicationLauncherButton = null;
        }

        private void loadConfig()
        {
            if (!System.IO.File.Exists(configName))
            {
                config = new ConfigNode(configName);
                windowManager = new WindowManager(world, a => onClicked(a));
            }
            else
            {
                config = ConfigNode.Load(configName);
                windowManager = new WindowManager(config.GetNode("Windows"), world, a => onClicked(a));
            }
        }

        private void saveConfig()
        {
            if (DateTime.Now < configSaveTime)
                return;

            config.SetNode("Windows", windowManager.AsConfigNode(), true);

            config.Save(configName);
            configSaveTime = DateTime.Now.AddSeconds(5);
        }

        private void onClicked(string button)
        {
            if (button == "None") script = null;
            if (button == "Present") script = new PresentationScript();
            if (button == "Ascend") script = new AscendToOrbitScript();
            if (button == "Land") script = new LandScript();
            if (button == "Maneuver") script = new ManeuverScript();
            if (button == "Test") script = new TestScript();
        }
    }
}
