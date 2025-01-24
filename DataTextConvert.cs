using System;
using UnityEngine;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using static UnityEngine.JsonUtility;


namespace Obeliskial_Essentials
{
    public class DataTextConvert
    {
        /*
         *                                                                                                           
         *    888888888888  ,ad8888ba,        ad88888ba  888888888888  88888888ba   88  888b      88    ,ad8888ba,   
         *         88      d8"'    `"8b      d8"     "8b      88       88      "8b  88  8888b     88   d8"'    `"8b  
         *         88     d8'        `8b     Y8,              88       88      ,8P  88  88 `8b    88  d8'            
         *         88     88          88     `Y8aaaaa,        88       88aaaaaa8P'  88  88  `8b   88  88             
         *         88     88          88       `"""""8b,      88       88""""88'    88  88   `8b  88  88      88888  
         *         88     Y8,        ,8P             `8b      88       88    `8b    88  88    `8b 88  Y8,        88  
         *         88      Y8a.    .a8P      Y8a     a8P      88       88     `8b   88  88     `8888   Y8a.    .a88  
         *         88       `"Y8888Y"'        "Y88888P"       88       88      `8b  88  88      `888    `"Y88888P"   
         *
         *   Returns a standardised identification string.
         */
        public static string ToString(AuraCurseData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(CardData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(CardbackData data)
        {
            return data != null ? data.CardbackId : "";
        }
        public static string ToString(SubClassData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(PerkData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(PerkNodeData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(KeyNotesData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(NPCData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(TraitData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(ItemData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(NodeData data)
        {
            return data != null ? data.NodeId : "";
        }
        public static string ToString(LootData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(LootItem data)
        {
            return ToJson(ToText(data), true);
        }
        public static string ToString(CombatData data)
        {
            return data != null ? data.CombatId : "";
        }
        public static string ToString(EventData data)
        {
            return data != null ? data.EventId : "";
        }
        public static string ToString(EventReplyDataText data)
        {
            return data != null ? data.medsTempID : "";
        }
        public static string ToString(ZoneData data)
        {
            return data != null ? data.ZoneId : "";
        }
        public static string ToString(EventRequirementData data)
        {
            return data != null ? data.RequirementId : "";
        }
        public static string ToString(Sprite sprite)
        {
            if (sprite != null)
                return sprite.name;
            return "";
        }
        public static string ToString(SkinData data)
        {
            return data != null ? data.SkinId : "";
        }
        public static string ToString(PackData data)
        {
            return data != null ? data.PackId : "";
        }
        public static string ToString(ChallengeData data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(ChallengeTrait data)
        {
            return data != null ? data.Id : "";
        }
        public static string ToString(CinematicData data)
        {
            return data != null ? data.CinematicId : "";
        }
        public static string ToString(ThermometerTierData data)
        {
            return data != null ? data.name : "";
        }
        public static string ToString(CardPlayerPackData data)
        {
            return data != null ? data.PackId : "";
        }
        public static string ToString(UnityEngine.Object data)
        {
            return data != null ? data.name : "";
        }
        public static string ToString(AudioClip data)
        {
            return data != null ? data.name : "";
        }
        public static string ToString(GameObject data)
        {
            return data != null ? data.name : "";
        }
        public static string ToString(NodesConnectedRequirement data)
        {
            return ToJson(ToText(data), true);
        }
        public static string ToString(CombatEffect data)
        {
            return ToJson(ToText(data), true);
        }
        public static string ToString(CorruptionPackData data)
        {
            return data != null ? data.PackName : "";
        }
        public static string ToString(CardPlayerPairsPackData data)
        {
            return data != null ? data.PackId : "";
        }
        public static string ToString(Vector2 data)
        {
            return data.ToString();
        }
        public static string ToString(Vector3 data)
        {
            return data.ToString();
        }
        public static string ToString(TierRewardData data)
        {
            return data != null ? data.TierNum.ToString() : "";
        }
        public static string ToString<T>(T data)
        {
            if (typeof(T).BaseType == typeof(Enum))
                return Enum.GetName(data.GetType(), data);
            LogError("ToString<T> is capturing a type that it isn't set up for!!! " + typeof(T));
            return "";
        }
        // I'm pretty sure these can be simplified? probably with the var keyword? f!L pls lmk
        public static string[] ToString(SubClassData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(CardData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(PerkData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(HeroCards[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToJson(ToText(data[a]), true);
            return text;
        }
        public static string[] ToString(EventData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(CombatData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(CombatEffect[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(NPCData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(ChallengeData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(ChallengeTrait[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(NodeData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(PerkNodeData[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(LootItem[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToString(data[a]);
            return text;
        }
        public static string[] ToString(NodesConnectedRequirement[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToJson(ToText(data[a]), true);
            return text;
        }
        public static string[] ToString(AICards[] data)
        {
            string[] text = new string[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToJson(ToText(data[a]), true);
            return text;
        }
        public static string[] ToString<T>(T[] data)
        {
            if (data.Length > 0)
            {
                if (data[0].GetType().BaseType == typeof(Enum))
                {
                    string[] text = new string[data.Length];
                    for (int a = 0; a < data.Length; a++)
                        text[a] = Enum.GetName(data[a].GetType(), data[a]);
                    return text;
                }
                LogError("[] ToString<T> is capturing a type that it isn't set up for!!! " + typeof(T));
            }
            return new string[0];
        }
        /*
         *                                                                                                           
         *    888888888888  ,ad8888ba,          888888888888  88888888888  8b        d8  888888888888  
         *         88      d8"'    `"8b              88       88            Y8,    ,8P        88       
         *         88     d8'        `8b             88       88             `8b  d8'         88       
         *         88     88          88             88       88aaaaa          Y88P           88       
         *         88     88          88             88       88"""""          d88b           88       
         *         88     Y8,        ,8P             88       88             ,8P  Y8,         88       
         *         88      Y8a.    .a8P              88       88            d8'    `8b        88       
         *         88       `"Y8888Y"'               88       88888888888  8P        Y8       88       
         *
         *   Converts input AtO type to corresponding DataText type.
         */
        public static CardDataText ToText(CardData data)
        {
            CardDataText text = new();
            text.ID = data.Id;
            text.AcEnergyBonus = ToString(data.AcEnergyBonus);
            text.AcEnergyBonus2 = ToString(data.AcEnergyBonus2);
            text.AcEnergyBonusQuantity = data.AcEnergyBonusQuantity;
            text.AcEnergyBonus2Quantity = data.AcEnergyBonus2Quantity;
            text.AddCard = data.AddCard;
            text.AddCardChoose = data.AddCardChoose;
            text.AddCardCostTurn = data.AddCardCostTurn;
            text.AddCardFrom = ToString(data.AddCardFrom);
            text.AddCardId = data.AddCardId;
            text.AddCardList = ToString(data.AddCardList);
            text.AddCardPlace = ToString(data.AddCardPlace);
            text.AddCardReducedCost = data.AddCardReducedCost;
            text.AddCardType = ToString(data.AddCardType);
            text.AddCardTypeAux = ToString(data.AddCardTypeAux);
            text.AddCardVanish = data.AddCardVanish;
            text.Aura = ToString(data.Aura);
            text.Aura2 = ToString(data.Aura2);
            text.Aura3 = ToString(data.Aura3);
            text.AuraCharges = data.AuraCharges;
            text.AuraChargesSpecialValue1 = data.AuraChargesSpecialValue1;
            text.AuraChargesSpecialValue2 = data.AuraChargesSpecialValue2;
            text.AuraChargesSpecialValueGlobal = data.AuraChargesSpecialValueGlobal;
            text.AuraCharges2 = data.AuraCharges2;
            text.AuraCharges2SpecialValue1 = data.AuraCharges2SpecialValue1;
            text.AuraCharges2SpecialValue2 = data.AuraCharges2SpecialValue2;
            text.AuraCharges2SpecialValueGlobal = data.AuraCharges2SpecialValueGlobal;
            text.AuraCharges3 = data.AuraCharges3;
            text.AuraCharges3SpecialValue1 = data.AuraCharges3SpecialValue1;
            text.AuraCharges3SpecialValue2 = data.AuraCharges3SpecialValue2;
            text.AuraCharges3SpecialValueGlobal = data.AuraCharges3SpecialValueGlobal;
            text.AuraSelf = ToString(data.AuraSelf);
            text.AuraSelf2 = ToString(data.AuraSelf2);
            text.AuraSelf3 = ToString(data.AuraSelf3);
            text.AutoplayDraw = data.AutoplayDraw;
            text.AutoplayEndTurn = data.AutoplayEndTurn;
            text.BaseCard = data.BaseCard;
            text.CardClass = ToString(data.CardClass);
            text.CardName = data.CardName;
            text.CardNumber = data.CardNumber;
            text.CardRarity = ToString(data.CardRarity);
            text.CardType = ToString(data.CardType);
            text.CardTypeAux = ToString(data.CardTypeAux);
            /* CardTypeList is built from CardType and CardTypeAux when GetCardTypes() is called, so we don't need to use it. */
            text.CardUpgraded = ToString(data.CardUpgraded);
            text.Corrupted = data.Corrupted;
            text.Curse = ToString(data.Curse);
            text.Curse2 = ToString(data.Curse2);
            text.Curse3 = ToString(data.Curse3);
            text.CurseCharges = data.CurseCharges;
            text.CurseChargesSpecialValue1 = data.CurseChargesSpecialValue1;
            text.CurseChargesSpecialValue2 = data.CurseChargesSpecialValue2;
            text.CurseChargesSpecialValueGlobal = data.CurseChargesSpecialValueGlobal;
            text.CurseCharges2 = data.CurseCharges2;
            text.CurseCharges2SpecialValue1 = data.CurseCharges2SpecialValue1;
            text.CurseCharges2SpecialValue2 = data.CurseCharges2SpecialValue2;
            text.CurseCharges2SpecialValueGlobal = data.CurseCharges2SpecialValueGlobal;
            text.CurseCharges3 = data.CurseCharges3;
            text.CurseCharges3SpecialValue1 = data.CurseCharges3SpecialValue1;
            text.CurseCharges3SpecialValue2 = data.CurseCharges3SpecialValue2;
            text.CurseCharges3SpecialValueGlobal = data.CurseCharges3SpecialValueGlobal;
            text.CurseSelf = ToString(data.CurseSelf);
            text.CurseSelf2 = ToString(data.CurseSelf2);
            text.CurseSelf3 = ToString(data.CurseSelf3);
            text.Damage = data.Damage;
            text.DamageSpecialValue1 = data.DamageSpecialValue1;
            text.DamageSpecialValue2 = data.DamageSpecialValue2;
            text.DamageSpecialValueGlobal = data.DamageSpecialValueGlobal;
            text.Damage2 = data.Damage2;
            text.Damage2SpecialValue1 = data.Damage2SpecialValue1;
            text.Damage2SpecialValue2 = data.Damage2SpecialValue2;
            text.Damage2SpecialValueGlobal = data.Damage2SpecialValueGlobal;
            text.DamageEnergyBonus = data.DamageEnergyBonus;
            text.DamageSelf = data.DamageSelf;
            text.DamageSelf2 = data.DamageSelf2;
            text.DamageSides = data.DamageSides;
            text.DamageSides2 = data.DamageSides2;
            text.DamageType = ToString(data.DamageType);
            text.DamageType2 = ToString(data.DamageType2);
            text.Description = data.Description;
            // text.DescriptionID = data.descriptionid
            text.DiscardCard = data.DiscardCard;
            text.DiscardCardAutomatic = data.DiscardCardAutomatic;
            text.DiscardCardPlace = ToString(data.DiscardCardPlace);
            text.DiscardCardType = ToString(data.DiscardCardType);
            text.DiscardCardTypeAux = ToString(data.DiscardCardTypeAux);
            text.DispelAuras = data.DispelAuras;
            text.DrawCard = data.DrawCard;
            text.EffectCastCenter = data.EffectCastCenter;
            text.EffectCaster = data.EffectCaster;
            text.EffectCasterRepeat = data.EffectCasterRepeat;
            text.EffectPostCastDelay = data.EffectPostCastDelay;
            text.EffectPostTargetDelay = data.EffectPostTargetDelay;
            text.EffectPreAction = data.EffectPreAction;
            text.EffectRepeat = data.EffectRepeat;
            text.EffectRepeatDelay = data.EffectRepeatDelay;
            text.EffectRepeatEnergyBonus = data.EffectRepeatEnergyBonus;
            text.EffectRepeatMaxBonus = data.EffectRepeatMaxBonus;
            text.EffectRepeatModificator = data.EffectRepeatModificator;
            text.EffectRepeatTarget = ToString(data.EffectRepeatTarget);
            text.EffectRequired = data.EffectRequired;
            text.EffectTarget = data.EffectTarget;
            text.EffectTrail = data.EffectTrail;
            text.EffectTrailAngle = ToString(data.EffectTrailAngle);
            text.EffectTrailRepeat = data.EffectTrailRepeat;
            text.EffectTrailSpeed = data.EffectTrailSpeed;
            text.EndTurn = data.EndTurn;
            text.EnergyCost = data.EnergyCost;
            text.EnergyCostForShow = data.EnergyCostForShow;
            text.EnergyRecharge = data.EnergyRecharge;
            text.EnergyReductionPermanent = data.EnergyReductionPermanent;
            text.EnergyReductionTemporal = data.EnergyReductionTemporal;
            text.EnergyReductionToZeroPermanent = data.EnergyReductionToZeroPermanent;
            text.EnergyReductionToZeroTemporal = data.EnergyReductionToZeroTemporal;
            text.ExhaustCounter = data.ExhaustCounter;
            text.FlipSprite = data.FlipSprite;
            text.Fluff = data.Fluff;
            text.FluffPercent = data.FluffPercent;
            text.GoldGainQuantity = data.GoldGainQuantity;
            text.Heal = data.Heal;
            text.HealAuraCurseName = ToString(data.HealAuraCurseName);
            text.HealAuraCurseName2 = ToString(data.HealAuraCurseName2);
            text.HealAuraCurseName3 = ToString(data.HealAuraCurseName3);
            text.HealAuraCurseName4 = ToString(data.HealAuraCurseName4);
            text.HealAuraCurseSelf = ToString(data.HealAuraCurseSelf);
            text.HealCurses = data.HealCurses;
            text.HealEnergyBonus = data.HealEnergyBonus;
            text.HealSelf = data.HealSelf;
            text.HealSelfPerDamageDonePercent = data.HealSelfPerDamageDonePercent;
            text.HealSelfSpecialValue1 = data.HealSelfSpecialValue1;
            text.HealSelfSpecialValue2 = data.HealSelfSpecialValue2;
            text.HealSelfSpecialValueGlobal = data.HealSelfSpecialValueGlobal;
            text.HealSides = data.HealSides;
            text.HealSpecialValue1 = data.HealSpecialValue1;
            text.HealSpecialValue2 = data.HealSpecialValue2;
            text.HealSpecialValueGlobal = data.HealSpecialValueGlobal;
            text.IgnoreBlock = data.IgnoreBlock;
            text.IgnoreBlock2 = data.IgnoreBlock2;
            text.IncreaseAuras = data.IncreaseAuras;
            text.IncreaseCurses = data.IncreaseCurses;
            text.Innate = data.Innate;
            text.IsPetAttack = data.IsPetAttack;
            text.IsPetCast = data.IsPetCast;
            text.Item = "";
            if (data.Item != null)
                text.Item = ToJson(ToText(data.Item), true);
            text.ItemEnchantment = "";
            if (data.ItemEnchantment != null)
                text.ItemEnchantment = ToJson(ToText(data.ItemEnchantment), true);
            text.KillPet = data.KillPet;
            text.Lazy = data.Lazy;
            text.LookCards = data.LookCards;
            text.LookCardsDiscardUpTo = data.LookCardsDiscardUpTo;
            text.LookCardsVanishUpTo = data.LookCardsVanishUpTo;
            text.MaxInDeck = data.MaxInDeck;
            text.ModifiedByTrait = data.ModifiedByTrait;
            text.SelfKillHiddenSeconds = data.SelfKillHiddenSeconds;
            text.EnergyRechargeSpecialValueGlobal = data.EnergyRechargeSpecialValueGlobal;
            text.SelfKillHiddenSeconds = data.SelfKillHiddenSeconds;
            text.MoveToCenter = data.MoveToCenter;
            text.OnlyInWeekly = data.OnlyInWeekly;
            text.PetFront = data.PetFront;
            text.PetInvert = data.PetInvert;
            text.PetModel = ToString(data.PetModel);
            text.PetOffset = ToString(data.PetOffset);
            text.PetSize = ToString(data.PetSize);
            text.Playable = data.Playable;
            text.PullTarget = data.PullTarget;
            text.PushTarget = data.PushTarget;
            text.ReduceAuras = data.ReduceAuras;
            text.ReduceCurses = data.ReduceCurses;
            text.RelatedCard = data.RelatedCard;
            text.RelatedCard2 = data.RelatedCard2;
            text.RelatedCard3 = data.RelatedCard3;
            text.SelfHealthLoss = data.SelfHealthLoss;
            text.SelfHealthLossSpecialGlobal = data.SelfHealthLossSpecialGlobal;
            text.SelfHealthLossSpecialValue1 = data.SelfHealthLossSpecialValue1;
            text.SelfHealthLossSpecialValue2 = data.SelfHealthLossSpecialValue2;
            text.ShardsGainQuantity = data.ShardsGainQuantity;
            text.ShowInTome = data.ShowInTome;
            text.Sku = data.Sku;
            text.Sound = ToString(data.Sound);
            text.SoundPreAction = ToString(data.SoundPreAction);
            text.SoundPreActionFemale = ToString(data.SoundPreActionFemale);
            text.SpecialAuraCurseName1 = ToString(data.SpecialAuraCurseName1);
            text.SpecialAuraCurseName2 = ToString(data.SpecialAuraCurseName2);
            text.SpecialAuraCurseNameGlobal = ToString(data.SpecialAuraCurseNameGlobal);
            text.SpecialValue1 = ToString(data.SpecialValue1);
            text.SpecialValue2 = ToString(data.SpecialValue2);
            text.SpecialValueGlobal = ToString(data.SpecialValueGlobal);
            text.SpecialValueModifier1 = data.SpecialValueModifier1;
            text.SpecialValueModifier2 = data.SpecialValueModifier2;
            text.SpecialValueModifierGlobal = data.SpecialValueModifierGlobal;
            text.Sprite = ToString(data.Sprite);
            text.Starter = data.Starter;
            text.StealAuras = data.StealAuras;
            text.SummonAura = ToString(data.SummonAura);
            text.SummonAura2 = ToString(data.SummonAura2);
            text.SummonAura3 = ToString(data.SummonAura3);
            text.SummonAuraCharges = data.SummonAuraCharges;
            text.SummonAuraCharges2 = data.SummonAuraCharges2;
            text.SummonAuraCharges3 = data.SummonAuraCharges3;
            text.SummonUnit = ToString(data.SummonUnit);
            text.SummonUnitNum = data.SummonUnitNum;
            text.TargetPosition = ToString(data.TargetPosition);
            text.TargetSide = ToString(data.TargetSide);
            text.TargetType = ToString(data.TargetType);
            text.TransferCurses = data.TransferCurses;
            text.UpgradedFrom = data.UpgradedFrom;
            text.UpgradesTo1 = data.UpgradesTo1;
            text.UpgradesTo2 = data.UpgradesTo2;
            text.UpgradesToRare = ToString(data.UpgradesToRare);
            text.Vanish = data.Vanish;
            text.Visible = data.Visible;
            if (data.Sprite != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.Sprite, "card", text.CardClass);
            return text;
        }

        public static TraitDataText ToText(TraitData data)
        {
            TraitDataText text = new();
            text.ID = data.Id;
            text.Activation = ToString(data.Activation);
            text.AuraCurseBonus1 = ToString(data.AuracurseBonus1);
            text.AuraCurseBonus2 = ToString(data.AuracurseBonus2);
            text.AuraCurseBonus3 = ToString(data.AuracurseBonus3);
            text.AuraCurseBonusValue1 = data.AuracurseBonusValue1;
            text.AuraCurseBonusValue2 = data.AuracurseBonusValue2;
            text.AuraCurseBonusValue3 = data.AuracurseBonusValue3;
            text.AuraCurseImmune1 = data.AuracurseImmune1;
            text.AuraCurseImmune2 = data.AuracurseImmune2;
            text.AuraCurseImmune3 = data.AuracurseImmune3;
            text.CharacterStatModified = ToString(data.CharacterStatModified);
            text.CharacterStatModifiedValue = data.CharacterStatModifiedValue;
            text.DamageBonusFlat = ToString(data.DamageBonusFlat);
            text.DamageBonusFlat2 = ToString(data.DamageBonusFlat2);
            text.DamageBonusFlat3 = ToString(data.DamageBonusFlat3);
            text.DamageBonusFlatValue = data.DamageBonusFlatValue;
            text.DamageBonusFlatValue2 = data.DamageBonusFlatValue2;
            text.DamageBonusFlatValue3 = data.DamageBonusFlatValue3;
            text.DamageBonusPercent = ToString(data.DamageBonusPercent);
            text.DamageBonusPercent2 = ToString(data.DamageBonusPercent2);
            text.DamageBonusPercent3 = ToString(data.DamageBonusPercent3);
            text.DamageBonusPercentValue = data.DamageBonusPercentValue;
            text.DamageBonusPercentValue2 = data.DamageBonusPercentValue2;
            text.DamageBonusPercentValue3 = data.DamageBonusPercentValue3;
            text.Description = data.Description;
            text.HealFlatBonus = data.HealFlatBonus;
            text.HealPercentBonus = data.HealPercentBonus;
            text.HealReceivedFlatBonus = data.HealReceivedFlatBonus;
            text.HealReceivedPercentBonus = data.HealReceivedPercentBonus;
            text.ResistModified1 = ToString(data.ResistModified1);
            text.ResistModified2 = ToString(data.ResistModified2);
            text.ResistModified3 = ToString(data.ResistModified3);
            text.ResistModifiedValue1 = data.ResistModifiedValue1;
            text.ResistModifiedValue2 = data.ResistModifiedValue2;
            text.ResistModifiedValue3 = data.ResistModifiedValue3;
            text.TimesPerRound = data.TimesPerRound;
            text.TimesPerTurn = data.TimesPerTurn;
            text.TraitCard = ToString(data.TraitCard);
            text.TraitCardForAllHeroes = ToString(data.TraitCardForAllHeroes);
            text.TraitName = data.TraitName;
            return text;
        }

        public static SubClassDataText ToText(SubClassData data)
        {
            SubClassDataText text = new();
            text.ID = data.Id;
            text.ActionSound = ToString(data.ActionSound); // surprised this didn't break in 1.4.0?
            text.Cards = ToString(data.Cards);
            text.ChallengePack0 = ToString(data.ChallengePack0);
            text.ChallengePack1 = ToString(data.ChallengePack1);
            text.ChallengePack2 = ToString(data.ChallengePack2);
            text.ChallengePack3 = ToString(data.ChallengePack3);
            text.ChallengePack4 = ToString(data.ChallengePack4);
            text.ChallengePack5 = ToString(data.ChallengePack5);
            text.ChallengePack6 = ToString(data.ChallengePack6);
            text.CharacterDescription = data.CharacterDescription;
            text.CharacterDescriptionStrength = data.CharacterDescriptionStrength;
            text.CharacterName = data.CharacterName;
            text.Energy = data.Energy;
            text.EnergyTurn = data.EnergyTurn;
            // removed in 1.4.0     text.ExpansionCharacter = data.ExpansionCharacter;
            text.Female = data.Female;
            text.FluffOffsetX = data.FluffOffsetX;
            text.FluffOffsetY = data.FluffOffsetY;
            //text.GameObjectAnimated = ToString(data.GameObjectAnimated);
            text.HeroClass = ToString(data.HeroClass);
            text.HeroClassSecondary = ToString(data.HeroClassSecondary);
            text.HitSound = ToString(data.GetHitSound());
            text.HP = data.Hp;
            text.MaxHP = data.MaxHp;
            text.OrderInList = data.OrderInList;
            text.Item = ToString(data.Item);
            text.ResistSlashing = data.ResistSlashing;
            text.ResistBlunt = data.ResistBlunt;
            text.ResistPiercing = data.ResistPiercing;
            text.ResistFire = data.ResistFire;
            text.ResistCold = data.ResistCold;
            text.ResistLightning = data.ResistLightning;
            text.ResistHoly = data.ResistHoly;
            text.ResistShadow = data.ResistShadow;
            text.ResistMind = data.ResistMind;
            text.Speed = data.Speed;
            //text.Sprite = ToString(data.Sprite);
            //text.SpriteBorder = ToString(data.SpriteBorder);
            text.SpriteBorderLocked = ToString(data.SpriteBorderLocked);
            //text.SpriteBorderSmall = ToString(data.SpriteBorderSmall);
            //text.SpritePortrait = ToString(data.SpritePortrait);
            //text.SpriteSpeed = ToString(data.SpriteSpeed);
            text.StickerAngry = ToString(data.StickerAngry);
            text.StickerBase = ToString(data.StickerBase);
            text.StickerIndifferent = ToString(data.StickerIndiferent);
            text.StickerLove = ToString(data.StickerLove);
            text.StickerSurprise = ToString(data.StickerSurprise);
            text.StickerOffsetX = data.StickerOffsetX;
            text.SubclassName = data.SubClassName;
            text.Trait0 = ToString(data.Trait0);
            text.Trait1A = ToString(data.Trait1A);
            text.Trait1B = ToString(data.Trait1B);
            text.Trait2A = ToString(data.Trait2A);
            text.Trait2B = ToString(data.Trait2B);
            text.Trait3A = ToString(data.Trait3A);
            text.Trait3B = ToString(data.Trait3B);
            text.Trait4A = ToString(data.Trait4A);
            text.Trait4B = ToString(data.Trait4B);
            if (medsExportJSON.Value && medsExportSprites.Value)
            {
                if (data.SpriteBorderLocked != null)
                    ExportSprite(data.SpriteBorderLocked, "subclass", data.Id);
                /*
                if (data.GameObjectAnimated != null)
                    foreach (SpriteRenderer SR in data.GameObjectAnimated.GetComponentsInChildren<SpriteRenderer>(true))
                        if (SR.sprite != null)
                            ExportSprite(data.Sprite, "subclass", data.Id, "GameObject");
                if (data.Sprite != null)
                    ExportSprite(data.Sprite, "subclass", data.Id, "Sprite");
                if (data.SpriteBorder != null)
                    ExportSprite(data.SpriteBorder, "subclass", data.Id, "SpriteBorder");
                if (data.SpriteBorderSmall != null)
                    ExportSprite(data.SpriteBorderSmall, "subclass", data.Id, "SpriteBorderSmall");
                if (data.SpritePortrait != null)
                    ExportSprite(data.SpritePortrait, "subclass", data.Id, "SpritePortrait");*/
                if (data.SpriteSpeed != null)
                    ExportSprite(data.SpriteSpeed, "subclass", data.Id);
                if (data.StickerAngry != null)
                    ExportSprite(data.StickerAngry, "subclass", data.Id);
                if (data.StickerBase != null)
                    ExportSprite(data.StickerBase, "subclass", data.Id);
                if (data.StickerIndiferent != null)
                    ExportSprite(data.StickerIndiferent, "subclass", data.Id);
                if (data.StickerLove != null)
                    ExportSprite(data.StickerLove, "subclass", data.Id);
                if (data.StickerSurprise != null)
                    ExportSprite(data.StickerSurprise, "subclass", data.Id);
            }
            text.AutoUnlock = data.InitialUnlock; // #XMAS
            text.SourceCharacterName = data.SourceCharacterName;// #XMAS
            return text;
        }

        public static CardDataText[] ToText(CardData[] data)
        {
            CardDataText[] text = new CardDataText[data.Length];
            for (int a = 0; a < data.Length; a++)
                text[a] = ToText(data[a]);
            return text;
        }

        public static HeroCardsText ToText(HeroCards data)
        {
            HeroCardsText text = new();
            text.UnitsInDeck = data.UnitsInDeck;
            text.Card = ToString(data.Card);
            return text;
        }

        public static ItemDataText ToText(ItemData data)
        {
            ItemDataText text = new();
            text.ACG1MultiplyByEnergyUsed = data.Acg1MultiplyByEnergyUsed;
            text.ACG2MultiplyByEnergyUsed = data.Acg2MultiplyByEnergyUsed;
            text.ACG3MultiplyByEnergyUsed = data.Acg3MultiplyByEnergyUsed;
            text.Activation = ToString(data.Activation);
            text.ActivationOnlyOnHeroes = data.ActivationOnlyOnHeroes;
            text.AuraCurseBonus1 = ToString(data.AuracurseBonus1);
            text.AuraCurseBonus2 = ToString(data.AuracurseBonus2);
            text.AuraCurseBonusValue1 = data.AuracurseBonusValue1;
            text.AuraCurseBonusValue2 = data.AuracurseBonusValue2;
            text.AuraCurseCustomAC = ToString(data.AuracurseCustomAC);
            text.AuraCurseCustomModValue1 = data.AuracurseCustomModValue1;
            text.AuraCurseCustomModValue2 = data.AuracurseCustomModValue2;
            text.AuraCurseCustomString = data.AuracurseCustomString;
            text.AuraCurseGain1 = ToString(data.AuracurseGain1);
            text.AuraCurseGain2 = ToString(data.AuracurseGain2);
            text.AuraCurseGain3 = ToString(data.AuracurseGain3);
            text.AuraCurseGainValue1 = data.AuracurseGainValue1;
            text.AuraCurseGainValue2 = data.AuracurseGainValue2;
            text.AuraCurseGainValue3 = data.AuracurseGainValue3;
            text.AuraCurseGainSelf1 = ToString(data.AuracurseGainSelf1);
            text.AuraCurseGainSelf2 = ToString(data.AuracurseGainSelf2);
            text.AuraCurseGainSelfValue1 = data.AuracurseGainSelfValue1;
            text.AuraCurseGainSelfValue2 = data.AuracurseGainSelfValue2;
            text.AuraCurseImmune1 = ToString(data.AuracurseImmune1);
            text.AuraCurseImmune2 = ToString(data.AuracurseImmune2);
            text.AuraCurseNumForOneEvent = data.AuraCurseNumForOneEvent;
            text.AuraCurseSetted = ToString(data.AuraCurseSetted);
            text.CardNum = data.CardNum;
            text.CardPlace = ToString(data.CardPlace);
            text.CardsReduced = data.CardsReduced;
            text.CardToGain = ToString(data.CardToGain);
            text.CardToGainList = ToString(data.CardToGainList.ToArray());
            text.CardToGainType = ToString(data.CardToGainType);
            text.CardToReduceType = ToString(data.CardToReduceType);
            text.CastedCardType = ToString(data.CastedCardType);
            text.CastEnchantmentOnFinishSelfCast = data.CastEnchantmentOnFinishSelfCast;
            text.ChanceToDispel = data.ChanceToDispel;
            text.ChanceToDispelNum = data.ChanceToDispelNum;
            text.CharacterStatModified = ToString(data.CharacterStatModified);
            text.CharacterStatModified2 = ToString(data.CharacterStatModified2);
            text.CharacterStatModified3 = ToString(data.CharacterStatModified3);
            text.CharacterStatModifiedValue = data.CharacterStatModifiedValue;
            text.CharacterStatModifiedValue2 = data.CharacterStatModifiedValue2;
            text.CharacterStatModifiedValue3 = data.CharacterStatModifiedValue3;
            text.CostReducePermanent = data.CostReducePermanent;
            text.CostReduceReduction = data.CostReduceReduction;
            text.CostReduceEnergyRequirement = data.CostReduceEnergyRequirement;
            text.CostReduction = data.CostReduction;
            text.CostZero = data.CostZero;
            text.CursedItem = data.CursedItem;
            text.DamageFlatBonus = ToString(data.DamageFlatBonus);
            text.DamageFlatBonus2 = ToString(data.DamageFlatBonus2);
            text.DamageFlatBonus3 = ToString(data.DamageFlatBonus3);
            text.DamageFlatBonusValue = data.DamageFlatBonusValue;
            text.DamageFlatBonusValue2 = data.DamageFlatBonusValue2;
            text.DamageFlatBonusValue3 = data.DamageFlatBonusValue3;
            text.DamagePercentBonus = ToString(data.DamagePercentBonus);
            text.DamagePercentBonus2 = ToString(data.DamagePercentBonus2);
            text.DamagePercentBonus3 = ToString(data.DamagePercentBonus3);
            text.DamagePercentBonusValue = data.DamagePercentBonusValue;
            text.DamagePercentBonusValue2 = data.DamagePercentBonusValue2;
            text.DamagePercentBonusValue3 = data.DamagePercentBonusValue3;
            text.DamageToTarget = data.DamageToTarget;
            text.DamageToTargetType = ToString(data.DamageToTargetType);
            text.DestroyAfterUse = data.DestroyAfterUse;
            text.DestroyAfterUses = data.DestroyAfterUses;
            text.DestroyEndOfTurn = data.DestroyEndOfTurn;
            text.DestroyStartOfTurn = data.DestroyStartOfTurn;
            text.DrawCards = data.DrawCards;
            text.DrawMultiplyByEnergyUsed = data.DrawMultiplyByEnergyUsed;
            text.DropOnly = data.DropOnly;
            text.DTTMultiplyByEnergyUsed = data.DttMultiplyByEnergyUsed;
            text.DuplicateActive = data.DuplicateActive;
            text.EffectCaster = data.EffectCaster;
            text.EffectItemOwner = data.EffectItemOwner;
            text.EffectTarget = data.EffectTarget;
            text.EffectCasterDelay = data.EffectCasterDelay;
            text.EffectTargetDelay = data.EffectTargetDelay;
            text.EmptyHand = data.EmptyHand;
            text.EnergyQuantity = data.EnergyQuantity;
            text.ExactRound = data.ExactRound;
            text.HealFlatBonus = data.HealFlatBonus;
            text.HealPercentBonus = data.HealPercentBonus;
            text.HealPercentQuantity = data.HealPercentQuantity;
            text.HealPercentQuantitySelf = data.HealPercentQuantitySelf;
            text.HealQuantity = data.HealQuantity;
            text.HealReceivedFlatBonus = data.HealReceivedFlatBonus;
            text.HealReceivedPercentBonus = data.HealReceivedPercentBonus;
            text.ID = data.Id;
            text.IsEnchantment = data.IsEnchantment;
            text.ItemSound = ToString(data.ItemSound);
            text.ItemTarget = ToString(data.ItemTarget);
            text.LowerOrEqualPercentHP = data.LowerOrEqualPercentHP;
            text.MaxHealth = data.MaxHealth;
            text.ModifiedDamageType = ToString(data.ModifiedDamageType);
            text.NotShowCharacterBonus = data.NotShowCharacterBonus;
            text.OnlyAddItemToNPCs = data.OnlyAddItemToNPCs;
            text.PassSingleAndCharacterRolls = data.PassSingleAndCharacterRolls;
            text.PercentDiscountShop = data.PercentDiscountShop;
            text.PercentRetentionEndGame = data.PercentRetentionEndGame;
            text.Permanent = data.Permanent;
            text.QuestItem = data.QuestItem;
            text.ReduceHighestCost = data.ReduceHighestCost;
            text.ResistModified1 = ToString(data.ResistModified1);
            text.ResistModified2 = ToString(data.ResistModified2);
            text.ResistModified3 = ToString(data.ResistModified3);
            text.ResistModifiedValue1 = data.ResistModifiedValue1;
            text.ResistModifiedValue2 = data.ResistModifiedValue2;
            text.ResistModifiedValue3 = data.ResistModifiedValue3;
            text.RoundCycle = data.RoundCycle;
            text.SpriteBossDrop = ToString(data.SpriteBossDrop);
            text.TimesPerCombat = data.TimesPerCombat;
            text.TimesPerTurn = data.TimesPerTurn;
            text.UsedEnergy = data.UsedEnergy;
            text.UseTheNextInsteadWhenYouPlay = data.UseTheNextInsteadWhenYouPlay;
            text.Vanish = data.Vanish;
            if (data.SpriteBossDrop != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.SpriteBossDrop, "item", "SpriteBossDrop");
            return text;
        }

        public static AICardsText ToText(AICards data)
        {
            AICardsText text = new();
            text.AddCardRound = data.AddCardRound;
            text.AuraCurseCastIf = ToString(data.AuracurseCastIf);
            text.Card = ToString(data.Card);
            text.OnlyCastIf = ToString(data.OnlyCastIf);
            text.PercentToCast = data.PercentToCast;
            text.Priority = data.Priority;
            text.TargetCast = ToString(data.TargetCast);
            text.UnitsInDeck = data.UnitsInDeck;
            text.ValueCastIf = data.ValueCastIf;
            return text;
        }
        public static NPCDataText ToText(NPCData data)
        {
            NPCDataText text = new();
            text.AICards = ToString(data.AICards);
            text.AuraCurseImmune = data.AuracurseImmune.ToArray();
            text.BaseMonster = ToString(data.BaseMonster);
            text.BigModel = data.BigModel;
            text.CardsInHand = data.CardsInHand;
            text.Description = data.Description;
            text.Difficulty = data.Difficulty;
            text.Energy = data.Energy;
            text.EnergyTurn = data.EnergyTurn;
            text.ExperienceReward = data.ExperienceReward;
            text.Female = data.Female;
            text.FinishCombatOnDead = data.FinishCombatOnDead;
            text.FluffOffsetX = data.FluffOffsetX;
            text.FluffOffsetY = data.FluffOffsetY;
            text.GameObjectAnimated = ToString(data.GameObjectAnimated);
            text.GoldReward = data.GoldReward;
            text.HellModeMob = ToString(data.HellModeMob);
            //#TODO #BUG needs update for 1.4.0     text.HitSound = ToString(data.HitSound);
            text.HP = data.Hp;
            text.ID = data.Id;
            text.IsBoss = data.IsBoss;
            text.IsNamed = data.IsNamed;
            text.NgPlusMob = ToString(data.NgPlusMob);
            text.NPCName = data.NPCName;
            text.PosBottom = data.PosBottom;
            text.PreferredPosition = ToString(data.PreferredPosition);
            text.ResistBlunt = data.ResistBlunt;
            text.ResistCold = data.ResistCold;
            text.ResistFire = data.ResistFire;
            text.ResistHoly = data.ResistHoly;
            text.ResistLightning = data.ResistLightning;
            text.ResistMind = data.ResistMind;
            text.ResistPiercing = data.ResistPiercing;
            text.ResistShadow = data.ResistShadow;
            text.ResistSlashing = data.ResistSlashing;
            text.ScriptableObjectName = data.ScriptableObjectName;
            text.Speed = data.Speed;
            text.Sprite = ToString(data.Sprite);
            text.SpritePortrait = ToString(data.SpritePortrait);
            text.SpriteSpeed = ToString(data.SpriteSpeed);
            text.TierMob = ToString(data.TierMob);
            text.TierReward = ToString(data.TierReward);
            text.UpgradedMob = ToString(data.UpgradedMob);
            if (medsExportJSON.Value && medsExportSprites.Value)
            {
                LogDebug("exporting NPC: " + text.ID);
                if (data.GameObjectAnimated != null)
                    foreach (SpriteRenderer SR in data.GameObjectAnimated.GetComponentsInChildren<SpriteRenderer>(true))
                        if (SR.sprite != null)
                            ExportSprite(SR.sprite, "NPC", data.Id, "", true);
                if (data.Sprite != null)
                    ExportSprite(data.Sprite, "NPC", data.Id);
                if (data.SpritePortrait != null)
                    ExportSprite(data.SpritePortrait, "NPC", data.Id);
                if (data.SpriteSpeed != null)
                    ExportSprite(data.SpriteSpeed, "NPC", data.Id);
            }
            return text;
        }
        public static PerkDataText ToText(PerkData data)
        {
            PerkDataText text = new();
            text.AdditionalCurrency = data.AdditionalCurrency;
            text.AdditionalShards = data.AdditionalShards;
            text.AuraCurseBonus = ToString(data.AuracurseBonus);
            text.AuraCurseBonusValue = data.AuracurseBonusValue;
            text.CardClass = ToString(data.CardClass);
            text.CustomDescription = data.CustomDescription;
            text.DamageFlatBonus = ToString(data.DamageFlatBonus);
            text.DamageFlatBonusValue = data.DamageFlatBonusValue;
            text.EnergyBegin = data.EnergyBegin;
            text.HealQuantity = data.HealQuantity;
            text.Icon = ToString(data.Icon);
            text.IconTextValue = data.IconTextValue;
            text.ID = data.Id;
            text.Level = data.Level;
            text.MainPerk = data.MainPerk;
            text.MaxHealth = data.MaxHealth;
            text.ObeliskPerk = data.ObeliskPerk;
            text.ResistModified = ToString(data.ResistModified);
            text.ResistModifiedValue = data.ResistModifiedValue;
            text.Row = data.Row;
            text.SpeedQuantity = data.SpeedQuantity;
            if (data.Icon != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.Icon, "perk");
            return text;
        }
        public static AuraCurseDataText ToText(AuraCurseData data)
        {
            AuraCurseDataText text = new();
            text.ACName = data.ACName;
            text.AuraConsumed = data.AuraConsumed;
            text.AuraDamageIncreasedPercent = data.AuraDamageIncreasedPercent;
            text.AuraDamageIncreasedPercent2 = data.AuraDamageIncreasedPercent2;
            text.AuraDamageIncreasedPercent3 = data.AuraDamageIncreasedPercent3;
            text.AuraDamageIncreasedPercent4 = data.AuraDamageIncreasedPercent4;
            text.AuraDamageIncreasedPercentPerStack = data.AuraDamageIncreasedPercentPerStack;
            text.AuraDamageIncreasedPercentPerStack2 = data.AuraDamageIncreasedPercentPerStack2;
            text.AuraDamageIncreasedPercentPerStack3 = data.AuraDamageIncreasedPercentPerStack3;
            text.AuraDamageIncreasedPercentPerStack4 = data.AuraDamageIncreasedPercentPerStack4;
            text.AuraDamageIncreasedPercentPerStackPerEnergy = data.AuraDamageIncreasedPercentPerStackPerEnergy;
            text.AuraDamageIncreasedPercentPerStackPerEnergy2 = data.AuraDamageIncreasedPercentPerStackPerEnergy2;
            text.AuraDamageIncreasedPercentPerStackPerEnergy3 = data.AuraDamageIncreasedPercentPerStackPerEnergy3;
            text.AuraDamageIncreasedPercentPerStackPerEnergy4 = data.AuraDamageIncreasedPercentPerStackPerEnergy4;
            text.AuraDamageIncreasedPerStack = data.AuraDamageIncreasedPerStack;
            text.AuraDamageIncreasedPerStack2 = data.AuraDamageIncreasedPerStack2;
            text.AuraDamageIncreasedPerStack3 = data.AuraDamageIncreasedPerStack3;
            text.AuraDamageIncreasedPerStack4 = data.AuraDamageIncreasedPerStack4;
            text.AuraDamageIncreasedTotal = data.AuraDamageIncreasedTotal;
            text.AuraDamageIncreasedTotal2 = data.AuraDamageIncreasedTotal2;
            text.AuraDamageIncreasedTotal3 = data.AuraDamageIncreasedTotal3;
            text.AuraDamageIncreasedTotal4 = data.AuraDamageIncreasedTotal4;
            text.AuraDamageType = ToString(data.AuraDamageType);
            text.AuraDamageType2 = ToString(data.AuraDamageType2);
            text.AuraDamageType3 = ToString(data.AuraDamageType3);
            text.AuraDamageType4 = ToString(data.AuraDamageType4);
            text.AuraDamageChargesBasedOnACCharges = ToString(data.AuraDamageChargesBasedOnACCharges);
            text.ConsumedDamageChargesBasedOnACCharges = ToString(data.ConsumedDamageChargesBasedOnACCharges);
            text.BlockChargesGainedPerStack = data.BlockChargesGainedPerStack;
            text.CardsDrawPerStack = data.CardsDrawPerStack;
            text.CharacterStatAbsolute = data.CharacterStatAbsolute;
            text.CharacterStatAbsoluteValue = data.CharacterStatAbsoluteValue;
            text.CharacterStatAbsoluteValuePerStack = data.CharacterStatAbsoluteValuePerStack;
            text.CharacterStatChargesMultiplierNeededForOne = data.CharacterStatChargesMultiplierNeededForOne;
            text.CharacterStatModified = ToString(data.CharacterStatModified);
            text.CharacterStatModifiedValue = data.CharacterStatModifiedValue;
            text.CharacterStatModifiedValuePerStack = data.CharacterStatModifiedValuePerStack;
            text.ChargesAuxNeedForOne1 = data.ChargesAuxNeedForOne1;
            text.ChargesAuxNeedForOne2 = data.ChargesAuxNeedForOne2;
            text.ChargesMultiplierDescription = data.ChargesMultiplierDescription;
            text.CombatlogShow = data.CombatlogShow;
            text.ConsumeAll = data.ConsumeAll;
            text.ConsumedAtCast = data.ConsumedAtCast;
            text.ConsumedAtRound = data.ConsumedAtRound;
            text.ConsumedAtRoundBegin = data.ConsumedAtRoundBegin;
            text.ConsumedAtTurn = data.ConsumedAtTurn;
            text.ConsumedAtTurnBegin = data.ConsumedAtTurnBegin;
            text.CursePreventedPerStack = data.CursePreventedPerStack;
            text.DamagePreventedPerStack = data.DamagePreventedPerStack;
            text.DamageReflectedConsumeCharges = data.DamageReflectedConsumeCharges;
            text.DamageReflectedType = ToString(data.DamageReflectedType);
            text.DamageSidesWhenConsumed = data.DamageSidesWhenConsumed;
            text.DamageSidesWhenConsumedPerCharge = data.DamageSidesWhenConsumedPerCharge;
            text.DamageTypeWhenConsumed = ToString(data.DamageTypeWhenConsumed);
            text.DamageWhenConsumed = data.DamageWhenConsumed;
            text.DamageWhenConsumedPerCharge = data.DamageWhenConsumedPerCharge;
            text.Description = data.Description;
            text.DieWhenConsumedAll = data.DieWhenConsumedAll;
            text.DisabledCardTypes = ToString(data.DisabledCardTypes);
            text.DoubleDamageIfCursesLessThan = data.DoubleDamageIfCursesLessThan;
            text.EffectTick = data.EffectTick;
            text.EffectTickSides = data.EffectTickSides;
            text.ExplodeAtStacks = data.ExplodeAtStacks;
            text.GainAuraCurseConsumption = ToString(data.GainAuraCurseConsumption);
            text.GainAuraCurseConsumption2 = ToString(data.GainAuraCurseConsumption2);
            text.GainAuraCurseConsumptionPerCharge = data.GainAuraCurseConsumptionPerCharge;
            text.GainAuraCurseConsumptionPerCharge2 = data.GainAuraCurseConsumptionPerCharge2;
            text.GainCharges = data.GainCharges;
            text.GainChargesFromThisAuraCurse = ToString(data.GainChargesFromThisAuraCurse);
            text.GainChargesFromThisAuraCurse2 = ToString(data.GainChargesFromThisAuraCurse2);
            text.HealAttackerConsumeCharges = data.HealAttackerConsumeCharges;
            text.HealAttackerPerStack = data.HealAttackerPerStack;
            text.HealDonePercent = data.HealDonePercent;
            text.HealDonePercentPerStack = data.HealDonePercentPerStack;
            text.HealDonePercentPerStackPerEnergy = data.HealDonePercentPerStackPerEnergy;
            text.HealDonePerStack = data.HealDonePerStack;
            text.HealReceivedTotal = data.HealReceivedTotal;
            text.HealSidesWhenConsumed = data.HealSidesWhenConsumed;
            text.HealSidesWhenConsumedPerCharge = data.HealSidesWhenConsumedPerCharge;
            text.HealWhenConsumed = data.HealWhenConsumed;
            text.HealWhenConsumedPerCharge = data.HealWhenConsumedPerCharge;
            text.IconShow = data.IconShow;
            text.ID = data.Id;
            text.IncreasedDamageReceivedType = ToString(data.IncreasedDamageReceivedType);
            text.IncreasedDamageReceivedType2 = ToString(data.IncreasedDamageReceivedType2);
            text.IncreasedDirectDamageChargesMultiplierNeededForOne = data.IncreasedDirectDamageChargesMultiplierNeededForOne;
            text.IncreasedDirectDamageChargesMultiplierNeededForOne2 = data.IncreasedDirectDamageChargesMultiplierNeededForOne2;
            text.IncreasedDirectDamageReceivedPerStack = (int) data.IncreasedDirectDamageReceivedPerStack;
            text.IncreasedDirectDamageReceivedPerStack2 = (int) data.IncreasedDirectDamageReceivedPerStack2;
            text.IncreasedDirectDamageReceivedPerTurn = data.IncreasedDirectDamageReceivedPerTurn;
            text.IncreasedDirectDamageReceivedPerTurn2 = data.IncreasedDirectDamageReceivedPerTurn2;
            text.IncreasedPercentDamageReceivedPerStack = data.IncreasedPercentDamageReceivedPerStack;
            text.IncreasedPercentDamageReceivedPerStack2 = data.IncreasedPercentDamageReceivedPerStack2;
            text.IncreasedPercentDamageReceivedPerTurn = data.IncreasedPercentDamageReceivedPerTurn;
            text.IncreasedPercentDamageReceivedPerTurn2 = data.IncreasedPercentDamageReceivedPerTurn2;
            text.Invulnerable = data.Invulnerable;
            text.IsAura = data.IsAura;
            text.MaxCharges = data.MaxCharges;
            text.MaxMadnessCharges = data.MaxMadnessCharges;
            text.ModifyCardCostPerChargeNeededForOne = data.ModifyCardCostPerChargeNeededForOne;
            text.NoRemoveBlockAtTurnEnd = data.NoRemoveBlockAtTurnEnd;
            text.Preventable = data.Preventable;
            text.PreventedAuraCurse = ToString(data.PreventedAuraCurse);
            text.PreventedAuraCurseStackPerStack = data.PreventedAuraCurseStackPerStack;
            text.PreventedDamagePerStack = data.PreventedDamagePerStack;
            text.PreventedDamageTypePerStack = ToString(data.PreventedDamageTypePerStack);
            text.PriorityOnConsumption = data.PriorityOnConsumption;
            text.ProduceDamageWhenConsumed = data.ProduceDamageWhenConsumed;
            text.ProduceHealWhenConsumed = data.ProduceHealWhenConsumed;
            text.Removable = data.Removable;
            text.RemoveAuraCurse = ToString(data.RemoveAuraCurse);
            text.RemoveAuraCurse2 = ToString(data.RemoveAuraCurse2);
            text.ResistModified = ToString(data.ResistModified);
            text.ResistModified2 = ToString(data.ResistModified2);
            text.ResistModified3 = ToString(data.ResistModified3);
            text.ResistModifiedPercentagePerStack = data.ResistModifiedPercentagePerStack;
            text.ResistModifiedPercentagePerStack2 = data.ResistModifiedPercentagePerStack2;
            text.ResistModifiedPercentagePerStack3 = data.ResistModifiedPercentagePerStack3;
            text.ResistModifiedValue = data.ResistModifiedValue;
            text.ResistModifiedValue2 = data.ResistModifiedValue2;
            text.ResistModifiedValue3 = data.ResistModifiedValue3;
            text.RevealCardsPerCharge = data.RevealCardsPerCharge;
            text.SkipsNextTurn = data.SkipsNextTurn;
            text.Sound = ToString(data.Sound);
            text.Sprite = ToString(data.Sprite);
            text.Stealth = data.Stealth;
            text.Taunt = data.Taunt;
            if (data.Sprite != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.Sprite, "auraCurse");
            return text;
        }
        public static NodeDataText ToText(NodeData data)
        {
            NodeDataText text = new();
            text.CombatPercent = data.CombatPercent;
            text.Description = data.Description;
            text.DisableCorruption = data.DisableCorruption;
            text.DisableRandom = data.DisableRandom;
            text.EventPercent = data.EventPercent;
            text.ExistsPercent = data.ExistsPercent;
            text.ExistsSku = data.ExistsSku;
            text.GoToTown = data.GoToTown;
            text.NodeBackgroundImg = ToString(data.NodeBackgroundImg);
            text.NodeCombat = ToString(data.NodeCombat);
            text.NodeCombatTier = ToString(data.NodeCombatTier);
            text.NodeEvent = ToString(data.NodeEvent);
            text.NodeEventPercent = data.NodeEventPercent;
            text.NodeEventPriority = data.NodeEventPriority;
            text.NodeEventTier = ToString(data.NodeEventTier);
            text.NodeGround = ToString(data.NodeGround);
            text.NodeId = data.NodeId;
            text.NodeName = data.NodeName;
            text.NodeRequirement = ToString(data.NodeRequirement);
            text.NodesConnected = ToString(data.NodesConnected);
            text.NodesConnectedRequirement = ToString(data.NodesConnectedRequirement);
            text.NodeZone = ToString(data.NodeZone);
            text.TravelDestination = data.TravelDestination;
            text.VisibleIfNotRequirement = data.VisibleIfNotRequirement;
            text.SourceNodeName = data.SourceNodeName; // #XMAS
            if (data.NodeBackgroundImg != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.NodeBackgroundImg, "node");
            return text;
        }
        public static KeyNotesDataText ToText(KeyNotesData data)
        {
            KeyNotesDataText text = new();
            text.ID = data.Id;
            text.KeynoteName = data.KeynoteName;
            text.Description = data.Description;
            text.DescriptionExtended = data.DescriptionExtended;
            return text;
        }
        public static LootDataText ToText(LootData data)
        {
            LootDataText text = new();
            text.DefaultPercentEpic = data.DefaultPercentEpic;
            text.DefaultPercentMythic = data.DefaultPercentMythic;
            text.DefaultPercentRare = data.DefaultPercentRare;
            text.DefaultPercentUncommon = data.DefaultPercentUncommon;
            text.GoldQuantity = data.GoldQuantity;
            text.ID = data.Id;
            text.LootItemTable = ToString(data.LootItemTable);
            text.NumItems = data.NumItems;
            text.ShadyModel = ToString(data.ShadyModel); // #XMAS
            text.ShadyScaleX = data.ShadyScaleX; // #XMAS
            text.ShadyScaleY = data.ShadyScaleY; // #XMAS
            text.ShadyOffsetX = data.ShadyOffsetX; // #XMAS
            text.ShadyOffsetY = data.ShadyOffsetY; // #XMAS
            return text;
        }
        public static LootItemText ToText(LootItem data)
        {
            LootItemText text = new();
            text.LootCard = ToString(data.LootCard);
            text.LootPercent = data.LootPercent;
            text.LootRarity = ToString(data.LootRarity);
            text.LootType = ToString(data.LootType);
            return text;
        }
        public static PerkNodeDataText ToText(PerkNodeData data)
        {
            PerkNodeDataText text = new();
            text.Column = data.Column;
            text.Cost = ToString(data.Cost);
            text.ID = data.Id;
            text.LockedInTown = data.LockedInTown;
            text.NotStack = data.NotStack;
            text.Perk = ToString(data.Perk);
            text.PerkRequired = ToString(data.PerkRequired);
            text.PerksConnected = ToString(data.PerksConnected);
            text.Row = data.Row;
            text.Sprite = ToString(data.Sprite);
            text.Type = data.Type;
            if (data.Sprite != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.Sprite, "perkNode");
            return text;
        }
        public static ChallengeDataText ToText(ChallengeData data)
        {
            ChallengeDataText text = new();
            text.Boss1 = ToString(data.Boss1);
            text.Boss2 = ToString(data.Boss2);
            text.BossCombat = ToString(data.BossCombat);
            text.CorruptionList = ToString(data.CorruptionList.ToArray());
            text.Hero1 = ToString(data.Hero1);
            text.Hero2 = ToString(data.Hero2);
            text.Hero3 = ToString(data.Hero3);
            text.Hero4 = ToString(data.Hero4);
            text.ID = data.Id;
            text.IDSteam = data.IdSteam;
            text.Loot = ToString(data.Loot);
            text.Seed = data.Seed;
            text.Traits = ToString(data.Traits.ToArray());
            text.Week = data.Week;
            return text;
        }
        public static ChallengeTraitText ToText(ChallengeTrait data)
        {
            ChallengeTraitText text = new();
            text.Icon = ToString(data.Icon);
            text.ID = data.Id;
            text.IsMadnessTrait = data.IsMadnessTrait;
            text.Name = data.Name;
            text.Order = data.Order;
            if (data.Icon != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.Icon, "challengeTrait");
            return text;
        }
        public static CombatDataText ToText(CombatData data)
        {
            CombatDataText text = new();
            text.CinematicData = ToString(data.CinematicData);
            text.CombatBackground = ToString(data.CombatBackground);
            text.CombatEffect = ToString(data.CombatEffect);
            text.CombatID = data.CombatId;
            text.CombatMusic = ToString(data.CombatMusic);
            text.CombatTier = ToString(data.CombatTier);
            text.Description = data.Description;
            text.EventData = ToString(data.EventData);
            text.EventRequirementData = ToString(data.EventRequirementData);
            text.HealHeroes = data.HealHeroes;
            text.NPCList = ToString(data.NPCList);
            text.NPCRemoveInMadness0Index = data.NpcRemoveInMadness0Index;
            text.ThermometerTierData = ToString(data.ThermometerTierData);
            text.IsRift = data.IsRift; // #XMAS
            return text;
        }
        public static CombatEffectText ToText(CombatEffect data)
        {
            CombatEffectText text = new();
            text.AuraCurse = ToString(data.AuraCurse);
            text.AuraCurseCharges = data.AuraCurseCharges;
            text.AuraCurseTarget = ToString(data.AuraCurseTarget);
            return text;
        }
        public static EventDataText ToText(EventData data)
        {
            EventDataText text = new();
            text.medsNode = medsNodeEvent.ContainsKey(data.EventId) ? medsNodeEvent[data.EventId] : "";
            text.medsPercent = medsNodeEventPercent.ContainsKey(data.EventId) ? medsNodeEventPercent[data.EventId] : 100;
            text.medsPriority = medsNodeEventPriority.ContainsKey(data.EventId) ? medsNodeEventPriority[data.EventId] : 0;
            text.Description = data.Description;
            text.DescriptionAction = data.DescriptionAction;
            text.EventIconShader = ToString(data.EventIconShader);
            text.EventID = data.EventId;
            text.EventName = data.EventName;
            text.EventSpriteBook = ToString(data.EventSpriteBook);
            text.EventSpriteDecor = ToString(data.EventSpriteDecor);
            text.EventSpriteMap = ToString(data.EventSpriteMap);
            text.EventTier = ToString(data.EventTier);
            text.EventUniqueID = data.EventUniqueId;
            text.HistoryMode = data.HistoryMode;
            text.ReplyRandom = data.ReplyRandom;
            text.Replies = new string[data.Replys.Length];
            for (int a = 0; a < data.Replys.Length; a++)
            {
                medsEventReplyDataText[text.EventID + "_" + a.ToString()] = ToText(data.Replys[a], text.EventID, a);
                //text.Replies[a] = text.EventID + "_" + a.ToString();
                text.Replies[a] = ToJson(medsEventReplyDataText[text.EventID + "_" + a.ToString()], true);
            }
            text.RequiredClass = ToString(data.RequiredClass);
            text.Requirement = ToString(data.Requirement);
            if (medsExportJSON.Value && medsExportSprites.Value)
            {
                if (data.EventSpriteBook != null)
                    ExportSprite(data.EventSpriteBook, "event", "EventSpriteBook");
                if (data.EventSpriteDecor != null)
                    ExportSprite(data.EventSpriteDecor, "event", "EventSpriteDecor");
                if (data.EventSpriteMap != null)
                    ExportSprite(data.EventSpriteMap, "event", "EventSpriteMap");
            }
            return text;
        }
        public static EventRequirementDataText ToText(EventRequirementData data)
        {
            EventRequirementDataText text = new();
            text.RequirementID = data.RequirementId;
            text.RequirementName = data.RequirementName;
            text.AssignToPlayerAtBegin = data.AssignToPlayerAtBegin;
            text.Description = data.Description;
            text.ItemSprite = ToString(data.ItemSprite);
            text.RequirementTrack = data.RequirementTrack;
            text.TrackSprite = ToString(data.TrackSprite);
            text.ItemTrack = data.ItemTrack;
            // get requirementzonefinishtrack with reflections #TODO: find out if there's some benefit to using traditional reflections vs harmony traverse.create??
            text.RequirementZoneFinishTrack = ToString(Traverse.Create(data).Field("requirementZoneFinishTrack").GetValue<Enums.Zone>());
            if (medsExportJSON.Value && medsExportSprites.Value)
            {
                if (data.ItemSprite != null)
                    ExportSprite(data.ItemSprite, "eventRequirement", "ItemSprite");
                if (data.TrackSprite != null)
                    ExportSprite(data.TrackSprite, "eventRequirement", "TrackSprite");
            }
            return text;
        }
        public static EventReplyDataText ToText(EventReplyData data, string medsEvent = "", int a = -1)
        {
            EventReplyDataText text = new();
            text.medsEvent = medsEvent;
            if (a != -1)
                text.medsTempID = medsEvent + "_" + a.ToString();
            text.DustCost = data.DustCost;
            text.GoldCost = data.GoldCost;
            text.IndexForAnswerTranslation = data.IndexForAnswerTranslation;
            text.RepeatForAllCharacters = data.RepeatForAllCharacters;
            text.ReplyActionText = ToString(data.ReplyActionText);
            text.ReplyShowCard = ToString(data.ReplyShowCard);
            text.ReplyText = data.ReplyText;
            text.RequiredClass = ToString(data.RequiredClass);
            text.Requirement = ToString(data.Requirement);
            text.RequirementBlocked = ToString(data.RequirementBlocked);
            text.RequirementCard = ToString(data.RequirementCard.ToArray());
            text.RequirementItem = ToString(data.RequirementItem);
            text.RequirementMultiplayer = data.RequirementMultiplayer;
            text.RequirementSku = data.RequirementSku;

            text.SSAddCard1 = ToString(data.SsAddCard1);
            text.SSAddCard2 = ToString(data.SsAddCard2);
            text.SSAddCard3 = ToString(data.SsAddCard3);
            text.SSAddItem = ToString(data.SsAddItem);
            text.SSCardPlayerGame = data.SsCardPlayerGame;
            text.SSCardPlayerGamePackData = ToString(data.SsCardPlayerGamePackData);
            text.SSCardPlayerPairsGame = data.SsCardPlayerPairsGame;
            text.SSCardPlayerPairsGamePackData = ToString(data.SsCardPlayerPairsGamePackData);
            text.SSCharacterReplacement = ToString(data.SsCharacterReplacement);
            text.SSCharacterReplacementPosition = data.SsCharacterReplacementPosition;
            text.SSCombat = ToString(data.SsCombat);
            text.SSCorruptionUI = data.SsCorruptionUI;
            text.SSCorruptItemSlot = ToString(data.SsCorruptItemSlot);
            text.SSCraftUI = data.SsCraftUI;
            text.SSCraftUIMaxType = ToString(data.SsCraftUIMaxType);
            text.SSDiscount = data.SsDiscount;
            text.SSDustReward = data.SsDustReward;
            text.SSEvent = ToString(data.SsEvent);
            text.SSExperienceReward = data.SsExperienceReward;
            text.SSFinishEarlyAccess = data.SsFinishEarlyAccess;
            text.SSFinishGame = data.SsFinishGame;
            text.SSFinishObeliskMap = data.SsFinishObeliskMap;
            text.SSGoldReward = data.SsGoldReward;
            text.SSHealerUI = data.SsHealerUI;
            text.SSLootList = ToString(data.SsLootList);
            text.SSMaxQuantity = data.SsMaxQuantity;
            text.SSMerchantUI = data.SsMerchantUI;
            text.SSNodeTravel = ToString(data.SsNodeTravel);
            text.SSPerkData = ToString(data.SsPerkData);
            text.SSPerkData1 = ToString(data.SsPerkData1);
            text.SSRemoveItemSlot = ToString(data.SsRemoveItemSlot);
            text.SSRequirementLock = ToString(data.SsRequirementLock);
            text.SSRequirementLock2 = ToString(data.SsRequirementLock2);
            text.SSRequirementUnlock = ToString(data.SsRequirementUnlock);
            text.SSRequirementUnlock2 = ToString(data.SsRequirementUnlock2);
            text.SSRewardHealthFlat = data.SsRewardHealthFlat;
            text.SSRewardHealthPercent = data.SsRewardHealthPercent;
            text.SSRewardText = data.SsRewardText;
            text.SSRewardTier = ToString(data.SsRewardTier);
            text.SSRoll = data.SsRoll;
            text.SSRollCard = ToString(data.SsRollCard);
            text.SSRollMode = ToString(data.SsRollMode);
            text.SSRollNumber = data.SsRollNumber;
            text.SSRollNumberCritical = data.SsRollNumberCritical;
            text.SSRollNumberCriticalFail = data.SsRollNumberCriticalFail;
            text.SSRollTarget = ToString(data.SsRollTarget);
            text.SSShopList = ToString(data.SsShopList);
            text.SSSteamStat = data.SsSteamStat;
            text.SSSupplyReward = data.SsSupplyReward;
            text.SSUnlockClass = ToString(data.SsUnlockClass);
            text.SSUnlockSkin = ToString(data.SsUnlockSkin);
            text.SSUnlockSteamAchievement = data.SsUnlockSteamAchievement;
            text.SSUpgradeRandomCard = data.SsUpgradeRandomCard;
            text.SSUpgradeUI = data.SsUpgradeUI;

            text.SSCAddCard1 = ToString(data.SscAddCard1);
            text.SSCAddCard2 = ToString(data.SscAddCard2);
            text.SSCAddCard3 = ToString(data.SscAddCard3);
            text.SSCAddItem = ToString(data.SscAddItem);
            text.SSCCardPlayerGame = data.SscCardPlayerGame;
            text.SSCCardPlayerGamePackData = ToString(data.SscCardPlayerGamePackData);
            text.SSCCardPlayerPairsGame = data.SscCardPlayerPairsGame;
            text.SSCCardPlayerPairsGamePackData = ToString(data.SscCardPlayerPairsGamePackData);
            text.SSCCombat = ToString(data.SscCombat);
            text.SSCCorruptionUI = data.SscCorruptionUI;
            text.SSCCorruptItemSlot = ToString(data.SscCorruptItemSlot);
            text.SSCCraftUI = data.SscCraftUI;
            text.SSCCraftUIMaxType = ToString(data.SscCraftUIMaxType);
            text.SSCDiscount = data.SscDiscount;
            text.SSCDustReward = data.SscDustReward;
            text.SSCEvent = ToString(data.SscEvent);
            text.SSCExperienceReward = data.SscExperienceReward;
            text.SSCFinishEarlyAccess = data.SscFinishEarlyAccess;
            text.SSCFinishGame = data.SscFinishGame;
            text.SSCGoldReward = data.SscGoldReward;
            text.SSCHealerUI = data.SscHealerUI;
            text.SSCLootList = ToString(data.SscLootList);
            text.SSCMaxQuantity = data.SscMaxQuantity;
            text.SSCMerchantUI = data.SscMerchantUI;
            text.SSCNodeTravel = ToString(data.SscNodeTravel);
            text.SSCRemoveItemSlot = ToString(data.SscRemoveItemSlot);
            text.SSCRequirementLock = ToString(data.SscRequirementLock);
            text.SSCRequirementUnlock = ToString(data.SscRequirementUnlock);
            text.SSCRequirementUnlock2 = ToString(data.SscRequirementUnlock2);
            text.SSCRewardHealthFlat = data.SscRewardHealthFlat;
            text.SSCRewardHealthPercent = data.SscRewardHealthPercent;
            text.SSCRewardText = data.SscRewardText;
            text.SSCRewardTier = ToString(data.SscRewardTier);
            text.SSCShopList = ToString(data.SscShopList);
            text.SSCSupplyReward = data.SscSupplyReward;
            text.SSCUnlockClass = ToString(data.SscUnlockClass);
            text.SSCUnlockSteamAchievement = data.SscUnlockSteamAchievement;
            text.SSCUpgradeRandomCard = data.SscUpgradeRandomCard;
            text.SSCUpgradeUI = data.SscUpgradeUI;
            text.FLAddCard1 = ToString(data.FlAddCard1);
            text.FLAddCard2 = ToString(data.FlAddCard2);
            text.FLAddCard3 = ToString(data.FlAddCard3);
            text.FLAddItem = ToString(data.FlAddItem);
            text.FLCardPlayerGame = data.FlCardPlayerGame;
            text.FLCardPlayerGamePackData = ToString(data.FlCardPlayerGamePackData);
            text.FLCardPlayerPairsGame = data.FlCardPlayerPairsGame;
            text.FLCardPlayerPairsGamePackData = ToString(data.FlCardPlayerPairsGamePackData);
            text.FLCombat = ToString(data.FlCombat);
            text.FLCorruptionUI = data.FlCorruptionUI;
            text.FLCorruptItemSlot = ToString(data.FlCorruptItemSlot);
            text.FLCraftUI = data.FlCraftUI;
            text.FLCraftUIMaxType = ToString(data.FlCraftUIMaxType);
            text.FLDiscount = data.FlDiscount;
            text.FLDustReward = data.FlDustReward;
            text.FLEvent = ToString(data.FlEvent);
            text.FLExperienceReward = data.FlExperienceReward;
            text.FLGoldReward = data.FlGoldReward;
            text.FLHealerUI = data.FlHealerUI;
            text.FLLootList = ToString(data.FlLootList);
            text.FLMaxQuantity = data.FlMaxQuantity;
            text.FLMerchantUI = data.FlMerchantUI;
            text.FLNodeTravel = ToString(data.FlNodeTravel);
            text.FLRemoveItemSlot = ToString(data.FlRemoveItemSlot);
            text.FLRequirementLock = ToString(data.FlRequirementLock);
            text.FLRequirementUnlock = ToString(data.FlRequirementUnlock);
            text.FLRequirementUnlock2 = ToString(data.FlRequirementUnlock2);
            text.FLRewardHealthFlat = data.FlRewardHealthFlat;
            text.FLRewardHealthPercent = data.FlRewardHealthPercent;
            text.FLRewardText = data.FlRewardText;
            text.FLRewardTier = ToString(data.FlRewardTier);
            text.FLShopList = ToString(data.FlShopList);
            text.FLSupplyReward = data.FlSupplyReward;
            text.FLUnlockClass = ToString(data.FlUnlockClass);
            text.FLUnlockSteamAchievement = data.FlUnlockSteamAchievement;
            text.FLUpgradeRandomCard = data.FlUpgradeRandomCard;
            text.FLUpgradeUI = data.FlUpgradeUI;

            text.FLCAddCard1 = ToString(data.FlcAddCard1);
            text.FLCAddCard2 = ToString(data.FlcAddCard2);
            text.FLCAddCard3 = ToString(data.FlcAddCard3);
            text.FLCAddItem = ToString(data.FlcAddItem);
            text.FLCCardPlayerGame = data.FlcCardPlayerGame;
            text.FLCCardPlayerGamePackData = ToString(data.FlcCardPlayerGamePackData);
            text.FLCCardPlayerPairsGame = data.FlcCardPlayerPairsGame;
            text.FLCCardPlayerPairsGamePackData = ToString(data.FlcCardPlayerPairsGamePackData);
            text.FLCCombat = ToString(data.FlcCombat);
            text.FLCCorruptionUI = data.FlcCorruptionUI;
            text.FLCCorruptItemSlot = ToString(data.FlcCorruptItemSlot);
            text.FLCCraftUI = data.FlcCraftUI;
            text.FLCCraftUIMaxType = ToString(data.FlcCraftUIMaxType);
            text.FLCDiscount = data.FlcDiscount;
            text.FLCDustReward = data.FlcDustReward;
            text.FLCEvent = ToString(data.FlcEvent);
            text.FLCExperienceReward = data.FlcExperienceReward;
            text.FLCGoldReward = data.FlcGoldReward;
            text.FLCHealerUI = data.FlcHealerUI;
            text.FLCLootList = ToString(data.FlcLootList);
            text.FLCMaxQuantity = data.FlcMaxQuantity;
            text.FLCMerchantUI = data.FlcMerchantUI;
            text.FLCNodeTravel = ToString(data.FlcNodeTravel);
            text.FLCRemoveItemSlot = ToString(data.FlcRemoveItemSlot);
            text.FLCRequirementLock = ToString(data.FlcRequirementLock);
            text.FLCRequirementUnlock = ToString(data.FlcRequirementUnlock);
            text.FLCRequirementUnlock2 = ToString(data.FlcRequirementUnlock2);
            text.FLCRewardHealthFlat = data.FlcRewardHealthFlat;
            text.FLCRewardHealthPercent = data.FlcRewardHealthPercent;
            text.FLCRewardText = data.FlcRewardText;
            text.FLCRewardTier = ToString(data.FlcRewardTier);
            text.FLCShopList = ToString(data.FlcShopList);
            text.FLCSupplyReward = data.FlcSupplyReward;
            text.FLCUnlockClass = ToString(data.FlcUnlockClass);
            text.FLCUnlockSteamAchievement = data.FlcUnlockSteamAchievement;
            text.FLCUpgradeRandomCard = data.FlcUpgradeRandomCard;
            text.FLCUpgradeUI = data.FlcUpgradeUI;

            return text;
        }
        public static ZoneDataText ToText(ZoneData data)
        {
            ZoneDataText text = new();
            text.ChangeTeamOnEntrance = data.ChangeTeamOnEntrance;
            text.DisableExperienceOnThisZone = data.DisableExperienceOnThisZone;
            text.DisableMadnessOnThisZone = data.DisableMadnessOnThisZone;
            text.NewTeam = ToString(data.NewTeam.ToArray());
            text.ObeliskFinal = data.ObeliskFinal;
            text.ObeliskHigh = data.ObeliskHigh;
            text.ObeliskLow = data.ObeliskLow;
            text.RestoreTeamOnExit = data.RestoreTeamOnExit;
            text.ZoneID = data.ZoneId;
            text.ZoneName = data.ZoneName;
            return text;
        }
        public static PackDataText ToText(PackData data)
        {
            PackDataText text = new();
            text.Card0 = ToString(data.Card0);
            text.Card1 = ToString(data.Card1);
            text.Card2 = ToString(data.Card2);
            text.Card3 = ToString(data.Card3);
            text.Card4 = ToString(data.Card4);
            text.Card5 = ToString(data.Card5);
            text.CardSpecial0 = ToString(data.CardSpecial0);
            text.CardSpecial1 = ToString(data.CardSpecial1);
            text.PackClass = ToString(data.PackClass);
            text.PackID = data.PackId;
            text.PackName = data.PackName;
            text.PerkList = ToString(data.PerkList.ToArray());
            text.RequiredClass = ToString(data.RequiredClass);
            return text;
        }
        public static NodesConnectedRequirementText ToText(NodesConnectedRequirement data)
        {
            NodesConnectedRequirementText text = new();
            text.NodeData = ToString(data.NodeData);
            text.ConnectionRequirement = ToString(data.ConectionRequeriment);
            text.ConnectionIfNotNode = ToString(data.ConectionIfNotNode);
            return text;
        }
        public static CorruptionPackDataText ToText(CorruptionPackData data)
        {
            CorruptionPackDataText text = new();
            text.HighPack = ToString(data.HighPack.ToArray());
            text.LowPack = ToString(data.LowPack.ToArray());
            text.PackClass = ToString(data.PackClass);
            text.PackName = data.PackName;
            text.PackTier = data.PackTier;
            return text;
        }
        public static CardPlayerPackDataText ToText(CardPlayerPackData data)
        {
            CardPlayerPackDataText text = new();
            text.Card0 = ToString(data.Card0);
            text.Card0RandomBoon = data.Card0RandomBoon;
            text.Card0RandomInjury = data.Card0RandomInjury;
            text.Card1 = ToString(data.Card1);
            text.Card1RandomBoon = data.Card1RandomBoon;
            text.Card1RandomInjury = data.Card1RandomInjury;
            text.Card2 = ToString(data.Card2);
            text.Card2RandomBoon = data.Card2RandomBoon;
            text.Card2RandomInjury = data.Card2RandomInjury;
            text.Card3 = ToString(data.Card3);
            text.Card3RandomBoon = data.Card3RandomBoon;
            text.Card3RandomInjury = data.Card3RandomInjury;
            text.ModIterations = data.ModIterations;
            text.ModSpeed = data.ModSpeed;
            text.PackId = data.PackId;
            return text;
        }
        public static CardPlayerPairsPackDataText ToText(CardPlayerPairsPackData data)
        {
            CardPlayerPairsPackDataText text = new();
            text.PackId = data.PackId;
            text.Card0 = ToString(data.Card0);
            text.Card1 = ToString(data.Card1);
            text.Card2 = ToString(data.Card2);
            text.Card3 = ToString(data.Card3);
            text.Card4 = ToString(data.Card4);
            text.Card5 = ToString(data.Card5);
            return text;
        }
        public static CardbackDataText ToText(CardbackData data)
        {
            CardbackDataText text = new();
            text.AdventureLevel = data.AdventureLevel;
            text.BaseCardback = data.BaseCardback;
            text.CardbackID = data.CardbackId;
            text.CardbackName = data.CardbackName;
            text.CardbackOrder = data.CardbackOrder;
            text.CardbackSprite = ToString(data.CardbackSprite);
            text.CardbackSubclass = ToString(data.CardbackSubclass);
            text.Locked = data.Locked;
            text.ObeliskLevel = data.ObeliskLevel;
            text.RankLevel = data.RankLevel;
            text.ShowIfLocked = data.ShowIfLocked;
            text.Sku = data.Sku;
            text.SteamStat = data.SteamStat;
            if (data.CardbackSprite != null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.CardbackSprite, "cardback");
            return text;
        }
        public static SkinDataText ToText(SkinData data)
        {
            SkinDataText text = new();
            text.BaseSkin = data.BaseSkin;
            text.PerkLevel = data.PerkLevel;
            text.SkinGO = ToString(data.SkinGo);
            text.SkinID = data.SkinId;
            text.SkinName = data.SkinName;
            text.SkinOrder = data.SkinOrder;
            text.SkinSubclass = ToString(data.SkinSubclass);
            text.Sku = data.Sku;
            text.SpritePortrait = ToString(data.SpritePortrait);
            text.SpritePortraitGrande = ToString(data.SpritePortraitGrande);
            text.SpriteSilueta = ToString(data.SpriteSilueta);
            text.SpriteSiluetaGrande = ToString(data.SpriteSiluetaGrande);
            text.SteamStat = data.SteamStat;
            if (medsExportJSON.Value && medsExportSprites.Value)
            {
                if (data.SkinGo != null)
                    foreach (SpriteRenderer SR in data.SkinGo.GetComponentsInChildren<SpriteRenderer>(true))
                        if (SR.sprite != null)
                            ExportSprite(SR.sprite, "skin", data.SkinId, "", true);
                if (data.SpritePortrait != null)
                    ExportSprite(data.SpritePortrait, "skin", data.SkinId);
                if (data.SpritePortraitGrande != null)
                    ExportSprite(data.SpritePortraitGrande, "skin", data.SkinId);
                if (data.SpriteSilueta != null)
                    ExportSprite(data.SpriteSilueta, "skin", data.SkinId);
                if (data.SpriteSiluetaGrande != null)
                    ExportSprite(data.SpriteSiluetaGrande, "skin", data.SkinId);
            }
            return text;
        }
        public static CinematicDataText ToText(CinematicData data)
        {
            CinematicDataText text = new();
            text.CinematicBSO = ToString(data.CinematicBSO);
            text.CinematicCombat = ToString(data.CinematicCombat);
            text.CinematicEndAdventure = data.CinematicEndAdventure;
            text.CinematicEvent = ToString(data.CinematicEvent);
            text.CinematicGo = ToString(data.CinematicGo);
            text.CinematicID = data.CinematicId;
            return text;
        }
        public static TierRewardDataText ToText(TierRewardData data)
        {
            TierRewardDataText text = new();
            text.tierNum = data.TierNum;
            text.common = data.Common;
            text.uncommon = data.Uncommon;
            text.rare = data.Rare;
            text.epic = data.Epic;
            text.mythic = data.Mythic;
            text.dust = data.Dust;
            return text;
        }


        // OTHER


        public static NodeDataText ToFULLText(NodeData data)
        {
            NodeDataText text = new();
            text.CombatPercent = data.CombatPercent;
            text.Description = data.Description;
            text.DisableCorruption = data.DisableCorruption;
            text.DisableRandom = data.DisableRandom;
            text.EventPercent = data.EventPercent;
            text.ExistsPercent = data.ExistsPercent;
            text.ExistsSku = data.ExistsSku;
            text.GoToTown = data.GoToTown;
            text.NodeBackgroundImg = ToString(data.NodeBackgroundImg);
            if ((UnityEngine.Object)data.NodeBackgroundImg != (UnityEngine.Object)null && medsExportJSON.Value && medsExportSprites.Value)
                ExportSprite(data.NodeBackgroundImg, "node");
            text.NodeCombat = ToString(data.NodeCombat);
            text.NodeCombatTier = ToString(data.NodeCombatTier);
            text.NodeEvent = new string[data.NodeEvent.Length];
            for (int a = 0; a < data.NodeEvent.Length; a++)
                text.NodeEvent[a] = ToJson(ToText(data.NodeEvent[a]));
            text.NodeEventPercent = data.NodeEventPercent;
            text.NodeEventPriority = data.NodeEventPriority;
            text.NodeEventTier = ToString(data.NodeEventTier);
            text.NodeGround = ToString(data.NodeGround);
            text.NodeId = data.NodeId;
            text.NodeName = data.NodeName;
            text.NodeRequirement = ToString(data.NodeRequirement);
            text.NodesConnected = ToString(data.NodesConnected);
            text.NodesConnectedRequirement = ToString(data.NodesConnectedRequirement);
            text.NodeZone = ToString(data.NodeZone);
            text.TravelDestination = data.TravelDestination;
            text.VisibleIfNotRequirement = data.VisibleIfNotRequirement;
            if (medsNodeSource.ContainsKey(data.NodeId))
            {
                text.medsPosX = medsNodeSource[data.NodeId].transform.position.x;
                text.medsPosY = medsNodeSource[data.NodeId].transform.position.y;
            }
            return text;
        }
        public static PlayerDeckDataText ToText(PlayerDeck data)
        {
            PlayerDeckDataText PDDT = new();
            foreach (string subclassID in data.DeckTitle.Keys)
            {
                HeroDeckDataText HDDT = new();
                HDDT.SubclassID = subclassID;
                for (int a = 0; a < data.DeckTitle[subclassID].Length; a++)
                {
                    SavedDeckDataText SDDT = new();
                    SDDT.Title = data.DeckTitle[subclassID][a];
                    SDDT.Cards = data.DeckCards[subclassID][a];
                    HDDT.SavedDecks.Add(SDDT);
                }
                PDDT.Heroes.Add(HDDT);
            }
            return PDDT;
        }
        public static PlayerPerkDataText ToText(PlayerPerk data)
        {
            PlayerPerkDataText PPDT = new();
            foreach (string subclassID in data.PerkConfigTitle.Keys)
            {
                HeroPerkDataText HPDT = new();
                HPDT.SubclassID = subclassID;
                for (int a = 0; a < data.PerkConfigTitle[subclassID].Length; a++)
                {
                    SavedPerkDataText SPDT = new();
                    SPDT.Title = data.PerkConfigTitle[subclassID][a];
                    SPDT.Perks = data.PerkConfigPerks[subclassID][a];
                    SPDT.Points = data.PerkConfigPoints[subclassID][a];
                }
                PPDT.Heroes.Add(HPDT);
            }
            return PPDT;
        }
    }
}