using HarmonyLib;
using EnhancedDynamics;
using MTM101BaldAPI.Reflection;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class NanaPeelPatches
    {
        [HarmonyPatch(typeof(ITM_NanaPeel), "Update")]
        [HarmonyPostfix]
        public static void PostfixNanaPeelUpdate(ITM_NanaPeel __instance)
        {
            BasePlugin.SlippingState_ED = AccessTools.FieldRefAccess<ITM_NanaPeel, bool>(__instance, "slipping");
        }
    }
}