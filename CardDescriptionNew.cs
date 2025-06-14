﻿using HarmonyLib;
using System;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using static Obeliskial_Essentials.Essentials;
using System.Collections.Generic;
using TMPro;
using BepInEx;
using UnityEngine.Experimental.U2D;

namespace Obeliskial_Essentials
{
    [HarmonyPatch]
    public class CardDescriptionNew
    {
        public static string medsColorUpgradePlain = "5E3016";
        public static string medsColorUpgradeGold = "875700";
        public static string medsColorUpgradeBlue = "215382";
        public static string medsColorUpgradeRare = "7F15A6";

        public enum TextLocation
        {
            Beginning,
            AfterAddDiscardDraw,
            AfterDispelSummonBeforeTransfer,
            AfterTransferIncreaseACBeforeDamage,
            AfterDamageBeforeHeal,
            AfterHealBeforeAuras,
            AfterAurasBeforeCurses,
            AfterCurseBeforeSpecialValue,
            AfterSpecialBeforeRepeat,
            ItemBeginning,
            ItemAfterHPResistsBeforeStats,
            ItemAfterStatsBeforeDamageBonus,
            ItemAfterDamageBonusBeforeHeal,
            ItemAfterHealBonusBeforeACBonus,
            ItemAfterACBonusBeforeMiscText,
            ItemEnchantBeforeCast,
            ItemBeforeActivation,
            ItemAfterActivationBeforeEvent,
            ItemActivationBeforeGoldShards,
            ItemActivationAfterGoldBeforeHeal,
            ItemActivationAfterHealBeforeDamage,
            ItemActivationAfterDamageBeforeAC,
            ItemActivationAfterACBeforeMiscText,
            ItemActivationAfterMiscText,
            ItemActivationAfterACBeforeSelf,
            ItemActivationAfterSelfBeforeAddCard,
            ItemActivationAfterAddCardBeforeReduceCost,
            ItemActivationAfterAllActivationBeforeDestroyConditions,
            End
        }

        public static Dictionary<TextLocation, Dictionary<string, List<string>>> TextToAddAtLocations;

        public static HashSet<string> ModifiedCards;


