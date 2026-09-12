using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using Cards;
using static Obeliskial_Essentials.Essentials;
using System.IO;
using HarmonyLib;
using BepInEx;
using System.Linq;
using static Obeliskial_Essentials.Patches;

namespace Obeliskial_Essentials
{
    /// <summary>
    /// Reflection-based export of AtO data objects to JSON.
    /// New fields and nested objects on game types are picked up automatically.
    /// ScriptableObject references become IDs; nested plain objects are expanded in place.
    /// ItemData nested on cards is fully expanded (same as the old exporter).
    /// </summary>
    public class DataTextConvert
    {
        static readonly Dictionary<Type, string> ExportFolders = new()
        {
            { typeof(SubClassData), "subclass" },
            { typeof(TraitData), "trait" },
            { typeof(CardRealtimeData), "card" },
            { typeof(CardDataNew), "card" },
            { typeof(PerkData), "perk" },
            { typeof(AuraCurseData), "auraCurse" },
            { typeof(NPCData), "npc" },
            { typeof(NodeData), "node" },
            { typeof(LootData), "loot" },
            { typeof(PerkNodeData), "perkNode" },
            { typeof(ChallengeData), "challengeData" },
            { typeof(ChallengeTrait), "challengeTrait" },
            { typeof(CombatData), "combatData" },
            { typeof(EventData), "event" },
            { typeof(EventReplyData), "eventReply" },
            { typeof(EventReplyDataText), "eventReply" },
            { typeof(EventRequirementData), "eventRequirement" },
            { typeof(ZoneData), "zone" },
            { typeof(KeyNotesData), "keynote" },
            { typeof(PackData), "pack" },
            { typeof(CardPlayerPackData), "cardPlayerPack" },
            { typeof(CardPlayerPairsPackData), "pairsPack" },
            { typeof(ItemData), "item" },
            { typeof(CardbackData), "cardback" },
            { typeof(SkinData), "skin" },
            { typeof(CorruptionPackData), "corruptionPack" },
            { typeof(CinematicData), "cinematic" },
            { typeof(TierRewardData), "tierReward" },
        };

        static readonly string[] IdPropertyNames =
        {
            "Id", "ID", "NodeId", "CombatId", "EventId", "ZoneId", "RequirementId",
            "CardbackId", "SkinId", "PackId", "CinematicId", "PackName", "medsTempID"
        };

        static readonly Dictionary<Type, MemberCache> MemberCaches = new();

        class MemberCache
        {
            public List<PropertyInfo> Properties = new();
            public List<FieldInfo> Fields = new();
        }

        class ExportContext
        {
            public object Root;
            public Type RootType;
            public string RootId;
            public string CardClass;
            public bool Full;
            public readonly HashSet<object> Visiting = new(ReferenceEqualityComparer.Instance);
        }

        sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }

        public static string GetExportFolder(object data)
        {
            if (data == null)
                return "unknown";
            if (data is Dictionary<string, object>)
                return "eventReply";
            Type type = data.GetType();
            if (ExportFolders.TryGetValue(type, out string folder))
                return folder;
            return type.Name;
        }

        public static Dictionary<string, object> ToText(object data)
        {
            return ConvertRoot(data, false);
        }

        public static Dictionary<string, object> ToFULLText(NodeData data)
        {
            return ConvertRoot(data, true);
        }

        public static Dictionary<string, object> ToText(PlayerDeck data)
        {
            Dictionary<string, object> result = new();
            List<object> heroes = new();
            if (data?.DeckTitle != null)
            {
                foreach (string subclassID in data.DeckTitle.Keys)
                {
                    Dictionary<string, object> hero = new() { { "SubclassID", subclassID } };
                    List<object> savedDecks = new();
                    for (int a = 0; a < data.DeckTitle[subclassID].Length; a++)
                    {
                        savedDecks.Add(new Dictionary<string, object>
                        {
                            { "Title", data.DeckTitle[subclassID][a] },
                            { "Cards", ToObjectList(data.DeckCards[subclassID][a]) }
                        });
                    }
                    hero["SavedDecks"] = savedDecks;
                    heroes.Add(hero);
                }
            }
            result["Heroes"] = heroes;
            return result;
        }

