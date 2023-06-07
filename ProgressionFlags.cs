using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Randomizer
{
    public static class ProgressionFlags
    {
        // Set start flags to fix progression bugs with cutscenes
        [HarmonyPatch(typeof(SaveSlot), "useSaveFile")]
        [HarmonyPostfix]
        public static void LoadDefaultValues_MyPatch()
        {
            DD_Randomizer.freshload = true;
            if (!GameSave.GetSaveData().IsKeyUnlocked("cts_bus"))
            {
                GameSave.GetSaveData().SetKeyState("crow_cut1", true, true);
                GameSave.GetSaveData().SetKeyState("bard_bar_intro", true, true);
                GameSave.GetSaveData().SetKeyState("bard_cracked_block", true, true);
                GameSave.GetSaveData().SetKeyState("bard_fort_intro", true, true);
                GameSave.GetSaveData().SetKeyState("bard_fortress", true, true);
                GameSave.GetSaveData().SetKeyState("bard_crows", true, true);
                GameSave.GetSaveData().SetKeyState("bard_betty_cave", true, true);
                GameSave.GetSaveData().SetKeyState("bard_pre_betty", true, true);
                GameSave.GetSaveData().SetKeyState("pothead_intro_1", true, true);
                GameSave.GetSaveData().SetKeyState("pothead_intro_2", true, true);
                GameSave.GetSaveData().SetKeyState("pothead_intro_3", true, true);
                GameSave.GetSaveData().SetKeyState("potkey_intro", true, true);
                GameSave.GetSaveData().SetKeyState("pothead_confession1", true, true);
                GameSave.GetSaveData().SetKeyState("pothead_m_4", true, true);
                GameSave.GetSaveData().SetKeyState("phcs_1", true, true);
                GameSave.GetSaveData().SetKeyState("phcs_1.5", true, true);
                GameSave.GetSaveData().SetKeyState("phcs_5", true, true);
                GameSave.GetSaveData().SetKeyState("phcs_break", true, true);
                GameSave.GetSaveData().SetKeyState("phcs_2", true, true);
                GameSave.GetSaveData().SetKeyState("phcs_3", true, true);
                GameSave.GetSaveData().SetKeyState("ach_pothead", true, true);
                GameSave.GetSaveData().SetKeyState("frog_boss_wall_chat", true, true);
                GameSave.GetSaveData().SetKeyState("frog_dung_meet_1", true, true);
                GameSave.GetSaveData().SetKeyState("watched_frogwall", true, true);
                GameSave.GetSaveData().SetKeyState("frog_boss_swim_chat", true, true);
                GameSave.GetSaveData().SetKeyState("frog_dung_meet_3", true, true);
                GameSave.GetSaveData().SetKeyState("watched_frogswim", true, true);
                GameSave.GetSaveData().SetKeyState("frog_boss_sewer_chat", true, true);
                GameSave.GetSaveData().SetKeyState("frog_dung_meet_2", true, true);
                GameSave.GetSaveData().SetKeyState("watched_frogsewer", true, true);
                GameSave.GetSaveData().SetKeyState("frog_wall_chat_last", true, true);
                GameSave.GetSaveData().SetKeyState("frog_dung_meet_last", true, true);
                GameSave.GetSaveData().SetKeyState("frog_ghoul_intro", true, true);
                GameSave.GetSaveData().SetKeyState("c_swamp_intro", true, true);
                GameSave.GetSaveData().SetKeyState("shop_prompted", true, true);

                if (DD_Randomizer.Settings["skipChandler"].toggleState.Value)
                {
                    GameSave.GetSaveData().SetKeyState("cts_bus", true, true);
                    GameSave.GetSaveData().SetKeyState("handler_intro", true, true);
                    GameSave.GetSaveData().SetKeyState("sdoor_tutorial_hub", true, true);
                    GameSave.GetSaveData().SetKeyState("sdoor_tutorial", true, true);
                    GameSave.GetSaveData().SetKeyState("handler_intro2", true, true);
                    GameSave.GetSaveData().SetKeyState("handler_intro3", true, true);
                    GameSave.GetSaveData().SetKeyState("cts_handler", true, true);

                    // set spawn to tutorial door
                    GameSave.GetSaveData().SetSpawnPoint("lvl_hallofdoors", "sdoor_tutorial");
                }

                //GameSave.GetSaveData().SetSpawnPoint("lvl_Graveyard", "forest_buggy");
                //DoorTrigger.currentTargetDoor = "forest_buggy";

                GameSave.SaveGameState();
            }
        }
    }
}
