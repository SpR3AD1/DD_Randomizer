using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class Doors
    {
        // Patching doors randomized
        [HarmonyPatch(typeof(ShortcutDoor), "Awake")]
        [HarmonyPostfix]
        public static void Awake_ShortcutDoor_MyPatch(ShortcutDoor __instance, DoorTrigger ___doorTrigger)
        {
            int id = LoadingZones.IDs.FindIndex(x => x.Equals(___doorTrigger.doorId, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                id += LoadingZones.scenes.GetRange(id, 2).FindIndex(x => x.Equals(___doorTrigger.sceneToLoad, StringComparison.OrdinalIgnoreCase));
            }
            if (id > -1)
            {
                string scene_to_load = LoadingZones.scenes[DD_Randomizer.shuffleIDs[id]];
                string door_to_load = LoadingZones.IDs[DD_Randomizer.shuffleIDs[id]].Replace("avarice_", "");
                if (scene_to_load.Contains("AVARICE_WAVES_Mansion") && GameSave.GetSaveData().IsKeyUnlocked("unlocked_fire") ||
                    scene_to_load.Contains("AVARICE_WAVES_Forest") && GameSave.GetSaveData().IsKeyUnlocked("unlocked_bombs") ||
                    scene_to_load.Contains("AVARICE_WAVES_Fortress") && GameSave.GetSaveData().IsKeyUnlocked("unlocked_hookshot"))
                {
                    scene_to_load = "lvl_hallofdoors";
                }
                ___doorTrigger.sceneToLoad = scene_to_load;
                ___doorTrigger.targetDoor = door_to_load;
                ___doorTrigger.doorId = door_to_load;
                __instance.keyId = "";
            }
        }

        // fix save file spawn for doors
        [HarmonyPatch(typeof(DoorTrigger), "loadNextScene")]
        [HarmonyPostfix]
        public static void Save_MyPatch(DoorTrigger __instance)
        {
            if (__instance.doorId != "")
            {
                GameSave.GetSaveData().SetSpawnPoint(__instance.sceneToLoad, __instance.doorId);
            }
            GameSave.SaveGameState();
        }
    }
}
