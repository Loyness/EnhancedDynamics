using UnityEngine;
using HarmonyLib;
using EnhancedDynamics;
using MTM101BaldAPI.OptionsAPI;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class FOVControllerPatches
    {
        private static float baseFOV => Singleton<DefaultCategorySettings>.Instance.fovValue;
        private static float convertedFOV;
        private static float targetFOV;
        private static float UpdateFOV;
        private static float transitionProgress = 1f;
        private static float startFOV;
        private static float lerpSpeed = 3f;
        private static bool toggleFOV => Singleton<DefaultCategorySettings>.Instance.fovToggle;

        [HarmonyPatch(typeof(GameCamera), "Awake")]
        [HarmonyPostfix]
        public static void PostfixAwake(GameCamera __instance)
        {
            __instance.camCom.fieldOfView = 60f; // default
        }

        [HarmonyPatch(typeof(GameCamera), "LateUpdate")]
        [HarmonyPostfix]
        public static void Postfix0(GameCamera __instance)
        {
            convertedFOV = (baseFOV + 1) * 30f;

            if (toggleFOV)
            {
                if (Singleton<InputManager>.Instance.GetDigitalInput("Run", false))
                {
                    targetFOV = convertedFOV * 1.2f;
                }
            }
            else
            {
                targetFOV = convertedFOV;
            }

            if (transitionProgress == 1f)
            {
                startFOV = __instance.camCom.fieldOfView;
                transitionProgress = 0f;
                targetFOV = convertedFOV;
            }

            if (transitionProgress < 1f)
            {
                transitionProgress = Mathf.Min(transitionProgress + lerpSpeed * Time.deltaTime, 1f);
                float smoothProgress = (1f - Mathf.Cos(transitionProgress * Mathf.PI)) * 0.5f;
                UpdateFOV = Mathf.Lerp(startFOV, targetFOV, smoothProgress);
            }

            //debug
            //BasePlugin.logsblablabla.LogInfo("Enhanced Dynamics | Sprint Hold: " + Singleton<InputManager>.Instance.GetDigitalInput("Run", false).ToString());
            //BasePlugin.logsblablabla.LogInfo("Enhanced Dynamics | FOV Toggle: " + toggleFOV.ToString());
            //BasePlugin.logsblablabla.LogInfo("Enhanced Dynamics | FOV: " + UpdateFOV.ToString());

            __instance.camCom.fieldOfView = UpdateFOV;
            __instance.billboardCam.fieldOfView = UpdateFOV;
        }
    }
}