        public static Dictionary<string, object> ToText(PlayerPerk data)
        {
            Dictionary<string, object> result = new();
            List<object> heroes = new();
            if (data?.PerkConfigTitle != null)
            {
                foreach (string subclassID in data.PerkConfigTitle.Keys)
                {
                    Dictionary<string, object> hero = new() { { "SubclassID", subclassID } };
                    List<object> savedPerks = new();
                    for (int a = 0; a < data.PerkConfigTitle[subclassID].Length; a++)
                    {
                        savedPerks.Add(new Dictionary<string, object>
                        {
                            { "Title", data.PerkConfigTitle[subclassID][a] },
                            { "Perks", ToObjectList(data.PerkConfigPerks[subclassID][a]) },
                            { "Points", data.PerkConfigPoints[subclassID][a] }
                        });
                    }
                    hero["SavedPerks"] = savedPerks;
                    heroes.Add(hero);
                }
            }
            result["Heroes"] = heroes;
            return result;
        }

        static List<object> ToObjectList(IEnumerable values)
        {
            List<object> list = new();
            if (values == null)
                return list;
            foreach (object value in values)
                list.Add(value);
            return list;
        }

        public static string ToString(object data)
        {
            return GetReferenceId(data);
        }

        public static string ToJson(object value, bool pretty = true)
        {
            StringBuilder sb = new();
            WriteJson(sb, value, pretty, 0);
            return sb.ToString();
        }

        static Dictionary<string, object> ConvertRoot(object data, bool full)
        {
            if (data == null)
                return new Dictionary<string, object>();
            if (data is Dictionary<string, object> existing)
                return existing;
            if (data is PlayerDeck deck)
                return ToText(deck);
            if (data is PlayerPerk perk)
                return ToText(perk);

            ExportContext ctx = new()
            {
                Root = data,
                RootType = data.GetType(),
                RootId = GetReferenceId(data),
                Full = full
            };
            ctx.CardClass = GetMemberString(data, "CardClass");
            object converted = ConvertValue(data, ctx, isRoot: true);
            return converted as Dictionary<string, object> ?? new Dictionary<string, object>();
        }

        static object ConvertValue(object value, ExportContext ctx, bool isRoot = false)
        {
            if (value == null)
                return null;
            if (value is UnityEngine.Object unityObj && unityObj == null)
                return "";

            Type type = value.GetType();
            if (type.IsEnum)
                return Enum.GetName(type, value) ?? value.ToString();

            if (IsPrimitiveJsonValue(type))
                return value;

            if (value is string || value is char || value is Guid)
                return value.ToString();

            if (value is Vector2 || value is Vector3 || value is Vector4)
                return value.ToString();

            if (value is Dictionary<string, object> already)
                return already;

            if (!isRoot && value is UnityEngine.Object unityValue)
                return ConvertUnityObject(unityValue, ctx);

            if (value is IDictionary dictionary)
                return ConvertDictionary(dictionary, ctx);

            if (value is IEnumerable enumerable && value is not string && value is not UnityEngine.Object)
                return ConvertEnumerable(enumerable, ctx);

            if (ShouldSkipNestedType(type))
                return type.Name;

            return ConvertObject(value, ctx, isRoot);
        }

        static object ConvertUnityObject(UnityEngine.Object value, ExportContext ctx)
        {
            if (value is Sprite sprite)
                MaybeExportSprite(sprite, ctx);
            else if (value is GameObject gameObject)
                MaybeExportGameObjectSprites(gameObject, ctx);

            if (ShouldEmbedUnityObject(value, ctx))
                return ConvertObject(value, ctx, isRoot: false);

            return GetReferenceId(value);
        }

        static bool ShouldEmbedUnityObject(UnityEngine.Object value, ExportContext ctx)
        {
            if (value is Sprite || value is AudioClip || value is GameObject || value is Component || value is Texture)
                return false;
            if (value is ItemData)
                return true;
            if (ctx.Full && value is EventData)
                return true;
            return false;
        }

        static Dictionary<string, object> ConvertDictionary(IDictionary dictionary, ExportContext ctx)
        {
            Dictionary<string, object> result = new();
            foreach (DictionaryEntry entry in dictionary)
            {
                string key = entry.Key?.ToString() ?? "";
                result[key] = ConvertValue(entry.Value, ctx);
            }
            return result;
        }

        static List<object> ConvertEnumerable(IEnumerable enumerable, ExportContext ctx)
        {
            List<object> list = new();
            foreach (object item in enumerable)
                list.Add(ConvertValue(item, ctx));
            return list;
        }

