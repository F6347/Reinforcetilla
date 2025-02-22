using System;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using GorillaNetworking;
using HarmonyLib;

namespace Reinforcetilla.Patches
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "GetDesiredGameType")]
    class GamemodePatch
    {
        static bool Prefix(GorillaNetworkJoinTrigger __instance, ref string __result)
        {
            HashSet<GameModeType> validGM = GameMode.GameModeZoneMapping.GetModesForZone(__instance.zone, NetworkSystem.Instance.SessionIsPrivate);
            __result = validGM.Contains(Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value.Replace("MODDED_", ""))) ? GorillaComputer.instance.currentGameMode.Value : validGM.First().ToString();
            return false;
        }
    }
}
