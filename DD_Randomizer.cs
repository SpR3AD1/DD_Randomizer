using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using BepInEx.Configuration;
using Vector3 = UnityEngine.Vector3;

namespace DD_Randomizer
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class DD_Randomizer : BaseUnityPlugin
    {
        public const string pluginGuid = "DD_Randomizer";
        public const string pluginName = "DD_Randomizer";
        public const string pluginVersion = "0.1.0";

        public static ManualLogSource Log;

        // Door or Loadingzone ID
        public static readonly List<string> IDs = new List<string>()
        {
            "sdoor_covenant",
            "sdoor_covenant",
            "sdoor_betty",
            "sdoor_betty",
            "sdoor_sailor",
            "sdoor_sailor",
            "sdoor_fortress",
            "sdoor_fortress",
            "sdoor_mountaintops",
            "sdoor_mountaintops",
            "sdoor_forest_dung",
            "sdoor_forest_dung",
            "sdoor_frogboss",
            "sdoor_frogboss",
            "sdoor_forest",
            "sdoor_forest",
            "sdoor_swamp",
            "sdoor_swamp",
            "sdoor_graveyard",
            "sdoor_graveyard",
            "sdoor_tutorial",
            "sdoor_tutorial",
            "sdoor_grandmaboss",
            "sdoor_grandmaboss",
            "sdoor_mansion",
            "sdoor_mansion",
            "sdoor_basementromp",
            "sdoor_basementromp",
            "sdoor_gardens",
            "sdoor_gardens",
            "hod_anc_forest",
            "hod_anc_forest",
            "hod_anc_fortress",
            "hod_anc_fortress",
            "hod_anc_mansion",
            "hod_anc_mansion",
            "tdoor_gy",
            "tdoor_gy",
            "d_gardenstomansion",
            "d_gardenstomansion",
            "d_mansiontobasement",
            "d_mansiontobasement",
            "d_CrowCavestoMountaintops",
            "d_CrowCavestoMountaintops",
            "d_basementtoboss",
            "d_basementtoboss",
            "d_basementtoromp",
            "d_basementtoromp",
            "d_crypttogardens",
            "d_crypttogardens",
            "d_graveyardtocrypt",
            "d_graveyardtocrypt",
            "d_graveyardtosailorcaves",
            "d_graveyardtosailorcaves",
            "d_connecttosailor",
            "d_connecttosailor",
            "d_sailortofortress",
            "d_sailortofortress",
            "d_fortresstoroof",
            "d_fortresstoroof",
            "d_mountaintopstobetty",
            "d_mountaintopstobetty",
            "d_swamp_enter",
            "d_swamp_enter",
            "d_frog_boss",
            "d_frog_boss",
            "forest_buggy",
            "forest_buggy",/*
            "avarice_hod_anc_forest",
            "avarice_hod_anc_forest",
            "avarice_hod_anc_fortress",
            "avarice_hod_anc_fortress"*/
        };

        // matching scenes
        public static readonly List<string> scenes = new List<string>()
        {
            "lvlConnect_Fortress_Mountaintops",
            "lvl_hallofdoors",
            "boss_betty",
            "lvl_hallofdoors",
            "lvl_SailorMountain",
            "lvl_hallofdoors",
            "lvl_frozenfortress",
            "lvl_hallofdoors",
            "lvl_Mountaintops",
            "lvl_hallofdoors",
            "lvl_Forest",
            "lvl_hallofdoors",
            "boss_frog",
            "lvl_hallofdoors",
            "lvl_Forest",
            "lvl_hallofdoors",
            "lvl_Swamp",
            "lvl_hallofdoors",
            "lvl_Graveyard",
            "lvl_hallofdoors",
            "lvl_Tutorial",
            "lvl_hallofdoors",
            "boss_grandma",
            "lvl_hallofdoors",
            "lvl_GrandmaMansion",
            "lvl_hallofdoors",
            "lvl_GrandmaBasement",
            "lvl_hallofdoors",
            "lvl_GrandmaGardens",
            "lvl_hallofdoors",
            "lvl_Forest",
            "lvl_hallofdoors",
            "lvl_frozenfortress",
            "lvl_hallofdoors",
            "lvl_GrandmaMansion",
            "lvl_hallofdoors",
            "lvl_Graveyard",
            "lvl_Tutorial",
            "lvl_GrandmaGardens",
            "lvl_GrandmaMansion",
            "lvl_GrandmaMansion",
            "lvlConnect_Mansion_Basement",
            "lvlConnect_Fortress_Mountaintops",
            "lvl_mountaintops",
            "lvl_GrandmaBasement",
            "boss_grandma",
            "lvlConnect_Mansion_Basement",
            "lvl_GrandmaBasement",
            "lvlConnect_Graveyard_Gardens",
            "lvl_GrandmaGardens",
            "lvl_graveyard",
            "lvlConnect_Graveyard_Gardens",
            "lvlconnect_graveyard_sailor",
            "lvl_graveyard",
            "lvl_sailormountain",
            "lvlconnect_graveyard_sailor",
            "lvl_frozenfortress",
            "lvl_sailormountain",
            "lvlConnect_Fortress_Mountaintops",
            "lvl_FrozenFortress",
            "boss_betty",
            "lvl_mountaintops",
            "lvl_Swamp",
            "lvl_Forest",
            "boss_frog",
            "lvl_swamp",
            "lvl_Graveyard",
            "lvl_Forest",/*
            "AVARICE_WAVES_Forest",
            "lvl_Forest",
            "AVARICE_WAVES_Fortress",
            "lvl_FrozenFortress"*/
        };

        // list with shuffled IDs
        public static List<int> shuffleIDs = new List<int>(new int[IDs.Count()]);

        // new file loaded? Used for fixing transition spawn after S&Q
        public static bool freshload = false;

        // Random Seed
        public static ConfigEntry<string> RandomSeed;
        public static String RandomSeedString;

        //options
        public static bool skipChandler = true;
        public static bool twoWayLocks = true;
        public static bool randomAvas = false;


        public void Awake()
        {
            Log = base.Logger;
            RandomSeed = base.Config.Bind("General",   // The section under which the option is shown
                                    "RandomSeed",  // The key of the configuration option in the configuration file
                                    "hello world", // The default value
                                    "Random Seed"); // Description of the option to show in the config file
            RandomSeedString = RandomSeed.Value;
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll(typeof(DD_Randomizer));
        }

        public static bool show = false;

        // Gui for entering RandomSeed
        public void OnGUI()
        {
            if (TitleScreen.instance && TitleScreen.instance.saveMenu.HasControl())
            {
                GUIStyle mylabelStyle = new GUIStyle("label");
                mylabelStyle.fontSize = 50;
                mylabelStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(Screen.width / 2 - 400, 100, 150, 100), "Seed:", mylabelStyle);
                GUIStyle mytextFieldStyle = new GUIStyle("textField");
                mytextFieldStyle.fontSize = 50;
                mytextFieldStyle.alignment = TextAnchor.MiddleCenter;
                RandomSeedString = GUI.TextField(new Rect(Screen.width / 2 - 200, 100, 600, 100), RandomSeedString, mytextFieldStyle);
                RandomSeed.Value = RandomSeedString;


                MenuGUI.OnGUI();
            }
        }

        // Randomizing
        [HarmonyPatch(typeof(SaveSlot), "LoadSave")]
        [HarmonyPostfix]
        public static void Randomize()
        {
            var Exits = Enumerable.Range(0, IDs.Count()).ToList();
            var Entries = Enumerable.Range(0, IDs.Count()).ToList();
            var OrgExits = Enumerable.Range(0, IDs.Count()).ToList();
            var OrgEntries = Enumerable.Range(0, IDs.Count()).ToList();
            for (int i = 0; i < IDs.Count(); i++)
            {
                shuffleIDs[i] = -1;
            }
            for (int i = 0; i < Entries.Count(); i++)
            {
                int id = IDs.FindIndex(x => x.Equals(IDs[i], StringComparison.OrdinalIgnoreCase));
                if (id > -1)
                {
                    id += 1 - scenes.GetRange(id, 2).FindIndex(x => x.Equals(scenes[i], StringComparison.OrdinalIgnoreCase));
                }
                if (id > -1)
                {
                    Entries[i] = id;
                    OrgEntries[i] = id;
                }
                else
                {
                    Log.LogWarning("Error at generating Random Seed");
                }
            }

            Random rnd = new Random(RandomSeed.Value.GetHashCode());
            for (int k=0; k<IDs.Count();k++)
            {
                if (shuffleIDs[k] < 0)
                {
                    var ActiveExits = new List<int>();
                    for (int i = 0; i < Entries.Count(); i++)
                    {
                        if ((IDs[OrgEntries[k]] == "sdoor_tutorial") && (scenes[OrgEntries[k]] == "lvl_hallofdoors"))
                        {
                            if (!scenes[Exits[i]].Contains("lvl_hallofdoors"))
                            {
                                ActiveExits.Add(Exits[i]);
                            }
                        }
                        else
                        {
                            if ((scenes[OrgEntries[k]] != "lvl_hallofdoors") || (!(scenes[Exits[i]].Contains("lvl_hallofdoors") && IDs[Exits[i]].Contains("sdoor_tutorial"))))
                            {
                                ActiveExits.Add(Exits[i]);
                            }
                        }
                    }
                    ActiveExits.Remove(OrgEntries[k]);

                    int swap = rnd.Next(0, ActiveExits.Count());

                    shuffleIDs[k] = ActiveExits[swap];
                    shuffleIDs[OrgEntries[OrgExits.FindIndex(x => x == ActiveExits[swap])]] = Entries[0];

                    Exits.Remove(ActiveExits[swap]);
                    Exits.Remove(Entries[0]);
                    Entries.Remove(ActiveExits[swap]);
                    Entries.RemoveAt(0);

                }
            }
        }

        // Patching doors randomized
        [HarmonyPatch(typeof(ShortcutDoor), "Awake")]
        [HarmonyPostfix]
        public static void Awake_ShortcutDoor_MyPatch(ShortcutDoor __instance, DoorTrigger ___doorTrigger)
        {
            int id = IDs.FindIndex(x => x.Equals(___doorTrigger.doorId, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                id += scenes.GetRange(id, 2).FindIndex(x => x.Equals(___doorTrigger.sceneToLoad, StringComparison.OrdinalIgnoreCase));
            }
            if (id > -1)
            {
                string scene_to_load = scenes[shuffleIDs[id]];
                string door_to_load = IDs[shuffleIDs[id]];
                ___doorTrigger.sceneToLoad = scene_to_load;
                ___doorTrigger.targetDoor = door_to_load;
                ___doorTrigger.doorId = door_to_load;
                __instance.keyId = "";
            }
        }

        // Patching Gondola randomized
        [HarmonyPatch(typeof(ForestBuggy), "Awake")]
        [HarmonyPostfix]
        public static void Awake_ForestBuggy_MyPatch(ForestBuggy __instance)
        {
            int id = IDs.FindIndex(x => x.Equals(__instance.doorId, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                id += scenes.GetRange(id, 2).FindIndex(x => x.Equals(__instance.targetScene, StringComparison.OrdinalIgnoreCase));
            }
            if (id > -1)
            {
                string scene_to_load = scenes[shuffleIDs[id]];
                string door_to_load = IDs[shuffleIDs[id]];
                __instance.targetScene = scene_to_load;
                __instance.doorId = door_to_load;
            }
        }

        // Patching Gondola PlayerControll flags and Save File Spawn
        [HarmonyPatch(typeof(ForestBuggy), "Trigger")]
        [HarmonyPostfix]
        public static void Trigger_ForestBuggy_MyPatch(ForestBuggy __instance)
        {
            Log.LogWarning("Gondola Trigger");
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

        // Patching two way locks
        [HarmonyPatch(typeof(ButtonPromptArea), "Start")]
        [HarmonyPostfix]
        public static void Lock_Triggerarea_MyPatch(ButtonPromptArea __instance)
        {
            //if (Settings["two_way_locks"])
            //{ 
                if (__instance.prompt == "prompt_unlock")
                {
                    __instance.transform.localPosition = new Vector3(0, -1, 0);
                }
            //}
        }

        // Patching Avarice_Enter randomized
        [HarmonyPatch(typeof(SceneLoader), "LoadScene")]
        [HarmonyPrefix]
        public static bool Avarice_Enter_MyPatch(SceneLoader __instance)
        {
            Log.LogWarning("Enter avarice: " + __instance.sceneName);
            int id = scenes.FindIndex(x => x.Equals(__instance.sceneName, StringComparison.OrdinalIgnoreCase));
            Log.LogWarning("id1: " + id.ToString());
            if (id > -1)
            {
                string scene_to_load = scenes[shuffleIDs[id]];
                string door_to_load = IDs[shuffleIDs[id]];
                __instance.sceneName = scene_to_load;
                GameSave.GetSaveData().SetSpawnPoint(scene_to_load, door_to_load);
                GameSave.SaveGameState();
                Log.LogWarning("Enter avarice new: " + __instance.sceneName);
            }
            return true;
        }

        // Patching Avarice_Leave randomized
        [HarmonyPatch(typeof(RespawnPrompter), "Leave")]
        [HarmonyPrefix]
        public static bool Avarice_Leave_MyPatch(RespawnPrompter __instance)
        {
            int id = IDs.FindIndex(x => x.Equals("avarice_" + __instance.returnDoorId, StringComparison.OrdinalIgnoreCase));
            if (id > -1)
            {
                id += scenes.GetRange(id, 2).FindIndex(x => x.Equals(__instance.returnSceneId, StringComparison.OrdinalIgnoreCase));
            }
            if (id > -1)
            {
                string scene_to_load = scenes[shuffleIDs[id]];
                string door_to_load = IDs[shuffleIDs[id]];
                __instance.returnSceneId = scene_to_load;
                __instance.returnDoorId = door_to_load;
            }
            return true;
        }

        // Patching transition triggers randomized
        [HarmonyPatch(typeof(DoorTrigger), "OnTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnTriggerEnter_MyPatch(DoorTrigger __instance, Collider collider, ShortcutDoor ___parentDoor, bool ___triggered)
        {
            if (___parentDoor == null)
            {
                int id = IDs.FindIndex(x => x.Equals(__instance.doorId, StringComparison.OrdinalIgnoreCase));
                if (id > -1)
                {
                    id += scenes.GetRange(id, 2).FindIndex(x => x.Equals(__instance.sceneToLoad, StringComparison.OrdinalIgnoreCase));
                }
                PlayerInputControl component = collider.gameObject.GetComponent<PlayerInputControl>();
                if (id > -1 && component != null && ___triggered == false)
                {
                    string scene_to_load = scenes[shuffleIDs[id]];
                    string door_to_load = IDs[shuffleIDs[id]];
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

        // fix S&Q transition spawn
        [HarmonyPatch(typeof(DoorTrigger), "Start")]
        [HarmonyPostfix]
        public static void DoorTriggerSpawn_mypatch(DoorTrigger __instance)
        {
            if (freshload)
            {
                if (__instance.doorId == GameSave.GetSaveData().GetSpawnID())
                {
                    PlayerGlobal.instance.SetPosition(__instance.spawnPoint.position, false, false);
                    PlayerGlobal.instance.SetRotation(__instance.spawnPoint.rotation);
                    PlayerGlobal.SetSpawnPos(__instance.spawnPoint.position, __instance.spawnPoint.rotation);
                    PlayerGlobal.instance.SetSafePos(__instance.spawnPoint.position);
                    freshload = false;
                }
            }
        }

        // fix death Spawn
        [HarmonyPatch(typeof(ShortcutDoor), "SetAsRespawnDoor")]
        [HarmonyPrefix]
        public static bool SetAsRespawnDoor_mypatch()
        {
            return false;
        }

        // fix death Spawn
        [HarmonyPatch(typeof(DoorOverrider), "activate")]
        [HarmonyPrefix]
        public static bool DoorOverrider_activate_mypatch()
        {
            return false;
        }

        // Set start flags to fix progression bugs with cutscenes
        [HarmonyPatch(typeof(SaveSlot), "useSaveFile")]
        [HarmonyPostfix]
        public static void LoadDefaultValues_MyPatch()
        {
            freshload = true;
            if (!GameSave.GetSaveData().IsKeyUnlocked("cts_bus"))
            {
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

                //if (Settings["chandlerskip"])
                //{
                    GameSave.GetSaveData().SetKeyState("cts_bus", true, true);
                    GameSave.GetSaveData().SetKeyState("handler_intro", true, true);
                    GameSave.GetSaveData().SetKeyState("sdoor_tutorial_hub", true, true);
                    GameSave.GetSaveData().SetKeyState("sdoor_tutorial", true, true);
                    GameSave.GetSaveData().SetKeyState("handler_intro2", true, true);
                    GameSave.GetSaveData().SetKeyState("handler_intro3", true, true);
                    GameSave.GetSaveData().SetKeyState("cts_handler", true, true);

                    // set spawn to tutorial door
                    GameSave.GetSaveData().SetSpawnPoint("lvl_hallofdoors", "sdoor_tutorial");
                //}

                //GameSave.GetSaveData().SetSpawnPoint("avarice1_spawn", "AVARICE_WAVES_Forest");

                GameSave.SaveGameState();
            }
        }
    }
}
