using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Bootstrap;
using System.Collections;
using UnityEngine;

namespace EnhancedDynamics
{
    // do not forget to change version here
    [BepInPlugin("imloyness.enhanced.dynamics", "EnhancedDynamics", "1.01")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource logsblablabla = BepInEx.Logging.Logger.CreateLogSource("EnhancedDynamics");

        //configs
        public static ConfigEntry<int> FOVValue;
        public static ConfigEntry<int> CameraBobbingIntensity;

        public static ConfigEntry<bool> FOVToggle;
        public static ConfigEntry<bool> CameraBobbingToggle;
        public static ConfigEntry<bool> idleinhaleToggle;
        public static ConfigEntry<bool> VerticalCameraToggle;

        
        //shared variables
        public static bool FrozenState_ED;
        public static bool SlippingState_ED;
        public static float Velocity_ED;
        public static float Stamina_ED;

        private void Awake()
        {
            // config setup
            FOVValue = Config.Bind("Camera", "FOV Value", 1, "Set the FOV multiplier (1 = 30 FOV, 2 = 60 FOV, 3 = 90 FOV, 4 = 120 FOV)");
            CameraBobbingIntensity = Config.Bind("Camera", "Camera Bobbing Intensity", 2, "Set the Camera Bobbing Intensity");

            FOVToggle = Config.Bind("Camera", "FOV Toggle", true, "Enable/Disable FOV changes");
            CameraBobbingToggle = Config.Bind("Camera", "Camera Bobbing Toggle", true, "Enable/Disable Camera Bobbing");
            idleinhaleToggle = Config.Bind("Camera", "Idle Inhale Toggle", true, "Enable/Disable Idle Inhale Animation (Requires Camera Bobbing to be enabled.)");
            VerticalCameraToggle = Config.Bind("Camera", "Vertical Camera Toggle", true, "Enable/Disable Vertical Camera");
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return new WaitForSeconds(0.3f);

            var harmony = new Harmony("imloyness.enhanced.dynamics");

            try
            {
                harmony.PatchAll();
            }
            catch (System.Exception e)
            {
                logsblablabla.LogWarning("Enhanced Dynamics | Harmony patching failed: " + e.Message + ". Skipping patches.");
                harmony.PatchAll(typeof(CameraPatches));
            }

            if (Chainloader.PluginInfos.ContainsKey("mtm101.rulerp.bbplus.baldidevapi"))
            {
                logsblablabla.LogInfo("Enhanced Dynamics | MTM101BaldAPI detected, adding OptionsAPI integration.");
                MTMBald101APIIntegration.TryIntegrate();
            }

            logsblablabla.LogInfo("Enhanced Dynamics | v1.01 | by imloyness | Loaded.");
        }
    }
}