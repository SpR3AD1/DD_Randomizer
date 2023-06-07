using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class Settingsclass
    {
        public static readonly Dictionary<string, string> Settingslist = new Dictionary<string, string>()
        {
            { "skipChandler", "Skip bus & Chandler" },
            { "twoWayLocks", "Allow gate opening from both sides" },
            { "randomAvas", "Randomize avarices" },
        };
    }
}
