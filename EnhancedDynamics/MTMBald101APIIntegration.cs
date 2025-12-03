using BepInEx;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;

namespace EnhancedDynamics
{
    internal static class MTMBald101APIIntegration
    {
        public static void TryIntegrate()
        {
            var singletonThing = new GameObject("randomSingleton");
            singletonThing.AddComponent<DefaultCategorySettings>();

            CustomOptionsCore.OnMenuInitialize += AddCategory;
        }

        static void AddCategory(OptionsMenu __instance, CustomOptionsHandler handler)
        {
            handler.AddCategory<CategorySettings>("Enhanced\nDynamics");
        }
    }
}