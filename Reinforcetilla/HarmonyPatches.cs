using System;
using System.Reflection;
using HarmonyLib;
using Reinforcetilla;

namespace Reinforcetilla
{

    public class HarmonyPatches
    {
        private static Harmony instance;

        public static bool IsPatched { get; private set; }
        public const string InstanceId = "com.f6347.gorillatag.reinforcetilla";

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = new Harmony(InstanceId);
                }

                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                instance.UnpatchSelf();
                IsPatched = false;
            }
        }
    }
}
