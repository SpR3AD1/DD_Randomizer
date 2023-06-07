using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DD_Randomizer
{
    public static class Fixes
    {
        // Patching two way locks
        [HarmonyPatch(typeof(ButtonPromptArea), "Start")]
        [HarmonyPostfix]
        public static void Lock_Triggerarea_MyPatch(ButtonPromptArea __instance)
        {
            if (DD_Randomizer.Settings["twoWayLocks"].toggleState.Value)
            {
                if (__instance.prompt == "prompt_unlock")
                {
                    __instance.transform.localPosition = new Vector3(0, -1, 0);
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
    }
}
