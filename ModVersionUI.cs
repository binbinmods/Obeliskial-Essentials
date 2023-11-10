using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using static Obeliskial_Essentials.Essentials;

namespace Obeliskial_Essentials
{

    public class ObeliskialUI : MonoBehaviour
    {
        private static UIBase uiBase;
        private static GameObject uiRoot;
        private static GameObject showAtStartGO;
        private static Toggle showAtStartToggle;
        internal static Text modVersions;
        /*private static GameObject uiVert;
        private static RectTransform uiRect;
        private static GameObject lockAtOGO;
        internal static Toggle lockAtOToggle;

        internal static ButtonRef settingsBtn;
        internal static ButtonRef userToolsBtn;
        internal static ButtonRef devToolsBtn;
        internal static ButtonRef hideBtn;
        internal static Text labelMouseX;
        internal static Text labelMouseY;*/
        internal static bool ShowUI
        {
            get => uiBase != null && uiBase.Enabled;
            set
            {
                if (uiBase == null || !uiBase.RootObject || uiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, value);
            }
        }
        internal static void InitUI()
        {
            uiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID, UpdateUI);
            //MedsUI MedsPanel = new MedsUI(uiBase);
            uiRoot = UIFactory.CreateUIObject("medsNavbar", uiBase.RootObject);
            uiRoot.AddComponent<Image>().color = new Color(0.03f, 0.008f, 0.05f, 0.9f);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(uiRoot, false, false, true, true, 5, 8, 8, 8, 8);
            //uiVert = UIFactory.CreateVerticalGroup(uiNav, "medsNavVert", true, false, true, true, 5, new Vector4(4, 4, 4, 4), new Color(0.03f, 0.008f, 0.05f, 0.9f), TextAnchor.UpperLeft);

            RectTransform uiRect = uiRoot.GetComponent<RectTransform>();
            uiRect.pivot = new Vector2(0.5f, 0.5f);
            uiRect.anchorMin = new Vector2(0.5f, 0.5f);
            uiRect.anchorMax = new Vector2(0.5f, 0.5f);
            uiRect.anchoredPosition = new Vector2(uiRect.anchoredPosition.x, uiRect.anchoredPosition.y);
            uiRect.sizeDelta = new(350f, 350f);
            Text title = UIFactory.CreateLabel(uiRoot, "Title", "Registered Mods", TextAnchor.UpperLeft, fontSize: 25);
            title.fontStyle = FontStyle.Bold;
            //title.gameObject.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.9f);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 100, minHeight: 20, flexibleHeight: 0); //, flexibleWidth: 100);

