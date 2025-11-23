using HarmonyLib;
using BepInEx;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class NanaPeelPatches
    {
        [HarmonyPatch(typeof(ITM_NanaPeel), "Update")]
        [HarmonyPostfix]
        public static void PostfixNanaPeelUpdate(ITM_NanaPeel __instance)
        {
            BasePlugin.SlippingState_ED = __instance.slipping;
        }
    }
}