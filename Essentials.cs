using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using System.IO;
using UnityEngine;
using TMPro;
using static Enums;
using Steamworks.Data;
using Steamworks;
using System.Threading.Tasks;
using System;
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
        internal const int ModDate = 20240516;
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;

        public static ConfigEntry<bool> medsExportJSON { get; private set; }
        public static ConfigEntry<bool> medsExportSprites { get; private set; }
        public static ConfigEntry<bool> medsShowAtStart { get; private set; }
        public static ConfigEntry<bool> medsConsistency { get; private set; }
        public static ConfigEntry<bool> medsSkipLogos { get; private set; }
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
        public static readonly string[] vanillaSubclasses = { "mercenary", "sentinel", "berserker", "warden", "ranger", "assassin", "archer", "minstrel", "elementalist", "pyromancer", "loremaster", "warlock", "cleric", "priest", "voodoowitch", "prophet", "bandit", "fallen", "paladin", "queen" };
        public static Dictionary<string, string> medsTexts = new();
        private static List<string> medsExportedSpritePaths = new();
        internal static int medsForceWeekly = 0;
        public static Dictionary<int, Mod> medsMods = new();
        internal static Dictionary<string, List<string>> medsModAuthors = new();
        public static Dictionary<string, string> medsLoadedDependencies = new();
        public static string medsCloneTwo = "";
        public static string medsCloneThree = "";
        public static string medsCloneFour = "";
        public static Dictionary<string, List<string>> medsBaseCardsListSearch = new();
        public static List<string> medsF2UIOpen = new();
        public static List<string> medsCheckSummary = new();
        private void Awake()
        {
            Log = Logger;
            LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");
            medsExportJSON = Config.Bind(new ConfigDefinition("Debug", "Export Vanilla Content"), false, new ConfigDescription("Export AtO class data to JSON files that are compatible with Obeliskial Content."));
            medsExportSprites = Config.Bind(new ConfigDefinition("Debug", "Export Sprites"), true, new ConfigDescription("Export sprites when exporting JSON files."));
            medsShowAtStart = Config.Bind(new ConfigDefinition("Debug", "Show At Start"), true, new ConfigDescription("Show the mod version window when the game loads."));
            medsConsistency = Config.Bind(new ConfigDefinition("Should Be Vanilla", "Disable Paradox Integration"), true, new ConfigDescription("Disable Paradox integration and telemetry (does not include launcher)."));
            medsSkipLogos = Config.Bind(new ConfigDefinition("Should Be Vanilla", "Skip Logos"), true, new ConfigDescription("Skip logos on startup."));
            UniverseLib.Universe.Init(1f, ObeliskialUI.InitUI, LogHandler, new()
            {
                Disable_EventSystem_Override = false, // or null
                Force_Unlock_Mouse = true, // or null
                Unhollowed_Modules_Folder = null
            });
            UniverseLib.Universe.Init(1f, DevTools.Init, LogHandler, new()
            {
                Disable_EventSystem_Override = false, // or null
                Force_Unlock_Mouse = true, // or null
                Unhollowed_Modules_Folder = null
            });
            UniverseLib.Universe.Init(1f, ProfileEditor.Init, LogHandler, new()
            {
                Disable_EventSystem_Override = false, // or null
                Force_Unlock_Mouse = true, // or null
                Unhollowed_Modules_Folder = null
            });
            RegisterMod(_name: PluginInfo.PLUGIN_NAME, _author: "stiffmeds", _description: "Essential reference classes and methods for Across the Obelisk modding.", _version: PluginInfo.PLUGIN_VERSION, _date: ModDate, _link: @"https://across-the-obelisk.thunderstore.io/package/meds/Obeliskial_Essentials/", _priority: int.MaxValue, _type: new string[1] { "Core" });
            //AddModVersionText(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, ModDate.ToString());
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
        public static void RegisterMod(string _name, string _author = "", string _description = "", string _version = "1.0.0", int _date = 19920101, string _link = "", string[] _dependencies = null, string _contentFolder = "", int _priority = 100, string[] _type = null, string _comment = "", bool _enabled = true)
        {
            Mod _mod = new(_name, _author, _description, _version, _date, _link, _dependencies, _contentFolder, _priority, _type, _comment, _enabled);
            RegisterMod(_mod);
        }
        public static void RegisterMod(Mod _mod)
        {
            if (medsModAuthors.ContainsKey(_mod.Author.ToLower()))
            {
                if (medsModAuthors[_mod.Author.ToLower()].Contains(_mod.Name.ToLower()))
                {
                    LogError("Attempting to register duplicate mod: " + _mod.Author + " - " + _mod.Name);
                }
                else
                {
                    medsModAuthors[_mod.Author.ToLower()].Add(_mod.Name.ToLower());
                    medsMods[medsMods.Count] = _mod;
                }
            }
            else
            {
                medsModAuthors[_mod.Author.ToLower()] = new() { _mod.Name.ToLower() };
                medsMods[medsMods.Count] = _mod;
            }
            if (_mod.Enabled)
            {
                if (_mod.ContentFolder.IsNullOrWhiteSpace()) // no custom content to load
                    medsLoadedDependencies[_mod.Author.ToLower().Replace(" ", "_") + "-" + _mod.Name.ToLower().Replace(" ", "_")] = _mod.Version.ToLower();
                string newText = _mod.Name + " v" + _mod.Version + (_mod.Date == 19920101 ? "" : " (" + _mod.Date.ToString() + ")");
                if (medsVersionText.IsNullOrWhiteSpace())
                    medsVersionText = newText;
                else if (!medsVersionText.Contains(newText))
                    medsVersionText += "\n" + newText;
            }
        }

        public static string TextChargesLeft(int currentCharges, int chargesTotal)
        {
            int cCharges = currentCharges;
            int cTotal = chargesTotal;
            return "<br><color=#FFF>" + cCharges.ToString() + "/" + cTotal.ToString() + "</color>";
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
                    File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + ".json"), "[{\n");
                    if (textFULL != "")
                    {
                        FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", type + "_FULL"));
                        File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + "_FULL.json"), "[{\n");
                    }
                }
                WriteToJSON(type, text, id);
                if (textFULL != "")
                    WriteToJSON(type + "_FULL", textFULL, id);
                if (a == data.Length)
                {
                    // WriteToJSON(type, combined.Remove(combined.Length - 1) + "}", a, h);
                    File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + ".json"), "\"" + id + "\": " + text + "}]");
                    if (textFULL != "")
                        File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + "_FULL.json"), "\"" + id + "\": " + textFULL + "]");
                    Log.LogInfo("exported " + a + " " + type + " values!");
                }
                else
                {
                    File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + ".json"), "\"" + id + "\": " + text + ",");
                    if (textFULL != "")
                        File.AppendAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined", type + "_FULL.json"), "\"" + id + "\": " + textFULL + ",");
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
                    theList += "\n" + (SteamManager.Instance.shameList.Contains(scoreboardGlobal[a].User.Id.ToString()) ? "SHAME:" : "") + scoreboardGlobal[a].User.Id.ToString() + "\t" + scoreboardGlobal[a].Score + "\t" + string.Join("\t", scoreboardGlobal[a].Details);
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "scoreboardGlobal.json"), theList);
                theList = "";
                for (int a = 0; a < scoreboardSingle.Length; a++)
                    theList += "\n" + scoreboardSingle[a].User.Id.ToString() + "\t" + scoreboardSingle[a].Score + "\t" + string.Join("\t", scoreboardSingle[a].Details);
                File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "scoreboardSingle.json"), theList);
            }
        }

        public static System.Collections.IEnumerator FullCardSpriteOutputCo(bool toToKFolder = false, UniverseLib.UI.Models.ButtonRef _btn = null)
        {
            string origText = "";
            if (_btn != null)
            {
                _btn.ButtonText.alignment = TextAnchor.MiddleLeft;
                origText = _btn.ButtonText.text;
            }
            //SceneStatic.LoadByName("TomeofKnowledge");
            //yield return Globals.Instance.WaitForSeconds(5f);
            SnapshotCamera snapshotCamera = SnapshotCamera.MakeSnapshotCamera(0);
            CardScreenManager.Instance.ShowCardScreen(true);
            // for each card in cards
            Dictionary<string, CardData> allCards = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            LogInfo("i herd u liek memory leaks ;)");
            int a = 1;
            foreach (KeyValuePair<string, CardData> kvp in allCards)
            {
                if (a == 100)
                    Globals.Instance.StartCoroutine(DevTools.medsButtonTextRevert(DevTools.btnProfileEditor, "i herd u liek memory leaks ;)"));
                LogInfo("EXTRACTING CARD IMAGE:" + kvp.Key);
                if (_btn != null)
                    _btn.ButtonText.text = "Creating image " + a.ToString() + @"/" + allCards.Count() + ": " + kvp.Key;
                CardScreenManager.Instance.SetCardData(kvp.Value);
                GameObject cardGO = Traverse.Create(CardScreenManager.Instance).Field("cardGO").GetValue<GameObject>();
                if ((UnityEngine.Object)cardGO != (UnityEngine.Object)null)
                {
                    cardGO.transform.Find("BorderCard").gameObject.SetActive(false);
                    cardGO.transform.Find("Lock").gameObject.SetActive(false);
                    Texture2D snapshot = snapshotCamera.TakeObjectSnapshot(cardGO, UnityEngine.Color.clear, new Vector3(0, 0.008f, 1), Quaternion.Euler(new Vector3(0f, 0f, 0f)), new Vector3(0.78f, 0.78f, 0.78f), 297, 450);
                    string path = toToKFolder ? Path.Combine(Paths.GameRootPath, "Tome of Knowledge", "card_images") : Path.Combine(Paths.GameRootPath, "Card Images", DataTextConvert.ToString(kvp.Value.CardClass));
                    SnapshotCamera.SavePNG(snapshot, kvp.Key, Directory.CreateDirectory(path).FullName);
                    /*if (kvp.Value.CardType == CardType.Corruption)
                        SnapshotCamera.SavePNG(snapshot, kvp.Key, Directory.CreateDirectory(Path.Combine(Application.dataPath, "../Card Images", DataTextConvert.ToString(kvp.Value.CardType))).FullName);*/
                    UnityEngine.Object.Destroy(snapshot);
                    UnityEngine.Object.Destroy(cardGO);
                }
                a++;
                yield return Globals.Instance.WaitForSeconds(0.01f);
            }
            if (_btn != null)
            {
                _btn.ButtonText.alignment = TextAnchor.MiddleCenter;
                _btn.ButtonText.text = origText;
                Globals.Instance.StartCoroutine(DevTools.medsButtonTextRevert(_btn));
            }
            Application.OpenURL(@"file://" + Path.Combine(Paths.GameRootPath, toToKFolder ? "Tome of Knowledge" : "Card Images"));
            yield return null;
        }
        public static void TomeOfKnowledgeExport(bool exportImages = true, UniverseLib.UI.Models.ButtonRef _btn = null)
        {
            List<string> doneList = new();
            foreach(string id in Globals.Instance.Cards.Keys)
            {
                CardData card = Globals.Instance.GetCardData(id, false);
                if (card != null)
                {
                    CardData relatedCard = Globals.Instance.GetCardData(card.RelatedCard, false);
                    if (relatedCard != null && (card.CardClass == CardClass.Monster || (card.CardClass == CardClass.Item && relatedCard.CardClass == CardClass.Special) || (card.CardClass == CardClass.Special && relatedCard.CardClass == CardClass.Special && relatedCard.CardType != CardType.Enchantment)) && !doneList.Contains(relatedCard.Id))
                        doneList.Add(relatedCard.Id);
                    CardData relatedCard2 = Globals.Instance.GetCardData(card.RelatedCard2, false);
                    if (relatedCard2 != null && (card.CardClass == CardClass.Monster || (card.CardClass == CardClass.Item && relatedCard2.CardClass == CardClass.Special) || (card.CardClass == CardClass.Special && relatedCard2.CardClass == CardClass.Special && relatedCard2.CardType != CardType.Enchantment)) && !doneList.Contains(relatedCard2.Id))
                        doneList.Add(relatedCard2.Id);
                    CardData relatedCard3 = Globals.Instance.GetCardData(card.RelatedCard3, false);
                    if (relatedCard3 != null && (card.CardClass == CardClass.Monster || (card.CardClass == CardClass.Item && relatedCard3.CardClass == CardClass.Special) || (card.CardClass == CardClass.Special && relatedCard3.CardClass == CardClass.Special && relatedCard3.CardType != CardType.Enchantment)) && !doneList.Contains(relatedCard3.Id))
                        doneList.Add(relatedCard3.Id);
                }
            }
            Dictionary<string, List<string>> cardIDs = new();
            Dictionary<string, List<string>> relatedIDs = new();
            // force order
            cardIDs["Armageddon (Corruptor)"] = new();
            cardIDs["Barrier"] = new();
            cardIDs["Bonk Hammer (Item)"] = new();
            cardIDs["Camouflage"] = new();
            cardIDs["Charge Battery"] = new();
            cardIDs["Death's Toll (Monster)"] = new();
            cardIDs["Fire Blast"] = new();
            cardIDs["Flash"] = new();
            cardIDs["Foresight"] = new();
            cardIDs["Frost Bolt"] = new();
            cardIDs["Harley (Item)"] = new();
            cardIDs["Heal"] = new();
            cardIDs["Heal"] = new();
            cardIDs["Holy Smite"] = new();
            cardIDs["Intercept"] = new();
            cardIDs["Mana Gem"] = new();
            cardIDs["Poison Dart"] = new();
            cardIDs["Quick Shot"] = new();
            cardIDs["Rampage"] = new();
            cardIDs["Sharpen"] = new();
            cardIDs["Skillful"] = new();
            cardIDs["Sneak Peek"] = new();
            cardIDs["The Yogrinch (Corruptor)"] = new();
            cardIDs["Thunderclap"] = new();
            cardIDs["Toxic Blob (Item)"] = new();
            cardIDs["Zap"] = new();
            foreach (string id in Globals.Instance.Cards.Keys)
            {
                if (doneList.Contains(id))
                    continue;
                CardData card = Globals.Instance.GetCardData(id, false);
                if (card != null)
                {
                    string cardname = card.CardName;
                    if (card.CardType == CardType.Corruption)
                        cardname += " (Corruptor)";
                    else if (card.CardClass == CardClass.Monster)
                        cardname += " (Monster)";
                    else if (card.CardClass == CardClass.Special && !card.Starter && card.CardType != CardType.Enchantment)
                        cardname += " (Special)";
                    else if (card.CardClass == CardClass.Item)
                        cardname += " (Item)";
                    if (!cardIDs.ContainsKey(cardname))
                        cardIDs[cardname] = new();
                    List<string> newRelated = new();
                    cardIDs[cardname].Add(card.Id);
                    foreach (string relatedID in GetRelatedCardIDs(card))
                        newRelated.Add(relatedID);
                    // blue upgrade
                    CardData upgrade1 = Globals.Instance.GetCardData(card.UpgradesTo1, false);
                    if (upgrade1 != null && !cardIDs[cardname].Contains(upgrade1.Id))
                    {
                        cardIDs[cardname].Add(upgrade1.Id);
                        doneList.Add(upgrade1.Id);
                        foreach (string relatedID in GetRelatedCardIDs(upgrade1))
                            newRelated.Add(relatedID);
                    }
                    // yellow upgrade
                    CardData upgrade2 = Globals.Instance.GetCardData(card.UpgradesTo2, false);
                    if (upgrade2 != null && !cardIDs[cardname].Contains(upgrade2.Id))
                    {
                        cardIDs[cardname].Add(upgrade2.Id);
                        doneList.Add(upgrade2.Id);
                        foreach (string relatedID in GetRelatedCardIDs(upgrade2))
                            newRelated.Add(relatedID);
                    }
                    // purple upgrade
                    if (card.UpgradesToRare != null)
                    {
                        cardIDs[cardname].Add(card.UpgradesToRare.Id);
                        doneList.Add(card.UpgradesToRare.Id);
                        foreach (string relatedID in GetRelatedCardIDs(card.UpgradesToRare))
                            newRelated.Add(relatedID);
                    }

                    cardIDs[cardname] = cardIDs[cardname].Distinct().ToList();
                    if (newRelated.Count > 0)
                    {
                        if (!relatedIDs.ContainsKey(cardname))
                            relatedIDs[cardname] = newRelated;
                        else
                            relatedIDs[cardname].AddRange(newRelated);
                        relatedIDs[cardname] = relatedIDs[cardname].Distinct().ToList();
                    }

                    doneList.Add(id);
                }
            }
            string combined = "{\n  \"cards\": {";
            string combinedNames = "\n  \"choices\": [";
            foreach (string cardName in cardIDs.Keys)
            {
                combined += "\n    \"" + cardName.ToLower() + "\": {\n      \"IDs\": [\"" + String.Join("\",\"", cardIDs[cardName]) + "\"]";
                if (relatedIDs.ContainsKey(cardName))
                    combined += ",\n      \"RelatedIDs\": [\"" + String.Join("\",\"", relatedIDs[cardName]) + "\"]";
                combined += "\n    },";
                combinedNames += "\n    \"" + cardName + "\",";
            }
            combined = combined.Remove(combined.Length - 1) + "\n  }," + combinedNames.Remove(combinedNames.Length - 1) + "\n  ]\n}";
            //combinedNames = combinedNames.Remove(combinedNames.Length - 1) + "\n]";
            FolderCreate(Path.Combine(Paths.GameRootPath, "Tome of Knowledge"));
            File.WriteAllText(Path.Combine(Paths.GameRootPath, "Tome of Knowledge", "cards.json"), combined);
            if (_btn != null)
                _btn.ButtonText.text = "Tome of Knowledge bot data export";
            if (exportImages)
                Globals.Instance.StartCoroutine(FullCardSpriteOutputCo(true, _btn));
            //File.WriteAllText(Path.Combine(Paths.GameRootPath, "Tome of Knowledge", "autocomplete.txt"), combinedNames);
        }
        public static List<string> GetRelatedCardIDs(CardData _card)
        {
            List<string> IDs = new();
            if (_card != null)
            {
                CardData relatedCard = Globals.Instance.GetCardData(_card.RelatedCard, false);
                if (relatedCard != null && !IDs.Contains(relatedCard.Id))
                    IDs.Add(relatedCard.Id);
                CardData relatedCard2 = Globals.Instance.GetCardData(_card.RelatedCard2, false);
                if (relatedCard2 != null && !IDs.Contains(relatedCard2.Id))
                    IDs.Add(relatedCard2.Id);
                CardData relatedCard3 = Globals.Instance.GetCardData(_card.RelatedCard3, false);
                if (relatedCard3 != null && !IDs.Contains(relatedCard3.Id))
                    IDs.Add(relatedCard3.Id);
            }
            return IDs;
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

        public static void medsIncludeInBaseSearch(string _term, string id, bool includeFullTerm = true)
        {
            if (SceneStatic.GetSceneName() != "Game")
                return;
            _term = _term.ToLower();
            string[] strArray = _term.Split(' ', StringSplitOptions.None);
            id = id.ToLower();
            foreach (string key in strArray)
            {
                if (key.Trim() != "")
                {
                    if (!medsBaseCardsListSearch.ContainsKey(key))
                        medsBaseCardsListSearch.Add(key, new List<string>());
                    if (!medsBaseCardsListSearch[key].Contains(id))
                        medsBaseCardsListSearch[key].Add(id);
                }
            }
            if (!includeFullTerm)
                return;
            if (!medsBaseCardsListSearch.ContainsKey(_term))
                medsBaseCardsListSearch.Add(_term, new List<string>());
            if (medsBaseCardsListSearch[_term].Contains(id))
                return;
            medsBaseCardsListSearch[_term].Add(id);
        }

        public static void medsCreateCardClones()
        {
            Traverse.Create(Globals.Instance).Field("_CardsListSearch").SetValue(medsBaseCardsListSearch);
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

        internal static void medsGetWeeklyInfo()
        {
            string s = "Week\tName\tStart\tEnd\tHero 1\tHero 2\tHero 3\tHero 4\tTrait1Name\tTrait1Desc\tTrait2Name\tTrait2Desc\tTrait3Name\tTrait3Desc\tTrait4Name\tTrait4Desc\tTrait5Name\tTrait5Desc\tTrait6Name\tTrait6Desc\tBoss 1\tBoss 2\tBoss 3\tSeed";
            foreach (ChallengeData _wkly in Globals.Instance.WeeklyDataSource.Values)
            {
                s += "\n" + _wkly.Week.ToString() + "\t" + (_wkly.IdSteam.IsNullOrWhiteSpace() || Texts.Instance.GetText(_wkly.IdSteam).IsNullOrWhiteSpace() ? "Week " + _wkly.Week.ToString() : Texts.Instance.GetText(_wkly.IdSteam)) + "\t";
                DateTime _dte = new DateTime(2023, 11, 9, 14, 0, 0);
                s += _dte.AddDays(7 * _wkly.Week).ToString("d MMMM yyyy @ h tt") + "\t" + _dte.AddDays(7 * _wkly.Week + 7).ToString("d MMMM yyyy @ h tt") + "\t";
                s += _wkly.Hero1.CharacterName + "\t" + _wkly.Hero2.CharacterName + "\t" + _wkly.Hero3.CharacterName + "\t" + _wkly.Hero4.CharacterName + "\t";
                foreach (ChallengeTrait _trt in _wkly.Traits)
                    s += _trt.Name + "\t" + Texts.Instance.GetText(_trt.Id + "Desc") + "\t";
                s += _wkly.Boss1.NPCName + "\t" + _wkly.Boss2.NPCName + "\t";
                int x = 0;
                foreach (NPCData _npc in _wkly.BossCombat.NPCList)
                {
                    if (_npc != null && _npc.IsBoss)
                    {
                        x++;
                        s += (x > 1 ? "&" : "") + _npc.NPCName;
                    }
                }
                s += "\t" + _wkly.Seed;
            }
            
            /*
            week 1 starts: 9  November 
            week 2 starts: 16 November
            week 3 starts: 23 November 2023 @ 2 pm (UTC/GMT)
            */
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "weeklies.txt"), s);
        }

        internal static void medsGetWeeklyCards()
        {
            Dictionary<string, string[]> cardsDrafted = Traverse.Create(ChallengeSelectionManager.Instance).Field("cardsDrafted").GetValue<Dictionary<string, string[]>>();
            Dictionary<string, string> cardsDraftedSpecial = Traverse.Create(ChallengeSelectionManager.Instance).Field("cardsDraftedSpecial").GetValue<Dictionary<string, string>>();
            Dictionary<string, string> cardsDraftedPackname = Traverse.Create(ChallengeSelectionManager.Instance).Field("cardsDraftedPackname").GetValue<Dictionary<string, string>>();
            Hero[] theTeam = Traverse.Create(ChallengeSelectionManager.Instance).Field("theTeam").GetValue<Hero[]>();
            CardScreenManager.Instance.ShowCardScreen(true);
            SnapshotCamera snapshotCamera = SnapshotCamera.MakeSnapshotCamera(0);
            for (int a = 0; a < 4; a++) // hero
            {
                Hero _hero = theTeam[a];
                if (_hero == null)
                    continue;

                for (int b = 0; b < 2; b++) // first pack / rerolled pack
                {
                    for (int d = 0; d < 3; d++) // card 1 / 2 / 3
                    {
                        for (int c = 0; c < 8; c++) // pack 1-8
                        {
                            string sTmp = a.ToString() + "_" + b.ToString() + "_" + c.ToString();
                            CardData crd = Globals.Instance.GetCardData(cardsDrafted[sTmp][d], false);
                            CardScreenManager.Instance.SetCardData(crd);
                            GameObject cardGO = Traverse.Create(CardScreenManager.Instance).Field("cardGO").GetValue<GameObject>();
                            if ((UnityEngine.Object)cardGO != (UnityEngine.Object)null)
                            {
                                cardGO.transform.Find("BorderCard").gameObject.SetActive(false);
                                Texture2D snapshot = snapshotCamera.TakeObjectSnapshot(cardGO, UnityEngine.Color.clear, new Vector3(0, 0.008f, 1), Quaternion.Euler(new Vector3(0f, 0f, 0f)), new Vector3(0.78f, 0.78f, 0.78f), 297, 450);
                                SnapshotCamera.SavePNG(snapshot, a.ToString() + "_" + _hero.SourceName + "_" + c.ToString() + "_" + cardsDraftedPackname[sTmp] + "_" + b.ToString() + "_" + d.ToString(), Directory.CreateDirectory(Path.Combine(Application.dataPath, "../Weeklies", medsForceWeekly.ToString())).FullName);
                                UnityEngine.Object.Destroy(snapshot);
                                UnityEngine.Object.Destroy(cardGO);
                            }
                            if (d == 0)
                            {
                                crd = Globals.Instance.GetCardData(cardsDraftedSpecial[sTmp + "_special"].Split("_")[0], false);
                                CardScreenManager.Instance.SetCardData(crd);
                                cardGO = Traverse.Create(CardScreenManager.Instance).Field("cardGO").GetValue<GameObject>();
                                if ((UnityEngine.Object)cardGO != (UnityEngine.Object)null)
                                {
                                    cardGO.transform.Find("BorderCard").gameObject.SetActive(false);
                                    Texture2D snapshot = snapshotCamera.TakeObjectSnapshot(cardGO, UnityEngine.Color.clear, new Vector3(0, 0.008f, 1), Quaternion.Euler(new Vector3(0f, 0f, 0f)), new Vector3(0.78f, 0.78f, 0.78f), 297, 450);
                                    SnapshotCamera.SavePNG(snapshot, a.ToString() + "_" + _hero.SourceName + "_" + c.ToString() + "_" + cardsDraftedPackname[sTmp] + "_" + b.ToString() + "_special", Directory.CreateDirectory(Path.Combine(Application.dataPath, "../Weeklies", medsForceWeekly.ToString())).FullName);
                                    UnityEngine.Object.Destroy(snapshot);
                                    UnityEngine.Object.Destroy(cardGO);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        internal static void medsListCorruptors()
        {
            List<string> easy = new();
            List<string> average = new();
            List<string> hard = new();
            List<string> extreme = new();
            List<CardData> cards = new();
            foreach (string cor in Globals.Instance.CardListByType[CardType.Corruption])
            {
                CardData card = Globals.Instance.GetCardData(cor);
                if (card != null)
                {
                    cards.Add(card);
                    if (card.CardRarity == CardRarity.Common)
                    {
                        easy.Add(cor);
                    }
                    else if (card.CardRarity == CardRarity.Uncommon)
                    {
                        easy.Add(cor);
                        average.Add(cor);
                    }
                    else if (card.CardRarity == CardRarity.Rare)
                    {
                        easy.Add(cor);
                        average.Add(cor);
                        hard.Add(cor);
                    }
                    else
                    {
                        easy.Add(cor);
                        average.Add(cor);
                        hard.Add(cor);
                        extreme.Add(cor);
                    }
                }
            }
            File.WriteAllText(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "corruptorIndex.txt"), "const corruptorIndex = Object.freeze({\n    easy: [\"" + String.Join("\", \"", easy) + "\"],\n    average: [\"" + String.Join("\", \"", average) + "\"],\n    hard: [\"" + String.Join("\", \"", hard) + "\"],\n    extreme: [\"" + String.Join("\", \"", extreme) + "\"]\n});");
            ExtractData(cards.ToArray());
        }
        internal static void ChangeUIState()
        {
            List<string> currentF2UIOpen = new();
            if (DevTools.ShowUI)
            {
                currentF2UIOpen.Add("DevTools");
                DevTools.ShowUI = false;
            }
            if (ProfileEditor.ShowUI)
            {
                currentF2UIOpen.Add("ProfileEditor");
                ProfileEditor.ShowUI = false;
            }
            if (currentF2UIOpen.Count > 0)
            { // store list of previously open windows
                medsF2UIOpen = currentF2UIOpen;
            }
            else
            { // open previously stored list
                foreach (string UIType in medsF2UIOpen)
                {
                    switch (UIType)
                    {
                        case "ProfileEditor":
                            ProfileEditor.ShowUI = true;
                            break;
                    }
                }
                // always open main panel
                DevTools.ShowUI = true;
            }
        }
        internal static void medsSkinSpritePositions(SkinData _skin)
        {
            SpriteRenderer[] GOSRs = _skin.SkinGo.GetComponentsInChildren<SpriteRenderer>(true);
            string s = _skin.SkinName + ":\n";
            foreach (SpriteRenderer _sr in GOSRs)
                s += _sr.sprite.name + ": (" + _sr.sprite.rect.xMin + "," + (_sr.sprite.texture.height - _sr.sprite.rect.yMin) + "),(" + _sr.sprite.rect.xMax + "," + (_sr.sprite.texture.height - _sr.sprite.rect.yMax) + ")\n";
            LogInfo(s);
        }
        internal static void LogShopItems(string _seed = "", string _shop = "caravanshop", string _node = "", int _townReroll = 0, bool _obeliskChallenge = false, int _madness = 0, int _corruptorCount = 0, bool _poverty = false)
        {
            string reroll = "";
            string node = _node; // have to do this because can't use ref keyword w/ default values
            if (_shop == "caravanshop" && node == "")
                node = "sen_44";
            // #TODO: other shops/nodes
            if (_townReroll > 0)
            {
                // #TODO: I have not looked into how the reroll string is generated
                // reroll = ??
            }
            if (AtOManager.Instance != null) // a game is in progress
            {
                // replace with current game values if defaults are used
                if (_seed == "")
                    _seed = AtOManager.Instance.GetGameId();
                if (node == "")
                    node = AtOManager.Instance.currentMapNode;
                if (_townReroll == 0)
                    reroll = AtOManager.Instance.shopItemReroll;
                if (_obeliskChallenge == false && GameManager.Instance != null && GameManager.Instance.IsObeliskChallenge())
                    _obeliskChallenge = true;
                _madness = _obeliskChallenge ? AtOManager.Instance.GetObeliskMadness() : AtOManager.Instance.GetNgPlus();
                _corruptorCount = AtOManager.Instance.GetMadnessDifficulty() - _madness;
                _poverty = AtOManager.Instance.IsChallengeTraitActive("poverty") || MadnessManager.Instance != null && MadnessManager.Instance.IsMadnessTraitActive("poverty");
            }
            LootData lootData = Globals.Instance.GetLootData(_shop);
            if (lootData == null)
            {
                LogError("Unable to get shop items for " + _shop + " (shop does not exist!)");
                return;
            }
            List<string> ts1 = new List<string>();
            List<string> ts2 = new List<string>();
            for (int index = 0; index < Globals.Instance.CardListByClass[Enums.CardClass.Item].Count; ++index)
                ts2.Add(Globals.Instance.CardListByClass[Enums.CardClass.Item][index]);
            int deterministicHashCode = (node + _seed + reroll).GetDeterministicHashCode();
            UnityEngine.Random.InitState(deterministicHashCode);
            ts2.Shuffle<string>(deterministicHashCode);
            int index1 = 0;
            int num1 = !_obeliskChallenge ? lootData.NumItems : lootData.LootItemTable.Length;
            for (int index2 = 0; index2 < num1 && ts1.Count < lootData.NumItems; ++index2)
            {
                if (index2 < lootData.LootItemTable.Length)
                {
                    LootItem lootItem = lootData.LootItemTable[index2];
                    if ((double)UnityEngine.Random.Range(0, 100) < (double)lootItem.LootPercent)
                    {
                        if (lootItem.LootCard != null)
                        {
                            ts1.Add(lootItem.LootCard.Id);
                        }
                        else
                        {
                            bool flag = false;
                            int num2 = 0;
                            CardData cardData = (CardData)null;
                            for (; !flag && num2 < 10000; ++num2)
                            {
                                if (index1 >= ts2.Count)
                                    index1 = 0;
                                string str = ts2[index1];
                                // if (!ts1.Contains(str) && (!AtOManager.Instance.ItemBoughtOnThisShop(_itemListId, str) && AtOManager.Instance.HowManyTownRerolls() > 0 || AtOManager.Instance.HowManyTownRerolls() == 0))
                                // replaced with a line that DOESN'T check if item has already been bought
                                // usually, if an item has been bought it will not show up in town again
                                if (!ts1.Contains(str))
                                {
                                    cardData = Globals.Instance.GetCardData(str, false);
                                    if (cardData.Item != null && !cardData.Item.DropOnly)
                                    {
                                        if (cardData.CardUpgraded == Enums.CardUpgraded.Rare)
                                            flag = false;
                                        else if (cardData.Item.PercentRetentionEndGame > 0 && (_madness > 2 || _obeliskChallenge))
                                            flag = false;
                                        else if (cardData.Item.PercentDiscountShop > 0 && _poverty)
                                            flag = false;
                                        else if (lootItem.LootType == Enums.CardType.None && cardData.CardRarity == lootItem.LootRarity)
                                            flag = true;
                                        else if (cardData.CardType == lootItem.LootType && cardData.CardRarity == lootItem.LootRarity)
                                            flag = true;
                                    }
                                }
                                ++index1;
                            }
                            if (flag && cardData != null)
                                ts1.Add(cardData.Id);
                        }
                    }
                }
            }
            for (int count = ts1.Count; count < lootData.NumItems; ++count)
            {
                bool flag = false;
                int num3 = 0;
                CardData cardData = (CardData)null;
                int num4 = UnityEngine.Random.Range(0, 100);
                while (!flag && num3 < 10000)
                {
                    if (index1 >= ts2.Count)
                        index1 = 0;
                    string str = ts2[index1];
                    // if (!ts1.Contains(str) && (!AtOManager.Instance.ItemBoughtOnThisShop(_itemListId, str) && AtOManager.Instance.HowManyTownRerolls() > 0 || AtOManager.Instance.HowManyTownRerolls() == 0))
                    // replaced with a line that DOESN'T check if item has already been bought
                    // usually, if an item has been bought it will not show up in town again
                    if (!ts1.Contains(str))
                    {
                        cardData = Globals.Instance.GetCardData(str, false);
                        if (cardData.Item != null && !cardData.Item.DropOnly)
                        {
                            if (cardData.CardUpgraded == Enums.CardUpgraded.Rare)
                                flag = false;
                            else if (cardData.Item.PercentRetentionEndGame > 0 && (_madness > 2 || _obeliskChallenge))
                                flag = false;
                            else if (cardData.Item.PercentDiscountShop > 0 && _poverty)
                                flag = false;
                            else if ((double)num4 < (double)lootData.DefaultPercentMythic)
                            {
                                if (cardData.CardRarity == Enums.CardRarity.Mythic)
                                    flag = true;
                            }
                            else if ((double)num4 < (double)lootData.DefaultPercentEpic + (double)lootData.DefaultPercentMythic)
                            {
                                if (cardData.CardRarity == Enums.CardRarity.Epic)
                                    flag = true;
                            }
                            else if ((double)num4 < (double)lootData.DefaultPercentRare + (double)lootData.DefaultPercentEpic + (double)lootData.DefaultPercentMythic)
                            {
                                if (cardData.CardRarity == Enums.CardRarity.Rare)
                                    flag = true;
                            }
                            else if ((double)num4 < (double)lootData.DefaultPercentUncommon + (double)lootData.DefaultPercentRare + (double)lootData.DefaultPercentEpic + (double)lootData.DefaultPercentMythic)
                            {
                                if (cardData.CardRarity == Enums.CardRarity.Uncommon)
                                    flag = true;
                            }
                            else if (cardData.CardRarity == Enums.CardRarity.Common)
                                flag = true;
                        }
                    }
                    ++index1;
                    ++num3;
                    if (!flag && num3 % 100 == 0)
                        num4 += 10;
                }
                if (flag && cardData != null)
                    ts1.Add(cardData.Id);
                else
                    break;
            }
            ts1.Shuffle<string>(deterministicHashCode);
            if (!_shop.Contains("towntier") && (!_obeliskChallenge && _madness > 0 || _obeliskChallenge))
            {
                int num5 = 0;
                if (_shop.Contains("exoticshop"))
                    num5 += 8;
                else if (_shop.Contains("rareshop"))
                    num5 += 4;
                if (_obeliskChallenge)
                {
                    if (_madness > 8)
                        num5 += 4;
                    else if (_madness > 4)
                        num5 += 2;
                }
                else
                    num5 += Functions.FuncRoundToInt(0.2f * (float)(_madness + _corruptorCount));
                for (int index3 = 0; index3 < ts1.Count; ++index3)
                {
                    int num6 = UnityEngine.Random.Range(0, 100);
                    CardData cardData = Globals.Instance.GetCardData(ts1[index3], false);
                    if (!(cardData == null))
                    {
                        bool flag = false;
                        if ((cardData.CardRarity == Enums.CardRarity.Mythic || cardData.CardRarity == Enums.CardRarity.Epic) && num6 < 3 + num5)
                            flag = true;
                        else if (cardData.CardRarity == Enums.CardRarity.Rare && num6 < 5 + num5)
                            flag = true;
                        else if (cardData.CardRarity == Enums.CardRarity.Uncommon && num6 < 7 + num5)
                            flag = true;
                        else if (cardData.CardRarity == Enums.CardRarity.Common && num6 < 9 + num5)
                            flag = true;
                        if (flag && cardData.UpgradesToRare != null)
                            ts1[index3] = cardData.UpgradesToRare.Id;
                    }
                }
            }
            LogInfo("SHOP CONTENTS for " + _shop + " at node " + node + " in seed " + _seed + " (reroll: " + (reroll == "" ? _townReroll.ToString() : reroll) + ", " + (_obeliskChallenge ? "OC " : "") + "madness " + _madness.ToString() + "|" + _corruptorCount.ToString() + (_poverty ? " with poverty" : "") + ")");
            foreach (string cardID in ts1)
            {
                CardData card = Globals.Instance.GetCardData(cardID);
                if (card != null)
                    LogInfo(card.CardName + (card.CardUpgraded == CardUpgraded.Rare ? " (Corrupted)" : (card.CardUpgraded == CardUpgraded.A ? " (Blue)" : (card.CardUpgraded == CardUpgraded.B ? " (Yellow)" : ""))) + " [" + card.Id + "]");
            }
        }
        internal static void CalculateChecksums(bool _FaloRowi = false)
        {
            LogInfo("CALCULATING CHECKSUMS");
            medsCheckSummary = new();
            string sPath = Path.Combine(Paths.ConfigPath, "CHECKSUMS" + (GameManager.Instance != null && GameManager.Instance.IsMultiplayer() ? "_MP" : "") + ".txt");
            List<string> checksums = new(){
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_SubClassSource").GetValue<Dictionary<string, SubClassData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_TraitsSource").GetValue<Dictionary<string, TraitData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_PerksSource").GetValue<Dictionary<string, PerkData>>().Select(item => item.Value).ToArray()),
                //ActualChecksums(Traverse.Create(Globals.Instance).Field("_AurasCursesSource").GetValue<Dictionary<string, AuraCurseData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_NPCsSource").GetValue<Dictionary<string, NPCData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_NodeDataSource").GetValue<Dictionary<string, NodeData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_LootDataSource").GetValue<Dictionary<string, LootData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_PerksNodesSource").GetValue<Dictionary<string, PerkNodeData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_WeeklyDataSource").GetValue<Dictionary<string, ChallengeData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_ChallengeTraitsSource").GetValue<Dictionary<string, ChallengeTrait>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_CombatDataSource").GetValue<Dictionary<string, CombatData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_Events").GetValue<Dictionary<string, EventData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_Requirements").GetValue<Dictionary<string, EventRequirementData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_ZoneDataSource").GetValue<Dictionary<string, ZoneData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Globals.Instance.KeyNotes.Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_PackDataSource").GetValue<Dictionary<string, PackData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_CardPlayerPackDataSource").GetValue<Dictionary<string, CardPlayerPackData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_ItemDataSource").GetValue<Dictionary<string, ItemData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_CardbackDataSource").GetValue<Dictionary<string, CardbackData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_SkinDataSource").GetValue<Dictionary<string, SkinData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_CorruptionPackDataSource").GetValue<Dictionary<string, CorruptionPackData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_Cinematics").GetValue<Dictionary<string, CinematicData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_TierRewardDataSource").GetValue<Dictionary<int, TierRewardData>>().Select(item => item.Value).ToArray()),
                ActualChecksums(Traverse.Create(Globals.Instance).Field("_CardPlayerPairsPackDataSource").GetValue<Dictionary<string, CardPlayerPairsPackData>>().Select(item => item.Value).ToArray())
            };
            File.WriteAllText(sPath, "CHECKSUMS\nSUMMARY:\n" + String.Join("\n", medsCheckSummary) + "\n\n=================================================================================\n" + (medsVersionText.Split("\n").Length).ToString() + " mods with overall checksum " + medsVersionText.GetHashCode().ToString() + "\n" + medsVersionText);
            foreach (string _checksum in checksums)
                File.AppendAllText(sPath, _checksum);
            if (_FaloRowi)
            {
                File.AppendAllText(sPath, "\n\nArchitect's Ring CARD\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetCardData("architectsring"))));
                File.AppendAllText(sPath, "\n\nArchitect's Ring ITEM\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetItemData("architectsring"))));
                File.AppendAllText(sPath, "\n\nSacred Axe CARD\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetCardData("sacredaxe"))));
                File.AppendAllText(sPath, "\n\nSacred Axe ITEM\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetItemData("sacredaxe"))));
                File.AppendAllText(sPath, "\n\nTurban CARD\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetCardData("turban"))));
                File.AppendAllText(sPath, "\n\nTurban ITEM\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetItemData("turban"))));
                File.AppendAllText(sPath, "\n\nTurban (rare) CARD\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetCardData("turbanrare"))));
                File.AppendAllText(sPath, "\n\nTurban (rare) ITEM\n" + JsonUtility.ToJson(DataTextConvert.ToText(Globals.Instance.GetItemData("turbanrare"))));
            }
        }
        public static string ActualChecksums<T>(T[] data)
        {
            string type = "";
            string result = "";
            for (int a = 1; a <= data.Length; a++)
            {
                string id = "";
                string text = "";
                if (data[a - 1].GetType() == typeof(SubClassData))
                {
                    type = "subclass";
                    SubClassData d = (SubClassData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(TraitData))
                {
                    type = "trait";
                    TraitData d = (TraitData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CardData))
                {
                    type = "card";
                    CardData d = (CardData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(PerkData))
                {
                    type = "perk";
                    PerkData d = (PerkData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(AuraCurseData))
                {
                    type = "auraCurse";
                    AuraCurseData d = (AuraCurseData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(NPCData))
                {
                    type = "npc";
                    NPCData d = (NPCData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(NodeData))
                {
                    type = "node";
                    NodeData d = (NodeData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(LootData))
                {
                    type = "loot";
                    LootData d = (LootData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(PerkNodeData))
                {
                    type = "perkNode";
                    PerkNodeData d = (PerkNodeData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(ChallengeData))
                {
                    type = "challengeData";
                    ChallengeData d = (ChallengeData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(ChallengeTrait))
                {
                    type = "challengeTrait";
                    ChallengeTrait d = (ChallengeTrait)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CombatData))
                {
                    type = "combatData";
                    CombatData d = (CombatData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(EventData))
                {
                    type = "event";
                    EventData d = (EventData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(EventReplyDataText))
                {
                    type = "eventReply";
                    EventReplyDataText d = (EventReplyDataText)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(d).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(EventRequirementData))
                {
                    type = "eventRequirement";
                    EventRequirementData d = (EventRequirementData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(ZoneData))
                {
                    type = "zone";
                    ZoneData d = (ZoneData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(KeyNotesData))
                {
                    type = "keynote";
                    KeyNotesData d = (KeyNotesData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(PackData))
                {
                    type = "pack";
                    PackData d = (PackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CardPlayerPackData))
                {
                    type = "cardPlayerPack";
                    CardPlayerPackData d = (CardPlayerPackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CardPlayerPairsPackData))
                {
                    type = "pairsPack";
                    CardPlayerPairsPackData d = (CardPlayerPairsPackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(ItemData))
                {
                    type = "item";
                    ItemData d = (ItemData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CardbackData))
                {
                    type = "cardback";
                    CardbackData d = (CardbackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(SkinData))
                {
                    type = "skin";
                    SkinData d = (SkinData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CorruptionPackData))
                {
                    type = "corruptionPack";
                    CorruptionPackData d = (CorruptionPackData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(CinematicData))
                {
                    type = "cinematic";
                    CinematicData d = (CinematicData)(object)data[a - 1];
                    id = DataTextConvert.ToString(d);
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else if (data[a - 1].GetType() == typeof(TierRewardData))
                {
                    type = "tierReward";
                    TierRewardData d = (TierRewardData)(object)data[a - 1];
                    id = d.TierNum.ToString();
                    text = JsonUtility.ToJson(DataTextConvert.ToText(d)).GetHashCode().ToString();
                }
                else
                {
                    Log.LogError("Unknown type while extracting data: " + data[a - 1].GetType());
                    return result;
                }
                result += "\n" + id + ": " + text;
            }
            medsCheckSummary.Add(type + " (" + data.Length.ToString() + "): " + result.GetHashCode().ToString());
            result = "\n\n=================================================================================\n" + data.Length.ToString() + " " + type + " with overall checksum " + result.GetHashCode().ToString() + result;
            return result;
        }


    }
}
