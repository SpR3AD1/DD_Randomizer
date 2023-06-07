using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class Avarice
    {
        // Patching Avarice_Enter randomized
        [HarmonyPatch(typeof(SceneLoader), "LoadScene")]
        [HarmonyPrefix]
        public static bool Avarice_Enter_MyPatch(SceneLoader __instance)
        {
            int id = LoadingZones.scenes.FindIndex(x => x.Equals(__instance.sceneName, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                string scene_to_load = LoadingZones.scenes[DD_Randomizer.shuffleIDs[id]];
                string door_to_load = LoadingZones.IDs[DD_Randomizer.shuffleIDs[id]].Replace("avarice_", "");
                __instance.sceneName = scene_to_load;
                if ((door_to_load.Contains("sdoor_")) && scene_to_load.Contains("hallofdoors"))
                {
                    GameSave.GetSaveData().SetKeyState(door_to_load, true, true);
                }
                GameSave.GetSaveData().SetSpawnPoint(scene_to_load, door_to_load);
                DoorTrigger.currentTargetDoor = door_to_load.Replace("avarice_", "");
                GameSave.SaveGameState();
            }
            return true;
        }

        // Patching Avarice_Leave randomized
        [HarmonyPatch(typeof(RespawnPrompter), "Leave")]
        [HarmonyPrefix]
        public static bool Avarice_Leave_MyPatch(RespawnPrompter __instance)
        {
            int id = LoadingZones.IDs.FindIndex(x => x.Equals("avarice_" + __instance.returnDoorId, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                id += LoadingZones.scenes.GetRange(id, 2).FindIndex(x => x.Equals("lvl_hallofdoors", StringComparison.OrdinalIgnoreCase));
            }
            if (id > -1)
            {
                string scene_to_load = LoadingZones.scenes[DD_Randomizer.shuffleIDs[id]];
                string door_to_load = LoadingZones.IDs[DD_Randomizer.shuffleIDs[id]].Replace("avarice_", "");
                __instance.returnSceneId = scene_to_load;
                __instance.returnDoorId = door_to_load.Replace("avarice_", "");
                DoorTrigger.currentTargetDoor = door_to_load.Replace("avarice_", "");
            }
            return true;
        }

        // fix avarice flags for reentering
        [HarmonyPatch(typeof(GameSave), "SetKeyState")]
        [HarmonyPrefix]
        public static bool SetKeyState_mypatch(string id, bool state, bool save = false)
        {
            if (id == "unlocked_fire")
            {
                GameSave.GetSaveData().SetKeyState("avarice_mansion_won", false, true);
            }
            if (id == "unlocked_bombs")
            {
                GameSave.GetSaveData().SetKeyState("avarice_forest_won", false, true);
            }
            if (id == "unlocked_hookshot")
            {
                GameSave.GetSaveData().SetKeyState("avarice_fortress_won", false, true);
            }
            return true;
        }
    }
}
