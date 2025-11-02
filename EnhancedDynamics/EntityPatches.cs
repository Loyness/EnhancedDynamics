using HarmonyLib;

namespace EnhancedDynamics
{
    public static class EntityPatches
    {
        [HarmonyPatch(typeof(Entity), "Update")]
        [HarmonyPostfix]
        public static void GetVariablePostFix(Entity __instance)
        {
            BasePlugin.FrozenState_ED = __instance.Frozen;
        }
    }
}