using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class Randomize
    {
        public static int count = 0;

        private static string Seed = DD_Randomizer.RandomSeed.Value;

        // Randomizing
        [HarmonyPatch(typeof(SaveSlot), "LoadSave")]
        [HarmonyPostfix]
        public static void Randomize_on_LoadSave()
        {

            // Remove avarice from the data pool if not checked
            if (!DD_Randomizer.Settings["randomAvas"].toggleState.Value)
            {
                int id = LoadingZones.IDs.FindIndex(x => x.Contains("avarice_"));
                while (id > -1)
                {
                    LoadingZones.IDs.RemoveAt(id);
                    LoadingZones.scenes.RemoveAt(id);
                    id = LoadingZones.IDs.FindIndex(x => x.Contains("avarice_"));
                }
            }

            // initialize lists for randomization
            var Exits = Enumerable.Range(0, LoadingZones.IDs.Count()).ToList();
            var Entrances = Enumerable.Range(0, LoadingZones.IDs.Count()).ToList();
            var OrgExits = Enumerable.Range(0, LoadingZones.IDs.Count()).ToList();
            var OrgEntrances = Enumerable.Range(0, LoadingZones.IDs.Count()).ToList();
            for (int i = 0; i < LoadingZones.IDs.Count(); i++)
            {
                // default value for shuffle-list
                DD_Randomizer.shuffleIDs[i] = -1;
            }

            // find matching entrances to the exits
            for (int i = 0; i < Entrances.Count(); i++)
            {
                int id = LoadingZones.IDs.FindIndex(x => x.Equals(LoadingZones.IDs[i], StringComparison.OrdinalIgnoreCase));
                if (id > -1)
                {
                    id += 1 - LoadingZones.scenes.GetRange(id, 2).FindIndex(x => x.Equals(LoadingZones.scenes[i], StringComparison.OrdinalIgnoreCase));
                }
                if (id > -1)
                {
                    Entrances[i] = id;
                    OrgEntrances[i] = id;
                }
                else
                {
                    DD_Randomizer.Log.LogWarning("Error at generating Random Seed");
                }
            }

            // Initializing RNG-Generator based on Seed Value
            Random rnd = new Random(Seed.GetHashCode());

            // loop through every loading zone
            try
            {
                for (int k = 0; k < LoadingZones.IDs.Count(); k++)
                {
                    if (DD_Randomizer.shuffleIDs[k] < 0)
                    {
                        // IMPLEMENT LOGIC HERE by defining possible exits for an entrance
                        var ActiveExits = new List<int>();
                        for (int i = 0; i < Entrances.Count(); i++)
                        {
                            // Exits for tutorial door in HoD (must not lead to HoD)
                            if ((LoadingZones.IDs[OrgEntrances[k]] == "sdoor_tutorial") && (LoadingZones.scenes[OrgEntrances[k]] == "lvl_hallofdoors") || (LoadingZones.scenes[OrgEntrances[k]].Contains("AVARICE_WAVES_")))
                            {
                                if (!LoadingZones.scenes[Exits[i]].Contains("lvl_hallofdoors"))
                                {
                                    ActiveExits.Add(Exits[i]);
                                }
                            }
                            else
                            {
                                // Exits for other than tutorial door in HoD (must not lead to tutorial door in HoD if coming from HoD)
                                if ((LoadingZones.scenes[OrgEntrances[k]] != "lvl_hallofdoors") || (!((LoadingZones.scenes[Exits[i]].Contains("lvl_hallofdoors") && LoadingZones.IDs[Exits[i]].Contains("sdoor_tutorial")) || LoadingZones.scenes[Exits[i]].Contains("AVARICE_WAVES_"))))
                                {
                                    ActiveExits.Add(Exits[i]);
                                }
                            }
                        }

                        // Remove itself from the possible exits pool
                        ActiveExits.Remove(OrgEntrances[k]);

                        // pick a random entry of the remaining possible exits
                        int swap = rnd.Next(0, ActiveExits.Count());

                        // shuffle two-way connection
                        DD_Randomizer.shuffleIDs[k] = ActiveExits[swap];
                        DD_Randomizer.shuffleIDs[OrgEntrances[OrgExits.FindIndex(x => x == ActiveExits[swap])]] = Entrances[0];

                        // remove used entries from the available pool
                        Exits.Remove(ActiveExits[swap]);
                        Exits.Remove(Entrances[0]);
                        Entrances.Remove(ActiveExits[swap]);
                        Entrances.RemoveAt(0);
                    }
                }
            }
            catch
            {
                Seed = rnd.Next().ToString();
                count++;
                DD_Randomizer.Log.LogWarning("Retry");
                if (count < 100)
                    Randomize_on_LoadSave();
                else
                    DD_Randomizer.shuffleIDs = Enumerable.Range(0, LoadingZones.IDs.Count()).ToList();
            }
        }
    }
}
