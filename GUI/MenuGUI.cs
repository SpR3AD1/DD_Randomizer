using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DD_Randomizer
{
    public static class MenuGUI
    {
        public static GUIBox.GUIBox gui;
        public static bool hasInit = false;

        public static Dictionary<string, GUIBox.ToggleOption> toggleOptions = new Dictionary<string, GUIBox.ToggleOption>();

        public static String RandomSeedString;


        public static void Init()
        {
            foreach (var key in DD_Randomizer.Settings.Keys)
            {
                toggleOptions.Add(key, new GUIBox.ToggleOption(DD_Randomizer.Settings[key].Description, DD_Randomizer.Settings[key].toggleState.Value));
            }
            var main = new GUIBox.OptionCategory("Randomizer Options", options: toggleOptions.Values.ToArray());
            gui = new GUIBox.GUIBox(new UnityEngine.Vector2(0.01f, 0.01f), main);

            hasInit = true;
        }

        public static void OnGUI()
        {
            if (!hasInit)
            {
                Init();
            }
            foreach (var key in DD_Randomizer.Settings.Keys)
            {
                DD_Randomizer.Settings[key].toggleState.Value = toggleOptions[key].GetState();
            }

            // RandomSeed GUI
            GUIStyle mylabelStyle = new GUIStyle("label");
            mylabelStyle.fontSize = 50 * Screen.height / 1440;
            mylabelStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - (400 * Screen.width / 2560), 100 * Screen.height / 1440, 150 * Screen.width / 2560, 100 * Screen.height / 1440), "Seed:", mylabelStyle);
            GUIStyle mytextFieldStyle = new GUIStyle("textField");
            mytextFieldStyle.fontSize = 50 * Screen.height / 1440;
            mytextFieldStyle.alignment = TextAnchor.MiddleCenter;
            RandomSeedString = GUI.TextField(new Rect(Screen.width / 2 - (200 * Screen.width / 2560), 100 * Screen.height / 1440, 600 * Screen.width / 2560, 100 * Screen.height / 1440), RandomSeedString, mytextFieldStyle);
            DD_Randomizer.RandomSeed.Value = RandomSeedString;

            gui.OnGUI();
        }
    }
}
