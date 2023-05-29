using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;
using Random = System.Random;
using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using AK.Wwise;

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
            "forest_buggy"
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
            "lvl_Forest"
        };

        // list with shuffled IDs
        public static List<int> shuffleIDs = new List<int>(new int[IDs.Count()]);

        public static bool freshload = false;

        // Random Seed
        public static ConfigEntry<string> RandomSeed;
        public static String RandomSeedString;

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
            }
        }

        // Randomizing
        [HarmonyPatch(typeof(SaveSlot), "LoadSave")]
        [HarmonyPostfix]
        public static void Randomize()
        {
            var Entries = Enumerable.Range(0, IDs.Count()).ToList();
            var Exits = Enumerable.Range(0, IDs.Count()).ToList();
            for (int i = 0; i < Exits.Count(); i++)
            {
                int id = IDs.FindIndex(x => x.Equals(IDs[i], StringComparison.OrdinalIgnoreCase));
                if (id > -1)
                {
                    id += 1 - scenes.GetRange(id, 2).FindIndex(x => x.Equals(scenes[i], StringComparison.OrdinalIgnoreCase));
                }
                if (id > -1)
                {
                    Exits[i] = id;
                }
                else
                {
                    Log.LogWarning("Error at generating Rondom Seed");
                }
            }

            Random rnd = new Random(RandomSeed.Value.GetHashCode());
            while (Exits.Count() > 0)
            {
                var ActiveExits = new List<int>();
                for (int i = 1; i < Entries.Count(); i++)
                {
                    if ((IDs[Entries[0]] == "sdoor_tutorial") && (scenes[Entries[0]] == "lvl_Tutorial"))
                    {
                        if (scenes[Entries[i]].Contains("lvl_hallofdoors"))
                        {
                            ActiveExits.Add(Exits[i]);
                        }
                    }
                    else
                    {
                        if ((scenes[Entries[0]] != "lvl_hallofdoors") || !(scenes[Entries[i]].Contains("lvl_hallofdoors") && IDs[Entries[i]].Contains("sdoor_tutorial")))
                        {
                            ActiveExits.Add(Exits[i]);
                        }
                    }
                }

                int swap = rnd.Next(0, ActiveExits.Count());

                shuffleIDs[Entries[0]] = ActiveExits[swap];
                shuffleIDs[Entries[Exits.FindIndex(x => x == ActiveExits[swap])]] = Exits[0];

                Entries.RemoveAt(Exits.FindIndex(x => x == ActiveExits[swap]));
                Entries.RemoveAt(0);
                Exits.Remove(ActiveExits[swap]);
                Exits.RemoveAt(0);

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
                if (scene_to_load == "lvl_hallofdoors")
                    __instance.keyId = IDs[shuffleIDs[id]];
                else
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
            CameraMovementControl.instance.SetCutsceneMode(false);
            PlayerGlobal.instance.UnPauseInput();
            if ((__instance.doorId.Contains("sdoor_")) && __instance.targetScene.Contains("hallofdoors"))
            {
                GameSave.GetSaveData().SetKeyState(__instance.doorId, true, true);
                GameSave.SaveGameState();
            }
        }

        // Patching transition triggers randomized
        [HarmonyPatch(typeof(DoorTrigger), "OnTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnTriggerEnter_MyPatch(DoorTrigger __instance, Collider collider, ShortcutDoor ___parentDoor, bool ___triggered)
        {
            Log.LogWarning("hit trigger: " + __instance.doorId + " " + __instance.sceneToLoad);
            if (___parentDoor == null)
            {

                //Log.LogWarning("hit trigger: " + __instance.doorId + " " + __instance.sceneToLoad);
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
                    //Log.LogWarning("changed to: " + door_to_load + " " + scene_to_load);
                    if ((door_to_load.Contains("sdoor_")) && scene_to_load.Contains("hallofdoors"))
                    {
                        GameSave.GetSaveData().SetKeyState(door_to_load, true, true);
                        GameSave.SaveGameState();
                    }
                }
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
                Log.LogWarning("saved: " + __instance.sceneToLoad.ToString() + " " + __instance.doorId.ToString());
                GameSave.GetSaveData().SetSpawnPoint(__instance.sceneToLoad, __instance.doorId);
            }
            GameSave.SaveGameState();
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
            if (!GameSave.GetSaveData().IsKeyUnlocked("cts_bus"))
            {
                GameSave.GetSaveData().SetKeyState("cts_bus", true, true);
                GameSave.GetSaveData().SetKeyState("handler_intro", true, true);
                GameSave.GetSaveData().SetKeyState("sdoor_tutorial_hub", true, true);
                GameSave.GetSaveData().SetKeyState("sdoor_tutorial", true, true);
                GameSave.GetSaveData().SetKeyState("handler_intro2", true, true);
                GameSave.GetSaveData().SetKeyState("handler_intro3", true, true);
                GameSave.GetSaveData().SetKeyState("cts_handler", true, true);
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

                // set spawn to tutorial door
                GameSave.GetSaveData().SetSpawnPoint("lvl_hallofdoors", "sdoor_tutorial");
                // set spawn to lockstone
                GameSave.GetSaveData().SetSpawnPoint("lvl_frozenfortress", "d_sailortofortress");

                GameSave.SaveGameState();

                freshload = true;
            }
        }

        // in progress - why is GetSpawnID() empty? - Maybe not needed if GetSpawnID() is fixed
        [HarmonyPatch(typeof(DoorTrigger), "Start")]
        [HarmonyPostfix]
        public static void DoorTriggerSpawn_mypatch(DoorTrigger __instance)
        {
            /*
            Log.LogWarning("DoorTrigger: " + freshload.ToString());
            if (freshload)
            {
                Log.LogWarning("DoorTrigger - spawn: " + GameSave.GetSaveData().GetSpawnID());
                if (__instance.doorId == GameSave.GetSaveData().GetSpawnID())
                {
                    PlayerGlobal.instance.SetPosition(__instance.spawnPoint.position, false, false);
                    PlayerGlobal.instance.SetRotation(__instance.spawnPoint.rotation);
                    PlayerGlobal.SetSpawnPos(__instance.spawnPoint.position, __instance.spawnPoint.rotation);
                    PlayerGlobal.instance.SetSafePos(__instance.spawnPoint.position);
                    freshload = false;
                }
            }*/
        }

        // in progress - just logging
        [HarmonyPatch(typeof(GameSave), "populateDataStructure")]
        [HarmonyPrefix]
        public static bool Respawn54s_mypatch(string ___spawnId)
        {
            Log.LogWarning("populateDataStructure - SpawnID: " + ___spawnId);
            return true;
        }

        // in progress - just logging
        [HarmonyPatch(typeof(GameSave), "Load")]
        [HarmonyPostfix]
        public static void Respawn54_mypatch(string ___spawnId)
        {
            Log.LogWarning("Load spawnID: " + ___spawnId);
            /*try
            {
                string callingFuncName = new StackFrame(1).GetMethod().Name;
                string callingFuncType = new StackFrame(1).GetMethod().DeclaringType.ToString();
                for (int i = 1; i < 3; i++)
                {
                    callingFuncName = new StackFrame(i).GetMethod().Name;
                    callingFuncType = new StackFrame(i).GetMethod().DeclaringType.ToString();
                    Log.LogWarning("caller[" + i.ToString() + "]: " + callingFuncType + "." + callingFuncName);
                }
            }
            finally { }*/
        }

        // in progress - just logging
        [HarmonyPatch(typeof(ShortcutDoor), "startUp")]
        [HarmonyPostfix]
        public static void Respawn3_mypatch(ShortcutDoor __instance, bool ___didStartup)
        {
            Log.LogWarning("ShortcutDoor starup spawn: " + GameSave.currentSave.GetSpawnID());
            /*
            if (!___didStartup)
            {
                DoorTrigger[] hinges = FindObjectsOfType(typeof(DoorTrigger)) as DoorTrigger[];
                foreach (DoorTrigger hinge in hinges)
                {
                    Log.LogWarning("---------------id spawn: " + hinge.doorId + "  " + GameSave.currentSave.GetSpawnID());
                    if (hinge.doorId == GameSave.currentSave.GetSpawnID())
                    {
                        PlayerGlobal.SetSpawnPos(hinge.transform.position, hinge.transform.rotation);
                        PlayerGlobal.instance.SetPosition(hinge.transform.position, false, false);
                    }

                }
                Log.LogWarning("keyId: " + __instance.keyId);
                Log.LogWarning("GetSpawnId: " + GameSave.currentSave.GetSpawnID());
                Log.LogWarning("spawnedatDoor: " + DoorTrigger.spawnedAtDoor.ToString());
                if (__instance.keyId == GameSave.currentSave.GetSpawnID() && !DoorTrigger.spawnedAtDoor)
                {
                    Log.LogWarning("-------------------------------start2: ");
                    DoorTrigger[] hinges = FindObjectsOfType(typeof(DoorTrigger)) as DoorTrigger[];
                    foreach (DoorTrigger hinge in hinges)
                    {
                        Log.LogWarning("---------------id spawn: " + hinge.doorId + "  " + GameSave.currentSave.GetSpawnID());
                        if (hinge.doorId == GameSave.currentSave.GetSpawnID())
                        {
                            PlayerGlobal.SetSpawnPos(hinge.transform.position, hinge.transform.rotation);
                            PlayerGlobal.instance.SetPosition(hinge.transform.position, false, false);
                        }
                        
                    }
                }
            }*/
        }

        // in progress - just logging  - who called SetPosition?
        [HarmonyPatch(typeof(PlayerGlobal), "SetPosition")]
        [HarmonyPrefix]
        public static bool SetPosition_mypatch(Vector3 pos)
        {
            Log.LogWarning("Position set to: " + pos);
            try
            {
                string callingFuncName = new StackFrame(1).GetMethod().Name;
                string callingFuncType = new StackFrame(1).GetMethod().DeclaringType.ToString();
                for (int i = 1; i < 6; i++)
                {
                    callingFuncName = new StackFrame(i).GetMethod().Name;
                    callingFuncType = new StackFrame(i).GetMethod().DeclaringType.ToString();
                    Log.LogWarning("caller[" + i.ToString() + "]: " + callingFuncType + "." + callingFuncName);
                }
            }
            catch { return true; }
            return true;
        }
    }
}