        static Dictionary<string, object> ConvertObject(object data, ExportContext ctx, bool isRoot)
        {
            if (!data.GetType().IsValueType && !ctx.Visiting.Add(data))
                return new Dictionary<string, object> { { "_cycle", GetReferenceId(data) } };

            Dictionary<string, object> result = new();
            try
            {
                MemberCache members = GetMembers(data.GetType());
                foreach (PropertyInfo property in members.Properties)
                {
                    object raw;
                    try
                    {
                        raw = property.GetValue(data);
                    }
                    catch (Exception ex)
                    {
                        LogDebug("DataTextConvert skipped property " + data.GetType().Name + "." + property.Name + ": " + ex.Message);
                        continue;
                    }
                    result[property.Name] = ConvertValue(raw, ctx);
                }
                foreach (FieldInfo field in members.Fields)
                {
                    object raw;
                    try
                    {
                        raw = field.GetValue(data);
                    }
                    catch (Exception ex)
                    {
                        LogDebug("DataTextConvert skipped field " + data.GetType().Name + "." + field.Name + ": " + ex.Message);
                        continue;
                    }
                    result[ToPascalCase(field.Name)] = ConvertValue(raw, ctx);
                }

                Enrich(data, result, ctx, isRoot);
            }
            finally
            {
                if (!data.GetType().IsValueType)
                    ctx.Visiting.Remove(data);
            }
            return result;
        }

        static void Enrich(object data, Dictionary<string, object> result, ExportContext ctx, bool isRoot)
        {
            if (data is NodeData node)
            {
                if (medsNodeSource != null && !string.IsNullOrEmpty(node.NodeId) && medsNodeSource.ContainsKey(node.NodeId))
                {
                    Vector3 pos = medsNodeSource[node.NodeId].transform.position;
                    result["medsPosX"] = pos.x;
                    result["medsPosY"] = pos.y;
                }
            }
            else if (data is EventData ev)
            {
                string eventId = ev.EventId ?? "";
                result["medsNode"] = medsNodeEvent != null && medsNodeEvent.ContainsKey(eventId) ? medsNodeEvent[eventId] : "";
                result["medsPercent"] = medsNodeEventPercent != null && medsNodeEventPercent.ContainsKey(eventId) ? medsNodeEventPercent[eventId] : 100;
                result["medsPriority"] = medsNodeEventPriority != null && medsNodeEventPriority.ContainsKey(eventId) ? medsNodeEventPriority[eventId] : 0;
                RegisterEventReplies(ev, result, eventId);
            }
            else if (data is EventRequirementData req)
            {
                if (!result.ContainsKey("RequirementZoneFinishTrack"))
                {
                    try
                    {
                        object zone = HarmonyLib.Traverse.Create(req).Field("requirementZoneFinishTrack").GetValue();
                        result["RequirementZoneFinishTrack"] = ConvertValue(zone, ctx);
                    }
                    catch
                    {
                        // Field may not exist in this game version.
                    }
                }
            }

            if (isRoot && data is SubClassData subclass)
            {
                try
                {
                    AudioClip hitSound = subclass.GetHitSound();
                    if (!result.ContainsKey("HitSound") || string.IsNullOrEmpty(result["HitSound"] as string))
                        result["HitSound"] = GetReferenceId(hitSound);
                }
                catch
                {
                    // GetHitSound may not exist in this game version.
                }
            }
        }

        static void RegisterEventReplies(EventData ev, Dictionary<string, object> result, string eventId)
        {
            if (ev.Replys == null)
                return;
            string repliesKey = result.ContainsKey("Replys") ? "Replys" : (result.ContainsKey("Replies") ? "Replies" : null);
            if (repliesKey == null || result[repliesKey] is not List<object> replies)
                return;
            for (int a = 0; a < replies.Count && a < ev.Replys.Length; a++)
            {
                if (replies[a] is not Dictionary<string, object> reply)
                    continue;
                string tempId = eventId + "_" + a.ToString();
                reply["medsEvent"] = eventId;
                reply["medsTempID"] = tempId;
                medsEventReplyDataText[tempId] = reply;
            }
        }

