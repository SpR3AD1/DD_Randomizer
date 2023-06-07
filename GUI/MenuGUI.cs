using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class MenuGUI
    {
        public static GUIBox.GUIBox gui;
        public static bool hasInit = false;

        public static Dictionary<string, GUIBox.ToggleOption> toggleOptions = new Dictionary<string, GUIBox.ToggleOption>();


        public static void Init()
        {
            foreach (var key in DD_Randomizer.Settings.Keys)
            {
                toggleOptions.Add(key, new GUIBox.ToggleOption(DD_Randomizer.Settings[key].Description, DD_Randomizer.Settings[key].toggleState.Value));
            }
            var main = new GUIBox.OptionCategory("Randomizer Options", options: new GUIBox.BaseOption[] {toggleOptions["skipChandler"], toggleOptions["twoWayLocks"], toggleOptions["randomAvas"], toggleOptions["test"] });
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

            gui.OnGUI();
        }
    }
}
