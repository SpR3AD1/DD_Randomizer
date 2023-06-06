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
        public static GUIBox.ToggleOption chandler = new GUIBox.ToggleOption("Skip bus & Chandler", DD_Randomizer.skipChandler.Value);

        //two way locks
        public static GUIBox.ToggleOption gates = new GUIBox.ToggleOption("Allow gate opening from both sides", DD_Randomizer.twoWayLocks.Value);

        //avas
        public static GUIBox.ToggleOption avas = new GUIBox.ToggleOption("Randomize avarices", DD_Randomizer.randomAvas.Value);

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
            DD_Randomizer.skipChandler.Value = chandler.GetState();
            DD_Randomizer.twoWayLocks.Value = gates.GetState();
            DD_Randomizer.randomAvas.Value = avas.GetState();

            gui.OnGUI();
        }
    }
}