        static MemberCache GetMembers(Type type)
        {
            lock (MemberCaches)
            {
                if (MemberCaches.TryGetValue(type, out MemberCache cached))
                    return cached;
            }

            MemberCache cache = new();
            HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
            for (Type t = type; t != null && t != typeof(object) && t != typeof(ValueType); t = t.BaseType)
            {
                if (t == typeof(UnityEngine.Object) || t == typeof(ScriptableObject) || t == typeof(Component))
                    break;

                foreach (PropertyInfo property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (!property.CanRead || property.GetIndexParameters().Length > 0)
                        continue;
                    MethodInfo getter = property.GetGetMethod();
                    if (getter == null || !getter.IsPublic)
                        continue;
                    if (ShouldSkipMember(property.Name, property.PropertyType))
                        continue;
                    if (!names.Add(property.Name))
                        continue;
                    cache.Properties.Add(property);
                }

                foreach (FieldInfo field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (field.Name.Contains("k__BackingField") || field.Name.Contains("<"))
                        continue;
                    if (ShouldSkipMember(field.Name, field.FieldType))
                        continue;
                    bool include = field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;
                    if (!include)
                        continue;
                    if (names.Contains(field.Name) || names.Contains(ToPascalCase(field.Name)))
                        continue;
                    names.Add(field.Name);
                    cache.Fields.Add(field);
                }
            }

            cache.Properties.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
            cache.Fields.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
            lock (MemberCaches)
            {
                MemberCaches[type] = cache;
            }
            return cache;
        }

        static bool ShouldSkipMember(string name, Type memberType)
        {
            if (string.IsNullOrEmpty(name) || name[0] == '<')
                return true;
            if (ShouldSkipNestedType(memberType))
                return true;
            return false;
        }

        static bool ShouldSkipNestedType(Type type)
        {
            if (type == null)
                return true;
            if (typeof(Delegate).IsAssignableFrom(type) || typeof(Pointer).IsAssignableFrom(type))
                return true;
            if (type == typeof(IntPtr) || type == typeof(UIntPtr) || type == typeof(Type))
                return true;
            if (typeof(Exception).IsAssignableFrom(type) || typeof(MemberInfo).IsAssignableFrom(type))
                return true;
            string typeName = type.Name;
            if (typeName.Contains("<") || typeName.Contains("Resolver") || typeName.Contains("Validator"))
                return true;
            if (typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(type))
                return true;
            if (type.Namespace != null && (type.Namespace.StartsWith("System.Reflection") || type.Namespace.StartsWith("System.Linq.Expressions")))
                return true;
            return false;
        }

        static bool IsPrimitiveJsonValue(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type.IsPrimitive || type == typeof(decimal);
        }

        static string GetMemberString(object data, string name)
        {
            if (data == null)
                return "";
            PropertyInfo property = data.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanRead)
            {
                try
                {
                    object value = property.GetValue(data);
                    return value is Enum ? Enum.GetName(value.GetType(), value) : value?.ToString() ?? "";
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        static string GetReferenceId(object data)
        {
            if (data == null)
                return "";
            if (data is UnityEngine.Object unityObj && unityObj == null)
                return "";
            if (data is string s)
                return s;
            if (data is Dictionary<string, object> dict)
            {
                foreach (string key in IdPropertyNames)
                {
                    if (dict.TryGetValue(key, out object value) && value != null)
                        return value.ToString();
                }
                return "";
            }
            if (data is Enum)
                return Enum.GetName(data.GetType(), data) ?? data.ToString();
            if (data is EventReplyDataText reply)
                return reply.medsTempID ?? "";

            Type type = data.GetType();
            foreach (string name in IdPropertyNames)
            {
                PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null || !property.CanRead)
                    continue;
                try
                {
                    object value = property.GetValue(data);
                    if (value != null)
                        return value.ToString();
                }
                catch
                {
                    // Try the next candidate.
                }
            }

            PropertyInfo tierNum = type.GetProperty("TierNum", BindingFlags.Public | BindingFlags.Instance);
            if (tierNum != null)
            {
                try
                {
                    object value = tierNum.GetValue(data);
                    if (value != null)
                        return value.ToString();
                }
                catch
                {
                    // Fall through to Unity name.
                }
            }

            if (data is UnityEngine.Object unity)
                return unity.name ?? "";
            return "";
        }

        static string ToPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            if (name.StartsWith("_") && name.Length > 1)
                name = name.Substring(1);
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }

        static void MaybeExportSprite(Sprite sprite, ExportContext ctx)
        {
            if (sprite == null || !medsExportJSON.Value || !medsExportSprites.Value)
                return;
            string folder = GetSpriteFolder(ctx.RootType);
            string subType = ctx.RootId ?? "";
            if (ctx.RootType == typeof(CardDataNew) || ctx.RootType == typeof(CardRealtimeData))
                subType = ctx.CardClass ?? "";
            ExportSprite(sprite, folder, subType);
        }

