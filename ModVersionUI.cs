using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;

namespace Obeliskial_Essentials
{

    public class ModVersionUI : MonoBehaviour
    {
        internal static UIBase uiBase { get; private set; }
        internal static Text modVersions;
        internal static VersionPanel myVersionPanel;
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

            uiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID, UpdateUI);
            myVersionPanel = new(uiBase);

            Canvas.ForceUpdateCanvases();
            ShowUI = true;
            UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, true);
            myVersionPanel.Rect.anchorMin = new Vector2(1f, 1f);
            myVersionPanel.Rect.anchorMax = new Vector2(1f, 1f);
            myVersionPanel.Dragger.OnEndResize();
        }
        internal static void UpdateUI()
        {
            modVersions.text = Essentials.medsVersionText;
        }

    }
    public class VersionPanel : UniverseLib.UI.Panels.PanelBase
    {
        public VersionPanel(UIBase owner) : base(owner) { }
        public override string Name => "Mod Versions (F1 to hide)";
        public override int MinWidth => 300;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new(1.35f, 1.7f);
        public override Vector2 DefaultAnchorMax => new(1.35f, 1.7f);
        public override bool CanDragAndResize => true;
        protected override void ConstructPanelContent()
        {
            ModVersionUI.modVersions = UIFactory.CreateLabel(ContentRoot, "Mod Versions", "Obeliskial\nEssentials\nv" + PluginInfo.PLUGIN_VERSION, TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(ModVersionUI.modVersions.gameObject);
        }

        // override other methods as desired
    }
}