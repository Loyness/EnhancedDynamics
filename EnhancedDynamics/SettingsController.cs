using MTM101BaldAPI.OptionsAPI;
using TMPro;
using UnityEngine;

namespace EnhancedDynamics
{
    public class DefaultCategorySettings : Singleton<DefaultCategorySettings>
    {
        public bool fovToggle;
        public bool cameraBobbingToggle;
        public bool idleInhaleToggle;
        public bool verticalCameraToggle;
        public int fovValue;
        public int cameraBobbingIntensity;

        public void RefreshFromConfig()
        {
            fovToggle = BasePlugin.FOVToggle.Value;
            cameraBobbingToggle = BasePlugin.CameraBobbingToggle.Value;
            idleInhaleToggle = BasePlugin.idleinhaleToggle.Value;
            verticalCameraToggle = BasePlugin.VerticalCameraToggle.Value;
            fovValue = BasePlugin.FOVValue.Value;
            cameraBobbingIntensity = BasePlugin.CameraBobbingIntensity.Value;
        }
    }

    public class CategorySettings : CustomOptionsCategory
    {
        MenuToggle FOVToggle;
        AdjustmentBars FOVValue;
        MenuToggle CameraBobbing;
        AdjustmentBars CameraBobbingIntensity;
        MenuToggle IdleInhale;
        MenuToggle VerticalCamera;

        public override void Build()
        {
            FOVValue = CreateNewBar("fovValue", "Base FOV", "Base field of view value (default: 60)", 1, 3, Color.black);
            CameraBobbingIntensity = CreateNewBar("cameraBobbingIntensity", "Bobbing Intensity", "Adjusts the intensity of the camera bobbing effect", 2, 3, Color.black);

            FOVToggle = CreateNewToggle("fovToggle", "FOV Transitions", "Enables FOV transitions when running, making the game feels more intensive", 4);
            CameraBobbing = CreateNewToggle("cameraBobbingToggle", "Camera Bobbing", "Enables camera bobbing when walking/running", 5);
            IdleInhale = CreateNewToggle("idleInhaleToggle", "Idle Inhale Animation", "Enables a subtle camera bobbing effect when idle to simulate breathing", 6);
            VerticalCamera = CreateNewToggle("verticalCameraToggle", "Vertical Camera", "Your neck is finally gonna be free :fire:", 7);

            DefaultCategorySettings.Instance.RefreshFromConfig();
            
            FOVToggle.Set(DefaultCategorySettings.Instance.fovToggle);
            CameraBobbing.Set(DefaultCategorySettings.Instance.cameraBobbingToggle);
            IdleInhale.Set(DefaultCategorySettings.Instance.idleInhaleToggle);
            VerticalCamera.Set(DefaultCategorySettings.Instance.verticalCameraToggle);
            FOVValue.Adjust(DefaultCategorySettings.Instance.fovValue);
            CameraBobbingIntensity.Adjust(DefaultCategorySettings.Instance.cameraBobbingIntensity);
        }

        public void SendData()
        {
            BasePlugin.FOVToggle.Value = FOVToggle.Value;
            BasePlugin.CameraBobbingToggle.Value = CameraBobbing.Value;
            BasePlugin.idleinhaleToggle.Value = IdleInhale.Value;
            BasePlugin.VerticalCameraToggle.Value = VerticalCamera.Value;
            BasePlugin.FOVValue.Value = FOVValue.GetRaw();
            BasePlugin.CameraBobbingIntensity.Value = CameraBobbingIntensity.GetRaw();
        }

        public AdjustmentBars CreateNewBar(string id, string name,string tooltipKey,int order, int length, Color textColor) {
            Vector3 originVec = new Vector3(90f,70f,0f);
            var bar = CreateBars(() => {
                SendData();
            },id, originVec + new Vector3(-30,order*-29.5f,0), length);
            CreateText(id + "_text", name, originVec + new Vector3(-175,order*-28,0),MTM101BaldAPI.UI.BaldiFonts.ComicSans24,TextAlignmentOptions.Right, new Vector2(250,10),textColor,false);
            AddTooltip(bar,tooltipKey);
            
            return bar;
        }

        public MenuToggle CreateNewToggle(string id, string name, string tooltipKey, int order) {
            Vector3 originVec = new Vector3(90f, 85f, 0f);
            var toggle = CreateToggle(id, name, false, originVec + new Vector3(0, order * -29.5f, 0), 250f);
            AddTooltip(toggle, tooltipKey);
            
            var hotspotButton = toggle.transform.Find("HotSpot")?.GetComponent<StandardMenuButton>();
            hotspotButton.OnPress.AddListener(() => SendData());


            return toggle;
        }
    }
}