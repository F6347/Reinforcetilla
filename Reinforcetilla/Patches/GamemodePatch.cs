using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Windows;

namespace Reinforcetilla.Patches
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "GetDesiredGameType")]
    class GamemodePatch
    {
        static bool Prefix(GorillaNetworkJoinTrigger __instance, ref string __result)
        {
            __result = __instance.forceGameType == "" ? GorillaComputer.instance.currentGameMode.Value : char.ToUpper(__instance.forceGameType[0]) + __instance.forceGameType.Substring(1).ToLower();
            return false;
        }
    }
}
