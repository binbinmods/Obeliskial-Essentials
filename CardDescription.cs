using HarmonyLib;
using System;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using static Obeliskial_Essentials.Essentials;
using System.Collections.Generic;
using TMPro;
using BepInEx;
using static Obeliskial_Essentials.CardDescriptionNew;
using UnityEngine.Experimental.U2D;

namespace Obeliskial_Essentials
{
    [HarmonyPatch]
    public class CardDescriptionUpdated
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardData), "AddFormattedDescription")]
        public static void AddFormattedDescriptionPrefix(ref CardData __instance,
                                                   StringBuilder builder, string descriptionId, string[] descriptionArgs)
        {
            // BinbinCustomText(TextLocation.Beginning, ref builder, __instance.Id);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardData), "AddFormattedDescription")]
        public static void AddFormattedDescriptionPostfix(ref CardData __instance,
                                                   StringBuilder builder, string descriptionId, string[] descriptionArgs)
        {
            // BinbinCustomText(TextLocation.End, ref builder, __instance.Id);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardData), "AppendCardDescription")]
        public static void AppendCardDescriptionPrefix(ref CardData __instance,
                                                   Character character, StringBuilder builder, StringBuilder aux, string grColor, string endColor, string br1, string goldColor)
        {
            BinbinCustomText(TextLocation.Beginning, ref builder, __instance.Id);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardData), "AppendCardDescription")]
        public static void AppendCardDescriptionPostfix(ref CardData __instance,
                                                   Character character, StringBuilder builder, StringBuilder aux, string grColor, string endColor, string br1, string goldColor)
        {
            BinbinCustomText(TextLocation.End, ref builder, __instance.Id);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardData), "AppendItemDescription")]
        public static void AppendItemDescriptionPrefix(ref CardData __instance,
                                                   Character character, StringBuilder builder, StringBuilder aux, string grColor, string endColor, string goldColor)
        {
            BinbinCustomText(TextLocation.ItemBeginning, ref builder, __instance.Id);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardData), "AppendItemDescription")]
        public static void AppendItemDescriptionPostfix(ref CardData __instance,
                                                   Character character, StringBuilder builder, StringBuilder aux, string grColor, string endColor, string goldColor)
        {
            BinbinCustomText(TextLocation.End, ref builder, __instance.Id);
        }

    }
}