using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class Gondola
    {
        // Patching Gondola randomized
        [HarmonyPatch(typeof(ForestBuggy), "Awake")]
        [HarmonyPostfix]
        public static void Awake_ForestBuggy_MyPatch(ForestBuggy __instance)
        {
            int id = LoadingZones.IDs.FindIndex(x => x.Equals(__instance.doorId, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                id += LoadingZones.scenes.GetRange(id, 2).FindIndex(x => x.Equals(__instance.targetScene, StringComparison.OrdinalIgnoreCase));
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
                __instance.targetScene = scene_to_load;
                __instance.doorId = door_to_load;
            }
        }

        // Patching Gondola PlayerControll flags and Save File Spawn
        [HarmonyPatch(typeof(ForestBuggy), "Trigger")]
        [HarmonyPostfix]
        public static void Trigger_ForestBuggy_MyPatch(ForestBuggy __instance)
        {
            CameraMovementControl.instance.SetCutsceneMode(false);
            PlayerGlobal.instance.UnPauseInput();
            PlayerGlobal.instance.UnPauseInput_Cutscene();
            if ((__instance.doorId.Contains("sdoor_")) && __instance.targetScene.Contains("hallofdoors"))
            {
                GameSave.GetSaveData().SetKeyState(__instance.doorId, true, true);
                GameSave.SaveGameState();
            }
            // you'll spawn on at the door that leads to gondola
            GameSave.GetSaveData().SetSpawnPoint(__instance.targetScene, __instance.doorId);
            GameSave.SaveGameState();
        }
    }
}
