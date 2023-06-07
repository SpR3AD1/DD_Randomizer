using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DD_Randomizer
{
    public static class Transitions
    {
        // Patching transition triggers randomized
        [HarmonyPatch(typeof(DoorTrigger), "OnTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnTriggerEnter_MyPatch(DoorTrigger __instance, Collider collider, ShortcutDoor ___parentDoor, bool ___triggered)
        {
            if (___parentDoor == null)
            {
                int id = LoadingZones.IDs.FindIndex(x => x.Equals(__instance.doorId, StringComparison.OrdinalIgnoreCase));
                if (id > -1)
                {
                    id += LoadingZones.scenes.GetRange(id, 2).FindIndex(x => x.Equals(__instance.sceneToLoad, StringComparison.OrdinalIgnoreCase));
                }
                PlayerInputControl component = collider.gameObject.GetComponent<PlayerInputControl>();
                if (id > -1 && component != null && ___triggered == false)
                {
                    string scene_to_load = LoadingZones.scenes[DD_Randomizer.shuffleIDs[id]];
                    string door_to_load = LoadingZones.IDs[DD_Randomizer.shuffleIDs[id]].Replace("avarice_", "");
                    if (scene_to_load.Contains("AVARICE_WAVES_Mansion") && GameSave.GetSaveData().IsKeyUnlocked("unlocked_fire") ||
                        scene_to_load.Contains("AVARICE_WAVES_Forest") && GameSave.GetSaveData().IsKeyUnlocked("unlocked_bombs") ||
                        scene_to_load.Contains("AVARICE_WAVES_Fortress") && GameSave.GetSaveData().IsKeyUnlocked("unlocked_hookshot"))
                    {
                        scene_to_load = "lvl_hallofdoors";
                    }
                    __instance.sceneToLoad = scene_to_load;
                    __instance.targetDoor = door_to_load;
                    __instance.doorId = door_to_load;
                }
            }
            if ((__instance.targetDoor.Contains("sdoor_")) && __instance.sceneToLoad.Contains("hallofdoors"))
            {
                GameSave.GetSaveData().SetKeyState(__instance.targetDoor, true, true);
                GameSave.SaveGameState();
            }
            return true;
        }

        // fix S&Q transition spawn
        [HarmonyPatch(typeof(DoorTrigger), "Start")]
        [HarmonyPostfix]
        public static void DoorTriggerSpawn_mypatch(DoorTrigger __instance)
        {
            if (DD_Randomizer.freshload)
            {
                if (__instance.doorId == GameSave.GetSaveData().GetSpawnID())
                {
                    PlayerGlobal.instance.SetPosition(__instance.spawnPoint.position, false, false);
                    PlayerGlobal.instance.SetRotation(__instance.spawnPoint.rotation);
                    PlayerGlobal.SetSpawnPos(__instance.spawnPoint.position, __instance.spawnPoint.rotation);
                    PlayerGlobal.instance.SetSafePos(__instance.spawnPoint.position);
                    DD_Randomizer.freshload = false;
                }
            }
        }
    }
}
