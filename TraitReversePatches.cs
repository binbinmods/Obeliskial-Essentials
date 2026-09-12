using System;
using System.Collections.Generic;
using BattleMatch;
using Cards;
using HarmonyLib;

namespace Obeliskial_Essentials
{
    [HarmonyPatch]
    public static class TraitReversePatches
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "HasAnyTrait")]
        public static bool HasAnyTrait(Character character, params string[] traitIds)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.HasAnyTrait");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "IsWeaponsExpertBonusAllowed")]
        public static bool IsWeaponsExpertBonusAllowed(Trait __instance, Character character, string auraCurseId)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.IsWeaponsExpertBonusAllowed");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "DoCombatLogEntry")]
        public static void DoCombatLogEntry(Trait __instance, Enums.EventActivation _theEvent, string _trait, Character _character, Character _target, int _auxInt, string _auxString, CardRealtimeData _castedCard)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.DoCombatLogEntry");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "PlayRuneVFX")]
        public static void PlayRuneVFX(Trait __instance, Character character, string rune)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.PlayRuneVFX");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ReduceDeckCostOn3Runes")]
        public static void ReduceDeckCostOn3Runes(Trait __instance, Enums.EventActivation theEvent, Character character, Character target, int auxInt, string auxString, CardRealtimeData castedCard, string trait, string rune, Func<CardRealtimeData, bool> CardTypeCondition)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ReduceDeckCostOn3Runes");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ReduceDeckCost")]
        public static void ReduceDeckCost(Trait __instance, Character character, int reduction, bool playParticles, Func<CardRealtimeData, bool> CardTypeCondition, bool skipHand = false, string reductionSourceKey = "")
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ReduceDeckCost");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "DisplayTraitScrollText")]
        public static void DisplayTraitScrollText(Trait __instance, Enums.EventActivation theEvent, Character character, Character target, int auxInt, string auxString, CardRealtimeData castedCard, string trait)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.DisplayTraitScrollText");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetTraitCombatText")]
        public static string GetTraitCombatText(Trait __instance, string trait, Character character)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetTraitCombatText");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetRuneReductionSourceKey")]
        public static string GetRuneReductionSourceKey(Trait __instance, Character character, string trait, string rune)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetRuneReductionSourceKey");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "TrackRuneCostReduction")]
        public static void TrackRuneCostReduction(Trait __instance, string sourceKey, CardRealtimeData cardData, int amount)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.TrackRuneCostReduction");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetTrackedRuneCostReduction")]
        public static int GetTrackedRuneCostReduction(Trait __instance, string sourceKey, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetTrackedRuneCostReduction");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ConsumeTrackedRuneCostReduction")]
        public static bool ConsumeTrackedRuneCostReduction(Trait __instance, Character character, string trait, string rune, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ConsumeTrackedRuneCostReduction");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ResetDeckCost")]
        public static void ResetDeckCost(Trait __instance, Character character, Func<CardRealtimeData, bool> CardTypeCondition, string reductionSourceKey = "")
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ResetDeckCost");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ApplyMindCollapseCostReductionsToHand")]
        public static void ApplyMindCollapseCostReductionsToHand(Trait __instance, Character character, CardRealtimeData castedCard)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ApplyMindCollapseCostReductionsToHand");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ApplyMindCollapseCostReduction")]
        public static bool ApplyMindCollapseCostReduction(Trait __instance, Character character, CardRealtimeData cardData, int energyReduction = 1)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ApplyMindCollapseCostReduction");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ResetMindCollapseCostReductions")]
        public static void ResetMindCollapseCostReductions(Trait __instance, Character character)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ResetMindCollapseCostReductions");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetLowestHealthHero")]
        public static Character GetLowestHealthHero(Trait __instance)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetLowestHealthHero");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "HealTarget")]
        public static void HealTarget(Trait __instance, Character character, int healValue, Character healer = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.HealTarget");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetHandCards")]
        public static List<CardRealtimeData> GetHandCards(Trait __instance, Character character, Func<CardRealtimeData, bool> condition, CardRealtimeData excludedCard = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetHandCards");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetHighestCostHandCard")]
        public static CardRealtimeData GetHighestCostHandCard(Trait __instance, Character character, Func<CardRealtimeData, bool> condition, CardRealtimeData excludedCard = null, bool randomizeAmongHighest = true)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetHighestCostHandCard");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ReduceHighestCostHandCardCost")]
        public static bool ReduceHighestCostHandCardCost(Trait __instance, Character character, Func<CardRealtimeData, bool> condition, int reduction, string traitText = null, bool permanent = false, bool playDissolveParticle = true, bool showTraitText = true, bool updateHandCardsBeforeVisuals = true, CardRealtimeData excludedCard = null, bool randomizeAmongHighest = true)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ReduceHighestCostHandCardCost");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ReduceTypedHandCardCosts")]
        public static int ReduceTypedHandCardCosts(Trait __instance, Character character, Enums.CardType cardType, int reduction, string traitText = null, bool permanent = false, bool playDissolveParticle = true, bool showTraitTextPerCard = true, bool updateHandCardsBeforeVisuals = true, CardRealtimeData excludedCard = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ReduceTypedHandCardCosts");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ReduceHandCardCosts")]
        public static int ReduceHandCardCosts(Trait __instance, Character character, Func<CardRealtimeData, bool> condition, int reduction, string traitText = null, bool permanent = false, bool playDissolveParticle = true, bool showTraitTextPerCard = true, bool updateHandCardsBeforeVisuals = true, CardRealtimeData excludedCard = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ReduceHandCardCosts");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "ReduceCardCost")]
        public static bool ReduceCardCost(Trait __instance, Character character, CardRealtimeData cardData, int reduction, string traitText = null, bool permanent = false, bool playDissolveParticle = true, bool showTraitText = true, bool updateHandCardsBeforeVisuals = true)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.ReduceCardCost");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "TryUseLimitedHighestCostReductionTrait")]
        public static bool TryUseLimitedHighestCostReductionTrait(Trait __instance, Character character, string traitId, string traitText, Func<CardRealtimeData, bool> condition, bool permanent = false, bool playDissolveParticle = true, bool updateHandCardsBeforeVisuals = true, string extraChargeTrait = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.TryUseLimitedHighestCostReductionTrait");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "TryUseDualityCostReductionTrait")]
        public static void TryUseDualityCostReductionTrait(Trait __instance, Character character, CardRealtimeData castedCard, string traitId, string traitText, Enums.CardClass firstTriggerClass, Enums.CardClass firstTargetClass, Enums.CardClass secondTriggerClass, Enums.CardClass secondTargetClass, string extraChargeTrait = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.TryUseDualityCostReductionTrait");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetTraitExtraCharges")]
        public static int GetTraitExtraCharges(Trait __instance, Character character, string extraChargeTrait)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetTraitExtraCharges");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "TryGetTraitUsageData")]
        public static bool TryGetTraitUsageData(Trait __instance, string traitId, int extraCharges, Character character, out TraitData traitData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.TryGetTraitUsageData");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "TryGetDualityTargetClass")]
        public static bool TryGetDualityTargetClass(Trait __instance, Enums.CardClass castedClass, Enums.CardClass firstTriggerClass, Enums.CardClass firstTargetClass, Enums.CardClass secondTriggerClass, Enums.CardClass secondTargetClass, out Enums.CardClass targetClass)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.TryGetDualityTargetClass");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "CanUseTrait", new Type[] { typeof(string), typeof(int) })]
        public static bool CanUseTrait(Trait __instance, string trait, int timesPerTurn)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.CanUseTrait");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "isHealCard")]
        public static bool isHealCard(Trait __instance, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.isHealCard");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "isDefenseCard")]
        public static bool isDefenseCard(Trait __instance, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.isDefenseCard");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "isSkillCard")]
        public static bool isSkillCard(Trait __instance, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.isSkillCard");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "isSpellCard")]
        public static bool isSpellCard(Trait __instance, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.isSpellCard");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "HasAnyCardType")]
        public static bool HasAnyCardType(Trait __instance, CardRealtimeData card, IEnumerable<Enums.CardType> validTypes)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.HasAnyCardType");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "GetRandomCharacter")]
        public static Character GetRandomCharacter(Trait __instance, Team team, Func<Character, bool> extraCondition = null)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.GetRandomCharacter");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "IsValidTraitTriggerCard")]
        public static bool IsValidTraitTriggerCard(Trait __instance, CardRealtimeData card)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.IsValidTraitTriggerCard");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trait), "isAttackCard")]
        public static bool isAttackCard(Trait __instance, CardRealtimeData cardData)
        {
            throw new NotImplementedException("Reverse patch stub for Trait.isAttackCard");
        }
    }
}
