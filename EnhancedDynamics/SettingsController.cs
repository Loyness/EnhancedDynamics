using MTM101BaldAPI.OptionsAPI;
using TMPro;
using UnityEngine;

namespace EnhancedDynamics
{
    public class DefaultCategorySettings : Singleton<DefaultCategorySettings>
    {
        public bool fovToggle = BasePlugin.FOVToggle.Value;
        public int fovValue = BasePlugin.FOVValue.Value;
        public bool cameraBobbingToggle = BasePlugin.CameraBobbingToggle.Value;
        public int cameraBobbingIntensity = BasePlugin.CameraBobbingIntensity.Value;
        public bool idleInhaleToggle = BasePlugin.idleinhaleToggle.Value;
    }

    public class CategorySettings : CustomOptionsCategory
    {
        MenuToggle FOVToggle;
        AdjustmentBars FOVValue;
        MenuToggle CameraBobbing;
        AdjustmentBars CameraBobbingIntensity;
        MenuToggle IdleInhale;

        public override void Build()
        {
            FOVToggle = CreateNewToggle("fovToggle", "FOV Transitions", "Enables FOV transitions when running, making the game feels more intensive", 1);
            CameraBobbing = CreateNewToggle("cameraBobbingToggle", "Camera Bobbing", "Enables camera bobbing when walking/running", 3);
            FOVValue = CreateNewBar("fovValue", "Base FOV", "Base field of view value (default: 60)", 2, 3, Color.black);
            CameraBobbingIntensity = CreateNewBar("cameraBobbingIntensity", "Bobbing Intensity", "Adjusts the intensity of the camera bobbing effect", 4, 3, Color.black);
            IdleInhale = CreateNewToggle("idleInhaleToggle", "Idle Inhale Animation", "Enables a subtle camera bobbing effect when idle to simulate breathing", 5);
            CreateApplyButton(() => SendData());
            
            // Load saved data
            FOVToggle.Set(DefaultCategorySettings.Instance.fovToggle);
            CameraBobbing.Set(DefaultCategorySettings.Instance.cameraBobbingToggle);
            IdleInhale.Set(DefaultCategorySettings.Instance.idleInhaleToggle);
            FOVValue.Adjust(DefaultCategorySettings.Instance.fovValue);
            CameraBobbingIntensity.Adjust(DefaultCategorySettings.Instance.cameraBobbingIntensity);
        }

        public void SendData()
        {
            DefaultCategorySettings.Instance.fovToggle = FOVToggle.Value;
            DefaultCategorySettings.Instance.cameraBobbingToggle = CameraBobbing.Value;
            DefaultCategorySettings.Instance.idleInhaleToggle = IdleInhale.Value;
            DefaultCategorySettings.Instance.fovValue = FOVValue.GetRaw();
            DefaultCategorySettings.Instance.cameraBobbingIntensity = CameraBobbingIntensity.GetRaw();

            BasePlugin.FOVToggle.Value = FOVToggle.Value;
            BasePlugin.CameraBobbingToggle.Value = CameraBobbing.Value;
            BasePlugin.idleinhaleToggle.Value = IdleInhale.Value;
            BasePlugin.FOVValue.Value = FOVValue.GetRaw();
            BasePlugin.CameraBobbingIntensity.Value = CameraBobbingIntensity.GetRaw();
        }

        public AdjustmentBars CreateNewBar(string id, string name,string tooltipKey,int order, int length, Color textColor) {
            Vector3 originVec = new Vector3(40f,85f,0f);
            var bar = CreateBars(() => {
                SendData();
            },id, originVec + new Vector3(-30,order*-29.5f,0), length);
            CreateText(id + "_text", name, originVec + new Vector3(-125,order*-28,0),MTM101BaldAPI.UI.BaldiFonts.ComicSans24,TextAlignmentOptions.Left, new Vector2(250,10),textColor,false);
            AddTooltip(bar,tooltipKey);
            
            return bar;
        }

        public MenuToggle CreateNewToggle(string id, string name, string tooltipKey, int order) {
            Vector3 originVec = new Vector3(40f, 85f, 0f);
            var toggle = CreateToggle(id, name, false, originVec + new Vector3(0, order * -29.5f, 0), 250f);
            AddTooltip(toggle, tooltipKey);

            return toggle;
        }
    }
}