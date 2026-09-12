using HarmonyLib;
using System;

namespace ModNameToReplace
{
    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod
    public class AntiCheat
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetObeliskScore")]
        public static bool SetObeliskScorePrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetScore")]
        public static bool SetScorePrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetSingularityScore")]
        public static bool SetSingularityScorePrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetObeliskScoreLeaderboard")]
        public static bool SetObeliskScoreLeaderboardPrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetScoreLeaderboard")]
        public static bool SetScoreLeaderboardPrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetSingularityScoreLeaderboard")]
        public static bool SetSingularityScoreLeaderboardPrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetWeeklyScore")]
        public static bool SetWeeklyScorePrefix(ref SteamManager __instance,
            int score,
            int week,
            string nick,
            string nickgroup,
            bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetWeeklyScoreLeaderboard")]
        public static bool SetWeeklyScoreLeaderboardPrefix(ref SteamManager __instance, int score,
            int week,
            string nick,
            string nickgroup,
            bool singleplayer = true)
        {
            return false;
        }

    }
}