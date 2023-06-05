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

        //chandler
        public static GUIBox.ToggleOption chandler = new GUIBox.ToggleOption("Skip bus & Chandler", true);

        //two way locks
        public static GUIBox.ToggleOption gates = new GUIBox.ToggleOption("Allow gate opening from both sides", true);

        //avas
        public static GUIBox.ToggleOption avas = new GUIBox.ToggleOption("Randomize avarices");

        public static void Init()
        {
            DD_Randomizer.Log.LogWarning("init");
            var main = new GUIBox.OptionCategory("Randomizer Options", options: new GUIBox.BaseOption[] { chandler, gates, avas });
            gui = new GUIBox.GUIBox(new UnityEngine.Vector2(0.01f, 0.01f), main);

            hasInit = true;
        }

        public static void OnGUI()
        {
            if (!hasInit)
            {
                Init();
            }
            DD_Randomizer.skipChandler = chandler.GetState();
            DD_Randomizer.twoWayLocks = gates.GetState();
            DD_Randomizer.randomAvas = avas.GetState();

            gui.OnGUI();
        }
    }
}
