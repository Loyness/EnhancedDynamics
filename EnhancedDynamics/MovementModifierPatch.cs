using HarmonyLib;

namespace EnhancedDynamics
{
    public static class MovementModifierPatch
    {
        [HarmonyPatch(typeof(MovementModifier), "Update")]
        [HarmonyPostfix]
        public static void PostfixMovementModifierUpdate(MovementModifier __instance)
        {
            BasePlugin.MovementMultiplier_ED = __instance.movementMultiplier;
        }
    }
}