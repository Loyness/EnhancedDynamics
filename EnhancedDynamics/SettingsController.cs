using MTM101BaldAPI.OptionsAPI;
using TMPro;
using UnityEngine;

namespace EnhancedDynamics
{
    public class DefaultCategorySettings : Singleton<DefaultCategorySettings>
    {
        public bool fovToggle = false;
        public int fovValue = 1;
        public bool cameraBobbingToggle = true;
        public int cameraBobbingIntensity = 1;
    }

    public class CategorySettings : CustomOptionsCategory
    {
        MenuToggle FOVToggle;
        AdjustmentBars FOVValue;
        MenuToggle CameraBobbing;
        AdjustmentBars CameraBobbingIntensity;

        public override void Build()
        {
            FOVToggle = CreateNewToggle("fovToggle", "FOV Transitions", "Enables FOV transitions when running, making the game feels more intensive", 1);
            FOVValue = CreateNewBar("fovValue", "Base FOV", "Base field of view value (default: 60)", 2, 3, Color.black);

            CameraBobbing = CreateNewToggle("cameraBobbingToggle", "Camera Bobbing", "Enables camera bobbing when walking/running", 3);
            CameraBobbingIntensity = CreateNewBar("cameraBobbingIntensity", "Bobbing Intensity", "Adjusts the intensity of the camera bobbing effect", 4, 3, Color.black);
            CreateApplyButton(() => SendData());

            FOVToggle.Set(DefaultCategorySettings.Instance.fovToggle);
            FOVValue.Adjust(DefaultCategorySettings.Instance.fovValue);
            CameraBobbing.Set(DefaultCategorySettings.Instance.cameraBobbingToggle);
            CameraBobbingIntensity.Adjust(DefaultCategorySettings.Instance.cameraBobbingIntensity);
        }

        public void SendData()
        {
            DefaultCategorySettings.Instance.fovToggle = FOVToggle.Value;
            DefaultCategorySettings.Instance.fovValue = FOVValue.GetRaw();
            DefaultCategorySettings.Instance.cameraBobbingToggle = CameraBobbing.Value;
            DefaultCategorySettings.Instance.cameraBobbingIntensity = CameraBobbingIntensity.GetRaw();
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