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
using BepInEx;
using System.IO;
using System.Reflection;

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
        public override int MinWidth => 350;
        public override int MinHeight => 400;
        public override Vector2 DefaultAnchorMin => new(0f, 1f);
        public override Vector2 DefaultAnchorMax => new(0f, 1f);
        public override bool CanDragAndResize => true;
        internal static Text labelMouseXY;
        public static GameObject lockAtOGO;
        public static Toggle lockAtOToggle;
        internal static InputFieldRef inputStartingNode;
        internal static InputFieldRef inputActivateEvent;
        internal static InputFieldRef inputActivateCombat;
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

            // Mouse coordinates
            labelMouseXY = UIFactory.CreateLabel(medsDevToolsGO, "labelMouseX", "Mouse x: ", TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(labelMouseXY.gameObject, minWidth: 100);

            // create scrollview
            GameObject scrollView = UIFactory.CreateScrollView(medsDevToolsGO, "medsDevToolsScroll", out GameObject devToolsScrollContent, out _, new Color32(18, 4, 20, 255));
            UIFactory.SetLayoutElement(scrollView, minWidth: 350, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(devToolsScrollContent, spacing: 5);


            // +150 Party XP
            ButtonRef btnPartyXP = UIFactory.CreateButton(devToolsScrollContent, "btnPartyXP", "+150 Party XP");
            UIFactory.SetLayoutElement(btnPartyXP.Component.gameObject, minWidth: 100, minHeight: 30);
            btnPartyXP.Component.onClick.AddListener(delegate
            {
                bool noError = true;
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        AtOManager.Instance.GetHero(i).GrantExperience(150);
                    }
                    catch (Exception e)
                    {
                        noError = false;
                        LogError("Failed to add 150 xp to hero " + i.ToString() + ": " + e.Message);
                    };
                }
                if (noError)
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnPartyXP));
                else
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnPartyXP, "Error! Please see LogOutput.log!"));
            });

            // Set Enemy HP to 1
            ButtonRef btn1HPEnemies = UIFactory.CreateButton(devToolsScrollContent, "btn1HPEnemies", "Set Enemy HP to 1");
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
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btn1HPEnemies));
                }
                catch (Exception e)
                {
                    LogError("Failed to set enemy HP to 1: " + e.Message);
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btn1HPEnemies, "Error! Please see LogOutput.log!"));
                };
            });

            // Starting Node
            GameObject hStartingNode = UIFactory.CreateUIObject("hStartingNode", devToolsScrollContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(hStartingNode, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            Text labelStartingNode = UIFactory.CreateLabel(hStartingNode, "labelStartingNode", "Starting node:");
            UIFactory.SetLayoutElement(labelStartingNode.gameObject, minWidth: 60, minHeight: 30);
            inputStartingNode = UIFactory.CreateInputField(hStartingNode, "inputStartingNode", "sen_0");
            inputStartingNode.Component.SetTextWithoutNotify("sen_0");
            UIFactory.SetLayoutElement(inputStartingNode.Component.gameObject, minWidth: 70, minHeight: 30);

            // Weekly 
            // export weekly data
            // force weekly week
            /* export cards
            GameObject hWeekly = UIFactory.CreateUIObject("hWeekly", devToolsScrollContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(hWeekly, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            Text labelWeeklyWeek = UIFactory.CreateLabel(hWeekly, "labelWeeklyWeek", "Starting node:");
            UIFactory.SetLayoutElement(labelWeeklyWeek.gameObject, minWidth: 60, minHeight: 30);
            inputStartingNode = UIFactory.CreateInputField(hWeekly, "inputStartingNode", "sen_0");
            inputStartingNode.Component.SetTextWithoutNotify("sen_0");
            UIFactory.SetLayoutElement(inputStartingNode.Component.gameObject, minWidth: 70, minHeight: 30);
            */

            // Card Image Export
            ButtonRef btnCardImageExport = UIFactory.CreateButton(devToolsScrollContent, "btnCardImageExport", "Card Image Export");
            UIFactory.SetLayoutElement(btnCardImageExport.Component.gameObject, minWidth: 100, minHeight: 30);
            btnCardImageExport.Component.onClick.AddListener(delegate
            {
                try
                {
                    Globals.Instance.StartCoroutine(FullCardSpriteOutputCo(false, btnCardImageExport));
                    btnCardImageExport.ButtonText.text = "Card Image Export";
                }
                catch (Exception e)
                {
                    LogError("Failed to create card images: " + e.Message);
                    btnCardImageExport.ButtonText.text = "Card Image Export";
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnCardImageExport, "Error! Please see LogOutput.log!"));
                };
            });

            // Tome of Knowledge Discord bot data export
            ButtonRef btnToKDataExport = UIFactory.CreateButton(devToolsScrollContent, "btnToKDataExport", "Tome of Knowledge bot data export");
            UIFactory.SetLayoutElement(btnToKDataExport.Component.gameObject, minWidth: 100, minHeight: 30);
            btnToKDataExport.Component.onClick.AddListener(delegate
            {
                try
                {
                    btnToKDataExport.ButtonText.text = "Exporting data; please wait...";
                    TomeOfKnowledgeExport(true, btnToKDataExport);
                }
                catch (Exception e)
                {
                    LogError("Failed to export Tome of Knowledge bot data: " + e.Message);
                    btnToKDataExport.ButtonText.text = "Tome of Knowledge bot data export";
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnToKDataExport, "Error! Please see LogOutput.log!"));
                };
            });

            // Activate Event
            GameObject hActivateEvent = UIFactory.CreateUIObject("hActivateEvent", devToolsScrollContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(hActivateEvent, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            ButtonRef btnActivateEvent = UIFactory.CreateButton(hActivateEvent, "btnActivateEvent", "Activate Event");
            UIFactory.SetLayoutElement(btnActivateEvent.Component.gameObject, minWidth: 150, minHeight: 30);
            inputActivateEvent = UIFactory.CreateInputField(hActivateEvent, "inputActivateEvent", "e_ulmin30_a");
            inputActivateEvent.Component.SetTextWithoutNotify("e_ulmin30_a");
            UIFactory.SetLayoutElement(inputActivateEvent.Component.gameObject, minWidth: 150, minHeight: 30);
            btnActivateEvent.Component.onClick.AddListener(delegate
            {
                string eventID = inputActivateEvent.Text.Trim();
                if (MapManager.Instance == null)
                {
                    LogError("Failed to activate event " + eventID + ": MapManager not active!");
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnActivateEvent, "Error! MapManager is not active!"));
                }
                else if (!Globals.Instance.Events.ContainsKey(eventID))
                {
                    LogError("Failed to activate event " + eventID + ": event does not exist!");
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnActivateEvent, "Error! Event does not exist!"));
                }
                else
                {
                    try
                    {
                        MapManager.Instance.GetType().GetMethod("DoEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(MapManager.Instance, new object[] { Globals.Instance.GetEventData(eventID) });
                    }
                    catch (Exception e)
                    {
                        LogError("Failed to activate event " + eventID + ": " + e.Message);
                        Globals.Instance.StartCoroutine(medsButtonTextRevert(btnActivateEvent, "Error! Please see LogOutput.log!"));
                    }
                }
            });


            // Activate Combat
            GameObject hActivateCombat = UIFactory.CreateUIObject("hActivateCombat", devToolsScrollContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(hActivateCombat, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            ButtonRef btnActivateCombat = UIFactory.CreateButton(hActivateCombat, "btnActivateCombat", "Activate Combat");
            UIFactory.SetLayoutElement(btnActivateCombat.Component.gameObject, minWidth: 150, minHeight: 30);
            inputActivateCombat = UIFactory.CreateInputField(hActivateCombat, "inputActivateCombat", "esen_16a");
            inputActivateCombat.Component.SetTextWithoutNotify("esen_16a");
            UIFactory.SetLayoutElement(inputActivateCombat.Component.gameObject, minWidth: 150, minHeight: 30);
            btnActivateCombat.Component.onClick.AddListener(delegate
            {
                string combatID = inputActivateCombat.Text.Trim();
                CombatData combat = Globals.Instance.GetCombatData(combatID);
                if (MapManager.Instance == null)
                {
                    LogError("Failed to activate combat " + combatID + ": MapManager not active!");
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnActivateCombat, "Error! MapManager is not active!"));
                }
                else if (combat == null)
                {
                    LogError("Failed to activate combat " + combatID + ": combat does not exist!");
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnActivateCombat, "Error! Combat does not exist!"));
                }
                else
                {
                    try
                    {
                        AtOManager.Instance.LaunchCombat(combat);
                    }
                    catch (Exception e)
                    {
                        LogError("Failed to activate combat " + combatID + ": " + e.Message);
                        Globals.Instance.StartCoroutine(medsButtonTextRevert(btnActivateCombat, "Error! Please see LogOutput.log!"));
                    }
                }
            });

            // caravan shop logger
            ButtonRef btnCaravanShopLog = UIFactory.CreateButton(devToolsScrollContent, "btnCaravanShopLog", "Write Caravan Shop to Log");
            UIFactory.SetLayoutElement(btnCaravanShopLog.Component.gameObject, minWidth: 150, minHeight: 30);
            btnCaravanShopLog.Component.onClick.AddListener(delegate
            {
                try
                {
                    LogShopItems();
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnCaravanShopLog, "Caravan items written to log!"));
                }
                catch (Exception e)
                {
                    Globals.Instance.StartCoroutine(medsButtonTextRevert(btnCaravanShopLog, "Error getting caravan shop items: " + e.Message));
                }
            });
            
            // Profile Editor
            btnProfileEditor = UIFactory.CreateButton(devToolsScrollContent, "btnProfileEditor", "Profile Editor");
            UIFactory.SetLayoutElement(btnProfileEditor.Component.gameObject, minWidth: 100, minHeight: 30);
            RuntimeHelper.SetColorBlock(btnProfileEditor.Component, UniversalUI.DisabledButtonColor, UniversalUI.DisabledButtonColor * 1.2f);
            btnProfileEditor.Component.onClick.AddListener(delegate
            {
                try
                {
                    ProfileEditor.ShowUI = !ProfileEditor.ShowUI;
                }
                catch (Exception e) { LogDebug("Failed to open profile editor: " + e.Message); };
            });

            // Calculate Checksums
            ButtonRef btnCalculateChecksums = UIFactory.CreateButton(devToolsScrollContent, "btnCalculateChecksums", "Calculate Checksums");
            UIFactory.SetLayoutElement(btnCalculateChecksums.Component.gameObject, minWidth: 100, minHeight: 30);
            btnCalculateChecksums.Component.onClick.AddListener(delegate
            {
                try
                {
                    btnCalculateChecksums.ButtonText.text = "Please wait...";
                    CalculateChecksums();
                    btnCalculateChecksums.ButtonText.text = "Done! config\\CHECKSUMS.txt";
                }
                catch (Exception e) { LogDebug("Failed to calculate checksums: " + e.Message); };
            });

            // Calculate Checksums (FALO ROWI EDITION)
            ButtonRef btnCalculateChecksumsFaloRowi = UIFactory.CreateButton(devToolsScrollContent, "btnCalculateChecksumsFaloRowi", "Checksums (FALO ROWI EDITION)");
            UIFactory.SetLayoutElement(btnCalculateChecksumsFaloRowi.Component.gameObject, minWidth: 100, minHeight: 30);
            btnCalculateChecksumsFaloRowi.Component.onClick.AddListener(delegate
            {
                try
                {
                    btnCalculateChecksumsFaloRowi.ButtonText.text = "Please wait...";
                    CalculateChecksums(true);
                    btnCalculateChecksumsFaloRowi.ButtonText.text = "Done! config\\CHECKSUMS" + (GameManager.Instance != null && GameManager.Instance.IsMultiplayer() ? "_MP" : "") + ".txt";
                }
                catch (Exception e) { LogDebug("Failed to calculate checksums (FALO ROWI EDITION): " + e.Message); };
            });

            // Disable AtO Buttons
            GameObject hDisableAtOButtons = UIFactory.CreateUIObject("hDisableAtOButtons", medsDevToolsGO);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(hDisableAtOButtons, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleCenter);
            lockAtOGO = UIFactory.CreateToggle(hDisableAtOButtons, "disableButtonsToggle", out lockAtOToggle, out Text lockAtOText);
            lockAtOText.text = "Disable AtO Buttons";
            lockAtOToggle.isOn = false;
            UIFactory.SetLayoutElement(lockAtOGO, minWidth: 85, minHeight: 30);

            Canvas.ForceUpdateCanvases();
        }
        internal static System.Collections.IEnumerator medsButtonTextRevert(ButtonRef _btn, string _tempText = "Done!", float _time = 5f)
        {
            string oldText = _btn.ButtonText.text;
            _btn.ButtonText.text = _tempText;
            yield return Globals.Instance.WaitForSeconds(_time);
            _btn.ButtonText.text = oldText;
            yield return null;
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
                labelMouseXY.text = "Mouse x: " + newPos.x.ToString("0.0000") + " | y: " + newPos.y.ToString("0.0000");
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
        internal static ButtonRef btnAllHeroesLockUnlock;
        internal static InputFieldRef inputAllHeroesRank;
        internal static InputFieldRef inputAllHeroesExperience;
        internal static InputFieldRef inputSupplies;
        internal static InputFieldRef inputAdventureMadness;
        internal static InputFieldRef inputAdventureMadness2;
        internal static InputFieldRef inputObeliskMadness;
        internal static GameObject profileScroll;
        internal static GameObject profileScrollContent;
        internal static Dictionary<string, ButtonRef> btnCardsLockUnlock = new();
        internal static Dictionary<string, GameObject> GOCardsCategories = new();
        internal static Dictionary<string, ButtonRef> btnCardsViewCategories = new();
        internal static Dictionary<string, List<string>> cardsByCategory = new();
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

            Text labelSupplies = UIFactory.CreateLabel(horizontalSupplies, "labelSupplies", "Supplies");
            UIFactory.SetLayoutElement(labelSupplies.gameObject, minWidth: 60, minHeight: 30);

            inputSupplies = UIFactory.CreateInputField(horizontalSupplies, "inputSupplies", "0-999");
            UIFactory.SetLayoutElement(inputSupplies.Component.gameObject, minWidth: 40, minHeight: 30);
            inputSupplies.Component.textComponent.alignment = TextAnchor.MiddleRight;
            inputSupplies.Component.characterValidation = InputField.CharacterValidation.Integer;

            // heroes 
            // create container + accordion
            GameObject GOHeroes = UIFactory.CreateUIObject("medsHeroes", profileScrollContent);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(GOHeroes, true, false, true, true, 5, 2, 2, 0, 0, TextAnchor.UpperLeft);
            ButtonRef btnViewHeroes = UIFactory.CreateButton(GOHeroes, "btnViewHeroes", "Show Heroes");
            UIFactory.SetLayoutElement(btnViewHeroes.Component.gameObject, minHeight: 25, minWidth: 100);
            GameObject GOHeroContent = UIFactory.CreateUIObject("medsHeroContent", GOHeroes);
            btnViewHeroes.ButtonText.fontStyle = FontStyle.Bold;
            btnViewHeroes.Component.onClick.AddListener(delegate
            {
                btnViewHeroes.ButtonText.text = GOHeroContent.gameObject.activeSelf ? "Show Heroes" : "Hide Heroes";
                Color color = GOHeroContent.gameObject.activeSelf ? UniversalUI.DisabledButtonColor : new Color(0.2f, 0.28f, 0.4f);
                RuntimeHelper.SetColorBlock(btnViewHeroes.Component, color, color * 1.2f);
                GOHeroContent.gameObject.SetActive(!GOHeroContent.gameObject.activeSelf);
            });
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOHeroContent, true, false, true, true, 5, 2, 2, 0, 0, TextAnchor.UpperLeft);

            // create columns
            Dictionary<int, GameObject> heroColumns = new();
            for (int b = 0; b < 3; b++)
            {
                heroColumns[b] = UIFactory.CreateUIObject("medsHeroesColumn" + b.ToString(), GOHeroContent);
                UIFactory.SetLayoutGroup<VerticalLayoutGroup>(heroColumns[b], true, false, true, true, 3, 0, 0, 0, 0, TextAnchor.UpperLeft);
            }
            // all heroes
            // locked/unlocked
            GameObject GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontalAllHeroesUnlock", heroColumns[0]);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleCenter);
            btnAllHeroesLockUnlock = UIFactory.CreateButton(GOSCDHorizontal, "btnAllHeroesLockUnlock", "Unlock All");
            UIFactory.SetLayoutElement(btnAllHeroesLockUnlock.Component.gameObject, minHeight: 25, minWidth: 90);
            btnAllHeroesLockUnlock.Component.onClick.AddListener(AllHeroesLockUnlock);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllHeroesUnlock", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 1, minHeight: 30);

            // hero rank
            GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontalAllHeroesRank", heroColumns[1]);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleCenter);
            ButtonRef btnAllHeroesSetRank = UIFactory.CreateButton(GOSCDHorizontal, "btnAllHeroesSetRank", "Set All");
            UIFactory.SetLayoutElement(btnAllHeroesSetRank.Component.gameObject, minHeight: 25, minWidth: 100);
            btnAllHeroesSetRank.Component.onClick.AddListener(AllHeroesRankXPApply);
            Text rank = UIFactory.CreateLabel(GOSCDHorizontal, "labelRankAll", "Rank:");
            UIFactory.SetLayoutElement(rank.gameObject, minWidth: 40, minHeight: 30);
            rank.alignment = TextAnchor.MiddleRight;
            inputAllHeroesRank = UIFactory.CreateInputField(GOSCDHorizontal, "inputAllHeroesRank", "");
            inputAllHeroesRank.Component.textComponent.alignment = TextAnchor.MiddleRight;
            inputAllHeroesRank.Component.characterValidation = InputField.CharacterValidation.Integer;
            inputAllHeroesRank.Component.SetTextWithoutNotify("50");
            inputAllHeroesRank.Component.onValueChanged.AddListener((newRank) => { AllHeroesRankUpdate(newRank); });
            UIFactory.SetLayoutElement(inputAllHeroesRank.Component.gameObject, minWidth: 22, minHeight: 25);

            // hero experience
            Text xp = UIFactory.CreateLabel(GOSCDHorizontal, "labelXPAll", "XP:");
            UIFactory.SetLayoutElement(xp.gameObject, minWidth: 30, minHeight: 30);
            xp.alignment = TextAnchor.MiddleRight;
            inputAllHeroesExperience = UIFactory.CreateInputField(GOSCDHorizontal, "inputAllHeroesExperience", "");
            inputAllHeroesExperience.Component.textComponent.alignment = TextAnchor.MiddleRight;
            inputAllHeroesExperience.Component.characterValidation = InputField.CharacterValidation.Integer;
            inputAllHeroesExperience.Component.SetTextWithoutNotify("95500");
            inputAllHeroesExperience.Component.onValueChanged.AddListener((newXP) => { AllHeroesXPUpdate(newXP); });
            UIFactory.SetLayoutElement(inputAllHeroesExperience.Component.gameObject, minWidth: 50, minHeight: 25);

            GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontalAllSpacer", heroColumns[2]);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleLeft);
            spacer = UIFactory.CreateLabel(GOSCDHorizontal, "spacerAllHeroes", " ");
            UIFactory.SetLayoutElement(spacer.gameObject, minWidth: 100, minHeight: 30);

            // individual heroes
            int a = 0;
            foreach (SubClassData scd in Globals.Instance.SubClass.Values)
            {
                if (!scd.MainCharacter || scd.Id == "medsdlcone" || scd.Id == "medsdlctwo" || scd.Id == "medsdlcthree" || scd.Id == "medsdlcfour")
                    continue;
                GOSCDHorizontal = UIFactory.CreateUIObject("medsSCDHorizontal" + scd.Id, heroColumns[a % 3]);
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOSCDHorizontal, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleCenter);
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
                UIFactory.SetLayoutElement(inputHeroesRank[scd.Id].Component.gameObject, minWidth: 22, minHeight: 25);

                // hero experience
                xp = UIFactory.CreateLabel(GOSCDHorizontal, "labelXP" + scd.Id, "XP:");
                UIFactory.SetLayoutElement(xp.gameObject, minWidth: 30, minHeight: 30);
                xp.alignment = TextAnchor.MiddleRight;
                inputHeroesExperience[scd.Id] = UIFactory.CreateInputField(GOSCDHorizontal, "medsHeroesExperience" + scd.Id, "");
                inputHeroesExperience[scd.Id].Component.textComponent.alignment = TextAnchor.MiddleRight;
                inputHeroesExperience[scd.Id].Component.characterValidation = InputField.CharacterValidation.Integer;
                inputHeroesExperience[scd.Id].Component.onValueChanged.AddListener((newXP) => { HeroXPUpdate(scd.Id, newXP); });
                UIFactory.SetLayoutElement(inputHeroesExperience[scd.Id].Component.gameObject, minWidth: 50, minHeight: 25);
                a++;
            }
            GOHeroContent.gameObject.SetActive(false);


            // cards 
            int maxColumns = 4;
            // ensure order
            cardsByCategory["Warrior"] = new();
            cardsByCategory["Scout"] = new();
            cardsByCategory["Mage"] = new();
            cardsByCategory["Healer"] = new();
            //cardsByCategory["Special"] = new();
            cardsByCategory["Boon"] = new();
            cardsByCategory["Injury"] = new();
            cardsByCategory["Weapon"] = new();
            cardsByCategory["Armor"] = new();
            cardsByCategory["Jewelry"] = new();
            cardsByCategory["Accesory"] = new();
            cardsByCategory["Pet"] = new();

            List<CardClass> acceptedCardClasses = new() { CardClass.Warrior, CardClass.Scout, CardClass.Mage, CardClass.Healer, CardClass.Boon, CardClass.Injury, CardClass.Item };
            /*
            skip if not:
                warrior, scout, mage, healer;
                boon or injury; or
                item.
            */
            foreach (CardData card in Globals.Instance.Cards.Values)
            {
                if (card.CardUpgraded != CardUpgraded.No || !acceptedCardClasses.Contains(card.CardClass))
                    continue;
                if (card.CardClass == CardClass.Item)
                    cardsByCategory[DataTextConvert.ToString(card.CardType)].Add(card.CardName + "|" + card.Id);
                else
                    cardsByCategory[DataTextConvert.ToString(card.CardClass)].Add(card.CardName + "|" + card.Id);
            }

            GameObject GOCards = UIFactory.CreateUIObject("medsCards", profileScrollContent);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(GOCards, true, false, true, true, 5, 2, 2, 0, 0, TextAnchor.UpperLeft);
            btnCardsViewCategories["All"] = UIFactory.CreateButton(GOCards, "btnViewCardsAll", "Show Cards");
            UIFactory.SetLayoutElement(btnCardsViewCategories["All"].Component.gameObject, minHeight: 25, minWidth: 100);
            GOCardsCategories["All"] = UIFactory.CreateUIObject("medsCardsVerticalAll", GOCards);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(GOCardsCategories["All"], true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.UpperCenter);
            btnCardsViewCategories["All"].ButtonText.fontStyle = FontStyle.Bold;
            btnCardsViewCategories["All"].Component.onClick.AddListener(delegate
            {
                btnCardsViewCategories["All"].ButtonText.text = GOCardsCategories["All"].gameObject.activeSelf ? "Show Cards" : "Hide Cards";
                Color color = GOCardsCategories["All"].gameObject.activeSelf ? UniversalUI.DisabledButtonColor : new Color(0.2f, 0.28f, 0.4f);
                RuntimeHelper.SetColorBlock(btnCardsViewCategories["All"].Component, color, color * 1.2f);
                GOCardsCategories["All"].gameObject.SetActive(!GOCardsCategories["All"].gameObject.activeSelf);
            });
            // all cards
            GameObject GOCardsHorizontal = UIFactory.CreateUIObject("medsCardsHorizontalAll", GOCardsCategories["All"]);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOCardsHorizontal, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleCenter);
            // #TODO: lock/unlock all
            btnCardsLockUnlock["All"] = UIFactory.CreateButton(GOCardsHorizontal, "btnAllCardsLockUnlock", "Unlock All Cards");
            UIFactory.SetLayoutElement(btnCardsLockUnlock["All"].Component.gameObject, minHeight: 25, minWidth: 200);
            btnCardsLockUnlock["All"].Component.onClick.AddListener(delegate { CardsLockUnlock("All"); });



            // GameObject GOCardsColumn = UIFactory.CreateUIObject
            /*
            IS IT actually a better idea to have three/four equal-width columns containing "cells" (HorizontalGroup containing individual card)

            current: vertical (cards) containing horizontals (rows) containing horizontals (individuals), needs that hacky workaround to left-align end items
            wouldbe: vertical (cards) containing verticals (columns) containing horizontals (individuals), probably still need hacky workaround for any cols with less items? UNLESS you _don't_ allow vertical resize (which may already be the case - the false in t/f/t/t for SetLayoutGroup?) and UpperCenter align? 
            so worth doing IF hacky workaround not required. Maybe give it a bit more detailed think 

            */
            foreach (string cardCategory in cardsByCategory.Keys)
            {
                cardsByCategory[cardCategory].Sort();
                // make category accordion
                btnCardsViewCategories[cardCategory] = UIFactory.CreateButton(GOCardsCategories["All"], "btnViewCards" + cardCategory, "Show " + cardCategory.Replace("Accesory", "Accessory") + " Cards");
                UIFactory.SetLayoutElement(btnCardsViewCategories[cardCategory].Component.gameObject, minHeight: 25, minWidth: 200);
                GOCardsCategories[cardCategory] = UIFactory.CreateUIObject("medsCardsCategory" + cardCategory, GOCardsCategories["All"]);
                btnCardsViewCategories[cardCategory].ButtonText.fontStyle = FontStyle.Bold;
                btnCardsViewCategories[cardCategory].Component.onClick.AddListener(delegate
                {
                    btnCardsViewCategories[cardCategory].ButtonText.text = GOCardsCategories[cardCategory].gameObject.activeSelf ? "Show " + cardCategory.Replace("Accesory", "Accessory") + " Cards" : "Hide " + cardCategory.Replace("Accesory", "Accessory") + " Cards";
                    Color color = GOCardsCategories[cardCategory].gameObject.activeSelf ? UniversalUI.DisabledButtonColor : new Color(0.2f, 0.28f, 0.4f);
                    RuntimeHelper.SetColorBlock(btnCardsViewCategories[cardCategory].Component, color, color * 1.2f);
                    GOCardsCategories[cardCategory].gameObject.SetActive(!GOCardsCategories[cardCategory].gameObject.activeSelf);
                });
                UIFactory.SetLayoutGroup<VerticalLayoutGroup>(GOCardsCategories[cardCategory], true, false, true, true, 3, 0, 0, 0, 0, TextAnchor.UpperCenter);
                // lock/unlock entire category
                GOCardsHorizontal = UIFactory.CreateUIObject("medsCards" + cardCategory + "HorizontalAll", GOCardsCategories[cardCategory]);
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOCardsHorizontal, false, false, true, true, 5, 0, 0, 0, 0, TextAnchor.MiddleCenter);
                btnCardsLockUnlock[cardCategory] = UIFactory.CreateButton(GOCardsHorizontal, "btn" + cardCategory + "CardsLockUnlock", "Unlock All " + cardCategory.Replace("Accesory", "Accessory") + " Cards");
                UIFactory.SetLayoutElement(btnCardsLockUnlock[cardCategory].Component.gameObject, minHeight: 25, minWidth: 200);
                btnCardsLockUnlock[cardCategory].Component.onClick.AddListener(delegate { CardsLockUnlock(cardCategory); });

                // container for columns of individual cards
                GOCardsHorizontal = UIFactory.CreateUIObject("medsCards" + cardCategory + "Horizontal", GOCardsCategories[cardCategory]);
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(GOCardsHorizontal, true, false, true, true, 5, 0, 0, 0, 0, TextAnchor.UpperCenter);
                // make four? columns
                Dictionary<int, GameObject> cardColumns = new();
                for (int b = 0; b < maxColumns; b++)
                {
                    cardColumns[b] = UIFactory.CreateUIObject("medsCards" + cardCategory + "Column" + b.ToString(), GOCardsHorizontal);
                    UIFactory.SetLayoutGroup<VerticalLayoutGroup>(cardColumns[b], true, false, true, true, 3, 0, 0, 0, 0, TextAnchor.UpperLeft);
                }
                a = 0;
                foreach (string sCard in cardsByCategory[cardCategory])
                {
                    string cardName = sCard.Split("|")[0];
                    string cardID = sCard.Split("|")[1];
                    // make individual card item
                    GameObject GOCardUnlocked = UIFactory.CreateToggle(cardColumns[a % maxColumns], "medsCardUnlockedToggle" + cardID, out Toggle toggleTemp, out Text toggleText);
                    toggleCardsUnlocked[cardID] = toggleTemp;
                    toggleText.text = cardName;
                    toggleText.fontStyle = FontStyle.Bold;
                    UIFactory.SetLayoutElement(GOCardUnlocked, minWidth: 100);
                    toggleCardsUnlocked[cardID].onValueChanged.AddListener(delegate { CardLockUnlock(cardID); });
                    a++;
                }
                GOCardsCategories[cardCategory].gameObject.SetActive(false);
            }
            GOCardsCategories["All"].gameObject.SetActive(false);


            profileScrollContent.SetActive(false);
            LogDebug("Profile Editor ConstructPanelContent end");
        }
        internal static void CardsLockUnlock(string _category)
        {
            if (_category == "All")
            {
                foreach (string cardID in toggleCardsUnlocked.Keys)
                    toggleCardsUnlocked[cardID].SetIsOnWithoutNotify(btnCardsLockUnlock["All"].ButtonText.text != "Lock All Cards");
                btnCardsLockUnlock["All"].ButtonText.text = btnCardsLockUnlock["All"].ButtonText.text == "Lock All Cards" ? "Unlock All Cards" : "Lock All Cards";
            }
            else
            {
                foreach (string sCard in cardsByCategory[_category])
                    toggleCardsUnlocked[sCard.Split("|")[1]].SetIsOnWithoutNotify(btnCardsLockUnlock[_category].ButtonText.text != "Lock All " + _category.Replace("Accesory", "Accessory") + " Cards");
                btnCardsLockUnlock[_category].ButtonText.text = btnCardsLockUnlock[_category].ButtonText.text == "Lock All " + _category.Replace("Accesory", "Accessory") + " Cards" ? "Unlock All " + _category.Replace("Accesory", "Accessory") + " Cards" : "Lock All " + _category.Replace("Accesory", "Accessory") + " Cards";
            }
        }
        internal static void CardLockUnlock(string _id)
        {
            // ... don't actually need to do anything?
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
            LogInfo("LOADING PLAYER PROFILE!");
            inputSupplies.Component.SetTextWithoutNotify(PlayerManager.Instance.SupplyActual.ToString());
            btnAllHeroesLockUnlock.ButtonText.text = "Lock All";
            foreach (string scID in toggleHeroesUnlocked.Keys)
            {
                bool unlocked = PlayerManager.Instance.IsHeroUnlocked(scID);
                LogInfo("Hero " + scID + (unlocked ? " is unlocked" : " is locked"));
                toggleHeroesUnlocked[scID].SetIsOnWithoutNotify(unlocked);
                inputHeroesRank[scID].Component.SetTextWithoutNotify(PlayerManager.Instance.GetPerkRank(scID).ToString());
                inputHeroesExperience[scID].Component.SetTextWithoutNotify(PlayerManager.Instance.GetProgress(scID).ToString());
                inputHeroesRank[scID].Component.interactable = unlocked;
                inputHeroesExperience[scID].Component.interactable = unlocked;
                if (!unlocked)
                    btnAllHeroesLockUnlock.ButtonText.text = "Unlock All";
            }
            btnCardsLockUnlock["All"].ButtonText.text = "Lock All Cards";
            foreach(string cardCategory in cardsByCategory.Keys)
            {
                btnCardsLockUnlock[cardCategory].ButtonText.text = "Lock All " + cardCategory.Replace("Accesory", "Accessory") + " Cards";
                foreach (string sCard in cardsByCategory[cardCategory])
                {
                    string cardID = sCard.Split("|")[1];
                    bool unlocked = PlayerManager.Instance.IsCardUnlocked(cardID);
                    LogInfo("Card " + cardID + (unlocked ? " is unlocked" : " is locked"));
                    toggleCardsUnlocked[cardID].SetIsOnWithoutNotify(unlocked);
                    if (!unlocked && btnCardsLockUnlock["All"].ButtonText.text == "Lock All Cards")
                        btnCardsLockUnlock["All"].ButtonText.text = "Unlock All Cards";
                    if (!unlocked && btnCardsLockUnlock[cardCategory].ButtonText.text == "Lock All " + cardCategory.Replace("Accesory", "Accessory") + " Cards")
                        btnCardsLockUnlock[cardCategory].ButtonText.text = "Unlock All " + cardCategory.Replace("Accesory", "Accessory") + " Cards";
                }
            }
            profileScrollContent.SetActive(true);
        }
        internal static void SavePlayerProfile()
        {
            LogInfo("SAVING PROFILE");
            if (int.TryParse(inputSupplies.Text, out int newSupplies))
                PlayerManager.Instance.SupplyActual = newSupplies;
            List<string> unlockedHeroes = new();
            foreach (string scID in toggleHeroesUnlocked.Keys)
            {
                if (toggleHeroesUnlocked[scID].isOn)
                    unlockedHeroes.Add(scID);
                if (int.TryParse(inputHeroesExperience[scID].Text, out int newXP))
                    PlayerManager.Instance.HeroProgress[scID] = newXP;
            }
            PlayerManager.Instance.UnlockedHeroes = unlockedHeroes;
            LogInfo("Unlocked heroes: " + string.Join(", ", unlockedHeroes));
            List<string> unlockedCards = new();
            foreach (string cardID in toggleCardsUnlocked.Keys)
                if (toggleCardsUnlocked[cardID].isOn)
                    unlockedCards.Add(cardID);
            PlayerManager.Instance.UnlockedCards = unlockedCards;
            LogInfo("Unlocked cards: " + string.Join(", ", unlockedCards));
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