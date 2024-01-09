using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using static Obeliskial_Essentials.Essentials;
using System;
using UniverseLib;
using static Enums;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

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

                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID + ".versionUI", value);
            }
        }
        internal static void InitUI()
        {
            uiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID + ".versionUI", UpdateUI);
            //MedsUI MedsPanel = new MedsUI(uiBase);
            uiRoot = UIFactory.CreateUIObject("medsVersionWindow", uiBase.RootObject);
            uiRoot.AddComponent<Image>().color = new Color32(9, 2, 15, 230);
            
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
                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID + ".versionUI", true);
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
    public class DevTools : UniverseLib.UI.Panels.PanelBase
    {
        public static DevTools Instance { get; internal set; }
        public DevTools(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        internal static UIBase uiBase;
        public override string Name => "Developer Tools";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0f, 1f);
        public override Vector2 DefaultAnchorMax => new(0f, 1f);
        public override bool CanDragAndResize => true;
        internal static Text labelMouseXY;
        public static GameObject lockAtOGO;
        public static Toggle lockAtOToggle;
        internal static InputFieldRef inputStartingNode;
        internal static ButtonRef btnProfileEditor;
        internal static bool ShowUI
        {
            get => uiBase != null && uiBase.Enabled;
            set
            {
                if (uiBase == null || !uiBase.RootObject || uiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID + ".devToolsUI", value);
                Instance.SetActive(value);
            }
        }
        protected override void ConstructPanelContent()
        {

            GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;
            closeHolder.transform.Find("CloseButton").gameObject.SetActive(false);

            ButtonRef btnClose = UIFactory.CreateButton(closeHolder.gameObject, "btnClose", "Close", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(btnClose.Component.gameObject, minHeight: 25, minWidth: 50);
            btnClose.Component.onClick.AddListener(delegate
            {
                ShowUI = false;
            });
            ButtonRef btnCloseAll = UIFactory.CreateButton(closeHolder.gameObject, "btnCloseAll", "Close All (F2)", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(btnCloseAll.Component.gameObject, minHeight: 25, minWidth: 100);
            btnCloseAll.Component.onClick.AddListener(ChangeUIState);

            GameObject medsDevToolsGO = UIFactory.CreateVerticalGroup(ContentRoot, "medsDevTools", true, true, true, true, 5, new Vector4(4, 4, 4, 4), new Color32(18, 4, 20, 255), TextAnchor.UpperLeft);
            //medsDevToolsGO.AddComponent<Image>().color = new Color(0.03f, 0.008f, 0.05f, 0.1f);

            labelMouseXY = UIFactory.CreateLabel(medsDevToolsGO, "labelMouseX", "Mouse x: ", TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(labelMouseXY.gameObject, minWidth: 100);

            ButtonRef btnPartyXP = UIFactory.CreateButton(medsDevToolsGO, "btnPartyXP", "+150 party xp");
            UIFactory.SetLayoutElement(btnPartyXP.Component.gameObject, minWidth: 100, minHeight: 30);
            btnPartyXP.Component.onClick.AddListener(delegate
            {
                for (int i = 0; i < 4; i++)
                {
                    try { AtOManager.Instance.GetHero(i).GrantExperience(150); }
                    catch (Exception e) { LogError("Failed to add 150 xp to hero " + i.ToString() + ": " + e.Message); };
                }
            });

            ButtonRef btn1HPEnemies = UIFactory.CreateButton(medsDevToolsGO, "btn1HPEnemies", "Set Enemy HP to 1");
            UIFactory.SetLayoutElement(btn1HPEnemies.Component.gameObject, minWidth: 100, minHeight: 30);
            btn1HPEnemies.Component.onClick.AddListener(delegate
            {
                try
                {
                    NPC[] teamNPC = MatchManager.Instance.GetTeamNPC();
                    foreach (NPC npc in teamNPC)
                    {
                        if (npc != null && npc.Alive)
                            npc.HpCurrent = 1;
                    }
                }
                catch (Exception e) { LogError("Failed to set enemy HP to 1: " + e.Message); };
            });
            inputStartingNode = UIFactory.CreateInputField(medsDevToolsGO, "inputStartingNode", "starting node");
            UIFactory.SetLayoutElement(inputStartingNode.Component.gameObject, minWidth: 100, minHeight: 30);
            lockAtOGO = UIFactory.CreateToggle(medsDevToolsGO, "disableButtonsToggle", out lockAtOToggle, out Text lockAtOText);
            lockAtOText.text = "Disable AtO Buttons";
            lockAtOToggle.isOn = false;
            UIFactory.SetLayoutElement(lockAtOGO, minWidth: 85, minHeight: 20);

            btnProfileEditor = UIFactory.CreateButton(medsDevToolsGO, "btnProfileEditor", "Profile Editor");
            UIFactory.SetLayoutElement(btnProfileEditor.Component.gameObject, minWidth: 100, minHeight: 30);
            RuntimeHelper.SetColorBlock(btnProfileEditor.Component, UniversalUI.DisabledButtonColor, UniversalUI.DisabledButtonColor * 1.2f);
            btnProfileEditor.Component.onClick.AddListener(delegate
            {
                try
                {
                    ProfileEditor.ShowUI = !ProfileEditor.ShowUI;
                    foreach (SubClassData _SCD in Globals.Instance.SubClass.Values)
                    {

                    }
                    //AlertManager.buttonClickDelegate = (AlertManager.OnButtonClickDelegate)null;
                    //AlertManager.Instance.AlertConfirm(@"Player profile exported to Across the Obelisk\BepInEx\config\player_export.json");
                }
                catch (Exception e) { LogDebug("Failed to open profile editor: " + e.Message); };
            });

            Canvas.ForceUpdateCanvases();
        }
        internal static void Init()
        {
            uiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID + ".devToolsUI", UpdateUI);
            DevTools devTools = new(uiBase);
            ShowUI = false;
            UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID + ".devToolsUI", false);
        }
        private static void UpdateUI()
        {
            try
            {
                Vector3 newPos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                labelMouseXY.text = "x: " + newPos.x.ToString("0.0000") + " | y: " + newPos.y.ToString("0.0000");
            }
            catch { }
        }

        // override other methods as desired
    }


    public class ProfileEditor : UniverseLib.UI.Panels.PanelBase
    {
        public static ProfileEditor Instance { get; internal set; }
        public ProfileEditor(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        internal static UIBase uiBase;
        public override string Name => "Profile Editor";
        public override int MinWidth => 900;
        public override int MinHeight => 600;
        public override Vector2 DefaultAnchorMin => new(0f, 1f);
        public override Vector2 DefaultAnchorMax => new(0f, 1f);
        public override Vector2 DefaultPosition => new(-200f, 200f); //-367, -187
        public override bool CanDragAndResize => true;
        internal static Dictionary<string, Toggle> toggleHeroesUnlocked = new();
        internal static Dictionary<string, InputFieldRef> inputHeroesRank = new();
        internal static Dictionary<string, InputFieldRef> inputHeroesExperience = new();
        internal static Dictionary<string, Toggle> toggleCardbacksUnlocked = new();
        internal static Dictionary<string, Toggle> toggleCardsUnlocked = new();
        internal static Dictionary<string, Toggle> toggleNodesUnlocked = new();
        internal static Toggle toggleAllHeroesUnlocked;
        internal static ButtonRef btnAllHeroesLockUnlock;
        internal static InputFieldRef inputAllHeroesRank;
        internal static InputFieldRef inputAllHeroesExperience;
        internal static InputFieldRef inputSupplies;
        internal static InputFieldRef inputAdventureMadness;
        internal static InputFieldRef inputAdventureMadness2;
        internal static InputFieldRef inputObeliskMadness;
        internal static GameObject profileScroll;
        internal static GameObject profileScrollContent;
        //internal static Text labelMouseXY;
        //internal static InputFieldRef inputStartingNode;
        internal static bool ShowUI
        {
            get => uiBase != null && uiBase.Enabled;
            set
            {
                if (uiBase == null || !uiBase.RootObject || uiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID + ".profileEditorUI", value);
                Instance.SetActive(value);
                Color color = value ? new Color(0.2f, 0.28f, 0.4f) : UniversalUI.DisabledButtonColor;
                RuntimeHelper.SetColorBlock(DevTools.btnProfileEditor.Component, color, color * 1.2f);
            }
        }
        protected override void ConstructPanelContent()
        {
            Text spacer;
            // create close buttons in title bar
            GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;
            closeHolder.transform.Find("CloseButton").gameObject.SetActive(false);
            ButtonRef btnClose = UIFactory.CreateButton(closeHolder.gameObject, "btnClose", "Close", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(btnClose.Component.gameObject, minHeight: 25, minWidth: 50);
            btnClose.Component.onClick.AddListener(delegate
            {
                ShowUI = false;
            });
            ButtonRef btnCloseAll = UIFactory.CreateButton(closeHolder.gameObject, "btnCloseAll", "Close All (F2)", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(btnCloseAll.Component.gameObject, minHeight: 25, minWidth: 100);
            btnCloseAll.Component.onClick.AddListener(ChangeUIState);

            // create body
            GameObject medsProfileEditorGO = UIFactory.CreateUIObject("medsProfileEditor", ContentRoot);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(medsProfileEditorGO, true, true, true, true, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(medsProfileEditorGO, minWidth: 300, flexibleWidth: 9999, minHeight: 300, flexibleHeight: 9999);

            // create load/save buttons
            GameObject horizontalLoadSave = UIFactory.CreateUIObject("HoriGroup", medsProfileEditorGO);
            UIFactory.SetLayoutElement(horizontalLoadSave, minHeight: 30, flexibleWidth: 9999, flexibleHeight: 0);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horizontalLoadSave, false, false, true, true, 20, 2, childAlignment: TextAnchor.MiddleCenter);

            ButtonRef btnLoad = UIFactory.CreateButton(horizontalLoadSave, "btnLoad", "Load", new Color(0.2f, 0.28f, 0.4f));
            UIFactory.SetLayoutElement(btnLoad.Component.gameObject, minHeight: 30, minWidth: 50, flexibleWidth: 0, flexibleHeight: 0);
            btnLoad.ButtonText.fontStyle = FontStyle.Bold;
            btnLoad.Component.onClick.AddListener(LoadPlayerProfile);

            ButtonRef btnSave = UIFactory.CreateButton(horizontalLoadSave, "btnSave", "Save", new Color(0.2f, 0.4f, 0.28f));
            UIFactory.SetLayoutElement(btnSave.Component.gameObject, minHeight: 30, minWidth: 50, flexibleWidth: 0, flexibleHeight: 0);
            btnSave.ButtonText.fontStyle = FontStyle.Bold;
            btnSave.Component.onClick.AddListener(SavePlayerProfile);

            // create scrollview
            profileScroll = UIFactory.CreateScrollView(medsProfileEditorGO, "medsProfileScroll", out profileScrollContent, out _, new Color32(18, 4, 20, 255));
            UIFactory.SetLayoutElement(profileScroll, minWidth: 600, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(profileScrollContent, spacing: 5, padTop: 4, padBottom: 4, padLeft: 4, padRight: 4);

            ButtonRef btnTutorial = UIFactory.CreateButton(profileScrollContent, "btnTutorial", "Complete Tutorials");
            UIFactory.SetLayoutElement(btnTutorial.Component.gameObject, minHeight: 25, minWidth: 100, flexibleWidth: 0, flexibleHeight: 0);
            btnTutorial.Component.onClick.AddListener(delegate
            {
                try
                {
                    PlayerManager.Instance.TutorialWatched = new List<string> { "town", "townCraft", "cardsReward", "eventRolls", "characterPerks", "townReward", "firstTurnEnergy", "cardTarget", "combatSpeed", "castNPC", "townItemCraft" };
                    SaveManager.SavePlayerData();
                }
                catch (Exception e) { LogError("Failed to complete tutorials: " + e.Message); };
            });

            GameObject horizontalSupplies = UIFactory.CreateUIObject("HoriGroup", profileScrollContent);
            UIFactory.SetLayoutElement(horizontalSupplies, minHeight: 30, flexibleWidth: 9999, flexibleHeight: 30);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horizontalSupplies, false, false, true, true, 5, 2, childAlignment: TextAnchor.UpperLeft);

            inputSupplies = UIFactory.CreateInputField(horizontalSupplies, "inputSupplies", "0-999");
            UIFactory.SetLayoutElement(inputSupplies.Component.gameObject, minWidth: 40, minHeight: 30);

            Text labelSupplies = UIFactory.CreateLabel(horizontalSupplies, "labelSupplies", "Supplies");
            UIFactory.SetLayoutElement(labelSupplies.gameObject, minWidth: 60, minHeight: 30);

            /*
            ButtonRef btnPartyXP = UIFactory.CreateButton(medsDevToolsGO, "btnPartyXP", "+150 party xp");
            UIFactory.SetLayoutElement(btnPartyXP.Component.gameObject, minWidth: 100, minHeight: 30);
            btnPartyXP.Component.onClick.AddListener(delegate
            {
                for (int i = 0; i < 4; i++)
                {
                    try { AtOManager.Instance.GetHero(i).GrantExperience(150); }
                    catch (Exception e) { LogError("Failed to add 150 xp to hero " + i.ToString() + ": " + e.Message); };
                }
            });


            // from UnityExplorer
            /*

            GameObject horiRow = UIFactory.CreateUIObject("HoriGroup", UIRoot);
            UIFactory.SetLayoutElement(horiRow, minHeight: 29, flexibleHeight: 150, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horiRow, false, false, true, true, 5, 2, childAlignment: TextAnchor.UpperLeft);
            horiRow.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Left name label

            Text NameLabel = UIFactory.CreateLabel(horiRow, "NameLabel", "<notset>", TextAnchor.MiddleLeft);
            NameLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
            LayoutElement NameLayout = UIFactory.SetLayoutElement(NameLabel.gameObject, minHeight: 25, minWidth: 20, flexibleHeight: 300, flexibleWidth: 0);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(NameLabel.gameObject, true, true, true, true);

            InputFieldRef HiddenNameLabel = UIFactory.CreateInputField(NameLabel.gameObject, "HiddenNameLabel", "");
            RectTransform hiddenRect = HiddenNameLabel.Component.GetComponent<RectTransform>();
            hiddenRect.anchorMin = Vector2.zero;
            hiddenRect.anchorMax = Vector2.one;
            HiddenNameLabel.Component.readOnly = true;
            HiddenNameLabel.Component.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;
            HiddenNameLabel.Component.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            HiddenNameLabel.Component.gameObject.GetComponent<Image>().color = Color.clear;
            HiddenNameLabel.Component.textComponent.color = Color.clear;
            UIFactory.SetLayoutElement(HiddenNameLabel.Component.gameObject, minHeight: 25, minWidth: 20, flexibleHeight: 300, flexibleWidth: 0);

            // Right vertical group

            GameObject RightGroupContent = UIFactory.CreateUIObject("RightGroup", horiRow);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(RightGroupContent, false, false, true, true, 4, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(RightGroupContent, minHeight: 25, minWidth: 200, flexibleWidth: 9999, flexibleHeight: 800);
            LayoutElement RightGroupLayout = RightGroupContent.GetComponent<LayoutElement>();

            ConstructEvaluateHolder(RightGroupContent);

            // Right horizontal group

            GameObject rightHoriGroup = UIFactory.CreateUIObject("RightHoriGroup", RightGroupContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(rightHoriGroup, false, false, true, true, 4, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(rightHoriGroup, minHeight: 25, minWidth: 200, flexibleWidth: 9999, flexibleHeight: 800);

            SubContentButton = UIFactory.CreateButton(rightHoriGroup, "SubContentButton", "▲", subInactiveColor);
            UIFactory.SetLayoutElement(SubContentButton.Component.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
            SubContentButton.OnClick += SubContentClicked;

            */
            // heroes 
            // create container + accordion
            GameObject GOHeroes = UIFactory.CreateUIObject("medsHeroes", profileScrollContent);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(GOHeroes, true, false, true, true, 5, 2, 2, 0, 0, TextAnchor.UpperLeft);
            ButtonRef btnViewHeroes = UIFactory.CreateButton(GOHeroes, "btnViewHeroes", "Show Heroes");
            UIFactory.SetLayoutElement(btnViewHeroes.Component.gameObject, minHeight: 25, minWidth: 100);
            GameObject GOHeroVertical = UIFactory.CreateUIObject("medsHeroVertical", GOHeroes);
            btnViewHeroes.ButtonText.fontStyle = FontStyle.Bold;
            btnViewHeroes.Component.onClick.AddListener(delegate
            {
                btnViewHeroes.ButtonText.text = GOHeroVertical.gameObject.activeSelf ? "Show Heroes" : "Hide Heroes";
                Color color = GOHeroVertical.gameObject.activeSelf ? UniversalUI.DisabledButtonColor : new Color(0.2f, 0.28f, 0.4f);
                RuntimeHelper.SetColorBlock(btnViewHeroes.Component, color, color * 1.2f);
                GOHeroVertical.gameObject.SetActive(!GOHeroVertical.gameObject.activeSelf);
            });
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(GOHeroVertical, true, false, true, true, 2, 2, 2, 0, 0, TextAnchor.UpperLeft);

            // all heroes
            GameObject GOHeroHorizontal = UIFactory.CreateUIObject("medsHeroHorizontalAll", GOHeroVertical);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOHeroHorizontal, true, false, true, true, 10, 0, 0, 0, 0, TextAnchor.MiddleCenter);
            // locked/unlocked
            GameObject GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontalAllA", GOHeroHorizontal);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllA1", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 40, minHeight: 30);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllA2", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 31, minHeight: 30);
            btnAllHeroesLockUnlock = UIFactory.CreateButton(GOSCDHorizontal, "btnAllHeroesLockUnlock", "Unlock All");
            UIFactory.SetLayoutElement(btnAllHeroesLockUnlock.Component.gameObject, minHeight: 25, minWidth: 100);
            btnAllHeroesLockUnlock.Component.onClick.AddListener(AllHeroesLockUnlock);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllA3", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 31, minHeight: 30);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllA4", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 40, minHeight: 30);

            // hero rank
            GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontalAllB", GOHeroHorizontal);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            ButtonRef btnAllHeroesSetRank = UIFactory.CreateButton(GOSCDHorizontal, "btnAllHeroesSetRank", "Set All");
            UIFactory.SetLayoutElement(btnAllHeroesSetRank.Component.gameObject, minHeight: 25, minWidth: 100);
            btnAllHeroesSetRank.Component.onClick.AddListener(AllHeroesRankXPApply);
            Text rank = UIFactory.CreateLabel(GOSCDHorizontal, "labelRankAll", "Rank:");
            UIFactory.SetLayoutElement(rank.gameObject, minWidth: 40, minHeight: 30);
            rank.alignment = TextAnchor.MiddleRight;
            inputAllHeroesRank = UIFactory.CreateInputField(GOSCDHorizontal, "inputAllHeroesRank", "");
            inputAllHeroesRank.Component.textComponent.alignment = TextAnchor.MiddleRight;
            inputAllHeroesRank.Component.characterValidation = InputField.CharacterValidation.Integer;
            inputAllHeroesRank.Component.SetTextWithoutNotify(Globals.Instance.PerkLevel.Count.ToString());
            inputAllHeroesRank.Component.onValueChanged.AddListener((newRank) => { AllHeroesRankUpdate(newRank); });

            // hero experience
            UIFactory.SetLayoutElement(inputAllHeroesRank.Component.gameObject, minWidth: 22, minHeight: 30);
            Text xp = UIFactory.CreateLabel(GOSCDHorizontal, "labelXPAll", "XP:");
            UIFactory.SetLayoutElement(xp.gameObject, minWidth: 30, minHeight: 30);
            xp.alignment = TextAnchor.MiddleRight;
            inputAllHeroesExperience = UIFactory.CreateInputField(GOSCDHorizontal, "inputAllHeroesExperience", "");
            inputAllHeroesExperience.Component.textComponent.alignment = TextAnchor.MiddleRight;
            inputAllHeroesExperience.Component.characterValidation = InputField.CharacterValidation.Integer;
            inputAllHeroesExperience.Component.SetTextWithoutNotify(Globals.Instance.PerkLevel[^1].ToString());
            inputAllHeroesExperience.Component.onValueChanged.AddListener((newXP) => { AllHeroesXPUpdate(newXP); });
            UIFactory.SetLayoutElement(inputAllHeroesExperience.Component.gameObject, minWidth: 50, minHeight: 30);

            GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontalAllC", GOHeroHorizontal);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllC1", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 100, minHeight: 30);
            btnAllHeroesLockUnlock.Component.onClick.AddListener(AllHeroesLockUnlock);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllC2", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 31, minHeight: 30);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllC3", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 40, minHeight: 30);
            btnAllHeroesLockUnlock.Component.onClick.AddListener(AllHeroesLockUnlock);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllC4", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 31, minHeight: 30);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllC5", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 40, minHeight: 30);

            // individual heroes
            GOHeroHorizontal = UIFactory.CreateUIObject("medsHeroHorizontal0", GOHeroVertical);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOHeroHorizontal, true, false, true, true, 10, 0, 0, 0, 0, TextAnchor.MiddleCenter);
            int a = 0;
            foreach (SubClassData scd in Globals.Instance.SubClass.Values)
            {
                if (!scd.MainCharacter || scd.Id == "medsdlcone" || scd.Id == "medsdlctwo" || scd.Id == "medsdlcthree" || scd.Id == "medsdlcfour")
                    continue;
                // create horizontal container for every fourth subclass
                if (a > 1 && a % 3 == 0)
                {
                    GOHeroHorizontal = UIFactory.CreateUIObject("medsHeroHorizontal" + a.ToString(), GOHeroVertical);
                    UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOHeroHorizontal, true, false, true, true, 10, 0, 0, 0, 0, TextAnchor.MiddleCenter);
                }
                else if (a % 3 != 0)
                {
                    spacer = UIFactory.CreateLabel(GOHeroHorizontal, "spacer" + a.ToString(), " ");
                    UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 0, minHeight: 30, flexibleWidth: 999);
                }
                GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontal" + scd.Id, GOHeroHorizontal);
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
                // locked/unlocked
                GameObject GOHeroesUnlocked = UIFactory.CreateToggle(GOSCDHorizontal, "medsHeroesUnlockedToggle" + scd.Id, out Toggle toggleTemp, out Text toggleText);
                toggleHeroesUnlocked[scd.Id] = toggleTemp;
                toggleText.text = scd.CharacterName;
                toggleText.fontStyle = FontStyle.Bold;
                UIFactory.SetLayoutElement(GOHeroesUnlocked, minWidth: 100, minHeight: 30);
                toggleHeroesUnlocked[scd.Id].onValueChanged.AddListener(delegate { HeroLockUnlock(scd.Id); });

                // hero rank
                rank = UIFactory.CreateLabel(GOSCDHorizontal, "labelRank" + scd.Id, "Rank:");
                UIFactory.SetLayoutElement(rank.gameObject, minWidth: 40, minHeight: 30);
                rank.alignment = TextAnchor.MiddleRight;
                inputHeroesRank[scd.Id] = UIFactory.CreateInputField(GOSCDHorizontal, "medsHeroesRank" + scd.Id,"");
                inputHeroesRank[scd.Id].Component.textComponent.alignment = TextAnchor.MiddleRight;
                inputHeroesRank[scd.Id].Component.characterValidation = InputField.CharacterValidation.Integer;
                inputHeroesRank[scd.Id].Component.onValueChanged.AddListener((newRank) => { HeroRankUpdate(scd.Id, newRank); });
                UIFactory.SetLayoutElement(inputHeroesRank[scd.Id].Component.gameObject, minWidth: 22, minHeight: 30);

                // hero experience
                xp = UIFactory.CreateLabel(GOSCDHorizontal, "labelXP" + scd.Id, "XP:");
                UIFactory.SetLayoutElement(xp.gameObject, minWidth: 30, minHeight: 30);
                xp.alignment = TextAnchor.MiddleRight;
                inputHeroesExperience[scd.Id] = UIFactory.CreateInputField(GOSCDHorizontal, "medsHeroesExperience" + scd.Id, "");
                inputHeroesExperience[scd.Id].Component.textComponent.alignment = TextAnchor.MiddleRight;
                inputHeroesExperience[scd.Id].Component.characterValidation = InputField.CharacterValidation.Integer;
                inputHeroesExperience[scd.Id].Component.onValueChanged.AddListener((newXP) => { HeroXPUpdate(scd.Id, newXP); });
                UIFactory.SetLayoutElement(inputHeroesExperience[scd.Id].Component.gameObject, minWidth: 50, minHeight: 30);
                a++;
            }
            while (a % 3 != 0)
            {
                spacer = UIFactory.CreateLabel(GOHeroHorizontal, "spacer" + a.ToString(), " ");
                UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 0, minHeight: 30, flexibleWidth: 999);
                GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontal" + a.ToString(), GOHeroHorizontal);
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
                spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerA" + a.ToString(), " ");
                UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 100, minHeight: 30);
                spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerB" + a.ToString(), " ");
                UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 40, minHeight: 30);
                spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerC" + a.ToString(), " ");
                UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 22, minHeight: 30);
                spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerD" + a.ToString(), " ");
                UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 30, minHeight: 30);
                spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerE" + a.ToString(), " ");
                UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 50, minHeight: 30);
                a++;
            }
            GOHeroVertical.gameObject.SetActive(false);

            // cards 
            // ensure order
            Dictionary<string, List<string>> cardsByCategory = new();
            cardsByCategory["Warrior"] = new();
            cardsByCategory["Scout"] = new();
            cardsByCategory["Mage"] = new();
            cardsByCategory["Healer"] = new();
            cardsByCategory["Special"] = new();
            cardsByCategory["Boon"] = new();
            cardsByCategory["Injury"] = new();
            cardsByCategory["Weapon"] = new();
            cardsByCategory["Armor"] = new();
            cardsByCategory["Jewelry"] = new();
            cardsByCategory["Accesory"] = new();
            cardsByCategory["Pet"] = new();

            foreach (CardData card in Globals.Instance.Cards.Values)
            {
                if (card.CardUpgraded != CardUpgraded.No || card.CardClass == CardClass.MagicKnight || card.CardClass == CardClass.Monster || card.CardType == CardType.None || (card.CardClass == CardClass.Special && !(card.CardType == CardType.Enchantment || card.Starter)))
                    continue;
                if (card.CardClass == CardClass.Item)
                    cardsByCategory[DataTextConvert.ToString(card.CardType)].Add(card.Id);
                else
                    cardsByCategory[DataTextConvert.ToString(card.CardClass)].Add(card.Id);
            }


            /*labelMouseXY = UIFactory.CreateLabel(medsDevToolsGO, "labelMouseX", "Mouse x: ", TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(labelMouseXY.gameObject, minWidth: 100);


            inputStartingNode = UIFactory.CreateInputField(medsDevToolsGO, "inputStartingNode", "starting node");
            UIFactory.SetLayoutElement(inputStartingNode.Component.gameObject, minWidth: 100, minHeight: 30);



            lockAtOGO = UIFactory.CreateToggle(medsDevToolsGO, "disableButtonsToggle", out lockAtOToggle, out Text lockAtOText);
            lockAtOText.text = "Disable AtO Buttons";
            lockAtOToggle.isOn = false;
            UIFactory.SetLayoutElement(lockAtOGO, minWidth: 85, minHeight: 20);*/


            // #TODO: can you change position here? also maybe add log to see when this occurs:
            profileScrollContent.SetActive(false);
            LogDebug("Profile Editor ConstructPanelContent end");
        }
        internal static void AllHeroesLockUnlock()
        {
            foreach (string scID in toggleHeroesUnlocked.Keys)
                toggleHeroesUnlocked[scID].isOn = btnAllHeroesLockUnlock.ButtonText.text != "Lock All";
            btnAllHeroesLockUnlock.ButtonText.text = btnAllHeroesLockUnlock.ButtonText.text == "Lock All" ? "Unlock All" : "Lock All";
        }
        internal static void AllHeroesRankUpdate(string _newRank)
        {
            if (int.TryParse(_newRank, out int iNewRank) && int.TryParse(inputAllHeroesExperience.Text, out int iNewXP))
            {
                iNewRank = Math.Clamp(iNewRank, 0, Globals.Instance.PerkLevel.Count);
                iNewXP = ClampXP(iNewXP, iNewRank);
                inputAllHeroesRank.Component.SetTextWithoutNotify(iNewRank.ToString());
                inputAllHeroesExperience.Component.SetTextWithoutNotify(iNewXP.ToString());
            }
            else
            {
                LogError("Unable to parse all heroes rank " + _newRank + " xp " + inputAllHeroesExperience.Text);
            }
        }
        internal static void AllHeroesXPUpdate(string _newXP)
        {
            if (int.TryParse(_newXP, out int iNewXP))
            {
                iNewXP = Math.Clamp(iNewXP, 0, Globals.Instance.PerkLevel[^1]);
                int iNewRank = 0;
                for (int index = 0; index < Globals.Instance.PerkLevel.Count && iNewXP >= Globals.Instance.PerkLevel[index]; ++index)
                    ++iNewRank;
                inputAllHeroesRank.Component.SetTextWithoutNotify(iNewRank.ToString());
                inputAllHeroesExperience.Component.SetTextWithoutNotify(iNewXP.ToString());
            }
            else
            {
                LogError("Unable to parse all heroes xp " + _newXP);
            }
        }
        internal static void AllHeroesRankXPApply()
        {
            foreach (string scID in inputHeroesRank.Keys)
                inputHeroesRank[scID].Component.SetTextWithoutNotify(inputAllHeroesRank.Text);
            foreach (string scID in inputHeroesExperience.Keys)
                inputHeroesExperience[scID].Component.SetTextWithoutNotify(inputAllHeroesExperience.Text);
        }
        internal static int ClampXP(int _XP, int _rank)
        {
            int iXPMin = _rank == 0 ? 0 : Globals.Instance.PerkLevel[_rank - 1];
            int iXPMax = _rank == Globals.Instance.PerkLevel.Count ? Globals.Instance.PerkLevel[_rank - 1] : Globals.Instance.PerkLevel[_rank];
            if (_XP >= iXPMax || _XP < iXPMin)
                return iXPMin;
            return _XP;
        }
        internal static void HeroLockUnlock(string _id)
        {
            inputHeroesExperience[_id].Component.interactable = toggleHeroesUnlocked[_id].isOn;
            inputHeroesRank[_id].Component.interactable = toggleHeroesUnlocked[_id].isOn;
            List<string> cardList = Globals.Instance.GetSubClassData(_id)?.GetCardsId() ?? new List<string>();
            CardData item = Globals.Instance.GetSubClassData(_id)?.Item;
            if (item != null)
                cardList.Add(item.Id);
            foreach (string cardID in cardList)
                if (toggleCardsUnlocked.ContainsKey(cardID))
                    toggleCardsUnlocked[cardID].SetIsOnWithoutNotify(toggleHeroesUnlocked[_id].isOn);
        }
        internal static void HeroRankUpdate(string _id, string _newRank)
        {
            if (int.TryParse(_newRank, out int iNewRank) && int.TryParse(inputHeroesExperience[_id].Text, out int iNewXP))
            {
                iNewRank = Math.Clamp(iNewRank, 0, Globals.Instance.PerkLevel.Count);
                iNewXP = ClampXP(iNewXP, iNewRank);
                inputHeroesRank[_id].Component.SetTextWithoutNotify(iNewRank.ToString());
                inputHeroesExperience[_id].Component.SetTextWithoutNotify(iNewXP.ToString());
            }
            else
            {
                LogError("Unable to parse subclass " + _id + " rank " + _newRank + " xp " + inputHeroesExperience[_id].Text);
            }
        }
        internal static void HeroXPUpdate(string _id, string _newXP)
        {
            if (int.TryParse(_newXP, out int iNewXP))
            {
                iNewXP = Math.Clamp(iNewXP, 0, Globals.Instance.PerkLevel[^1]);
                int iNewRank = 0;
                for (int index = 0; index < Globals.Instance.PerkLevel.Count && iNewXP >= Globals.Instance.PerkLevel[index]; ++index)
                    ++iNewRank;
                inputHeroesRank[_id].Component.SetTextWithoutNotify(iNewRank.ToString());
                inputHeroesExperience[_id].Component.SetTextWithoutNotify(iNewXP.ToString());
            }
            else
            {
                LogError("Unable to parse subclass " + _id + " xp " + _newXP);
            }
        }
        internal static void LoadPlayerProfile()
        {
            btnAllHeroesLockUnlock.ButtonText.text = "Lock All";
            foreach (string scID in toggleHeroesUnlocked.Keys)
            {
                bool unlocked = PlayerManager.Instance.IsHeroUnlocked(scID);
                LogDebug("Hero " + scID + (unlocked ? " is unlocked" : " is locked"));
                toggleHeroesUnlocked[scID].SetIsOnWithoutNotify(unlocked);
                inputHeroesRank[scID].Component.SetTextWithoutNotify(PlayerManager.Instance.GetPerkRank(scID).ToString());
                inputHeroesExperience[scID].Component.SetTextWithoutNotify(PlayerManager.Instance.GetProgress(scID).ToString());
                inputHeroesRank[scID].Component.interactable = unlocked;
                inputHeroesExperience[scID].Component.interactable = unlocked;
                if (!unlocked)
                    btnAllHeroesLockUnlock.ButtonText.text = "Unlock All";
            }
            inputSupplies.Text = PlayerManager.Instance.SupplyActual.ToString();
            profileScrollContent.SetActive(true);
        }
        internal static void SavePlayerProfile()
        {
            List<string> unlockedHeroes = new();
            foreach (string scID in toggleHeroesUnlocked.Keys)
            {
                if (toggleHeroesUnlocked[scID].isOn)
                    unlockedHeroes.Add(scID);
                if (int.TryParse(inputHeroesExperience[scID].Text, out int newXP))
                    PlayerManager.Instance.HeroProgress[scID] = newXP;
            }
            PlayerManager.Instance.UnlockedHeroes = unlockedHeroes;
            if (int.TryParse(inputSupplies.Text, out int newSupplies))
                PlayerManager.Instance.SupplyActual = newSupplies;
            SaveManager.SavePlayerData();
        }
        internal static void Init()
        {
            uiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID + ".profileEditorUI", UpdateUI);
            ProfileEditor profileEditor = new(uiBase);
            ShowUI = false;
            UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID + ".profileEditorUI", false);
        }
        private static void UpdateUI()
        {

        }

        // override other methods as desired
    }

}