            modVersions = UIFactory.CreateLabel(uiRoot, "Mod Versions", "Obeliskial Essentials v" + PluginInfo.PLUGIN_VERSION, TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(modVersions.gameObject, flexibleHeight: 100);

            GameObject uiHorizontal = UIFactory.CreateUIObject("medsTogglesHorizontal", uiRoot);
            UIFactory.SetLayoutElement(uiHorizontal, flexibleHeight: 0, flexibleWidth: 100);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(uiHorizontal, false, false, true, true, 20, 0, 0, 30, 10, TextAnchor.LowerCenter);
            //GameObject togglesHorizontal = UIFactory.CreateHorizontalGroup(uiVert, "medsTogglesHorizontal", true, true, true, true, 5, new Vector4(4, 4, 4, 4), new Color(), TextAnchor.UpperCenter);

            ButtonRef closeBtn = UIFactory.CreateButton(uiHorizontal, "closeBtn", "Close");
            UIFactory.SetLayoutElement(closeBtn.Component.gameObject, minWidth: 100, minHeight: 30);
            closeBtn.Component.onClick.AddListener(delegate
            {
                ShowUI = false;
            });

            showAtStartGO = UIFactory.CreateToggle(uiHorizontal, "showAtStartToggle", out showAtStartToggle, out Text showAtStartText, checkWidth: 20, checkHeight: 20);
            showAtStartText.text = "Do Not Show Again";
            showAtStartToggle.isOn = !medsShowAtStart.Value;
            UIFactory.SetLayoutElement(showAtStartGO, minWidth: 85, minHeight: 30);
            showAtStartToggle.onValueChanged.AddListener(delegate
            {
                medsShowAtStart.Value = !showAtStartToggle.isOn;
            });

            // ButtonRef tempBtn = UIFactory.CreateButton(medsNav, "tempButton", "TEST");
            // UIFactory.SetLayoutElement(tempBtn.Component.gameObject, minWidth: 80, minHeight: 30, flexibleWidth: 0);
            // RuntimeHelper.SetColorBlock(tempBtn.Component, new Color(0.22f, 0.54f, 0.22f), new Color(0.15f, 0.71f, 0.1f), new Color(0.08f, 0.5f, 0.06f));


            /*labelMouseX = UIFactory.CreateLabel(uiNav, "labelMouseX", "x:", TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(labelMouseX.gameObject, minWidth: 100);

            labelMouseY = UIFactory.CreateLabel(uiNav, "labelMouseY", "y:", TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(labelMouseY.gameObject, minWidth: 100);*/

            /*lockAtOGO = UIFactory.CreateToggle(togglesHorizontal, "disableButtonsToggle", out lockAtOToggle, out Text lockAtOText);
            lockAtOText.text = "Lock AtO";
            lockAtOToggle.isOn = false;
            UIFactory.SetLayoutElement(lockAtOGO, minWidth: 85, minHeight: 20);*/
            //settingsBtn = UIFactory.CreateButton(medsNav, "settingsButton", "Settings");
            //UIFactory.SetLayoutElement(settingsBtn.Component.gameObject, minWidth: 85, minHeight: 30, flexibleWidth: 0);


            //userToolsBtn = UIFactory.CreateButton(medsNav, "userToolsBtn", "User Tools");
            //UIFactory.SetLayoutElement(userToolsBtn.Component.gameObject, minWidth: 85, minHeight: 30, flexibleWidth: 0);

            //devToolsBtn = UIFactory.CreateButton(medsNav, "devToolsBtn", "Dev Tools");
            //UIFactory.SetLayoutElement(devToolsBtn.Component.gameObject, minWidth: 85, minHeight: 30, flexibleWidth: 0);


            //hideBtn = UIFactory.CreateButton(medsNav, "hideBtn", "Hide (F1)");
            //UIFactory.SetLayoutElement(hideBtn.Component.gameObject, minWidth: 85, minHeight: 30, flexibleWidth: 0);


            Canvas.ForceUpdateCanvases();
            if (medsShowAtStart.Value)
            {
                ShowUI = true;
                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, true);
            }
            else
            {
                ShowUI = false;
            }
            LogInfo($"UI... created?!");
        }
        private static void UpdateUI()
        {
            modVersions.text = medsVersionText;
            /*Vector3 newPos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            labelMouseX.text = "x:" + newPos.x.ToString();
            labelMouseY.text = "y:" + newPos.y.ToString();*/
        }
    }
    /*public class ModVersionUI : MonoBehaviour
    {
        internal static UIBase uiBase { get; private set; }
        //internal static Text modVersions;
        internal static VersionPanel myVersionPanel;
        internal static GameObject lockAtOGO;
        internal static Toggle lockAtOToggle;
        internal static GameObject showAtStartGO;
        internal static Toggle showAtStartToggle;
        public static bool ShowUI
        {
            get => uiBase != null && uiBase.Enabled;
            set
            {
                if (uiBase == null || !uiBase.RootObject || uiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, value);
            }
        }
        internal static void InitUI()
        {

            /*uiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID, UpdateUI);
            myVersionPanel = new(uiBase);
            myVersionPanel.Rect.pivot = new Vector2(0.5f, 0.3f);
            myVersionPanel.Rect.anchorMin = new Vector2(0.5f, 0.3f);
            myVersionPanel.Rect.anchorMax = new Vector2(0.5f, 0.3f);

            Canvas.ForceUpdateCanvases();
            ShowUI = medsShowAtStart.Value;
            UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, Essentials.medsShowAtStart.Value);
            /*myVersionPanel.Rect.anchorMin = new Vector2(1f, 1f);
            myVersionPanel.Rect.anchorMax = new Vector2(1f, 1f);
            myVersionPanel.Dragger.OnEndResize();
        }
        internal static void UpdateUI()
        {
            //modVersions.text = Essentials.medsVersionText;
        }

    }
    public class VersionPanel : UniverseLib.UI.Panels.PanelBase
    {
        public VersionPanel(UIBase owner) : base(owner) { }
        public override string Name => "Obeliskial Essentials (F1 to hide)";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0f, 1f);
        public override Vector2 DefaultAnchorMax => new(0f, 1f);
        public override bool CanDragAndResize => false;
        protected override void ConstructPanelContent()
        {
            //ObeliskialUI.modVersions = UIFactory.CreateLabel(ContentRoot, "Mod Versions", "Obeliskial\nEssentials\nv" + PluginInfo.PLUGIN_VERSION, TextAnchor.UpperLeft);
            //UIFactory.SetLayoutElement(ObeliskialUI.modVersions.gameObject);
        }

        // override other methods as desired
    }*/
}