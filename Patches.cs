using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Obeliskial_Essentials.Essentials;
using TMPro;
using Photon.Pun;
using System.Collections;
using System;
using static UnityEngine.Object;
using Photon.Realtime;
using System.Reflection;
using UnityEngine.InputSystem;
using System.Text;

/*
FULL LIST OF ATO CLASSES->METHODS THAT ARE PATCHED:
    MainMenuManager
        Start
    Globals
        CreateGameContent
*/

namespace Obeliskial_Essentials
{
    [HarmonyPatch]
    internal class Patches
    {
        private static PhotonView photonView;
        private static int ngValue;
        private static int ngValueMaster;
        private static string ngCorruptors;
        private static int obeliskMadnessValue;
        private static int obeliskMadnessValueMaster;
        private static BoxSelection[] boxSelection;
        private static Dictionary<GameObject, HeroSelection> boxHero = new();
        private static Dictionary<GameObject, bool> boxFilled = new();
        private static Dictionary<string, SubClassData[]> subclassDictionary = new();
        private static Dictionary<string, SubClassData> nonHistorySubclassDictionary = new();
        private static Dictionary<string, string> SubclassByName = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Globals), "CreateGameContent")]
        [HarmonyPriority(Priority.First)]
        private static void CreateGameContentPostfix()
        {
            LogDebug("Essentials CreateGameContent Postfix");
            // set up for Drop-Only Items, All The Pets
            Dictionary<string, CardData> medsCardsSource = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            foreach (string cardID in medsCardsSource.Keys)
            {
                CardData card = medsCardsSource[cardID];
                if (card.CardType == Enums.CardType.Pet && card.Item != null && (card.Item.QuestItem || card.ShowInTome))
                    if (!medsAllThePetsCards.Contains(cardID))
                        medsAllThePetsCards.Add(cardID);
                if (card.CardClass == Enums.CardClass.Item && card.CardType != Enums.CardType.Pet && card.Item != null && card.Item.DropOnly)
                    if (!medsDropOnlyItems.Contains(card.Item.Id))
                        medsDropOnlyItems.Add(card.Item.Id);
            }
            medsBasicCardListByType = Globals.Instance.CardListByType;
            medsBasicCardListByClass = Globals.Instance.CardListByClass;
            medsBasicCardListNotUpgraded = Globals.Instance.CardListNotUpgraded;
            medsBasicCardListNotUpgradedByClass = Globals.Instance.CardListNotUpgradedByClass;
            medsBasicCardListByClassType = Globals.Instance.CardListByClassType;
            medsBasicCardItemByType = Globals.Instance.CardItemByType;

            // export content
            if (medsExportJSON.Value)
            {
                LogInfo("PRAYGE; THE EXPORT HAS BEGUN");
                Node[] foundNodes = Resources.FindObjectsOfTypeAll<Node>();
                foreach (Node n in foundNodes)
                    medsNodeSource[n.name] = n;
                FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "sprite"));
                FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "card"));
                FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined"));

                Dictionary<string, NodeData> medsNodeDataSource = Traverse.Create(Globals.Instance).Field("_NodeDataSource").GetValue<Dictionary<string, NodeData>>();

                string fullList = "id\tname\tclass\n";
                foreach (KeyValuePair<string, CardData> kvp in medsCardsSource)
                    fullList += kvp.Key + "\t" + kvp.Value.CardName + "\t" + DataTextConvert.ToString(kvp.Value.CardClass) + "\t" + kvp.Value.CardUpgraded + "\n";
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "cardlist.json"), fullList);
                medsNodeEvent = new();
                medsNodeEventPercent = new();
                medsNodeEventPriority = new();
                LogDebug("building node-event relationships");
                foreach (KeyValuePair<string, NodeData> kvp in medsNodeDataSource)
                {
                    for (int a = 0; a < kvp.Value.NodeEvent.Length; a++)
                    {
                        medsNodeEvent[kvp.Value.NodeEvent[a].EventId] = kvp.Key;
                        medsNodeEventPercent[kvp.Value.NodeEvent[a].EventId] = kvp.Value.NodeEventPercent.Length > a ? kvp.Value.NodeEventPercent[a] : 100;
                        medsNodeEventPriority[kvp.Value.NodeEvent[a].EventId] = kvp.Value.NodeEventPriority.Length > a ? kvp.Value.NodeEventPriority[a] : 0;
                    }
                }
                LogDebug("finished building node-event relationships");
                ExtractData(Traverse.Create(Globals.Instance).Field("_SubClassSource").GetValue<Dictionary<string, SubClassData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_TraitsSource").GetValue<Dictionary<string, TraitData>>().Select(item => item.Value).ToArray());
                ExtractData(medsCardsSource.Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_PerksSource").GetValue<Dictionary<string, PerkData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_AurasCursesSource").GetValue<Dictionary<string, AuraCurseData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_NPCsSource").GetValue<Dictionary<string, NPCData>>().Select(item => item.Value).ToArray());
                ExtractData(medsNodeDataSource.Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_LootDataSource").GetValue<Dictionary<string, LootData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_PerksNodesSource").GetValue<Dictionary<string, PerkNodeData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_WeeklyDataSource").GetValue<Dictionary<string, ChallengeData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_ChallengeTraitsSource").GetValue<Dictionary<string, ChallengeTrait>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_CombatDataSource").GetValue<Dictionary<string, CombatData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_Events").GetValue<Dictionary<string, EventData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_Requirements").GetValue<Dictionary<string, EventRequirementData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_ZoneDataSource").GetValue<Dictionary<string, ZoneData>>().Select(item => item.Value).ToArray());
                ExtractData(Globals.Instance.KeyNotes.Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_PackDataSource").GetValue<Dictionary<string, PackData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_CardPlayerPackDataSource").GetValue<Dictionary<string, CardPlayerPackData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_ItemDataSource").GetValue<Dictionary<string, ItemData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_CardbackDataSource").GetValue<Dictionary<string, CardbackData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_SkinDataSource").GetValue<Dictionary<string, SkinData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_CorruptionPackDataSource").GetValue<Dictionary<string, CorruptionPackData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_Cinematics").GetValue<Dictionary<string, CinematicData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_TierRewardDataSource").GetValue<Dictionary<int, TierRewardData>>().Select(item => item.Value).ToArray());
                ExtractData(Traverse.Create(Globals.Instance).Field("_CardPlayerPairsPackDataSource").GetValue<Dictionary<string, CardPlayerPairsPackData>>().Select(item => item.Value).ToArray());
                ExtractData(medsEventReplyDataText.Select(item => item.Value).ToArray());
                //Plugin.FullNodeDataExport();
                medsExportJSON.Value = false; // turn off after exporting*/
                LogInfo("OUR PRAYERS WERE ANSWERED");
            }

        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HeroSelectionManager), "Start")]
        private static bool HSMStartPrefix(ref HeroSelectionManager __instance)
        {
            // temp logging for @Sara's issue
            LogDebug("HSMStartPrefix");
            Dictionary<string, string> cardbackUsed = Traverse.Create(PlayerManager.Instance).Field("cardbackUsed").GetValue<Dictionary<string, string>>();
            Dictionary<string, string> skinUsed = Traverse.Create(PlayerManager.Instance).Field("skinUsed").GetValue<Dictionary<string, string>>();
            foreach (string key in cardbackUsed.Keys)
                LogDebug("cardback for " + key + ": " + cardbackUsed[key]);
            foreach (string key in skinUsed.Keys)
                LogDebug("skin for " + key + ": " + skinUsed[key]);

            // replace StartCo with your own... :)
            photonView = Traverse.Create(__instance).Field("photonView").GetValue<PhotonView>();
            ngValueMaster = Traverse.Create(__instance).Field("ngValueMaster").GetValue<int>();
            ngValue = Traverse.Create(__instance).Field("ngValue").GetValue<int>();
            ngCorruptors = Traverse.Create(__instance).Field("ngCorruptors").GetValue<string>();
            obeliskMadnessValue = Traverse.Create(__instance).Field("obeliskMadnessValue").GetValue<int>();
            obeliskMadnessValueMaster = Traverse.Create(__instance).Field("obeliskMadnessValueMaster").GetValue<int>();
            boxSelection = Traverse.Create(__instance).Field("boxSelection").GetValue<BoxSelection[]>();
            boxHero = Traverse.Create(__instance).Field("boxHero").GetValue<Dictionary<GameObject, HeroSelection>>();
            boxFilled = Traverse.Create(__instance).Field("boxFilled").GetValue<Dictionary<GameObject, bool>>();
            subclassDictionary = Traverse.Create(__instance).Field("subclassDictionary").GetValue<Dictionary<string, SubClassData[]>>();
            nonHistorySubclassDictionary = Traverse.Create(__instance).Field("nonHistorySubclassDictionary").GetValue<Dictionary<string, SubClassData>>();
            SubclassByName = Traverse.Create(__instance).Field("SubclassByName").GetValue<Dictionary<string, string>>();
            __instance.StartCoroutine(medsHeroSelectionStartCo());
            return false;
        }
        private static IEnumerator medsHeroSelectionStartCo()
        {
            ngValueMaster = ngValue = 0;
            ngCorruptors = "";
            obeliskMadnessValue = obeliskMadnessValueMaster = 0;
            Traverse.Create(HeroSelectionManager.Instance).Field("ngValueMaster").SetValue(ngValueMaster);
            Traverse.Create(HeroSelectionManager.Instance).Field("ngValue").SetValue(ngValue);
            Traverse.Create(HeroSelectionManager.Instance).Field("ngCorruptors").SetValue(ngCorruptors);
            Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValue").SetValue(obeliskMadnessValue);
            Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValueMaster").SetValue(obeliskMadnessValueMaster);
            HeroSelectionManager.Instance.madnessLevel.text = string.Format(Texts.Instance.GetText("madnessNumber"), (object)0);
            if (GameManager.Instance.IsMultiplayer())
            {
                Debug.Log((object)"WaitingSyncro heroSelection");
                if (NetworkManager.Instance.IsMaster())
                {
                    NetworkManager.Instance.PlayerSkuList.Clear();
                    while (!NetworkManager.Instance.AllPlayersReady("heroSelection"))
                        yield return (object)Globals.Instance.WaitForSeconds(0.01f);
                    if (Globals.Instance.ShowDebug)
                        LogDebug("Game ready, Everybody checked heroSelection");
                    if (GameManager.Instance.IsLoadingGame())
                        photonView.RPC("NET_SetLoadingGame", RpcTarget.Others);
                    NetworkManager.Instance.PlayersNetworkContinue("heroSelection", AtOManager.Instance.GetWeekly().ToString());
                    yield return (object)Globals.Instance.WaitForSeconds(0.1f);
                }
                else
                {
                    GameManager.Instance.SetGameStatus(Enums.GameStatus.NewGame);
                    NetworkManager.Instance.SetWaitingSyncro("heroSelection", true);
                    NetworkManager.Instance.SetStatusReady("heroSelection");
                    while (NetworkManager.Instance.WaitingSyncro["heroSelection"])
                        yield return (object)Globals.Instance.WaitForSeconds(0.01f);
                    if (NetworkManager.Instance.netAuxValue != "")
                        AtOManager.Instance.SetWeekly(int.Parse(NetworkManager.Instance.netAuxValue));
                    if (Globals.Instance.ShowDebug)
                        LogDebug("heroSelection, we can continue!");
                }
            }
            if (GameManager.Instance.IsMultiplayer())
            {
                List<string> stringList = new List<string>();
                for (int index = 0; index < Globals.Instance.SkuAvailable.Count; ++index)
                {
                    if (SteamManager.Instance.PlayerHaveDLC(Globals.Instance.SkuAvailable[index]))
                        stringList.Add(Globals.Instance.SkuAvailable[index]);
                }
                string str = "";
                if (stringList.Count > 0)
                    str = JsonHelper.ToJson<string>(stringList.ToArray());
                if (NetworkManager.Instance.IsMaster())
                {
                    photonView.RPC("NET_SetSku", RpcTarget.All, (object)NetworkManager.Instance.GetPlayerNick(), (object)str);
                }
                else
                {
                    string roomName = NetworkManager.Instance.GetRoomName();
                    if (roomName != "")
                    {
                        SaveManager.SaveIntoPrefsString("coopRoomId", roomName);
                        SaveManager.SavePrefs();
                    }
                    NetworkManager.Instance.SetWaitingSyncro("skuWait", true);
                    photonView.RPC("NET_SetSku", RpcTarget.All, (object)NetworkManager.Instance.GetPlayerNick(), (object)str);
                }
                Debug.Log((object)"WaitingSyncro skuWait");
                if (NetworkManager.Instance.IsMaster())
                {
                    while (!NetworkManager.Instance.AllPlayersHaveSkuList())
                        yield return (object)Globals.Instance.WaitForSeconds(0.01f);
                    if (Globals.Instance.ShowDebug)
                        LogDebug("Game ready, Everybody checked skuWait");
                    NetworkManager.Instance.PlayersNetworkContinue("skuWait");
                    yield return (object)Globals.Instance.WaitForSeconds(0.1f);
                }
                else
                {
                    while (NetworkManager.Instance.WaitingSyncro["skuWait"])
                        yield return (object)Globals.Instance.WaitForSeconds(0.01f);
                    if (Globals.Instance.ShowDebug)
                        LogDebug("skuWait, we can continue!");
                }
            }
            LogDebug("about to show madness");
            MadnessManager.Instance.ShowMadness();
            MadnessManager.Instance.RefreshValues();
            MadnessManager.Instance.ShowMadness();
            HeroSelectionManager.Instance.playerHeroSkinsDict = new Dictionary<string, string>();
            HeroSelectionManager.Instance.playerHeroCardbackDict = new Dictionary<string, string>();
            boxSelection = new BoxSelection[HeroSelectionManager.Instance.boxGO.Length];
            for (int index = 0; index < HeroSelectionManager.Instance.boxGO.Length; ++index)
            {
                boxHero[HeroSelectionManager.Instance.boxGO[index]] = (HeroSelection)null;
                boxFilled[HeroSelectionManager.Instance.boxGO[index]] = false;
                boxSelection[index] = HeroSelectionManager.Instance.boxGO[index].GetComponent<BoxSelection>();
            }
            Traverse.Create(HeroSelectionManager.Instance).Field("boxSelection").SetValue(boxSelection);
            Traverse.Create(HeroSelectionManager.Instance).Field("boxFilled").SetValue(boxFilled);
            Traverse.Create(HeroSelectionManager.Instance).Field("boxHero").SetValue(boxHero);
            HeroSelectionManager.Instance.ShowDrag(false, Vector3.zero);
            LogDebug("about to begin looping through subclasses");
            int length = 5;
            int num1 = 5;
            foreach (KeyValuePair<string, SubClassData> keyValuePair in Globals.Instance.SubClass)
            {
                LogDebug("kvp: " + keyValuePair.Key);
                if (!keyValuePair.Value.MainCharacter)
                {
                    if (!nonHistorySubclassDictionary.ContainsKey(keyValuePair.Key))
                        nonHistorySubclassDictionary.Add(keyValuePair.Key, Globals.Instance.SubClass[keyValuePair.Key]);
                }
                else if (keyValuePair.Value.IsMultiClass())
                {
                    string key = "dlc";
                    // wouldn't everything just be SO much easier if the subclassdictionary was composed of string, List<string> pairs instead?
                    if (!subclassDictionary.ContainsKey(key))
                        subclassDictionary.Add(key, new SubClassData[length]);
                    if (Globals.Instance.SubClass[keyValuePair.Key].OrderInList >= subclassDictionary[key].Length)
                    {
                        SubClassData[] tempSCD = new SubClassData[Globals.Instance.SubClass[keyValuePair.Key].OrderInList + 1];
                        for (int a = 0; a < subclassDictionary[key].Length; a++)
                        {
                            // Plugin.Log.LogDebug("loop 1." + a);
                            if ((UnityEngine.Object)subclassDictionary[key][a] != (UnityEngine.Object)null)
                                tempSCD[a] = subclassDictionary[key][a];
                        }
                        subclassDictionary[key] = tempSCD;
                    }
                    subclassDictionary[key][Globals.Instance.SubClass[keyValuePair.Key].OrderInList] = Globals.Instance.SubClass[keyValuePair.Key];
                }
                else
                {
                    string key = Enum.GetName(typeof(Enums.HeroClass), (object)Globals.Instance.SubClass[keyValuePair.Key].HeroClass).ToLower().Replace(" ", "");
                    if (!subclassDictionary.ContainsKey(key))
                        subclassDictionary.Add(key, new SubClassData[length]);
                    if (Globals.Instance.SubClass[keyValuePair.Key].OrderInList >= subclassDictionary[key].Length)
                    {
                        SubClassData[] tempSCD = new SubClassData[Globals.Instance.SubClass[keyValuePair.Key].OrderInList + 1];
                        // Plugin.Log.LogDebug("SCDict length: " + subclassDictionary[key].Length + "\ntempSCD length: " + tempSCD.Length);
                        for (int a = 0; a < subclassDictionary[key].Length; a++)
                        {
                            // Plugin.Log.LogDebug("loop 2." + a);
                            if ((UnityEngine.Object)subclassDictionary[key][a] != (UnityEngine.Object)null)
                            {
                                tempSCD[a] = subclassDictionary[key][a];
                                // Plugin.Log.LogDebug("adding subclass " + subclassDictionary[key][a]);
                            }
                        }
                        //Plugin.Log.LogDebug("made it through the loop! original length: ");
                        subclassDictionary[key] = tempSCD;
                    }
                    // Plugin.Log.LogDebug("made it ALL THE WAY through the loop!");
                    // Plugin.Log.LogDebug("NEWLEN: " + subclassDictionary[key].Length);
                    // Plugin.Log.LogDebug(Globals.Instance.SubClass[keyValuePair.Key]);
                    // Plugin.Log.LogDebug("NEWOIL: " + Globals.Instance.SubClass[keyValuePair.Key].OrderInList);

                    subclassDictionary[key][Globals.Instance.SubClass[keyValuePair.Key].OrderInList] = Globals.Instance.SubClass[keyValuePair.Key];
                    // Plugin.Log.LogDebug("NEWOIL: " + Globals.Instance.SubClass[keyValuePair.Key].OrderInList);
                }
                // Plugin.Log.LogDebug("end of subclass loop!");
            }
            LogDebug("finished looping through subclasses");
            Traverse.Create(HeroSelectionManager.Instance).Field("nonHistorySubclassDictionary").SetValue(nonHistorySubclassDictionary);
            Traverse.Create(HeroSelectionManager.Instance).Field("subclassDictionary").SetValue(subclassDictionary);
            HeroSelectionManager.Instance._ClassWarriors.color = Functions.HexToColor(Globals.Instance.ClassColor["warrior"]);
            HeroSelectionManager.Instance._ClassHealers.color = Functions.HexToColor(Globals.Instance.ClassColor["healer"]);
            HeroSelectionManager.Instance._ClassMages.color = Functions.HexToColor(Globals.Instance.ClassColor["mage"]);
            HeroSelectionManager.Instance._ClassScouts.color = Functions.HexToColor(Globals.Instance.ClassColor["scout"]);
            HeroSelectionManager.Instance._ClassMagicKnights.color = Functions.HexToColor(Globals.Instance.ClassColor["magicknight"]);
            float num2 = 0.95f;
            float num3 = 0.55f;
            float num4 = 1.75f;
            float y = -0.65f;
            LogDebug("about to begin looping through subclassDictionary");
            for (int index1 = 0; index1 < 5; ++index1)
            {
                switch (index1)
                {
                    case 0:
                        num1 = subclassDictionary["warrior"].Length;
                        break;
                    case 1:
                        num1 = subclassDictionary["scout"].Length;
                        break;
                    case 2:
                        num1 = subclassDictionary["mage"].Length;
                        break;
                    case 3:
                        num1 = subclassDictionary["healer"].Length;
                        break;
                    case 4:
                        if (subclassDictionary.ContainsKey("dlc"))
                        {
                            num1 = subclassDictionary["dlc"].Length;
                            break;
                        }
                        break;
                }
                //Plugin.Log.LogDebug("index1: " + index1.ToString() + " (num1: " + num1.ToString() + ")");
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    SubClassData _subclassdata = (SubClassData)null;
                    GameObject gameObject1 = (GameObject)null;
                    switch (index1)
                    {
                        case 0:
                            _subclassdata = subclassDictionary["warrior"][index2];
                            gameObject1 = HeroSelectionManager.Instance.warriorsGO;
                            break;
                        case 1:
                            _subclassdata = subclassDictionary["scout"][index2];
                            gameObject1 = HeroSelectionManager.Instance.scoutsGO;
                            break;
                        case 2:
                            _subclassdata = subclassDictionary["mage"][index2];
                            gameObject1 = HeroSelectionManager.Instance.magesGO;
                            break;
                        case 3:
                            _subclassdata = subclassDictionary["healer"][index2];
                            gameObject1 = HeroSelectionManager.Instance.healersGO;
                            break;
                        case 4:
                            if (subclassDictionary.ContainsKey("dlc"))
                            {
                                _subclassdata = subclassDictionary["dlc"][index2];
                                gameObject1 = HeroSelectionManager.Instance.dlcsGO;
                                break;
                            }
                            break;
                    }
                    if (!((UnityEngine.Object)_subclassdata == (UnityEngine.Object)null))
                    {
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(HeroSelectionManager.Instance.heroSelectionPrefab, Vector3.zero, Quaternion.identity, gameObject1.transform);
                        gameObject2.transform.localPosition = new Vector3(num3 + num4 * (float)index2, y, 0.0f);
                        gameObject2.transform.localScale = new Vector3(num2, num2, 1f);
                        gameObject2.name = _subclassdata.SubClassName.ToLower();
                        HeroSelection component = gameObject2.transform.Find("Portrait").transform.GetComponent<HeroSelection>();
                        HeroSelectionManager.Instance.heroSelectionDictionary.Add(gameObject2.name, component);
                        component.blocked = !PlayerManager.Instance.IsHeroUnlocked(_subclassdata.Id);
                        if (component.blocked && GameManager.Instance.IsObeliskChallenge() && !GameManager.Instance.IsWeeklyChallenge())
                            component.blocked = false;
                        /* no longer auto-unlock custom heroes!
                        if (!(Plugin.medsSubclassList.Contains(_subclassdata.Id)))
                            component.blocked = false;*/
                        if (component.blocked && GameManager.Instance.IsWeeklyChallenge())
                        {
                            ChallengeData weeklyData = Globals.Instance.GetWeeklyData(Functions.GetCurrentWeeklyWeek());
                            if ((UnityEngine.Object)weeklyData != (UnityEngine.Object)null && (_subclassdata.Id == weeklyData.Hero1.Id || _subclassdata.Id == weeklyData.Hero2.Id || _subclassdata.Id == weeklyData.Hero3.Id || _subclassdata.Id == weeklyData.Hero4.Id))
                                component.blocked = false;
                        }
                        component.SetSubclass(_subclassdata);
                        //component.SetSprite(_subclassdata.SpriteSpeed, _subclassdata.SpriteBorderSmall, _subclassdata.SpriteBorderLocked);
                        string activeSkin = PlayerManager.Instance.GetActiveSkin(_subclassdata.Id);
                        if (activeSkin != "")
                        {
                            SkinData skinData = Globals.Instance.GetSkinData(activeSkin);
                            if (skinData == (SkinData)null)
                                skinData = Globals.Instance.GetSkinData(Globals.Instance.GetSkinBaseIdBySubclass(_subclassdata.Id));
                            if (skinData == (SkinData)null)
                                Log.LogError("SKINDATA NULL AAAAH");
                            string lower = _subclassdata.Id.ToLower();
                            // this.AddToPlayerHeroSkin(_subclassdata.Id, activeSkin);
                            HeroSelectionManager.Instance.playerHeroSkinsDict[lower] = activeSkin;
                            // end
                            component.SetSprite(skinData.SpritePortrait, skinData.SpriteSilueta, _subclassdata.SpriteBorderLocked);
                        }
                        else
                            component.SetSprite(_subclassdata.SpriteSpeed, _subclassdata.SpriteBorderSmall, _subclassdata.SpriteBorderLocked);
                        component.SetName(_subclassdata.CharacterName);
                        component.Init();
                        if ((UnityEngine.Object)_subclassdata.SpriteBorderLocked != (UnityEngine.Object)null && _subclassdata.SpriteBorderLocked.name == "regularBorderSmall")
                            component.ShowComingSoon();
                        SubclassByName.Add(_subclassdata.Id, _subclassdata.SubClassName);
                        if (GameManager.Instance.IsWeeklyChallenge())
                            component.blocked = true;
                        HeroSelectionManager.Instance.menuController.Add(component.transform);
                    }
                }
            }
            LogDebug("FINISHED looping through subclassDictionary");


            if (GameManager.Instance.IsMultiplayer() && GameManager.Instance.IsLoadingGame())
            {
                foreach (KeyValuePair<string, SubClassData> nonHistorySubclass in nonHistorySubclassDictionary)
                {
                    SubClassData _subclassdata = nonHistorySubclass.Value;
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(HeroSelectionManager.Instance.heroSelectionPrefab, Vector3.zero, Quaternion.identity);
                    gameObject.transform.localPosition = new Vector3(-10f, -10f, 100f);
                    gameObject.name = _subclassdata.SubClassName.ToLower();
                    HeroSelection component = gameObject.transform.Find("Portrait").transform.GetComponent<HeroSelection>();
                    HeroSelectionManager.Instance.heroSelectionDictionary.Add(gameObject.name, component);
                    component.blocked = true;
                    component.SetSubclass(_subclassdata);
                    component.SetSprite(_subclassdata.SpriteSpeed, _subclassdata.SpriteBorderSmall, _subclassdata.SpriteBorderLocked);
                    component.SetName(_subclassdata.CharacterName);
                    component.Init();
                    SubclassByName.Add(_subclassdata.Id, _subclassdata.SubClassName);
                }
            }
            Traverse.Create(HeroSelectionManager.Instance).Field("SubclassByName").SetValue(SubclassByName);
            if (!GameManager.Instance.IsObeliskChallenge() && AtOManager.Instance.IsFirstGame() && !GameManager.Instance.IsMultiplayer())
            {
                AtOManager.Instance.SetGameId("tuto");
                HeroSelectionManager.Instance.heroSelectionDictionary["mercenary"].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[0]);
                HeroSelectionManager.Instance.heroSelectionDictionary["ranger"].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[1]);
                HeroSelectionManager.Instance.heroSelectionDictionary["elementalist"].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[2]);
                HeroSelectionManager.Instance.heroSelectionDictionary["cleric"].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[3]);
                SandboxManager.Instance.DisableSandbox();
                yield return (object)Globals.Instance.WaitForSeconds(1f);
                // #TODO: reflections set all values [but this is only for first game, so should be fine?]
                HeroSelectionManager.Instance.BeginAdventure();
            }
            else
            {
                HeroSelectionManager.Instance.charPopupGO = UnityEngine.Object.Instantiate<GameObject>(HeroSelectionManager.Instance.charPopupPrefab, new Vector3(0.0f, 0.0f, -1f), Quaternion.identity);
                HeroSelectionManager.Instance.charPopup = HeroSelectionManager.Instance.charPopupGO.GetComponent<CharPopup>();
                HeroSelectionManager.Instance.charPopup.HideNow();
                HeroSelectionManager.Instance.separator.gameObject.SetActive(true);
                if (!GameManager.Instance.IsWeeklyChallenge())
                {
                    HeroSelectionManager.Instance.titleGroupDefault.gameObject.SetActive(true);
                    HeroSelectionManager.Instance.titleWeeklyDefault.gameObject.SetActive(false);
                    HeroSelectionManager.Instance.weeklyModifiersButton.gameObject.SetActive(false);
                    HeroSelectionManager.Instance.weeklyT.gameObject.SetActive(false);
                }
                else
                {
                    HeroSelectionManager.Instance.titleGroupDefault.gameObject.SetActive(false);
                    HeroSelectionManager.Instance.titleWeeklyDefault.gameObject.SetActive(true);
                    HeroSelectionManager.Instance.weeklyModifiersButton.gameObject.SetActive(true);
                    HeroSelectionManager.Instance.weeklyT.gameObject.SetActive(true);
                    Traverse.Create(HeroSelectionManager.Instance).Field("setWeekly").SetValue(true);
                    if (!GameManager.Instance.IsLoadingGame())
                        AtOManager.Instance.SetWeekly(Functions.GetCurrentWeeklyWeek());
                    HeroSelectionManager.Instance.weeklyNumber.text = AtOManager.Instance.GetWeeklyName(AtOManager.Instance.GetWeekly());
                }
                if (!GameManager.Instance.IsObeliskChallenge())
                {
                    HeroSelectionManager.Instance.madnessButton.gameObject.SetActive(true);
                    if (GameManager.Instance.IsMultiplayer())
                    {
                        if (NetworkManager.Instance.IsMaster())
                        {
                            if (GameManager.Instance.IsLoadingGame())
                            {
                                ngValueMaster = ngValue = AtOManager.Instance.GetNgPlus();
                                ngCorruptors = AtOManager.Instance.GetMadnessCorruptors();
                                Traverse.Create(HeroSelectionManager.Instance).Field("ngValueMaster").SetValue(ngValueMaster);
                                Traverse.Create(HeroSelectionManager.Instance).Field("ngValue").SetValue(ngValue);
                                Traverse.Create(HeroSelectionManager.Instance).Field("ngCorruptors").SetValue(ngCorruptors);
                                HeroSelectionManager.Instance.SetMadnessLevel();
                            }
                            else if (SaveManager.PrefsHasKey("madnessLevelCoop") && SaveManager.PrefsHasKey("madnessCorruptorsCoop"))
                            {
                                int num5 = SaveManager.LoadPrefsInt("madnessLevelCoop");
                                string str = SaveManager.LoadPrefsString("madnessCorruptorsCoop");
                                if (PlayerManager.Instance.NgLevel >= num5)
                                {
                                    ngValueMaster = ngValue = num5;
                                    if (str != "")
                                        ngCorruptors = str;
                                }
                                else
                                {
                                    ngValueMaster = ngValue = 0;
                                    ngCorruptors = "";
                                }
                                Traverse.Create(HeroSelectionManager.Instance).Field("ngValueMaster").SetValue(ngValueMaster);
                                Traverse.Create(HeroSelectionManager.Instance).Field("ngValue").SetValue(ngValue);
                                Traverse.Create(HeroSelectionManager.Instance).Field("ngCorruptors").SetValue(ngCorruptors);
                                HeroSelectionManager.Instance.SetMadnessLevel();
                            }
                        }
                    }
                    else if (SaveManager.PrefsHasKey("madnessLevel") && SaveManager.PrefsHasKey("madnessCorruptors"))
                    {
                        int num6 = SaveManager.LoadPrefsInt("madnessLevel");
                        string str = SaveManager.LoadPrefsString("madnessCorruptors");
                        if (PlayerManager.Instance.NgLevel >= num6)
                        {
                            ngValueMaster = ngValue = num6;
                            if (str != "")
                                ngCorruptors = str;
                        }
                        else
                        {
                            ngValueMaster = ngValue = 0;
                            ngCorruptors = "";
                        }
                        Traverse.Create(HeroSelectionManager.Instance).Field("ngValueMaster").SetValue(ngValueMaster);
                        Traverse.Create(HeroSelectionManager.Instance).Field("ngValue").SetValue(ngValue);
                        Traverse.Create(HeroSelectionManager.Instance).Field("ngCorruptors").SetValue(ngCorruptors);
                        HeroSelectionManager.Instance.SetMadnessLevel();
                    }
                }
                else if (!GameManager.Instance.IsWeeklyChallenge())
                {
                    HeroSelectionManager.Instance.madnessButton.gameObject.SetActive(true);
                    if (GameManager.Instance.IsMultiplayer())
                    {
                        if (NetworkManager.Instance.IsMaster())
                        {
                            if (GameManager.Instance.IsLoadingGame())
                            {
                                obeliskMadnessValue = obeliskMadnessValueMaster = AtOManager.Instance.GetObeliskMadness();
                                Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValue").SetValue(obeliskMadnessValue);
                                Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValueMaster").SetValue(obeliskMadnessValueMaster);
                                HeroSelectionManager.Instance.SetObeliskMadnessLevel();
                            }
                            else if (SaveManager.PrefsHasKey("obeliskMadnessCoop"))
                            {
                                int num7 = SaveManager.LoadPrefsInt("obeliskMadnessCoop");
                                obeliskMadnessValue = PlayerManager.Instance.ObeliskMadnessLevel < num7 ? (obeliskMadnessValueMaster = 0) : (obeliskMadnessValueMaster = num7);
                                Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValue").SetValue(obeliskMadnessValue);
                                Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValueMaster").SetValue(obeliskMadnessValueMaster);
                                HeroSelectionManager.Instance.SetObeliskMadnessLevel();
                            }
                        }
                    }
                    else if (SaveManager.PrefsHasKey("obeliskMadness"))
                    {
                        int num8 = SaveManager.LoadPrefsInt("obeliskMadness");
                        obeliskMadnessValue = PlayerManager.Instance.ObeliskMadnessLevel < num8 ? (obeliskMadnessValueMaster = 0) : (obeliskMadnessValueMaster = num8);
                        Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValue").SetValue(obeliskMadnessValue);
                        Traverse.Create(HeroSelectionManager.Instance).Field("obeliskMadnessValueMaster").SetValue(obeliskMadnessValueMaster);
                        HeroSelectionManager.Instance.SetObeliskMadnessLevel();
                    }
                }
                else
                    HeroSelectionManager.Instance.madnessButton.gameObject.SetActive(false);
                HeroSelectionManager.Instance.Resize();
                LogDebug("assigning heroes to boxes, if relevant");
                if (GameManager.Instance.IsWeeklyChallenge() && !GameManager.Instance.IsLoadingGame())
                {
                    HeroSelectionManager.Instance.gameSeedModify.gameObject.SetActive(false);
                    ChallengeData weeklyData = Globals.Instance.GetWeeklyData(Functions.GetCurrentWeeklyWeek());
                    if ((UnityEngine.Object)weeklyData != (UnityEngine.Object)null)
                    {
                        HeroSelectionManager.Instance.heroSelectionDictionary[weeklyData.Hero1.Id].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[0]);
                        HeroSelectionManager.Instance.heroSelectionDictionary[weeklyData.Hero2.Id].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[1]);
                        HeroSelectionManager.Instance.heroSelectionDictionary[weeklyData.Hero3.Id].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[2]);
                        HeroSelectionManager.Instance.heroSelectionDictionary[weeklyData.Hero4.Id].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[3]);
                    }
                    if (!GameManager.Instance.IsMultiplayer() || NetworkManager.Instance.IsMaster())
                    {
                        if ((UnityEngine.Object)weeklyData != (UnityEngine.Object)null)
                            AtOManager.Instance.SetGameId(weeklyData.Seed);
                        else
                            AtOManager.Instance.SetGameId();
                    }
                    GameManager.Instance.SceneLoaded();
                }
                else if (GameManager.Instance.IsLoadingGame() || AtOManager.Instance.IsFirstGame() && !GameManager.Instance.IsMultiplayer() && !GameManager.Instance.IsObeliskChallenge())
                {
                    HeroSelectionManager.Instance.gameSeedModify.gameObject.SetActive(false);
                    if (AtOManager.Instance.IsFirstGame())
                        AtOManager.Instance.SetGameId("tuto");
                }
                else
                {
                    if (!GameManager.Instance.IsMultiplayer() || NetworkManager.Instance.IsMaster())
                        AtOManager.Instance.SetGameId();
                    HeroSelectionManager.Instance.gameSeed.gameObject.SetActive(true);
                }
                if (!GameManager.Instance.IsMultiplayer() || NetworkManager.Instance.IsMaster())
                {
                    HeroSelectionManager.Instance.gameSeedTxt.text = AtOManager.Instance.GetGameId();
                    if (GameManager.Instance.IsMultiplayer() && NetworkManager.Instance.IsMaster())
                        photonView.RPC("NET_SetSeed", RpcTarget.Others, (object)AtOManager.Instance.GetGameId());
                }
                if (GameManager.Instance.IsWeeklyChallenge() || GameManager.Instance.IsObeliskChallenge() && obeliskMadnessValue > 7)
                    HeroSelectionManager.Instance.gameSeed.gameObject.SetActive(false);
                Traverse.Create(HeroSelectionManager.Instance).Field("playerHeroPerksDict").SetValue(new Dictionary<string, List<string>>());
                LogDebug("before multiplayer...");
                if (GameManager.Instance.IsMultiplayer())
                {
                    HeroSelectionManager.Instance.masterDescription.gameObject.SetActive(true);
                    if (NetworkManager.Instance.IsMaster())
                    {
                        int num = 0;
                        foreach (Player player in NetworkManager.Instance.PlayerList)
                        {
                            for (int index = 0; index < 4; ++index)
                            {
                                boxSelection[index].ShowPlayer(num);
                                boxSelection[index].SetPlayerPosition(num, player.NickName);
                            }
                            ++num;
                        }
                        for (int position = num; position < 4; ++position)
                        {
                            for (int index = 0; index < 4; ++index)
                                boxSelection[index].SetPlayerPosition(position, "");
                        }
                        Traverse.Create(HeroSelectionManager.Instance).Field("boxSelection").SetValue(boxSelection);
                        foreach (Player player in NetworkManager.Instance.PlayerList)
                        {
                            string playerNickReal = NetworkManager.Instance.GetPlayerNickReal(player.NickName);
                            if (playerNickReal == NetworkManager.Instance.Owner0)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 0);
                            if (playerNickReal == NetworkManager.Instance.Owner1)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 1);
                            if (playerNickReal == NetworkManager.Instance.Owner2)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 2);
                            if (playerNickReal == NetworkManager.Instance.Owner3)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 3);
                        }
                        //this.DrawBoxSelectionNames();
                        // custom DrawBoxSelectionNames
                        int drawboxNum = 0;
                        foreach (Player player in NetworkManager.Instance.PlayerList)
                        {
                            for (int index = 0; index < 4; ++index)
                            {
                                boxSelection[index].ShowPlayer(drawboxNum);
                                boxSelection[index].SetPlayerPosition(drawboxNum, player.NickName);
                            }
                            ++drawboxNum;
                        }
                        for (int position = drawboxNum; position < 4; ++position)
                        {
                            for (int index = 0; index < 4; ++index)
                                boxSelection[index].SetPlayerPosition(position, "");
                        }
                        foreach (Player player in NetworkManager.Instance.PlayerList)
                        {
                            string playerNickReal = NetworkManager.Instance.GetPlayerNickReal(player.NickName);
                            if (playerNickReal == NetworkManager.Instance.Owner0)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 0);
                            if (playerNickReal == NetworkManager.Instance.Owner1)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 1);
                            if (playerNickReal == NetworkManager.Instance.Owner2)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 2);
                            if (playerNickReal == NetworkManager.Instance.Owner3)
                                HeroSelectionManager.Instance.AssignPlayerToBox(player.NickName, 3);
                        }
                        // end custom DrawBoxSelectionNames
                        HeroSelectionManager.Instance.botonBegin.gameObject.SetActive(true);
                        HeroSelectionManager.Instance.botonBegin.Disable();
                        HeroSelectionManager.Instance.botonFollow.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        HeroSelectionManager.Instance.gameSeedModify.gameObject.SetActive(false);
                        HeroSelectionManager.Instance.botonBegin.gameObject.SetActive(false);
                        HeroSelectionManager.Instance.botonFollow.transform.parent.gameObject.SetActive(true);
                        HeroSelectionManager.Instance.ShowFollowStatus();
                    }
                    LogDebug("master loading game");
                    if (NetworkManager.Instance.IsMaster() && GameManager.Instance.IsLoadingGame())
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            Hero hero = AtOManager.Instance.GetHero(index);
                            if (hero != null && !((UnityEngine.Object)hero.HeroData == (UnityEngine.Object)null))
                            {
                                string subclassName = hero.SubclassName;
                                int perkRank = hero.PerkRank;
                                string skinUsed = hero.SkinUsed;
                                string cardbackUsed = hero.CardbackUsed;
                                // Plugin.Log.LogDebug("second AddToPlayerHeroSkin! SCDID: " + subclassName + " activeSkin: " + skinUsed);
                                string lower = subclassName.ToLower();
                                // custom AddToPlayerHeroSkin
                                // this.AddToPlayerHeroSkin(subclassName, skinUsed);
                                if (!HeroSelectionManager.Instance.playerHeroSkinsDict.ContainsKey(lower))
                                    HeroSelectionManager.Instance.playerHeroSkinsDict.Add(lower, skinUsed);
                                else
                                    HeroSelectionManager.Instance.playerHeroSkinsDict[lower] = skinUsed;
                                // custom AddToPlayerHeroCardback
                                // this.AddToPlayerHeroCardback(subclassName, cardbackUsed);
                                if (!HeroSelectionManager.Instance.playerHeroCardbackDict.ContainsKey(lower))
                                    HeroSelectionManager.Instance.playerHeroCardbackDict.Add(lower, cardbackUsed);
                                else
                                    HeroSelectionManager.Instance.playerHeroCardbackDict[lower] = cardbackUsed;
                                // end custom
                                if (HeroSelectionManager.Instance.heroSelectionDictionary.ContainsKey(subclassName))
                                {
                                    HeroSelectionManager.Instance.heroSelectionDictionary[subclassName].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[index]);
                                    if (hero.HeroData.HeroSubClass.MainCharacter)
                                    {
                                        HeroSelectionManager.Instance.heroSelectionDictionary[subclassName].SetRankBox(perkRank);
                                        HeroSelectionManager.Instance.heroSelectionDictionary[subclassName].SetSkin(skinUsed);
                                    }
                                }
                                photonView.RPC("NET_AssignHeroToBox", RpcTarget.Others, (object)hero.SubclassName.ToLower(), (object)index, (object)perkRank, (object)skinUsed, (object)cardbackUsed);
                            }
                        }
                    }
                }
                else
                {
                    HeroSelectionManager.Instance.masterDescription.gameObject.SetActive(false);
                    HeroSelectionManager.Instance.botonFollow.transform.parent.gameObject.SetActive(false);
                    HeroSelectionManager.Instance.botonBegin.gameObject.SetActive(true);
                    HeroSelectionManager.Instance.botonBegin.Disable();
                    if (!GameManager.Instance.IsWeeklyChallenge())
                    {
                        //this.PreAssign();
                        // custom PreAssign
                        if (!(PlayerManager.Instance.LastUsedTeam == null || PlayerManager.Instance.LastUsedTeam.Length != 4))
                        {
                            for (int index = 0; index < 4; ++index)
                            {
                                if (HeroSelectionManager.Instance.heroSelectionDictionary.ContainsKey(PlayerManager.Instance.LastUsedTeam[index]) && (GameManager.Instance.IsObeliskChallenge() || PlayerManager.Instance.IsHeroUnlocked(PlayerManager.Instance.LastUsedTeam[index])))
                                    HeroSelectionManager.Instance.heroSelectionDictionary[PlayerManager.Instance.LastUsedTeam[index]].AssignHeroToBox(HeroSelectionManager.Instance.boxGO[index]);
                            }
                        }
                        // end
                    }
                }
                yield return (object)Globals.Instance.WaitForSeconds(0.1f);
                HeroSelectionManager.Instance.RefreshSandboxButton();
                LogDebug("adventure mode");
                if (!GameManager.Instance.IsObeliskChallenge())
                {
                    HeroSelectionManager.Instance.sandboxButton.gameObject.SetActive(true);
                    HeroSelectionManager.Instance.madnessButton.localPosition = new Vector3(2.43f, HeroSelectionManager.Instance.madnessButton.localPosition.y, HeroSelectionManager.Instance.madnessButton.localPosition.z);
                    HeroSelectionManager.Instance.sandboxButton.localPosition = new Vector3(5.23f, HeroSelectionManager.Instance.sandboxButton.localPosition.y, HeroSelectionManager.Instance.sandboxButton.localPosition.z);
                    if (!GameManager.Instance.IsMultiplayer() || GameManager.Instance.IsMultiplayer() && NetworkManager.Instance.IsMaster())
                    {
                        string sandboxMods;
                        if (GameManager.Instance.GameStatus != Enums.GameStatus.LoadGame)
                        {
                            if ((!GameManager.Instance.IsMultiplayer() || NetworkManager.Instance.IsMaster()) && PlayerManager.Instance.NgLevel == 0)
                            {
                                SandboxManager.Instance.DisableSandbox();
                                AtOManager.Instance.ClearSandbox();
                            }
                            else
                                AtOManager.Instance.SetSandboxMods(SaveManager.LoadPrefsString("sandboxSettings"));
                            SandboxManager.Instance.LoadValuesFromAtOManager();
                            SandboxManager.Instance.AdjustTotalHeroesBoxToCoop();
                            SandboxManager.Instance.SaveValuesToAtOManager();
                            sandboxMods = AtOManager.Instance.GetSandboxMods();
                        }
                        else
                        {
                            sandboxMods = AtOManager.Instance.GetSandboxMods();
                            SandboxManager.Instance.LoadValuesFromAtOManager();
                        }
                        if (GameManager.Instance.IsMultiplayer() && NetworkManager.Instance.IsMaster())
                            photonView.RPC("NET_ShareSandbox", RpcTarget.Others, (object)Functions.CompressString(sandboxMods));
                        HeroSelectionManager.Instance.RefreshCharBoxesBySandboxHeroes();
                    }
                }
                else
                {
                    HeroSelectionManager.Instance.sandboxButton.gameObject.SetActive(false);
                    HeroSelectionManager.Instance.madnessButton.localPosition = new Vector3(3.8f, HeroSelectionManager.Instance.madnessButton.localPosition.y, HeroSelectionManager.Instance.madnessButton.localPosition.z);
                    SandboxManager.Instance.DisableSandbox();
                }
                HeroSelectionManager.Instance.readyButtonText.gameObject.SetActive(false);
                HeroSelectionManager.Instance.readyButton.gameObject.SetActive(false);
                if (GameManager.Instance.IsMultiplayer())
                {
                    if (NetworkManager.Instance.IsMaster())
                    {
                        NetworkManager.Instance.ClearAllPlayerManualReady();
                        NetworkManager.Instance.SetManualReady(true);
                    }
                    else
                    {
                        HeroSelectionManager.Instance.readyButtonText.gameObject.SetActive(true);
                        HeroSelectionManager.Instance.readyButton.gameObject.SetActive(true);
                    }
                }
                GameManager.Instance.SceneLoaded();
                if (!GameManager.Instance.TutorialWatched("characterPerks"))
                {
                    foreach (KeyValuePair<string, HeroSelection> heroSelection in HeroSelectionManager.Instance.heroSelectionDictionary)
                    {
                        if (heroSelection.Value.perkPointsT.gameObject.activeSelf)
                        {
                            GameManager.Instance.ShowTutorialPopup("characterPerks", heroSelection.Value.perkPointsT.gameObject.transform.position, Vector3.zero);
                            break;
                        }
                    }
                }
                LogDebug("multiplayer+loading+master");
                if (GameManager.Instance.IsMultiplayer() && GameManager.Instance.IsLoadingGame() && NetworkManager.Instance.IsMaster())
                {
                    bool flag = true;
                    List<string> stringList1 = new List<string>();
                    List<string> stringList2 = new List<string>();
                    for (int index = 0; index < 4; ++index)
                    {
                        Hero hero = AtOManager.Instance.GetHero(index);
                        if (hero != null && (UnityEngine.Object)hero.HeroData != (UnityEngine.Object)null && hero.OwnerOriginal != null)
                        {
                            string lower = hero.OwnerOriginal.ToLower();
                            if (!stringList1.Contains(lower))
                                stringList1.Add(lower);
                        }
                        else
                            break;
                    }
                    foreach (Player player in NetworkManager.Instance.PlayerList)
                    {
                        string lower = NetworkManager.Instance.GetPlayerNickReal(player.NickName).ToLower();
                        if (!stringList2.Contains(lower))
                            stringList2.Add(lower);
                    }
                    if (stringList1.Count != stringList2.Count)
                    {
                        flag = false;
                    }
                    else
                    {
                        for (int index = 0; index < stringList2.Count; ++index)
                        {
                            if (!stringList1.Contains(stringList2[index]))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (!flag)
                        photonView.RPC("NET_SetNotOriginal", RpcTarget.All);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Texts), "GetText")]
        public static void GetTextPostfix(string _id, ref string __result, string _type = "")
        {
            if (medsTexts.ContainsKey(_id))
                __result = medsTexts[_id];
            return;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputController), "DoKeyBinding")]
        public static bool DoKeyBindingPrefix(ref InputAction.CallbackContext _context)
        {
            if (Keyboard.current != null)
            {
                if (_context.control == Keyboard.current[Key.F1])
                {
                    ObeliskialUI.ShowUI = !ObeliskialUI.ShowUI;
                    return false;
                }
                else if (_context.control == Keyboard.current[Key.F2])
                {
                    ChangeUIState();
                    return false;
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "NodeScore")]
        public static void NodeScorePrefix()
        {
            /*this is really just used for score checking on the dev side, so I'm commenting it out :)

            Hero[] medsTeamAtO = Traverse.Create(AtOManager.Instance).Field("teamAtO").GetValue<Hero[]>();
            int medsMapVisitedNodesTMP = Traverse.Create(AtOManager.Instance).Field("mapVisitedNodesTMP").GetValue<int>();
            List<string> medsMapVisitedNodes = Traverse.Create(AtOManager.Instance).Field("mapVisitedNodes").GetValue<List<string>>();
            int medsCombatExpertise = Traverse.Create(AtOManager.Instance).Field("combatExpertise").GetValue<int>();
            int medsCombatExpertiseTMP = Traverse.Create(AtOManager.Instance).Field("combatExpertiseTMP").GetValue<int>();
            int medsExperienceGainedTMP = Traverse.Create(AtOManager.Instance).Field("experienceGainedTMP").GetValue<int>();
            int medsTotalDeathsTMP = Traverse.Create(AtOManager.Instance).Field("totalDeathsTMP").GetValue<int>();
            int medsBossesKilled = Traverse.Create(AtOManager.Instance).Field("bossesKilled").GetValue<int>();
            int medsBossesKilledTMP = Traverse.Create(AtOManager.Instance).Field("bossesKilledTMP").GetValue<int>();
            int medsCorruptionCommonCompleted = Traverse.Create(AtOManager.Instance).Field("corruptionCommonCompleted").GetValue<int>();
            int medsCorruptionCommonCompletedTMP = Traverse.Create(AtOManager.Instance).Field("corruptionCommonCompletedTMP").GetValue<int>();
            int medsCorruptionUncommonCompleted = Traverse.Create(AtOManager.Instance).Field("corruptionUncommonCompleted").GetValue<int>();
            int medsCorruptionUncommonCompletedTMP = Traverse.Create(AtOManager.Instance).Field("corruptionUncommonCompletedTMP").GetValue<int>();
            int medsCorruptionRareCompleted = Traverse.Create(AtOManager.Instance).Field("corruptionRareCompleted").GetValue<int>();
            int medsCorruptionRareCompletedTMP = Traverse.Create(AtOManager.Instance).Field("corruptionRareCompletedTMP").GetValue<int>();
            int medsCorruptionEpicCompleted = Traverse.Create(AtOManager.Instance).Field("corruptionEpicCompleted").GetValue<int>();
            int medsCorruptionEpicCompletedTMP = Traverse.Create(AtOManager.Instance).Field("corruptionEpicCompletedTMP").GetValue<int>();

            if (medsTeamAtO == null)
                return;
            bool flag = medsMapVisitedNodesTMP == 0;
            int num1 = 0;
            for (int index = 0; index < medsMapVisitedNodes.Count; ++index)
            {
                if ((UnityEngine.Object)Globals.Instance.GetNodeData(medsMapVisitedNodes[index]) != (UnityEngine.Object)null && (UnityEngine.Object)Globals.Instance.GetNodeData(medsMapVisitedNodes[index]).NodeZone != (UnityEngine.Object)null && !Globals.Instance.GetNodeData(medsMapVisitedNodes[index]).NodeZone.DisableExperienceOnThisZone)
                    ++num1;
            }
            int num2 = num1 - medsMapVisitedNodesTMP;
            if (!GameManager.Instance.IsObeliskChallenge())
            {
                if (num1 < 2)
                {
                    medsMapVisitedNodesTMP = 0;
                    num2 = 0;
                }
                else
                {
                    if (medsMapVisitedNodesTMP == 0)
                        num2 -= 2;
                    medsMapVisitedNodesTMP = num1;
                }
            }
            else if (num1 < 1)
            {
                medsMapVisitedNodesTMP = 0;
                num2 = 0;
            }
            else
            {
                if (medsMapVisitedNodesTMP == 0)
                    --num2;
                medsMapVisitedNodesTMP = num1;
            }
            int num3 = num2 * 36;
            int num4 = medsCombatExpertise - medsCombatExpertiseTMP;
            medsCombatExpertiseTMP = medsCombatExpertise;
            int num5 = num4;
            if (num5 < 0)
                num5 = 0;
            int num6 = num5 * 13;
            int num7 = 0;
            int num8 = 0;
            if (medsTeamAtO != null)
            {
                for (int index = 0; index < medsTeamAtO.Length; ++index)
                {
                    num7 += medsTeamAtO[index].Experience;
                    num8 += medsTeamAtO[index].TotalDeaths;
                }
            }
            int num9 = num7 - medsExperienceGainedTMP;
            medsExperienceGainedTMP = num7;
            int num10 = Functions.FuncRoundToInt((float)num9 * 0.5f);
            int num11 = num8 - medsTotalDeathsTMP;
            medsTotalDeathsTMP = num8;
            int num12 = -num11 * 100;
            int num13 = medsBossesKilled - medsBossesKilledTMP;
            medsBossesKilledTMP = medsBossesKilled;
            int num14 = num13 * 80;
            int num15 = medsCorruptionCommonCompleted - medsCorruptionCommonCompletedTMP;
            medsCorruptionCommonCompletedTMP = medsCorruptionCommonCompleted;
            int num16 = medsCorruptionUncommonCompleted - medsCorruptionUncommonCompletedTMP;
            medsCorruptionUncommonCompletedTMP = medsCorruptionUncommonCompleted;
            int num17 = medsCorruptionRareCompleted - medsCorruptionRareCompletedTMP;
            medsCorruptionRareCompletedTMP = medsCorruptionRareCompleted;
            int num18 = medsCorruptionEpicCompleted - medsCorruptionEpicCompletedTMP;
            medsCorruptionEpicCompletedTMP = medsCorruptionEpicCompleted;
            int num19 = num15 * 40 + num16 * 80 + num17 * 130 + num18 * 200;
            int num20 = num3 + num6 + num12 + num10 + num14 + num19;
            Plugin.Log.LogDebug("num1: " + num1);
            Plugin.Log.LogDebug("num2: " + num2);
            Plugin.Log.LogDebug("num3: " + num3);
            Plugin.Log.LogDebug("num4: " + num4);
            Plugin.Log.LogDebug("num5: " + num5);
            Plugin.Log.LogDebug("num6: " + num6);
            Plugin.Log.LogDebug("num7: " + num7);
            Plugin.Log.LogDebug("num8: " + num8);
            Plugin.Log.LogDebug("num9: " + num9);
            Plugin.Log.LogDebug("num10: " + num10);
            Plugin.Log.LogDebug("num11: " + num11);
            Plugin.Log.LogDebug("num12: " + num12);
            Plugin.Log.LogDebug("num13: " + num13);
            Plugin.Log.LogDebug("num14: " + num14);
            Plugin.Log.LogDebug("num15: " + num15);
            Plugin.Log.LogDebug("num16: " + num16);
            Plugin.Log.LogDebug("num17: " + num17);
            Plugin.Log.LogDebug("num18: " + num18);
            Plugin.Log.LogDebug("num19: " + num19);
            Plugin.Log.LogDebug("num20: " + num20);*/
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "CalculateScore")]
        public static void CalculateScorePrefix(bool _calculateMadnessMultiplier, int _auxValue)
        {
            /*int medsTotalScoreTMP = Traverse.Create(AtOManager.Instance).Field("totalScoreTMP").GetValue<int>();
            Log.LogDebug("_CMM: " + _calculateMadnessMultiplier);
            Log.LogDebug("_aux: " + _auxValue);
            Log.LogDebug("totalScoreTMP: " + medsTotalScoreTMP);
            medsTotalScoreTMP += Functions.FuncRoundToInt((float)(medsTotalScoreTMP * Functions.GetMadnessScoreMultiplier(AtOManager.Instance.GetMadnessDifficulty(), !GameManager.Instance.IsObeliskChallenge()) / 100));
            Log.LogDebug("score: " + medsTotalScoreTMP);*/
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TomeManager), "SelectTomeCards")]
        public static bool SelectTomeCardsPrefix(ref TomeManager __instance, int index = -1, bool absolute = false)
        {
            int medsActiveTomeCards = Traverse.Create(__instance).Field("activeTomeCards").GetValue<int>();
            int medsPageAct = Traverse.Create(__instance).Field("pageAct").GetValue<int>();
            int medsPageOld = Traverse.Create(__instance).Field("pageOld").GetValue<int>();
            int medsPageMax = Traverse.Create(__instance).Field("pageMax").GetValue<int>();
            int medsNumCards = Traverse.Create(__instance).Field("numCards").GetValue<int>();
            string medsSearchTerm = Traverse.Create(__instance).Field("searchTerm").GetValue<string>();
            List<string> medsCardList = new();
            if (index == medsActiveTomeCards && !absolute)
                return false;
            medsActiveTomeCards = index;
            Traverse.Create(__instance).Field("activeTomeCards").SetValue(medsActiveTomeCards);
            List<string> stringList = new List<string>();
            switch (index)
            {
                case -1:
                    stringList = Globals.Instance.CardListNotUpgraded;
                    break;
                case 0:
                    stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Warrior];
                    break;
                case 1:
                    stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Mage];
                    break;
                case 2:
                    stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Healer];
                    break;
                case 3:
                    stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Scout];
                    break;
                default:
                    if (index == 4 && GameManager.Instance.GetDeveloperMode())
                    {
                        stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Monster];
                        break;
                    }
                    switch (index)
                    {
                        case 5:
                            stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Boon];
                            break;
                        case 6:
                            stringList = Globals.Instance.CardListNotUpgradedByClass[Enums.CardClass.Injury];
                            break;
                        case 7:
                            stringList = Globals.Instance.CardItemByType[Enums.CardType.Weapon];
                            break;
                        case 8:
                            stringList = Globals.Instance.CardItemByType[Enums.CardType.Armor];
                            break;
                        case 9:
                            stringList = Globals.Instance.CardItemByType[Enums.CardType.Jewelry];
                            break;
                        case 10:
                            stringList = Globals.Instance.CardItemByType[Enums.CardType.Accesory];
                            break;
                        case 11:
                            stringList = Globals.Instance.CardItemByType[Enums.CardType.Pet];
                            break;
                        case 22:
                            stringList = Globals.Instance.CardListByType[Enums.CardType.Enchantment];
                            break;
                    }
                    break;
            }
            for (int index1 = 0; index1 < stringList.Count; ++index1)
            {
                CardData cardData = Globals.Instance.GetCardData(stringList[index1], false);
                if (!((UnityEngine.Object)cardData != (UnityEngine.Object)null) || cardData.ShowInTome)
                {
                    if (medsSearchTerm.Trim() != "")
                    {
                        if (index != 22 || cardData.CardClass != Enums.CardClass.Monster)
                        {
                            if (Globals.Instance.IsInSearch(medsSearchTerm, stringList[index1]))
                                medsCardList.Add(stringList[index1]);
                            if ((UnityEngine.Object)cardData != (UnityEngine.Object)null && index != 22)
                            {
                                if (cardData.UpgradesTo1 != "" && Globals.Instance.IsInSearch(medsSearchTerm, cardData.UpgradesTo1))
                                    medsCardList.Add(cardData.UpgradesTo1);
                                if (cardData.UpgradesTo2 != "" && Globals.Instance.IsInSearch(medsSearchTerm, cardData.UpgradesTo2))
                                    medsCardList.Add(cardData.UpgradesTo2);
                                if ((UnityEngine.Object)cardData.UpgradesToRare != (UnityEngine.Object)null && Globals.Instance.IsInSearch(medsSearchTerm, cardData.UpgradesToRare.Id))
                                    medsCardList.Add(cardData.UpgradesToRare.Id);
                            }
                        }
                    }
                    else if (index != 22 || cardData.CardUpgraded == Enums.CardUpgraded.No && cardData.CardClass != Enums.CardClass.Monster)
                        medsCardList.Add(stringList[index1]);
                }
            }
            //this.cardList.Sort(); // cards now sorted during CreateGameContent->CreateCardClones
            medsPageOld = medsPageAct = 0;
            medsPageMax = Mathf.CeilToInt((float)medsCardList.Count / (float)medsNumCards);

            Traverse.Create(__instance).Field("pageAct").SetValue(medsPageAct);
            Traverse.Create(__instance).Field("pageOld").SetValue(medsPageOld);
            Traverse.Create(__instance).Field("pageMax").SetValue(medsPageMax);
            Traverse.Create(__instance).Field("cardList").SetValue(medsCardList);

            //__instance.RedoPageNumbers();
            __instance.GetType().GetMethod("RedoPageNumbers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(__instance, new object[] { });

            //__instance.ActivateDeactivateButtons(index);
            __instance.GetType().GetMethod("ActivateDeactivateButtons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(__instance, new object[] { index });

            __instance.SetPage(1, true);
            return false; // do not run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "DrawArrow")]
        public static bool DrawArrowPrefix(ref MapManager __instance, Node _nodeSource, Node _nodeDestination)
        {
            if (!(_nodeSource.gameObject.activeSelf && _nodeDestination.gameObject.activeSelf))
                return false; // do not run original method if either node is not visible
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenuManager), "SetMenuCurrentProfile")]
        public static void SetMenuCurrentProfilePostfix()
        {
            MainMenuManager.Instance.profileMenuText.text += $" (Obeliskial)";
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Globals), "GetWeeklyData")]
        public static void GetWeeklyDataPrefix(ref int _week)
        {
            if (medsForceWeekly != 0)
                _week = medsForceWeekly;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotHeroChar), "OnMouseUp")]
        public static bool BotHeroCharClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonCardback), "OnMouseUp")]
        public static bool BotonCardbackClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonEndTurn), "OnMouseUp")]
        public static bool BotonEndTurnClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonFilter), "OnMouseUp")]
        public static bool BotonFilterClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonGeneric), "OnMouseUp")]
        public static bool BotonGenericClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonMenuGameMode), "OnMouseUp")]
        public static bool BotonMenuGameModeClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonRollover), "OnMouseUp")]
        public static bool BotonRolloverClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonScore), "OnMouseUp")]
        public static bool BotonScoreClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonSkin), "OnMouseUp")]
        public static bool BotonSkinClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BotonSupply), "OnMouseUp")]
        public static bool BotonSupplyClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(botTownUpgrades), "OnMouseUp")]
        public static bool botTownUpgradesClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BoxPlayer), "OnMouseUp")]
        public static bool BoxPlayerClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardCraftSelectorEnergy), "OnMouseUp")]
        public static bool CardCraftSelectorEnergyClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardCraftSelectorRarity), "OnMouseUp")]
        public static bool CardCraftSelectorRarityClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardItem), "OnMouseUp")]
        public static bool CardItemClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardVertical), "OnMouseUp")]
        public static bool CardVerticalClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterGOItem), "OnMouseUp")]
        public static bool CharacterGOItemClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterItem), "fOnMouseUp")]
        public static bool CharacterItemClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterLoot), "OnMouseUp")]
        public static bool CharacterLootClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharPopupClose), "OnMouseUp")]
        public static bool CharPopupCloseClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CombatTarget), "OnMouseUp")]
        public static bool CombatTargetClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DeckInHero), "OnMouseUp")]
        public static bool DeckInHeroClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DeckPile), "OnMouseUp")]
        public static bool DeckPileClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EmoteManager), "OnMouseUp")]
        public static bool EmoteManagerClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HeroSelection), "OnMouseUp")]
        public static bool HeroSelectionClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InitiativePortrait), "OnMouseUp")]
        public static bool InitiativePortraitClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemCombatIcon), "fOnMouseUp")]
        public static bool ItemCombatIconClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Node), "OnMouseUp")]
        public static bool NodeClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OverCharacter), "OnMouseUp")]
        public static bool OverCharacterClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PerkChallengeItem), "OnMouseUp")]
        public static bool PerkChallengeItemClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PerkColumnItem), "OnMouseUp")]
        public static bool PerkColumnItemClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PerkNode), "OnMouseUp")]
        public static bool PerkNodeClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RandomHeroSelector), "OnMouseUp")]
        public static bool RandomHeroSelectorClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Reply), "OnMouseUp")]
        public static bool ReplyClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TomeButton), "OnMouseUp")]
        public static bool TomeButtonClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TomeEdge), "OnMouseUp")]
        public static bool TomeEdgeClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TomeNumber), "OnMouseUp")]
        public static bool TomeNumberClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TomeRun), "OnMouseUp")]
        public static bool TomeRunClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TownBuilding), "OnMouseUp")]
        public static bool TownBuildingClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitLevel), "OnMouseUp")]
        public static bool TraitLevelClickCapture()
        {
            if (DevTools.ShowUI && DevTools.lockAtOToggle.isOn)
                return false;
            return true;
        }

        /* devs patched this method in version 1.3.02
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), "Awake")]
        public static void GameManagerAwakePrefix(ref GameManager __instance)
        {
            Traverse.Create(__instance).Field("pDXEnabled").SetValue(!(medsConsistency.Value));
        }*/


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MainMenuManager), "CreatePDXDropdowns")]
        public static bool CreatePDXDropdownsPrefix(ref MainMenuManager __instance)
        {
            if (medsConsistency.Value)
            {
                __instance.ResetPDXScreens();
                __instance.paradoxT.gameObject.SetActive(false);
                return false; // do not run original method
            }
            return true; // run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MainMenuManager), "ShowPDXPreLogin")]
        public static bool ShowPDXPreLoginPrefix(ref MainMenuManager __instance)
        {
            if (medsConsistency.Value)
            {
                __instance.ResetPDXScreens();
                __instance.paradoxT.gameObject.SetActive(false);
                return false; // do not run original method
            }
            return true; // run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MainMenuManager), "ShowPDXLogin")]
        public static bool ShowPDXLoginPrefix(ref MainMenuManager __instance)
        {
            if (medsConsistency.Value)
            {
                __instance.ResetPDXScreens();
                __instance.paradoxT.gameObject.SetActive(false);
                return false; // do not run original method
            }
            return true; // run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MainMenuManager), "ShowPDXLogged")]
        public static bool ShowPDXLoggedPrefix(ref MainMenuManager __instance)
        {
            if (medsConsistency.Value)
            {
                __instance.ResetPDXScreens();
                __instance.paradoxT.gameObject.SetActive(false);
                return false; // do not run original method
            }
            return true; // run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paradox.Startup), "ShowDocumentFromStartup")]
        public static bool ShowDocumentFromStartupPrefix()
        {
            if (medsConsistency.Value)
            {
                Paradox.Startup.waitingForLoginDocuments = false;
                MainMenuManager.Instance.paradoxDocumentPopup.gameObject.SetActive(false);
                return false; // do not run original method
            }
            return true; // run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paradox.Telemetry), "SendStartGame")]
        public static bool SendStartGamePrefix()
        {
            return !medsConsistency.Value; // do not run original method if Consistency is enabled
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paradox.Telemetry), "SendPlaysessionStart")]
        public static bool SendPlaysessionStartPrefix()
        {
            return !medsConsistency.Value; // do not run original method if Consistency is enabled
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paradox.Telemetry), "SendPlaysessionEnd")]
        public static bool SendPlaysessionEndPrefix()
        {
            return !medsConsistency.Value; // do not run original method if Consistency is enabled
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paradox.Telemetry), "SendActStart")]
        public static bool SendActStartPrefix()
        {
            return !medsConsistency.Value; // do not run original method if Consistency is enabled
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paradox.Telemetry), "SendUnlock")]
        public static bool SendUnlockPrefix()
        {
            return !medsConsistency.Value; // do not run original method if Consistency is enabled
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "SetCurrentNode")]
        public static void SetCurrentNodePrefix(ref string _nodeName)
        {
            if (_nodeName == "sen_0" && !DevTools.inputStartingNode.Text.IsNullOrWhiteSpace() && DevTools.inputStartingNode.Text.ToLower() != "starting node")
                _nodeName = DevTools.inputStartingNode.Text.ToLower();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LogosManager), "Awake")]
        public static void LogosManagerAwakePrefix(ref LogosManager __instance)
        {
            if (medsSkipLogos.Value)
                __instance.srList.Clear();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "GetTeamNPCReward")]
        public static bool GetTeamNPCRewardPrefix(ref TierRewardData __result, ref AtOManager __instance)
        {
            int num = 0;
            string[] teamNPCAtO = __instance.GetTeamNPC();
            for (int index = 0; index < teamNPCAtO.Length; ++index)
            {
                if (teamNPCAtO[index] != null && teamNPCAtO[index] != "")
                {
                    NPCData npcData = Globals.Instance.GetNPC(teamNPCAtO[index]);
                    if (npcData != null)
                    {
                        if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.PlayerHasRequirement(Globals.Instance.GetRequirementData("_tier2")) && (UnityEngine.Object)npcData.UpgradedMob != (UnityEngine.Object)null)
                            npcData = npcData.UpgradedMob;
                        if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.GetNgPlus() > 0 && npcData.NgPlusMob != null)
                            npcData = npcData.NgPlusMob;
                        if (npcData != null && MadnessManager.Instance.IsMadnessTraitActive("despair") && (UnityEngine.Object)npcData.HellModeMob != (UnityEngine.Object)null)
                            npcData = npcData.HellModeMob;
                        if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && (UnityEngine.Object)npcData.TierReward != (UnityEngine.Object)null && npcData.TierReward.TierNum > num)
                            num = npcData.TierReward.TierNum;
                    }
                }
            }
            __result = Globals.Instance.GetTierRewardData(num);
            return false; // do not run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "GetGoldFromCombat")]
        public static bool GetGoldFromCombatPrefix(ref int __result, ref AtOManager __instance)
        {
            int goldFromCombat = 0;
            string[] teamNPCAtO = __instance.GetTeamNPC();
            if (teamNPCAtO != null)
            {
                for (int index = 0; index < teamNPCAtO.Length; ++index)
                {
                    if (teamNPCAtO[index] != null && teamNPCAtO[index] != "")
                    {
                        NPCData npcData = Globals.Instance.GetNPC(teamNPCAtO[index]);
                        if (npcData != null)
                        {
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.PlayerHasRequirement(Globals.Instance.GetRequirementData("_tier2")) && (UnityEngine.Object)npcData.UpgradedMob != (UnityEngine.Object)null)
                                npcData = npcData.UpgradedMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.GetNgPlus() > 0 && npcData.NgPlusMob != null)
                                npcData = npcData.NgPlusMob;
                            if (npcData != null && MadnessManager.Instance.IsMadnessTraitActive("despair") && (UnityEngine.Object)npcData.HellModeMob != (UnityEngine.Object)null)
                                npcData = npcData.HellModeMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && npcData.GoldReward > 0)
                                goldFromCombat += npcData.GoldReward;
                        }
                    }
                }
            }
            __result = goldFromCombat;
            return false; // do not run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "GetExperienceFromCombat")]
        public static bool GetExperienceFromCombatPrefix(ref int __result, ref AtOManager __instance)
        {
            int experienceFromCombat = 0;
            string[] teamNPCAtO = __instance.GetTeamNPC();
            if (teamNPCAtO != null)
            {
                for (int index = 0; index < teamNPCAtO.Length; ++index)
                {
                    if (teamNPCAtO[index] != null && teamNPCAtO[index] != "")
                    {
                        NPCData npcData = Globals.Instance.GetNPC(teamNPCAtO[index]);
                        if (npcData != null)
                        {
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.PlayerHasRequirement(Globals.Instance.GetRequirementData("_tier2")) && (UnityEngine.Object)npcData.UpgradedMob != (UnityEngine.Object)null)
                                npcData = npcData.UpgradedMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.GetNgPlus() > 0 && npcData.NgPlusMob != null)
                                npcData = npcData.NgPlusMob;
                            if (npcData != null && MadnessManager.Instance.IsMadnessTraitActive("despair") && (UnityEngine.Object)npcData.HellModeMob != (UnityEngine.Object)null)
                                npcData = npcData.HellModeMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && npcData.ExperienceReward > 0)
                                experienceFromCombat += npcData.ExperienceReward;
                        }
                    }
                }
            }
            __result = experienceFromCombat;
            return false; // do not run original method
        }
        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(SaveManager), "SaveGame")]
        public static void SaveGamePrefix()
        {

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveManager), "SaveGameTurn")]
        public static void SaveGameTurnPrefix()
        {

        }*/

        // all of the below is just for testing

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AI), "DoAI")]
        public static void DoAIPrefix()
        {
            LogDebug("DoAI");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), "GenerateDecksNPCs")]
        public static void GenerateDecksNPCsPrefix()
        {
            LogDebug("GenerateDecksNPCs");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NPC), "BeginRound")]
        public static void BeginRoundPrefix()
        {
            LogDebug("BeginRound");
        }           

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NPC), "CreateOverDeck")]
        public static void CreateOverDeckPrefix()
        {
            LogDebug("CreateOverDeck");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), "AddCardToNPCDeck")]
        public static void AddCardToNPCDeckPrefix(ref int npcIndex, ref string idCard)
        {
            LogDebug("AddCardToNPCDeck: " + npcIndex.ToString() + ", " + idCard);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "TravelToThisNode")]
        public static void TravelToThisNodePrefix(ref MapManager __instance, Node _node)
        {
            Log.LogDebug("TRAVELTOTHISNODE PREFIX");
            if ((UnityEngine.Object)_node == (UnityEngine.Object)null)
            {
                Log.LogDebug("node is null! :(");
            }
            else if ((UnityEngine.Object)_node.nodeData == (UnityEngine.Object)null)
            {
                Log.LogDebug("nodedata is null!");
            }
            else
            {
                Log.LogDebug("nodeid: " + _node.nodeData.NodeId);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "BeginMapContinue")]
        public static void BeginMapContinuePrefix(ref MapManager __instance)
        {
            Log.LogDebug("BeginMapContinue PREFIX");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "PlayerSelectedNode")]
        public static void PlayerSelectedNodePrefix(ref MapManager __instance, Node _node)
        {
            Log.LogDebug("PlayerSelectedNode PREFIX");
            if (_node == null)
            {
                Log.LogDebug("PSN node null");
            }
            else if (_node.nodeData == null)
            {
                Log.LogDebug("PSN nodedata null");
            }
            else
            {
                Log.LogDebug("PSN node ID: " + _node.nodeData.NodeId);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "NET_PlayerSelectedNode")]
        public static void NET_PlayerSelectedNodePrefix(ref MapManager __instance)
        {
            Log.LogDebug("NET_PlayerSelectedNode PREFIX");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MapManager), "Awake")]
        public static void AwakePostfix(ref MapManager __instance)
        {
            Log.LogDebug("MAPMANAGER POSTFIX");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "GetNodeFromId")]
        public static void GetNodeFromIdPrefix(ref MapManager __instance, string nodeId)
        {
            Log.LogDebug("GetNodeFromId PREFIX: " + nodeId);
            /*foreach (KeyValuePair<string, Node> kvp in __instance.GetMapNodeDict())
                Plugin.Log.LogDebug("KVP KEY: " + kvp.Key);*/
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MapManager), "GetNodeFromId")]
        public static void GetNodeFromIdPostfix(ref MapManager __instance, Node __result)
        {
            Log.LogDebug("GetNodeFromId POSTFIX: ");
            /*foreach (KeyValuePair<string, Node> kvp in __instance.GetMapNodeDict())
                Plugin.Log.LogDebug("KVP KEY: " + kvp.Key);*/
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "GetMapNodes")]
        public static void GetMapNodesPrefix(ref MapManager __instance)
        {
            Log.LogDebug("GetMapNodesPrefix, worldtransform children: " + __instance.worldTransform.childCount);

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitRollOver), "OnMouseEnter")]
        public static void TraitRollOverOnMouseEnter()
        {
            Log.LogDebug("TrailRollOver OnMouseEnter Prefix");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TraitRollOver), "OnMouseEnter")]
        public static void TraitRollOverOnMouseEnter(ref TraitRollOver __instance)
        {
            Log.LogDebug("TrailRollOver OnMouseEnter Prefix");
            //if (__instance.td)
        }


        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapManager), "IncludeMapPrefab")]
        public static void IncludeMapPrefabPrefix(ref MapManager __instance, string nodeId)
        {
            Log.LogDebug("IncludeMapPrefabPrefix, worldtransform children: " + __instance.worldTransform.childCount);
            Log.LogDebug("nodeId is " + nodeId);
            NodeData medsND = Globals.Instance.GetNodeData(nodeId);
            Log.LogDebug(medsND.NodeId);
            Log.LogDebug(medsND.NodeZone.ZoneId);

            for (int index1 = 0; index1 < __instance.mapList.Count; ++index1)
            {
                //Log.LogDebug("index1: " + index1.ToString());
                if (__instance.mapList[index1].name.ToLower() == medsND.NodeZone.ZoneId.ToLower())
                {

                    //Log.LogDebug("FOUND IT");
                    bool flag2 = false;
                    for (int index2 = 0; index2 < __instance.worldTransform.childCount; ++index2)
                    {
                        if (__instance.worldTransform.GetChild(index2).gameObject.name == __instance.mapList[index1].name)
                        {
                            //Log.LogDebug("FLAG2");
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        //Log.LogDebug("NOFLAG2");
                        //UnityEngine.Object.Instantiate<GameObject>(__instance.mapList[index1], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, __instance.worldTransform).name = __instance.mapList[index1].name;
                    }
                }
                else
                {
                    //Log.LogDebug("Did not find: " + __instance.mapList[index1].name.ToLower());
                }
            }
        }*/

        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterItem), "SetParalyze")]
        public static void SetParalyzePrefix(ref CharacterItem __instance, bool state)
        {
            List<SpriteRenderer> medsAnimatedSprites = Traverse.Create(__instance).Field("animatedSprites").GetValue<List<SpriteRenderer>>();
            Animator medsAnim = Traverse.Create(__instance).Field("anim").GetValue<Animator>();
            Dictionary<string, Material> medsAnimatedSpritesDefaultMaterial = Traverse.Create(__instance).Field("animatedSpritesDefaultMaterial").GetValue<Dictionary<string, Material>>();
            SpriteRenderer medsCharImageSR = Traverse.Create(__instance).Field("charImageSR").GetValue<SpriteRenderer>();
            Transform medsShadowSprite = Traverse.Create(__instance).Field("shadowSprite").GetValue<Transform>();
            Plugin.Log.LogDebug("Paralyze1");
            if (state && __instance.IsItemParalyzed())
                return;
            Plugin.Log.LogDebug("Paralyze2");
            if (!state && !__instance.IsItemParalyzed())
            {
                Plugin.Log.LogDebug("Paralyze2a");
                if ((UnityEngine.Object)medsAnim != (UnityEngine.Object)null)
                    medsAnim.speed = 1f;
                Plugin.Log.LogDebug("Paralyze2b");
                if (__instance.IsItemStealth() || __instance.IsItemTaunt())
                    return;
            }
            Plugin.Log.LogDebug("Paralyze3");
            if (medsAnimatedSprites != null && medsAnimatedSprites.Count > 0)
            {
                Plugin.Log.LogDebug("Paralyze3a");
                if (state && (UnityEngine.Object)medsAnimatedSprites[0].sharedMaterial == (UnityEngine.Object)__instance.paralyzeMaterial || !state && (UnityEngine.Object)medsAnimatedSprites[0].sharedMaterial == (UnityEngine.Object)medsAnimatedSpritesDefaultMaterial[medsAnimatedSprites[0].name])
                    return;
                Plugin.Log.LogDebug("Paralyze3b");
                if (state)
                {
                    Plugin.Log.LogDebug("Paralyze3bi");
                    if ((double)medsAnim.speed > 0.0)
                    {
                        Plugin.Log.LogDebug("Paralyze3bi1");
                        medsAnim.SetTrigger("hit");
                        // surely the below isn't it
                        //this.StartCoroutine(this.StopAnim());
                    }
                }
                else
                {
                    Plugin.Log.LogDebug("Paralyze3bii");
                    medsAnim.speed = 1f;
                }
                Plugin.Log.LogDebug("Paralyze3c");
                for (int index = 0; index < medsAnimatedSprites.Count; ++index)
                {
                    Plugin.Log.LogDebug("Paralyze3c index" + index.ToString());
                    if (state)
                    {
                        if ((bool)(UnityEngine.Object)medsAnimatedSprites[index].transform.GetComponent("StealthHide"))
                        {
                            if (medsAnimatedSprites[index].gameObject.activeSelf)
                                medsAnimatedSprites[index].transform.gameObject.SetActive(false);
                        }
                        else
                            medsAnimatedSprites[index].sharedMaterial = __instance.paralyzeMaterial;
                    }
                    else if ((bool)(UnityEngine.Object)medsAnimatedSprites[index].transform.GetComponent("StealthHide"))
                    {
                        if (!medsAnimatedSprites[index].gameObject.activeSelf)
                            medsAnimatedSprites[index].transform.gameObject.SetActive(true);
                    }
                    else
                        medsAnimatedSprites[index].sharedMaterial = medsAnimatedSpritesDefaultMaterial[medsAnimatedSprites[index].name];
                }
            }
            else if (state)
            {
                Plugin.Log.LogDebug("Paralyze3d");
                medsCharImageSR.sharedMaterial = __instance.paralyzeMaterial;
            }
            else
            {
                Plugin.Log.LogDebug("Paralyze3e");
                Plugin.Log.LogDebug("Paralyze3ei SR.name: " + medsCharImageSR.name);
                medsCharImageSR.sharedMaterial = medsAnimatedSpritesDefaultMaterial[medsCharImageSR.name];
                Plugin.Log.LogDebug("Paralyze3eii");
            }
            Plugin.Log.LogDebug("Paralyze4");
            if (state || !((UnityEngine.Object)medsShadowSprite != (UnityEngine.Object)null) || medsShadowSprite.gameObject.activeSelf)
                return;
            Plugin.Log.LogDebug("Paralyze5");
            medsShadowSprite.gameObject.SetActive(true);
        }
        */
        /*
        // JANK TIME! WEE WOO
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuSaveButton), "SetGameData")]
        public static void SetGameDataPrefix(GameData _gameData, ref MenuSaveButton __instance)
        {
            NodeData nD = Globals.Instance.GetNodeData(_gameData.CurrentMapNode);
            if (_gameData.GameType == Enums.GameType.Adventure && (UnityEngine.Object)nD != (UnityEngine.Object)null && (UnityEngine.Object)nD.NodeZone != (UnityEngine.Object)null)
            {
                Plugin.Log.LogDebug("SetGameData zoneID: " + nD.NodeZone.ZoneId.ToLower());
                if (Globals.Instance.ZoneDataSource.ContainsKey(nD.NodeZone.ZoneId.ToLower()))
                {
                    Plugin.Log.LogDebug("CONTAINS ZONEID!");
                    string s = Globals.Instance.ZoneDataSource[nD.NodeZone.ZoneId.ToLower()].ZoneName;
                    Plugin.Log.LogDebug("getting text: " + s);
                    Plugin.Log.LogDebug("result: " + Texts.Instance.GetText(s));
                }
                else
                {
                    Plugin.Log.LogDebug("DOESNOTCONTAIN ZONEID!");
                    string s = nD.NodeZone.ZoneId.ToLower();
                    Plugin.Log.LogDebug("getting text: " + s);
                    Plugin.Log.LogDebug("result: " + Texts.Instance.GetText(s));
                }
            }
        }*/

    }
}
