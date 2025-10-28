using UnityEngine;
using HarmonyLib;
using EnhancedDynamics;
using MTM101BaldAPI.OptionsAPI;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class CameraPatches
    {
        // FOV parameters
        private static float baseFOV => Singleton<DefaultCategorySettings>.Instance.fovValue;
        private static float convertedFOV;
        private static float targetFOV;
        private static float UpdateFOV;
        private static float transitionProgress = 1f;
        private static float startFOV;
        private static float lerpSpeed = 3f;

        // Camera bobbing parameters
        private static float bobbingSpeed = 5f;
        private static float convertedBobAm;
        private static float bobbingAmount = Singleton<DefaultCategorySettings>.Instance.cameraBobbingIntensity;
        private static float sprintBobbingMultiplier = 1.5f;
        private static float timer = 0f;
        private static Vector3 lastBobOffset = Vector3.zero;

        //toggles
        private static bool toggleFOV => Singleton<DefaultCategorySettings>.Instance.fovToggle;
        private static bool toggleCameraBobbing => Singleton<DefaultCategorySettings>.Instance.cameraBobbingToggle;

        private static Vector3 CalculateBobOffset()
        {
            float horizontalBob = Mathf.Sin(timer * bobbingSpeed) * convertedBobAm;
            float verticalBob = Mathf.Cos(timer * bobbingSpeed * 2f) * convertedBobAm;
            Vector3 result = new Vector3(horizontalBob, verticalBob * 0.5f, 0f);
            return result;
        }

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
            convertedBobAm = (bobbingAmount + 1) * 0.25f;

            // FOV Transition Logic
            bool isRunning = Singleton<InputManager>.Instance.GetDigitalInput("Run", false);
            if (toggleFOV && isRunning)
            {
                targetFOV = convertedFOV * 1.2f;
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

            // Camera Bobbing Logic
            if (toggleCameraBobbing)
            {
                bool isMoving = 
                    Singleton<InputManager>.Instance.GetDigitalInput("MovementX", false) ||
                    Singleton<InputManager>.Instance.GetDigitalInput("MovementY", false);

                if (isMoving)
                {
                    timer += Time.deltaTime * (isRunning ? sprintBobbingMultiplier : 1f);
                    Vector3 targetBobOffset = CalculateBobOffset();
                    lastBobOffset = Vector3.Lerp(lastBobOffset, targetBobOffset, Time.deltaTime * 10f);
                    __instance.transform.position += lastBobOffset;
                }
                else
                {
                    lastBobOffset = Vector3.Lerp(lastBobOffset, Vector3.zero, Time.deltaTime * 5f);
                    __instance.transform.position += lastBobOffset;
                }
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

            //BasePlugin.logsblablabla.LogInfo("Enhanced Dynamics | Camera Bobbing Toggle: " + toggleCameraBobbing.ToString());
            //BasePlugin.logsblablabla.LogInfo("Enhanced Dynamics | Bobbing Amount: " + bobbingAmount.ToString());


            __instance.camCom.fieldOfView = UpdateFOV;
            __instance.billboardCam.fieldOfView = UpdateFOV;
        }
    }
}