using HarmonyLib;
using System.Collections.Generic;
using static Enums;
using static Obeliskial_Essentials.Essentials;
using UnityEngine;
using System.Collections;



namespace Obeliskial_Essentials
{
    internal class NodePath
    {
        internal List<string> pathNodes = new();
        internal int bossCount = 0;
        internal int totalXP = 0;
        internal int totalXP3 = 0;
        internal int combatCount = 0;
        internal int corruptorCount = 0;
    }
    public class NodePathCode
    {
        private static Dictionary<string, int> nodeExperience = new();
        private static Dictionary<string, int> nodeExperience3Plus = new();
        private static List<string> nodesWithCorruptor = new();
        private static Dictionary<string, List<string>> nodeConnections = new();
        private static NodeData currentNode = (NodeData)null;
        private static bool act3OrMore = false;
        private static List<string> newZoneNodeIDs = new() { "ulmin_0", "velka_0", "faen_0", "aqua_0", "wolf_0", "voidlow_0", "sen_0" };
        private static List<string> bossNodeIDs = new() { "velka_12", "velka_9", "velka_27", "velka_32", "velka_25", "ulmin_56", "pyr_7", "ulmin_38", "secta_5", "sen_33", "sen_27", "sen_31", "sewers_8", "faen_38", "faen_29", "faen_35", "faen_25", "spider_8", "aqua_27", "aqua_35", "forge_6" };
        private static Dictionary<int, string> pathSteps = new();
        private static Dictionary<string, List<NodePath>> nodePaths = new();
        //idk assume 225 for combat?
        private static void PathScoreCalc()
        {
            Dictionary<string, NodeData> medsNodeDataSource = Traverse.Create(Globals.Instance).Field("_NodeDataSource").GetValue<Dictionary<string, NodeData>>();
            act3OrMore = false; // act 2 data
            foreach (NodeData _node in medsNodeDataSource.Values)
                nodeExperience[_node.NodeId] = GetNodeXP(_node);
            act3OrMore = true; // act 3+ data
            foreach (NodeData _node in medsNodeDataSource.Values)
                nodeExperience3Plus[_node.NodeId] = GetNodeXP(_node);
            foreach (string nodeID in newZoneNodeIDs)
                AddPathStepNew(new List<string>(), nodeID);
            // temporary: ulminin only
            //AddPathStepNew(new List<string>(), "ulmin_0");
            foreach (string startID in nodePaths.Keys)
            {
                foreach (NodePath np in nodePaths[startID])
                {
                    np.corruptorCount = 0;
                    foreach (string nodeID in np.pathNodes)
                    {
                        //LogDebug("PATHNODE: " + nodeID);
                        if (nodeID == "voidlow_26" || nodeID == "voidhigh_13")
                            np.bossCount += 2;
                        else if (bossNodeIDs.Contains(nodeID))
                            np.bossCount++;
                        if (nodeExperience.ContainsKey(nodeID))
                            np.totalXP += nodeExperience[nodeID];
                        if (nodeExperience3Plus.ContainsKey(nodeID))
                            np.totalXP3 += nodeExperience3Plus[nodeID];
                        if (nodesWithCorruptor.Contains(nodeID))
                            np.corruptorCount++;
                    }
                    LogInfo("PATH\t" + string.Join(",", np.pathNodes) + "\t" + np.pathNodes.Count.ToString() + "\t" + np.bossCount.ToString() + "\t" + np.totalXP.ToString() + "\t" + np.totalXP3.ToString() + "\t" + np.corruptorCount.ToString());
                }
            }
        }
        private static void AddPathStepNew(List<string> path, string curNodeID)
        {
            List<string> newPath = new List<string>(path);
            newPath.Add(curNodeID);
            if (!nodeConnections.ContainsKey(curNodeID))
            {
                //LogInfo("PATH!: " + string.Join("\t", newPath.ToArray()));
                //LogDebug("PATH!: " + pathSteps.Values.Join(delimiter: "\t"));
                //AtOManager.Instance.StartCoroutine()
                NodePath thePath = new();
                thePath.pathNodes = newPath;
                if (!nodePaths.ContainsKey(newPath[0]))
                    nodePaths[newPath[0]] = new();
                nodePaths[newPath[0]].Add(thePath);
                // end of path!
            }
            else
            {
                foreach (string nextNodeID in nodeConnections[curNodeID])
                    if (!newPath.Contains(nextNodeID) || nextNodeID == "sen_12")
                        AddPathStepNew(newPath, nextNodeID);
                    
            }
        }
        private static int GetNodeXP(NodeData _node)
        {
            if (_node == null || _node.NodeZone == null || _node.NodeZone.DisableExperienceOnThisZone || _node.NodeZone.ObeliskLow || _node.NodeZone.ObeliskHigh || _node.NodeZone.ObeliskFinal)
                return 0;
            LogDebug("GetNodeXP: " + _node.NodeId);
            currentNode = _node;
            int highestEventXP = 0;
            foreach (EventData _event in _node.NodeEvent)
            {
                int eventXP = GetEventXP(_event, "event");
                if (eventXP > highestEventXP)
                    highestEventXP = eventXP;
            }
            int highestCombatXP = 0;
            foreach (CombatData _combat in _node.NodeCombat)
            {
                int combatXP = GetCombatXP(_combat, "combat");
                if (combatXP > highestCombatXP)
                    highestCombatXP = combatXP;
            }
            foreach (NodeData _nextNode in _node.NodesConnected)
            {
                //LogDebug("curNode: " + currentNode.NodeId + " nextNode: " + _nextNode.NodeId);
                ConnectPath(currentNode.NodeId, _nextNode.NodeId);
            }
            if (highestCombatXP == 0)
            {
                /*nodeCorruptorCount[_node.NodeId] = corruptorCount;
                nodeCombatCount[_node.NodeId] = eventCombatCount;*/
                return highestEventXP;
            }
            else
            {
                /*nodeCorruptorCount[_node.NodeId] = corruptorCount;
                nodeCombatCount[_node.NodeId] = combatCombatCount;*/
                if (!_node.DisableCorruption && _node.NodeId != "sen_1" && _node.NodeId != "sen_2" && _node.NodeId != "sen_3" && _node.NodeId != "aqua_27")
                    nodesWithCorruptor.Add(_node.NodeId);
                return highestCombatXP;
            }
        }
        private static int GetEventXP(EventData _event, string source)
        {
            if (_event == null)
                return 0;
            int highestReplyXP = 0;
            foreach (EventReplyData _reply in _event.Replys)
            {
                int replyXP = GetReplyXP(_reply, source);
                if (replyXP > highestReplyXP)
                    highestReplyXP = replyXP;
            }
            return highestReplyXP;
        }
        private static int GetReplyXP(EventReplyData _reply, string source)
        {
            if (_reply == null)
                return 0;
            int highestOutcomeXP = _reply.SsExperienceReward;
            highestOutcomeXP += GetEventXP(_reply.SsEvent, source);
            highestOutcomeXP += GetCombatXP(_reply.SsCombat, source);
            if (_reply.SsNodeTravel != null)
                ConnectPath(currentNode.NodeId, _reply.SsNodeTravel.NodeId);
            if (_reply.SsRoll) // if there's a roll, rather than just the single outcome...
            {
                int outcomeXP = _reply.FlExperienceReward;
                outcomeXP += GetEventXP(_reply.FlEvent, source);
                outcomeXP += GetCombatXP(_reply.FlCombat, source);
                if (outcomeXP > highestOutcomeXP)
                    highestOutcomeXP = outcomeXP;
                if (_reply.FlNodeTravel != null)
                    ConnectPath(currentNode.NodeId, _reply.FlNodeTravel.NodeId);
                if (_reply.SsRollNumberCritical != -1)
                {
                    outcomeXP = _reply.SscExperienceReward;
                    outcomeXP += GetEventXP(_reply.SscEvent, source);
                    outcomeXP += GetCombatXP(_reply.SscCombat, source);
                    if (outcomeXP > highestOutcomeXP)
                        highestOutcomeXP = outcomeXP;
                    if (_reply.SscNodeTravel != null)
                        ConnectPath(currentNode.NodeId, _reply.SscNodeTravel.NodeId);
                }
                if (_reply.SsRollNumberCriticalFail != -1)
                {
                    outcomeXP = _reply.FlcExperienceReward;
                    outcomeXP += GetEventXP(_reply.FlcEvent, source);
                    outcomeXP += GetCombatXP(_reply.FlcCombat, source);
                    if (outcomeXP > highestOutcomeXP)
                        highestOutcomeXP = outcomeXP;
                    if (_reply.FlcNodeTravel != null)
                        ConnectPath(currentNode.NodeId, _reply.FlcNodeTravel.NodeId);
                }
            }
            return highestOutcomeXP;
        }
        private static int GetCombatXP(CombatData _combat, string source)
        {
            if (_combat == null)
                return 0;
            int combatXP = 0;
            if (currentNode.NodeCombatTier != CombatTier.T0 &&
                !currentNode.DisableRandom &&
                _combat.CombatId != "eaqua_37a" &&
                _combat.CombatId != "eaqua_37b")
            {
                switch (currentNode.NodeCombatTier)
                {
                    case CombatTier.T1:
                        combatXP += 21; // Bob
                        combatXP += 20 + 20 + 19; // Living Rock + Raider + Farmer/Hatchling/Initiated/Raider/Sheep Shearer/Squirel [sic]/Wild Boar
                        break;
                    case CombatTier.T2:
                        combatXP += 35; // Kyno or Treevor
                        combatXP += 32 + 32 + 30; // Tainted Trunky/Tainted Dryad/Trunky
                        break;
                    case CombatTier.T3:
                        combatXP += act3OrMore ? 59 : 53; // Monty
                        combatXP += act3OrMore ? (62 + 62 + 61) : (54 + 54 + 54); // Difficulty 7: act3+: crucible_b/pendulum_plus_b/slime_plus_b | act2-: slime_plus/stormbringer_plus/crucible_plus
                        break;
                    case CombatTier.T4:
                        combatXP += act3OrMore ? 62 : 53; // Shredder
                        combatXP += act3OrMore ? (65 + 64 + 64) : (58 + 58 + 58); // Difficulty 9: act3+: warden_plus_b/reaper_plus_b/dracomancer_b | act2-: dracomancer/halberdier/spellbinder
                        break;
                    case CombatTier.T5:
                    case CombatTier.T7:
                        combatXP += act3OrMore ? 65 : 61; // Ariel/Celsius/Deifrang/Graham/Phil/Rusty
                        combatXP += act3OrMore ? (65 + 64 + 64) : (58 + 58 + 58); // Difficulty 9: act3+: warden_plus_b/reaper_plus_b/dracomancer_b | act2-: dracomancer/halberdier/spellbinder
                        break;
                    case CombatTier.T6:
                        combatXP += act3OrMore ? 62 : 53; // Shredder
                        combatXP += act3OrMore ? (62 + 62 + 61) : (54 + 54 + 54); // Difficulty 7: act3+: crucible_b/pendulum_plus_b/slime_plus_b | act2-: slime_plus/stormbringer_plus/crucible_plus
                        break;
                    case CombatTier.T10:
                        combatXP += 80; // Alexander/Bolphyer/Freezer/Guts/Martina/Neptu/Rocco/Stephen/Tayra/Vulcan
                        combatXP += 60 * 3; // Difficulty 11 (all 60 xp)
                        break;
                    case CombatTier.T11:
                    case CombatTier.T12:
                        combatXP += 80; // Alexander/Bolphyer/Freezer/Guts/Martina/Neptu/Rocco/Stephen/Tayra/Vulcan
                        combatXP += 80 * 3; // Difficulty 13: Dualist/Dark Knight/Knight/Angel/Dark Angel
                        break;
                    default:
                        LogError("Unsupported combat tier! node: " + currentNode.NodeId + " | combat: " + _combat.CombatId);
                        break;
                }
            }
            else
            {
                foreach (NPCData _npc in _combat.NPCList)
                {
                    NPCData curNPC = _npc;
                    if (curNPC == null)
                        continue;
                    if (act3OrMore && curNPC.UpgradedMob != null)
                        curNPC = curNPC.UpgradedMob;
                    if (curNPC.NgPlusMob != null)
                        curNPC = curNPC.NgPlusMob;
                    if (curNPC.HellModeMob != null)
                        curNPC = curNPC.HellModeMob;
                    combatXP += curNPC.ExperienceReward;
                }
            }
            combatXP += GetEventXP(_combat.EventData, source);
            return combatXP;
        }
        private static void ConnectPath(string nodeFrom, string nodeTo)
        {
            if (!newZoneNodeIDs.Contains(nodeTo))
            {
                if (!nodeConnections.ContainsKey(nodeFrom))
                    nodeConnections[nodeFrom] = new List<string>();
                if (!nodeConnections[nodeFrom].Contains(nodeTo))
                    nodeConnections[nodeFrom].Add(nodeTo);
            }

        }

    }
}
