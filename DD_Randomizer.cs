using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using BepInEx.Configuration;
using Vector3 = UnityEngine.Vector3;

namespace DD_Randomizer
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class DD_Randomizer : BaseUnityPlugin
    {
        public const string pluginGuid = "DD_Randomizer";
        public const string pluginName = "DD_Randomizer";
        public const string pluginVersion = "0.2.0";

        // Logger
        public static ManualLogSource Log;

        // list with shuffled IDs
        public static List<int> shuffleIDs = new List<int>(new int[LoadingZones.IDs.Count()]);

        // new file loaded? Used for fixing transition spawn after S&Q
        public static bool freshload = false;

        // Random Seed
        public static ConfigEntry<string> RandomSeed;

        // Struct for making Rndomizer Settings being dynamic
        public struct Setting
        {
            public Setting(ConfigEntry<bool> toggleState, string Description)
            {
                this.toggleState = toggleState;
                this.Description = Description;
            }
            public string Description;
            public ConfigEntry<bool> toggleState;
        }

        // Settings
        public static Dictionary<string, Setting> Settings = new Dictionary<string, Setting>();
        


        public void Awake()
        {
            Log = base.Logger;

            GenerateConfig();

            // Patch
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll(typeof(DD_Randomizer));
            harmony.PatchAll(typeof(ProgressionFlags));
            harmony.PatchAll(typeof(Randomize));
            harmony.PatchAll(typeof(Transitions));
            harmony.PatchAll(typeof(Gondola));
            harmony.PatchAll(typeof(Doors));
            harmony.PatchAll(typeof(Avarice));
            harmony.PatchAll(typeof(Fixes));
        }

        // Gui for entering RandomSeed & Settings
        public void OnGUI()
        {
            if (TitleScreen.instance && TitleScreen.instance.saveMenu.HasControl())
            {
                MenuGUI.OnGUI();
            }
        }

        public void GenerateConfig()
        {
            // Random Seed
            RandomSeed = base.Config.Bind<String>("General",   // The section under which the option is shown
                                    "RandomSeed",  // The key of the configuration option in the configuration file
                                    "hello world", // The default value
                                    "Random Seed"); // Description of the option to show in the config file
            MenuGUI.RandomSeedString = RandomSeed.Value;

            // Settings
            foreach (var key in Settingsclass.Settingslist.Keys)
            {
                Settings.Add(key, new Setting(base.Config.Bind<bool>("Settings", key, false, Settingsclass.Settingslist[key]), Settingsclass.Settingslist[key]));
            }
        }
    }
}
