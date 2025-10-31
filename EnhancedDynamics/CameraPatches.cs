using UnityEngine;
using HarmonyLib;
using EnhancedDynamics;
//using MTM101BaldAPI.OptionsAPI;
using Rewired;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class CameraPatches
    {
        // FOV parameters
        private static float baseFOV => BasePlugin.FOVValue.Value;
        private static float convertedFOV;
        private static float targetFOV;
        private static float UpdateFOV;
        private static float transitionProgress = 1f;
        private static float startFOV;
        private static float lerpSpeed = 3f;

        // Camera bobbing parameters
        private static float bobbingSpeed = 5f;
        private static float convertedBobAm;
        private static float bobbingAmount = BasePlugin.CameraBobbingIntensity.Value;
        private static float sprintBobbingMultiplier = 1.5f;
        private static float timer = 0f;
        private static Vector3 lastBobOffset = Vector3.zero;

        //toggles
        private static bool toggleFOV => BasePlugin.FOVToggle.Value;
        private static bool toggleCameraBobbing => BasePlugin.CameraBobbingToggle.Value;

        private static Vector3 CalculateBobOffset(float cameraYRotation)
        {
            float horizontalBob = Mathf.Sin(timer * bobbingSpeed) * convertedBobAm;
            float verticalBob = Mathf.Cos(timer * bobbingSpeed * 2f) * convertedBobAm * 0.5f;
            
            float angleRad = cameraYRotation * Mathf.Deg2Rad;
            
            float rotatedX = horizontalBob * Mathf.Cos(angleRad);
            float rotatedZ = horizontalBob * Mathf.Sin(angleRad);
            
            return new Vector3(rotatedX, verticalBob, rotatedZ);
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
            float newTargetFOV = toggleFOV && isRunning ? convertedFOV * 1.2f : convertedFOV;

            if (transitionProgress >= 1f)
            {
                // Check if we need to start a new transition
                if (Mathf.Abs(targetFOV - newTargetFOV) > 0.01f)
                {
                    startFOV = __instance.camCom.fieldOfView;
                    targetFOV = newTargetFOV;
                    transitionProgress = 0f;
                }
            }

            // Camera Bobbing Logic
            if (toggleCameraBobbing)
            {
                // ReInput cooking
                float moveX = ReInput.players.GetPlayer(0).GetAxis("MovementX");
                float moveY = ReInput.players.GetPlayer(0).GetAxis("MovementY");
                bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f;

                if (isMoving)
                {
                    timer += Time.deltaTime * (isRunning ? sprintBobbingMultiplier : 1f);
                    float cameraYRotation = __instance.transform.eulerAngles.y;
                    Vector3 targetBobOffset = CalculateBobOffset(cameraYRotation);
                    lastBobOffset = Vector3.Lerp(lastBobOffset, targetBobOffset, Time.deltaTime * 10f);
                    __instance.transform.position += lastBobOffset;
                }
                else if (lastBobOffset.magnitude > 0.001f)
                {
                    lastBobOffset = Vector3.Lerp(lastBobOffset, Vector3.zero, Time.deltaTime * 5f);
                    __instance.transform.position += lastBobOffset;
                }
            }
            
            //more fov logic
            if (transitionProgress < 1f)
            {
                transitionProgress = Mathf.Min(transitionProgress + lerpSpeed * Time.deltaTime, 1f);
                float smoothProgress = Mathf.Sin(transitionProgress * Mathf.PI * 0.5f);
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