using HarmonyLib;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class PlayerMovementPatches
    {
        [HarmonyPatch(typeof(PlayerMovement), "Update")]
        [HarmonyPostfix]
        public static void PostfixPlayerMovementUpdate(PlayerMovement __instance)
        {
            BasePlugin.Velocity_ED = AccessTools.FieldRefAccess<PlayerMovement, float>(__instance, "frameVelocity") * 10f;
            BasePlugin.Stamina_ED = __instance.stamina;
        }
    }
}