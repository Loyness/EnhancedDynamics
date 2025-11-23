using UnityEngine;
using HarmonyLib;
using Rewired;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class CameraPatches
    {
        // FOV
        private static float baseFOV => BasePlugin.FOVValue.Value;
        private static float convertedFOV;
        private static float targetFOV;
        private static float currentFOV;
        private static float transitionProgress = 1f;
        private static float startFOV;
        private const float LerpSpeed = 3f;

        // Bobbing
        private static float bobbingAmount => BasePlugin.CameraBobbingIntensity.Value;
        private static float convertedBobAm;
        private const float BobbingSpeed = 5f;
        private const float SprintBobbingMultiplier = 1.5f;
        private static float timer;
        private static Vector3 lastBobOffset = Vector3.zero;

        // Idle Inhale
        private static float idleInhaleTimer;
        private const float IdleInhaleSpeed = 1.5f;
        private const float IdleTransitionSpeed = 8f;
        private static float lastIdleOffset;

        // Toggles
        private static bool toggleFOV => BasePlugin.FOVToggle.Value;
        private static bool toggleCameraBobbing => BasePlugin.CameraBobbingToggle.Value;
        private static bool toggleIdleInhale => BasePlugin.idleinhaleToggle.Value;

        // Cached references
        private static Player rewiredPlayer;
        private static Transform cameraTransform;
        private static Camera camCom;
        private static Camera billboardCam;

        // Reusable vectors to avoid GC
        private static readonly Vector3 ZeroVector = Vector3.zero;
        private static Vector3 rightDirCache;

        [HarmonyPatch(typeof(GameCamera), "Awake")]
        [HarmonyPostfix]
        public static void PostfixAwake(GameCamera __instance)
        {
            camCom = __instance.camCom;
            billboardCam = __instance.billboardCam;
            cameraTransform = __instance.transform;
            camCom.fieldOfView = 60f;
            currentFOV = 60f;
            rewiredPlayer = ReInput.players.GetPlayer(0);
        }

        [HarmonyPatch(typeof(GameCamera), "LateUpdate")]
        [HarmonyPostfix]
        public static void PostfixLateUpdate(GameCamera __instance)
        {
            if (camCom == null || cameraTransform == null) return;

            float dt = Time.deltaTime;
            bool controllable = __instance.Controllable;
            bool frozen = BasePlugin.FrozenState_ED;
            bool slipping = BasePlugin.SlippingState_ED;
            bool hasStamina = BasePlugin.Stamina_ED > 0;

            // fov setup
            convertedFOV = (baseFOV + 1) * 30f;
            convertedBobAm = (bobbingAmount + 1) * 0.25f;

            bool isRunning = rewiredPlayer.GetButton("Run") && hasStamina;
            float newTargetFOV = toggleFOV && isRunning ? convertedFOV * 1.2f : convertedFOV;

            // fov transition
            if (Mathf.Abs(targetFOV - newTargetFOV) > 0.01f && transitionProgress >= 1f)
            {
                startFOV = currentFOV;
                targetFOV = newTargetFOV;
                transitionProgress = 0f;
            }

            if (transitionProgress < 1f)
            {
                transitionProgress = Mathf.Min(transitionProgress + LerpSpeed * dt, 1f);
                float t = Mathf.Sin(transitionProgress * Mathf.PI * 0.5f);
                currentFOV = Mathf.Lerp(startFOV, targetFOV, t);
            }
            else
            {
                currentFOV = newTargetFOV;
            }

            // camera bobbing
            if (toggleCameraBobbing)
            {
                float moveX = rewiredPlayer.GetAxis("MovementX");
                float moveY = rewiredPlayer.GetAxis("MovementY");
                bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f;

                if (isMoving)
                {
                    float speedMult = isRunning ? SprintBobbingMultiplier : 1f;
                    timer += dt * speedMult;

                    float angleRad = cameraTransform.eulerAngles.y * Mathf.Deg2Rad;
                    float cos = Mathf.Cos(angleRad);
                    float sin = Mathf.Sin(angleRad);
                    rightDirCache = new Vector3(cos, 0f, sin);

                    float horizontalBob = Mathf.Sin(timer * BobbingSpeed) * convertedBobAm;
                    float verticalBob = Mathf.Cos(timer * BobbingSpeed * 2f) * convertedBobAm * 0.5f;

                    Vector3 targetBob = rightDirCache * horizontalBob + new Vector3(0f, verticalBob, 0f);
                    lastBobOffset = Vector3.Lerp(lastBobOffset, targetBob, dt * 10f);
                }
                else
                {
                    if (lastBobOffset.sqrMagnitude > 0.0001f)
                    {
                        lastBobOffset = Vector3.Lerp(lastBobOffset, ZeroVector, dt * 5f);
                    }

                    if (toggleIdleInhale)
                    {
                        idleInhaleTimer += dt;
                        float targetIdle = Mathf.Sin(idleInhaleTimer * IdleInhaleSpeed) * (convertedBobAm * 0.2f);
                        lastIdleOffset = Mathf.Lerp(lastIdleOffset, targetIdle, dt * IdleTransitionSpeed);
                    }
                    else
                    {
                        lastIdleOffset = Mathf.Lerp(lastIdleOffset, 0f, dt * IdleTransitionSpeed);
                    }
                }

                Vector3 totalOffset = lastBobOffset + new Vector3(0f, lastIdleOffset, 0f);
                cameraTransform.position += totalOffset;
            }
            else
            {
                ResetOffsets(dt);
            }
            currentFOV = ApplyFOV(currentFOV);
        }

        private static void ResetOffsets(float dt)
        {
            lastBobOffset = Vector3.Lerp(lastBobOffset, ZeroVector, dt * 5f);
            lastIdleOffset = Mathf.Lerp(lastIdleOffset, 0f, dt * 5f);
            cameraTransform.position += lastBobOffset + new Vector3(0f, lastIdleOffset, 0f);
        }

        private static float ApplyFOV(float fov)
        {
            camCom.fieldOfView = fov;
            if (billboardCam != null)
                billboardCam.fieldOfView = fov;
            return fov;
        }
    }
}