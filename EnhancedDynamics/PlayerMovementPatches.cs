using HarmonyLib;
using UnityEngine;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class PlayerMovementPatches
    {
        [HarmonyPatch(typeof(PlayerMovement), "Update")]
        [HarmonyPostfix]
        public static void PostfixPlayerMovementUpdate(PlayerMovement __instance)
        {
            BasePlugin.Velocity_ED = __instance.frameVelocity * 10f;
            BasePlugin.Stamina_ED = __instance.stamina;
        }

    }
}