        static void MaybeExportGameObjectSprites(GameObject gameObject, ExportContext ctx)
        {
            if (gameObject == null || !medsExportJSON.Value || !medsExportSprites.Value)
                return;
            string folder = GetSpriteFolder(ctx.RootType);
            foreach (SpriteRenderer renderer in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.sprite != null)
                    ExportSprite(renderer.sprite, folder, ctx.RootId ?? "", "", true);
            }
        }

        static string GetSpriteFolder(Type rootType)
        {
            if (rootType == typeof(NPCData))
                return "NPC";
            if (ExportFolders.TryGetValue(rootType, out string folder))
                return folder;
            return rootType?.Name ?? "sprite";
        }

        static void WriteJson(StringBuilder sb, object value, bool pretty, int indent)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }
            if (value is string str)
            {
                WriteJsonString(sb, str);
                return;
            }
            if (value is bool boolean)
            {
                sb.Append(boolean ? "true" : "false");
                return;
            }
            if (value is char)
            {
                WriteJsonString(sb, value.ToString());
                return;
            }
            if (value is byte or sbyte or short or ushort or int or uint or long or ulong)
            {
                sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                return;
            }
            if (value is float or double or decimal)
            {
                sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                return;
            }
            if (value is Dictionary<string, object> obj)
            {
                sb.Append('{');
                if (obj.Count == 0)
                {
                    sb.Append('}');
                    return;
                }
                bool first = true;
                foreach (KeyValuePair<string, object> kvp in obj)
                {
                    if (!first)
                        sb.Append(',');
                    first = false;
                    if (pretty)
                    {
                        sb.Append('\n');
                        AppendIndent(sb, indent + 1);
                    }
                    WriteJsonString(sb, kvp.Key);
                    sb.Append(pretty ? ": " : ":");
                    WriteJson(sb, kvp.Value, pretty, indent + 1);
                }
                if (pretty)
                {
                    sb.Append('\n');
                    AppendIndent(sb, indent);
                }
                sb.Append('}');
                return;
            }
            if (value is IList list)
            {
                sb.Append('[');
                if (list.Count == 0)
                {
                    sb.Append(']');
                    return;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    if (pretty)
                    {
                        sb.Append('\n');
                        AppendIndent(sb, indent + 1);
                    }
                    WriteJson(sb, list[i], pretty, indent + 1);
                }
                if (pretty)
                {
                    sb.Append('\n');
                    AppendIndent(sb, indent);
                }
                sb.Append(']');
                return;
            }
            WriteJsonString(sb, value.ToString());
        }

        static void AppendIndent(StringBuilder sb, int indent)
        {
            for (int i = 0; i < indent; i++)
                sb.Append("    ");
        }

        static void WriteJsonString(StringBuilder sb, string value)
        {
            sb.Append('"');
            if (value != null)
            {
                foreach (char c in value)
                {
                    switch (c)
                    {
                        case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        default:
                            if (c < ' ')
                                sb.Append("\\u").Append(((int)c).ToString("x4"));
                            else
                                sb.Append(c);
                            break;
                    }
                }
            }
            sb.Append('"');
        }


        public static void ExportAllData()
        {
            LogInfo("PRAYGE; THE EXPORT HAS BEGUN");
            Node[] foundNodes = Resources.FindObjectsOfTypeAll<Node>();
            LogInfo("Found " + foundNodes.Length + " nodes");
            foreach (Node n in foundNodes)
            {
                medsNodeSource[n.name] = n;
            }
            FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "sprite"));
            FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "!combined"));
            FolderCreate(Path.Combine(Paths.ConfigPath, "Obeliskial_exported", "card"));

            Dictionary<string, NodeData> medsNodeDataSource = Traverse.Create(Globals.Instance).Field("_NodeDataSource").GetValue<Dictionary<string, NodeData>>();

            string fullList = "id\tname\tclass\n";
            foreach (KeyValuePair<string, CardRealtimeData> kvp in Patches.medsCardsSourceGlobal)
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
            medsNodeDataSourceGlobal = medsNodeDataSource;
            LogDebug("finished building node-event relationships");
            ExtractData(Traverse.Create(Globals.Instance).Field("_SubClassSource").GetValue<Dictionary<string, SubClassData>>().Select(item => item.Value).ToArray());
            ExtractData(Traverse.Create(Globals.Instance).Field("_TraitsSource").GetValue<Dictionary<string, TraitData>>().Select(item => item.Value).ToArray());
            ExtractData(medsCardsSourceGlobal.Select(item => item.Value).ToArray());
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
            medsExportNode = true;
            medsExportJSON.Value = false; // turn off after exporting*/
            LogInfo("OUR PRAYERS WERE ANSWERED");
        }
    }
}
