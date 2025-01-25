using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using HarmonyLib;

namespace Reinforcetilla.Patches
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "GetDesiredGameType")]
    class GamemodePatch
    {
        static bool Prefix(ref string __result)
        {
            __result = GorillaComputer.instance.currentGameMode.Value;
            return false;
        }
    }
}
