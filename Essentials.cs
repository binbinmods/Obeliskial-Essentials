using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using static Enums;
using Steamworks.Data;
using Steamworks;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static Unity.Audio.Handle;
using System.Text;


/*
FULL LIST OF ATO CLASSES->METHODS THAT ARE PATCHED:
    
*/

namespace Obeliskial_Essentials
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("AcrossTheObelisk.exe")]
    public class Essentials : BaseUnityPlugin
    {
        internal const int ModDate = 20231110;
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;

        public static ConfigEntry<bool> medsExportJSON { get; private set; }
        public static ConfigEntry<bool> medsExportSprites { get; private set; }
        public static ConfigEntry<bool> medsShowAtStart { get; private set; }
        internal static Dictionary<string, string> medsNodeEvent = new();
        internal static Dictionary<string, int> medsNodeEventPercent = new();
        internal static Dictionary<string, int> medsNodeEventPriority = new();
        internal static Dictionary<string, EventReplyDataText> medsEventReplyDataText = new();
        internal static Dictionary<string, Node> medsNodeSource = new();
        public static List<string> medsAllThePetsCards = new();
        public static List<string> medsDropOnlyItems = new();
        public static Dictionary<Enums.CardType, List<string>> medsBasicCardListByType = new();
        public static Dictionary<Enums.CardClass, List<string>> medsBasicCardListByClass = new();
        public static List<string> medsBasicCardListNotUpgraded = new();
        public static Dictionary<Enums.CardClass, List<string>> medsBasicCardListNotUpgradedByClass = new();
        public static Dictionary<string, List<string>> medsBasicCardListByClassType = new();
        public static Dictionary<Enums.CardType, List<string>> medsBasicCardItemByType = new();
        public static Dictionary<string, string> medsCustomCardDescriptions = new();
        public static RewardsManager RewardsManagerInstance;
        internal static string medsVersionText = "";
        public static readonly string[] vanillaSubclasses = { "mercenary", "sentinel", "berserker", "warden", "ranger", "assassin", "archer", "minstrel", "elementalist", "pyromancer", "loremaster", "warlock", "cleric", "priest", "voodoowitch", "prophet", "bandit", "fallen", "paladin" };
        public static Dictionary<string, string> medsTexts = new();
        private static List<string> medsExportedSpritePaths = new();
        private void Awake()
        {
            Log = Logger;
            LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");
            medsExportJSON = Config.Bind(new ConfigDefinition("Debug", "Export Vanilla Content"), false, new ConfigDescription("Export AtO class data to JSON files that are compatible with Obeliskial Content."));
            medsExportSprites = Config.Bind(new ConfigDefinition("Debug", "Export Sprites"), true, new ConfigDescription("Export sprites when exporting JSON files."));
            medsShowAtStart = Config.Bind(new ConfigDefinition("Debug", "Show At Start"), true, new ConfigDescription("Show the mod version window when the game loads."));
            UniverseLib.Universe.Init(1f, ObeliskialUI.InitUI, LogHandler, new()
            {
                Disable_EventSystem_Override = false, // or null
                Force_Unlock_Mouse = true, // or null
                Unhollowed_Modules_Folder = null
            });
            harmony.PatchAll();
        }
        internal static void LogDebug(string msg)
        {
            Log.LogDebug(msg);
        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(msg);
        }
        internal static void LogWarning(string msg)
        {
            Log.LogWarning(msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(msg);
        }
        void LogHandler(string message, UnityEngine.LogType type)
        {
            string log = message?.ToString() ?? "";
            switch (type)
            {
                case UnityEngine.LogType.Assert:
                case UnityEngine.LogType.Log:
                    LogInfo(log);
                    break;
                case UnityEngine.LogType.Warning:
                    LogWarning(log);
                    break;
                case UnityEngine.LogType.Error:
                case UnityEngine.LogType.Exception:
                    LogError(log);
                    break;
            }
        }

        public static void ExportSprite(Sprite spriteToExport, string spriteType, string subType = "", string subType2 = "", bool fullTextureExport = false)
        {
            if (spriteToExport.textureRect.width == 0 || spriteToExport.textureRect.height == 0) { return; }
            LogDebug("Exporting sprite: " + spriteToExport.name);
            string filePath = Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "sprite", spriteType);
            if (!subType.IsNullOrWhiteSpace())
            {
                filePath = Path.Combine(filePath, subType);
                if (!subType2.IsNullOrWhiteSpace())
                    filePath = Path.Combine(filePath, subType2);
            }
            Texture2D finalImage;
            Texture2D readableText;
            FolderCreate(filePath);
            filePath = Path.Combine(filePath, (fullTextureExport ? spriteToExport.texture.name : spriteToExport.name) + ".png");
            if (medsExportedSpritePaths.Contains(filePath))
                return;
            RenderTexture renderTex = RenderTexture.GetTemporary((int)spriteToExport.texture.width, (int)spriteToExport.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            // we flip it when doing the Graphics.Blit because the sprites are packed (which... flips them? idk?)
            Graphics.Blit(spriteToExport.texture, renderTex, new Vector2(1, -1), new Vector2(0, 1));
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            readableText = fullTextureExport ? new((int)spriteToExport.texture.width, (int)spriteToExport.texture.height) : new((int)spriteToExport.textureRect.width, (int)spriteToExport.textureRect.height);
            readableText.ReadPixels(fullTextureExport ? new Rect(0, 0, spriteToExport.texture.width, spriteToExport.texture.height) : new Rect(spriteToExport.textureRect.x, spriteToExport.textureRect.y, spriteToExport.textureRect.width, spriteToExport.textureRect.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            // flip it back
            finalImage = fullTextureExport ? new((int)spriteToExport.texture.width, (int)spriteToExport.texture.height) : new((int)spriteToExport.textureRect.width, (int)spriteToExport.textureRect.height);
            for (int i = 0; i < readableText.width; i++)
                for (int j = 0; j < readableText.height; j++)
                    finalImage.SetPixel(i, readableText.height - j - 1, readableText.GetPixel(i, j));
            finalImage.Apply();
            File.WriteAllBytes(filePath, ImageConversion.EncodeToPNG(finalImage));
            medsExportedSpritePaths.Add(filePath);
        }
        public static void FolderCreate(string folderPath)
        {
            DirectoryInfo medsDI = new(folderPath);
            if (!medsDI.Exists)
                medsDI.Create();
        }
        public static void AddModVersionText(string sModName, string sModVersion, string sModDate)
        {
            string newText = sModName + " v" + sModVersion + (sModDate.IsNullOrWhiteSpace() ? "" : (" (" + sModDate + ")"));
            if (medsVersionText.IsNullOrWhiteSpace())
                medsVersionText = newText;
            else if (!medsVersionText.Contains(newText))
                medsVersionText += "\n" + newText;
        }

        public static void ExtractData<T>(T[] data)
        {
            //string combined = "{";
            //int h = 1; // counts hundreds for combined files
            for (int a = 1; a <= data.Length; a++)
            {
                string type = "";
                string id = "";
                string text = "";
                string textFULL = "";
                bool pretty = true;
                if (data[a - 1].GetType() == typeof(SubClassData))
                {
                    type = "subclass";
                    SubClassData d = (SubClassData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(TraitData))
                {
                    type = "trait";
                    TraitData d = (TraitData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CardData))
                {
                    type = "card";
                    CardData d = (CardData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(PerkData))
                {
                    type = "perk";
                    PerkData d = (PerkData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(AuraCurseData))
                {
                    type = "auraCurse";
                    AuraCurseData d = (AuraCurseData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(NPCData))
                {
                    type = "npc";
                    NPCData d = (NPCData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(NodeData))
                {
                    type = "node";
                    NodeData d = (NodeData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                    textFULL = JsonUtility.ToJson(DataTextConvert.ToFULLText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(LootData))
                {
                    type = "loot";
                    LootData d = (LootData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(PerkNodeData))
                {
                    type = "perkNode";
                    PerkNodeData d = (PerkNodeData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(ChallengeData))
                {
                    type = "challengeData";
                    ChallengeData d = (ChallengeData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(ChallengeTrait))
                {
                    type = "challengeTrait";
                    ChallengeTrait d = (ChallengeTrait)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CombatData))
                {
                    type = "combatData";
                    CombatData d = (CombatData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(EventData))
                {
                    type = "event";
                    EventData d = (EventData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(EventReplyDataText))
                {
                    type = "eventReply";
                    EventReplyDataText d = (EventReplyDataText)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(d, pretty);
                }
                else if (data[a - 1].GetType() == typeof(EventRequirementData))
                {
                    type = "eventRequirement";
                    EventRequirementData d = (EventRequirementData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(ZoneData))
                {
                    type = "zone";
                    ZoneData d = (ZoneData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(KeyNotesData))
                {
                    type = "keynote";
                    KeyNotesData d = (KeyNotesData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(PackData))
                {
                    type = "pack";
                    PackData d = (PackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CardPlayerPackData))
                {
                    type = "cardPlayerPack";
                    CardPlayerPackData d = (CardPlayerPackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CardPlayerPairsPackData))
                {
                    type = "pairsPack";
                    CardPlayerPairsPackData d = (CardPlayerPairsPackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(ItemData))
                {
                    type = "item";
                    ItemData d = (ItemData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CardbackData))
                {
                    type = "cardback";
                    CardbackData d = (CardbackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(SkinData))
                {
                    type = "skin";
                    SkinData d = (SkinData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CorruptionPackData))
                {
                    type = "corruptionPack";
                    CorruptionPackData d = (CorruptionPackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(CinematicData))
                {
                    type = "cinematic";
                    CinematicData d = (CinematicData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else if (data[a - 1].GetType() == typeof(TierRewardData))
                {
                    type = "tierReward";
                    TierRewardData d = (TierRewardData)(object)data[a - 1];
                    id = d.TierNum.ToString();
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d), pretty);
                }
                else
                {
                    Log.LogError("Unknown type while extracting data: " + data[a - 1].GetType());
                    return;
                }
                //text = text.Replace(@""":false,", @""":0,").Replace(@""":false}", @""":0}").Replace(@""":true,", @""":1,").Replace(@""":true}", @""":1}");
                if (a == 1)
                {
                    FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", type));
                    File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + ".json"), "[");
                    if (textFULL != "")
                    {
                        FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", type + "_FULL"));
                        File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + "_FULL.json"), "[");
                    }
                }
                WriteToJSON(type, text, id);
                if (textFULL != "")
                    WriteToJSON(type + "_FULL", textFULL, id);
                if (a == data.Length)
                {
                    // WriteToJSON(type, combined.Remove(combined.Length - 1) + "}", a, h);
                    File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + ".json"), text + "]");
                    if (textFULL != "")
                        File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + "_FULL.json"), textFULL + "]");
                    Log.LogInfo("exported " + a + " " + type + " values!");
                }
                else
                {
                    File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + ".json"), text + ",");
                    if (textFULL != "")
                        File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + "_FULL.json"), textFULL + ",");
                }
            }
        }

        public static void WriteToJSON(string exportType, string exportText, string exportID)
        {
            FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", exportType));
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", exportType, exportID + ".json"), exportText);
        }
        public static bool IsHost()
        {
            if ((GameManager.Instance.IsMultiplayer() && NetworkManager.Instance.IsMaster()) || !GameManager.Instance.IsMultiplayer())
                return true;
            return false;
        }
        public static int TeamHeroToInt(Hero[] medsTeam)
        {
            int team = 0;
            for (int index = 0; index < 4; ++index)
                team += (Array.IndexOf(vanillaSubclasses, medsTeam[index].SubclassName) + 1) * (int)Math.Pow(100, index);
            LogDebug("TeamHeroToInt: " + team);
            return team;
        }
        public static string TeamIntToString(int team)
        {
            int[] iTeam = new int[4];
            string[] sTeam = new string[4];

            iTeam[3] = team / 1000000;
            iTeam[2] = (team % 1000000) / 10000;
            iTeam[1] = (team % 10000) / 100;
            iTeam[0] = (team % 100);
            for (int a = 0; a < 4; a++)
            {
                if (iTeam[a] < 1 || iTeam[a] > vanillaSubclasses.Length)
                    sTeam[a] = "UNKNOWN";
                else
                    sTeam[a] = vanillaSubclasses[iTeam[a] - 1];
            }
            LogDebug("TeamIntToString: " + string.Join(", ", sTeam));
            return string.Join(", ", sTeam);
        }
        public static async Task SetScoreLeaderboard(int score, bool singleplayer = true, string mode = "RankingAct4")
        {
            int gameId32 = Functions.StringToAsciiInt32(AtOManager.Instance.GetGameId());
            int details = Convert.ToInt32(gameId32 + score * 101);

            int seed = AtOManager.Instance.GetGameId().GetDeterministicHashCode();

            int team = TeamHeroToInt(AtOManager.Instance.GetTeam());
            int nodes = 0; // #TODO: nodelist
            string[] gameVersion = GameManager.Instance.gameVersion.Split(".");
            int vanillaVersion = int.Parse(gameVersion[0]) * 10000 + int.Parse(gameVersion[1]) * 100 + int.Parse(gameVersion[2]);
            int obeliskialVersion = ModDate;


            Leaderboard? leaderboardAsync = await SteamUserStats.FindLeaderboardAsync(mode + (singleplayer ? "" : "Coop"));
            if (leaderboardAsync.HasValue)
            {
                LeaderboardUpdate? nullable = await leaderboardAsync.Value.SubmitScoreAsync(score, new int[7]
                {
                        gameId32,
                        details,
                        vanillaVersion,
                        obeliskialVersion,
                        seed,
                        team,
                        nodes
                });
            }
            else
                Debug.Log((object)"Couldn't Get Leaderboard!");
        }
        public static void OptimalPathSeed()
        {
            List<string[]> nodeList = new();
            // Senenthia: 2 events, 4 corruptors
            nodeList.Add(new string[4] { "Betty", "sen_6", "e_sen6_b", "Senenthia" });
            nodeList.Add(new string[4] { "combat", "sen_9", "", "Senenthia" });
            nodeList.Add(new string[4] { "combat", "secta_2", "", "Senenthia" });
            nodeList.Add(new string[4] { "combat", "sen_19", "", "Senenthia" });
            nodeList.Add(new string[4] { "Soldier Trainer", "sen_37", "e_sen37_a", "Senenthia" });
            nodeList.Add(new string[4] { "combat", "sen_28", "", "Senenthia" });

            // Aquarfall: 7 corruptors
            nodeList.Add(new string[4] { "combat", "aqua_4", "", "Aquarfall" });
            nodeList.Add(new string[4] { "combat", "aqua_12", "", "Aquarfall" });
            nodeList.Add(new string[4] { "combat", "aqua_10", "", "Aquarfall" });
            nodeList.Add(new string[4] { "combat", "aqua_15", "", "Aquarfall" });
            nodeList.Add(new string[4] { "combat", "spider_3", "", "Aquarfall" });
            nodeList.Add(new string[4] { "combat", "spider_4", "", "Aquarfall" });
            nodeList.Add(new string[4] { "combat", "aqua_33", "", "Aquarfall" });

            // Faeborg: 2 events, 6 corruptors
            /*nodeList.Add(new string[4] { "Monster Trainer", "faen_7", "", "Faeborg" });
            nodeList.Add(new string[4] { "combat", "faen_8", "", "Faeborg" });
            nodeList.Add(new string[4] { "combat", "faen_14", "", "Faeborg" });
            nodeList.Add(new string[4] { "combat", "faen_24", "", "Faeborg" });
            nodeList.Add(new string[4] { "Binks", "faen_40", "e_faen40_a", "Faeborg" });
            nodeList.Add(new string[4] { "Charls", "faen_40", "e_faen40_b", "Faeborg" });
            nodeList.Add(new string[4] { "combat", "sewers_2", "", "Faeborg" });
            nodeList.Add(new string[4] { "combat", "sewers_12", "", "Faeborg" });
            nodeList.Add(new string[4] { "combat", "faen_37", "", "Faeborg" }); */

            // Velkarath: 6 corruptors
            nodeList.Add(new string[4] { "combat", "velka_2", "", "Velkarath" });
            nodeList.Add(new string[4] { "combat", "velka_5", "", "Velkarath" });
            nodeList.Add(new string[4] { "combat", "velka_13", "", "Velkarath" });
            nodeList.Add(new string[4] { "combat", "forge_1", "", "Velkarath" }); // using upper path because it's more consistent
            nodeList.Add(new string[4] { "combat", "velka_28", "", "Velkarath" });
            nodeList.Add(new string[4] { "combat", "velka_31", "", "Velkarath" });

            // Voidlow: 1 event, 5 corruptors
            nodeList.Add(new string[4] { "combat", "voidlow_2", "", "Voidlow" });
            nodeList.Add(new string[4] { "combat", "voidlow_9", "", "Voidlow" });
            nodeList.Add(new string[4] { "combat", "voidlow_10", "", "Voidlow" });
            nodeList.Add(new string[4] { "Chromatic Slime", "voidlow_27", "e_voidlow27_a", "Voidlow" });
            nodeList.Add(new string[4] { "combat", "voidlow_19", "", "Voidlow" });
            nodeList.Add(new string[4] { "combat", "voidlow_22", "", "Voidlow" });

            // Voidhigh: 2 corruptors
            nodeList.Add(new string[4] { "combat", "voidhigh_2", "", "Voidhigh" });
            nodeList.Add(new string[4] { "combat", "voidhigh_10", "", "Voidhigh" });

            for (int a = 1325259; a <= 9999999; a++)
            {
                string seed = a.ToString();
                CheckSeed(seed, nodeList);
            }

            /*for (int a = 0; a <= 9; a++)
            {
                for (int b = 0; b <= 9; b++)
                {
                    for (int c = 0; c <= 9; c++)
                    {
                        for (int d = 0; d <= 9; d++)
                        {
                            for (int e = 0; e <= 9; e++)
                            {
                                for (int f = 0; f <= 9; f++)
                                {
                                    for (int g = 0; g <= 9; g++)
                                    {
                                        string seed = a.ToString() + b.ToString() + c.ToString() + d.ToString() + e.ToString() + f.ToString() + g.ToString();
                                        CheckSeed(seed, nodeList);
                                    }
                                }
                            }
                        }
                    }
                }
            }*/
        }

        public static void CheckSeed(string seed, List<string[]> nodeList)
        {
            int medsCommon = 0;
            int medsUncommon = 0;
            int medsRare = 0;
            int medsEpic = 0;
            int medsEvents = 0;
            Dictionary<string, int> zoneEventCount = new();
            Dictionary<string, int> zoneCommonCount = new();
            Dictionary<string, int> zoneUncommonCount = new();
            Dictionary<string, int> zoneRareCount = new();
            Dictionary<string, int> zoneEpicCount = new();
            zoneEventCount["Senenthia"] = 0;
            zoneEventCount["Aquarfall"] = 0;
            zoneEventCount["Faeborg"] = 0;
            zoneEventCount["Velkarath"] = 0;
            zoneEventCount["Voidlow"] = 0;
            zoneEventCount["Voidhigh"] = 0;
            zoneCommonCount["Senenthia"] = 0;
            zoneCommonCount["Aquarfall"] = 0;
            zoneCommonCount["Faeborg"] = 0;
            zoneCommonCount["Velkarath"] = 0;
            zoneCommonCount["Voidlow"] = 0;
            zoneCommonCount["Voidhigh"] = 0;
            zoneUncommonCount["Senenthia"] = 0;
            zoneUncommonCount["Aquarfall"] = 0;
            zoneUncommonCount["Faeborg"] = 0;
            zoneUncommonCount["Velkarath"] = 0;
            zoneUncommonCount["Voidlow"] = 0;
            zoneUncommonCount["Voidhigh"] = 0;
            zoneRareCount["Senenthia"] = 0;
            zoneRareCount["Aquarfall"] = 0;
            zoneRareCount["Faeborg"] = 0;
            zoneRareCount["Velkarath"] = 0;
            zoneRareCount["Voidlow"] = 0;
            zoneRareCount["Voidhigh"] = 0;
            zoneEpicCount["Senenthia"] = 0;
            zoneEpicCount["Aquarfall"] = 0;
            zoneEpicCount["Faeborg"] = 0;
            zoneEpicCount["Velkarath"] = 0;
            zoneEpicCount["Voidlow"] = 0;
            zoneEpicCount["Voidhigh"] = 0;

            foreach (string[] nodeData in nodeList)
            {
                NodeData _node = Globals.Instance.GetNodeData(nodeData[1]);

                // Log.LogInfo("DHS: " + (_node.NodeId + seed + nameof(AtOManager.Instance.AssignSingleGameNode)));
                UnityEngine.Random.InitState((_node.NodeId + seed + nameof(AtOManager.Instance.AssignSingleGameNode)).GetDeterministicHashCode());
                // Log.LogInfo("DHC: " + (_node.NodeId + seed + nameof(AtOManager.Instance.AssignSingleGameNode)).GetDeterministicHashCode());
                if (UnityEngine.Random.Range(0, 100) < _node.ExistsPercent)
                {
                    bool flag1 = true;
                    bool flag2 = true;
                    if (_node.NodeEvent != null && _node.NodeEvent.Length != 0 && _node.NodeCombat != null && _node.NodeCombat.Length != 0)
                    {
                        if (UnityEngine.Random.Range(0, 100) < _node.CombatPercent)
                            flag1 = false;
                        else
                            flag2 = false;
                    }

                    if (flag1 && _node.NodeEvent != null && _node.NodeEvent.Length != 0) // event!
                    {
                        string str = "";
                        Dictionary<string, int> source = new Dictionary<string, int>();
                        for (int index = 0; index < _node.NodeEvent.Length; ++index)
                        {
                            int num = 10000;
                            if (index < _node.NodeEventPriority.Length)
                                num = _node.NodeEventPriority[index];
                            source.Add(_node.NodeEvent[index].EventId, num);
                        }
                        if (source.Count > 0)
                        {
                            Dictionary<string, int> dictionary1 = source.OrderBy<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>)(x => x.Value)).ToDictionary<KeyValuePair<string, int>, string, int>((Func<KeyValuePair<string, int>, string>)(x => x.Key), (Func<KeyValuePair<string, int>, int>)(x => x.Value));
                            int num1 = 1;
                            int num2 = dictionary1.ElementAt<KeyValuePair<string, int>>(0).Value;
                            while (num1 < dictionary1.Count && dictionary1.ElementAt<KeyValuePair<string, int>>(num1).Value == num2)
                                ++num1;
                            if (num1 == 1)
                            {
                                str = dictionary1.ElementAt<KeyValuePair<string, int>>(0).Key;
                            }
                            else
                            {
                                if (_node.NodeEventPercent != null && _node.NodeEvent.Length == _node.NodeEventPercent.Length)
                                {
                                    Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
                                    int index1 = 0;
                                    for (int index2 = 0; index2 < num1; ++index2)
                                    {
                                        int index3 = 0;
                                        while (index2 < _node.NodeEvent.Length)
                                        {
                                            if (_node.NodeEvent[index3].EventId == dictionary1.ElementAt<KeyValuePair<string, int>>(index1).Key)
                                            {
                                                dictionary2.Add(_node.NodeEvent[index3].EventId, _node.NodeEventPercent[index3]);
                                                ++index1;
                                                break;
                                            }
                                            ++index3;
                                        }
                                    }
                                    int num3 = UnityEngine.Random.Range(0, 100);
                                    int num4 = 0;
                                    foreach (KeyValuePair<string, int> keyValuePair in dictionary2)
                                    {
                                        num4 += keyValuePair.Value;
                                        if (num3 < num4)
                                        {
                                            str = keyValuePair.Key;
                                            break;
                                        }
                                    }
                                }
                                if (str == "")
                                {
                                    int index = UnityEngine.Random.Range(0, num1);
                                    str = dictionary1.ElementAt<KeyValuePair<string, int>>(index).Key;
                                }
                            }
                            if (str == nodeData[2])
                            {
                                // this is the event we want!
                                medsEvents++;
                                zoneEventCount[nodeData[3]]++;
                            }
                        }
                    }
                    else if (nodeData[0] == "combat" && flag2 && _node.NodeCombat != null && _node.NodeCombat.Length != 0) // combat!
                    {
                        string combatID = _node.NodeCombat[0].CombatId;
                        string str = _node.NodeId + seed;
                        int deterministicHashCode = str.GetDeterministicHashCode();
                        UnityEngine.Random.InitState(deterministicHashCode);

                        List<string> stringList = new List<string>();
                        for (int index = 0; index < Globals.Instance.CardListByType[Enums.CardType.Corruption].Count; ++index)
                        {
                            CardData cardData = Globals.Instance.GetCardData(Globals.Instance.CardListByType[Enums.CardType.Corruption][index], false);
                            if ((UnityEngine.Object)cardData != (UnityEngine.Object)null && !cardData.OnlyInWeekly)
                                stringList.Add(Globals.Instance.CardListByType[Enums.CardType.Corruption][index]);
                        }
                        bool flag3 = false;
                        int medsRandomCorruptionIndex;
                        string medsCorruptionIdCard = "";
                        CardData medsCDataCorruption = null;
                        while (!flag3)
                        {
                            int index1 = UnityEngine.Random.Range(0, stringList.Count);
                            medsRandomCorruptionIndex = index1;
                            medsCorruptionIdCard = stringList[index1];

                            if (!(medsCorruptionIdCard == "resurrection") && !(medsCorruptionIdCard == "resurrectiona") && !(medsCorruptionIdCard == "resurrectionb") && !(medsCorruptionIdCard == "resurrectionrare"))
                            {
                                for (int index2 = 0; index2 < deterministicHashCode % 10; ++index2)
                                    UnityEngine.Random.Range(0, 100);
                                medsCDataCorruption = Globals.Instance.GetCardData(medsCorruptionIdCard, false);
                                if (!((UnityEngine.Object)medsCDataCorruption == (UnityEngine.Object)null) && (!medsCDataCorruption.OnlyInWeekly))
                                    flag3 = true;
                            }
                        }

                        if ((UnityEngine.Object)medsCDataCorruption == (UnityEngine.Object)null)
                            medsCDataCorruption = Globals.Instance.GetCardData(medsCorruptionIdCard, false);
                        if (medsCDataCorruption.CardRarity == CardRarity.Common)
                        {
                            zoneCommonCount[nodeData[3]]++;
                            medsCommon++;
                        }
                        else if (medsCDataCorruption.CardRarity == CardRarity.Uncommon)
                        {
                            zoneUncommonCount[nodeData[3]]++;
                            medsUncommon++;
                        }
                        else if (medsCDataCorruption.CardRarity == CardRarity.Rare)
                        {
                            zoneRareCount[nodeData[3]]++;
                            medsRare++;
                        }
                        else if (medsCDataCorruption.CardRarity == CardRarity.Epic)
                        {
                            zoneEpicCount[nodeData[3]]++;
                            medsEpic++;
                        }
                    }
                }
            }
            /*string z = "Senenthia";
            Log.LogInfo("SEED " + seed + ": SENEN " + zoneEventCount[z] + "/2 events, " + (zoneCommonCount[z] + zoneUncommonCount[z] + zoneRareCount[z] + zoneEpicCount[z]).ToString() + "/4 combats (" + zoneEpicCount[z] + "E " + zoneRareCount[z] + "R " + zoneUncommonCount[z] + "U " + zoneCommonCount[z] + "C)");
            z = "Aquarfall";
            Log.LogInfo("SEED " + seed + ": AQUAR " + (zoneCommonCount[z] + zoneUncommonCount[z] + zoneRareCount[z] + zoneEpicCount[z]).ToString() + "/7 combats (" + zoneEpicCount[z] + "E " + zoneRareCount[z] + "R " + zoneUncommonCount[z] + "U " + zoneCommonCount[z] + "C)");
            // z = "Faeborg";
            // Log.LogInfo("SEED " + seed + ": FAEBO " + zoneEventCount[z] + "/2 events, " + (zoneCommonCount[z] + zoneUncommonCount[z] + zoneRareCount[z] + zoneEpicCount[z]).ToString() + "/6 combats (" + zoneEpicCount[z] + "E " + zoneRareCount[z] + "R " + zoneUncommonCount[z] + "U " + zoneCommonCount[z] + "C)");
            z = "Velkarath";
            Log.LogInfo("SEED " + seed + ": VELKA " + (zoneCommonCount[z] + zoneUncommonCount[z] + zoneRareCount[z] + zoneEpicCount[z]).ToString() + "/6 combats (" + zoneEpicCount[z] + "E " + zoneRareCount[z] + "R " + zoneUncommonCount[z] + "U " + zoneCommonCount[z] + "C)");
            z = "Voidlow";
            Log.LogInfo("SEED " + seed + ": VOIDL " + zoneEventCount[z] + "/1 events, " + (zoneCommonCount[z] + zoneUncommonCount[z] + zoneRareCount[z] + zoneEpicCount[z]).ToString() + "/5 combats (" + zoneEpicCount[z] + "E " + zoneRareCount[z] + "R " + zoneUncommonCount[z] + "U " + zoneCommonCount[z] + "C)");
            z = "Voidhigh";
            Log.LogInfo("SEED " + seed + ": VOIDH " + (zoneCommonCount[z] + zoneUncommonCount[z] + zoneRareCount[z] + zoneEpicCount[z]).ToString() + "/2 combats (" + zoneEpicCount[z] + "E " + zoneRareCount[z] + "R " + zoneUncommonCount[z] + "U " + zoneCommonCount[z] + "C)");*/
            Log.LogInfo("SEED " + seed + ": TOTAL " + medsEvents + "/3 events, " + (medsCommon + medsUncommon + medsRare + medsEpic).ToString() + "/24 combats (" + medsEpic + "E " + medsRare + "R " + medsUncommon + "U " + medsCommon + "C)");
        }

        public static void SetTeamExperience(int xp)
        {
            Hero[] medsTeamAtO = Traverse.Create(AtOManager.Instance).Field("teamAtO").GetValue<Hero[]>();
            for (int index = 0; index < medsTeamAtO.Length; ++index)
                medsTeamAtO[index].Experience = xp;
            Traverse.Create(AtOManager.Instance).Field("teamAtO").SetValue(medsTeamAtO);
        }

        public async static void CheckLeaderboards(string leaderboardType)
        {
            Leaderboard? leaderboard = new Leaderboard?();
            leaderboard = await SteamUserStats.FindLeaderboardAsync(leaderboardType);

            if (!leaderboard.HasValue)
            {
                Debug.Log((object)"Couldn't Get Leaderboard!");
            }
            else
            {
                LeaderboardEntry[] scoreboardGlobal = await leaderboard.Value.GetScoresAsync(450);
                Leaderboard leaderboard1 = leaderboard.Value;
                // LeaderboardEntry[] scoreboardFriends = await leaderboard1.GetScoresFromFriendsAsync();
                leaderboard1 = leaderboard.Value;
                LeaderboardEntry[] scoreboardSingle = await leaderboard1.GetScoresAroundUserAsync(0, 0);
                string theList = "ID\tScore\tDetails\t2\t3\t4\t5\t6\t7\t8\t9\t10";
                for (int a = 0; a < scoreboardGlobal.Length; a++)
                    theList += "\n" + scoreboardGlobal[a].User.Id.ToString() + "\t" + scoreboardGlobal[a].Score + "\t" + string.Join("\t", scoreboardGlobal[a].Details);
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "scoreboardGlobal.json"), theList);
                theList = "";
                for (int a = 0; a < scoreboardSingle.Length; a++)
                    theList += "\n" + scoreboardSingle[a].User.Id.ToString() + "\t" + scoreboardSingle[a].Score + "\t" + string.Join("\t", scoreboardSingle[a].Details);
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "scoreboardSingle.json"), theList);
            }
        }

        public static void FullCardSpriteOutput()
        {

            if (!((UnityEngine.Object)CardScreenManager.Instance != (UnityEngine.Object)null))
                return;
            SnapshotCamera snapshotCamera = SnapshotCamera.MakeSnapshotCamera(0);
            CardScreenManager.Instance.ShowCardScreen(true);
            // for each card in cards
            Dictionary<string, CardData> allCards = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            LogInfo("i herd u liek memory leaks ;)");
            foreach (KeyValuePair<string, CardData> kvp in allCards)
            {
                LogInfo("EXTRACTING CARD IMAGE:" + kvp.Key);
                CardScreenManager.Instance.SetCardData(kvp.Value);
                GameObject cardGO = Traverse.Create(CardScreenManager.Instance).Field("cardGO").GetValue<GameObject>();
                if ((UnityEngine.Object)cardGO != (UnityEngine.Object)null)
                {
                    cardGO.transform.Find("BorderCard").gameObject.SetActive(false);
                    Texture2D snapshot = snapshotCamera.TakeObjectSnapshot(cardGO, UnityEngine.Color.clear, new Vector3(0, 0.008f, 1), Quaternion.Euler(new Vector3(0f, 0f, 0f)), new Vector3(0.78f, 0.78f, 0.78f), 297, 450);
                    SnapshotCamera.SavePNG(snapshot, kvp.Key, Directory.CreateDirectory(Path.Combine(Application.dataPath, "../Card Images", DataTextConvert.ToString(kvp.Value.CardClass))).FullName);
                    /*if (kvp.Value.CardType == CardType.Corruption)
                        SnapshotCamera.SavePNG(snapshot, kvp.Key, Directory.CreateDirectory(Path.Combine(Application.dataPath, "../Card Images", DataTextConvert.ToString(kvp.Value.CardType))).FullName);*/
                    UnityEngine.Object.Destroy(snapshot);
                    UnityEngine.Object.Destroy(cardGO);
                }
            }
        }

        public static void WilburCardJSONExport()
        {
            string combinedCore = "{\"cards\":[";
            string combinedBoonInjury = "{\"cards\":[";
            string combinedItem = "{\"cards\":[";
            string combinedEnchantment = "{\"cards\":[";
            /*int h = 1; // counts hundreds for combined files
            int a = 1;*/
            foreach (string id in Globals.Instance.CardListNotUpgraded)
            {
                CardData card = Globals.Instance.GetCardData(id);
                if ((UnityEngine.Object)card != (UnityEngine.Object)null)
                {
                    string combined = "{\"name\":\"" + card.CardName + "\",\"cardTypes\":[\"" + Texts.Instance.GetText(DataTextConvert.ToString(card.CardType)) + "\"";
                    foreach (Enums.CardType t in card.CardTypeAux)
                        combined += ",\"" + Texts.Instance.GetText(DataTextConvert.ToString(t)) + "\"";
                    combined += "],\"versions\":{\"base\":" + WilburIndividualCardData(card);
                    // blue upgrade
                    CardData upgrade1 = Globals.Instance.GetCardData(card.UpgradesTo1);
                    if ((UnityEngine.Object)upgrade1 != (UnityEngine.Object)null)
                        combined += ",\"blue\":" + WilburIndividualCardData(upgrade1);
                    // yellow upgrade
                    CardData upgrade2 = Globals.Instance.GetCardData(card.UpgradesTo2);
                    if ((UnityEngine.Object)upgrade2 != (UnityEngine.Object)null)
                        combined += ",\"yellow\":" + WilburIndividualCardData(upgrade2);
                    // purple upgrade
                    if ((UnityEngine.Object)card.UpgradesToRare != (UnityEngine.Object)null)
                    {
                        // jank time? jank time.
                        CardData upgradeRare = Globals.Instance.GetCardData(card.UpgradesToRare.Id);
                        if ((UnityEngine.Object)upgradeRare != (UnityEngine.Object)null)
                            combined += ",\"rare\":" + WilburIndividualCardData(upgradeRare);
                    }
                    combined += "}},";
                    if (card.CardClass == Enums.CardClass.Warrior || card.CardClass == Enums.CardClass.Scout || card.CardClass == Enums.CardClass.Mage || card.CardClass == Enums.CardClass.Healer)
                    {
                        combinedCore += combined;
                    }
                    else if (card.CardClass == Enums.CardClass.Boon || card.CardClass == Enums.CardClass.Injury)
                    {
                        combinedBoonInjury += combined;
                    }
                    else if (card.CardClass == Enums.CardClass.Item || card.CardClass == Enums.CardClass.Enchantment || card.CardClass == Enums.CardClass.Special) // Enchantment cardClass isn't actually used, so this is just in case it _is_ used in the future
                    {
                        if ((UnityEngine.Object)card.ItemEnchantment != (UnityEngine.Object)null)
                        {
                            combinedEnchantment += combined;
                        }
                        else if ((UnityEngine.Object)card.Item != (UnityEngine.Object)null)
                        {
                            combinedItem += combined;
                        }
                        else
                        {
                            LogDebug("Did not export WilburCard " + id + " (invalid cardClass)");
                            continue;
                        }
                    }
                    else
                    {
                        LogDebug("Did not export WilburCard " + id + " (invalid cardClass)");
                        continue;
                    }
                    LogDebug("Exported WilburCard " + id);
                }
                else
                {
                    LogWarning("WARNING: could not WilburExport card " + id + " (could not find in Globals.Instance.Cards)");
                }
            }
            // remove trailing comma and write to file
            combinedCore = combinedCore.Remove(combinedCore.Length - 1) + "]}";
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "Wilbur_card_export_core.json"), combinedCore);
            combinedBoonInjury = combinedBoonInjury.Remove(combinedBoonInjury.Length - 1) + "]}";
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "Wilbur_card_export_booninjury.json"), combinedBoonInjury);
            combinedItem = combinedItem.Remove(combinedItem.Length - 1) + "]}";
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "Wilbur_card_export_item.json"), combinedItem);
            combinedEnchantment = combinedEnchantment.Remove(combinedEnchantment.Length - 1) + "]}";
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "Wilbur_card_export_enchantment.json"), combinedEnchantment);
            LogInfo("WilburExport complete!");
        }
        public static string WilburIndividualCardData(CardData card)
        {
            WilburCard wc = new();
            wc.target = card.Target;
            wc.cost = card.EnergyCost.ToString() + "<:ato_energy:1017803954308001902>";
            wc.description = WilburDescriptionCleaner(card.DescriptionNormalized);
            wc.vanish = card.Vanish;
            wc.innate = card.Innate;
            string unwieldy = JsonUtility.ToJson(wc).Replace(",\"innate\":false", "").Replace(",\"vanish\":false", "").Replace(@"\n\n", @"\n").Replace(@"\n\n", @"\n").Replace(@"\n- ", @"\n").Replace(@" -""", @"""");
            unwieldy = Regex.Replace(unwieldy, @"\\n(\d)", @" $1").Replace("  ", " ");
            return unwieldy;
        }
        public static string WilburDescriptionCleaner(string desc)
        {
            string newDesc = desc;
            // replace unused formatting tags (spacing, size, color, nobr, line height)
            newDesc = Regex.Replace(newDesc, @"<line-height=15%>\s*<[brBR]*>\s*<\/line-height>", "");
            newDesc = Regex.Replace(newDesc, @"<line-height=40%>\s*<[brBR]*>\s*<\/line-height>", "\n");
            newDesc = Regex.Replace(newDesc, @"<\/*(voffset|line-height|space|size|nobr|color)(=[\d#ABCDEFabcdef%.+-]*)*>", "");
            // replace <BR> with newline
            newDesc = newDesc.Replace("<BR>", "\n");
            newDesc = newDesc.Replace("<br>", "\n");
            // replace sprites with emojis
            newDesc = newDesc.Replace("<sprite name=bleed>", "<:ato_bleed:831708103178321941>");
            newDesc = newDesc.Replace("<sprite name=bless>", "<:ato_bless:831707950984593438>");
            newDesc = newDesc.Replace("<sprite name=block>", "<:ato_block:831707452901818368>");
            newDesc = newDesc.Replace("<sprite name=blunt>", "<:ato_blunt:831707206629064704>");
            newDesc = newDesc.Replace("<sprite name=buffer>", "<:ato_buffer:1017803855058182227>");
            newDesc = newDesc.Replace("<sprite name=burn>", "<:ato_burn:831708488195506202>");
            newDesc = newDesc.Replace("<sprite name=chill>", "<:ato_chill:831708639106170931>");
            newDesc = newDesc.Replace("<sprite name=cold>", "<:ato_cold:831708619330027561>");
            newDesc = newDesc.Replace("<sprite name=courage>", "<:ato_courage:1017803913040232488>");
            newDesc = newDesc.Replace("<sprite name=crack>", "<:ato_crack:831709232339615805>");
            newDesc = newDesc.Replace("<sprite name=dark>", "<:ato_dark:831708725571616788>");
            newDesc = newDesc.Replace("<sprite name=decay>", "<:ato_decay:831711525269536768>");
            newDesc = newDesc.Replace("<sprite name=disarm>", "<:ato_disarm:1017803928341065738>");
            newDesc = newDesc.Replace("<sprite name=doom>", "<:ato_doom:1017803942614282250>");
            newDesc = newDesc.Replace("<sprite name=energize>", "<:ato_energize:831710263631020072>");
            newDesc = newDesc.Replace("<sprite name=energy>", "<:ato_energy:1017803954308001902>");
            newDesc = newDesc.Replace("<sprite name=card>", "<:ato_card:1017803874498777118>");
            newDesc = newDesc.Replace("<sprite name=cards>", "<:ato_card:1017803874498777118>");
            newDesc = newDesc.Replace("<sprite name=evasion>", "<:ato_evasion:1017803990492254248>");
            newDesc = newDesc.Replace("<sprite name=fast>", "<:ato_fast:831709865957130260>");
            newDesc = newDesc.Replace("<sprite name=fatigue>", "<:ato_fatigue:1017804029348298882>");
            newDesc = newDesc.Replace("<sprite name=fire>", "<:ato_fire:831708462093697024>");
            newDesc = newDesc.Replace("<sprite name=fortify>", "<:ato_fortify:1017804040589017139>");
            newDesc = newDesc.Replace("<sprite name=fury>", "<:ato_fury:1017804054333771827>");
            newDesc = newDesc.Replace("<sprite name=heal>", "<:ato_heal:831708023822090292>");
            newDesc = newDesc.Replace("<sprite name=holy>", "<:ato_holy:831707893559853076>");
            newDesc = newDesc.Replace("<sprite name=health>", "<:ato_health:831711486132224070>");
            newDesc = newDesc.Replace("<sprite name=insane>", "<:ato_insane:831708800276627516>");
            newDesc = newDesc.Replace("<sprite name=inspire>", "<:ato_inspire:1017804069810741259>");
            newDesc = newDesc.Replace("<sprite name=insulate>", "<:ato_insulate:1017804082406232115>");
            newDesc = newDesc.Replace("<sprite name=invulnerable>", "<:ato_invulnerable:1017804093953159199>");
            newDesc = newDesc.Replace("<sprite name=lightning>", "<:ato_lightning:831708386659139646>");
            newDesc = newDesc.Replace("<sprite name=mark>", "<:ato_mark:831709147774320681>");
            newDesc = newDesc.Replace("<sprite name=mind>", "<:ato_mind:831708773490884619>");
            newDesc = newDesc.Replace("<sprite name=mitigate>", "<:ato_mitigate:1017804107995680848>");
            newDesc = newDesc.Replace("<sprite name=paralyze>", "<:ato_paralyze:1017804120633131018>");
            newDesc = newDesc.Replace("<sprite name=piercing>", "<:ato_piercing:831707237499797534>");
            newDesc = newDesc.Replace("<sprite name=powerful>", "<:ato_powerful:1017804148260995102>");
            newDesc = newDesc.Replace("<sprite name=poison>", "<:ato_poison:1017804131588657243>");
            newDesc = newDesc.Replace("<sprite name=cardrandom>", "<:ato_random:1095706686016200714>");
            newDesc = newDesc.Replace("<sprite name=regeneration>", "<:ato_regeneration:836680199109476392>");
            newDesc = newDesc.Replace("<sprite name=reinforce>", "<:ato_reinforce:1017804164107079732>");
            newDesc = newDesc.Replace("<sprite name=sanctify>", "<:ato_sanctify:831707981733298186>");
            newDesc = newDesc.Replace("<sprite name=shackle>", "<:ato_shackle:1017804176492871715>");
            newDesc = newDesc.Replace("<sprite name=shadow>", "<:ato_shadow:831708702981619783>");
            newDesc = newDesc.Replace("<sprite name=sharp>", "<:ato_sharp:1017804192024383608>");
            newDesc = newDesc.Replace("<sprite name=shield>", "<:ato_shield:831707596405997578>");
            newDesc = newDesc.Replace("<sprite name=sight>", "<:ato_sight:831708916340490240>");
            newDesc = newDesc.Replace("<sprite name=silence>", "<:ato_silence:1017804207732035665>");
            newDesc = newDesc.Replace("<sprite name=slashing>", "<:ato_slash:831707157426602034>");
            newDesc = newDesc.Replace("<sprite name=slow>", "<:ato_slow:831710190662844416>");
            newDesc = newDesc.Replace("<sprite name=spark>", "<:ato_spark:831708425775480892>");
            newDesc = newDesc.Replace("<sprite name=stanzai>", "<:ato_stanza1:831710428156395563>");
            newDesc = newDesc.Replace("<sprite name=stanzaii>", "<:ato_stanza2:831710440731574283>");
            newDesc = newDesc.Replace("<sprite name=stanzaiii>", "<:ato_stanza3:831710474202251265>");
            newDesc = newDesc.Replace("<sprite name=stealth>", "<:ato_stealth:831709537592803358>");
            newDesc = newDesc.Replace("<sprite name=stealthbonus>", "<:ato_stealth:831709537592803358>");
            newDesc = newDesc.Replace("<sprite name=stress>", "<:ato_stress:1017804219270561792>");
            newDesc = newDesc.Replace("<sprite name=thorns>", "<:ato_thorns:831709250656927805>");
            newDesc = newDesc.Replace("<sprite name=taunt>", "<:ato_taunt:831711779494821898>");
            newDesc = newDesc.Replace("<sprite name=vitality>", "<:ato_vitality:836679899904868384>");
            newDesc = newDesc.Replace("<sprite name=vulnerable>", "<:ato_vulnerable:834829138488983563>");
            newDesc = newDesc.Replace("<sprite name=weak>", "<:ato_weak:1017804231773790268>");
            newDesc = newDesc.Replace("<sprite name=wet>", "<:ato_wet:1017804243526221884>");
            newDesc = newDesc.Replace("<sprite name=scourge>", "<:ato_scourge:1142114629431087174>");
            newDesc = newDesc.Replace("<sprite name=zeal>", "<:ato_zeal:1142114574544404552>");
            // fix whitespace
            newDesc = newDesc.Replace("  ", " ");
            newDesc = newDesc.Replace("  ", " ");
            newDesc = newDesc.Replace("\n ", "\n");
            newDesc = newDesc.Replace(" \n", "\n");
            newDesc = newDesc.Replace(" <:ato_", "<:ato_");
            return newDesc.Trim();
        }

        public static void MapNodeExport() // exports map node positions into text format
        {
            Node[] foundNodes = Resources.FindObjectsOfTypeAll<Node>();
            string s = "name\tzone\tlocalx\tlocaly\tlocalz\tposx\tposy\tposz";
            foreach (Node n in foundNodes)
                s += "\n" + n.name + "\t" + n.nodeData.NodeZone.ZoneId + "\t" + n.transform.localPosition.x.ToString() + "\t" + n.transform.localPosition.y.ToString() + "\t" + n.transform.localPosition.z.ToString() + "\t" + n.transform.position.x.ToString() + "\t" + n.transform.position.y.ToString() + "\t" + n.transform.position.z.ToString();
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "nodePos.txt"), s);
        }

        public static void RoadExport(bool forExcel = false) // exports roads into text format
        {
            if (forExcel)
            {
                string s = "name\tpos1\tpos2\tpos3\tpos4\tpos5\tpos6\tpos7\tpos8\tpos9\tpos10\tpos11";
                for (int a = 0; a < MapManager.Instance.mapList.Count; a++)
                {
                    foreach (Transform transform1 in MapManager.Instance.mapList[a].transform)
                    {
                        if (transform1.gameObject.name == "Roads")
                        {
                            for (int b = 0; b < transform1.childCount; b++)
                            {
                                s += "\n" + transform1.GetChild(b).gameObject.name;
                                LineRenderer lr = transform1.GetChild(b).gameObject.GetComponent<LineRenderer>();
                                Vector3[] v3s = new Vector3[lr.positionCount];
                                lr.GetPositions(v3s);
                                foreach (Vector3 v3 in v3s)
                                {
                                    float mX = v3.x + transform1.position.x;
                                    float mY = v3.y + transform1.position.y;
                                    s += ",(" + mX + "," + mY + ")";
                                }
                            }
                        }
                    }
                }
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "linePosForExcel.txt"), s);
            }
            else
            {
                // actual roadsTXT
                string s = @"\\vanilla roadsTXT. Please ONLY use the roads you need for custom paths, because otherwise load times will be significantly increased and interactions between mods may cause errors and strange behaviour!";
                s += "\n" + @"\\node_from-node_to|(x1,y1),(x2,y2),(x3,y3),(x4,y4),... [etc]";
                for (int a = 0; a < MapManager.Instance.mapList.Count; a++)
                {
                    foreach (Transform transform1 in MapManager.Instance.mapList[a].transform)
                    {
                        if (transform1.gameObject.name == "Roads")
                        {
                            for (int b = 0; b < transform1.childCount; b++)
                            {
                                s += "\n" + transform1.GetChild(b).gameObject.name + "|";
                                LineRenderer lr = transform1.GetChild(b).gameObject.GetComponent<LineRenderer>();
                                Vector3[] v3s = new Vector3[lr.positionCount];
                                lr.GetPositions(v3s);
                                foreach (Vector3 v3 in v3s)
                                {
                                    float mX = v3.x + transform1.position.x;
                                    float mY = v3.y + transform1.position.y;
                                    s += ",(" + mX + "," + mY + ")";
                                }
                            }
                        }
                    }
                }
                s = s.Replace("|,", "|");
                FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "roadsTXT"));
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "roadsTXT", "vanilla.txt"), s);
            }
        }


        public static void medsCreateCardClones()
        {
            Traverse.Create(Globals.Instance).Field("_CardsListSearch").SetValue(new Dictionary<string, List<string>>());
            Dictionary<string, CardData> medsCardsSource = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            Dictionary<CardType, List<string>> medsCardListByType = new();
            Dictionary<CardClass, List<string>> medsCardListByClass = new();
            List<string> medsCardListNotUpgraded = new();
            Dictionary<CardClass, List<string>> medsCardListNotUpgradedByClass = new();
            Dictionary<string, List<string>> medsCardListByClassType = new();
            Dictionary<string, int> medsCardEnergyCost = new();
            Dictionary<CardType, List<string>> medsCardItemByType = new();
            List<string> medsSortNameID = new();
            foreach (CardType key in Enum.GetValues(typeof(Enums.CardType)))
            {
                if (key != Enums.CardType.None)
                    medsCardListByType[key] = new List<string>();
            }
            foreach (CardClass key in Enum.GetValues(typeof(Enums.CardClass)))
            {
                medsCardListByClass[key] = new List<string>();
                medsCardListNotUpgradedByClass[key] = new List<string>();
            }
            Dictionary<string, CardData> medsCards = new();
            foreach (string key in medsCardsSource.Keys)
                medsCards.Add(key, medsCardsSource[key]);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string key1 in medsCardsSource.Keys)
            {
                stringBuilder.Clear();
                medsCards[key1].InitClone(key1);
                CardData card = medsCards[key1];
                string text1;
                if (card.UpgradedFrom != "")
                {
                    stringBuilder.Append("c_");
                    stringBuilder.Append(card.UpgradedFrom);
                    stringBuilder.Append("_name");
                    text1 = Texts.Instance.GetText(stringBuilder.ToString(), "cards");
                }
                else
                {
                    stringBuilder.Append("c_");
                    stringBuilder.Append(card.Id);
                    stringBuilder.Append("_name");
                    text1 = Texts.Instance.GetText(stringBuilder.ToString(), "cards");
                }
                if (text1 != "")
                    card.CardName = text1;
                stringBuilder.Clear();
                stringBuilder.Append("c_");
                stringBuilder.Append(card.Id);
                stringBuilder.Append("_fluff");
                string text2 = Texts.Instance.GetText(stringBuilder.ToString(), "cards");
                if (text2 != "")
                    card.Fluff = text2;
                medsSortNameID.Add(card.CardName + "|" + key1);
            }
            // sort by name _then_ ID
            medsSortNameID.Sort();
            Dictionary<string, CardData> medsCardsSorted = new();
            Dictionary<string, CardData> medsCardsSourceSorted = new();
            //LogDebug("READY TO SORT CARDS! " + medsSortNameID.Count);
            foreach (string key in medsSortNameID)
            {
                string cID = key.Split("|")[1];
                //LogDebug("SORTING CARD: " + key);
                medsCardsSorted[cID] = medsCards[cID];
                medsCardsSourceSorted[cID] = medsCardsSource[cID];
            }
            //LogDebug("FINISHED SORTING CARDS!");
            medsCardsSource = medsCardsSourceSorted;
            medsCards = medsCardsSorted;

            foreach (string key1 in medsCardsSource.Keys)
            {
                CardData card = medsCards[key1];
                if ((card.CardClass != Enums.CardClass.Item || !card.Item.QuestItem) && card.ShowInTome)
                {
                    medsCardEnergyCost.Add(card.Id, card.EnergyCost);
                    Globals.Instance.IncludeInSearch(card.CardName, card.Id);
                    medsCardListByClass[card.CardClass].Add(card.Id);
                    if (card.CardUpgraded == Enums.CardUpgraded.No)
                    {
                        medsCardListNotUpgradedByClass[card.CardClass].Add(card.Id);
                        medsCardListNotUpgraded.Add(card.Id);
                        if (card.CardClass == Enums.CardClass.Item)
                        {
                            if (!medsCardItemByType.ContainsKey(card.CardType))
                                medsCardItemByType.Add(card.CardType, new List<string>());
                            if (!medsCardItemByType[card.CardType].Contains(card.Id))
                                medsCardItemByType[card.CardType].Add(card.Id);
                        }
                    }
                    List<Enums.CardType> cardTypes = card.GetCardTypes();
                    for (int index = 0; index < cardTypes.Count; ++index)
                    {
                        medsCardListByType[cardTypes[index]].Add(card.Id);
                        string key2 = Enum.GetName(typeof(Enums.CardClass), (object)card.CardClass) + "_" + Enum.GetName(typeof(Enums.CardType), (object)cardTypes[index]);
                        if (!medsCardListByClassType.ContainsKey(key2))
                            medsCardListByClassType[key2] = new List<string>();
                        if (!medsCardListByClassType[key2].Contains(card.Id))
                            medsCardListByClassType[key2].Add(card.Id);
                        Globals.Instance.IncludeInSearch(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)cardTypes[index])), card.Id);
                    }
                }
            }
            Traverse.Create(Globals.Instance).Field("_CardListByType").SetValue(medsCardListByType);
            Traverse.Create(Globals.Instance).Field("_CardListByClass").SetValue(medsCardListByClass);
            Traverse.Create(Globals.Instance).Field("_CardListNotUpgraded").SetValue(medsCardListNotUpgraded);
            Traverse.Create(Globals.Instance).Field("_CardListNotUpgradedByClass").SetValue(medsCardListNotUpgradedByClass);
            Traverse.Create(Globals.Instance).Field("_CardListByClassType").SetValue(medsCardListByClassType);
            Traverse.Create(Globals.Instance).Field("_CardEnergyCost").SetValue(medsCardEnergyCost);
            Traverse.Create(Globals.Instance).Field("_CardItemByType").SetValue(medsCardItemByType);
            Traverse.Create(Globals.Instance).Field("_CardEnergyCost").SetValue(medsCardEnergyCost);
            Traverse.Create(Globals.Instance).Field("_Cards").SetValue(medsCards);
            Traverse.Create(Globals.Instance).Field("_CardsSource").SetValue(medsCardsSource);
            foreach (string key in Globals.Instance.Cards.Keys)
                Globals.Instance.Cards[key].InitClone2();
            //medsCardListNotUpgraded.Sort(); // no longer necessary because we sort cards and cardssource instead?
        }

        public static void DropOnlyItemNodes()
        {
            Dictionary<string, NodeData> medsNodeDataSource = Traverse.Create(Globals.Instance).Field("_NodeDataSource").GetValue<Dictionary<string, NodeData>>();
            string sTotal = "";
            List<string> allItems = new();
            foreach (string nodeID in medsNodeDataSource.Keys)
            {
                List<string> nodeItems = new();
                foreach (CombatData _combat in medsNodeDataSource[nodeID].NodeCombat)
                    foreach (string sItem in DropOnlyItemCombat(_combat))
                        if (!nodeItems.Contains(sItem))
                            nodeItems.Add(sItem);
                foreach (EventData _event in medsNodeDataSource[nodeID].NodeEvent)
                    foreach (string sItem in DropOnlyItemEvent(_event))
                        if (!nodeItems.Contains(sItem))
                            nodeItems.Add(sItem);
                if (nodeItems.Count() > 0)
                {
                    nodeItems.Sort();
                    sTotal += medsNodeDataSource[nodeID].NodeZone.ZoneId + "\t" + medsNodeDataSource[nodeID].NodeName + "\t" + nodeID + "\t" + String.Join(", ", nodeItems.ToArray()) + "\n";
                }
                foreach (string sItem in nodeItems)
                    if (!allItems.Contains(sItem))
                        allItems.Add(sItem);
            }
            allItems.Sort();
            sTotal += String.Join(", ", allItems.ToArray());
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "DropOnly.txt"), sTotal);
        }
        public static List<string> DropOnlyItemCombat(CombatData _combat)
        {
            if (_combat == null)
                return new List<string>();
            LogDebug("checking combat: " + _combat.CombatId);
            List<string> combatItems = DropOnlyItemEvent(_combat.EventData);
            return combatItems;
        }
        public static List<string> DropOnlyItemEvent(EventData _event)
        {
            if (_event == null)
                return new List<string>();
            LogDebug("checking event: " + _event.EventId);
            int a = 0;
            List<string> eventItems = new();
            foreach (EventReplyData _eventReply in _event.Replys)
            {
                a++;
                LogDebug("checking eventreply: " + _event.EventId + " " + a.ToString());
                foreach (string sItem in DropOnlyItemCombat(_eventReply.SsCombat))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemCombat(_eventReply.SscCombat))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemCombat(_eventReply.FlCombat))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemCombat(_eventReply.FlcCombat))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemLoot(_eventReply.SsLootList))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemLoot(_eventReply.SscLootList))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemLoot(_eventReply.FlLootList))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                foreach (string sItem in DropOnlyItemLoot(_eventReply.FlcLootList))
                    if (!eventItems.Contains(sItem))
                        eventItems.Add(sItem);
                if (_eventReply.SsAddItem != null && _eventReply.SsAddItem.Item != null && !eventItems.Contains(_eventReply.SsAddItem.CardName))
                    eventItems.Add(_eventReply.SsAddItem.CardName);
                if (_eventReply.SscAddItem != null && _eventReply.SscAddItem.Item != null && !eventItems.Contains(_eventReply.SscAddItem.CardName))
                    eventItems.Add(_eventReply.SscAddItem.CardName);
                if (_eventReply.FlAddItem != null && _eventReply.FlAddItem.Item != null && !eventItems.Contains(_eventReply.FlAddItem.CardName))
                    eventItems.Add(_eventReply.FlAddItem.CardName);
                if (_eventReply.FlcAddItem != null && _eventReply.FlcAddItem.Item != null && !eventItems.Contains(_eventReply.FlcAddItem.CardName))
                    eventItems.Add(_eventReply.FlcAddItem.CardName);
            }
            return eventItems;
        }
        public static List<string> DropOnlyItemLoot(LootData _loot)
        {
            if (_loot == null)
                return new List<string>();
            LogDebug("checking loot: " + _loot.Id);
            List<string> lootItems = new();
            foreach (LootItem _lootItem in _loot.LootItemTable)
                if (_lootItem != null && _lootItem.LootCard != null && _lootItem.LootCard.Item != null && _lootItem.LootPercent == 100f && !lootItems.Contains(_lootItem.LootCard.CardName))
                    lootItems.Add(_lootItem.LootCard.CardName);
            return lootItems;
        }
    }
}
