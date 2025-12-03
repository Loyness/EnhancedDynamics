using HarmonyLib;
using UnityEngine;

namespace EnhancedDynamics
{
    [HarmonyPatch]
    public static class VerticalCameraFeature
    {
        public static float transitionProgress = 0f;
        private static float verticalRotation;
        private const float MAX_VERTICAL_ANGLE = 89f;
        private const float MIN_VERTICAL_ANGLE = -89f;

        [HarmonyPatch(typeof(PlayerMovement), "MouseMove")]
        [HarmonyPostfix]
        private static void PostfixPlayerMovementMouseMove(PlayerMovement __instance)
        {
            bool isSpacePressed = Singleton<InputManager>.Instance.GetDigitalInput("LookBack", false);
            if (BasePlugin.VerticalCameraToggle.Value == true)
            {
                //da calculations and logic
                int num = __instance.pm.reversed ? -1 : 1;
                
                bool flag = !Singleton<PlayerFileManager>.Instance.authenticMode;
                Vector2 vector;
                Vector2 vector2;
                if (flag)
                {
                    Singleton<InputManager>.Instance.GetAnalogInput(__instance.cameraAnalogData, out vector, out vector2, 0.1f);
                }
                else
                {
                    Singleton<InputManager>.Instance.GetAnalogInput(__instance.movementAnalogData, out vector, out vector2, 0.1f);
                    vector2.x = 0f;
                    vector2.y = 0f;
                }
                float mouseCameraSensitivity = Singleton<PlayerFileManager>.Instance.mouseCameraSensitivity;
                float controllerCameraSensitivity = Singleton<PlayerFileManager>.Instance.controllerCameraSensitivity;
                float num2 = vector2.y * mouseCameraSensitivity * num + vector.y * Time.deltaTime * controllerCameraSensitivity * num;
                num2 *= Time.timeScale * __instance.pm.PlayerTimeScale;
                verticalRotation -= num2;
                verticalRotation = Mathf.Clamp(verticalRotation, MIN_VERTICAL_ANGLE, MAX_VERTICAL_ANGLE);
            }
            else
            {
                //meant to reset vertical rotation when toggled off **smoothly** but is not
                transitionProgress = Mathf.Min(transitionProgress + 1 * Time.deltaTime, 1f);
                float smoothProgress = Mathf.Sin(transitionProgress * Mathf.PI * 0.5f);
                verticalRotation = Mathf.Lerp(verticalRotation, 0f, smoothProgress);
            }

            //the whole system
            Transform cameraBase = __instance.pm.cameraBase;
            bool flag2 = cameraBase != null;
            if (flag2)
            {
                Vector3 localEulerAngles = cameraBase.localEulerAngles;
                float finalVerticalRotation = isSpacePressed ? -verticalRotation : verticalRotation;
                localEulerAngles.x = finalVerticalRotation;
                cameraBase.localEulerAngles = localEulerAngles;
            }
        }
    }
}