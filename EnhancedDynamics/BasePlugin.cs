using System.Collections;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using UnityEngine.SceneManagement;
using MTM101BaldAPI.OptionsAPI;

namespace EnhancedDynamics
{
    [BepInPlugin("imloyness.enhanced.dynamics", "EnhancedDynamics", "28.10")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource logsblablabla = BepInEx.Logging.Logger.CreateLogSource("EnhancedDynamics");

        private void Awake()
        {
            var singletonThing = new GameObject("randomSingleton");
            singletonThing.AddComponent<DefaultCategorySettings>();
            DontDestroyOnLoad(singletonThing);
            new Harmony("imloyness.enhanced.dynamics").PatchAll();

            CustomOptionsCore.OnMenuInitialize += AddCategory;

            logsblablabla.LogInfo("Enhanced Dynamics | v28.10 | by imloyness | hi");
        }
        
        void AddCategory(OptionsMenu __instance, CustomOptionsHandler handler)
        {
            if (Singleton<CoreGameManager>.Instance != null) return;
            handler.AddCategory<CategorySettings>("Enhanced\nDynamics");
        }
    }
}