        /*  this is a copy of CardData.Instance.SetDescriptionNew() with some minor modifications:
                gold/shards gain for enchantments
            
            below is a copy of CardData.Instance.SetTarget() that has been rearranged for readability and has the following additions:
                hero cards: target middle hero
                hero cards: target slowest hero
                hero cards: target fastest hero
                hero cards: target least HP hero
                hero cards: target most HP hero
                hero cards: target middle monster
                hero cards: target slowest monster
                hero cards: target fastest monster
                hero cards: target least HP monster
                hero cards: target most HP monster
        */
        /*
        else if (!itemData.IsEnchantment && itemData.CastedCardType == Enums.CardType.None && (itemData.Activation == Enums.EventActivation.PreFinishCast || itemData.Activation == Enums.EventActivation.FinishCast || itemData.Activation == Enums.EventActivation.FinishFinishCast) && !itemData.EmptyHand)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemWhenYouPlay"), (object)"  <sprite name=cards>"));
                            stringBuilder1.Append(":\n");
                        }
        // CUSTOM DESCRIPTION: GOLD/SHARDS
                        if (__instance.GoldGainQuantity > 0 || __instance.ShardsGainQuantity > 0)
                        {
                            stringBuilder1.Append("Gain <color=#1E650F>");
                            if (__instance.GoldGainQuantity > 0)
                                stringBuilder1.Append(__instance.GoldGainQuantity.ToString() + "   <sprite name=gold>");
                            if (__instance.ShardsGainQuantity > 0)
                                stringBuilder1.Append(__instance.ShardsGainQuantity.ToString() + "   <sprite name=dust>");
                            stringBuilder1.Append("</color>\n");
                        }
                        if (__instance.GoldGainQuantity < 0 || __instance.ShardsGainQuantity < 0)
                        {
                            stringBuilder1.Append("Lose <color=#B00A00>");
                            if (__instance.GoldGainQuantity < 0)
                                stringBuilder1.Append(__instance.GoldGainQuantity.ToString() + "   <sprite name=gold>");
                            if (__instance.ShardsGainQuantity < 0)
                                stringBuilder1.Append(__instance.ShardsGainQuantity.ToString() + "   <sprite name=dust>");
                            stringBuilder1.Append("</color>\n");
                        }
                        // END CUSTOM DESCRIPTION: GOLD/SHARDS

                        // BEGIN CUSTOM DESCRIPTION - Multiplier for multiple Transforms

                        // if (str7 != "")
                        //     stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTransformIntoAnd"), (object)str5, (object)stringBuilder2.ToString(), (object)str7));
                        // else
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTransformInto"), (object)str5, (object)stringBuilder2.ToString()));
                        stringBuilder1.Append(str7);
                        stringBuilder1.Append(thirdACSprite);
                        if (str8 != "")
                        {
                            stringBuilder1.Append(" <c>");
                            stringBuilder1.Append(str8);
                            stringBuilder1.Append("</c>");
                        }
                        // END CUSTOM DESCRIPTION
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                        num2 = 0.0f;
                        str8 = "";
        */

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardData), "SetDescriptionNew")]
        public static bool SetDescriptionNewPrefix(ref CardData __instance,
                                                   //    int ___damagePreCalculated,
                                                   //    int ___damagePreCalculatedCombined,
                                                   //    int ___damagePreCalculatedCombined2, 
                                                   bool forceDescription = false, Character character = null, bool includeInSearch = true)
        {
            string medsDescriptionID = Traverse.Create(__instance).Field("descriptionId").GetValue<string>();
            int medsHealPreCalculated = Traverse.Create(__instance).Field("healPreCalculated").GetValue<int>();
            int medsHealSelfPreCalculated = Traverse.Create(__instance).Field("healSelfPreCalculated").GetValue<int>();
            int medsEnchantDamagePreCalculated = Traverse.Create(__instance).Field("enchantDamagePreCalculated").GetValue<int>();
            int medsDamagePreCalculatedCombined = Traverse.Create(__instance).Field("damagePreCalculatedCombined").GetValue<int>();

            string testCard = "transmuterformula";
            string testCard2 = "flashheal";
            if (__instance.Id == testCard)
            {
                LogDebug($"Testing Description for {testCard},\n Normalized {__instance.DescriptionNormalized},\n Regular {__instance.Description}");
            }
            if (__instance.Id == testCard2)
            {
                LogDebug($"Testing Description for {testCard2},\n Normalized {__instance.DescriptionNormalized},\n Regular {__instance.Description}");
            }

            // int damagePreCalculatedCombined = 
            if (medsCustomCardDescriptions.ContainsKey(__instance.Id))
            {
                if (__instance.Id == testCard)
                {
                    LogDebug($"Testing Description Custom for {testCard},\n Normalized {__instance.DescriptionNormalized},\n Regular {__instance.Description}");
                }
                if (__instance.Id == testCard2)
                {
                    LogDebug($"Testing Description Custom for {testCard2},\n Normalized {__instance.DescriptionNormalized},\n Regular {__instance.Description}");
                }

                __instance.DescriptionNormalized = medsCustomCardDescriptions[__instance.Id];
            }

            else if (forceDescription || !Globals.Instance.CardsDescriptionNormalized.ContainsKey(__instance.Id))
            {
                StringBuilder stringBuilder1 = new StringBuilder();
                StringBuilder stringBuilder2 = new StringBuilder();
                string str1 = "<line-height=15%><br></line-height>";
                string str2 = "<color=#444><size=-.15>";
                string str3 = "</size></color>";
                string str4 = "<color=#5E3016><size=-.15>";
                int num1 = 0;
                if (medsDescriptionID != "")
                {
                    LogDebug($"Adding Custom Description to {__instance.Id}");
                    stringBuilder1.Append(Functions.FormatStringCard(Texts.Instance.GetText(medsDescriptionID)));
                }
                else if ((UnityEngine.Object)__instance.Item == (UnityEngine.Object)null && (UnityEngine.Object)__instance.ItemEnchantment == (UnityEngine.Object)null)
                {
                    string str5 = "";
                    string str6 = "";
                    string str7 = "";
                    string thirdACSprite = "";
                    float num2 = 0.0f;
                    string str8 = "";
                    bool flag1 = false;
                    bool flag2 = false;
                    bool flag3 = true;

                    // Custom Text: Beginning                   
                    BinbinCustomText(TextLocation.Beginning, ref stringBuilder1, __instance.Id);

                    StringBuilder stringBuilder3 = new StringBuilder();
                    if (__instance.Damage > 0 || __instance.Damage2 > 0 || __instance.DamageSelf > 0 || __instance.DamageSelf2 > 0)
                        flag3 = false;
                    if (__instance.DrawCard != 0 && !__instance.DrawCardSpecialValueGlobal)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDraw"), (object)medsColorTextArray("", medsNumFormat(__instance.DrawCard), medsSpriteText("card"))));
                        stringBuilder1.Append("<br>");
                    }
                    if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.DiscardedCards)
                    {
                        if (__instance.DiscardCardPlace == Enums.CardPlace.Vanish)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsVanish"), (object)medsColorTextArray("", "X", medsSpriteText("card"))));
                        else
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDiscard"), (object)medsColorTextArray("", "X", medsSpriteText("card"))));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.DrawCardSpecialValueGlobal)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDraw"), (object)medsColorTextArray("aura", "X", medsSpriteText("card"))));
                        stringBuilder1.Append("<br>");
                    }
                    if (__instance.AddCard != 0)
                    {
                        string str9;
                        if (__instance.AddCardId != "")
                        {
                            stringBuilder2.Append(medsColorTextArray("", medsNumFormat(__instance.AddCard), medsSpriteText("card")));
                            CardData cardData = Globals.Instance.GetCardData(__instance.AddCardId, false);
                            if ((UnityEngine.Object)cardData != (UnityEngine.Object)null)
                            {
                                stringBuilder2.Append(__instance.ColorFromCardDataRarity(cardData));
                                stringBuilder2.Append(cardData.CardName);
                                stringBuilder2.Append("</color>");
                            }
                            str9 = stringBuilder2.ToString();
                            stringBuilder2.Clear();
                        }
                        else
                        {
                            if (__instance.AddCardChoose > 0)
                                stringBuilder2.Append(medsColorTextArray("", medsNumFormat(__instance.AddCardChoose), medsSpriteText("card")));
                            else
                                stringBuilder2.Append(medsColorTextArray("", medsNumFormat(__instance.AddCard), medsSpriteText("card")));
                            if (__instance.AddCardType != Enums.CardType.None)
                            {
                                stringBuilder2.Append("<color=#5E3016>");
                                stringBuilder2.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)__instance.AddCardType)));
                                stringBuilder2.Append("</color>");
                            }
                            str9 = stringBuilder2.ToString();
                            stringBuilder2.Clear();
                        }
                        string str10 = "";
                        if (__instance.AddCardReducedCost == -1)
                            str10 = !__instance.AddCardVanish ? (!__instance.AddCardCostTurn ? string.Format(Texts.Instance.GetText("cardsAddCost"), (object)0) : string.Format(Texts.Instance.GetText("cardsAddCostTurn"), (object)0)) : (!__instance.AddCardCostTurn ? string.Format(Texts.Instance.GetText("cardsAddCostVanish"), (object)0) : string.Format(Texts.Instance.GetText("cardsAddCostVanish"), (object)0));
                        else if (__instance.AddCardReducedCost > 0)
                            str10 = !__instance.AddCardVanish ? (!__instance.AddCardCostTurn ? string.Format(Texts.Instance.GetText("cardsAddCostReduced"), (object)__instance.AddCardReducedCost) : string.Format(Texts.Instance.GetText("cardsAddCostReducedTurn"), (object)__instance.AddCardReducedCost)) : (!__instance.AddCardCostTurn ? string.Format(Texts.Instance.GetText("cardsAddCostReducedVanish"), (object)__instance.AddCardReducedCost) : string.Format(Texts.Instance.GetText("cardsAddCostReducedVanishTurn"), (object)__instance.AddCardReducedCost));
                        string _id = "";
                        if (__instance.AddCardId != "")
                        {
                            if (__instance.AddCardPlace == Enums.CardPlace.RandomDeck)
                                _id = __instance.TargetSide == Enums.CardTargetSide.Self || __instance.TargetSide == Enums.CardTargetSide.Enemy && __instance.CardClass != Enums.CardClass.Monster ? "cardsIDShuffleDeck" : "cardsIDShuffleTargetDeck";
                            else if (__instance.AddCardPlace == Enums.CardPlace.Hand)
                                _id = "cardsIDPlaceHand";
                            else if (__instance.AddCardPlace == Enums.CardPlace.TopDeck)
                                _id = __instance.TargetSide != Enums.CardTargetSide.Self ? "cardsIDPlaceTargetTopDeck" : "cardsIDPlaceTopDeck";
                            else if (__instance.AddCardPlace == Enums.CardPlace.Discard)
                                _id = __instance.TargetSide == Enums.CardTargetSide.Self || __instance.TargetSide == Enums.CardTargetSide.Enemy && __instance.CardClass != Enums.CardClass.Monster ? "cardsIDPlaceDiscard" : "cardsIDPlaceTargetDiscard";
                        }
                        else if (__instance.AddCardFrom == Enums.CardFrom.Game)
                        {
                            if (__instance.AddCardPlace == Enums.CardPlace.RandomDeck)
                                _id = __instance.AddCardChoose != 0 ? "cardsDiscoverNumberToDeck" : "cardsDiscoverToDeck";
                            else if (__instance.AddCardPlace == Enums.CardPlace.Hand)
                                _id = __instance.AddCardChoose != 0 ? "cardsDiscoverNumberToHand" : "cardsDiscoverToHand";
                            else if (__instance.AddCardPlace == Enums.CardPlace.TopDeck && __instance.AddCardChoose != 0)
                                _id = "cardsDiscoverNumberToTopDeck";
                        }
                        else if (__instance.AddCardFrom == Enums.CardFrom.Deck)
                        {
                            if (__instance.AddCardPlace == Enums.CardPlace.Hand)
                                _id = __instance.AddCardChoose <= 0 ? (__instance.AddCard <= 1 ? "cardsRevealItToHand" : "cardsRevealThemToHand") : "cardsRevealNumberToHand";
                            else if (__instance.AddCardPlace == Enums.CardPlace.TopDeck)
                                _id = __instance.AddCardChoose <= 0 ? (__instance.AddCard <= 1 ? "cardsRevealItToTopDeck" : "cardsRevealThemToTopDeck") : "cardsRevealNumberToTopDeck";
                        }
                        else if (__instance.AddCardFrom == Enums.CardFrom.Discard)
                        {
                            if (__instance.AddCardPlace == Enums.CardPlace.TopDeck)
                                _id = "cardsPickToTop";
                            else if (__instance.AddCardPlace == Enums.CardPlace.Hand)
                                _id = "cardsPickToHand";
                        }
                        else if (__instance.AddCardFrom == Enums.CardFrom.Hand)
                        {
                            if (__instance.TargetSide == Enums.CardTargetSide.Friend)
                            {
                                if (__instance.AddCardPlace == Enums.CardPlace.TopDeck)
                                    _id = "cardsDuplicateToTargetTopDeck";
                                else if (__instance.AddCardPlace == Enums.CardPlace.RandomDeck)
                                    _id = "cardsDuplicateToTargetRandomDeck";
                            }
                            else if (__instance.AddCardPlace == Enums.CardPlace.Hand)
                                _id = "cardsDuplicateToHand";
                        }
                        else if (__instance.AddCardFrom == Enums.CardFrom.Vanish)
                        {
                            if (__instance.AddCardPlace == Enums.CardPlace.TopDeck)
                                _id = "cardsFromVanishToTop";
                            else if (__instance.AddCardPlace == Enums.CardPlace.Hand)
                                _id = "cardsFromVanishToHand";
                        }
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText(_id), (object)str9, (object)medsColorTextArray("", medsNumFormat(__instance.AddCard))));
                        if (str10 != "")
                        {
                            stringBuilder1.Append(" ");
                            stringBuilder1.Append(str2);
                            stringBuilder1.Append(str10);
                            stringBuilder1.Append(str3);
                        }
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.DiscardCard != 0)
                    {
                        if (__instance.DiscardCardType != Enums.CardType.None)
                        {
                            stringBuilder2.Append("<color=#5E3016>");
                            stringBuilder2.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)__instance.DiscardCardType)));
                            stringBuilder2.Append("</color>");
                        }
                        if (__instance.DiscardCardPlace == Enums.CardPlace.Discard)
                        {
                            if (!__instance.DiscardCardAutomatic)
                            {
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDiscard"), (object)medsColorTextArray("", medsNumFormat(__instance.DiscardCard), medsSpriteText("card"))));
                                stringBuilder1.Append(stringBuilder2);
                                stringBuilder1.Append("\n");
                            }
                            else
                            {
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDiscard"), (object)medsColorTextArray("", medsNumFormat(__instance.DiscardCard), medsSpriteText("cardrandom"))));
                                stringBuilder1.Append(stringBuilder2);
                                stringBuilder1.Append("\n");
                            }
                        }
                        else if (__instance.DiscardCardPlace == Enums.CardPlace.TopDeck)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPlaceToTop"), (object)medsColorTextArray("", medsNumFormat(__instance.DiscardCard), medsSpriteText("card"), stringBuilder2.ToString().Trim())));
                            stringBuilder1.Append("\n");
                        }
                        else if (__instance.DiscardCardPlace == Enums.CardPlace.Vanish)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsVanish"), (object)medsColorTextArray("", medsNumFormat(__instance.DiscardCard), medsSpriteText("card"), stringBuilder2.ToString().Trim())));
                            stringBuilder1.Append("\n");
                        }
                        stringBuilder2.Clear();
                    }
                    if (__instance.LookCards != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLook"), (object)medsColorTextArray("", medsNumFormat(__instance.LookCards), medsSpriteText("card"))));
                        stringBuilder1.Append("\n");
                        if (__instance.LookCardsDiscardUpTo == -1)
                        {
                            stringBuilder1.Append(Texts.Instance.GetText("cardsDiscardAny"));
                            stringBuilder1.Append("\n");
                        }
                        else if (__instance.LookCardsDiscardUpTo > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDiscardUpTo"), (object)medsColorTextArray("", medsNumFormat(__instance.LookCardsDiscardUpTo))));
                            stringBuilder1.Append("\n");
                        }
                        else if (__instance.LookCardsVanishUpTo > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsVanishUpTo"), (object)medsColorTextArray("", medsNumFormat(__instance.LookCardsVanishUpTo))));
                            stringBuilder1.Append("\n");
                        }
                    }

                    // Custom Text: After Add/Discard/Draw
                    BinbinCustomText(TextLocation.AfterAddDiscardDraw, ref stringBuilder1, __instance.Id);

                    num1 = 0;
                    if ((UnityEngine.Object)__instance.SummonUnit != (UnityEngine.Object)null && __instance.SummonUnitNum > 0)
                    {
                        stringBuilder2.Append("\n<color=#5E3016>");
                        if (__instance.SummonUnitNum > 1)
                        {
                            stringBuilder2.Append(__instance.SummonUnitNum);
                            stringBuilder2.Append(" ");
                        }
                        NPCData npc = Globals.Instance.GetNPC(__instance.SummonUnit?.Id);
                        if ((UnityEngine.Object)npc != (UnityEngine.Object)null)
                            stringBuilder2.Append(npc.NPCName);
                        if (__instance.Metamorph)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsMetamorph"), (object)stringBuilder2.ToString()));
                        else if (__instance.Evolve)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsEvolve"), (object)stringBuilder2.ToString()));
                        else
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSummon"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("</color>\n");
                        stringBuilder2.Clear();
                    }
                    if (__instance.DispelAuras > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurge"), (object)medsColorTextArray("aura", __instance.DispelAuras.ToString())));
                        stringBuilder1.Append("\n");
                    }
                    else if (__instance.DispelAuras == -1)
                    {
                        stringBuilder1.Append(Texts.Instance.GetText("cardsPurgeAll"));
                        stringBuilder1.Append("\n");
                    }
                    num1 = 0;
                    if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.None && __instance.SpecialValue1 == Enums.CardSpecialValue.None && __instance.SpecialValue2 == Enums.CardSpecialValue.None)
                    {
                        StringBuilder stringBuilder4 = new StringBuilder();
                        StringBuilder stringBuilder5 = new StringBuilder();
                        if ((UnityEngine.Object)__instance.HealAuraCurseName != (UnityEngine.Object)null)
                        {
                            if (__instance.HealAuraCurseName.IsAura)
                                stringBuilder4.Append(medsSpriteText(__instance.HealAuraCurseName.Id));
                            else
                                stringBuilder5.Append(medsSpriteText(__instance.HealAuraCurseName.Id));
                        }
                        if ((UnityEngine.Object)__instance.HealAuraCurseName2 != (UnityEngine.Object)null)
                        {
                            if (__instance.HealAuraCurseName2.IsAura)
                                stringBuilder4.Append(medsSpriteText(__instance.HealAuraCurseName2.Id));
                            else
                                stringBuilder5.Append(medsSpriteText(__instance.HealAuraCurseName2.Id));
                        }
                        if ((UnityEngine.Object)__instance.HealAuraCurseName3 != (UnityEngine.Object)null)
                        {
                            if (__instance.HealAuraCurseName3.IsAura)
                                stringBuilder4.Append(medsSpriteText(__instance.HealAuraCurseName3.Id));
                            else
                                stringBuilder5.Append(medsSpriteText(__instance.HealAuraCurseName3.Id));
                        }
                        if ((UnityEngine.Object)__instance.HealAuraCurseName4 != (UnityEngine.Object)null)
                        {
                            if (__instance.HealAuraCurseName4.IsAura)
                                stringBuilder4.Append(medsSpriteText(__instance.HealAuraCurseName4.Id));
                            else
                                stringBuilder5.Append(medsSpriteText(__instance.HealAuraCurseName4.Id));
                        }
                        if (stringBuilder4.Length > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurge"), (object)stringBuilder4.ToString()));
                            stringBuilder1.Append("\n");
                        }
                        if (stringBuilder5.Length > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispel"), (object)stringBuilder5.ToString()));
                            stringBuilder1.Append("\n");
                        }
                        if (__instance.HealCurses > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispel"), (object)medsColorTextArray("curse", __instance.HealCurses.ToString())));
                            stringBuilder1.Append("\n");
                        }
                        if (__instance.HealCurses == -1)
                        {
                            stringBuilder1.Append(Texts.Instance.GetText("cardsDispelAll"));
                            stringBuilder1.Append("\n");
                        }
                        if ((UnityEngine.Object)__instance.HealAuraCurseSelf != (UnityEngine.Object)null)
                        {
                            if (__instance.HealAuraCurseSelf.IsAura)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurgeYour"), (object)medsSpriteText(__instance.HealAuraCurseSelf.Id)));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispelYour"), (object)medsSpriteText(__instance.HealAuraCurseSelf.Id)));
                            stringBuilder1.Append("\n");
                        }
                    }
                    else
                    {
                        if (__instance.HealCurses > 0)
                        {
                            if (__instance.TargetSide == Enums.CardTargetSide.Enemy)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurge"), (object)__instance.HealCurses));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispel"), (object)__instance.HealCurses));
                            stringBuilder1.Append("\n");
                        }
                        if ((UnityEngine.Object)__instance.HealAuraCurseName4 != (UnityEngine.Object)null && (UnityEngine.Object)__instance.HealAuraCurseName3 == (UnityEngine.Object)null)
                        {
                            StringBuilder stringBuilder6 = new StringBuilder();
                            StringBuilder stringBuilder7 = new StringBuilder();
                            if (__instance.HealAuraCurseName4.IsAura)
                                stringBuilder6.Append(medsSpriteText(__instance.HealAuraCurseName4.Id));
                            else
                                stringBuilder7.Append(medsSpriteText(__instance.HealAuraCurseName4.Id));
                            if (stringBuilder6.Length > 0)
                            {
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurge"), (object)stringBuilder6.ToString()));
                                stringBuilder1.Append("\n");
                            }
                            if (stringBuilder7.Length > 0)
                            {
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispel"), (object)stringBuilder7.ToString()));
                                stringBuilder1.Append("\n");
                            }
                        }
                    }

                    BinbinCustomText(TextLocation.AfterDispelSummonBeforeTransfer, ref stringBuilder1, __instance.Id);

                    if (__instance.TransferCurses > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTransferCurse"), (object)__instance.TransferCurses.ToString()));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.StealAuras > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsStealAuras"), (object)__instance.StealAuras.ToString()));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.IncreaseAuras > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsIncreaseAura"), (object)__instance.IncreaseAuras.ToString()));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.IncreaseCurses > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsIncreaseCurse"), (object)__instance.IncreaseCurses.ToString()));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.ReduceAuras > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsReduceAura"), (object)__instance.ReduceAuras.ToString()));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.ReduceCurses > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsReduceCurse"), (object)__instance.ReduceCurses.ToString()));
                        stringBuilder1.Append("\n");
                    }

                    // Custom Text: After Transfer/Steal/Increase/Reduce
                    BinbinCustomText(TextLocation.AfterTransferIncreaseACBeforeDamage, ref stringBuilder1, __instance.Id);
                    // Custom Text: Before Damage
                    bool flag4 = false;
                    if (__instance.Damage > 0)
                    {
                        if (__instance.Damage > 0 && __instance.Damage2 > 0 && __instance.DamageType == __instance.DamageType2 && !__instance.DamageSpecialValueGlobal && !__instance.DamageSpecialValue1 && !__instance.DamageSpecialValue2 && !__instance.Damage2SpecialValueGlobal && !__instance.Damage2SpecialValue1 && !__instance.Damage2SpecialValue2)
                        {
                            stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(medsDamagePreCalculatedCombined), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                            flag4 = true;
                        }
                        else
                        {
                            if (__instance.Damage >= 1 && (__instance.DamageSpecialValueGlobal || __instance.DamageSpecialValue1 || __instance.DamageSpecialValue2))
                                stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                            else
                                stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamagePreCalculated), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                            if (__instance.Damage2 > 0)
                            {
                                flag4 = true;
                                if (__instance.Damage2 >= 1 && (__instance.Damage2SpecialValue1 || __instance.Damage2SpecialValue2 || __instance.Damage2SpecialValueGlobal))
                                {
                                    if (__instance.DamageType == __instance.DamageType2)
                                    {
                                        stringBuilder2.Append("<space=-.3>");
                                        stringBuilder2.Append("+");
                                        stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                                    }
                                    else
                                        stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                                }
                                else
                                    stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamagePreCalculated2), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                            }
                        }
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDealDamage"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (!flag4 && __instance.Damage2 > 0)
                    {
                        if (__instance.Damage2 >= 1 && (__instance.Damage2SpecialValueGlobal || __instance.Damage2SpecialValue1 || __instance.Damage2SpecialValue2))
                            stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                        else
                            stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamagePreCalculated2), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDealDamage"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    int num3 = 0;
                    if (__instance.DamageSelf > 0)
                    {
                        ++num3;
                        if (__instance.DamageSpecialValueGlobal || __instance.DamageSpecialValue1 || __instance.DamageSpecialValue2)
                            stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                        else
                            stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamageSelfPreCalculated), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                    }
                    if (__instance.DamageSelf2 > 0)
                    {
                        ++num3;
                        if (__instance.Damage2SpecialValueGlobal || __instance.Damage2SpecialValue1 || __instance.Damage2SpecialValue2)
                            stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                        else
                            stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamageSelfPreCalculated2), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                    }
                    if (num3 > 0)
                    {
                        if (num3 > 2)
                        {
                            stringBuilder2.Insert(0, str1);
                            stringBuilder2.Insert(0, "\n");
                        }
                        if (__instance.TargetSide == Enums.CardTargetSide.Self)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                        }
                        else
                        {
                            stringBuilder3.Append(string.Format(Texts.Instance.GetText("cardsYouSuffer"), (object)stringBuilder2.ToString()));
                            stringBuilder3.Append("\n");
                        }
                        stringBuilder2.Clear();
                    }

                    // Custom Text: After Damage Before Heal
                    BinbinCustomText(TextLocation.AfterDamageBeforeHeal, ref stringBuilder1, __instance.Id);

                    if (stringBuilder3.Length > 0)
                    {
                        stringBuilder1.Append(stringBuilder3.ToString());
                        stringBuilder3.Clear();
                    }
                    if ((double)__instance.HealSelfPerDamageDonePercent > 0.0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsHealSelfPerDamage"), (object)__instance.HealSelfPerDamageDonePercent.ToString()));
                        stringBuilder1.Append("\n");
                    }
                    if (__instance.SelfHealthLoss != 0 && !__instance.SelfHealthLossSpecialGlobal)
                    {
                        if (__instance.SelfHealthLossSpecialValue1)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLoseHp"), (object)medsColorTextArray("damage", "X HP")));
                        else
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLoseHp"), (object)medsColorTextArray("damage", medsNumFormat(__instance.SelfHealthLoss), "HP")));
                        stringBuilder1.Append("\n");
                    }
                    if ((__instance.TargetSide == Enums.CardTargetSide.Friend || __instance.TargetSide == Enums.CardTargetSide.FriendNotSelf) && __instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseYours && (UnityEngine.Object)__instance.SpecialAuraCurseNameGlobal != (UnityEngine.Object)null && (double)__instance.SpecialValueModifierGlobal > 0.0)
                    {
                        flag1 = true;
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsShareYour"), (object)medsSpriteText(__instance.SpecialAuraCurseNameGlobal.Id)));
                        stringBuilder1.Append("\n");
                    }
                    if (!__instance.Damage2SpecialValue1 && !flag1 && (__instance.SpecialValueGlobal != Enums.CardSpecialValue.None || __instance.SpecialValue1 != Enums.CardSpecialValue.None || __instance.SpecialValue2 != Enums.CardSpecialValue.None))
                    {
                        if (!__instance.DamageSpecialValueGlobal && !__instance.DamageSpecialValue1 && !__instance.DamageSpecialValue2)
                        {
                            if (__instance.TargetSide == Enums.CardTargetSide.Self && (__instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseTarget || __instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseYours))
                            {
                                if ((UnityEngine.Object)__instance.SpecialAuraCurseNameGlobal != (UnityEngine.Object)null)
                                    str5 = medsSpriteText(__instance.SpecialAuraCurseNameGlobal.Id);
                                if (__instance.AuraChargesSpecialValueGlobal)
                                {
                                    if ((UnityEngine.Object)__instance.Aura != (UnityEngine.Object)null)
                                        str6 = medsSpriteText(__instance.Aura.Id);
                                    if ((UnityEngine.Object)__instance.Aura2 != (UnityEngine.Object)null && __instance.AuraCharges2SpecialValueGlobal)
                                        str7 = medsSpriteText(__instance.Aura2.Id);
                                    if ((UnityEngine.Object)__instance.Aura3 != (UnityEngine.Object)null && __instance.AuraCharges3SpecialValue1)
                                        thirdACSprite = medsSpriteText(__instance.Aura3.Id);
                                }
                                else if (__instance.CurseChargesSpecialValueGlobal)
                                {
                                    if ((UnityEngine.Object)__instance.Curse != (UnityEngine.Object)null)
                                        str6 = medsSpriteText(__instance.Curse.Id);
                                    if ((UnityEngine.Object)__instance.Curse2 != (UnityEngine.Object)null && __instance.CurseCharges2SpecialValueGlobal)
                                        str7 = medsSpriteText(__instance.Curse2.Id);
                                    if ((UnityEngine.Object)__instance.Curse3 != (UnityEngine.Object)null && __instance.CurseCharges3SpecialValue1)
                                        thirdACSprite = medsSpriteText(__instance.Curse3.Id);
                                }
                            }
                            else if (__instance.SpecialValue1 == Enums.CardSpecialValue.AuraCurseTarget)
                            {
                                if ((UnityEngine.Object)__instance.SpecialAuraCurseName1 != (UnityEngine.Object)null)
                                    str5 = medsSpriteText(__instance.SpecialAuraCurseName1.Id);
                                if (__instance.AuraChargesSpecialValue1)
                                {
                                    if ((UnityEngine.Object)__instance.Aura != (UnityEngine.Object)null)
                                        str6 = medsSpriteText(__instance.Aura.Id);
                                    if ((UnityEngine.Object)__instance.Aura2 != (UnityEngine.Object)null && __instance.AuraCharges2SpecialValue1)
                                        str7 = medsSpriteText(__instance.Aura2.Id);
                                    if ((UnityEngine.Object)__instance.Curse3 != (UnityEngine.Object)null && __instance.CurseCharges3SpecialValue1)
                                        thirdACSprite = medsSpriteText(__instance.Curse3.Id);
                                }
                                else if (__instance.CurseChargesSpecialValue1)
                                {
                                    if ((UnityEngine.Object)__instance.Curse != (UnityEngine.Object)null)
                                        str6 = medsSpriteText(__instance.Curse.Id);
                                    if ((UnityEngine.Object)__instance.Curse2 != (UnityEngine.Object)null && __instance.CurseCharges2SpecialValue1)
                                        str7 = medsSpriteText(__instance.Curse2.Id);
                                    if ((UnityEngine.Object)__instance.Curse3 != (UnityEngine.Object)null && __instance.CurseCharges3SpecialValue1)
                                        thirdACSprite = medsSpriteText(__instance.Curse3.Id);
                                }
                            }
                            if (str5 != "" && str6 != "")
                            {
                                flag2 = true;
                                if (str5 == str6)
                                {
                                    if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseTarget)
                                    {
                                        if ((double)__instance.SpecialValueModifierGlobal == 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDoubleTarget"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifierGlobal == 200.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTripleTarget"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifierGlobal < 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLosePercentTarget"), (object)(float)(100.0 - (double)__instance.SpecialValueModifierGlobal), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                    }
                                    else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseYours)
                                    {
                                        if ((double)__instance.SpecialValueModifierGlobal == 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDoubleYour"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifierGlobal == 200.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTripleYour"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifierGlobal < 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLosePercentYour"), (object)(float)(100.0 - (double)__instance.SpecialValueModifierGlobal), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                    }
                                    else if (__instance.SpecialValue1 == Enums.CardSpecialValue.AuraCurseTarget)
                                    {
                                        if ((double)__instance.SpecialValueModifier1 == 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDoubleTarget"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifier1 == 200.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTripleTarget"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifier1 < 100.0 && (UnityEngine.Object)__instance.HealAuraCurseName != (UnityEngine.Object)null)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLosePercentTarget"), (object)(float)(100.0 - (double)__instance.SpecialValueModifier1), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else
                                            flag2 = false;
                                    }
                                    else if (__instance.SpecialValue1 == Enums.CardSpecialValue.AuraCurseYours)
                                    {
                                        if ((double)__instance.SpecialValueModifier1 == 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDoubleYour"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifier1 == 200.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTripleYour"), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                        else if ((double)__instance.SpecialValueModifier1 < 100.0)
                                        {
                                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLosePercentYour"), (object)(float)(100.0 - (double)__instance.SpecialValueModifier1), (object)str5));
                                            stringBuilder1.Append("\n");
                                        }
                                    }
                                }
                                else
                                {
                                    stringBuilder2.Append(str6);
                                    if ((double)__instance.SpecialValueModifier1 > 0.0)
                                        num2 = __instance.SpecialValueModifier1 / 100f;
                                    if ((double)num2 > 0.0 && (double)num2 != 1.0)
                                        str8 = "x " + num2.ToString();
                                    // BEGIN CHANGE - Multiplier for multiple Transforms

                                    // if (str7 != "")
                                    //     stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTransformIntoAnd"), (object)str5, (object)stringBuilder2.ToString(), (object)str7));
                                    // else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTransformInto"), (object)str5, (object)stringBuilder2.ToString()));
                                    stringBuilder1.Append(str7);
                                    stringBuilder1.Append(thirdACSprite);
                                    if (str8 != "")
                                    {
                                        stringBuilder1.Append(" <c>");
                                        stringBuilder1.Append(str8);
                                        stringBuilder1.Append("</c>");
                                    }
                                    // END CHANGE
                                    stringBuilder1.Append("\n");
                                    stringBuilder2.Clear();
                                    num2 = 0.0f;
                                    str8 = "";
                                }
                            }
                        }
                    }
                    if (__instance.EnergyRechargeSpecialValueGlobal)
                        stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText("energy")));

                    // Custom Text: Before Auras
                    BinbinCustomText(TextLocation.AfterHealBeforeAuras, ref stringBuilder1, __instance.Id);

                    AuraCurseData auraCurseData = (AuraCurseData)null;
                    if (!flag1 && !flag2)
                    {
                        int num4 = 0;
                        if ((UnityEngine.Object)__instance.Aura != (UnityEngine.Object)null && (__instance.AuraChargesSpecialValue1 || __instance.AuraChargesSpecialValueGlobal))
                        {
                            ++num4;
                            stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText(__instance.Aura.Id)));
                            auraCurseData = __instance.Aura;
                        }
                        if ((UnityEngine.Object)__instance.Aura2 != (UnityEngine.Object)null && (__instance.AuraCharges2SpecialValue1 || __instance.AuraCharges2SpecialValueGlobal))
                        {
                            ++num4;
                            if ((UnityEngine.Object)__instance.Aura != (UnityEngine.Object)null && (UnityEngine.Object)__instance.Aura == (UnityEngine.Object)__instance.Aura2)
                            {
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Aura.Id, __instance.AuraCharges, character)), medsSpriteText(__instance.Aura.Id)));
                                stringBuilder2.Append("+");
                            }
                            stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText(__instance.Aura2.Id)));
                            auraCurseData = __instance.Aura2;
                        }
                        if ((UnityEngine.Object)__instance.Aura3 != (UnityEngine.Object)null && (__instance.AuraCharges3SpecialValue1 || __instance.AuraCharges3SpecialValueGlobal))
                        {
                            ++num4;
                            if ((UnityEngine.Object)__instance.Aura != (UnityEngine.Object)null && (UnityEngine.Object)__instance.Aura == (UnityEngine.Object)__instance.Aura3)
                            {
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Aura.Id, __instance.AuraCharges, character)), medsSpriteText(__instance.Aura.Id)));
                                stringBuilder2.Append("+");
                            }
                            if ((UnityEngine.Object)__instance.Aura2 != (UnityEngine.Object)null && (UnityEngine.Object)__instance.Aura == (UnityEngine.Object)__instance.Aura3)
                            {
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Aura2.Id, __instance.AuraCharges2, character)), medsSpriteText(__instance.Aura3.Id)));
                                stringBuilder2.Append("+");
                            }
                            stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText(__instance.Aura3.Id)));
                            auraCurseData = __instance.Aura3;
                        }
                        if (num4 > 0)
                        {
                            if (__instance.TargetSide == Enums.CardTargetSide.Self)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGrant"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                            stringBuilder2.Clear();
                        }
                    }

                    // Custom Text: After Auras/BeforeCurses
                    BinbinCustomText(TextLocation.AfterAurasBeforeCurses, ref stringBuilder1, __instance.Id);

                    if (!flag1 && !flag2)
                    {
                        int num5 = 0;
                        if ((UnityEngine.Object)__instance.Curse != (UnityEngine.Object)null && (__instance.CurseChargesSpecialValue1 || __instance.CurseChargesSpecialValueGlobal))
                        {
                            ++num5;
                            stringBuilder2.Append(medsColorTextArray("curse", "X", medsSpriteText(__instance.Curse.Id)));
                        }
                        if ((UnityEngine.Object)__instance.Curse2 != (UnityEngine.Object)null && (__instance.CurseCharges2SpecialValue1 || __instance.CurseCharges2SpecialValueGlobal))
                        {
                            ++num5;
                            stringBuilder2.Append(medsColorTextArray("curse", "X", medsSpriteText(__instance.Curse2.Id)));
                        }
                        if ((UnityEngine.Object)__instance.Curse3 != (UnityEngine.Object)null && (__instance.CurseCharges3SpecialValue1 || __instance.CurseCharges3SpecialValueGlobal))
                        {
                            ++num5;
                            stringBuilder2.Append(medsColorTextArray("curse", "X", medsSpriteText(__instance.Curse3.Id)));
                        }
                        if (num5 > 0)
                        {
                            if (__instance.TargetSide == Enums.CardTargetSide.Self)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsApply"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                            stringBuilder2.Clear();
                        }
                        int num6 = 0;
                        if ((UnityEngine.Object)__instance.CurseSelf != (UnityEngine.Object)null && (__instance.CurseChargesSpecialValue1 || __instance.CurseChargesSpecialValueGlobal))
                        {
                            ++num6;
                            stringBuilder2.Append(medsColorTextArray("curse", "X", medsSpriteText(__instance.CurseSelf.Id)));
                        }
                        if ((UnityEngine.Object)__instance.CurseSelf2 != (UnityEngine.Object)null && (__instance.CurseCharges2SpecialValue1 || __instance.CurseCharges2SpecialValueGlobal))
                        {
                            ++num6;
                            stringBuilder2.Append(medsColorTextArray("curse", "X", medsSpriteText(__instance.CurseSelf2.Id)));
                        }
                        if ((UnityEngine.Object)__instance.CurseSelf3 != (UnityEngine.Object)null && (__instance.CurseCharges3SpecialValue1 || __instance.CurseCharges3SpecialValueGlobal))
                        {
                            ++num6;
                            stringBuilder2.Append(medsColorTextArray("curse", "X", medsSpriteText(__instance.CurseSelf3.Id)));
                        }
                        if (num6 > 0)
                        {
                            if (__instance.TargetSide == Enums.CardTargetSide.Self || (UnityEngine.Object)__instance.CurseSelf != (UnityEngine.Object)null || (UnityEngine.Object)__instance.CurseSelf2 != (UnityEngine.Object)null || (UnityEngine.Object)__instance.CurseSelf3 != (UnityEngine.Object)null)
                            {
                                if (__instance.TargetSide == Enums.CardTargetSide.Self)
                                {
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                                    stringBuilder1.Append("\n");
                                }
                                else if (!flag3)
                                {
                                    stringBuilder3.Append(string.Format(Texts.Instance.GetText("cardsYouSuffer"), (object)stringBuilder2.ToString()));
                                    stringBuilder3.Append("\n");
                                }
                                else
                                {
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsYouSuffer"), (object)stringBuilder2.ToString()));
                                    stringBuilder1.Append("\n");
                                }
                            }
                            else
                            {
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsApply"), (object)stringBuilder2.ToString()));
                                stringBuilder1.Append("\n");
                            }
                            stringBuilder2.Clear();
                        }
                    }
                    int num7 = 0;

                    // Custom Text: After Curse Before SpecialValue
                    BinbinCustomText(TextLocation.AfterCurseBeforeSpecialValue, ref stringBuilder1, __instance.Id);

                    if (__instance.Heal > 0 && (__instance.HealSpecialValue1 || __instance.HealSpecialValueGlobal))
                    {
                        stringBuilder2.Append(medsColorTextArray("heal", "X", medsSpriteText("heal")));
                        num1 = num7 + 1;
                    }
                    if (stringBuilder2.Length > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsHeal"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    int num8 = 0;
                    if (__instance.HealSelf > 0 && (__instance.HealSelfSpecialValue1 || __instance.HealSelfSpecialValueGlobal))
                    {
                        stringBuilder2.Append(medsColorTextArray("heal", "X", medsSpriteText("heal")));
                        num1 = num8 + 1;
                    }
                    if (stringBuilder2.Length > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsHealSelf"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }

                    if (!flag1 && !flag2)
                    {
                        if ((double)__instance.SpecialValueModifierGlobal > 0.0)
                            num2 = __instance.SpecialValueModifierGlobal / 100f;
                        else if ((double)__instance.SpecialValueModifier1 > 0.0)
                            num2 = __instance.SpecialValueModifier1 / 100f;
                        if ((double)num2 > 0.0 && (double)num2 != 1.0)
                            str8 = "x" + num2.ToString();
                        if (str8 == "")
                            str8 = "<space=-.1>";
                        if (__instance.SpecialValue1 == Enums.CardSpecialValue.AuraCurseYours)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYour"), (object)medsSpriteText(__instance.SpecialAuraCurseName1.Id), (object)str8));
                        else if (__instance.SpecialValue1 == Enums.CardSpecialValue.AuraCurseTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTarget"), (object)medsSpriteText(__instance.SpecialAuraCurseName1.Id), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseYours)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYour"), (object)medsSpriteText(__instance.SpecialAuraCurseNameGlobal.Id), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.AuraCurseTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTarget"), (object)medsSpriteText(__instance.SpecialAuraCurseNameGlobal.Id), (object)str8));
                        if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.HealthYours)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourHp"), (object)str8));
                        else if (__instance.SpecialValue1 == Enums.CardSpecialValue.HealthYours)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourHp"), (object)str8));
                        else if (__instance.SpecialValue1 == Enums.CardSpecialValue.HealthTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTargetHp"), (object)str8));
                        if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.SpeedYours)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourSpeed"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.SpeedTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTargetSpeed"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.SpeedDifference || __instance.SpecialValue1 == Enums.CardSpecialValue.SpeedDifference)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsDifferenceSpeed"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.DiscardedCards)
                        {
                            if (__instance.DiscardCardPlace == Enums.CardPlace.Vanish)
                                stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourVanishedCards"), (object)str8));
                            else
                                stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourDiscardedCards"), (object)str8));
                        }
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.VanishedCards)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourVanishedCards"), (object)str8));
                        if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsHand || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsHand || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsHand)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourHand"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsDeck || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsDeck || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsDeck)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourDeck"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsDiscard || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsDiscard || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsDiscard)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourDiscard"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsVanish || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsVanish || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsVanish)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourVanish"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsDeckTarget || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsDeckTarget || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsDeckTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTargetDeck"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsDiscardTarget || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsDiscardTarget || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsDiscardTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTargetDiscard"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.CardsVanishTarget || __instance.SpecialValue1 == Enums.CardSpecialValue.CardsVanishTarget || __instance.SpecialValue2 == Enums.CardSpecialValue.CardsVanishTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTargetVanish"), (object)medsSpriteText("card"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.MissingHealthYours || __instance.SpecialValue1 == Enums.CardSpecialValue.MissingHealthYours)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsYourMissingHealth"), (object)str8));
                        else if (__instance.SpecialValueGlobal == Enums.CardSpecialValue.MissingHealthTarget || __instance.SpecialValue1 == Enums.CardSpecialValue.MissingHealthTarget)
                            stringBuilder2.Append(string.Format(Texts.Instance.GetText("cardsXEqualsTargetMissingHealth"), (object)str8));
                        if (stringBuilder2.Length > 0)
                        {
                            stringBuilder1.Append(str4);
                            stringBuilder1.Append(stringBuilder2);
                            stringBuilder1.Append(str3);
                            stringBuilder1.Append("\n");
                            stringBuilder2.Clear();
                            if ((UnityEngine.Object)__instance.HealAuraCurseName != (UnityEngine.Object)null)
                            {
                                if (__instance.TargetSide == Enums.CardTargetSide.Self)
                                {
                                    if (__instance.HealAuraCurseName.IsAura)
                                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurgeYour"), (object)medsSpriteText(__instance.HealAuraCurseName.Id)));
                                    else
                                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispelYour"), (object)medsSpriteText(__instance.HealAuraCurseName.Id)));
                                }
                                else if (__instance.HealAuraCurseName.IsAura)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurge"), (object)medsSpriteText(__instance.HealAuraCurseName.Id)));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispel"), (object)medsSpriteText(__instance.HealAuraCurseName.Id)));
                                stringBuilder1.Append("\n");
                            }
                            if ((UnityEngine.Object)__instance.HealAuraCurseSelf != (UnityEngine.Object)null)
                            {
                                if (__instance.HealAuraCurseSelf.IsAura)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsPurgeYour"), (object)medsSpriteText(__instance.HealAuraCurseSelf.Id)));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispelYour"), (object)medsSpriteText(__instance.HealAuraCurseSelf.Id)));
                                stringBuilder1.Append("\n");
                            }
                        }
                        num2 = 0.0f;
                    }
                    if (__instance.SelfHealthLoss > 0 && __instance.SelfHealthLossSpecialGlobal)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsYouLose"), (object)medsColorTextArray("damage", "X", "HP")));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    int num9 = 0;
                    if ((UnityEngine.Object)__instance.CurseSelf != (UnityEngine.Object)null && __instance.CurseCharges > 0)
                    {
                        ++num9;
                        stringBuilder2.Append(medsColorTextArray("curse", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.CurseSelf.Id, __instance.CurseCharges, character)), medsSpriteText(__instance.CurseSelf.Id)));
                    }
                    if ((UnityEngine.Object)__instance.CurseSelf2 != (UnityEngine.Object)null && __instance.CurseCharges2 > 0)
                    {
                        ++num9;
                        stringBuilder2.Append(medsColorTextArray("curse", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.CurseSelf2.Id, __instance.CurseCharges2, character)), medsSpriteText(__instance.CurseSelf2.Id)));
                    }
                    if ((UnityEngine.Object)__instance.CurseSelf3 != (UnityEngine.Object)null && __instance.CurseCharges3 > 0)
                    {
                        ++num9;
                        stringBuilder2.Append(medsColorTextArray("curse", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.CurseSelf3.Id, __instance.CurseCharges3, character)), medsSpriteText(__instance.CurseSelf3.Id)));
                    }
                    if (num9 > 0)
                    {
                        if (num9 > 2)
                        {
                            stringBuilder2.Insert(0, str1);
                            stringBuilder2.Insert(0, "\n");
                        }
                        if (__instance.TargetSide == Enums.CardTargetSide.Self)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                        }
                        else
                        {
                            stringBuilder3.Append(string.Format(Texts.Instance.GetText("cardsYouSuffer"), (object)stringBuilder2.ToString()));
                            stringBuilder3.Append("\n");
                        }
                        stringBuilder2.Clear();
                    }
                    int num10 = 0;
                    if (__instance.EnergyRecharge < 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsLoseHp"), (object)stringBuilder2.Append(medsColorTextArray("system", medsNumFormat(Mathf.Abs(__instance.EnergyRecharge)), medsSpriteText("energy")))));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }

                    if (__instance.EnergyRecharge > 0)
                    {
                        ++num10;
                        stringBuilder2.Append(medsColorTextArray("system", medsNumFormat(__instance.EnergyRecharge), medsSpriteText("energy")));
                    }

                    // // CHANGE: Grant Energy replaced with Gain energy if targets enemies
                    // if (__instance.EnergyRecharge > 0 && __instance.TargetSide == Enums.CardTargetSide.Enemy)
                    // {
                    //     // LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");
                    //     // string energySprite = medsSpriteText("energy");
                    //     LogDebug("Binbin Testing Text: Gain Energy");
                    //     LogDebug($"Binbin Testing Text Pre sb2: {stringBuilder2}");
                    //     LogDebug($"Binbin Testing Text Pre sb1: {stringBuilder1}");
                    //     stringBuilder1.Replace($"Grant", $"Gain");
                    //     LogDebug($"Binbin Testing TextPost: {stringBuilder1}");
                    // }
                    // // END CHANGE


                    if ((UnityEngine.Object)__instance.Aura != (UnityEngine.Object)null && __instance.AuraCharges > 0 && (UnityEngine.Object)__instance.Aura != (UnityEngine.Object)auraCurseData)
                    {
                        ++num10;
                        stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Aura.Id, __instance.AuraCharges, character)), medsSpriteText(__instance.Aura.Id)));
                    }
                    if ((UnityEngine.Object)__instance.Aura2 != (UnityEngine.Object)null && __instance.AuraCharges2 > 0 && (UnityEngine.Object)__instance.Aura2 != (UnityEngine.Object)auraCurseData)
                    {
                        ++num10;
                        stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Aura2.Id, __instance.AuraCharges2, character)), medsSpriteText(__instance.Aura2.Id)));
                    }
                    if ((UnityEngine.Object)__instance.Aura3 != (UnityEngine.Object)null && __instance.AuraCharges3 > 0 && (UnityEngine.Object)__instance.Aura3 != (UnityEngine.Object)auraCurseData)
                    {
                        ++num10;
                        stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Aura3.Id, __instance.AuraCharges3, character)), medsSpriteText(__instance.Aura3.Id)));
                    }
                    if (num10 > 0)
                    {
                        if (num10 > 2)
                        {
                            stringBuilder2.Insert(0, str1);
                            stringBuilder2.Insert(0, "\n");
                        }
                        if (__instance.TargetSide == Enums.CardTargetSide.Self)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                        else
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGrant"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    int num11 = 0;
                    if ((UnityEngine.Object)__instance.AuraSelf != (UnityEngine.Object)null && __instance.AuraCharges > 0)
                    {
                        ++num11;
                        stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.AuraSelf.Id, __instance.AuraCharges, character)), medsSpriteText(__instance.AuraSelf.Id)));
                    }
                    if ((UnityEngine.Object)__instance.AuraSelf2 != (UnityEngine.Object)null && __instance.AuraCharges2 > 0)
                    {
                        ++num11;
                        stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.AuraSelf2.Id, __instance.AuraCharges2, character)), medsSpriteText(__instance.AuraSelf2.Id)));
                    }
                    if ((UnityEngine.Object)__instance.AuraSelf3 != (UnityEngine.Object)null && __instance.AuraCharges3 > 0)
                    {
                        ++num11;
                        stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.AuraSelf3.Id, __instance.AuraCharges3, character)), medsSpriteText(__instance.AuraSelf3.Id)));
                    }
                    if (!flag1)
                    {
                        if ((UnityEngine.Object)__instance.AuraSelf != (UnityEngine.Object)null && (__instance.AuraChargesSpecialValue1 || __instance.AuraChargesSpecialValueGlobal))
                        {
                            ++num11;
                            stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText(__instance.AuraSelf.Id)));
                        }
                        if ((UnityEngine.Object)__instance.AuraSelf2 != (UnityEngine.Object)null && (__instance.AuraCharges2SpecialValue1 || __instance.AuraCharges2SpecialValueGlobal))
                        {
                            ++num11;
                            stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText(__instance.AuraSelf2.Id)));
                        }
                        if ((UnityEngine.Object)__instance.AuraSelf3 != (UnityEngine.Object)null && (__instance.AuraCharges3SpecialValue1 || __instance.AuraCharges3SpecialValueGlobal))
                        {
                            ++num11;
                            stringBuilder2.Append(medsColorTextArray("aura", "X", medsSpriteText(__instance.AuraSelf3.Id)));
                        }
                    }
                    if (num11 > 0)
                    {
                        if (num11 > 2)
                        {
                            stringBuilder2.Insert(0, str1);
                            stringBuilder2.Insert(0, "\n");
                        }
                        if (__instance.TargetSide == Enums.CardTargetSide.Self)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                        }
                        else
                        {
                            stringBuilder3.Append(string.Format(Texts.Instance.GetText("cardsYouGain"), (object)stringBuilder2.ToString()));
                            stringBuilder3.Append("\n");
                        }
                        stringBuilder2.Clear();
                    }
                    int num12 = 0;
                    if (__instance.CurseCharges > 0 && (UnityEngine.Object)__instance.Curse != (UnityEngine.Object)null)
                    {
                        ++num12;
                        stringBuilder2.Append(medsColorTextArray("curse", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Curse.Id, __instance.CurseCharges, character)), medsSpriteText(__instance.Curse.Id)));
                    }
                    if (__instance.CurseCharges2 > 0 && (UnityEngine.Object)__instance.Curse2 != (UnityEngine.Object)null)
                    {
                        ++num12;
                        stringBuilder2.Append(medsColorTextArray("curse", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Curse2.Id, __instance.CurseCharges2, character)), medsSpriteText(__instance.Curse2.Id)));
                    }
                    if (__instance.CurseCharges3 > 0 && (UnityEngine.Object)__instance.Curse3 != (UnityEngine.Object)null)
                    {
                        ++num12;
                        stringBuilder2.Append(medsColorTextArray("curse", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, __instance.Curse3.Id, __instance.CurseCharges3, character)), medsSpriteText(__instance.Curse3.Id)));
                    }
                    if (num12 > 0)
                    {
                        if (num12 > 2)
                        {
                            stringBuilder2.Insert(0, str1);
                            stringBuilder2.Insert(0, "\n");
                        }
                        if (__instance.TargetSide == Enums.CardTargetSide.Self)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                        else
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsApply"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (__instance.Heal > 0 && !__instance.HealSpecialValue1 && !__instance.HealSpecialValueGlobal)
                    {
                        stringBuilder2.Append(medsColorTextArray("heal", medsNumFormat(medsHealPreCalculated), medsSpriteText("heal")));
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsHeal"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    // CHANGE: ADDED HEALSIDES
                    if (__instance.HealSides > 0)
                    {
                        // __instance.HealSpecialValueGlobal;
                        // LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");
                        LogDebug($"Binbin Testing Text: HealSides for {__instance.Id}");
                        string healSprite = medsSpriteText("heal");
                        string healAmount = medsColorTextArray("heal", medsNumFormat(__instance.HealSides));
                        string toAdd = $"Heal sides {healAmount} {healSprite} \n";
                        LogDebug($"Binbin Testing Text: {toAdd}");
                        stringBuilder1.Append(toAdd);
                    }

                    // END CHANGE
                    if (__instance.HealSelf > 0 && !__instance.HealSelfSpecialValue1 && !__instance.HealSelfSpecialValueGlobal)
                    {
                        stringBuilder2.Append(medsColorTextArray("heal", medsNumFormat(medsHealSelfPreCalculated), medsSpriteText("heal")));
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsHealSelf"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (__instance.DamageSides > 0)
                    {
                        if (__instance.DamageSpecialValueGlobal)
                            stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                        else
                            stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamageSidesPreCalculated), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType))));
                    }
                    if (__instance.DamageSides2 > 0)
                    {
                        if (__instance.Damage2SpecialValueGlobal)
                            stringBuilder2.Append(medsColorTextArray("damage", "X", medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                        else
                            stringBuilder2.Append(medsColorTextArray("damage", medsNumFormat(__instance.DamageSidesPreCalculated2), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType2))));
                    }
                    if (stringBuilder2.Length > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTargetSides"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (stringBuilder3.Length > 0)
                        stringBuilder1.Append(stringBuilder3.ToString());

                    if (__instance.KillPet)
                    {
                        stringBuilder1.Append(Texts.Instance.GetText("killPet"));
                        stringBuilder1.Append("\n");
                    }

                    if (__instance.DamageEnergyBonus > 0 || __instance.HealEnergyBonus > 0 || (UnityEngine.Object)__instance.AcEnergyBonus != (UnityEngine.Object)null)
                    {
                        StringBuilder stringBuilder8 = new StringBuilder();
                        stringBuilder8.Append("<line-height=30%><br></line-height>");
                        StringBuilder stringBuilder9 = new StringBuilder();
                        stringBuilder9.Append(str2);
                        stringBuilder9.Append("[");
                        stringBuilder9.Append(Texts.Instance.GetText("overchargeAcronym"));
                        stringBuilder9.Append("]");
                        stringBuilder9.Append(str3);
                        stringBuilder9.Append("  ");
                        if (__instance.DamageEnergyBonus > 0)
                        {
                            stringBuilder8.Append(stringBuilder9.ToString());
                            stringBuilder8.Append(string.Format(Texts.Instance.GetText("cardsDealDamage"), (object)medsColorTextArray("damage", medsNumFormat(__instance.DamageEnergyBonus), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)__instance.DamageType)))));
                            stringBuilder8.Append("\n");
                        }
                        if ((UnityEngine.Object)__instance.AcEnergyBonus != (UnityEngine.Object)null)
                        {
                            stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(__instance.AcEnergyBonusQuantity), medsSpriteText(__instance.AcEnergyBonus.Id)));
                            if ((UnityEngine.Object)__instance.AcEnergyBonus2 != (UnityEngine.Object)null)
                            {
                                stringBuilder2.Append(" ");
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(__instance.AcEnergyBonus2Quantity), medsSpriteText(__instance.AcEnergyBonus2.Id)));
                            }
                            if (__instance.AcEnergyBonus.IsAura)
                            {
                                if (__instance.TargetSide == Enums.CardTargetSide.Self)
                                {
                                    stringBuilder8.Append(stringBuilder9.ToString());
                                    stringBuilder8.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                                }
                                else
                                {
                                    stringBuilder8.Append(stringBuilder9.ToString());
                                    stringBuilder8.Append(string.Format(Texts.Instance.GetText("cardsGrant"), (object)stringBuilder2.ToString()));
                                }
                            }
                            else if (!__instance.AcEnergyBonus.IsAura)
                            {
                                if (__instance.TargetSide == Enums.CardTargetSide.Self)
                                {
                                    stringBuilder8.Append(stringBuilder9.ToString());
                                    stringBuilder8.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                                }
                                else
                                {
                                    stringBuilder8.Append(stringBuilder9.ToString());
                                    stringBuilder8.Append(string.Format(Texts.Instance.GetText("cardsApply"), (object)stringBuilder2.ToString()));
                                }
                            }
                            stringBuilder8.Append("\n");
                        }
                        if (__instance.HealEnergyBonus > 0)
                        {
                            stringBuilder8.Append(stringBuilder9.ToString());
                            stringBuilder8.Append(string.Format(Texts.Instance.GetText("cardsHeal"), (object)medsColorTextArray("heal", medsNumFormat(__instance.HealEnergyBonus), medsSpriteText("heal"))));
                            stringBuilder8.Append("\n");
                        }
                        stringBuilder1.Append(stringBuilder8.ToString());
                        stringBuilder2.Clear();
                        stringBuilder8.Clear();
                    }

                    // Custom Text: After SpecialValue Before Repeat
                    BinbinCustomText(TextLocation.AfterSpecialBeforeRepeat, ref stringBuilder1, __instance.Id);

                    if (__instance.EffectRepeat > 1 || __instance.EffectRepeatMaxBonus > 0)
                    {
                        stringBuilder1.Append(str1);
                        stringBuilder1.Append("<nobr><size=-.05><color=#1A505A>- ");
                        if (__instance.EffectRepeatMaxBonus > 0)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsRepeatUpTo"), (object)__instance.EffectRepeatMaxBonus));
                        else if (__instance.EffectRepeatTarget == Enums.EffectRepeatTarget.Chain)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsRepeatChain"), (object)(__instance.EffectRepeat - 1)));
                        else if (__instance.EffectRepeatTarget == Enums.EffectRepeatTarget.NoRepeat)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsRepeatJump"), (object)(__instance.EffectRepeat - 1)));
                        else
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsRepeat"), (object)(__instance.EffectRepeat - 1)));
                        if (__instance.EffectRepeatModificator != 0 && __instance.EffectRepeatTarget != Enums.EffectRepeatTarget.Chain)
                        {
                            stringBuilder1.Append(" (");
                            if (__instance.EffectRepeatModificator > 0)
                                stringBuilder1.Append("+");
                            stringBuilder1.Append(__instance.EffectRepeatModificator);
                            if (Functions.SpaceBeforePercentSign())
                                stringBuilder1.Append(" ");
                            stringBuilder1.Append("%)");
                        }
                        stringBuilder1.Append(" -</color></size></nobr>");
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (__instance.IgnoreBlock || __instance.IgnoreBlock2)
                    {
                        stringBuilder1.Append(str1);
                        stringBuilder1.Append(str2);
                        stringBuilder1.Append(Texts.Instance.GetText("cardsIgnoreBlock"));
                        stringBuilder1.Append(str3);
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (__instance.GoldGainQuantity != 0 && __instance.ShardsGainQuantity != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("customGainPerHeroAnd"), (object)medsColorTextArray("aura", __instance.GoldGainQuantity.ToString(), medsSpriteText("gold")), (object)medsColorTextArray("aura", __instance.ShardsGainQuantity.ToString(), medsSpriteText("dust"))));
                        stringBuilder1.Append("\n");
                    }
                    else if (__instance.GoldGainQuantity != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("customGainPerHero"), (object)medsColorTextArray("aura", __instance.GoldGainQuantity.ToString(), medsSpriteText("gold"))));
                        stringBuilder1.Append("\n");
                    }
                    else if (__instance.ShardsGainQuantity != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("customGainPerHero"), (object)medsColorTextArray("aura", __instance.ShardsGainQuantity.ToString(), medsSpriteText("dust"))));
                        stringBuilder1.Append("\n");
                    }
                    if ((double)__instance.SelfKillHiddenSeconds > 0.0)
                    {
                        stringBuilder1.Append(Texts.Instance.GetText("escapes"));
                        stringBuilder1.Append("\n");
                    }
                }
                else
                {

                    // Custom Text: Item Beginning
                    BinbinCustomText(TextLocation.ItemBeginning, ref stringBuilder1, __instance.Id);

                    ItemData itemData = !((UnityEngine.Object)__instance.Item != (UnityEngine.Object)null) ? __instance.ItemEnchantment : __instance.Item;
                    if (itemData.MaxHealth != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemMaxHp"), (object)medsNumFormatItem(itemData.MaxHealth, true)));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.ResistModified1 == Enums.DamageType.All)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemAllResistances"), (object)medsNumFormatItem(itemData.ResistModifiedValue1, true, true)));
                        stringBuilder1.Append("\n");
                    }
                    int num13 = 0;
                    int num14 = 0;
                    if (itemData.ResistModified1 != Enums.DamageType.None && itemData.ResistModified1 != Enums.DamageType.All)
                    {
                        stringBuilder2.Append(medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.ResistModified1)));
                        num14 = itemData.ResistModifiedValue1;
                        ++num13;
                    }
                    if (itemData.ResistModified2 != Enums.DamageType.None && itemData.ResistModified2 != Enums.DamageType.All)
                    {
                        stringBuilder2.Append(medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.ResistModified2)));
                        if (num14 == 0)
                            num14 = itemData.ResistModifiedValue2;
                        ++num13;
                    }
                    if (itemData.ResistModified3 != Enums.DamageType.None && itemData.ResistModified3 != Enums.DamageType.All)
                    {
                        stringBuilder2.Append(medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.ResistModified3)));
                        if (num14 == 0)
                            num14 = itemData.ResistModifiedValue3;
                        ++num13;
                    }
                    if (num13 > 0)
                    {
                        if (num13 > 1)
                            stringBuilder2.Append("\n");
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemXResistances"), (object)stringBuilder2.ToString(), (object)medsNumFormatItem(num14, true, true)));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }

                    // Custom Text: Item After HPResists Before Stats
                    BinbinCustomText(TextLocation.ItemAfterHPResistsBeforeStats, ref stringBuilder1, __instance.Id);

                    if (itemData.CharacterStatModified == Enums.CharacterStat.Speed)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemSpeed"), (object)medsNumFormatItem(itemData.CharacterStatModifiedValue, true)));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.CharacterStatModified == Enums.CharacterStat.EnergyTurn)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemEnergyRegeneration"), (object)medsSpriteText("energy"), (object)medsNumFormatItem(itemData.CharacterStatModifiedValue, true)));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.CharacterStatModified2 == Enums.CharacterStat.Speed)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemSpeed"), (object)medsNumFormatItem(itemData.CharacterStatModifiedValue2, true)));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.CharacterStatModified2 == Enums.CharacterStat.EnergyTurn)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemEnergyRegeneration"), (object)medsSpriteText("energy"), (object)medsNumFormatItem(itemData.CharacterStatModifiedValue2, true)));
                        stringBuilder1.Append("\n");
                    }

                    // Custom Text: Item After Stats Before DamageBonus
                    BinbinCustomText(TextLocation.ItemAfterStatsBeforeDamageBonus, ref stringBuilder1, __instance.Id);
                    if (itemData.DamageFlatBonus == Enums.DamageType.All)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemAllDamages"), (object)medsNumFormatItem(itemData.DamageFlatBonusValue, true)));
                        stringBuilder1.Append("\n");
                    }
                    int num15 = 0;
                    int num16 = 0;
                    if (itemData.DamageFlatBonus != Enums.DamageType.None && itemData.DamageFlatBonus != Enums.DamageType.All)
                    {
                        stringBuilder2.Append(medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.DamageFlatBonus)));
                        num16 = itemData.DamageFlatBonusValue;
                        ++num15;
                    }
                    if (itemData.DamageFlatBonus2 != Enums.DamageType.None && itemData.DamageFlatBonus2 != Enums.DamageType.All)
                    {
                        stringBuilder2.Append(medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.DamageFlatBonus2)));
                        ++num15;
                    }
                    if (itemData.DamageFlatBonus3 != Enums.DamageType.None && itemData.DamageFlatBonus3 != Enums.DamageType.All)
                    {
                        stringBuilder2.Append(medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.DamageFlatBonus3)));
                        ++num15;
                    }

                    // CHANGE: Added Support for DamagePercentBonus2 and 3. Added support for other damageTypes.
                    if (itemData.DamagePercentBonus == Enums.DamageType.All)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemAllDamages"), (object)medsNumFormatItem(Functions.FuncRoundToInt(itemData.DamagePercentBonusValue), true, true)));
                        stringBuilder1.Append("\n");
                    }
                    else if (itemData.DamagePercentBonus != Enums.DamageType.None && itemData.DamagePercentBonusValue != 0)
                    {
                        string dt = itemData.DamagePercentBonus.ToString().ToLower();
                        string textToAdd = "<space=.3><size=+.1><sprite name=" + dt + "></size> damage  {0}";
                        textToAdd = string.Format(textToAdd, (object)medsNumFormatItem(Functions.FuncRoundToInt(itemData.DamagePercentBonusValue), true, true));
                        stringBuilder1.Append(textToAdd);
                        stringBuilder1.Append("\n");
                    }

                    if (itemData.DamagePercentBonus2 == Enums.DamageType.All)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemAllDamages"), (object)medsNumFormatItem(Functions.FuncRoundToInt(itemData.DamagePercentBonusValue2), true, true)));
                        stringBuilder1.Append("\n");
                    }
                    else if (itemData.DamagePercentBonus2 != Enums.DamageType.None && itemData.DamagePercentBonusValue2 != 0)
                    {
                        string dt = itemData.DamagePercentBonus2.ToString().ToLower();
                        string textToAdd = "<space=.3><size=+.1><sprite name=" + dt + "></size> damage  {0}";
                        textToAdd = string.Format(textToAdd, (object)medsNumFormatItem(Functions.FuncRoundToInt(itemData.DamagePercentBonusValue2), true, true));
                        stringBuilder1.Append(textToAdd);
                        stringBuilder1.Append("\n");
                    }

                    if (itemData.DamagePercentBonus3 == Enums.DamageType.All)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemAllDamages"), (object)medsNumFormatItem(Functions.FuncRoundToInt(itemData.DamagePercentBonusValue3), true, true)));
                        stringBuilder1.Append("\n");
                    }
                    else if (itemData.DamagePercentBonus3 != Enums.DamageType.None && itemData.DamagePercentBonusValue3 != 0)
                    {
                        string dt = itemData.DamagePercentBonus3.ToString().ToLower();
                        string textToAdd = "<space=.3><size=+.1><sprite name=" + dt + "></size> damage  {0}";
                        textToAdd = string.Format(textToAdd, (object)medsNumFormatItem(Functions.FuncRoundToInt(itemData.DamagePercentBonusValue3), true, true));
                        stringBuilder1.Append(textToAdd);
                        stringBuilder1.Append("\n");
                    }
                    // END CHANGE


                    if (num15 > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemXDamages"), (object)stringBuilder2.ToString(), (object)medsNumFormatItem(num16, true)));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    // Custom Text: After DamageBonus before heal                    
                    BinbinCustomText(TextLocation.ItemAfterDamageBonusBeforeHeal, ref stringBuilder1, __instance.Id);
                    if (itemData.UseTheNextInsteadWhenYouPlay && (double)itemData.HealPercentBonus != 0.0)
                    {
                        string str11 = "";
                        if (itemData.DestroyAfterUses > 1)
                            str11 = "(" + itemData.DestroyAfterUses.ToString() + ") ";
                        StringBuilder stringBuilder10 = new StringBuilder();
                        stringBuilder10.Append("<size=-.15><color=#444>[");
                        stringBuilder10.Append(Texts.Instance.GetText("itemTheNext"));
                        stringBuilder10.Append("]</color></size><br>");
                        stringBuilder2.Append("<color=#5E3016>");
                        stringBuilder2.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)itemData.CastedCardType)));
                        stringBuilder2.Append("</color>");
                        stringBuilder1.Append(string.Format(stringBuilder10.ToString(), (object)str11, (object)stringBuilder2.ToString()));
                        stringBuilder2.Clear();
                        stringBuilder10.Clear();
                    }
                    if (itemData.HealFlatBonus != 0)
                    {
                        stringBuilder1.Append(medsSpriteText("heal"));
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(Functions.LowercaseFirst(Texts.Instance.GetText("healDone")));
                        stringBuilder1.Append(medsNumFormatItem(itemData.HealFlatBonus, true));
                        stringBuilder1.Append("\n");
                    }
                    if ((double)itemData.HealPercentBonus != 0.0)
                    {
                        stringBuilder1.Append(medsSpriteText("heal"));
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(Functions.LowercaseFirst(Texts.Instance.GetText("healDone")));
                        stringBuilder1.Append(medsNumFormatItem(Functions.FuncRoundToInt(itemData.HealPercentBonus), true, true));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.HealReceivedFlatBonus != 0)
                    {
                        stringBuilder1.Append(medsSpriteText("heal"));
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(Functions.LowercaseFirst(Texts.Instance.GetText("healTaken")));
                        stringBuilder1.Append(medsNumFormatItem(Functions.FuncRoundToInt((float)itemData.HealReceivedFlatBonus), true));
                        stringBuilder1.Append("\n");
                    }
                    if ((double)itemData.HealReceivedPercentBonus != 0.0)
                    {
                        stringBuilder1.Append(medsSpriteText("heal"));
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(Functions.LowercaseFirst(Texts.Instance.GetText("healTaken")));
                        stringBuilder1.Append(medsNumFormatItem(Functions.FuncRoundToInt(itemData.HealReceivedPercentBonus), true, true));
                        stringBuilder1.Append("\n");
                    }
                    // Custom Text: Item After HealBonus before ACBonus 
                    BinbinCustomText(TextLocation.ItemAfterHealBonusBeforeACBonus, ref stringBuilder1, __instance.Id);
                    if ((UnityEngine.Object)itemData.AuracurseBonus1 != (UnityEngine.Object)null && itemData.AuracurseBonusValue1 > 0 && (UnityEngine.Object)itemData.AuracurseBonus2 != (UnityEngine.Object)null && itemData.AuracurseBonusValue2 > 0 && itemData.AuracurseBonusValue1 == itemData.AuracurseBonusValue2)
                    {
                        stringBuilder2.Append(medsSpriteText(itemData.AuracurseBonus1.Id));
                        stringBuilder2.Append(medsSpriteText(itemData.AuracurseBonus2.Id));
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemCharges"), (object)stringBuilder2.ToString(), (object)medsNumFormatItem(itemData.AuracurseBonusValue1, true)));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    else
                    {
                        if ((UnityEngine.Object)itemData.AuracurseBonus1 != (UnityEngine.Object)null && itemData.AuracurseBonusValue1 > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemCharges"), (object)medsSpriteText(itemData.AuracurseBonus1.Id), (object)medsNumFormatItem(itemData.AuracurseBonusValue1, true)));
                            stringBuilder1.Append("\n");
                        }
                        if ((UnityEngine.Object)itemData.AuracurseBonus2 != (UnityEngine.Object)null && itemData.AuracurseBonusValue2 > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemCharges"), (object)medsSpriteText(itemData.AuracurseBonus2.Id), (object)medsNumFormatItem(itemData.AuracurseBonusValue2, true)));
                            stringBuilder1.Append("\n");
                        }
                    }
                    // Custom Text: Item After ACBonus before MiscText
                    BinbinCustomText(TextLocation.ItemAfterACBonusBeforeMiscText, ref stringBuilder1, __instance.Id);
                    int num17 = 0;
                    if ((UnityEngine.Object)itemData.AuracurseImmune1 != (UnityEngine.Object)null)
                    {
                        ++num17;
                        stringBuilder2.Append(medsColorTextArray("curse", medsSpriteText(itemData.AuracurseImmune1.Id)));
                    }
                    if ((UnityEngine.Object)itemData.AuracurseImmune2 != (UnityEngine.Object)null)
                    {
                        ++num17;
                        stringBuilder2.Append(medsColorTextArray("curse", medsSpriteText(itemData.AuracurseImmune2.Id)));
                    }
                    if (num17 > 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsImmuneTo"), (object)stringBuilder2.ToString()));
                        stringBuilder1.Append("\n");
                        stringBuilder2.Clear();
                    }
                    if (itemData.PercentDiscountShop != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemDiscount"), (object)medsNumFormatItem(itemData.PercentDiscountShop, true, true)));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.PercentRetentionEndGame != 0)
                    {
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemDieRetain"), (object)medsNumFormatItem(itemData.PercentRetentionEndGame, true, true)));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.AuracurseCustomString != "" && (UnityEngine.Object)itemData.AuracurseCustomAC != (UnityEngine.Object)null)
                    {
                        StringBuilder stringBuilder11 = new StringBuilder();
                        if ((itemData.AuracurseCustomString == "itemCustomTextMaxChargesIncrasedOnEnemies" || itemData.AuracurseCustomString == "itemCustomTextMaxChargesIncrasedOnHeroes") && itemData.AuracurseCustomModValue1 > 0)
                            stringBuilder11.Append("+");
                        stringBuilder11.Append(itemData.AuracurseCustomModValue1);
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText(itemData.AuracurseCustomString), (object)medsColorTextArray("aura", medsSpriteText(itemData.AuracurseCustomAC.Id)), (object)stringBuilder11.ToString(), (object)itemData.AuracurseCustomModValue2));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.Id == "harleyrare" || itemData.Id == "templelurkerpetrare" || itemData.Id == "mentalscavengerspetrare")
                    {
                        stringBuilder1.Append(Texts.Instance.GetText("immortal"));
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.ModifiedDamageType != Enums.DamageType.None)
                    {
                        stringBuilder1.Append("<nobr>");
                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsTransformDamage"), (object)medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.ModifiedDamageType))));
                        stringBuilder1.Append("</nobr>");
                        stringBuilder1.Append("\n");
                    }
                    if (itemData.IsEnchantment && (itemData.CastedCardType != Enums.CardType.None || (itemData.Activation == Enums.EventActivation.PreFinishCast || itemData.Activation == Enums.EventActivation.FinishCast || itemData.Activation == Enums.EventActivation.FinishFinishCast) && !itemData.EmptyHand))
                    {

                        // Custom Text: Enchantment Only Before Cast
                        BinbinCustomText(TextLocation.ItemEnchantBeforeCast, ref stringBuilder1, __instance.Id);
                        if (itemData.CastedCardType != Enums.CardType.None)
                        {
                            stringBuilder2.Append("<color=#5E3016>");
                            stringBuilder2.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)itemData.CastedCardType)));
                            stringBuilder2.Append("</color>");
                        }
                        else
                            stringBuilder2.Append(" <sprite name=cards>");
                        if (itemData.UseTheNextInsteadWhenYouPlay)
                        {
                            if ((double)itemData.HealPercentBonus == 0.0)
                            {
                                string str12 = "";
                                if (itemData.DestroyAfterUses > 1)
                                    str12 = "(" + itemData.DestroyAfterUses.ToString() + ") ";
                                StringBuilder stringBuilder12 = new StringBuilder();
                                stringBuilder12.Append("<size=-.15><color=#444>[");
                                stringBuilder12.Append(Texts.Instance.GetText("itemTheNext"));
                                stringBuilder12.Append("]</color></size><br>");
                                stringBuilder1.Append(string.Format(stringBuilder12.ToString(), (object)str12, (object)stringBuilder2.ToString()));
                            }
                        }
                        else
                        {
                            stringBuilder1.Append("<size=-.15>");
                            stringBuilder1.Append("<color=#444>[");
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemWhenYouPlay"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("]</color>");
                            stringBuilder1.Append("</size><br>");
                        }
                        stringBuilder2.Clear();
                    }

                    // Custom Text: Item Before Activation
                    BinbinCustomText(TextLocation.ItemBeforeActivation, ref stringBuilder1, __instance.Id);
                    if (itemData.Activation != Enums.EventActivation.None && itemData.Activation != Enums.EventActivation.PreBeginCombat)
                    {
                        if (stringBuilder1.Length > 0)
                            stringBuilder1.Append("<line-height=15%><br></line-height>");
                        StringBuilder stringBuilder13 = new StringBuilder();
                        if (itemData.TimesPerTurn == 1)
                            stringBuilder13.Append(Texts.Instance.GetText("itemOncePerTurn"));
                        else if (itemData.TimesPerTurn == 2)
                            stringBuilder13.Append(Texts.Instance.GetText("itemTwicePerTurn"));
                        else if (itemData.TimesPerTurn == 3)
                            stringBuilder13.Append(Texts.Instance.GetText("itemThricePerTurn"));
                        else if (itemData.TimesPerTurn == 4)
                            stringBuilder13.Append(Texts.Instance.GetText("itemFourPerTurn"));
                        else if (itemData.TimesPerTurn == 5)
                            stringBuilder13.Append(Texts.Instance.GetText("itemFivePerTurn"));
                        else if (itemData.TimesPerTurn == 6)
                            stringBuilder13.Append(Texts.Instance.GetText("itemSixPerTurn"));
                        else if (itemData.TimesPerTurn == 7)
                            stringBuilder13.Append(Texts.Instance.GetText("itemSevenPerTurn"));
                        else if (itemData.TimesPerTurn == 8)
                            stringBuilder13.Append(Texts.Instance.GetText("itemEightPerTurn"));
                        if (stringBuilder13.Length > 0)
                        {
                            stringBuilder1.Append("<size=-.15>");
                            stringBuilder1.Append("<color=#444>[");
                            stringBuilder1.Append(stringBuilder13.ToString());
                            stringBuilder1.Append("]</color>");
                            stringBuilder1.Append("</size><br>");
                        }
                        StringBuilder stringBuilder14 = new StringBuilder();
                        if (itemData.Activation == Enums.EventActivation.BeginCombat)
                            stringBuilder14.Append(Texts.Instance.GetText("itemCombatStart"));
                        else if (itemData.Activation == Enums.EventActivation.BeginCombatEnd)
                            stringBuilder14.Append(Texts.Instance.GetText("itemCombatEnd"));
                        else if (itemData.Activation == Enums.EventActivation.BeginTurnAboutToDealCards || itemData.Activation == Enums.EventActivation.BeginTurnCardsDealt)
                        {
                            if (itemData.RoundCycle > 1)
                                stringBuilder14.Append(string.Format(Texts.Instance.GetText("itemEveryNRounds"), (object)itemData.RoundCycle.ToString()));
                            else if (itemData.ExactRound == 1)
                                stringBuilder14.Append(Texts.Instance.GetText("itemFirstTurn"));
                            // CHANGE: Added support for Turns other than Turn 1.
                            else if (itemData.ExactRound > 1)
                            {
                                LogDebug("Binbin Testing Text: ExactTurn");
                                LogDebug(Texts.Instance.GetText("itemFirstTurn"));
                                string textToAdd = Texts.Instance.GetText("itemEveryRound");
                                textToAdd = textToAdd.Replace("Every Turn", $"On Turn {itemData.ExactRound}");
                                stringBuilder14.Append(textToAdd);
                            }
                            // END CHANGE
                            else
                                stringBuilder14.Append(Texts.Instance.GetText("itemEveryRound"));
                        }
                        else if (itemData.Activation == Enums.EventActivation.Damage)
                            stringBuilder14.Append(Texts.Instance.GetText("itemDamageDone"));
                        else if (itemData.Activation == Enums.EventActivation.Damaged)
                        {
                            if ((double)itemData.LowerOrEqualPercentHP < 100.0)
                                stringBuilder14.Append(string.Format(Texts.Instance.GetText("itemWhenDamagedBelow"), (object)(itemData.LowerOrEqualPercentHP.ToString() + (Functions.SpaceBeforePercentSign() ? " " : "") + "%")));
                            else
                                stringBuilder14.Append(Texts.Instance.GetText("itemWhenDamaged"));
                        }
                        else if (itemData.Activation == Enums.EventActivation.Hitted)
                            stringBuilder14.Append(Texts.Instance.GetText("itemWhenHitted"));
                        else if (itemData.Activation == Enums.EventActivation.Block)
                            stringBuilder14.Append(Texts.Instance.GetText("itemWhenBlock"));
                        else if (itemData.Activation == Enums.EventActivation.Heal)
                            stringBuilder14.Append(Texts.Instance.GetText("itemHealDoneAction"));
                        else if (itemData.Activation == Enums.EventActivation.Healed)
                            stringBuilder14.Append(Texts.Instance.GetText("itemWhenHealed"));
                        else if (itemData.Activation == Enums.EventActivation.Evaded)
                            stringBuilder14.Append(Texts.Instance.GetText("itemWhenEvaded"));
                        else if (itemData.Activation == Enums.EventActivation.Evade)
                            stringBuilder14.Append(Texts.Instance.GetText("itemWhenEvade"));
                        else if (itemData.Activation == Enums.EventActivation.BeginRound)
                        {

                            if (itemData.RoundCycle > 1)
                                stringBuilder14.Append(string.Format(Texts.Instance.GetText("itemEveryRoundRoundN"), (object)itemData.RoundCycle.ToString()));
                            // CHANGE: Added support for ExactRounds
                            else if (itemData.ExactRound == 1)
                            {
                                LogDebug("Binbin Testing Text: First Round");
                                LogDebug(Texts.Instance.GetText("itemEveryRoundRound"));
                                string textToAdd = Texts.Instance.GetText("itemEveryRoundRound");
                                textToAdd = textToAdd.Replace("Every Round", $"First Round");
                                stringBuilder14.Append(textToAdd);
                            }
                            else if (itemData.ExactRound > 1)
                            {
                                LogDebug("Binbin Testing Text: ExactRound");
                                LogDebug(Texts.Instance.GetText("itemEveryRoundRound"));
                                string textToAdd = Texts.Instance.GetText("itemEveryRoundRound");
                                textToAdd = textToAdd.Replace("Every Round", $"On Round {itemData.ExactRound}");
                                stringBuilder14.Append(textToAdd);
                            }
                            // END CHANGE
                            else
                                stringBuilder14.Append(Texts.Instance.GetText("itemEveryRoundRound"));
                        }
                        else if (itemData.Activation == Enums.EventActivation.BeginTurn)
                        {
                            if (itemData.RoundCycle > 1)
                                stringBuilder14.Append(string.Format(Texts.Instance.GetText("itemEveryNRounds"), (object)itemData.RoundCycle.ToString()));
                            else
                                stringBuilder14.Append(Texts.Instance.GetText("itemEveryRound"));
                        }
                        else if (itemData.Activation == Enums.EventActivation.Killed)
                            stringBuilder14.Append(Texts.Instance.GetText("itemWhenKilled"));
                        else if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent == 0)
                            stringBuilder14.Append(string.Format(Texts.Instance.GetText("itemWhenYouApply"), (object)medsColorTextArray("curse", medsSpriteText(itemData.AuraCurseSetted.Id))));
                        if (stringBuilder14.Length > 0)
                        {
                            stringBuilder1.Append("<size=-.15>");
                            stringBuilder1.Append("<color=#444>[");
                            stringBuilder1.Append(stringBuilder14.ToString());
                            stringBuilder1.Append("]</color>");
                            stringBuilder1.Append("</size><br>");
                        }
                        if (itemData.UsedEnergy)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyForEnergyUsed"), (object)medsColorTextArray("system", medsSpriteText("energy"))));
                            stringBuilder1.Append("\n");
                        }
                        if (itemData.EmptyHand)
                        {
                            stringBuilder1.Append(Texts.Instance.GetText("itemWhenHandEmpty"));
                            stringBuilder1.Append(":<br>");
                        }


                        // Custom Text: Item After Activation Before Event
                        BinbinCustomText(TextLocation.ItemAfterActivationBeforeEvent, ref stringBuilder1, __instance.Id);
                        if (itemData.ChanceToDispel > 0 && itemData.ChanceToDispelNum > 0)
                        {
                            if (itemData.ChanceToDispel < 100)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemChanceToDispel"), (object)medsColorTextArray("aura", medsNumFormatItem(itemData.ChanceToDispel, percent: true)), (object)medsColorTextArray("curse", medsNumFormatItem(itemData.ChanceToDispelNum))));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDispel"), (object)medsColorTextArray("curse", medsNumFormatItem(itemData.ChanceToDispelNum))));
                            stringBuilder1.Append("\n");
                        }
                        if (!itemData.IsEnchantment && itemData.CastedCardType != Enums.CardType.None)
                        {
                            stringBuilder2.Append("<color=#5E3016>");
                            stringBuilder2.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)itemData.CastedCardType)));
                            stringBuilder2.Append("</color>");
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemWhenYouPlay"), (object)stringBuilder2.ToString()));
                            stringBuilder2.Clear();
                            stringBuilder1.Append(":\n");
                        }
                        else if (!itemData.IsEnchantment && itemData.CastedCardType == Enums.CardType.None && (itemData.Activation == Enums.EventActivation.PreFinishCast || itemData.Activation == Enums.EventActivation.FinishCast || itemData.Activation == Enums.EventActivation.FinishFinishCast) && !itemData.EmptyHand)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemWhenYouPlay"), (object)"  <sprite name=cards>"));
                            stringBuilder1.Append(":\n");
                        }

                        // Custom Text: Item Custom GoldShards
                        // CUSTOM DESCRIPTION: GOLD/SHARDS
                        if (__instance.GoldGainQuantity > 0 || __instance.ShardsGainQuantity > 0)
                        {
                            stringBuilder1.Append("Gain <color=#1E650F>");
                            if (__instance.GoldGainQuantity > 0)
                                stringBuilder1.Append(__instance.GoldGainQuantity.ToString() + "   <sprite name=gold>");
                            if (__instance.ShardsGainQuantity > 0)
                                stringBuilder1.Append(__instance.ShardsGainQuantity.ToString() + "   <sprite name=dust>");
                            stringBuilder1.Append("</color>\n");
                        }
                        if (__instance.GoldGainQuantity < 0 || __instance.ShardsGainQuantity < 0)
                        {
                            stringBuilder1.Append("Lose <color=#B00A00>");
                            if (__instance.GoldGainQuantity < 0)
                                stringBuilder1.Append(__instance.GoldGainQuantity.ToString() + "   <sprite name=gold>");
                            if (__instance.ShardsGainQuantity < 0)
                                stringBuilder1.Append(__instance.ShardsGainQuantity.ToString() + "   <sprite name=dust>");
                            stringBuilder1.Append("</color>\n");
                        }
                        // END CUSTOM DESCRIPTION: GOLD/SHARDS

                        if (itemData.DrawCards > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDraw"), (object)medsColorTextArray("", medsNumFormat(itemData.DrawCards), medsSpriteText("card"))));
                            stringBuilder1.Append("<br>");
                        }

                        BinbinCustomText(TextLocation.ItemActivationAfterGoldBeforeHeal, ref stringBuilder1, __instance.Id);
                        // Custom Text: ItemActivation After Gold Before Heal
                        if (itemData.HealQuantity > 0)
                        {
                            stringBuilder2.Append("<color=#111111>");
                            stringBuilder2.Append(medsNumFormatItem(itemData.HealQuantity, true));
                            stringBuilder2.Append("</color>");
                            if (itemData.ItemTarget == Enums.ItemTarget.AllHero)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverHeroes"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.Self)
                            {
                                if (itemData.Activation == Enums.EventActivation.Killed)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemResurrectHP"), (object)stringBuilder2.ToString()));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverSelf"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.AllEnemy)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverSelf"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("<br>");
                            stringBuilder2.Clear();
                        }
                        if (itemData.EnergyQuantity > 0 && itemData.ItemTarget == Enums.ItemTarget.Self)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)medsColorTextArray("system", medsNumFormat(itemData.EnergyQuantity), medsSpriteText("energy"))));
                        if (itemData.HealPercentQuantity > 0)
                        {
                            stringBuilder2.Append("<color=#111111>");
                            stringBuilder2.Append(medsNumFormatItem(itemData.HealPercentQuantity, true, true));
                            stringBuilder2.Append("</color>");
                            if (itemData.ItemTarget == Enums.ItemTarget.AllHero)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverHeroes"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.Self)
                            {
                                if (itemData.Activation == Enums.EventActivation.Killed)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemResurrectHP"), (object)stringBuilder2.ToString()));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverSelf"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.LowestFlatHpEnemy)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverLowestHPMonster"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.LowestFlatHpHero)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverLowestHPHero"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.AllEnemy)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverSelf"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.RandomHero)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverRandomHPHero"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.RandomEnemy)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemRecoverRandomHPMonster"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("<br>");
                            stringBuilder2.Clear();
                        }
                        if (itemData.HealPercentQuantitySelf < 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsYouLose"), (object)medsColorTextArray("damage", medsNumFormat(Mathf.Abs(itemData.HealPercentQuantitySelf)), Functions.SpaceBeforePercentSign() ? "" : "<space=-.1>", "% HP")));
                            stringBuilder1.Append("<br>");
                        }

                        // Custom Text: ItemActivation After Heal Before Damage
                        BinbinCustomText(TextLocation.ItemActivationAfterHealBeforeDamage, ref stringBuilder1, __instance.Id);
                        if (itemData.DamageToTarget > 0)
                        {
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsDealDamage"), (object)medsColorTextArray("damage", medsNumFormat(medsEnchantDamagePreCalculated), medsSpriteText(Enum.GetName(typeof(Enums.DamageType), (object)itemData.DamageToTargetType)))));
                            stringBuilder1.Append("\n");
                        }
                        int num18 = 0;
                        bool flag5 = true;

                        // Custom Text: ItemActivation After Damage Before AC
                        BinbinCustomText(TextLocation.ItemActivationAfterDamageBeforeAC, ref stringBuilder1, __instance.Id);
                        if ((UnityEngine.Object)itemData.AuracurseGain1 != (UnityEngine.Object)null && itemData.AuracurseGainValue1 > 0)
                        {
                            ++num18;
                            if (itemData.NotShowCharacterBonus)
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(itemData.AuracurseGainValue1), medsSpriteText(itemData.AuracurseGain1.Id)));
                            else
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, itemData.AuracurseGain1.Id, itemData.AuracurseGainValue1, character)), medsSpriteText(itemData.AuracurseGain1.Id)));
                            if (!itemData.AuracurseGain1.IsAura)
                                flag5 = false;
                        }
                        if ((UnityEngine.Object)itemData.AuracurseGain2 != (UnityEngine.Object)null && itemData.AuracurseGainValue2 > 0)
                        {
                            ++num18;
                            if (itemData.NotShowCharacterBonus)
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(itemData.AuracurseGainValue2), medsSpriteText(itemData.AuracurseGain2.Id)));
                            else
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, itemData.AuracurseGain2.Id, itemData.AuracurseGainValue2, character)), medsSpriteText(itemData.AuracurseGain2.Id)));
                        }
                        if ((UnityEngine.Object)itemData.AuracurseGain3 != (UnityEngine.Object)null && itemData.AuracurseGainValue3 > 0)
                        {
                            ++num18;
                            if (itemData.NotShowCharacterBonus)
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(itemData.AuracurseGainValue3), medsSpriteText(itemData.AuracurseGain3.Id)));
                            else
                                stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, itemData.AuracurseGain3.Id, itemData.AuracurseGainValue3, character)), medsSpriteText(itemData.AuracurseGain3.Id)));
                        }
                        int num19;

                        // Custom Text: ItemAfterACBeforeSelf
                        BinbinCustomText(TextLocation.ItemActivationAfterACBeforeSelf, ref stringBuilder1, __instance.Id);

                        if (num18 > 0)
                        {
                            if (itemData.ItemTarget == Enums.ItemTarget.Self)
                            {
                                if (itemData.HealQuantity > 0 || itemData.EnergyQuantity > 0 || itemData.HealPercentQuantity > 0)
                                {
                                    StringBuilder stringBuilder15 = new StringBuilder();
                                    if (flag5)
                                        stringBuilder15.Append(string.Format(Texts.Instance.GetText("cardsAnd"), (object)stringBuilder1.ToString(), (object)string.Format(Functions.LowercaseFirst(Texts.Instance.GetText("cardsGain")), (object)stringBuilder2.ToString())));
                                    else
                                        stringBuilder15.Append(string.Format(Texts.Instance.GetText("cardsAnd"), (object)stringBuilder1.ToString(), (object)string.Format(Functions.LowercaseFirst(Texts.Instance.GetText("cardsSuffer")), (object)stringBuilder2.ToString())));
                                    stringBuilder1.Clear();
                                    stringBuilder1.Append(stringBuilder15.ToString());
                                }
                                else if (flag5)
                                {
                                    string str13 = stringBuilder1.ToString();
                                    if (str13.Length > 8 && str13.Substring(str13.Length - 9) == "<c>, </c>")
                                        stringBuilder1.Append(string.Format(Functions.LowercaseFirst(Texts.Instance.GetText("cardsGain")), (object)stringBuilder2.ToString()));
                                    else
                                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                                }
                                else if (stringBuilder1.ToString().Substring(stringBuilder1.ToString().Length - 9) == "<c>, </c>")
                                    stringBuilder1.Append(string.Format(Functions.LowercaseFirst(Texts.Instance.GetText("cardsSuffer")), (object)stringBuilder2.ToString()));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.AllEnemy)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyEnemies"), (object)stringBuilder2.ToString()));
                            else if (itemData.ItemTarget == Enums.ItemTarget.AllHero)
                            {
                                if (__instance.CardClass == Enums.CardClass.Monster)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyHeroesFromMonster"), (object)stringBuilder2.ToString()));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyHeroes"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.RandomHero)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemForEveryCharge"), (object)medsColorTextArray("curse", medsSpriteText(itemData.AuraCurseSetted.Id))));
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyRandomHero"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.RandomEnemy)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemForEveryCharge"), (object)medsColorTextArray("curse", medsSpriteText(itemData.AuraCurseSetted.Id))));
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyRandomEnemy"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.HighestFlatHpEnemy)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemForEveryCharge"), (object)medsColorTextArray("curse", medsSpriteText(itemData.AuraCurseSetted.Id))));
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyHighestFlatHpEnemy"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.LowestFlatHpEnemy)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                {
                                    StringBuilder stringBuilder16 = stringBuilder1;
                                    string text = Texts.Instance.GetText("itemForEveryCharge");
                                    string[] strArray = new string[2];
                                    string str14;
                                    if (itemData.AuraCurseNumForOneEvent <= 1)
                                    {
                                        str14 = "";
                                    }
                                    else
                                    {
                                        num19 = itemData.AuraCurseNumForOneEvent;
                                        str14 = num19.ToString();
                                    }
                                    strArray[0] = str14;
                                    strArray[1] = medsSpriteText(itemData.AuraCurseSetted.Id);
                                    string str15 = medsColorTextArray("curse", strArray);
                                    string str16 = string.Format(text, (object)str15);
                                    stringBuilder16.Append(str16);
                                }
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyLowestFlatHpEnemy"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.HighestFlatHpHero)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                {
                                    StringBuilder stringBuilder17 = stringBuilder1;
                                    string text = Texts.Instance.GetText("itemForEveryCharge");
                                    string[] strArray = new string[2];
                                    string str17;
                                    if (itemData.AuraCurseNumForOneEvent <= 1)
                                    {
                                        str17 = "";
                                    }
                                    else
                                    {
                                        num19 = itemData.AuraCurseNumForOneEvent;
                                        str17 = num19.ToString();
                                    }
                                    strArray[0] = str17;
                                    strArray[1] = medsSpriteText(itemData.AuraCurseSetted.Id);
                                    string str18 = medsColorTextArray("curse", strArray);
                                    string str19 = string.Format(text, (object)str18);
                                    stringBuilder17.Append(str19);
                                }
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyHighestFlatHpHero"), (object)stringBuilder2.ToString()));
                            }
                            else if (itemData.ItemTarget == Enums.ItemTarget.LowestFlatHpHero)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                {
                                    StringBuilder stringBuilder18 = stringBuilder1;
                                    string text = Texts.Instance.GetText("itemForEveryCharge");
                                    string[] strArray = new string[2];
                                    string str20;
                                    if (itemData.AuraCurseNumForOneEvent <= 1)
                                    {
                                        str20 = "";
                                    }
                                    else
                                    {
                                        num19 = itemData.AuraCurseNumForOneEvent;
                                        str20 = num19.ToString();
                                    }
                                    strArray[0] = str20;
                                    strArray[1] = medsSpriteText(itemData.AuraCurseSetted.Id);
                                    string str21 = medsColorTextArray("curse", strArray);
                                    string str22 = string.Format(text, (object)str21);
                                    stringBuilder18.Append(str22);
                                }
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyLowestFlatHpHero"), (object)stringBuilder2.ToString()));
                            }
                            else if (__instance.TargetSide == Enums.CardTargetSide.Enemy || itemData.ItemTarget == Enums.ItemTarget.CurrentTarget)
                            {
                                if ((UnityEngine.Object)itemData.AuraCurseSetted != (UnityEngine.Object)null && itemData.AuraCurseNumForOneEvent > 0)
                                {
                                    StringBuilder stringBuilder19 = stringBuilder1;
                                    string text = Texts.Instance.GetText("itemApplyForEvery");
                                    string[] strArray = new string[2];
                                    string str23;
                                    if (itemData.AuraCurseNumForOneEvent <= 1)
                                    {
                                        str23 = "";
                                    }
                                    else
                                    {
                                        num19 = itemData.AuraCurseNumForOneEvent;
                                        str23 = num19.ToString();
                                    }
                                    strArray[0] = str23;
                                    strArray[1] = medsSpriteText(itemData.AuraCurseSetted.Id);
                                    string str24 = medsColorTextArray("curse", strArray);
                                    string str25 = stringBuilder2.ToString();
                                    string str26 = string.Format(text, (object)str24, (object)str25);
                                    stringBuilder19.Append(str26);
                                }
                                else if (itemData.ItemTarget == Enums.ItemTarget.Random)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemApplyRandom"), (object)stringBuilder2.ToString()));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsApply"), (object)stringBuilder2.ToString()));
                            }
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGrant"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                            stringBuilder2.Clear();
                        }
                        int num20 = 0;
                        bool flag6 = true;
                        if ((UnityEngine.Object)itemData.AuracurseGainSelf1 != (UnityEngine.Object)null && itemData.AuracurseGainSelfValue1 > 0)
                        {
                            ++num20;
                            stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, itemData.AuracurseGainSelf1.Id, itemData.AuracurseGainSelfValue1, character)), medsSpriteText(itemData.AuracurseGainSelf1.Id)));
                            if (!itemData.AuracurseGainSelf1.IsAura)
                                flag6 = false;
                        }
                        if ((UnityEngine.Object)itemData.AuracurseGainSelf2 != (UnityEngine.Object)null && itemData.AuracurseGainSelfValue2 > 0)
                        {
                            ++num20;
                            stringBuilder2.Append(medsColorTextArray("aura", medsNumFormat(medsGetFinalAuraCharges(__instance.CardClass, itemData.AuracurseGainSelf2.Id, itemData.AuracurseGainSelfValue2, character)), medsSpriteText(itemData.AuracurseGainSelf2.Id)));
                        }
                        if (num20 > 0)
                        {
                            if (itemData.HealQuantity > 0 || itemData.EnergyQuantity > 0 || itemData.HealPercentQuantity > 0)
                            {
                                StringBuilder stringBuilder20 = new StringBuilder();
                                if (flag6)
                                    stringBuilder20.Append(string.Format(Texts.Instance.GetText("cardsAnd"), (object)stringBuilder1.ToString(), (object)string.Format(Functions.LowercaseFirst(Texts.Instance.GetText("cardsGain")), (object)stringBuilder2.ToString())));
                                else
                                    stringBuilder20.Append(string.Format(Texts.Instance.GetText("cardsAnd"), (object)stringBuilder1.ToString(), (object)string.Format(Functions.LowercaseFirst(Texts.Instance.GetText("cardsSuffer")), (object)stringBuilder2.ToString())));
                                stringBuilder1.Clear();
                                stringBuilder1.Append(stringBuilder20.ToString());
                            }
                            else if (flag6)
                            {
                                if (stringBuilder1.ToString().Substring(stringBuilder1.ToString().Length - 9) == "<c>, </c>")
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsGain"), (object)stringBuilder2.ToString()));
                            }
                            else if (stringBuilder1.ToString().Substring(stringBuilder1.ToString().Length - 9) == "<c>, </c>")
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsSuffer"), (object)stringBuilder2.ToString()));
                            stringBuilder1.Append("\n");
                            stringBuilder2.Clear();
                        }

                        // Custom Text: ItemActivation After Self Before AddCard
                        BinbinCustomText(TextLocation.ItemActivationAfterSelfBeforeAddCard, ref stringBuilder1, __instance.Id);
                        if (itemData.CardNum > 0)
                        {
                            string str27;
                            if ((UnityEngine.Object)itemData.CardToGain != (UnityEngine.Object)null)
                            {
                                if (itemData.CardNum > 1)
                                    stringBuilder2.Append(medsColorTextArray("", medsNumFormat(itemData.CardNum), medsSpriteText("card")));
                                else
                                    stringBuilder2.Append(medsSpriteText("card"));
                                CardData cardData = Globals.Instance.GetCardData(itemData.CardToGain.Id, false);
                                if ((UnityEngine.Object)cardData != (UnityEngine.Object)null)
                                {
                                    stringBuilder2.Append(__instance.ColorFromCardDataRarity(cardData));
                                    stringBuilder2.Append(cardData.CardName);
                                    stringBuilder2.Append("</color>");
                                }
                                str27 = stringBuilder2.ToString();
                                stringBuilder2.Clear();
                            }
                            else
                            {
                                if (itemData.CardNum > 1)
                                    stringBuilder2.Append(medsColorTextArray("", medsNumFormat(itemData.CardNum), medsSpriteText("card")));
                                else
                                    stringBuilder2.Append(medsSpriteText("card"));
                                if (itemData.CardToGainType != Enums.CardType.None)
                                {
                                    stringBuilder2.Append("<color=#5E3016>");
                                    stringBuilder2.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)itemData.CardToGainType)));
                                    stringBuilder2.Append("</color>");
                                }
                                str27 = stringBuilder2.ToString();
                                stringBuilder2.Clear();
                            }
                            string str28 = "";
                            if (itemData.Permanent)
                            {
                                if (itemData.Vanish)
                                {
                                    if (itemData.CostZero)
                                        str28 = string.Format(Texts.Instance.GetText("cardsAddCostVanish"), (object)0);
                                    else if (itemData.CostReduction > 0)
                                        str28 = string.Format(Texts.Instance.GetText("cardsAddCostReducedVanish"), (object)medsNumFormatItem(itemData.CostReduction, true));
                                }
                                else if (itemData.CostZero)
                                    str28 = string.Format(Texts.Instance.GetText("cardsAddCost"), (object)0);
                                else if (itemData.CostReduction > 0)
                                    str28 = string.Format(Texts.Instance.GetText("cardsAddCostReduced"), (object)medsNumFormatItem(itemData.CostReduction, true));
                            }
                            else if (itemData.Vanish)
                            {
                                if (itemData.CostZero)
                                    str28 = string.Format(Texts.Instance.GetText("cardsAddCostVanishTurn"), (object)0);
                                else if (itemData.CostReduction > 0)
                                    str28 = string.Format(Texts.Instance.GetText("cardsAddCostReducedVanishTurn"), (object)medsNumFormatItem(itemData.CostReduction, true));
                            }
                            else if (itemData.CostZero)
                                str28 = string.Format(Texts.Instance.GetText("cardsAddCostTurn"), (object)0);
                            else if (itemData.CostReduction > 0)
                                str28 = string.Format(Texts.Instance.GetText("cardsAddCostReducedTurn"), (object)medsNumFormatItem(itemData.CostReduction, true));
                            if (itemData.DuplicateActive)
                            {
                                if (itemData.CardPlace == Enums.CardPlace.Hand)
                                    stringBuilder1.Append(Texts.Instance.GetText("cardsDuplicateHand"));
                            }
                            else if (itemData.CardPlace == Enums.CardPlace.RandomDeck)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsIDShuffleDeck"), (object)str27));
                            else if (itemData.CardPlace == Enums.CardPlace.Cast)
                            {
                                if ((UnityEngine.Object)itemData.CardToGain != (UnityEngine.Object)null)
                                {
                                    CardData cardData = Globals.Instance.GetCardData(itemData.CardToGain.Id, false);
                                    if ((UnityEngine.Object)cardData != (UnityEngine.Object)null)
                                    {
                                        stringBuilder2.Append(__instance.ColorFromCardDataRarity(cardData));
                                        stringBuilder2.Append(cardData.CardName);
                                        stringBuilder2.Append("</color>");
                                    }
                                }
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsCast"), (object)stringBuilder2.ToString()));
                                stringBuilder2.Clear();
                            }
                            else if (itemData.CardPlace == Enums.CardPlace.Hand)
                            {
                                if (itemData.CardNum > 1)
                                    stringBuilder2.Append(medsColorTextArray("", medsNumFormat(itemData.CardNum), medsSpriteText("card")));
                                else
                                    stringBuilder2.Append(medsSpriteText("card"));
                                if ((UnityEngine.Object)itemData.CardToGain != (UnityEngine.Object)null)
                                {
                                    CardData cardData = Globals.Instance.GetCardData(itemData.CardToGain.Id, false);
                                    if ((UnityEngine.Object)cardData != (UnityEngine.Object)null)
                                    {
                                        stringBuilder2.Append(__instance.ColorFromCardDataRarity(cardData));
                                        stringBuilder2.Append(cardData.CardName);
                                        stringBuilder2.Append("</color>");
                                    }
                                }
                                stringBuilder2.Clear();
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("cardsIDPlaceHand"), (object)str27));
                            }
                            if (str28 != "")
                            {
                                stringBuilder1.Append(" ");
                                stringBuilder1.Append(str2);
                                stringBuilder1.Append(str28);
                                stringBuilder1.Append(str3);
                            }
                            if (itemData.CardsReduced == 0)
                                stringBuilder1.Append("\n");
                            else
                                stringBuilder1.Append(" ");
                        }

                        BinbinCustomText(TextLocation.ItemActivationAfterAddCardBeforeReduceCost, ref stringBuilder1, __instance.Id);
                        // Custom Text: ItemActivation After AddCard Before CardReduce
                        if (itemData.CardsReduced > 0)
                        {
                            StringBuilder stringBuilder21 = new StringBuilder();
                            stringBuilder21.Append("<color=#5E3016>");
                            stringBuilder21.Append(itemData.CardsReduced);
                            stringBuilder21.Append("</color>");
                            string str29 = stringBuilder21.ToString();
                            string str30;
                            if (itemData.CardToReduceType == Enums.CardType.None)
                            {
                                str30 = "  <sprite name=cards>";
                            }
                            else
                            {
                                stringBuilder21.Clear();
                                stringBuilder21.Append("<color=#5E3016>");
                                stringBuilder21.Append(Texts.Instance.GetText(Enum.GetName(typeof(Enums.CardType), (object)itemData.CardToReduceType)));
                                stringBuilder21.Append("</color>");
                                str30 = stringBuilder21.ToString();
                            }
                            stringBuilder21.Clear();
                            stringBuilder21.Append("<color=#111111>");
                            stringBuilder21.Append(Mathf.Abs(itemData.CostReduceReduction));
                            stringBuilder21.Append("</color>");
                            string str31 = stringBuilder21.ToString();
                            string str32 = itemData.CostReduceEnergyRequirement <= 0 ? "<space=-.2>" : "<color=#444><size=-.2>" + string.Format(Texts.Instance.GetText("itemReduceCost"), (object)itemData.CostReduceEnergyRequirement) + "</size></color>";
                            if (itemData.CostReducePermanent && itemData.ReduceHighestCost)
                            {
                                string str33;
                                if (itemData.CardsReduced == 1)
                                {
                                    str33 = "";
                                }
                                else
                                {
                                    num19 = itemData.CardsReduced;
                                    str33 = "<color=#111111>(" + num19.ToString() + ")</color> ";
                                }
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemReduceHighestPermanent"), (object)str33, (object)str30, (object)str31, (object)str32));
                            }
                            else if (itemData.CostReducePermanent)
                            {
                                if (itemData.CardsReduced != 10)
                                {
                                    if (itemData.CostReduceReduction > 0)
                                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemReduce"), (object)str29, (object)str30, (object)str31, (object)str32));
                                    else
                                        stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemIncrease"), (object)str29, (object)str30, (object)str31, (object)str32));
                                }
                                else if (itemData.CostReduceReduction > 0)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemReduceAll"), (object)str30, (object)str31, (object)str32));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemIncreaseAll"), (object)str30, (object)str31, (object)str32));
                            }
                            else if (itemData.ReduceHighestCost)
                            {
                                string str34;
                                if (itemData.CardsReduced == 1)
                                {
                                    str34 = "";
                                }
                                else
                                {
                                    num19 = itemData.CardsReduced;
                                    str34 = "<color=#111111>(" + num19.ToString() + ")</color> ";
                                }
                                if (itemData.CostReduceReduction > 0)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemReduceHighestTurn"), (object)str34, (object)str30, (object)str31, (object)str32));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemIncreaseHighestDiscarded"), (object)str34, (object)str30, (object)str31, (object)str32));
                            }
                            else if (itemData.CardsReduced != 10)
                            {
                                if (itemData.CostReduceReduction > 0)
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemReduceTurn"), (object)str29, (object)str30, (object)str31, (object)str32));
                                else
                                    stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemIncreaseTurn"), (object)str29, (object)str30, (object)str31, (object)str32));
                            }
                            else if (itemData.CostReduceReduction > 0)
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemReduceTurnAll"), (object)str30, (object)str31, (object)str32));
                            else
                                stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemIncreaseTurnAll"), (object)str30, (object)str31, (object)str32));
                            stringBuilder1.Append("\n");
                        }
                    }

                    // Custom Text: ItemActivation After AllActivation BeforeDestroyConditions
                    BinbinCustomText(TextLocation.ItemActivationAfterAllActivationBeforeDestroyConditions, ref stringBuilder1, __instance.Id);
                    if (itemData.DestroyStartOfTurn || itemData.DestroyEndOfTurn)
                    {
                        stringBuilder1.Append("<voffset=-.1><size=-.05><color=#1A505A>- ");
                        stringBuilder1.Append(Texts.Instance.GetText("itemDestroyStartTurn"));
                        stringBuilder1.Append(" -</color></size>");
                    }
                    if (itemData.DestroyAfterUses > 0 && !itemData.UseTheNextInsteadWhenYouPlay)
                    {
                        stringBuilder1.Append("<nobr><size=-.05><color=#1A505A>- ");
                        if (itemData.DestroyAfterUses > 1)
                            stringBuilder1.Append(string.Format(Texts.Instance.GetText("itemLastUses"), (object)itemData.DestroyAfterUses));
                        else
                            stringBuilder1.Append(Texts.Instance.GetText("itemLastUse"));
                        stringBuilder1.Append(" -</color></size></nobr>");
                    }
                    if (itemData.TimesPerCombat > 0)
                    {
                        stringBuilder1.Append("<nobr><size=-.05><color=#1A505A>- ");
                        if (itemData.TimesPerCombat == 1)
                            stringBuilder1.Append(Texts.Instance.GetText("itemOncePerCombat"));
                        else if (itemData.TimesPerCombat == 2)
                            stringBuilder1.Append(Texts.Instance.GetText("itemTwicePerCombat"));
                        else if (itemData.TimesPerCombat == 3)
                            stringBuilder1.Append(Texts.Instance.GetText("itemThricePerCombat"));
                        stringBuilder1.Append(" -</color></size></nobr>");
                    }
                    if (itemData.PassSingleAndCharacterRolls)
                    {
                        stringBuilder1.Append(Texts.Instance.GetText("cardsPassEventRoll"));
                        stringBuilder1.Append("\n");
                    }
                }

                // Custom Text End    
                BinbinCustomText(TextLocation.End, ref stringBuilder1, __instance.Id);


                stringBuilder1.Replace("<c>", "<color=#5E3016>");
                stringBuilder1.Replace("</c>", "</color>");
                stringBuilder1.Replace("<nb>", "<nobr>");
                stringBuilder1.Replace("</nb>", "</nobr>");
                stringBuilder1.Replace("<br1>", "<br><line-height=15%><br></line-height>");
                stringBuilder1.Replace("<br2>", "<br><line-height=30%><br></line-height>");
                stringBuilder1.Replace("<br3>", "<br><line-height=50%><br></line-height>");
                __instance.DescriptionNormalized = stringBuilder1.ToString();
                __instance.DescriptionNormalized = Regex.Replace(__instance.DescriptionNormalized, "[,][ ]*(<(.*?)>)*(.)", (MatchEvaluator)(m => m.ToString().ToLower()));
                __instance.DescriptionNormalized = Regex.Replace(__instance.DescriptionNormalized, "<br>\\w", (MatchEvaluator)(m => m.ToString().ToUpper()));
                // Traverse.Create(__instance).Field("descriptionNormalized").SetValue(__instance.DescriptionNormalized);
                Globals.Instance.CardsDescriptionNormalized[__instance.Id] = stringBuilder1.ToString();
                if (includeInSearch)
                    Globals.Instance.IncludeInSearch(Regex.Replace(Regex.Replace(__instance.DescriptionNormalized, "<sprite name=(.*?)>", (MatchEvaluator)(m => Texts.Instance.GetText(m.Groups[1].Value))), "(<(.*?)>)*", ""), __instance.Id, false);
            }
            else
            {
                if (__instance.Id == testCard)
                {
                    LogDebug($"Testing Description Cache for {testCard},\n Normalized {__instance.DescriptionNormalized},\n Regular {__instance.Description}");
                    LogDebug($"Testing Description Cache same {Globals.Instance.CardsDescriptionNormalized[__instance.Id] == __instance.DescriptionNormalized}");
                }
                if (__instance.Id == testCard2)
                {
                    LogDebug($"Testing Description Cache for {testCard2},\n Normalized {__instance.DescriptionNormalized},\n Regular {__instance.Description}");
                }
                __instance.DescriptionNormalized = Globals.Instance.CardsDescriptionNormalized[__instance.Id];
            }


            return false;

        }

        //supporting
        public static string medsNumFormatItem(int num, bool plus = false, bool percent = false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" <nobr>");
            if (num > 0)
            {
                stringBuilder.Append("<color=#263ABC><size=+.1>");
                if (plus)
                    stringBuilder.Append("+");
            }
            else
            {
                stringBuilder.Append("<color=#720070><size=+.1>");
                if (plus)
                    stringBuilder.Append("-");
            }
            stringBuilder.Append(Mathf.Abs(num));
            if (percent)
                stringBuilder.Append("%");
            stringBuilder.Append("</color></size></nobr>");
            return stringBuilder.ToString();
        }
        public static string medsNumFormat(int oldNum)
        {
            int num = oldNum;
            if (num < 0)
                num = 0;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<size=+.1>");
            stringBuilder.Append(num);
            stringBuilder.Append("</size>");
            return stringBuilder.ToString();
        }
        public static string medsColorTextArray(string type, params string[] text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<nobr>");
            switch (type)
            {
                case "":
                    int num = 0;
                    foreach (string str in text)
                    {
                        if (num > 0)
                            stringBuilder.Append(" ");
                        stringBuilder.Append(str);
                        ++num;
                    }
                    if (type != "")
                        stringBuilder.Append("</color>");
                    stringBuilder.Append("</nobr> ");
                    return stringBuilder.ToString();
                case "damage":
                    stringBuilder.Append("<color=#B00A00>");
                    goto case "";
                case "heal":
                    stringBuilder.Append("<color=#1E650F>");
                    goto case "";
                case "aura":
                    stringBuilder.Append("<color=#263ABC>");
                    goto case "";
                case "curse":
                    stringBuilder.Append("<color=#720070>");
                    goto case "";
                case "system":
                    stringBuilder.Append("<color=#5E3016>");
                    goto case "";
                default:
                    stringBuilder.Append("<color=#5E3016");
                    stringBuilder.Append(">");
                    goto case "";
            }
        }
        public static string medsSpriteText(string sprite)
        {

            StringBuilder stringBuilder = new StringBuilder();
            string str = sprite.ToLower().Replace(" ", "");
            if (str == "stealthbonus")
            {
                str = "stealth";
            }
            switch (str)
            {
                case "block":
                case "card":
                    stringBuilder.Append("<space=.2>");
                    break;
                case "piercing":
                    stringBuilder.Append("<space=.4>");
                    break;
                case "bleed":
                    stringBuilder.Append("<space=.1>");
                    break;
                case "bless":
                    stringBuilder.Append("<space=.1>");
                    break;
                default:
                    stringBuilder.Append("<space=.3>");
                    break;
            }
            stringBuilder.Append("<size=+.1><sprite name=");
            stringBuilder.Append(str);
            stringBuilder.Append("></size>");
            switch (str)
            {
                case "bleed":
                    stringBuilder.Append("<space=-.4>");
                    goto case "reinforce";
                case "reinforce":
                case "fire":
                    return stringBuilder.ToString();
                case "card":
                    stringBuilder.Append("<space=-.2>");
                    goto case "reinforce";
                case "powerful":
                case "fury":
                    stringBuilder.Append("<space=-.1>");
                    goto case "reinforce";
                default:
                    stringBuilder.Append("<space=-.2>");
                    goto case "reinforce";
            }
        }
        public static string medsColorFromCardDataRarity(CardData _cData)
        {
            if (!((UnityEngine.Object)_cData != (UnityEngine.Object)null))
                return "<color=#5E3016>";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<color=#");
            if (_cData.CardUpgraded == Enums.CardUpgraded.A)
                stringBuilder.Append(medsColorUpgradeBlue);
            else if (_cData.CardUpgraded == Enums.CardUpgraded.B)
                stringBuilder.Append(medsColorUpgradeGold);
            else if (_cData.CardUpgraded == Enums.CardUpgraded.Rare)
                stringBuilder.Append(medsColorUpgradeRare);
            else
                stringBuilder.Append(medsColorUpgradePlain);
            stringBuilder.Append(">");
            return stringBuilder.ToString();
        }

        public static int medsGetFinalAuraCharges(Enums.CardClass _cardClass, string acId, int charges, Character character = null) => character == null ? charges : charges + character.GetAuraCurseQuantityModification(acId, _cardClass);


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardData), "SetTarget")]
        public static bool SetTargetPrefix(ref CardData __instance)
        {
            if (__instance.AutoplayDraw)
                __instance.Target = Texts.Instance.GetText("onDraw");
            else if (__instance.AutoplayEndTurn)
                __instance.Target = Texts.Instance.GetText("onEndTurn");
            else if (__instance.TargetType == Enums.CardTargetType.Global && __instance.TargetSide == Enums.CardTargetSide.Anyone)
                __instance.Target = Texts.Instance.GetText("global");
            else if (__instance.TargetSide == Enums.CardTargetSide.Self)
                __instance.Target = Texts.Instance.GetText("self");
            else if (__instance.TargetSide == Enums.CardTargetSide.Anyone && __instance.TargetPosition == Enums.CardTargetPosition.Random)
                __instance.Target = Texts.Instance.GetText("random");
            else if (__instance.TargetType == Enums.CardTargetType.Single && __instance.TargetSide == Enums.CardTargetSide.Anyone && __instance.TargetPosition == Enums.CardTargetPosition.Anywhere)
                __instance.Target = Texts.Instance.GetText("anyone");
            else if (__instance.CardClass != Enums.CardClass.Monster)
            {
                if (__instance.TargetSide == Enums.CardTargetSide.Friend)
                {
                    if (__instance.TargetType == Enums.CardTargetType.Global)
                    {
                        __instance.Target = Texts.Instance.GetText("allHeroes");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Random)
                    {
                        __instance.Target = Texts.Instance.GetText("randomHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Front)
                    {
                        __instance.Target = Texts.Instance.GetText("frontHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Back)
                    {
                        __instance.Target = Texts.Instance.GetText("backHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Middle)
                    {
                        __instance.Target = Texts.Instance.GetText("middleHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Slowest)
                    {
                        __instance.Target = Texts.Instance.GetText("slowestHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Fastest)
                    {
                        __instance.Target = Texts.Instance.GetText("fastestHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.MostHP)
                    {
                        __instance.Target = Texts.Instance.GetText("mostHPHero");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.LeastHP)
                    {
                        __instance.Target = Texts.Instance.GetText("leastHPHero");
                    }
                    else
                    {
                        __instance.Target = Texts.Instance.GetText("hero");
                    }
                }
                else if (__instance.TargetSide == Enums.CardTargetSide.FriendNotSelf)
                {
                    __instance.Target = __instance.TargetType == Enums.CardTargetType.Global ? "All Other Heroes" : Texts.Instance.GetText("otherHero");
                }
                else if (__instance.TargetSide == Enums.CardTargetSide.Enemy)
                {
                    if (__instance.TargetType == Enums.CardTargetType.Global)
                    {
                        __instance.Target = Texts.Instance.GetText("allMonsters");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Random)
                    {
                        __instance.Target = Texts.Instance.GetText("randomMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Front)
                    {
                        __instance.Target = Texts.Instance.GetText("frontMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Back)
                    {
                        __instance.Target = Texts.Instance.GetText("backMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.MostHP)
                    {
                        __instance.Target = Texts.Instance.GetText("mostHPMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.LeastHP)
                    {
                        __instance.Target = Texts.Instance.GetText("leastHPMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Middle)
                    {
                        __instance.Target = Texts.Instance.GetText("middleMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Slowest)
                    {
                        __instance.Target = Texts.Instance.GetText("slowestMonster");
                    }
                    else if (__instance.TargetPosition == Enums.CardTargetPosition.Fastest)
                    {
                        __instance.Target = Texts.Instance.GetText("fastestMonster");
                    }
                    else
                    {
                        __instance.Target = Texts.Instance.GetText("monster");
                    }
                }
            }
            else if (__instance.CardClass == Enums.CardClass.Monster)
            {
                if (__instance.TargetSide == Enums.CardTargetSide.Friend)
                    __instance.Target = __instance.TargetType != Enums.CardTargetType.Global ? (__instance.TargetPosition != Enums.CardTargetPosition.Random ? (__instance.TargetPosition != Enums.CardTargetPosition.Front ? (__instance.TargetPosition != Enums.CardTargetPosition.Back ? (__instance.TargetPosition != Enums.CardTargetPosition.Middle ? (__instance.TargetPosition != Enums.CardTargetPosition.Slowest ? (__instance.TargetPosition != Enums.CardTargetPosition.Fastest ? (__instance.TargetPosition != Enums.CardTargetPosition.LeastHP ? (__instance.TargetPosition != Enums.CardTargetPosition.MostHP ? Texts.Instance.GetText("monster") : Texts.Instance.GetText("mostHPMonster")) : Texts.Instance.GetText("leastHPMonster")) : Texts.Instance.GetText("fastestMonster")) : Texts.Instance.GetText("slowestMonster")) : Texts.Instance.GetText("middleMonster")) : Texts.Instance.GetText("backMonster")) : Texts.Instance.GetText("frontMonster")) : Texts.Instance.GetText("randomMonster")) : Texts.Instance.GetText("allMonsters");
                else if (__instance.TargetSide == Enums.CardTargetSide.FriendNotSelf)
                    __instance.Target = Texts.Instance.GetText("otherMonster");
                else if (__instance.TargetSide == Enums.CardTargetSide.Enemy)
                    __instance.Target = __instance.TargetType != Enums.CardTargetType.Global ? (__instance.TargetPosition != Enums.CardTargetPosition.Random ? (__instance.TargetPosition != Enums.CardTargetPosition.Front ? (__instance.TargetPosition != Enums.CardTargetPosition.Back ? (__instance.TargetPosition != Enums.CardTargetPosition.Middle ? (__instance.TargetPosition != Enums.CardTargetPosition.Slowest ? (__instance.TargetPosition != Enums.CardTargetPosition.Fastest ? (__instance.TargetPosition != Enums.CardTargetPosition.LeastHP ? (__instance.TargetPosition != Enums.CardTargetPosition.MostHP ? Texts.Instance.GetText("hero") : Texts.Instance.GetText("mostHPHero")) : Texts.Instance.GetText("leastHPHero")) : Texts.Instance.GetText("fastestHero")) : Texts.Instance.GetText("slowestHero")) : Texts.Instance.GetText("middleHero")) : Texts.Instance.GetText("backHero")) : Texts.Instance.GetText("frontHero")) : Texts.Instance.GetText("randomHero")) : Texts.Instance.GetText("allHeroes");
            }
            Globals.Instance.IncludeInSearch(__instance.Target, __instance.Id);
            return false; // do not run original method
        }



        public static void BinbinNormalizeDescription(ref CardData __instance, StringBuilder stringBuilder1, bool includeInSearch)
        {

            stringBuilder1.Replace("<c>", "<color=#5E3016>");
            stringBuilder1.Replace("</c>", "</color>");
            stringBuilder1.Replace("<nb>", "<nobr>");
            stringBuilder1.Replace("</nb>", "</nobr>");
            stringBuilder1.Replace("<br1>", "<br><line-height=15%><br></line-height>");
            stringBuilder1.Replace("<br2>", "<br><line-height=30%><br></line-height>");
            stringBuilder1.Replace("<br3>", "<br><line-height=50%><br></line-height>");
            __instance.DescriptionNormalized = stringBuilder1.ToString();
            __instance.DescriptionNormalized = Regex.Replace(__instance.DescriptionNormalized, "[,][ ]*(<(.*?)>)*(.)", (MatchEvaluator)(m => m.ToString().ToLower()));
            __instance.DescriptionNormalized = Regex.Replace(__instance.DescriptionNormalized, "<br>\\w", (MatchEvaluator)(m => m.ToString().ToUpper()));
            Globals.Instance.CardsDescriptionNormalized[__instance.Id] = stringBuilder1.ToString();
            if (includeInSearch)
                Globals.Instance.IncludeInSearch(Regex.Replace(Regex.Replace(__instance.DescriptionNormalized, "<sprite name=(.*?)>", (MatchEvaluator)(m => Texts.Instance.GetText(m.Groups[1].Value))), "(<(.*?)>)*", ""), __instance.Id, false);

        }

        public static void BinbinCustomText(TextLocation location, ref StringBuilder stringBuilder, string card)
        {
            if (TextToAddAtLocations == null)
            {
                TextToAddAtLocations = [];
            }

            if (TextToAddAtLocations.TryGetValue(location, out Dictionary<string, List<string>> cardsToAddTextTo))
            {
                if (cardsToAddTextTo.TryGetValue(card, out List<string> textsToAdd))
                {
                    foreach (string text in textsToAdd)
                    {
                        stringBuilder.Append(text);
                        stringBuilder.Append(text.EndsWith("\n") ? "" : "\n");
                    }

                }
            }
            // if (!TextToAddAtLocations.ContainsKey(location))
            // {
            //     return;
            // }
            // Dictionary<CardData, List<string>> cardsToAddTextTo = TextToAddAtLocations[location];
            // if (!cardsToAddTextTo.ContainsKey(card))
            // {
            //     return;
            // }
            // List<string> textsToAdd = cardsToAddTextTo[card];

        }

        public static void AddTextToCardDescription(string text, TextLocation location, string card, bool includeAB = false, bool includeRare = false)
        {
            TextToAddAtLocations ??= [];

            if (!TextToAddAtLocations.ContainsKey(location))
            {
                TextToAddAtLocations[location] = [];
            }

            Dictionary<string, List<string>> cardsToAddTextTo = TextToAddAtLocations[location];

            AddCardToDict(text, location, card, cardsToAddTextTo);

            if (includeAB)
            {
                AddCardToDict(text, location, card + "a", cardsToAddTextTo);
                AddCardToDict(text, location, card + "b", cardsToAddTextTo);

            }
            if (includeRare)
            {
                AddCardToDict(text, location, card + "rare", cardsToAddTextTo);

            }

        }

        public static void AddCardToDict(string text, TextLocation location, string card, Dictionary<string, List<string>> cardsToAddTextTo, bool refreshDescription = false)
        {
            if (!cardsToAddTextTo.ContainsKey(card))
            {
                cardsToAddTextTo[card] = [];
            }
            cardsToAddTextTo[card].Add(text);

            // Refreshes the card description if the AddText function is called in weird locations
            Globals.Instance?.GetCardData(card, false)?.SetDescriptionNew(true, null, true);

            TextToAddAtLocations[location] = cardsToAddTextTo;
            ModifiedCards ??= [];
            ModifiedCards.Add(card);
        }

        // public static void AddTextToCardDescription(string text, TextLocation location, string card)
        // {
        //     if (Globals.Instance.GetCardData(card, false) == null)
        //     {
        //         LogError($"Card {card} not found to add description to.");
        //         return;
        //     }
        //     AddTextToCardDescription(text, location, Globals.Instance.GetCardData(card, false));

        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardItem), nameof(CardItem.SetCard))]
        public static void SetCardPostfix(
                ref CardItem __instance,
                ref TMP_Text ___descriptionTextTM,
                string id,
                bool deckScale = true,
                Hero _theHero = null,
                NPC _theNPC = null,
                bool GetFromGlobal = false,
                bool _generated = false)
        {
            LogDebug($"SetCardPostfix {id}");
            bool isNull = __instance == null && __instance.CardData == null;
            if (isNull)
            {
                return;
            }
            string cardId = id.Split("_")[0];
            bool needsUpdating = ModifiedCards?.Contains(cardId) ?? false;
            if (needsUpdating)
            {

                if (Globals.Instance.CardsDescriptionNormalized.TryGetValue(cardId, out string newDescription))
                {
                    if (newDescription.IsNullOrWhiteSpace())
                    {
                        return;
                    }
                    ___descriptionTextTM.text = newDescription;
                    LogDebug($"SetCardPostfix - Setting Combat Description for {__instance.CardData.Id} - {newDescription}");
                }
                else
                {
                    LogError($"SetCardPostfix - Missing CardDescription for {cardId} with internalId {__instance.CardData.Id}");
                }

            }
        }
    }

}
