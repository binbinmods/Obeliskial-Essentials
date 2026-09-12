using System;
using HarmonyLib;
using UnityEngine;
using Cards;
using static Enums;
using static UnityEngine.Mathf;
using static Obeliskial_Essentials.Essentials;
using static Obeliskial_Essentials.CombatFunctions;

namespace Obeliskial_Essentials
{
    /// <summary>
    /// Unified GlobalAuraCurseModificationByTraitsAndItems postfix.
    /// Consolidated from AtO-Perk-Manager, Binbin Balances, and hero mods.
    /// </summary>
    [HarmonyPatch]
    internal class GACM
    {

        public static bool shackle1fFlag = false;
        public static int nInjuries = 0;

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModifyResist")]
        public static AuraCurseData GlobalAuraCurseModifyResist(
            AtOManager instance,
            AuraCurseData _acData,
            Enums.DamageType _damageType,
            int _resistModified,
            float _resistModifiedPercentage)
        {
            throw new NotImplementedException("Reverse patch stub for AtOManager.GlobalAuraCurseModifyResist");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModifyDamage")]
        public static AuraCurseData GlobalAuraCurseModifyDamage(
            AtOManager instance,
            AuraCurseData _acData,
            Enums.DamageType _damageType,
            int _damageModified,
            int _damageModifiedPerStack,
            int _damageModifiedPercentage)
        {
            throw new NotImplementedException("Reverse patch stub for AtOManager.GlobalAuraCurseModifyDamage");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), nameof(AtOManager.GlobalAuraCurseModificationByTraitsAndItems))]
        [HarmonyPriority(Priority.First)]
        public static void GACMPostfix(
            ref AtOManager __instance,
            ref AuraCurseData __result,
            string _type,
            string _acId,
            Character _characterCaster,
            Character _characterTarget)
        {
            if (__result == null)
                return;

            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;

            ApplyPerkGACM(ref __instance, ref __result, _type, _acId, characterOfInterest);
            ApplyBalanceGACM(ref __result, _acId, characterOfInterest);
            ApplyHeroTraitGACM(ref __instance, ref __result, _type, _acId, _characterCaster, _characterTarget, characterOfInterest);
        }

