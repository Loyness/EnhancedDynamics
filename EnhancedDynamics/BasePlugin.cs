using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Bootstrap;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using UnityEngine;

namespace EnhancedDynamics
{
    [BepInPlugin("imloyness.enhanced.dynamics", "EnhancedDynamics", "02.11")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource logsblablabla = BepInEx.Logging.Logger.CreateLogSource("EnhancedDynamics");

        public static ConfigEntry<bool> FOVToggle;
        public static ConfigEntry<int> FOVValue;
        public static ConfigEntry<bool> CameraBobbingToggle;
        public static ConfigEntry<int> CameraBobbingIntensity;

        public static bool FrozenState_ED;
        public static bool SlippingState_ED;

        private void Awake()
        {
            // configs
            FOVToggle = Config.Bind("Camera", "FOV Toggle", true, "Enable/Disable FOV changes");
            FOVValue = Config.Bind("Camera", "FOV Value", 1, "Set the FOV multiplier (1 = 30 FOV, 2 = 60 FOV, 3 = 90 FOV, 4 = 120 FOV)");
            CameraBobbingToggle = Config.Bind("Camera", "Camera Bobbing Toggle", true, "Enable/Disable Camera Bobbing");
            CameraBobbingIntensity = Config.Bind("Camera", "Camera Bobbing Intensity", 1, "Set the Camera Bobbing Intensity");
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return new WaitForSeconds(0.3f);

            new Harmony("imloyness.enhanced.dynamics").PatchAll();

            if (Chainloader.PluginInfos.ContainsKey("mtm101.rulerp.bbplus.baldidevapi"))
            {
                logsblablabla.LogInfo("Enhanced Dynamics | MTM101BaldAPI detected, adding OptionsAPI integration.");
                MTMBald101APIIntegration.TryIntegrate();
            }
            logsblablabla.LogInfo("Enhanced Dynamics | v02.11 | by imloyness | hi");
        }
    }
}