using UnityEngine;
using HarmonyLib;
using Rewired;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class CameraPatches
    {
        // FOV parameters
        public static float baseFOV => BasePlugin.FOVValue.Value;
        public static float convertedFOV;
        public static float targetFOV;
        public static float UpdateFOV;
        public static float transitionProgress = 1f;
        public static float startFOV;
        private static float lerpSpeed = 3f;

        // Camera bobbing parameters
        public static float bobbingSpeed = 5f;
        public static float bobbingSpeedConverted;
        public static float convertedBobAm;
        public static float bobbingAmount = BasePlugin.CameraBobbingIntensity.Value;
        public static float sprintBobbingMultiplier = 1.5f;
        public static float timer = 0f;
        public static Vector3 lastBobOffset = Vector3.zero;

        // Idle inhale parameters (separate for continuous animation)
        public static float idleInhaleTimer = 0f;
        public static float idleInhaleSpeed = 1.5f; // Slower for breathing-like effect, adjust as needed
        public static float lastIdleOffset = 0f;
        public static float idleTransitionSpeed = 8f; // Smooth transition in/out

        //toggles
        public static bool toggleFOV => BasePlugin.FOVToggle.Value;
        public static bool toggleCameraBobbing => BasePlugin.CameraBobbingToggle.Value;
        public static bool toggleIdleInhale => BasePlugin.idleinhaleToggle.Value;

        public static Vector3 CalculateBobOffset(float cameraYRotation)
        {
            float horizontalBob = Mathf.Sin(timer * bobbingSpeedConverted) * convertedBobAm;
            float verticalBob = Mathf.Cos(timer * bobbingSpeedConverted * 2f) * convertedBobAm * 0.5f;

            float angleRad = cameraYRotation * Mathf.Deg2Rad;

            float rotatedX = horizontalBob * Mathf.Cos(angleRad);
            float rotatedZ = horizontalBob * Mathf.Sin(angleRad);

            return new Vector3(rotatedX, verticalBob, rotatedZ);
        }
        
        public static float CalculateIdleInhale()
        {
            // Continuous sine wave for proper breathing animation
            return Mathf.Sin(idleInhaleTimer * idleInhaleSpeed) * (convertedBobAm * 0.2f);
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
            float newTargetFOV = toggleFOV && isRunning && __instance.Controllable && !BasePlugin.FrozenState_ED && !BasePlugin.SlippingState_ED && BasePlugin.Stamina_ED > 0 ? convertedFOV * 1.2f : convertedFOV;

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
            if (toggleCameraBobbing && __instance.Controllable && !BasePlugin.FrozenState_ED && !BasePlugin.SlippingState_ED)
            {
                // ReInput cooking
                float moveX = ReInput.players.GetPlayer(0).GetAxis("MovementX");
                float moveY = ReInput.players.GetPlayer(0).GetAxis("MovementY");
                bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f;

                if (isMoving)
                {
                    bobbingSpeedConverted = bobbingSpeed; // if i ever find out how do i grab velocity normally
                    timer += Time.deltaTime * (isRunning && BasePlugin.Stamina_ED > 0 ? sprintBobbingMultiplier : 1f);
                    float cameraYRotation = __instance.transform.eulerAngles.y;
                    Vector3 targetBobOffset = CalculateBobOffset(cameraYRotation);
                    lastBobOffset = Vector3.Lerp(lastBobOffset, targetBobOffset, Time.deltaTime * 10f);
                    __instance.transform.position += lastBobOffset;
                }
                else
                {
                    if (lastBobOffset.magnitude > 0.001f)
                    {
                        lastBobOffset = Vector3.Lerp(lastBobOffset, Vector3.zero, Time.deltaTime * 5f);
                        __instance.transform.position += lastBobOffset;
                    }

                    if (toggleIdleInhale)
                    {
                        idleInhaleTimer += Time.deltaTime;
                        float targetIdleOffset = CalculateIdleInhale();
                        lastIdleOffset = Mathf.Lerp(lastIdleOffset, targetIdleOffset, Time.deltaTime * idleTransitionSpeed);
                        __instance.transform.position += new Vector3(0f, lastIdleOffset, 0f);
                    }
                    else if (lastIdleOffset != 0f)
                    {
                        lastIdleOffset = Mathf.Lerp(lastIdleOffset, 0f, Time.deltaTime * idleTransitionSpeed);
                        __instance.transform.position += new Vector3(0f, lastIdleOffset, 0f);
                    }
                }
            }
            else
            {
                // Reset all offsets if bobbing disabled or uncontrollable
                lastBobOffset = Vector3.Lerp(lastBobOffset, Vector3.zero, Time.deltaTime * 5f);
                lastIdleOffset = Mathf.Lerp(lastIdleOffset, 0f, Time.deltaTime * 5f);
                __instance.transform.position += lastBobOffset + new Vector3(0f, lastIdleOffset, 0f);
            }

            //more fov logic
            if (transitionProgress < 1f)
            {
                transitionProgress = Mathf.Min(transitionProgress + lerpSpeed * Time.deltaTime, 1f);
                float smoothProgress = Mathf.Sin(transitionProgress * Mathf.PI * 0.5f);
                UpdateFOV = Mathf.Lerp(startFOV, targetFOV, smoothProgress);
            }

            __instance.camCom.fieldOfView = UpdateFOV;
            __instance.billboardCam.fieldOfView = UpdateFOV;
        }
    }
}