        public static void ApplyPerkGACM(
            ref AtOManager __instance,
            ref AuraCurseData __result,
            string _type,
            string _acId,
            Character characterOfInterest)
        {
            if (characterOfInterest != null)
            {
                bool hasRust = false;
                if (characterOfInterest != null)
                {
                    hasRust = characterOfInterest.HasEffect("rust");
                }

                float rustMultiplier = hasRust ? 1.5f : 1.0f;
                if (TeamHasPerk("mainperkrust0b") && hasRust && (_acId == "crack" || _acId == "poison" || _acId == "slow"))
                {
                    // rust0b: Rust on enemies instead increases the effect of Crack, Poison and Slow by 20% per charge
                    int nRust = characterOfInterest.GetAuraCharges("rust");
                    rustMultiplier = 1.0f + 0.2f * nRust;
                }

                switch (_acId)
                {


                    // leech0d: Charges applied +1. Decrease healing done by Leech by 50%.
                    // leech0e: Leech explodes at the end of turn.
                    // leech0f: Increase curses applied by Leech by 100%. Leech no longer reduces enemy resistances.
                    // leech0g: Rather than healing, when Leech explodes, it deals damage to all enemies equal to the target's Bleed.
                    case "leech":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "leech0d", AppliesTo.Heroes))
                        {
                            __result.HealPerChargeOnExplode *= 0.5f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "leech0g", AppliesTo.Heroes))
                        {
                            __result.HealPerChargeOnExplode = 0;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "leech0d", AppliesTo.Heroes))
                        {
                            __result.ACChargesPerStackChargeOnExplode *= 2;
                            __result.ResistModified = Enums.DamageType.None;
                            __result.ResistModifiedValue = 0;
                        }
                        break;

                    // infuse0d: Charges applied +1. Infuse on all heroes loses 3 charges per turn.
                    // infuse0e: Max. Infuse charges +4. Infuse no longer increases resistances.
                    // infuse0f: Infuse on heroes increases damage by 0.5 per Reinforce/Insulate/Courage charge rather than 1/Infuse charge.
                    // infuse0g: Infuse on heroes increases the effectiveness of Reinforce/Insulate/Courage by 15% per charge rather than 50%.
                    // infuse0h: Infuse on this hero loses one less charge.

                    case "infuse":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0d", AppliesTo.Heroes))
                        {
                            __result.AuraConsumed = 3;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0e", AppliesTo.Heroes))
                        {
                            __result.MaxCharges += 4;
                            __result.MaxMadnessCharges += 4;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0f", AppliesTo.Heroes))
                        {
                            __result.AuraDamageConditionalBonuses = [];
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0h", AppliesTo.ThisHero))
                        {
                            __result.AuraConsumed -= 1;
                        }
                        break;
                    case "evasion":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "evasion0b", AppliesTo.Heroes))
                        {
                            __result.ConsumeAll = true;
                            __result.GainCharges = true;
                            __result.ConsumedAtTurnBegin = true;
                        }
                        break;
                    case "mark":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mark1e", AppliesTo.Monsters))
                        {
                            // mark1e: Every 2 mark charges increases piercing damage by 3.
                            __result.IncreasedDamageReceivedType = Enums.DamageType.Piercing;
                            // __result.IncreasedDirectDamageChargesMultiplierNeededForOne = 2;
                            __result.IncreasedDirectDamageReceivedPerStack = 1.5f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mark1g", AppliesTo.Global))
                        {
                            // mark1g: Halfs the bonus damage from Mark. Mark decreases speed by 1 per charge.
                            __result.IncreasedDirectDamageReceivedPerStack *= 0.5f;
                            __result.CharacterStatModified = Enums.CharacterStat.Speed;
                            __result.CharacterStatModifiedValuePerStack = -1;
                        }
                        break;
                    case "disarm":
                        //disarm1b - cannot be dispelled unless specified, increases resists by 10%
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "disarm1b", AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedValue = 10;
                        }
                        break;

                    case "silence":
                        //silence1b - cannot be dispelled unless specified, increases damage by 7
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "silence1b", AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedTotal = 7;
                        }
                        break;

                    case "stealth":
                        //  Unused
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "stealth1d", AppliesTo.Heroes))
                        {
                            __result.AuraDamageIncreasedPercentPerStack = 0.0f;
                        }
                        break;

                    case "fast":
                        // fast0b: Fast on this hero can stack, but loses all charges at the start of turn.";
                        // fast0c: Fast on this hero falls off at the end of turn.";

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "fast0b", AppliesTo.Heroes))
                        {
                            __result.GainCharges = true;
                            __result.ConsumeAll = true;

                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "fast0c", AppliesTo.Heroes))
                        {
                            __result.ConsumedAtTurn = true;
                            __result.ConsumedAtTurnBegin = false;

                        }
                        break;

                    case "slow":
                        // slow0b: Slow on monsters can stack up to 10, but only reduces Speed by 1 per charge";
                        // slow0c: Slow on heroes can stack up to 10, but only reduces Speed by 1 per charge";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "slow0b", AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = 10;
                            __result.MaxMadnessCharges = 10;
                            __result.CharacterStatModifiedValuePerStack = -1 * rustMultiplier;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "slow0c", AppliesTo.Heroes))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = 10;
                            __result.MaxMadnessCharges = 10;
                            __result.CharacterStatModifiedValuePerStack = -1 * rustMultiplier;
                        }
                        break;

                    // fortify1e: Fortify on all heroes is capped at 5 but reduces damage done by 1 per charge.  
                    // fortify1f: At the start of combat, apply 2 Fortify to all heroes. Fortify on all heroes has a maximum of 2.
                    case "fortify":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "fortify1e", AppliesTo.Heroes))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = __result.MaxMadnessCharges = 5;
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.All, 0, -1, 0);
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "fortify1f", AppliesTo.Heroes))
                        {
                            __result.MaxCharges = __result.MaxMadnessCharges = 2;
                        }
                        break;
                    case "fury":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "fury1d", AppliesTo.Heroes))
                        {
                            __result.ConsumeAll = true;
                        }
                        break;

                    case "sharp":
                        // sharp1d: shadow damaage for all heroes
                        // sharp1e: If Sharp on a hero would increase a damage type, it increases it by 1.5 damage per charge. Sharp on heroes only stacks to 25.";
                        // insane2e: Insane on this hero increases the effectiveness of sharp by 1% per charge.";
                        // zeal0e: While this hero has Zeal, Sharp increases their Holy damage by 1 per charge.
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperksharp1d", AppliesTo.Heroes))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, 1, 0);
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "zeal0e", AppliesTo.ThisHero) && characterOfInterest.HasEffect("zeal"))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Holy, 0, 1, 0);
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "sharp1g", AppliesTo.ThisHero))
                        {
                            float amountToModify = AtOManager.Instance.team.TeamHaveTrait("shrilltone") ? 1.5f : 1;
                            if (hasRust)
                                amountToModify *= 0.5f;
                            if (AtOManager.Instance.team.TeamHaveTrait("shrilltone"))
                            {
                                if (__result.AuraDamageType == Enums.DamageType.Mind)
                                {
                                    __result.AuraDamageIncreasedPerStack = amountToModify;
                                }
                                if (__result.AuraDamageType2 == Enums.DamageType.Mind)
                                {
                                    __result.AuraDamageIncreasedPerStack2 = amountToModify;
                                }
                                if (__result.AuraDamageType3 == Enums.DamageType.Mind)
                                {
                                    __result.AuraDamageIncreasedPerStack3 = amountToModify;
                                }
                                if (__result.AuraDamageType4 == Enums.DamageType.Mind)
                                {
                                    __result.AuraDamageIncreasedPerStack4 = amountToModify;
                                }
                            }
                            else
                            {
                                __result.AuraDamageType4 = Enums.DamageType.Mind;
                                __result.AuraDamageIncreasedPerStack4 = amountToModify;
                            }


                        }


                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "sharp1e", AppliesTo.Heroes))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, 1, 0);
                            __result.MaxCharges = 25;
                            __result.MaxMadnessCharges = 25;
                            __result.AuraDamageIncreasedPerStack = hasRust ? 0.75f : 1.5f;
                            __result.AuraDamageIncreasedPerStack2 = hasRust ? 0.75f : 1.5f;
                            __result.AuraDamageIncreasedPerStack3 = hasRust ? 0.75f : 1.5f;
                            __result.AuraDamageIncreasedPerStack4 = hasRust ? 0.75f : 1.5f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "insane2e", AppliesTo.ThisHero))
                        {
                            // Doesn't need rust to be applied to it since it is a multiplier
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, 1, 0);
                            int n = characterOfInterest.GetAuraCharges("insane");
                            __result.AuraDamageIncreasedPerStack *= 1 + 0.01f * n;
                            __result.AuraDamageIncreasedPerStack2 *= 1 + 0.01f * n;
                            __result.AuraDamageIncreasedPerStack3 *= 1 + 0.01f * n;
                            __result.AuraDamageIncreasedPerStack4 *= 1 + 0.01f * n;
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "sharp1f", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPerStack = 0;
                            __result.AuraDamageIncreasedPerStack2 = 0;
                            __result.AuraDamageIncreasedPerStack3 = 0;
                            __result.AuraDamageIncreasedPerStack4 = 0;
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, 1, 0);
                        }
                        break;

                    case "crack":
                        // insane2d: Crack on monsters increases Blunt damage by an addition 1 for every 50 charges of Insane on that monster.";
                        // crack2d: Crack on monsters reduces Speed by 0.2.";
                        // crack2e: Crack on monsters reduces Lightning resistance by 0.3% per charge.
                        // crack2f: Crack increases fire damage too
                        // crack2g: Crack increases mind damage too
                        // crack2h: Crack on monsters reduces Slashing resistance by 0.15% per charge.
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "insane2d", AppliesTo.Monsters))
                        {
                            int n = characterOfInterest.GetAuraCharges("insane");
                            __result.IncreasedDirectDamageReceivedPerStack += FloorToInt(0.02f * rustMultiplier * n);
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "crack2d", AppliesTo.Monsters))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.Speed;
                            __result.CharacterStatModifiedValuePerStack = -1 * rustMultiplier * 0.2f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "crack2e", AppliesTo.Monsters))
                        {
                            float amountToModify = -0.2f * rustMultiplier;
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Lightning, 0, amountToModify);
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "crack2f", AppliesTo.Monsters))
                        {
                            __result.IncreasedDamageReceivedType2 = Enums.DamageType.Fire;
                            __result.IncreasedDirectDamageReceivedPerStack2 = 0.75f * rustMultiplier;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "crack2g", AppliesTo.Global))
                        {
                            __result.IncreasedDamageReceivedType2 = Enums.DamageType.Mind;
                            __result.IncreasedDirectDamageReceivedPerStack2 = 0.75f * rustMultiplier;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "crack2h", AppliesTo.Global))
                        {
                            float amountToModify = -0.15f * rustMultiplier;
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Slashing, 0, amountToModify);
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Piercing, 0, amountToModify);
                        }
                        break;

                    case "shackle":
                        // shackle1b: This hero is immune to Shackle.";
                        // shackle1c: Shackle cannot be prevented.";
                        // shackle1d: At start of your turn, gain Fortify equal to your twice your Shackles.";
                        // shackle1e: Shackle increases Dark charges you apply by 1 per charge of Shackle.";
                        // shackle1f: Shackles on monsters increases all damage received by 1 per base Speed.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "shackle1f", AppliesTo.Monsters))
                        {
                            shackle1fFlag = !shackle1fFlag;
                            if (!shackle1fFlag)
                            {
                                break;
                            }
                            int baseSpeed = characterOfInterest.GetSpeed()[1];
                            __result.IncreasedDamageReceivedType = Enums.DamageType.All;
                            float multiplier = 1.0f;
                            // int n_shackle = Math.Max(1,characterOfInterest.GetAuraCharges("shackle"));
                            __result.IncreasedDirectDamageReceivedPerStack = RoundToInt(baseSpeed * multiplier);
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "shackle1c", AppliesTo.Global))
                        {
                            __result.Preventable = false;
                        }

                        break;

                    case "mitigate":
                        // mitigate1a: At the start of your turn, gain 2 Mitigate, but only stack to 5.";
                        // mitigate1b: Mitigate on this hero does not lose charges at start of turn and stacks to 12.";
                        // mitigate1c: At the start of your turn, gain 7 Block per Mitigate charge.";
                        // mitigate1d: Mitigate reduces incoming damage by 2 per charge, but loses all charges at the start of your turn.";
                        // mitigate1e: Mitigate on heroes and monsters increases damage done by 10% per charge.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mitigate1a", AppliesTo.ThisHero))
                        {
                            __result.MaxCharges = 5;
                            __result.MaxMadnessCharges = 5;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mitigate1b", AppliesTo.ThisHero))
                        {
                            __result.ConsumedAtTurnBegin = false;
                            __result.ConsumedAtTurn = false;
                            __result.MaxCharges = 12;
                            __result.MaxMadnessCharges = 12;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mitigate1d", AppliesTo.ThisHero))
                        {
                            __result.ConsumeAll = true;
                            __result.IncreasedDirectDamageReceivedPerStack = -2;
                            __result.ChargesMultiplierDescription = 2;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mitigate1e", AppliesTo.Global))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack = 10;
                        }
                        break;

                    case "poison":
                        // poison2d: If Restricted Power is enabled, increases Max Charges to 300.";
                        // poison2e: Poison on heroes and monsters reduces Slashing resistance by 0.25% per charge.";
                        // poison2f: Poison on monsters deals shadow damage
                        // poison2g: When a monster with Poison dies, transfer 50% of their Poison charges to a random monster.";
                        // poison2h: -1 Poison. When this hero applies poison, deal Mind damage to the target equal to 20% of their Poison charges.";
                        // decay1e: Every stack of decay increases the damage dealt by poison by 20%.";


                        // rust0c: "Rather than increasing Poison Damage by 50%, Rust increases Poison Damage by 10% per stack (up to a max of 200%). Only affects Poison Damage.";

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkpoison2c", AppliesTo.Monsters))
                        {
                            __result.ConsumedAtTurnBegin = true;
                            __result.ConsumedAtTurn = false;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "poison2d", AppliesTo.Global))
                        {
                            __result.MaxMadnessCharges = Mathf.RoundToInt(300 * rustMultiplier);
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "poison2e", AppliesTo.Global))
                        {
                            __result.ResistModified3 = Enums.DamageType.Slashing;
                            __result.ResistModifiedPercentagePerStack3 = -0.18f * rustMultiplier;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "poison2f", AppliesTo.Monsters))
                        {
                            __result.DamageTypeWhenConsumed = Enums.DamageType.Shadow;

                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "decay1e", AppliesTo.Global))
                        {
                            // multiplier so no need for rust
                            int n_decay = characterOfInterest.GetAuraCharges("decay");
                            float multiplier = 1 + 0.2f * n_decay;
                            __result.DamageWhenConsumedPerCharge *= multiplier;
                        }

                        // if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "rust0c", AppliesTo.Global))
                        // {
                        //     float undoRust = 1.0f / 1.5f;
                        //     __result.DamageWhenConsumedPerCharge *= undoRust;
                        //     int nRust = characterOfInterest.GetAuraCharges("rust");
                        //     float newRustMultiplier = Min(1 + 0.1f * nRust, 3.0f); // caps at +200%
                        //     __result.DamageWhenConsumedPerCharge *= newRustMultiplier;
                        // }

                        break;

                    case "bleed":
                        // bleed2b: If Restricted Power is enabled, increases Max Charges to 300.";
                        // bleed2c: Can no longer be dispelled unless specified
                        // bleed2d: If Restricted Power is enabled, increases Max Charges to 300.";
                        // bleed2e: When this hero hits an enemy with Bleed, they heal for 25% of the target's Bleed charges.";
                        // bleed2f: Bleed on heroes and monsters reduces Piercing resist by 0.20% per charge.";
                        // bleed2g: When this hero kills an enemy with Bleed, all monsters lose HP equal to 25% of the killed target's Bleed charges.";
                        // decay1f: Every stack of decay increases the damage dealt by Bleed by 20%.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkbleed2b", AppliesTo.ThisHero))
                        {
                            __result.MaxCharges = 50;
                            __result.MaxMadnessCharges = 50;
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkbleed2c", AppliesTo.Monsters))
                        {
                            __result.Removable = false;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "bleed2d", AppliesTo.Global))
                        {
                            __result.MaxMadnessCharges = 300;

                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "bleed2f", AppliesTo.Global))
                        {
                            __result.ResistModified3 = Enums.DamageType.Piercing;
                            __result.ResistModifiedPercentagePerStack3 = -0.20f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "decay1f", AppliesTo.Global))
                        {
                            // multiplier so no need for rust
                            int n_decay = characterOfInterest.GetAuraCharges("decay");
                            float multiplier = 1 + 0.2f * n_decay;
                            __result.DamageWhenConsumedPerCharge *= multiplier;
                        }
                        break;

                    case "thorns":
                        // thorns1d: Cannot be purged unless specified.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "thorns1d", AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                        }
                        break;

                    case "reinforce":
                        // reinforce1b: Increased to 40%;
                        // reinforce1d: Reinforce increases Block charges by 2 per charge of Reinforce.";
                        // reinforce1e: Reinforce on this hero now increases Piercing, Lightning, and Mind Resistance
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkreinforce1b", AppliesTo.Heroes))
                        {
                            __result.ResistModifiedValue = 40;
                            __result.ResistModifiedValue2 = 40;
                            __result.ResistModifiedValue3 = 40;
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0f", AppliesTo.Heroes) && characterOfInterest.HasEffect("infuse"))
                        {
                            __result.AuraDamageType = Enums.DamageType.Slashing;
                            __result.AuraDamageType = Enums.DamageType.Piercing;
                            __result.AuraDamageType = Enums.DamageType.Blunt;
                            __result.AuraDamageIncreasedPerStack = __result.AuraDamageIncreasedPerStack2 = __result.AuraDamageIncreasedPerStack3 = 1;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "reinforce1e", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.Mind;
                            __result.AuraDamageType = Enums.DamageType.Piercing;
                            __result.AuraDamageType = Enums.DamageType.Lightning;
                        }

                        break;

                    case "block":
                        // block5b: If Restricted Power is enabled, increases Max Charges to 600.";
                        // block5c: At start of combat, apply 2 Block to all heroes.";
                        // //block5d: Block only functions if you are above 50% Max Health [Currently not working].";
                        // block5e: When this hero gains Block, they deal 1 Blunt to themselves and a random monster.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "block5b", AppliesTo.Heroes))
                        {
                            __result.MaxMadnessCharges = 600;
                        }
                        break;
                    case "vulnerable":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "crack2i", AppliesTo.Monsters))
                        {
                            int toIncrease = FloorToInt(0.04f * characterOfInterest.GetAuraCharges("crack"));
                            __result.MaxCharges += toIncrease;
                            __result.MaxMadnessCharges += toIncrease;
                        }

                        break;
                    case "taunt":
                        // taunt1e: Taunt on this hero can stack and increases damage by 1 per charge.";
                        // taunt1h: Taunt on monsters decreases All Resistances by 5% per charge.
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "taunt1h", AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.All, 0, -5f);
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "taunt1e", AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack = 1;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "taunt1g", AppliesTo.ThisHero))
                        {
                            __result.ConsumedAtTurn = false;
                        }
                        break;

                    // rust0f: Rust on this hero does not Prevent or Dispel Reinforce. At the start of your turn, suffer 2 Rust.";
                    // rust0e: Rust on enemies does not Prevent or Dispel Reinforce. Rust on enemies reduces Physical resistance by 5% per charge.
                    case "rust":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "rust0f", AppliesTo.ThisHero))
                        {
                            AuraCurseData noneAC = GetAuraCurseData("None");
                            __result.PreventedAuraCurse = noneAC;
                            __result.PreventedAuraCurseStackPerStack = 0;
                            __result.RemoveAuraCurse = noneAC;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "rust0e", AppliesTo.ThisHero))
                        {
                            AuraCurseData noneAC = GetAuraCurseData("None");
                            __result.PreventedAuraCurse = noneAC;
                            __result.PreventedAuraCurseStackPerStack = 0;
                            __result.RemoveAuraCurse = noneAC;
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Slashing, 0, -5f);
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Piercing, 0, -5f);
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Blunt, 0, -5f);
                        }
                        break;
                    // taunt1f: Taunt on heroes increases maximum Powerful by 1 per charge.
                    // powerful1e: Powerful on this hero has no cap, but increases damage done by 2% per charge.
                    case "powerful":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "taunt1f", AppliesTo.Heroes))
                        {
                            int nTaunt = characterOfInterest.GetAuraCharges("taunt");
                            __result.MaxCharges += nTaunt;
                            __result.MaxMadnessCharges += nTaunt;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "powerful1e", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPercentPerStack = 2f;
                            __result.MaxCharges = __result.MaxMadnessCharges = -1;
                        }
                        break;
                    // inspire0e: Inspire on this hero is lost at the end of turn and increases Holy and Mind damage by 0.5 per charge
                    case "inspire":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "inspire0e", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.Holy;
                            __result.AuraDamageType2 = Enums.DamageType.Mind;
                            __result.AuraDamageIncreasedPerStack = 0.5f;
                            __result.AuraDamageIncreasedPerStack2 = 0.5f;
                        }
                        break;
                    case "insulate":
                        // insulate1d: Insulate on this hero prevents their Speed from being lowered by Chill.  
                        // insulate1e: Insulate on this hero increases Elemental damage by 5% per stack, but only increases Elemental resistances by 15%. Insulate on this hero stacks to 8.
                        // insulate1f: Insulate on this hero now increases Blunt, Chill, and Shadow Resistance

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0f", AppliesTo.Heroes) && characterOfInterest.HasEffect("infuse"))
                        {
                            __result.AuraDamageType = Enums.DamageType.Fire;
                            __result.AuraDamageType = Enums.DamageType.Lightning;
                            __result.AuraDamageType = Enums.DamageType.Cold;
                            __result.AuraDamageIncreasedPerStack = __result.AuraDamageIncreasedPerStack2 = __result.AuraDamageIncreasedPerStack3 = 1;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkinsulate1b", AppliesTo.Heroes))
                        {
                            __result.ResistModifiedValue = 40;
                            __result.ResistModifiedValue2 = 40;
                            __result.ResistModifiedValue3 = 40;
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "insulate1e", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.Fire;
                            __result.AuraDamageType2 = Enums.DamageType.Cold;
                            __result.AuraDamageType3 = Enums.DamageType.Lightning;
                            __result.AuraDamageIncreasedPercentPerStack = 5.0f;
                            __result.AuraDamageIncreasedPercentPerStack2 = 5.0f;
                            __result.AuraDamageIncreasedPercentPerStack3 = 5.0f;
                            __result.ResistModifiedValue = 15;
                            __result.ResistModifiedValue2 = 15;
                            __result.ResistModifiedValue3 = 15;
                            __result.GainCharges = true;
                            __result.MaxCharges = 8;
                            __result.MaxMadnessCharges = 8;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "insulate1f", AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.Blunt;
                            __result.ResistModified2 = Enums.DamageType.Cold;
                            __result.ResistModified3 = Enums.DamageType.Shadow;
                        }


                        break;
                    case "spellsword":
                        // spellsword1a: Max stacks +2";
                        // spellsword1b: Spellsword on heroes reduces incoming damage by 2, but does not increase damage";

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "spellsword1a", AppliesTo.Heroes))
                        {
                            __result.MaxCharges += 2;
                            if (__result.MaxMadnessCharges != -1)
                                __result.MaxMadnessCharges += 2;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "spellsword1b", AppliesTo.Heroes))
                        {
                            __result.AuraDamageType = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPerStack = 0;
                            __result.IncreasedDirectDamageReceivedPerStack = -2;
                            __result.ChargesMultiplierDescription = 2;
                        }

                        break;
                    case "energize":
                        // energize1b: Energize gives 2 energy per charge, but you can only have a maximum of 1 Energize.";
                        // energize1c: Energize increases All Damage by 1 per charge.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "energize1b", AppliesTo.ThisHero))
                        {
                            __result.MaxCharges = 1;
                            __result.MaxMadnessCharges = 1;
                            __result.CharacterStatModifiedValuePerStack = 2;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "energize1c", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack = 1;
                        }

                        break;
                    case "burn":
                        // mainperkburn2d: changed to be less than 4 rather than less than 3 curses
                        // scourge0h: Scourge on monsters increases burn damage by 15%/stack";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkburn2d", AppliesTo.Monsters))
                        {
                            __result.DoubleDamageIfCursesLessThan = 4;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0h", AppliesTo.Heroes))
                        {
                            int scourge_charges = characterOfInterest.GetAuraCharges("scourge");
                            float multiplier = 0.15f * scourge_charges + 1;
                            __result.DamageWhenConsumedPerCharge *= multiplier;
                        }
                        break;
                    case "chill":
                        // chill2e: Chill reduces Cold and Mind resistance by 0.5% per charge.";
                        // chill2f: At the start of your turn, suffer 3 Chill. Chill on this hero reduces Speed by 1 for every 10 charges";
                        // chill2g: Chill on this hero reduces Speed by 1 for every 3 charges but does not reduce Cold resistance.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "chill2e", AppliesTo.Global))
                        {
                            __result.ResistModified = Enums.DamageType.Cold;
                            __result.ResistModified2 = Enums.DamageType.Mind;
                            __result.ResistModifiedPercentagePerStack = -0.5f;
                            __result.ResistModifiedPercentagePerStack2 = -0.5f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "chill2f", AppliesTo.ThisHero))
                        {
                            __result.CharacterStatChargesMultiplierNeededForOne = 10;

                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "chill2g", AppliesTo.ThisHero))
                        {
                            __result.CharacterStatChargesMultiplierNeededForOne = 3;
                            __result.ResistModified = Enums.DamageType.None;
                            __result.ResistModifiedPercentagePerStack = 0.0f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "insulate1d", AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.None;
                            __result.CharacterStatAbsoluteValuePerStack = 0;
                        }
                        break;
                    case "wet":
                        // wet1d: Wet does not Dispel or Prevent Burn.";
                        // zeal0g: While any hero and monster has Zeal, Wet increases all resistances by 0.5%
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "zeal0g", AppliesTo.Global) && characterOfInterest.HasEffect("zeal"))
                        {
                            __result.ResistModified3 = Enums.DamageType.All;
                            // __result.ResistModifiedPercentagePerStack3 = 0.5f;
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.All, 0, 0.5f);
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "wet1d", AppliesTo.Global))
                        {
                            // Not sure if this is working
                            AuraCurseData noneAC = GetAuraCurseData("None");
                            __result.PreventedAuraCurse = noneAC;
                            __result.PreventedAuraCurseStackPerStack = 0;
                            __result.RemoveAuraCurse = noneAC;
                        }
                        bool hasRust0d = IfCharacterHas(characterOfInterest, CharacterHas.Perk, "rust0d", AppliesTo.Monsters);
                        if (hasRust0d)
                        {
                            __result.IncreasedDirectDamageReceivedPerStack *= 2.25f;
                            __result.IncreasedDirectDamageReceivedPerStack2 *= 2.25f;
                            __result.ResistModifiedPercentagePerStack *= 2.25f;
                            __result.ResistModifiedPercentagePerStack2 *= 2.25f;
                            __result.ResistModifiedPercentagePerStack3 *= 2.25f;
                        }
                        // if (hasRust0d)
                        // {
                        //     __result.IncreasedDirectDamageReceivedPerStack = 1.5f;
                        // }
                        // if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkwet1a", AppliesTo.Monsters) && hasRust0d)
                        // {
                        //     __result.IncreasedDamageReceivedType2 = Enums.DamageType.Cold;
                        //     __result.IncreasedDirectDamageReceivedPerStack2 = 1.5f;
                        // }
                        // if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkwet1b", AppliesTo.Monsters) && hasRust0d)
                        // {
                        //     __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Lightning, 0, -1.5f);
                        //     __result.AuraConsumed = 0;
                        // }
                        break;

                    case "spark":
                        // spark2d: Gain +1 Lightning Damage for every 5 stacks of Spark on this hero."
                        // spark2e: Spark deal Fire damage. Spark decreases Fire resistance by 0.5% per charge and Lightning resistance by 0.5% per charge.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "spark2d", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.Lightning;
                            __result.AuraDamageIncreasedPerStack = 1;
                            __result.ChargesAuxNeedForOne1 = 5;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "spark2e", AppliesTo.Monsters))
                        {
                            __result.DamageTypeWhenConsumed = Enums.DamageType.Fire;
                            __result.ResistModified = Enums.DamageType.Lightning;
                            __result.ResistModified2 = Enums.DamageType.Fire;
                            __result.ResistModifiedPercentagePerStack = -0.5f;
                            __result.ResistModifiedPercentagePerStack2 = -0.5f;
                        }
                        break;

                    case "shield":
                        // shield5b: If Restricted Power is enabled, increases Max Charges to 300.";
                        // shield5d: Shield on you increases Holy damage by 0.2 per charge
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "shield5b", AppliesTo.Global))
                        {
                            __result.MaxMadnessCharges = 300;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "shield5d", AppliesTo.Heroes))
                        {
                            // __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Holy, 0, 0.2f, 0);
                            __result.AuraDamageType = Enums.DamageType.Holy;
                            __result.AuraDamageIncreasedPerStack = 0.2f;
                        }
                        break;

                    case "regeneration":
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "regeneration1d", AppliesTo.Heroes))
                        {
                            __result.PreventedAuraCurse = GetAuraCurseData("vulnerable");
                            __result.PreventedAuraCurseStackPerStack = 1;
                        }

                        break;
                    case "dark":
                        // scourge0e: Dark no longer explodes. Every charge of Scourge increases damage due to other curses by 5%.";
                        // dark2e: Dark explosions deal Fire damage. Dark reduces Fire resistance by 0.25% per charge in addition to reducing Shadow resistance..";
                        // burn1e: Burn increases the damage dealt by Dark explosions by 0.5% per charge.";
                        // sanctify2d: Every 5 stacks of Sanctify increase the number of Dark charges needed for an explosion by 1.";

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkdark2b", AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "dark2e", AppliesTo.Global))
                        {
                            __result.DamageTypeWhenConsumed = Enums.DamageType.Fire;
                            __result.ResistModified2 = Enums.DamageType.Fire;
                            __result.ResistModifiedPercentagePerStack2 = -0.5f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "sanctify2d", AppliesTo.Global))
                        {
                            int n = characterOfInterest.GetAuraCharges("sanctify");
                            __result.ExplodeAtStacks += FloorToInt(0.2f * n);
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "burn2e", AppliesTo.Global))
                        {
                            int n_charges = characterOfInterest.GetAuraCharges("burn");
                            float multiplier = 1 + 0.05f * n_charges;
                            __result.DamageWhenConsumedPerCharge *= multiplier;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0e", AppliesTo.Global))
                        {
                            __result.ExplodeAtStacks = -1;
                        }
                        break;

                    case "decay":
                        // decay1d: Decay purges Insulate.";

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "decay1d", AppliesTo.Global))
                        {
                            __result.RemoveAuraCurse = GetAuraCurseData("insulate");
                        }


                        break;

                    case "courage":
                        // courage1d: Courage increases Shield gained by this hero by 1 per charge.
                        // courage1e: Courge on this hero now increases Slashing, Fire, and Holy resistance.

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "infuse0f", AppliesTo.Heroes) && characterOfInterest.HasEffect("infuse"))
                        {
                            __result.AuraDamageType = Enums.DamageType.Holy;
                            __result.AuraDamageType = Enums.DamageType.Shadow;
                            __result.AuraDamageType = Enums.DamageType.Mind;
                            __result.AuraDamageIncreasedPerStack = __result.AuraDamageIncreasedPerStack2 = __result.AuraDamageIncreasedPerStack3 = 1;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "mainperkcourage1b", AppliesTo.Heroes))
                        {
                            __result.ResistModifiedValue = 40;
                            __result.ResistModifiedValue2 = 40;
                            __result.ResistModifiedValue3 = 40;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "courage1f", AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.Slashing;
                            __result.ResistModified2 = Enums.DamageType.Fire;
                            __result.ResistModified3 = Enums.DamageType.Holy;
                        }
                        break;

                    case "zeal":
                        // zeal0d: Zeal on this hero increases All Damage done by 1.5% per Bleed charge on this hero
                        // zeal0e: While this hero has Zeal, Sharp increases their Holy damage by 1 per charge.
                        // zeal0f: Zeal on all heroes increases Speed by 2 per charge.
                        // zeal0g: When this hero loses Zeal, deal indirect Holy and Fire damage equal to 4x the number of stacks lost to all monsters.
                        // zeal0h: When this hero loses Zeal at end of turn, deal indirect Holy and Fire damage to all monsters equal to 4x the number of charges lost..
                        // zeal0i: Zeal on this hero can stack, but no longer increases Resistances. At the end of turn, suffer 5 Burn per charge..
                        // zeal0j: Zeal on heroes makes Thorns apply half their damage as Burn rather than dealing damage (untested).
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "zeal0d", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercent = Mathf.RoundToInt(1.5f * characterOfInterest.GetAuraCharges("bleed"));
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "zeal0i", AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.ResistModified = Enums.DamageType.None;
                            __result.ResistModifiedPercentagePerStack = 0.0f;
                            __result.GainAuraCurseConsumption = GetAuraCurseData("burn");
                            __result.GainAuraCurseConsumptionPerCharge = 5;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "zeal0f", AppliesTo.Heroes))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.Speed;
                            __result.CharacterStatModifiedValuePerStack = 2;
                        }
                        break;

                    case "scourge":
                        // scourge0d: Scourge on monsters also deals 1 Shadow damage per Sight charge (Not working) TODO
                        // scourge0e: Dark no longer explodes. Every charge of Scourge increases damage due to other curses by 5%.
                        // scourge0f: Scourge on monsters can Stack but increases all resists by 3% per stack.
                        // scourge0g: Scourge deals damage based on Sight rather than Chill.
                        // scourge0h: Scourge on monsters increases burn damage by 15%/stack
                        // scourge0i: Dark explosions deal 10% of their damage to the target's sides for each charge of Scourge
                        // scourge0j: If an enemy has two or less curses, Scourge deals 4x damage

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0d", AppliesTo.Monsters))
                        {
                            __result.DamageTypeWhenConsumed = Enums.DamageType.Shadow;
                            __result.DamageWhenConsumed += characterOfInterest.GetAuraCharges("sight");
                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0f", AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                            __result.ResistModified2 = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack2 = 3.0f;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0g", AppliesTo.Global))
                        {
                            __result.ConsumedDamageChargesBasedOnACCharges = GetAuraCurseData("sight");
                            __result.DamageWhenConsumedPerCharge = 2;
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0j", AppliesTo.Global))
                        {
                            if (characterOfInterest.GetCurseList().Count <= 2)
                            {
                                __result.DamageWhenConsumedPerCharge *= 4;
                            }
                        }
                        break;

                    case "weak":
                        // weak1c: Monsters cannot be immune to Weak, but no longer have their damage reduced by Insane.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "weak1c", AppliesTo.Monsters))
                        {
                            __result.Preventable = false;
                            __result.AuraDamageIncreasedPercent = -20;
                            __result.HealDonePercent = -20;
                        }
                        break;

                    case "vitality":
                        // vitality1d: Vitality on this hero dispels Poison.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "vitality1d", AppliesTo.ThisHero))
                        {
                            __result.RemoveAuraCurse = GetAuraCurseData("poison");
                        }
                        break;

                    case "bless":
                        // bless1d: Bless on all heroes increases Slashing, Fire, and Holy damage by 3% per charge but does not increase damage by 1.";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "bless1d", AppliesTo.Heroes))
                        {
                            __result.AuraDamageType = Enums.DamageType.Slashing;
                            __result.AuraDamageType2 = Enums.DamageType.Fire;
                            __result.AuraDamageType3 = Enums.DamageType.Holy;
                            __result.AuraDamageIncreasedPercentPerStack = 3.0f;
                            __result.AuraDamageIncreasedPercentPerStack2 = 3.0f;
                            __result.AuraDamageIncreasedPercentPerStack3 = 3.0f;
                            __result.AuraDamageIncreasedPerStack = 0.0f;
                        }
                        break;
                }
                if (IfCharacterHas(characterOfInterest, CharacterHas.Perk, "scourge0e", AppliesTo.Monsters))
                {
                    __result.DamageWhenConsumed = Mathf.RoundToInt(characterOfInterest.GetAuraCharges("scourge") * 0.05f + 1) * __result.DamageWhenConsumed;
                    __result.DamageWhenConsumedPerCharge *= 1 + characterOfInterest.GetAuraCharges("scourge") * 0.05f;
                }
            }
        }

        public static void ApplyBalanceGACM(ref AuraCurseData __result, string _acId, Character characterOfInterest)
        {
            if (characterOfInterest != null && characterOfInterest.Alive)
            {
                string itemID;

                switch (_acId)
                {

                    case "bleed":
                        itemID = "bloodstone";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "yoggercleaver";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbtreefellingaxe", AppliesTo.Monsters) || IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbtreefellingaxerare", AppliesTo.Monsters))
                        {
                            __result.Preventable = false;
                        }
                        itemID = "boneclawsrare";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, AppliesTo.ThisHero))
                        {
                            __result.DamageTypeWhenConsumed = Enums.DamageType.None;
                            __result.DamageWhenConsumedPerCharge = 0;
                        }

                        itemID = "mozzy";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, AppliesTo.ThisHero) || IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID + "rare", AppliesTo.ThisHero))
                        {
                            __result.ConsumedAtTurn = true;
                            __result.ConsumedAtTurnBegin = false;
                        }

                        break;
                    case "bless":
                        itemID = "topazring";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                    case "block":
                        itemID = "crusaderhelmet";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                    case "burn":
                        itemID = "solring";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "ringoffire";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "captainspresencered";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, itemID, AppliesTo.ThisHero))
                        {
                            __result.Preventable = false;
                            __result.Removable = false;
                        }
                        break;
                    case "chill":
                        itemID = "lunaring";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "neverfrost";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                    case "crack":
                        itemID = "bronzegear";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "ironkanabo";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                        break;
                    case "dark":
                        itemID = "blackpyramid";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID + "rare", AppliesTo.Monsters))
                        {
                            __result.ExplodeAtStacks = 34;
                        }
                        else if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, AppliesTo.Monsters))
                        {
                            __result.ExplodeAtStacks = 30;
                        }
                        itemID = "soullanternrare";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, AppliesTo.ThisHero))
                        {
                            __result.ExplodeAtStacks = 0;
                            __result.DamageTypeWhenConsumed = Enums.DamageType.None;
                            __result.DamageWhenConsumedPerCharge = 0;
                        }

                        break;
                    case "fast":
                        itemID = "rocketbootsrare";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.ConsumeAll = false;
                        }
                        break;
                    case "mark":
                        itemID = "hellblade";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "redsteelcloack";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                        break;
                    case "poison":
                        itemID = "thepolluter";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "venomamulet";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbslimepoison", AppliesTo.Monsters) ||
                            IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbslimepoisonrare", AppliesTo.Monsters))
                        {
                            __result.Preventable = false;
                            // __result.Removable = false;
                        }
                        break;
                    case "powerful":
                        itemID = "mysticstaff";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "powergloverare";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                    case "sight":
                        itemID = "eeriering";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                    case "scourge":
                        itemID = "captainspresenceblack";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, itemID, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                        }
                        break;
                    case "thorns":
                        itemID = "corruptedplateb";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "shieldofthorns";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "thornyring";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "yggdrasilroot";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "bbbthehedgehog";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "bbbphalanx";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "heartofthorns";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbportablewallofflames", AppliesTo.ThisHero) || IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbportablewallofflamesrare", AppliesTo.ThisHero))
                        {
                            __result.DamageReflectedType = Enums.DamageType.Fire;
                        }

                        break;
                    case "vitality":
                        itemID = "heartamulet";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "bbbsausagelinknecklace";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                    case "wet":
                        itemID = "bucket";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        itemID = "waterskin";
                        UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                        break;
                }
            }
        }

        public static void ApplyHeroTraitGACM(
            ref AtOManager __instance,
            ref AuraCurseData __result,
            string _type,
            string _acId,
            Character _characterCaster,
            Character _characterTarget,
            Character characterOfInterest)
        {
            string traitOfInterest;
            string enchantmentOfInterest;
            string enchant;
            bool hasRust = characterOfInterest != null && characterOfInterest.EffectCharges("rust") >= 0;

            // --- Trickster (always) ---
            //Draw Power increases max powerful charges by 5 lose an additional 2 charges per turn         

            switch (_acId)
            {
                case "scourge":
                    // --- Ainz ---
                    {
                        // "overlordtrait2a":
                        // Scourge on enemies can stack
                        // "overlordtrait2b":

                        // trait 4a;

                        // trait 4b:
                        // Decay on enemies can stack, cannot be dispelled unless specified, and increases All Damage taken by 2 per stack.

                        traitOfInterest = "overlordtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                        }
                    }

                    // --- Hecar ---
                    {
                        // 0: Insane on you does not reduce damage, increases healing by 1% per charge, and stacks to 200
                        // item 1a: Scourge on this hero can stack and is lost at the end of turn.
                        // 2a: Scourge on this hero increases Damage and Healing by 10% per charge.
                        // item 3b: Crack reduces mind resistance by 1%/charge
                        // 4a: Scourge on all characters can stack. 
                        // 4a: Scourge loses half charges when consumed.
                        // 4b: Insane on this hero increases mind damage by 3% per charge.                

                        traitOfInterest = "moontouchedtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType2 = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack2 = 10.0f;
                            __result.HealDonePercentPerStack = 10;

                        }

                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "moontouchedtrait1b", AppliesTo.ThisHero) ||
                            IfCharacterHas(characterOfInterest, CharacterHas.Item, "moontouchedtrait1ba", AppliesTo.ThisHero) ||
                            IfCharacterHas(characterOfInterest, CharacterHas.Item, "moontouchedtrait1bb", AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.ConsumedAtTurn = true;
                            __result.ConsumedAtTurnBegin = false;
                        }

                        traitOfInterest = "moontouchedtrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result.GainCharges = true;
                            __result.AuraConsumed = 0;
                            __result.ConsumeAll = false;
                        }
                    }
                    break;

                case "decay":
                    // --- Ainz ---
                    {
                        traitOfInterest = "overlordtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                            __result.Removable = true;
                            __result.IncreasedDamageReceivedType = Enums.DamageType.All;
                            __result.IncreasedDirectDamageReceivedPerStack = 2;
                        }
                    }

                    // --- EbonyWarrior ---
                    {
                        // "ebonywarriortrait4a":
                        // Decay on Monsters can stack. 

                        traitOfInterest = "ebonywarriortrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                        }
                    }
                    break;

                case "wet":
                    // --- Akula ---
                    {
                        // "sharktrait0":
                        // Wet on you increases speed by 1 per 2 charges. 
                        // Wet on Enemies prevents Bleed from being prevented or removed unless specified. 

                        // "sharktrait2a"
                        // Block +1 for every 3 Wet on you. Speed +1 for every 15 Bleed on enemies.

                        traitOfInterest = "sharktrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.Speed;
                            __result.CharacterStatModifiedValuePerStack = 1;
                            __result.CharacterStatChargesMultiplierNeededForOne = 2;
                        }
                        traitOfInterest = "sharktrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                        }
                    }

                    // --- Azshara ---
                    {
                        traitOfInterest = "flamewakertrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result.PreventedAuraCurse = null;
                            __result.RemoveAuraCurse = null;
                            __result.PreventedAuraCurseStackPerStack = 0;

                        }
                    }
                    break;

                case "bleed":
                    // --- Akula ---
                    {
                        traitOfInterest = "sharktrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters) && characterOfInterest.HasEffect("wet"))
                        {
                            __result.Preventable = false;
                            __result.Removable = false;
                        }
                    }

                    // --- Bloodrager ---
                    {
                        // item barbarianslasher - +0.25 damage/stack vitality
                        // corrupted - +1 damage/stack vitality

                        // "barbariantrait0":
                        // Bless, Fury and Sharp are half as effective at increasing your damage. 
                        // Bleed on you increases damage by 2%/charge 

                        // enchantment1a: Bleed cannot be removed, prevented, or restricted

                        // 4a: +100 max vit on this hero
                        // 4b: +200 max bleed globally

                        traitOfInterest = "barbariantrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack = 2.5f;
                        }

                        enchantmentOfInterest = "barbariantrait1a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, enchantmentOfInterest, AppliesTo.Global))
                        {
                            __result.Preventable = false;
                            __result.Removable = false;
                            __result.MaxCharges = -1;
                            __result.MaxMadnessCharges = -1;
                        }
                        traitOfInterest = "barbariantrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global) && __result.MaxMadnessCharges != -1)
                        {
                            __result.MaxMadnessCharges += 200;
                        }

                        traitOfInterest = "barbariantrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.DoubleDamageIfCursesLessThan = 100;
                        }
                    }

                    // --- Jason ---
                    {
                        // "afflictortrait0":
                        // Poison and bleed cannot be prevented by immunities or buffer, 
                        // nor can they be dispelled (even when specified).

                        // "afflictortrait2b":

                        // trait 4a;

                        // trait 4b:

                        traitOfInterest = "afflictortrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.Preventable = false;
                            __result.Removable = false;
                        }
                        // traitOfInterest = "afflictortrait4b";
                        // if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        // {
                        //     __result.MaxCharges = -1;
                        //     __result.MaxMadnessCharges = -1;
                        // }
                    }
                    break;

                case "taunt":
                    // --- Albedo ---
                    {
                        // "overseertrait2a":

                        // "overseertrait2b":
                        // Taunt on you increases resistances by 5% per charge. Taunt on enemies reduces resistances by 5% per charge.
                        // trait 4a;

                        // trait 4b:
                        // Taunt on heroes and monsters can Stack to 10

                        traitOfInterest = "overseertrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result = GlobalAuraCurseModifyResist(AtOManager.Instance, __result, Enums.DamageType.All, 0, 5f);
                        }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(AtOManager.Instance, __result, Enums.DamageType.All, 0, -5f);
                        }
                        traitOfInterest = "overseertrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = __result.MaxMadnessCharges = 10;
                        }
                    }

                    // --- Cheryl ---
                    {
                        traitOfInterest = "royalguardtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = __result.MaxMadnessCharges = 4;
                        }
                    }

                    // --- Damali ---
                    {
                        traitOfInterest = "minitaurtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = 10f;
                        }
                    }

                    // --- Dorlf ---
                    {
                        // "irongolemtrait0":
                        // Taunt on you cannot be purged unless specified. At the start of combat, gain 1 Taunt, 1 Reinforce, and 2 Fortify

                        // "irongolemtrait2b":
                        // Taunt on you can stack up to 10. Reduce the cost of your highest cost card by 1 until discarded, repeat for every 2 Taunt on you.

                        // trait 4b:
                        // Taunt +1. Taunt on you increases Physical and Lightning damage by 2 per charge. Taunt on you can stack to 15.

                        traitOfInterest = "irongolemtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                        }
                        traitOfInterest = "irongolemtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = __result.MaxMadnessCharges = 10;
                        }
                        traitOfInterest = "irongolemtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = __result.MaxMadnessCharges = 15;
                            __result.AuraDamageType = Enums.DamageType.Slashing;
                            __result.AuraDamageType2 = Enums.DamageType.Blunt;
                            __result.AuraDamageType3 = Enums.DamageType.Piercing;
                            __result.AuraDamageType4 = Enums.DamageType.Lightning;
                            __result.AuraDamageIncreasedPerStack = __result.AuraDamageIncreasedPerStack2 = __result.AuraDamageIncreasedPerStack3 = __result.AuraDamageIncreasedPerStack4 = 2;
                        }
                    }

                    // --- Fabricator ---
                    {
                        traitOfInterest = "fabricatortrait2b";
                        // "fabricatortrait2b": Taunt +1. Taunt on this hero can stack. This hero gains 5% more Block and Shield for each stack of Taunt.
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                        }
                    }
                    break;

                case "chill":
                    // --- Aurelion ---
                    {
                        // "cryohealertrait0":
                        // Chill on heroes and enemies reduces Mind resistance by 0.2% per charge. 
                        // Insane on heroes and enemies reduces Cold resistance by 0.2% per charge.

                        // "cryohealertrait2a":
                        // Chill on you cannot be dispelled unless specified. 
                        // When you play a Defense that costs Energy, refund 1 Energy and suffer 2 Chill. (3 times/turn)

                        // trait 4a;
                        // Evasion on you can't be purged unless specified. 
                        // Stealth grants 25% additional damage per charge.",

                        // trait 4b:
                        // Heroes Only lose 75% stealth charges rounding down when acting in stealth.

                        traitOfInterest = "cryohealertrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Mind, 0, -0.2f);
                        }

                        traitOfInterest = "cryohealertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                        }
                    }

                    // --- Graendor ---
                    {
                        // "icebreakertrait2a":
                        // Chill reduces Blunt damage resistance by 0.5%/charge.

                        traitOfInterest = "icebreakertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result = GlobalAuraCurseModifyResist(AtOManager.Instance, __result, Enums.DamageType.Blunt, 0, -0.5f);
                        }
                    }
                    break;

                case "insane":
                    // --- Aurelion ---
                    {
                        traitOfInterest = "cryohealertrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Cold, 0, -0.2f);
                        }
                    }

                    // --- Hecar ---
                    {
                        traitOfInterest = "moontouchedtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            // __result.AuraDamageType3 = Enums.DamageType.Mind;
                            // __result.AuraDamageIncreasedPercentPerStack3 = 2.0f;
                            __result.AuraDamageType = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPercentPerStack = 0;
                            __result.HealDonePercentPerStack = 1;
                            __result.MaxCharges = 200;
                            __result.MaxMadnessCharges = 200;
                        }

                        traitOfInterest = "moontouchedtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType3 = Enums.DamageType.Mind;
                            __result.AuraDamageIncreasedPercentPerStack3 = 3.0f;
                        }
                    }
                    break;

                case "burn":
                    // --- Azshara ---
                    {
                        // "flamewakertrait0":
                        // Wet doesn't dispel/prevent burn 

                        // "flamewakertrait2a":
                        // Sparks only deal damage to the target. 
                        // Sparks reduce Fire resistance by 0.5% per charge. 
                        // Burn reduces Lightning reistance by 0.5% per charge",

                        traitOfInterest = "flamewakertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result = GlobalAuraCurseModifyResist(AtOManager.Instance, __result, Enums.DamageType.Lightning, 0, -0.5f);
                        }
                    }

                    // --- Hanshek ---
                    {
                        // "royalmagetrait2a":
                        // Burn on enemies reduces Dark resistance by 0.5% per charge.",

                        // "royalmagetrait2b":
                        // Stealth on heroes increases All Damage by an additional 15% per charge and All Resistances by an additional 5% per charge.",

                        // trait 4a;
                        // Evasion on you can't be purged unless specified. 
                        // Stealth grants 25% additional damage per charge.",

                        // trait 4b:
                        // Heroes Only lose 75% stealth charges rounding down when acting in stealth.

                        traitOfInterest = "royalmagetrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, -0.5f);
                        }
                    }
                    break;

                case "spark":
                    // --- Azshara ---
                    {
                        traitOfInterest = "flamewakertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result = GlobalAuraCurseModifyResist(AtOManager.Instance, __result, Enums.DamageType.Fire, 0, -0.5f);
                            __result.DamageSidesWhenConsumed = 0;
                            __result.DamageSidesWhenConsumedPerCharge = 0;
                            __result.AuraDamageType = Enums.DamageType.Lightning;
                            __result.DamageWhenConsumedPerCharge = 1;
                        }
                    }

                    // --- Franky ---
                    {
                        // "creationtrait0":
                        // Spark on you does not deal damage to sides. 
                        // Regeneration of you increases All Damage by 1% per charge.

                        // "creationtrait2a":
                        // Transform Shadow damage to Lightning damage. - TODO
                        // Sparks on you increase Holy and Lightning damage by 2% per charge

                        traitOfInterest = "creationtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.DamageSidesWhenConsumedPerCharge = 0;
                            __result.DamageWhenConsumedPerCharge = 1;
                        }
                        traitOfInterest = "creationtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Holy, 0, 0, 2);
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Lightning, 0, 0, 2);
                        }
                    }
                    break;

                case "vitality":
                    // --- Bloodrager ---
                    {
                        traitOfInterest = "barbariantrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.MaxMadnessCharges += 100;
                        }
                        traitOfInterest = "barbariantrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModifiedValuePerStack *= 2;
                        }
                        string itemId = "barbarianslasher";
                        // if(IfCharacterHas(characterOfInterest, CharacterHas.Item, itemId, AppliesTo.ThisHero)||
                        //    IfCharacterHas(characterOfInterest, CharacterHas.Item, itemId+"a", AppliesTo.ThisHero)||
                        //    IfCharacterHas(characterOfInterest, CharacterHas.Item, itemId+"b", AppliesTo.ThisHero)
                        // )
                        // {
                        //     __result.AuraDamageType = Enums.DamageType.All;
                        //     __result.AuraDamageIncreasedPerStack = 0.25f;
                        // }
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemId + "rare", AppliesTo.ThisHero)
                        )
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack = 0.5f;
                        }
                        enchantmentOfInterest = "barbariantrait1a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, enchantmentOfInterest, AppliesTo.ThisHero)
                        )
                        {
                            __result.RemoveAuraCurse = (AuraCurseData)null;
                            // __result.RemoveAuraCurse2 = null;
                        }
                    }
                    break;

                case "fury":
                    // --- Bloodrager ---
                    {
                        traitOfInterest = "barbariantrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPercentPerStack *= 0.5f;
                        }
                    }

                    // --- Damali ---
                    {
                        // "minitaurtrait0":
                        // Fury on you increases Speed by 1 per charge.

                        // "minitaurtrait2b":
                        // Trait2b Increases All Damage by 3% per Speed above 15.
                        //  Slow on Enemies reduces Blunt Resistance by 4% per Charge.

                        // trait 4b:
                        // Fury on you increases All Resistances by 0.5% per charge. 
                        // Taunt on you increases All Resistances by 10% per charge. 
                        // Chase Down applies to all Heroes.

                        traitOfInterest = "minitaurtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.Speed;
                            __result.CharacterStatAbsoluteValuePerStack = 1;
                            __result.CharacterStatChargesMultiplierNeededForOne = 2;

                        }
                        traitOfInterest = "minitaurtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = 0.5f;
                        }
                    }
                    break;

                case "sharp":
                    // --- Bloodrager ---
                    {
                        traitOfInterest = "barbariantrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPerStack *= 0.5f;
                            __result.AuraDamageIncreasedPerStack2 *= 0.5f;
                            __result.AuraDamageIncreasedPerStack3 *= 0.5f;
                            __result.AuraDamageIncreasedPerStack4 *= 0.5f;
                        }
                    }

                    // --- Isolde ---
                    {
                        // "sopranotrait2b":
                        // Sharp on enemies reduces All Resistances by 1% per charge.

                        // trait 4b:
                        // Sharp on Monsters reduces All Damage by 0.5 per charge

                        traitOfInterest = "sopranotrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = hasRust ? -4.5f : -3;
                        }
                        traitOfInterest = "sopranotrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.AuraDamageType4 = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack4 = hasRust ? -0.75f : -0.5f;
                        }
                        traitOfInterest = "sopranotrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ConsumedAtTurn = false;
                            __result.AuraConsumed = 0;
                        }
                    }
                    break;

                case "bless":
                    // --- Bloodrager ---
                    {
                        traitOfInterest = "barbariantrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPerStack *= 0.5f;
                            __result.AuraDamageIncreasedPerStack2 *= 0.5f;
                            __result.AuraDamageIncreasedPerStack3 *= 0.5f;
                            __result.AuraDamageIncreasedPerStack4 *= 0.5f;
                        }
                    }
                    break;

                case "mitigate":
                    // --- Cheryl ---
                    {
                        // "royalguardtrait0":
                        // Mitigate caps at 5

                        // "royalguardtrait2a":
                        // Taunt can stack

                        traitOfInterest = "royalguardtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.MaxCharges = __result.MaxMadnessCharges = 5;
                        }
                    }
                    break;

                case "powerful":
                    // --- CursedProdigy ---
                    {
                        traitOfInterest = "cursedprodigytrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.MaxCharges += 10;
                            __result.MaxMadnessCharges += 10;
                        }
                        string itemId = "cursedprodigycursedwandrare";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemId, AppliesTo.Heroes))
                        {
                            __result.MaxCharges += 5;
                            __result.MaxMadnessCharges += 5;
                        }
                    }
                    break;

                case "slow":
                    // --- Damali ---
                    {
                        traitOfInterest = "minitaurtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.ResistModified = Enums.DamageType.Blunt;
                            __result.ResistModifiedPercentagePerStack = -4;
                        }
                        enchant = "minitaurkeepkicking";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, enchant, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                        }
                    }

                    // --- Ekkhrono ---
                    {
                        //2b: Slow on monsters can stack and increases all damage taken by 1 per charge.
                        traitOfInterest = "timeassassintrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                            __result.IncreasedDamageReceivedType = Enums.DamageType.All;
                            __result.IncreasedDirectDamageReceivedPerStack = 1;
                            __result.IncreasedDirectDamageChargesMultiplierNeededForOne = 1;
                        }
                    }
                    break;

                case "fast":
                    // --- Damali ---
                    {
                        enchant = "minitaurbullishbovine";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, enchant, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                        }
                    }
                    break;

                case "dark":
                    // --- Daniel ---
                    {
                        traitOfInterest = "redeemertrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.ExplodeAtStacks = 0;
                            __result.DamageTypeWhenConsumed = Enums.DamageType.None;
                            __result.DamageWhenConsumedPerCharge = 0;
                            __result.HealReceivedPercentPerStack = 1;
                        }
                    }
                    break;

                case "shield":
                    // --- Dorlf ---
                    {
                        traitOfInterest = "irongolemtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = 0.2f;
                        }
                    }
                    break;

                case "stealth":
                    // --- Ekkhrono ---
                    {
                        //4b: Stealth +2. Stealth on this hero cannot be purged. After playing a card while in Stealth, gain 1 Stealth instead of losing charges. (2 times/turn)
                        traitOfInterest = "timeassassintrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.Removable = false;
                        }
                    }

                    // --- Kaa ---
                    {
                        traitOfInterest = "shadowscaletrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack += 5;
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack += 15;
                        }
                    }
                    break;

                case "regeneration":
                    // --- Franky ---
                    {
                        traitOfInterest = "creationtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.All, 0, 0, 1);
                            // int nToAdd = characterOfInterest.GetAuraCharges("spark") / 20;
                            // __result.MaxCharges += nToAdd;
                            // __result.MaxMadnessCharges += nToAdd;
                        }
                    }
                    break;

                case "wet ":
                    // --- Grandchampy ---
                    {
                        // "wizenedtrait0": Thorns on you deal Cold Damage rather than Piercing Damage.

                        // "wizenedtrait2a":

                        // "wizenedtrait2b":

                        // trait 4a;Wet on enemies increases Blunt and Cold damage by 1 per charge.

                        // trait 4b:

                        traitOfInterest = "wizenedtrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.IncreasedDamageReceivedType = Enums.DamageType.Blunt;
                            __result.IncreasedDirectDamageReceivedPerStack = 1f;
                            __result.IncreasedDamageReceivedType2 = Enums.DamageType.Cold;
                            __result.IncreasedDirectDamageReceivedPerStack2 = 1f;
                        }
                    }
                    break;

                case "thorns":
                    // --- Grandchampy ---
                    {
                        traitOfInterest = "wizenedtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.DamageReflectedType = Enums.DamageType.Cold;
                        }
                    }
                    break;

                case "stanzai":
                    // --- Gustavia ---
                    {
                        AppliesTo appliesTo;
                        // "serenadertrait0":
                        // Stanza increases Healing and Mind damage by 3/Stanza

                        // "serenadertrait2b":
                        // Stanza increases Healing and ALL damage by 3/Stanza

                        // trait 4a:
                        // Stanza increases Healing and Mind damage by 3/Stanza for all heroes

                        traitOfInterest = "serenadertrait0";
                        // __result.Removable = true;
                        // __result.GainAuraCurseConsumption = null;
                        appliesTo = characterOfInterest.HaveTrait("serenadertrait4a") ? AppliesTo.Heroes : AppliesTo.ThisHero;
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, appliesTo))
                        {
                            Enums.DamageType damageTypeIncreased = IfCharacterHas(characterOfInterest, CharacterHas.Trait, "serenadertrait2b", appliesTo) ? Enums.DamageType.All : Enums.DamageType.Mind;
                            __result.AuraDamageType = damageTypeIncreased;
                            __result.AuraDamageIncreasedTotal = 3;
                            __result.HealDoneTotal = 3;
                        }
                    }
                    break;

                case "stanzaii":
                    // --- Gustavia ---
                    {
                        AppliesTo appliesTo;
                        traitOfInterest = "serenadertrait0";
                        appliesTo = characterOfInterest.HaveTrait("serenadertrait4b") ? AppliesTo.Heroes : AppliesTo.ThisHero;
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, appliesTo))
                        {
                            Enums.DamageType damageTypeIncreased = IfCharacterHas(characterOfInterest, CharacterHas.Trait, "serenadertrait2b", appliesTo) ? Enums.DamageType.All : Enums.DamageType.Mind;
                            __result.AuraDamageType = damageTypeIncreased;
                            __result.AuraDamageIncreasedTotal = 6;
                            __result.HealDoneTotal = 6;
                        }
                    }
                    break;

                case "stanzaiii":
                    // --- Gustavia ---
                    {
                        AppliesTo appliesTo;
                        traitOfInterest = "serenadertrait0";
                        appliesTo = characterOfInterest.HaveTrait("serenadertrait4b") ? AppliesTo.Heroes : AppliesTo.ThisHero;
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, appliesTo))
                        {
                            Enums.DamageType damageTypeIncreased = IfCharacterHas(characterOfInterest, CharacterHas.Trait, "serenadertrait2b", appliesTo) ? Enums.DamageType.All : Enums.DamageType.Mind;
                            __result.AuraDamageType = damageTypeIncreased;
                            __result.AuraDamageIncreasedTotal = 9;
                            __result.HealDoneTotal = 9;
                        }
                    }
                    break;

                case "crack":
                    // --- Hecar ---
                    {
                        // traitOfInterest = "moontouchedtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "moontouchedtrait3b", AppliesTo.Monsters) ||
                            IfCharacterHas(characterOfInterest, CharacterHas.Item, "moontouchedtrait3ba", AppliesTo.Monsters) ||
                            IfCharacterHas(characterOfInterest, CharacterHas.Item, "moontouchedtrait3bb", AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Mind, 0, -1.0f);
                        }
                    }
                    break;

                case "poison":
                    // --- Jason ---
                    {
                        traitOfInterest = "afflictortrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.Preventable = false;
                            __result.Removable = false;
                        }
                        // traitOfInterest = "afflictortrait4b";
                        // if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        // {
                        //     __result.MaxCharges = -1;
                        //     __result.MaxMadnessCharges = -1;
                        // }
                    }
                    break;

                case "evasion":
                    // --- Kaa ---
                    {
                        // "shadowscaletrait2a":
                        // Evasion on you stacks and increases All Damage by 1 per charge. 

                        // "shadowscaletrait2b":
                        // Stealth on heroes increases All Damage by an additional 15% per charge and All Resistances by an additional 5% per charge.",

                        // trait 4a;
                        // Evasion on you can't be purged unless specified. 
                        // Stealth grants 25% additional damage per charge.",

                        // trait 4b:
                        // Heroes Only lose 75% stealth charges rounding down when acting in stealth.

                        traitOfInterest = "shadowscaletrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.GainCharges = true;
                            __result.AuraDamageType = Enums.DamageType.All;
                            float multiplierAmount = 1.0f;  //characterOfInterest.HaveTrait("shadowscaletrait4a") ? 0.3f : 0.2f;
                            __result.AuraDamageIncreasedPerStack = multiplierAmount;
                            // __result.HealDoneTotal = Mathf.RoundToInt(multiplierAmount * characterOfInterest.GetAuraCharges("shield"));
                        }
                        traitOfInterest = "shadowscaletrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                        }
                    }
                    break;
            }

            // --- Kaerion ---
            traitOfInterest = "shadowknighttrait4a";
            if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters) && !__result.IsAura)
            {
                __result.DamageWhenConsumedPerCharge *= 1.25f;
            }

            switch (_acId)
            {
                case "zeal":
                    // --- Laios ---
                    {
                        // "enforcertrait2a":
                        // Crack on monsters increases Holy Damage taken by 1 per charge. 
                        // Sanctify reduces Blunt resistance by 0.5% per charge.

                        // "enforcertrait2b":
                        // 

                        // trait 4a;
                        // 

                        // trait 4b:
                        // 

                        traitOfInterest = "enforcertrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.ConsumedAtTurn = false;
                            __result.ConsumedAtTurnBegin = false;
                        }
                    }

                    // --- Tellann ---
                    {
                        // "exaltedtrait0":  

                        // "exaltedtrait2a":
                        // Zeal on heroes can stack up to 20, but all charges are lost at the end of turn.              

                        // "exaltedtrait2b":
                        // Burn on you increases Shadow Damage by 0.5 per charge. 
                        // Dark on enemies increases Fire Damage received by 1 per charge.

                        // "exaltedtrait4a":
                        // When a hero hits a monster with Dark, they heal 1 HP per charge

                        // "exaltedtrait4b":
                        // Burn on allies no longer reduces resistances. At the start of your turn, reduce the cost of your highest cost card by one for every 30 Burn on you.

                        traitOfInterest = "exaltedtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.MaxCharges = __result.MaxMadnessCharges = 10;
                            __result.GainCharges = true;
                            // __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, 1, 0); ;
                            __result.HealDonePercentPerStack = 10;
                        }
                    }

                    // --- Penitent ---
                    {
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, "penitenttrait4a", AppliesTo.ThisHero))
                        {

                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack = 1 + nInjuries;
                            __result.HealDonePerStack = 1 + nInjuries;
                        }
                    }
                    break;

                case "sanctify":
                    // --- Laios ---
                    {
                        traitOfInterest = "enforcertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Blunt, 0, -0.5f);
                        }
                    }

                    // --- Tai ---
                    {
                        // "dualisttrait2a":
                        // Double's Sanctify's effectiveness, but 
                        // Sanctify explodes at 38 charges, dealing 2 Shadow Damage per charge. 
                        // Dark explosions deal Holy Damage.


                        // "dualisttrait2b":
                        // Sanctify +2, Dark +2

                        // trait 4a;
                        // When dark or Sanctify explode, randomly apply energize and dark or inspire and sanctify to a random hero

                        // trait 4b:
                        // Dark and Sanctify explosions deal 30% more damage.

                        traitOfInterest = "dualisttrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {

                            __result.HealAttackerPerStack *= 2;
                            __result.ResistModifiedPercentagePerStack *= 2;
                            __result.ResistModifiedPercentagePerStack2 *= 2;
                            __result.ExplodeAtStacks = 38;
                            __result.DamageWhenConsumedPerCharge = 2;
                            __result.DamageTypeWhenConsumed = Enums.DamageType.Shadow;
                        }
                        traitOfInterest = "dualisttrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result.DamageWhenConsumedPerCharge *= 1.3f;
                        }
                    }
                    break;

                case "crack":
                    // --- Laios ---
                    {
                        traitOfInterest = "enforcertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            // __result = GlobalAuraCurseModifyDamage(__instance, __result, Enums.DamageType.Holy, 0, 1, 0);
                            __result.IncreasedDamageReceivedType2 = Enums.DamageType.Holy;
                            __result.IncreasedDirectDamageReceivedPerStack2 = 1;
                        }
                    }

                    // --- Tristan ---
                    {
                        traitOfInterest = "owlknightcognitivecalm";
                        // AppliesTo appliesTo = characterOfInterest.HaveTrait("owlknighttrait2a") ? AppliesTo.Heroes : AppliesTo.ThisHero;
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.AuraDamageType = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPerStack = 0;
                            __result.AuraDamageType2 = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPerStack2 = 0;
                            __result.AuraDamageType3 = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPerStack3 = 0;
                            __result.AuraDamageType4 = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPerStack4 = 0;
                            __result.HealAttackerPerStack = 1;
                            __result.HealAttackerConsumeCharges = 1;
                        }
                    }
                    break;

                case "fortify":
                    // --- Malakir ---
                    {
                        // "transmutertrait4a":
                        // Fortify and Sharp on heroes increases all damage by 0.25

                        traitOfInterest = "transmutertrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.AuraDamageType2 = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack2 = 0.25f;
                        }
                    }

                    // --- Tristan ---
                    {
                        // "owlknighttrait0":
                        // Fortify on you increases Mind Damage by 1 per charge and stacks to 50.

                        traitOfInterest = "owlknighttrait0";
                        AppliesTo appliesTo = characterOfInterest.HaveTrait("owlknighttrait2a") ? AppliesTo.Heroes : AppliesTo.ThisHero;
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, appliesTo))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = 50;
                            __result.MaxMadnessCharges = 50;
                            __result.AuraDamageType4 = Enums.DamageType.Mind;
                            __result.AuraDamageIncreasedPerStack4 = 1;
                        }
                    }
                    break;

                case "sharp":
                    // --- Malakir ---
                    {
                        traitOfInterest = "transmutertrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.AuraDamageType4 = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack4 = 0.25f;
                        }
                    }
                    break;

                case "poison":
                    // --- Malia ---
                    {
                        // "snaketrait0":
                        // Poison no longer deals Damage to you, instead it reduces Block gained by 1 and increases Max HP by 2 per charge. 
                        // When you gain Block, suffer that much Poison -this is not affected by modifiers-

                        // "snaketrait2a":

                        // "snaketrait2b":
                        // Poison increases healing received by 

                        // trait 4a;

                        // trait 4b:
                        // Immune to Slow. Chill no longer reduces your Speed. When you play a Defense with cost >=3, dispel Chill and Slow on all other heroes (once per turn).

                        traitOfInterest = "snaketrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                            __result.DamageWhenConsumedPerCharge = 0;
                            __result.CharacterStatModified = Enums.CharacterStat.Hp;
                            __result.CharacterStatModifiedValuePerStack = 2 * GetRustMultiplier(characterOfInterest, _acId);
                        }

                        traitOfInterest = "snaketrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.HealReceivedPercentPerStack = 1;
                        }
                    }

                    // --- Pestily ---
                    {
                        if (_type == "set")
                        {
                            if (_characterTarget != null && __instance.team.CharacterHaveTrait(_characterTarget.SubclassName, "pestilyshadowpoison"))
                            {
                                __result.AuraDamageType = Enums.DamageType.Shadow;
                                int damageIncrease = FloorToInt((float)_characterTarget.GetAuraCharges("poison") * 0.10f);
                                //__result.AuraDamageIncreasedPerStack=0.1f;
                                __result.AuraDamageIncreasedTotal = damageIncrease;
                            }
                            if (_characterTarget != null && __instance.team.CharacterHaveTrait(_characterTarget.SubclassName, "pestilyantidote"))
                            {
                                __result.MaxCharges = 300;
                                __result.ProduceDamageWhenConsumed = false;
                                __result.DamageWhenConsumedPerCharge = 0.0f;
                            }
                        }
                        if (_type == "consume")
                        {
                            if (_characterCaster != null && __instance.team.CharacterHaveTrait(_characterCaster.SubclassName, "pestilyantidote"))
                            {
                                __result.MaxCharges = 300;
                                __result.ProduceDamageWhenConsumed = false;
                                __result.DamageWhenConsumedPerCharge = 0.0f;
                            }
                        }
                    }
                    break;

                case "chill":
                    // --- Malia ---
                    {
                        traitOfInterest = "snaketrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.None;
                            __result.CharacterStatModifiedValuePerStack = 0;
                            __result.CharacterStatChargesMultiplierNeededForOne = 1;
                            __result.ChargesAuxNeedForOne2 = 0;
                        }
                    }

                    // --- Ratone ---
                    {
                        traitOfInterest = "ratkingtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.Removable = false;
                        }
                    }

                    // --- Rosalinde ---
                    {
                        traitOfInterest = "augurtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Holy, 0, -0.2f);
                        }
                    }

                    // --- Ursur ---
                    {
                        traitOfInterest = "ursurbearlynoticeable";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = 0.25f;

                        }

                        traitOfInterest = "ursurunbearable";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.None;
                            __result.CharacterStatChargesMultiplierNeededForOne = 0;

                        }
                    }
                    break;

                case "dark":
                    // --- Malukah ---
                    {
                        // "voodoowitchtrait2a":

                        // "voodoowitchtrait2b":
                        // Sanctify increases Dark Explosion damage by 2% per charge of Sanctify

                        // trait 4a;
                        // Dark on Heroes stacks to 32 charges. Vitality on heroes increases All Damage by 1% per charge.",

                        // trait 4b:
                        // Shadow and Holy Damage +30%. Dark reduces Holy Resistance by 1% per charge.",

                        traitOfInterest = "voodoowitchtrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.MaxCharges = 32;
                            __result.MaxMadnessCharges = 32;
                        }
                        traitOfInterest = "voodoowitchtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result.ResistModified = Enums.DamageType.Holy;
                            __result.ResistModifiedPercentagePerStack = -1.0f;
                        }
                    }

                    // --- Ratone ---
                    {
                        // "ratkingtrait2a":
                        // +8 to stacks needed to explode dark, 

                        // "ratkingtrait2b":
                        // Chill on you cannot be dispelled unless specified and 
                        // increases Dark explosion damage by 1% per charge

                        // trait 4a;
                        // Dark on you cannot be dispelled unless specified. 
                        // Dark on you increases All Damage by 2 per charge.

                        // trait 4b:
                        // Vermintide adds an infestation for every 2 dark on you. 
                        // Winter Night increases Dark explosion damage by 2% per charge.

                        traitOfInterest = "ratkingtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ExplodeAtStacks += 8;
                        }
                        traitOfInterest = "ratkingtrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.Removable = false;
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPerStack = 1;
                        }
                        traitOfInterest = "ratkingtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            float percentPerCharge = AtOManager.Instance.team.TeamHaveTrait("ratkingtrait4b") ? 0.02f : 0.01f;
                            Character ratone = GetCharacterBySubclass("ratking");
                            float multiplier = 1 + percentPerCharge * (ratone != null ? ratone.GetAuraCharges("chill") : 0);
                            __result.DamageWhenConsumedPerCharge *= multiplier;
                        }
                    }

                    // --- Tai ---
                    {
                        traitOfInterest = "dualisttrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.DamageTypeWhenConsumed = Enums.DamageType.Holy;
                        }
                        traitOfInterest = "dualisttrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.DamageWhenConsumedPerCharge *= 1.3f;
                        }
                    }

                    // --- Tellann ---
                    {
                        traitOfInterest = "exaltedtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.IncreasedDamageReceivedType = Enums.DamageType.Fire;
                            __result.IncreasedDirectDamageReceivedPerStack = 1;
                        }
                        traitOfInterest = "exaltedtrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.HealAttackerPerStack = 1;
                            __result.HealAttackerConsumeCharges = 1;
                        }
                    }
                    break;

                case "vitality":
                    // --- Malukah ---
                    {
                        traitOfInterest = "voodoowitchtrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack += 1.0f;
                        }
                    }

                    // --- Splody ---
                    {
                        traitOfInterest = "thebombtrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModifiedValuePerStack = Mathf.RoundToInt(__result.CharacterStatModifiedValuePerStack * 1.5f);
                        }
                    }

                    // --- Penitent ---
                    {
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, "penitenttrait4b", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            // __result.AuraDamageIncreasedPerStack = FloorToInt(characterOfInterest.GetAuraCharges("vitality")*0.14286f);
                            __result.AuraDamageIncreasedPerStack = 0.3f;
                        }
                    }
                    break;

                case "evasion":
                    // --- Monty ---
                    {
                        // "bunnytrait0":
                        // Evasion on heroes and monsters can stack, 
                        // but can also be consumed to prevent one instance of a curse.

                        // "bunnytrait2a":
                        //Bunny Fur - Buffer +1. 
                        // Buffer on heroes can stack and prevents 2 instances of a curse per charge.

                        // "bunnytrait2b":
                        // Fast on you can stack. 


                        // trait 4a;

                        // trait 4b:
                        // Fast +1. Buffer on heroes increases all damage by 1 per charge.

                        traitOfInterest = "bunnytrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Global))
                        {
                            __result.PreventedAuraCurseStackPerStack = 1;
                            __result.GainCharges = true;
                        }
                    }
                    break;

                case "buffer":
                    // --- Monty ---
                    {
                        traitOfInterest = "bunnytrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.GainCharges = true;
                            __result.MaxCharges = __result.MaxMadnessCharges = 10;
                            __result.PreventedAuraCurseStackPerStack += 1;
                        }
                        traitOfInterest = "bunnytrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.All, 0, 1, 0);
                        }
                    }
                    break;

                case "fast":
                    // --- Monty ---
                    {
                        traitOfInterest = "bunnytrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.GainCharges = true;
                        }
                    }

                    // --- Nenukil ---
                    {
                        if (CharacterHasTraitGACM(characterOfInterest, "mountedcannonaltered", AppliesTo.Heroes))
                        {
                            __result.ConsumedAtTurn = true;
                            __result.ConsumedAtTurnBegin = false;
                        }
                        // Greased gears makes Fast stack
                        if (CharacterHasTraitGACM(characterOfInterest, "greasedgearsaltered", AppliesTo.Heroes))
                        {
                            __result.GainCharges = true;
                            __result.ConsumeAll = true;
                        }
                    }

                    // --- Simone ---
                    {
                        // "ambushertrait2a":
                        // Fast on you does not increase speed, 
                        // but increases Stealth Damage by 3% per charge

                        // "ambushertrait2b":

                        // trait 4a;

                        // trait 4b:
                        // Mark on enemies increases All Damage received by 3 per charge.

                        traitOfInterest = "ambushertrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.None;
                            __result.CharacterStatAbsoluteValuePerStack = 0;
                        }
                    }
                    break;

                case "burn":
                    // --- Nenukil ---
                    {
                        if (CharacterHasTraitGACM(characterOfInterest, "mountedcannonaltered", AppliesTo.Heroes))
                        {
                            __result.DamageWhenConsumedPerCharge = 0.5f;
                        }
                        if (CharacterHasTraitGACM(characterOfInterest, "greasedgearsaltered", AppliesTo.Heroes))
                        {
                            __result.AuraDamageType = Enums.DamageType.Blunt;
                            __result.AuraDamageIncreasedPercentPerStack = 1.0f;
                        }
                    }

                    // --- Rosalinde ---
                    {
                        traitOfInterest = "augurtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Holy, 0, -0.2f);
                        }
                    }

                    // --- Splody ---
                    {
                        traitOfInterest = "thebombtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack = 2;
                        }
                    }

                    // --- Tellann ---
                    {
                        traitOfInterest = "exaltedtrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            // __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.Shadow, 0, 0.5f, 0); ;
                            // __result.AuraDamageType = Enums.DamageType.Shadow;
                            // __result.AuraDamageIncreasedPerStack = 0.5f;
                            __result.IncreasedDamageReceivedType = Enums.DamageType.Shadow;
                            __result.IncreasedDirectDamageReceivedPerStack = 0.25f;

                        }
                        traitOfInterest = "exaltedtrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            __result.ResistModified = __result.ResistModified2 = __result.ResistModified3 = Enums.DamageType.None;
                            __result.ResistModifiedPercentagePerStack = __result.ResistModifiedPercentagePerStack2 = __result.ResistModifiedPercentagePerStack3 = 0;
                        }
                    }
                    break;

                case "spark":
                    // --- Rosalinde ---
                    {
                        traitOfInterest = "augurtrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result = GlobalAuraCurseModifyResist(__instance, __result, Enums.DamageType.Holy, 0, -0.2f);
                        }
                    }
                    break;

                case "shield":
                    // --- Salara ---
                    {
                        // "savanttrait4a":
                        // Shield on Hero increases All Damage and Healing Done by 0.2 per charge

                        traitOfInterest = "savanttrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.Mind;
                            float multiplierAmount = 0.2f;  //characterOfInterest.HaveTrait("savanttrait4a") ? 0.3f : 0.2f;
                            __result.AuraDamageIncreasedPerStack = multiplierAmount;
                            // __result.HealDoneTotal = Mathf.RoundToInt(multiplierAmount * characterOfInterest.GetAuraCharges("shield"));
                        }
                    }
                    break;

                case "insane":
                    // --- Salara ---
                    {
                        traitOfInterest = "savanttrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            // __result.ResistModified = Enums.DamageType.Mind;                        
                            __result.ResistModifiedPercentagePerStack -= 1;
                            // __result.HealDoneTotal = Mathf.RoundToInt(multiplierAmount * characterOfInterest.GetAuraCharges("shield"));
                        }
                    }

                    // --- Tristan ---
                    {
                        traitOfInterest = "owlknighttrait0";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.ResistModified2 = Enums.DamageType.Blunt;
                            __result.ResistModifiedPercentagePerStack2 = -0.3f;
                        }
                    }
                    break;

                case "block":
                    // --- Senenthia ---
                    {
                        // "castletrait2a":

                        // "castletrait2b":

                        // trait 4a;

                        // trait 4b: Block on you increases All Damage by 2% per charge. When you deal damage, reduce Block by 10%.

                        traitOfInterest = "castletrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result = GlobalAuraCurseModifyDamage(AtOManager.Instance, __result, Enums.DamageType.All, 0, 0, 2);
                        }
                    }
                    break;

                case "stealth":
                    // --- Simone ---
                    {
                        traitOfInterest = "ambushertrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPercentPerStack += 2 * characterOfInterest.GetAuraCharges("fast");
                        }
                    }
                    break;

                case "mark":
                    // --- Simone ---
                    {
                        traitOfInterest = "ambushertrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPerStack = 3;
                        }
                    }

                    // --- Tripp ---
                    {
                        // "trappertrait2a":

                        // "trappertrait2b":
                        // Mark on enemies decreases speed by 1 per charge. 

                        // trait 4a;
                        // Slow on enemies can stack
                        // trait 4b:

                        traitOfInterest = "trappertrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.Speed;
                            __result.CharacterStatAbsoluteValuePerStack = -1;
                        }
                    }
                    break;

                case "powerful":
                    // --- Penitent ---
                    {
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, "penitenttrait0", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPercentPerStack = -5;
                        }
                    }

                    // --- Trickster ---
                    {
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, "tricksterdrawpower", AppliesTo.ThisHero))
                        {
                            __result.MaxCharges += 5;
                            __result.MaxMadnessCharges += 5;
                            __result.AuraConsumed += 2;
                            // __result.ConsumeAll=true;
                        }
                    }

                    // --- Wukong ---
                    {
                        // trait 4a;
                        // Taunt on enemies increases their maximum Vulnerable charges by 4 per charge. 
                        // Taunt on heroes increases their maximum Powerful charges by 3 per charge.

                        // trait 4b:
                        // Adds Twin Magic (Buffed Twin Scrolls) to all heroes decks. 
                        // Sight on enemies reduces All Resistances by 0.5% per charge.

                        traitOfInterest = "tacticiantrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Heroes))
                        {
                            int amountToAdd = 3 * characterOfInterest.GetAuraCharges("taunt");
                            __result.MaxCharges += amountToAdd;
                            __result.MaxMadnessCharges += amountToAdd;
                            // __result.HealDoneTotal = Mathf.RoundToInt(multiplierAmount * characterOfInterest.GetAuraCharges("shield"));
                        }
                    }
                    break;

                case "weak":
                    // --- Penitent ---
                    {
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, "penitenttrait0", AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType = Enums.DamageType.None;
                            __result.AuraDamageIncreasedPercent = 0;
                            __result.HealDonePercent = 0;
                        }
                    }
                    break;

                case "thorns":
                    // --- Thornton ---
                    {
                        // "cactustrait2b":
                        // Thorns on you increases all resists by 0.1% per charge

                        traitOfInterest = "cactustrait2b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = 0.1f;
                        }
                        traitOfInterest = "cactustrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.DamageReflectedModifierType = Enums.RefectedDamageModifierType.DamagePerAuraCharge;
                            __result.DamageReflectedMultiplier = Mathf.FloorToInt(1 + 0.05f * characterOfInterest.GetAuraCharges("vitality"));
                        }
                    }
                    break;

                case "slow":
                    // --- Tripp ---
                    {
                        traitOfInterest = "trappertrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.GainCharges = true;
                        }
                    }
                    break;

                case "zealotry":
                    // --- Tusk ---
                    {
                        // "walrustrait2a":

                        // "walrustrait2b":

                        // trait 4a;

                        // trait 4b:

                        traitOfInterest = "walrustrait2a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageIncreasedPercentPerStack = 3.0f;
                        }
                    }
                    break;

                case "bleed":
                    // --- Ursur ---
                    {
                        traitOfInterest = "ursurbearlynoticeable";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                        {
                            __result.AuraDamageType2 = Enums.DamageType.All;
                            __result.AuraDamageIncreasedPercentPerStack2 = 1.5f;

                            // __result.ProduceDamageWhenConsumed = false;
                            __result.DamageWhenConsumedPerCharge *= 0.5f;

                        }
                    }
                    break;

                case "vulnerable":
                    // --- Wukong ---
                    {
                        traitOfInterest = "tacticiantrait4a";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            int amountToAdd = 4 * characterOfInterest.GetAuraCharges("taunt");
                            __result.MaxCharges += amountToAdd;
                            __result.MaxMadnessCharges += amountToAdd;
                            // __result.HealDoneTotal = Mathf.RoundToInt(multiplierAmount * characterOfInterest.GetAuraCharges("shield"));
                        }
                    }
                    break;

                case "sight":
                    // --- Wukong ---
                    {
                        traitOfInterest = "tacticiantrait4b";
                        if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                        {
                            __result.ResistModified = Enums.DamageType.All;
                            __result.ResistModifiedPercentagePerStack = -0.5f;
                        }
                    }
                    break;
            }
        }

        public static void UpdateMaxMadnessChargesByItem(ref AuraCurseData __result, Character characterOfInterest, string itemID)
        {
            if (__result == null)
            {
                return;
            }

            AppliesTo appliesTo = __result.IsAura ? AppliesTo.Heroes : AppliesTo.Monsters;

            if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID + "rare", appliesTo))
            {
                ItemData itemData = Globals.Instance.GetItemData(itemID + "rare");
                if (itemData == null)
                    return;

                if (__result.MaxCharges != -1)
                {
                    __result.MaxCharges += itemData.AuracurseCustomModValue1;
                }
                if (__result.MaxMadnessCharges != -1)
                {
                    __result.MaxMadnessCharges += itemData.AuracurseCustomModValue1;
                }
            }
            else if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, appliesTo))
            {
                ItemData itemData = Globals.Instance.GetItemData(itemID);
                if (itemData == null)
                    return;

                if (__result.MaxCharges != -1)
                {
                    __result.MaxCharges += itemData.AuracurseCustomModValue1;
                }
                if (__result.MaxMadnessCharges != -1)
                {
                    __result.MaxMadnessCharges += itemData.AuracurseCustomModValue1;
                }
            }
        }

        public static float GetRustMultiplier(Character characterOfInterest, string _acId)
        {
            bool hasRust = false;
            if (characterOfInterest != null)
            {
                hasRust = characterOfInterest.HasEffect("rust");
            }

            float rustMultiplier = hasRust ? 1.5f : 1.0f;
            if (TeamHasPerk("mainperkrust0b") && hasRust && (_acId == "crack" || _acId == "poison" || _acId == "slow"))
            {
                // rust0b: Rust on enemies instead increases the effect of Crack, Poison and Slow by 20% per charge
                int nRust = characterOfInterest.GetAuraCharges("rust");
                rustMultiplier = 1.0f + 0.2f * nRust;
            }
            return rustMultiplier;
        }

        public static Character GetCharacterBySubclass(string subclassName)
        {
            if (AtOManager.Instance == null || string.IsNullOrEmpty(subclassName))
                return null;
            foreach (Character character in AtOManager.Instance.team.heroes)
            {
                if (character != null && character.SubclassName != null &&
                    character.SubclassName.ToLower().Contains(subclassName.ToLower()))
                {
                    return character;
                }
            }
            return null;
        }

    }
}
