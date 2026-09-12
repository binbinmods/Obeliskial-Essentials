using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleMatch;
using BepInEx;
using Cards;
using HarmonyLib;
using UnityEngine;
using static Obeliskial_Essentials.CombatFunctions;
using static Obeliskial_Essentials.Essentials;
using static Obeliskial_Essentials.TraitReversePatches;

namespace Obeliskial_Essentials
{
    [HarmonyPatch]
    public static class TraitFunctionsForHeroes
    {
        public class TraitContext
        {
            public Trait trait;
            public TraitData traitData;
            public string traitName;
            public string traitId;
            public Character _character;
            public Character _target;
            public CardRealtimeData _castedCard;
            public int _auxInt;
            public string _auxString;
            public Enums.EventActivation _theEvent;
            public List<string> heroHand;
            public List<CardRealtimeData> cardDataList;
            public List<Character> teamHero;
            public List<Character> teamNpc;
        }

        delegate void CustomTraitAction(TraitContext ctx);

        static int pestilyLevel5ActivationCounter = 0;
        static int pestilyLevel5MaxActivations = 3;

        static readonly Dictionary<string, CustomTraitAction> TraitHandlers = new()
        {
            ["overlordtrait0"] = OverlordTrait0,
            ["overlordtrait2b"] = OverlordTrait2b,
            ["sharktrait4a"] = SharkTrait4a,
            ["sharktrait4b"] = SharkTrait4b,
            ["overseertrait0"] = OverseerTrait0,
            ["overseertrait4a"] = OverseerTrait4a,
            ["cryohealertrait2a"] = CryohealerTrait2a,
            ["cryohealertrait2b"] = CryohealerTrait2b,
            ["cryohealertrait4b"] = CryohealerTrait4b,
            ["flamewakertrait2b"] = FlamewakerTrait2b,
            ["flamewakertrait4a"] = FlamewakerTrait4a,
            ["flamewakertrait4b"] = FlamewakerTrait4b,
            ["barbariantrait2a"] = BarbarianTrait2a,
            ["barbariantrait2b"] = BarbarianTrait2b,
            ["barbariantrait4a"] = BarbarianTrait4a,
            ["tipsytrait0"] = TipsyTrait0,
            ["thermomancertrait0"] = ThermomancerTrait0,
            ["thermomancertrait2a"] = ThermomancerTrait2a,
            ["thermomancertrait2b"] = ThermomancerTrait2b,
            ["crawlertrait2a"] = CrawlerTrait2a,
            ["crawlertrait2b"] = CrawlerTrait2b,
            ["crawlertrait4b"] = CrawlerTrait4b,
            ["royalguardtrait0"] = RoyalguardTrait0,
            ["royalguardtrait2a"] = RoyalguardTrait2a,
            ["cursedprodigytrait2b"] = CursedprodigyTrait2b,
            ["cursedprodigytrait4a"] = CursedprodigyTrait4a,
            ["cursedprodigytrait4b"] = CursedprodigyTrait4b,
            ["bruisertrait0"] = BruiserTrait0,
            ["bruisertrait2a"] = BruiserTrait2a,
            ["bruisertrait2b"] = BruiserTrait2b,
            ["bruisertrait4a"] = BruiserTrait4a,
            ["bruisertrait4b"] = BruiserTrait4b,
            ["minitaurtrait2a"] = MinitaurTrait2a,
            ["minitaurtrait4a"] = MinitaurTrait4a,
            ["redeemertrait2a"] = RedeemerTrait2a,
            ["redeemertrait2b"] = RedeemerTrait2b,
            ["redeemertrait4b"] = RedeemerTrait4b,
            ["irongolemtrait0"] = IrongolemTrait0,
            ["irongolemtrait2b"] = IrongolemTrait2b,
            ["irongolemtrait4a"] = IrongolemTrait4a,
            ["yetitrait0"] = YetiTrait0,
            ["yetitrait2a"] = YetiTrait2a,
            ["yetitrait2b"] = YetiTrait2b,
            ["yetitrait4a"] = YetiTrait4a,
            ["yetitrait4b"] = YetiTrait4b,
            ["immutabletrait0"] = ImmutableTrait0,
            ["ebonywarriortrait0"] = EbonywarriorTrait0,
            ["ebonywarriortrait4a"] = EbonywarriorTrait4a,
            ["ebonywarriortrait4b"] = EbonywarriorTrait4b,
            ["timeassassintrait0"] = TimeassassinTrait0,
            ["timeassassintrait2a"] = TimeassassinTrait2a,
            ["fabricatortrait0"] = FabricatorTrait0,
            ["fabricatortrait2b"] = FabricatorTrait2b,
            ["fabricatortrait4b"] = FabricatorTrait4b,
            ["creationtrait2b"] = CreationTrait2b,
            ["creationtrait4a"] = CreationTrait4a,
            ["icebreakertrait0"] = IcebreakerTrait0,
            ["icebreakertrait2b"] = IcebreakerTrait2b,
            ["icebreakertrait4a"] = IcebreakerTrait4a,
            ["icebreakertrait4b"] = IcebreakerTrait4b,
            ["wizenedtrait2a"] = WizenedTrait2a,
            ["wizenedtrait2b"] = WizenedTrait2b,
            ["wizenedtrait4a"] = WizenedTrait4a,
            ["wizenedtrait4b"] = WizenedTrait4b,
            ["serenadertrait2a"] = SerenaderTrait2a,
            ["serenadertrait2b"] = SerenaderTrait2b,
            ["serenadertrait4a"] = SerenaderTrait4a,
            ["royalmagetrait0"] = RoyalmageTrait0,
            ["royalmagetrait2b"] = RoyalmageTrait2b,
            ["royalmagetrait4a"] = RoyalmageTrait4a,
            ["royalmagetrait4b"] = RoyalmageTrait4b,
            ["moontouchedtrait2b"] = MoontouchedTrait2b,
            ["moontouchedtrait4b"] = MoontouchedTrait4b,
            ["sopranotrait0"] = SopranoTrait0,
            ["sopranotrait2a"] = SopranoTrait2a,
            ["sopranotrait4a"] = SopranoTrait4a,
            ["afflictortrait2a"] = AfflictorTrait2a,
            ["afflictortrait2b"] = AfflictorTrait2b,
            ["afflictortrait4b"] = AfflictorTrait4b,
            ["shadowscaletrait0"] = ShadowscaleTrait0,
            ["shadowscaletrait2a"] = ShadowscaleTrait2a,
            ["shadowknighttrait0"] = ShadowknightTrait0,
            ["shadowknighttrait2a"] = ShadowknightTrait2a,
            ["shadowknighttrait2b"] = ShadowknightTrait2b,
            ["shadowknighttrait4b"] = ShadowknightTrait4b,
            ["executionertrait0"] = ExecutionerTrait0,
            ["executionertrait2a"] = ExecutionerTrait2a,
            ["executionertrait2b"] = ExecutionerTrait2b,
            ["enforcertrait0"] = EnforcerTrait0,
            ["enforcertrait2b"] = EnforcerTrait2b,
            ["enforcertrait4a"] = EnforcerTrait4a,
            ["enforcertrait4b"] = EnforcerTrait4b,
            ["transmutertrait0"] = TransmuterTrait0,
            ["transmutertrait2a"] = TransmuterTrait2a,
            ["transmutertrait2b"] = TransmuterTrait2b,
            ["transmutertrait4b"] = TransmuterTrait4b,
            ["snaketrait2a"] = SnakeTrait2a,
            ["snaketrait2b"] = SnakeTrait2b,
            ["snaketrait4a"] = SnakeTrait4a,
            ["snaketrait4b"] = SnakeTrait4b,
            ["voodoowitchtrait2a"] = VoodoowitchTrait2a,
            ["witchtrait0"] = WitchTrait0,
            ["witchtrait4a"] = WitchTrait4a,
            ["bunnytrait2b"] = BunnyTrait2b,
            ["bunnytrait4a"] = BunnyTrait4a,
            ["loadedgunaltered"] = Loadedgunaltered,
            ["mountedcannonaltered"] = Mountedcannonaltered,
            ["exoskeletonaltered"] = Exoskeletonaltered,
            ["pestilybioheal"] = Pestilybioheal,
            ["pestilyshadowpoison"] = Pestilyshadowpoison,
            ["pestilyantidote"] = Pestilyantidote,
            ["pestilyhealingtoxins"] = Pestilyhealingtoxins,
            ["pestilytoxichealing"] = Pestilytoxichealing,
            ["ratkingtrait2a"] = RatkingTrait2a,
            ["ratkingtrait4a"] = RatkingTrait4a,
            ["riftlingtrait0"] = RiftlingTrait0,
            ["riftlingtrait2a"] = RiftlingTrait2a,
            ["riftlingtrait2b"] = RiftlingTrait2b,
            ["augurtrait0"] = AugurTrait0,
            ["augurtrait2a"] = AugurTrait2a,
            ["augurtrait2b"] = AugurTrait2b,
            ["augurtrait4a"] = AugurTrait4a,
            ["augurtrait4b"] = AugurTrait4b,
            ["sufferertrait2a"] = SuffererTrait2a,
            ["sufferertrait2b"] = SuffererTrait2b,
            ["sufferertrait4a"] = SuffererTrait4a,
            ["sufferertrait4b"] = SuffererTrait4b,
            ["savanttrait2a"] = SavantTrait2a,
            ["savanttrait2b"] = SavantTrait2b,
            ["savanttrait4a"] = SavantTrait4a,
            ["savanttrait4b"] = SavantTrait4b,
            ["castletrait0"] = CastleTrait0,
            ["castletrait2a"] = CastleTrait2a,
            ["castletrait2b"] = CastleTrait2b,
            ["castletrait4a"] = CastleTrait4a,
            ["castletrait4b"] = CastleTrait4b,
            ["kingtrait0"] = KingTrait0,
            ["kingtrait2a"] = KingTrait2a,
            ["ambushertrait0"] = AmbusherTrait0,
            ["ambushertrait4a"] = AmbusherTrait4a,
            ["thebombtrait2b"] = ThebombTrait2b,
            ["thebombtrait4a"] = ThebombTrait4a,
            ["dualisttrait0"] = DualistTrait0,
            ["dualisttrait4b"] = DualistTrait4b,
            ["exaltedtrait0"] = ExaltedTrait0,
            ["exaltedtrait4b"] = ExaltedTrait4b,
            ["penitenttrait2a"] = PenitentTrait2a,
            ["penitenttrait2b"] = PenitentTrait2b,
            ["penitenttrait4b"] = PenitentTrait4b,
            ["trickstermagictrick"] = Trickstermagictrick,
            ["trickstertrickupyoursleeve"] = Trickstertrickupyoursleeve,
            ["tricksterlearnrealmagic"] = Tricksterlearnrealmagic,
            ["tricksterdistractingact"] = Tricksterdistractingact,
            ["cactustrait0"] = CactusTrait0,
            ["cactustrait2a"] = CactusTrait2a,
            ["trappertrait0"] = TrapperTrait0,
            ["trappertrait2a"] = TrapperTrait2a,
            ["trappertrait4a"] = TrapperTrait4a,
            ["owlknighttrait2a"] = OwlknightTrait2a,
            ["owlknighttrait2b"] = OwlknightTrait2b,
            ["owlknighttrait4b"] = OwlknightTrait4b,
            ["walrustrait0"] = WalrusTrait0,
            ["ulfvitrcalltherain"] = CallTheRain,
            ["ulfvitrmagnet"] = Magnet,
            ["ulfvitrregenerator"] = Regenerator,
            ["ulfvitrconductor"] = Conductor,
            ["ulfvitrlifebloom"] = LifeBloom,
            ["ursurursineblood"] = UrsineBlood,
            ["ursurbristlyhide"] = BristlyHide,
            ["ursurbearwithit"] = BearWithIt,
            ["ursurunbearable"] = Ursurunbearable,
            ["tacticiantrait0"] = TacticianTrait0,
            ["tacticiantrait2a"] = TacticianTrait2a,
        };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Trait), "DoTrait")]
        public static void DoTraitPostfix(Enums.EventActivation _theEvent, string _trait, Character _character, Character _target, int _auxInt, string _auxString, CardRealtimeData _castedCard, Trait __instance)
        {
            if (MatchManager.Instance == null)
                return;
            if (!TraitHandlers.TryGetValue(_trait, out CustomTraitAction handler))
                return;
            if (!IsLivingHero(_character))
                return;

            TraitData traitData = Globals.Instance.GetTraitData(_trait);
            TraitContext ctx = new TraitContext
            {
                trait = __instance,
                traitData = traitData,
                traitName = traitData != null ? traitData.TraitName : _trait,
                traitId = _trait,
                _character = _character,
                _target = _target,
                _castedCard = _castedCard,
                _auxInt = _auxInt,
                _auxString = _auxString,
                _theEvent = _theEvent,
                cardDataList = [],
                teamHero = AtOManager.Instance != null && AtOManager.Instance.team != null ? new List<Character>(AtOManager.Instance.team.heroes.ToArray()) : [],
                teamNpc = MatchManager.Instance.GetTeamNPC() != null ? MatchManager.Instance.GetTeamNPC().Characters : [],
            };
            ctx.heroHand = GetHeroHandList(ctx);
            handler(ctx);
        }

        #region Ainz
        static void OverlordTrait0(TraitContext ctx)
        {
            // "overlordtrait0":
            // At the start of each turn, apply 2 Scourge to a random monster.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
            randomEnemy?.SetAuraTrait(ctx._character, "scourge", 2);
            // DisplayTraitScroll(ctx._character, ctx.traitData, "shadowimpact1", randomEnemy);
        }

        static void OverlordTrait2b(TraitContext ctx)
        {
            // "overlordtrait2b":
            // When you play a card that costs 6 or more Energy, refund 2.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard != null && MatchManager.Instance.energyJustWastedByHero >= 7)
            {
                GainEnergy(ctx._character, 2, ctx.traitData);
            }
        }

        #endregion

        #region Akula
        static void SharkTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // when you play a Defense, reduce your highest cost card by 2 until discarded(3 uses)
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(Enums.CardType.None, ctx.heroHand);
                ReduceCardCost(ref highestCostCard, amountToReduce: 2, isPermanent: false);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void SharkTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // Heal yourself for 30% of damage done. 
            // All Damage for all heroes is increased by 1% per Speed. 
            // Your damage is increased by 2% per Speed instead.
            Vampirism(ref ctx._character, ctx._auxInt, 0.3f, ctx._castedCard);
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
        }

        #endregion

        #region Albedo
        static void OverseerTrait0(TraitContext ctx)
        {
            // "overseertrait0":
            // At the start of combat, gain 26 Block and 1 foritfy.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "block", 26);
            ctx._character.SetAuraTrait(ctx._character, "fortify", 1);
            ctx._character.HeroItem?.ScrollCombatText(ctx.traitName, Enums.CombatScrollEffectType.Trait);
        }

        static void OverseerTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Sharp +1. When you play a Melee Attack, gain 1 Sharp.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard != null && ctx._castedCard.HasCardType(Enums.CardType.Melee_Attack))
            {
                ctx._character.SetAuraTrait(ctx._character, "sharp", 1);
                if (ctx._character.HeroItem != null)
                {
                    ctx._character.HeroItem.ScrollCombatText(ctx.traitName, Enums.CombatScrollEffectType.Trait);
                    EffectsManager.Instance.PlayEffectAC("sharp", isHero: true, ctx._character.HeroItem.CharImageT, flip: false);
                }
            }
        }

        #endregion

        #region Aurelion
        static void CryohealerTrait2a(TraitContext ctx)
        {
            // "cryohealertrait2a"
            // When you play a Defense that costs Energy, 
            // refund 1 Energy and suffer 2 Chill. (3 times/turn)

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                ctx._character?.SetAuraTrait(ctx._character, "chill", 2);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void CryohealerTrait2b(TraitContext ctx)
        {
            // "cryohealertrait2b":
            // At the start of your turn, reduce the cost of all Healing Spells by 1 until discarded.
            Mastery([Enums.CardType.Healing_Spell], 1, "Healing Mastery");
        }

        static void CryohealerTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // When you play a Defense, heal all allies equal to 0.2 their Chill and Draw a card. (3x/turn)
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

                for (int i = 0; i < ctx.teamHero.Count; i++)
                {
                    Character hero = ctx.teamHero[i];
                    if (IsLivingHero(hero))
                    {
                        int healAmount = Mathf.RoundToInt(0.2f * hero.GetAuraCharges("chill"));
                        if (healAmount > 0)
                        {
                            TraitHeal(ref ctx._character, hero, healAmount, ctx.traitName);
                            LogDebug($"Healing {hero.Id} for {healAmount} from Trait {ctx.traitId}");
                        }
                    }
                }
                DrawCards(1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Azshara
        static void FlamewakerTrait2b(TraitContext ctx)
        {
            // "flamewakertrait2b":
            // When you play a Lightning Spell, reduce the cost of the highest cost Fire Spell by 1 until discarded. When you play a Fire Spell, reduce the cost of the highest cost Lighting Spell by 1 until discarded. (3 times/turn)
            DualityCardType(ctx._character, ctx._castedCard, [Enums.CardType.Lightning_Spell], [Enums.CardType.Fire_Spell], ctx.traitId);
        }

        static void FlamewakerTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Whenever you play a Skill, Draw a Book. Whenver you play a Book, Draw a Skill. (3 times/turn)
            // if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Skill))
            // {
            //     LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            //     DrawCards(1, Enums.CardFrom.Deck, 0, [Enums.CardType.Book]);
            //     IncrementTraitActivations(ctx.traitId);
            // }
            // if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Book))
            // {
            //     LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            //     DrawCards(1, Enums.CardFrom.Deck, 0, [Enums.CardType.Skill]);
            //     IncrementTraitActivations(ctx.traitId);
            // }
            if (CanIncrementTraitActivations(ctx.traitId) && (ctx._castedCard.HasCardType(Enums.CardType.Skill) || ctx._castedCard.HasCardType(Enums.CardType.Book)))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                DrawCards(1);//, Enums.CardFrom.Deck, 0, [Enums.CardType.Book]);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void FlamewakerTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // Playing a Non-Fire or Non-Lightning Spell will add a random Fire or Lightning Spell of that costs one more to your hand. This card has its cost reduced by 3.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            bool AppropriateCardType = ctx._castedCard.HasCardType(Enums.CardType.Spell) && !(ctx._castedCard.HasCardType(Enums.CardType.Lightning_Spell) || ctx._castedCard.HasCardType(Enums.CardType.Fire_Spell));
            if (CanIncrementTraitActivations(ctx.traitId) && AppropriateCardType)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                int cost = MatchManager.Instance.energyJustWastedByHero + 1;
                bool vanish = false;
                int costReduction = 3;
                bool costZero = false;
                bool permanentCostReduction = true;

                string randomCard = GetRandomCardOfTypeAndCost(Enums.HeroClass.Mage, [Enums.CardType.Lightning_Spell, Enums.CardType.Fire_Spell], cost);//, costReduction: -3, vanish: false, permanentCostReduction: true);
                if (randomCard.IsNullOrWhiteSpace())
                {
                    LogError($"No card found for trait {ctx.traitId} with cost {cost}");
                    return;
                }
                AddCardToHand(randomCard, false, vanish, costZero, costReduction, permanentCostReduction);

                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Bloodrager
        static void BarbarianTrait2a(TraitContext ctx)
        {
            // Speed -2. All resistances -40%.  -- Done in JSON
            // Double your current Max HP and your HP gained by Vitality. -- Done in AssignTraitPostfix
            // At the start of combat, gain 2 vitality 

            // Handled in GACM/GetTraitAuraCurseModifiers
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

            if (IsLivingHero(ctx._character))
            {
                ctx._character.SetAuraTrait(ctx._character, "vitality", 2);
            }
        }

        static void BarbarianTrait2b(TraitContext ctx)
        {
            // trait 2b:  At the start of your turn, reduce the cost of your highest cost card by 3 until discarded.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (IsLivingHero(ctx._character))
            {
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(Enums.CardType.None, ctx.heroHand);
                ReduceCardCost(ref highestCostCard, ctx._character, 3, isPermanent: false);
            }
        }

        static void BarbarianTrait4a(TraitContext ctx)
        {
            // When you apply Vitality to another character, gain that much Vitality (unaffected by modifiers). 
            // +100 Max Vitality charges for you.

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (IsLivingHero(ctx._character) && ctx._target.Alive && ctx._target != null && ctx._auxInt != 0 && ctx._auxString == "vitality" && ctx._target.SourceName != ctx._character.SourceName)
            {
                ctx._character.SetAuraCurse(ctx._character, GetAuraCurseData("vitality"), ctx._auxInt, fromTrait: true, useCharacterMods: false);
            }
        }

        #endregion

        #region Bombur
        static void TipsyTrait0(TraitContext ctx)
        {
            // "tipsytrait0":
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "evade", 1);
        }

        #endregion

        #region Caldris
        static void ThermomancerTrait0(TraitContext ctx)
        {
            // if an enemy has Burn, apply Chill 2 to them. If an enemy has Chill, apply Burn 2 to them. 
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            for (int i = 0; i < ctx.teamNpc.Count; i++)
            {
                Character npc = ctx.teamNpc[i];
                if (!IsLivingNPC(npc)) continue;
                if (npc.HasEffect("burn"))
                {
                    npc.SetAuraTrait(ctx._character, "chill", 2);
                }
                if (npc.HasEffect("chill"))
                {
                    npc.SetAuraTrait(ctx._character, "burn", 2);
                }

            }
        }

        static void ThermomancerTrait2a(TraitContext ctx)
        {
            // "thermomancertrait2a"
            // Whenever you apply Chill, deal 2 Fire damage to the target.
            if (ctx._auxString == "chill")
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                if (IsLivingNPC(ctx._target))
                {
                    int damageAmount = ctx._character?.DamageWithCharacterBonus(2, Enums.DamageType.Fire, Enums.CardClass.None) ?? 2;
                    ctx._target.IndirectDamage(Enums.DamageType.Fire, damageAmount, ctx._character);
                }
            }
        }

        static void ThermomancerTrait2b(TraitContext ctx)
        {
            // "thermomancertrait2b":
            // Stealth on heroes increases All Damage by an additional 15% per charge and All Resistances by an additional 5% per charge.",
            if (ctx._auxString == "burn")
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                if (IsLivingNPC(ctx._target))
                {
                    int damageAmount = ctx._character?.DamageWithCharacterBonus(2, Enums.DamageType.Cold, Enums.CardClass.None) ?? 2;
                    ctx._target.IndirectDamage(Enums.DamageType.Cold, damageAmount, ctx._character);
                }
            }
        }

        #endregion

        #region Carl
        static void CrawlerTrait2a(TraitContext ctx)
        {
            // "crawlertrait2a"
            // When you play a Flask or Attack that costs energy, 
            // Refund 1 and apply 2 Burn or 2 Crack to a random enemy respectively. (3 times/turn)                

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Flask) && ctx._castedCard.HasCardType(Enums.CardType.Attack) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
                randomEnemy?.SetAuraTrait(ctx._character, "crack", 2);
                randomEnemy?.SetAuraTrait(ctx._character, "burn", 2);
                IncrementTraitActivations(ctx.traitId);
            }
            else if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Flask) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
                randomEnemy?.SetAuraTrait(ctx._character, "burn", 2);
                IncrementTraitActivations(ctx.traitId);
            }
            else if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Attack) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
                randomEnemy?.SetAuraTrait(ctx._character, "crack", 2);
                ctx._character?.ModifyEnergy(1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void CrawlerTrait2b(TraitContext ctx)
        {
            // "crawlertrait2b":
            // When hit, Gain 2 Vitality.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character?.SetAuraTrait(ctx._character, "vitality", 2);
        }

        static void CrawlerTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // Whenever you play a Flask, Draw 1.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard.HasCardType(Enums.CardType.Flask))
            {
                DrawCards(1);
            }
        }

        #endregion

        #region Cheryl
        static void RoyalguardTrait0(TraitContext ctx)
        {
            // At the start of combat, apply 3 Mitigate to all heros
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ApplyAuraCurseToAll("mitigate", 3, AppliesTo.Heroes, sourceCharacter: ctx._character, useCharacterMods: true, isPreventable: true);
        }

        static void RoyalguardTrait2a(TraitContext ctx)
        {
            // "royalguardtrait2a"
            // For every Taunt you apply, apply 1 Mitigate

            if (ctx._auxString == "taunt")
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                LogDebug($"{ctx._character.Id} is applying Mitigate to {ctx._target.Id}");
                ctx._target.SetAuraTrait(ctx._character, "mitigate", ctx._auxInt);
            }
        }

        #endregion

        #region CursedProdigy
        static void CursedprodigyTrait2b(TraitContext ctx)
        {
            // trait 2b: At the start of your turn, increase the cost of all of your cards by 1 until discarded. Then half the cost of the highest cost Fire Spell in your hand. Repeat for Cold, Lighting, Shadow, and Curse Spells.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (!AtOManager.Instance.team.TeamHaveTrait("cursedprodigytrait4a") && IsLivingHero(ctx._character))
            {
                for (int i = 0; i < ctx.heroHand.Count; i++)
                {
                    CardRealtimeData cardData = MatchManager.Instance.GetCardData(ctx.heroHand[i]);
                    ReduceCardCost(ref cardData, ctx._character, -1);
                }
            }
            // LogDebug("Increased Costs");

            Enums.CardType[] cardTypes = [Enums.CardType.Fire_Spell, Enums.CardType.Cold_Spell, Enums.CardType.Lightning_Spell, Enums.CardType.Shadow_Spell, Enums.CardType.Curse_Spell];
            foreach (Enums.CardType cardType in cardTypes)
            {
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(cardType, ctx.heroHand);
                if (highestCostCard == null)
                {
                    continue;
                }
                int energy = highestCostCard.EnergyCost - highestCostCard.EnergyReductionPermanent - highestCostCard.EnergyReductionTemporal;
                LogDebug($"Highest cost card: {highestCostCard.CardName} with energy {energy}, cost {highestCostCard.EnergyCost}, reduction {highestCostCard.EnergyReductionPermanent}, temporal reduction {highestCostCard.EnergyReductionTemporal}");
                if (highestCostCard != null && IsLivingHero(ctx._character)) //energy >= 6 && 
                {
                    int amountToReduce = Mathf.FloorToInt(energy / 2);
                    ReduceCardCost(ref highestCostCard, ctx._character, amountToReduce);
                }
            }

            // DisplayTraitScroll(ref ctx._character, ctx.traitData);
        }
        public static int firstCurseDamage = 0;
        public static HashSet<Enums.CardType> empoweredTypes = [];
        public static int damageMultiplier = 0;
        // public static HashSet<Enums.CardType> firstCurseTypes = [Enums.CardType.Fire_Spell, Enums.CardType.Cold_Spell, Enums.CardType.Lightning_Spell];

        static void CursedprodigyTrait4a(TraitContext ctx)
        {
            // The first Curse you play each turn deals triple damage. Cursed Elements gives +2 Elemental Charges for every 3 Curse Spells in your deck. Sorcerous Mastery no longer increases the cost.
            // done in GetTraitAuraCurseModifiersPostfix and GetTraitDamagePercentModifiersPostfix
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard != null && ctx._castedCard.HasCardType(Enums.CardType.Curse_Spell))
            {
                firstCurseDamage = 200;
                IncrementTraitActivations(ctx.traitId);
            }
            else
            {
                firstCurseDamage = 0;
            }
        }

        static void CursedprodigyTrait4b(TraitContext ctx)
        {
            // TODO: Implement this
            // Fire Empowers Cold, Cold Empowers Lightning, Lightning Empowers Fire. Empowered Spells deal 30% bonus damage that is increased by 30% for each consecutively played Empowered Spell this turn.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            HashSet<Enums.CardType> cardSet = [.. ctx._castedCard.CardTypeAux];
            cardSet.Add(ctx._castedCard.CardType);
            cardSet.IntersectWith(empoweredTypes);
            // if (cardSet.Count > 0)
            // {
            //     damageMultiplier += 30;
            //     empoweredTypes = [];
            //     foreach (Enums.CardType cardType in cardSet)
            //     {
            //         empoweredTypes.Add(GetEmpoweredType(cardType));
            //     }
            // }
            // else
            // {
            //     // empoweredTypes = [];
            //     empoweredTypes = [Enums.CardType.Fire_Spell, Enums.CardType.Cold_Spell, Enums.CardType.Lightning_Spell];
            //     damageMultiplier = 0;
            // }
        }

        #endregion

        #region Cyro
        static void BruiserTrait0(TraitContext ctx)
        {
            // Cold Aura: Immune to Chill. At the start of turn, apply 3 chill to all monsters
            LogDebug($"trait - {ctx.traitName}");
            int nToAdd = ctx._character.HaveTrait("bruisertrait4a") ? 5 : 3;
            ApplyAuraCurseToAll("chill", nToAdd, AppliesTo.Monsters, ctx._character, useCharacterMods: true);
        }

        static void BruiserTrait2a(TraitContext ctx)
        {
            //Bruiser Duality:  When you play a Warrior card, reduce the cost of the highest Scout card by 1 until discarded. When you play a Scout card, reduce the cost of the highest Warrior card by 1 until discarded. (4 times/turn)
            LogDebug($"trait - {ctx.traitName}");
            Duality(ctx.trait, ctx._character, ctx._castedCard, Enums.CardClass.Warrior, Enums.CardClass.Scout, ctx.traitId, extraChargeTrait: "bruisertrait4a");
        }

        static void BruiserTrait2b(TraitContext ctx)
        {
            // Dragonoid Flexibility: When you play a Warrior card, apply 5 block to all heroes. When you play a Scout card, apply 1 vulnerable to all monsters. (4 times/turn)
            LogDebug($"trait - {ctx.traitName}");
            int bonusActivations = ctx._character.HaveTrait("bruisertrait4b") ? 1 : 0;
            if (CanIncrementTraitActivations(ctx.traitId, bonusActivations))
            {
                LogDebug("canIncrementTraitActivations - True");
                if (ctx._castedCard.CardClass == Enums.CardClass.Warrior)
                {
                    ApplyAuraCurseToAll("block", 5, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
                    IncrementTraitActivations(ctx.traitId);
                    // LogDebug("Displaying charges - bonusActivations");

                    // DisplayRemainingChargesForTrait(ref ctx._character,ctx.traitId, bonusActivations:bonusActivations);
                }
                if (ctx._castedCard.CardClass == Enums.CardClass.Scout)
                {
                    ApplyAuraCurseToAll("vulnerable", 1, AppliesTo.Monsters, ctx._character, useCharacterMods: true);
                    IncrementTraitActivations(ctx.traitId);
                    // DisplayRemainingChargesForTrait(ref ctx._character, ctx.traitId, bonusActivations: bonusActivations);
                }
                LogDebug($"trait END - {ctx.traitName}");

            }
        }

        static void BruiserTrait4a(TraitContext ctx)
        {
            // Dragonoid's Aura: Chill +1. Cold Aura applies 2 additional Chill. Apply 10 block to self at start of turn. +1 Duality Activation
            LogDebug($"trait - {ctx.traitName}");
            // ApplyAuraCurseToAll("vulnerable", 1, AppliesTo.Monsters, ctx._character);
            ctx._character.SetAuraTrait(ctx._character, "block", 10);
        }

        static void BruiserTrait4b(TraitContext ctx)
        {
            // Chill +3, Fast +1, Vulnerable +1. -20% Resistances. Cold-Blooded: Fast +1. Cold Aura applies 1 fast to all heroes. Apply 2 sharpen to self at start of turn. +1 Flexibility Activation.
            LogDebug($"trait - {ctx.traitName}");
            // ApplyAuraCurseToAll("evasion", 1, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
            ApplyAuraCurseToAll("fast", 1, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
            ctx._character.SetAuraTrait(ctx._character, "sharp", 2);
            ctx._character.SetAuraTrait(ctx._character, "evasion", 1);
        }

        #endregion

        #region Damali
        static void MinitaurTrait2a(TraitContext ctx)
        {
            // "minitaurtrait2a"
            // Whenever you apply Slow, gain 4 block for each applied

            if (ctx._auxString == "slow")
            {
                // LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character.SetAuraTrait(ctx._character, "block", 4);
            }
        }

        static void MinitaurTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // At the start of your turn, reduce the cost of all cards by 1 for every 20 Fury on you.

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ReduceCostByStacks(Enums.CardType.None, "fury", 20, ref ctx._character, ref ctx.heroHand, ref ctx.cardDataList, ctx.traitName, true);
        }

        #endregion

        #region Daniel
        static void RedeemerTrait2a(TraitContext ctx)
        {
            // When you play a Healer card, reduce the cost of the highest cost Mage card in your hand by 1 until discarded. When you play a Mage card, reduce the cost of the highest cost Healer card in your hand by 1 until discarded. (3 times/turn)
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Duality(ctx.trait, ctx._character, ctx._castedCard, Enums.CardClass.Mage, Enums.CardClass.Healer, ctx.traitId, extraChargeTrait: "redeemertrait4a");
        }

        static void RedeemerTrait2b(TraitContext ctx)
        {
            // When you play a "Fire Spell" card Purge 1, "Holy Spell" card gain 1 Bless, "Shadow Spell" card increase curse charges on all monsters by 10%. (6 times/turn)
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

            if (CanIncrementTraitActivations(ctx.traitId))
            {
                if (ctx._castedCard.HasCardType(Enums.CardType.Fire_Spell))
                {
                    LogDebug($"Handling Trait {ctx.traitId}: Purging");

                    Character randomNpc = GetRandomCharacter(ctx.teamNpc);
                    randomNpc.DispelAuras(1);

                    IncrementTraitActivations(ctx.traitId);
                    // DisplayRemainingChargesForTrait(ref ctx._character, ctx.traitData);
                    // DisplayTraitScroll(ref ctx._character, ctx.traitData);
                }
                if (ctx._castedCard.HasCardType(Enums.CardType.Holy_Spell))
                {
                    LogDebug($"Handling Trait {ctx.traitId}: Gaining Bless");
                    ctx._character.SetAuraTrait(ctx._character, "bless", 1);

                    IncrementTraitActivations(ctx.traitId);
                    // DisplayRemainingChargesForTrait(ref ctx._character, ctx.traitData);
                    // DisplayTraitScroll(ref ctx._character, ctx.traitData);
                }
                if (ctx._castedCard.HasCardType(Enums.CardType.Shadow_Spell))
                {
                    LogDebug($"Handling Trait {ctx.traitId}: Increasing Curses");
                    foreach (Character npc in ctx.teamNpc)
                    {
                        if (IsLivingNPC(npc))
                        {
                            ModifyAllAurasOrCursesByPercent(10, IsAuraOrCurse.Curse, npc, ctx._character);
                        }
                    }
                    IncrementTraitActivations(ctx.traitId);
                    // DisplayRemainingChargesForTrait(ref ctx._character, ctx.traitData);
                    // DisplayTraitScroll(ref ctx._character, ctx.traitData);
                }
            }
        }

        static void RedeemerTrait4b(TraitContext ctx)
        {
            // At the end of your turn, grant 2 Bless and 2 Zeal to all heroes, and transform all Dark charges on heroes into Burn charges.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            foreach (Character hero in ctx.teamHero)
            {
                if (!IsLivingHero(hero))
                {
                    continue;
                }
                hero.SetAuraTrait(ctx._character, "bless", 2);
                hero.SetAuraTrait(ctx._character, "zeal", 2);
                int nDark = hero.GetAuraCharges("dark");
                hero.HealAuraCurse(GetAuraCurseData("dark"));
                hero.SetAuraTrait(ctx._character, "burn", nDark);
            }
        }

        #endregion

        #region Dorlf
        static void IrongolemTrait0(TraitContext ctx)
        {
            // Taunt on you cannot be purged unless specified. At the start of combat, gain 1 Taunt, 1 Reinforce, and 2 Fortify
            ctx._character.SetAuraTrait(ctx._character, "taunt", 1);
            ctx._character.SetAuraTrait(ctx._character, "reinforce", 1);
            ctx._character.SetAuraTrait(ctx._character, "fortify", 2);
        }

        static void IrongolemTrait2b(TraitContext ctx)
        {
            // "irongolemtrait2b":
            // Taunt on you can stack up to 10. 
            // Reduce the cost of your highest cost card by 1 until discarded, repeat for every 3 Taunt on you.


            int nIterations = 1 + ctx._character.GetAuraCharges("taunt") / 3;
            for (int i = 0; i < nIterations; i++)
            {
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(Enums.CardType.None);
                if (highestCostCard != null)
                {
                    LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName} - Reducing cost of {highestCostCard.CardName} by 1");
                    ReduceCardCost(ref highestCostCard, ctx._character, 1);
                }
            }
        }

        static void IrongolemTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // When you play a Defense, reduce the cost of your highest cost Defense by 3 until discarded. (2 times/turn)

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense))
            {
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(Enums.CardType.Defense);
                if (highestCostCard != null)
                {
                    ReduceCardCost(ref highestCostCard, ctx._character, 3);
                }
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Drogg
        static void YetiTrait0(TraitContext ctx)
        {
            // Transform damage to Blunt
            // Start of Turn suffer 5 Chill
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "chill", 5);
        }

        static void YetiTrait2a(TraitContext ctx)
        {
            // "yetitrait2a"
            //Your Cold Spells are also Ranged Attacks. 
            // When you play a Cold Spell that costs Energy, refund 1 Energy and apply 1 Crack to a random enemy. (2 times/turn)                

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Cold_Spell))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character.ModifyEnergy(1);
                Character randNPC = GetRandomCharacter(ctx.teamNpc);
                randNPC.SetAuraTrait(ctx._character, "crack", 1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void YetiTrait2b(TraitContext ctx)
        {
            // trait 2b:  
            // Your Cold Spells are also Defenses. 
            // When you play a Cold Spell that costs Energy, refund 1 Energy and apply 1 Fortify to the hero with the highest Block. (2 times/turn)

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Cold_Spell))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                List<Character> highestBlockHeroes = [];
                Character highestBlockHero = null;
                int curBlock = 0;
                foreach (Character hero in ctx.teamHero)
                {
                    if (IsLivingHero(hero) && hero.GetAuraCharges("block") > curBlock)
                    {
                        curBlock = hero.GetAuraCharges("block");
                        highestBlockHeroes = [hero];
                    }
                    else if (IsLivingHero(hero) && hero.GetAuraCharges("block") == curBlock)
                    {
                        highestBlockHeroes.Add(hero);
                    }
                }
                if (highestBlockHeroes == null)
                {
                    return;
                }
                else
                {
                    highestBlockHero = GetRandomCharacter(highestBlockHeroes.ToArray());
                }
                ctx._character?.ModifyEnergy(1);
                highestBlockHero?.SetAuraTrait(ctx._character, "fortify", 1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void YetiTrait4a(TraitContext ctx)
        {
            // "yetitrait4a":
            // When you play a Cold Spell, suffer 2 Chill. When you play a Fire Spell, gain 2 Fury.

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard.HasCardType(Enums.CardType.Cold_Spell))
            {
                ctx._character.SetAuraTrait(ctx._character, "chill", 2);
            }
            if (ctx._castedCard.HasCardType(Enums.CardType.Fire_Spell))
            {
                ctx._character.SetAuraTrait(ctx._character, "fury", 2);
            }
        }

        static void YetiTrait4b(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // "yetitrait4b":
            // When you play a Defense, gain 2 Block and apply 2 Chill to all enemies.
            if (ctx._castedCard.HasCardType(Enums.CardType.Defense))
            {
                ctx._character.SetAuraTrait(ctx._character, "block", 2);
                ApplyAuraCurseToAll("chill", 2, AppliesTo.Monsters, ctx._character, useCharacterMods: true);
            }
        }

        #endregion

        #region Durin
        static void ImmutableTrait0(TraitContext ctx)
        {
            // "immutabletrait0":
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "evade", 1);
        }

        #endregion

        #region EbonyWarrior
        static void EbonywarriorTrait0(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // "ebonywarriortrait0":
            if (MatchManager.Instance.GetCurrentRound() != 1)
            {
                return;
            }
            // At the start of combat, gain 22 Block and 4 Regeneration
            ctx._character.SetAuraTrait(ctx._character, "block", 22);
            ctx._character.SetAuraTrait(ctx._character, "regeneration", 4);
        }

        static void EbonywarriorTrait4a(TraitContext ctx)
        {
            // "ebonywarriortrait4a":
            // Decay on Monsters can stack. 
            // At the start of your turn, apply 1 Decay to a random Monster for each Regeneration on you. - Does not gain bonuses -

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Dictionary<Character, int> npcDecay = new();
            List<Character> npcs = new();
            foreach (Character npc in ctx.teamNpc)
            {
                if (IsLivingNPC(npc))
                {
                    npcDecay.Add(npc, 0);
                    npcs.Add(npc);
                }
            }
            if (npcs.Count == 0)
            {
                LogDebug("No NPCs to apply decay to");
                return;
            }
            for (int i = 0; i < ctx._character.GetAuraCharges("regeneration"); i++)
            {
                // apply decay to a random monster
                int randomIndex = SafeRandomInt(0, npcs.Count);
                npcDecay[npcs[randomIndex]]++;
            }

            foreach (KeyValuePair<Character, int> kvp in npcDecay)
            {
                Character target = kvp.Key;
                int decayToApply = kvp.Value;
                if (decayToApply > 0)
                {
                    target.SetAuraCurse(target, GetAuraCurseData("decay"), decayToApply, useCharacterMods: false);
                }
            }
        }

        static void EbonywarriorTrait4b(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // "ebonywarriortrait4b":
            // When you are dealt damage, gain Block equal to half that amount. - Does not gain bonuses -
            int blockToGain = Mathf.RoundToInt(ctx._auxInt);
            ctx._character.SetAuraCurse(ctx._character, GetAuraCurseData("block"), blockToGain, useCharacterMods: false);
        }

        #endregion

        #region Ekkhrono
        static void TimeassassinTrait0(TraitContext ctx)
        {
            // Immune to Slow. At the start of your turn, apply 1 Slow to all characters.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

            if (!IsLivingHero(ctx._character))
            {
                return;
            }

            ApplyAuraCurseToAll("slow", 1, AppliesTo.Monsters, ctx._character, true);
        }

        static void TimeassassinTrait2a(TraitContext ctx)
        {
            // When you play a \"Spell\" card, reduce the cost of the highest cost \"Attack\" card in your hand by 1.  When you play an \"Attack\" card, reduce the cost of the highest cost \"Spell\" card in your hand by 1. (2 times/turn)
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

            Duality(ctx.trait, ctx._character, ctx._castedCard, Enums.CardClass.Scout, Enums.CardClass.Mage, ctx.traitId, permanentReduction: true);
        }

        #endregion

        #region Fabricator
        static void FabricatorTrait0(TraitContext ctx)
        {
            // When you play an Enchantment on a hero grant 1 Inspire and 1 Energize
            // LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // if (ctx._castedCard != null)
            // {
            //     LogDebug($"Casting {ctx._castedCard.Id}");
            // }

            if (ctx._castedCard != null && ctx._castedCard.HasCardType(Enums.CardType.Enchantment))
            {
                LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");
                if (MatchManager.Instance == null) { return; }

                Transform targetTransform = Traverse.Create(MatchManager.Instance).Field("targetTransform").GetValue<Transform>(); ;
                if (targetTransform == null) { LogDebug("null transform"); return; }

                Character targetHero = MatchManager.Instance.GetHeroById(targetTransform.name);
                if (!IsLivingHero(targetHero)) { return; }

                targetHero.SetAuraCurse(ctx._character, GetAuraCurseData("inspire"), 1, useCharacterMods: false);
                targetHero.SetAuraCurse(ctx._character, GetAuraCurseData("energize"), 1, useCharacterMods: false);
                // DisplayTraitScroll(ref ctx._character, ctx.traitData);

            }
        }

        static void FabricatorTrait2b(TraitContext ctx)
        {
            // Taunt +1. Handled in json.
            // Taunt on this hero can stack. -- handled in GACM
            // This hero gains 5% more Block and Shield for each stack of Taunt.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._auxInt >= 0 && (ctx._auxString == "shield" || ctx._auxString == "block"))
            {
                LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");
                AuraCurseData shieldOrBlock = GetAuraCurseData(ctx._auxString);
                int bonusCharges = Mathf.RoundToInt(ctx._auxInt * 0.05f * ctx._character.GetAuraCharges("taunt"));
                ctx._character.SetAuraCurse(ctx._character, shieldOrBlock, bonusCharges, useCharacterMods: false);

            }
            // DisplayTraitScroll(ref ctx._character, ctx.traitData);
        }

        static void FabricatorTrait4b(TraitContext ctx)
        {
            // At end of turn, all heroes gain 10 Block and Shield for every Taunt you have.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            int nToApply = 10 * ctx._character.GetAuraCharges("taunt");
            ApplyAuraCurseToAll("block", nToApply, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
            ApplyAuraCurseToAll("shield", nToApply, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
            // DisplayTraitScroll(ref ctx._character, ctx.traitData);
        }

        #endregion

        #region Franky
        static void CreationTrait2b(TraitContext ctx)
        {
            // "creationtrait2b":
            // When you heal an ally with Spark, apply 1 Bless, 3 Spark, and gain 1 Energy. (3 times/turn)

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._target.HasEffect("spark"))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._target?.SetAuraTrait(ctx._character, "bless", 1);
                ctx._target?.SetAuraTrait(ctx._character, "spark", 3);
                ctx._character?.ModifyEnergy(1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void CreationTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Bless +1. When you apply Bless, deal 6 Lightning Damage to a random enemy.


            if (ctx._auxString == "bless")
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
                int damageToDeal = ctx._character.DamageWithCharacterBonus(6, Enums.DamageType.Lightning, Enums.CardClass.None);
                randomEnemy.IndirectDamage(Enums.DamageType.Lightning, damageToDeal, ctx._character, null, "");
                EffectsManager.Instance.PlayEffectAC("lightningimpact2", false, randomEnemy.NPCItem.CharImageT, true);
            }
        }

        #endregion

        #region Graendor
        static void IcebreakerTrait0(TraitContext ctx)
        {
            // "icebreakertrait0": On Block, apply 3 Chill to target
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "chill", 3);
        }

        static void IcebreakerTrait2b(TraitContext ctx)
        {
            // "icebreakertrait2b":
            // On Block, AoE 10 Shield
            if (CanIncrementTraitActivations(ctx.traitId, useRound: true))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ApplyAuraCurseToAll("shield", 10, AppliesTo.Heroes, sourceCharacter: ctx._character, useCharacterMods: true);
                IncrementTraitActivations(ctx.traitId, useRound: true);
            }
        }

        static void IcebreakerTrait4a(TraitContext ctx)
        {
            // trait 4a; When you play a defense, draw 1
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                DrawCards(1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void IcebreakerTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // When you play an attack, gain 1 regen
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Attack))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

                ctx._character?.SetAuraTrait(ctx._character, "regeneration", 1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Grandchampy
        static void WizenedTrait2a(TraitContext ctx)
        {
            // "wizenedtrait2a": When you play a Defense that costs energy Refund 1. Can be activated an additional time each turn for every 20 Chill on you. (Once per turn)
            int bonusActivations = ctx._character.EffectCharges("chill") / 20;
            if (CanIncrementTraitActivations(ctx.traitId, bonusActivations: bonusActivations) && ctx._castedCard.HasCardType(Enums.CardType.Defense) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                IncrementTraitActivations(ctx.traitId);
                GainEnergy(ctx._character, 1);
                ApplyAuraCurseToAll("wet", 1, AppliesTo.Monsters, sourceCharacter: ctx._character, useCharacterMods: true);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void WizenedTrait2b(TraitContext ctx)
        {
            // "wizenedtrait2b": When you apply Chill, gain 1 Thorns 
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._auxString == "chill" && ctx._auxInt > 0)
            {
                ctx._target.SetAuraTrait(ctx._character, "thorns", 1);
                DisplayTraitScroll(ref ctx._character, ctx.traitData);
            }
        }

        static void WizenedTrait4a(TraitContext ctx)
        {
            // trait 4a; On hit, apply 2 Wet. Wet increases Blunt and Cold damage by 1 per charge.
            if (IsLivingNPC(ctx._target))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._target.SetAuraTrait(ctx._character, "wet", 2);
            }
        }

        static void WizenedTrait4b(TraitContext ctx)
        {
            // trait 4b: At the start of your turn, apply Block equal to your Chill to all heroes.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            int chillAmount = ctx._character.EffectCharges("chill");
            if (chillAmount > 0)
            {
                ApplyAuraCurseToAll("block", chillAmount, AppliesTo.Heroes, sourceCharacter: ctx._character, useCharacterMods: true);
            }
        }

        #endregion

        #region Gustavia
        static void SerenaderTrait2a(TraitContext ctx)
        {
            if (!IsLivingHero(ctx._character))
            {
                return;
            }
            // "serenadertrait2a"
            // When you play an Attack or Small Weapon, reduce the cost of your highest cost Spell by 1. When you play a Spell, reduce the cost of your highest cost Attack or Small Weapon by 1. (3 times/turn)

            int bonusActivations = ctx._character.HaveTrait("serenadertrait4b") ? 1 : 0;
            DualityCardType(ctx._character, ctx._castedCard, [Enums.CardType.Small_Weapon, Enums.CardType.Attack], [Enums.CardType.Spell], ctx.traitId, bonusActivations);
        }

        static void SerenaderTrait2b(TraitContext ctx)
        {
            // "serenadertrait2b":
            // Salient Stanza increases All Damage. 
            // When you play Attack or Small without Stanza, gain Stanza. This increases from Stanza I to II to III throughout the turn. When you play a Song, lose Stanza.
            if (!IsLivingHero(ctx._character) || ctx._castedCard == null)
            {
                LogDebug("Nonliving character or null card");
                return;
            }
            // string ctx.traitName = ctx.traitData.TraitName;

            LogDebug($"Handling Trait {"serenadertrait2b"}");

            bool hasStanza = ctx._character.HasEffect("stanzai") || ctx._character.HasEffect("stanzaii") || ctx._character.HasEffect("stanzaiii");
            LogDebug($"Has Stanza {hasStanza} - card - {ctx._castedCard.CardName}");
            if (hasStanza && ctx._castedCard.HasCardType(Enums.CardType.Song))
            {
                // ctx._castedCard.EffectRequired = "";
                // Traverse.Create(__instance).Field("castedCard").SetValue(ctx._castedCard);
                // ctx._character.SetAuraTrait(ctx._character, "stanzaiii", 1);
                // ctx._character.SetAuraTrait(ctx._character, "spellsword", 4);

                for (int index = ctx._character.AuraCurseList.Count - 1; index >= 0; --index)
                {
                    if (ctx._character.AuraCurseList[index] != null && ctx._character.AuraCurseList[index].ACData != null && (ctx._character.AuraCurseList[index].ACData.Id == "stanzai" || ctx._character.AuraCurseList[index].ACData.Id == "stanzaii" || ctx._character.AuraCurseList[index].ACData.Id == "stanzaiii"))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("<s>");
                        stringBuilder.Append(Functions.UppercaseFirst(ctx._character.AuraCurseList[index].ACData.ACName));
                        stringBuilder.Append("</s>");
                        string text = stringBuilder.ToString();
                        Enums.CombatScrollEffectType type = !ctx._character.AuraCurseList[index].ACData.IsAura ? Enums.CombatScrollEffectType.Curse : Enums.CombatScrollEffectType.Aura;
                        if (ctx._character.HeroItem != null)
                            ctx._character.HeroItem.ScrollCombatText(text, type);
                        ctx._character.AuraCurseList.RemoveAt(index);
                    }
                }

                // LogDebug($"Testin1234 - Healed Stanza ");
            }
            else if (!hasStanza && (ctx._castedCard.HasCardType(Enums.CardType.Small_Weapon) || ctx._castedCard.HasCardType(Enums.CardType.Attack)))
            {
                IncrementTraitActivations(ctx.traitId);
                int activations = MatchManager.Instance.activatedTraits[ctx.traitId];
                string stanza;
                if (activations == 1)
                {
                    stanza = "stanzai";
                }
                else if (activations == 2)
                {
                    stanza = "stanzaii";
                }
                else
                {
                    stanza = "stanzaiii";
                }
                ctx._character.SetAuraTrait(ctx._character, stanza, 1);
                if (ctx._character.HaveTrait("serenadertrait4b"))
                {
                    GainEnergy(ctx._character, 1);
                }
            }
        }

        static void SerenaderTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Salient Stanza applies to All Heroes. Done in GACM
            // When you apply Regen, apply 2 Bless and Sharp
            if (ctx._character == null || ctx._target == null || !ctx._target.Alive || !ctx._character.Alive)
            {
                return;
            }
            if (ctx._auxString.ToLower() == "regeneration")
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                // Not sure if ctx._target or ctx._character who is the source
                ctx._target.SetAuraTrait(ctx._character, "bless", 2);
                ctx._target.SetAuraTrait(ctx._character, "sharp", 2);
            }
        }

        #endregion

        #region Hanshek
        static void RoyalmageTrait0(TraitContext ctx)
        {
            // At the start of combat, gain 2 Insulate and 2 Courage.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "insulate", 2);
            ctx._character.SetAuraTrait(ctx._character, "courage", 2);
        }

        static void RoyalmageTrait2b(TraitContext ctx)
        {
            // "royalmagetrait2b":
            // When you play a Spell, add the Curse Spell type to it.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard != null && ctx._castedCard.HasCardType(Enums.CardType.Spell))
            {
                if (!ctx._castedCard.HasCardType(Enums.CardType.Curse_Spell))
                {
                    List<Enums.CardType> currentTypes = ctx._castedCard.CardTypeAux != null ? [.. ctx._castedCard.CardTypeAux] : [];
                    currentTypes.Add(Enums.CardType.Curse_Spell);
                    Traverse.Create(ctx._castedCard).Field("cardTypeAux").SetValue(currentTypes);
                    LogDebug($"Added Curse type to {ctx._castedCard.CardName}");
                    Traverse.Create(ctx.trait).Field("castedCard").SetValue(ctx._castedCard);
                }
            }
        }

        static void RoyalmageTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // At the start of your turn, reduce the cost of all Curse Spells by 1 until discarded.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ReduceCardTypeCostUntilDiscarded(Enums.CardType.Curse_Spell, 1, ref ctx._character, ref ctx.heroHand, ref ctx.cardDataList, ctx.traitName);
        }

        static void RoyalmageTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // Once per combat, when you play the \"Hellfire\" card put a 0 cost copy with Vanish into your deck.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard != null && ctx._castedCard.Id.StartsWith("royalmagehellfire") && ctx._character.HeroData != null && ctx._character.HeroData.HeroSubClass != null && !MatchManager.Instance.ItemExecuteForThisCombat(ctx._character.HeroData.HeroSubClass.Id, ctx.traitId, 1, ""))
            {

                {
                    string cardInDictionary = MatchManager.Instance.CreateCardInDictionary(ctx._castedCard.Id);
                    CardRealtimeData cardData = MatchManager.Instance.GetCardData(cardInDictionary);
                    cardData.EnergyReductionToZeroPermanent = true;
                    MatchManager.Instance.ModifyCardInDictionary(cardInDictionary, cardData);
                    MatchManager.Instance.GenerateNewCard(1, cardInDictionary, false, Enums.CardPlace.RandomDeck, heroIndex: MatchManager.Instance.GetHeroHeroActive().HeroIndex);
                    MatchManager.Instance.SetTraitInfoText();
                    ctx._character.HeroItem.ScrollCombatText(ctx.traitName + Functions.TextChargesLeft(MatchManager.Instance.ItemExecutedInThisCombat(MatchManager.Instance.GetHeroHeroActive().SubclassName, ctx.traitId), 1), Enums.CombatScrollEffectType.Trait);
                }
            }
        }

        #endregion

        #region Hecar
        static void MoontouchedTrait2b(TraitContext ctx)
        {
            // TODO trait 2b
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // At the start of your turn, reduce your highest cost card by 1 until discarded. Repeat for every 20 Insane on you.
            if (!IsLivingHero(ctx._character))// || ctx._character.GetAuraCharges("insane") < 20)
            {
                return;
            }

            int nInsane = ctx._character.GetAuraCharges("insane");
            int iterations = Mathf.FloorToInt(nInsane * 0.05f) + 1;
            for (int i = 0; i < iterations; i++)
            {
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(Enums.CardType.Spell, ctx.heroHand);
                ReduceCardCost(ref highestCostCard, ctx._character, 1, isPermanent: false);
            }

            // Debating having it half your insane charges first.
            ctx._character.HealAuraCurse(GetAuraCurseData("insane"));
            ctx._character.SetAuraCurse(ctx._character, GetAuraCurseData("insane"), Mathf.RoundToInt(nInsane * 0.5f), useCharacterMods: false, canBePreventable: false);
        }

        static void MoontouchedTrait4b(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // Insane on you increases mind damage by 3% per charge. When you play a card, suffer 3 Insane, this does not benefit from modifiers. (5x/turn).
            if (CanIncrementTraitActivations(ctx.traitId))
            {
                ctx._character.SetAuraCurse(ctx._character, GetAuraCurseData("insane"), 3, useCharacterMods: false);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Isolde
        static void SopranoTrait0(TraitContext ctx)
        {
            // At the start of your turn, apply 1 Sharp, 4 Chill, 4 Insane to all heroes and monsters.
            ApplyAuraCurseToAll("sharp", 1, AppliesTo.Global, ctx._character, useCharacterMods: true);
            ApplyAuraCurseToAll("chill", 4, AppliesTo.Global, ctx._character, useCharacterMods: true);
            ApplyAuraCurseToAll("insane", 4, AppliesTo.Global, ctx._character, useCharacterMods: true);
        }

        static void SopranoTrait2a(TraitContext ctx)
        {
            // "sopranotrait2a"
            // When you play a Song, reduce the cost of the highest cost Elemental Spell card by 1 until discarded.
            //  When you play an Elemental Spell, reduce the cost of the highest cost Song by 1 until discarded. (3 times/turn) 
            int bonusActivations = ctx._character.HaveTrait("sopranotrait4b") ? 1 : 0;

            DualityCardType(ctx._character, ctx._castedCard, [Enums.CardType.Song], [Enums.CardType.Fire_Spell, Enums.CardType.Cold_Spell, Enums.CardType.Lightning_Spell], ctx.traitId, bonusActivations);
        }

        static void SopranoTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Once per turn, when you play a Cold Spell, add a corrupted Neverending Story to your hand. Cost 0 and Vanish.


            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Cold_Spell))
            {
                if (!(MatchManager.Instance != null) || !(ctx._castedCard != null))
                    return;
                AddCardToHand("neverendingstoryrare", randomlyUpgraded: false, costZero: true, vanish: true);
                IncrementTraitActivations(ctx.traitId);
                // ProgressStanza(ctx._character);
            }

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
        }

        #endregion

        #region Jason
        static void AfflictorTrait2a(TraitContext ctx)
        {
            // "afflictortrait2a"
            // When you damage a monster, deal Shadow Damage equal to 15% of its Bleed stacks and Holy Damage equal to 15% of its Poison stacks
            if (!IsLivingNPC(ctx._target))
            {
                float multiplier = 0.15f;
                int holyDamage = Mathf.RoundToInt(ctx._target.GetAuraCharges("poison") * multiplier);
                int shadowDamage = Mathf.RoundToInt(ctx._target.GetAuraCharges("bleed") * multiplier);
                ctx._target.IndirectDamage(Enums.DamageType.Shadow, shadowDamage, null);
                ctx._target.IndirectDamage(Enums.DamageType.Holy, holyDamage, null);
            }
        }

        static void AfflictorTrait2b(TraitContext ctx)
        {
            // "afflictortrait2b":
            // When you play a card that costs 2 or more, 
            // reduce by 1 the cost of your highest cost card that costs 2 or more.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId) && MatchManager.Instance.energyJustWastedByHero >= 2)
            {
                CardRealtimeData highCost = GetRandomHighestCostCard(Enums.CardType.None);
                if (highCost.GetCardFinalCost() >= 2)
                {
                    ReduceCardCost(ref highCost, amountToReduce: 1, isPermanent: true);
                }
            }
        }

        static void AfflictorTrait4b(TraitContext ctx)
        {
            // trait 4b:
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (IsLivingNPC(ctx._target))
            {
                ctx._target?.SetAuraTrait(ctx._character, "harbingerofdoom", 1);
            }
        }

        #endregion

        #region Kaa
        static void ShadowscaleTrait0(TraitContext ctx)
        {
            // Gain 1 evade at combat start 
            ctx._character.SetAuraTrait(ctx._character, "evade", 1);
        }

        static void ShadowscaleTrait2a(TraitContext ctx)
        {
            // "shadowscaletrait2a"
            // Evasion +1. 
            // Evasion on you stacks and increases All Damage by 1 per charge. 
            // When you play a Defense card, gain 1 Energy and Draw 1. (2 times/turn)

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                DrawCards(1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Kaerion
        static void ShadowknightTrait0(TraitContext ctx)
        {
            // "shadowknighttrait0": When you damage an enemy, Heal for 2% of your health per unique Curse on them (Maximum of 10%). -This heal does not benefit from modifiers-
            if (CanIncrementTraitActivations(ctx.traitId) && IsLivingHero(ctx._character) && IsLivingNPC(ctx._target))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                int toHeal = Mathf.RoundToInt(ctx._character.GetMaxHP() * 0.02f * Mathf.Min(5, ctx._target.GetCurseList().Count()));
                TraitHeal(ref ctx._character, ctx._character, toHeal, ctx.traitName);

                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void ShadowknightTrait2a(TraitContext ctx)
        {
            // "shadowknighttrait2a":
            // When you Heal on your turn, gain 1 Block. At at full health, gain 1 Regeneration instead.

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "block", 1);
        }

        static void ShadowknightTrait2b(TraitContext ctx)
        {
            // "shadowknighttrait2b":
            // At the start of your turn, spread 10% of all Curses from the enemy with the most unique Curses to adjacent enemies.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Character enemyWithMostCurses = GetCharacterWithMostUniqueCurses(ctx.teamNpc);
            if (enemyWithMostCurses != null)
            {
                var targetSides = enemyWithMostCurses.GetOwnerTeam().GetSideCharacters(enemyWithMostCurses.NPCIndex);
                foreach (Character side in targetSides)
                {
                    Character adjacent = side;
                    if (adjacent != null && adjacent != enemyWithMostCurses)
                    {
                        SpreadPercentageOfCurses(enemyWithMostCurses, ref adjacent, 0.1f);
                    }
                }

            }
        }

        static void ShadowknightTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // The first Curse you apply each turn is doubled. Damage from Curse effects is increased by 25%.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId))
            {
                AuraCurseData ac = Globals.Instance.GetAuraCurseData(ctx._auxString);
                if (ac == null || ac.IsAura)
                {
                    return;
                }
                int mult = ac.GainCharges ? 1 : 2;
                ctx._target.SetAuraCurse(null, ac, ctx._auxInt * mult);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Kolossos
        static void ExecutionerTrait0(TraitContext ctx)
        {
            // "executionertrait0":
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "sharp", 2);
        }

        static void ExecutionerTrait2a(TraitContext ctx)
        {
            // At the start of your turn, reduce the cost of the \"Melee Attack\" cards in your hand by 1 until they are discarded.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ReduceCardTypeCostUntilDiscarded(Enums.CardType.Melee_Attack, 1, ref ctx._character, ref ctx.heroHand, ref ctx.cardDataList, ctx.traitName);
        }

        static void ExecutionerTrait2b(TraitContext ctx)
        {
            // "executionertrait2b":
            // Twice per turn, when dealing damage with a hit, gain 3 Fury
            if (CanIncrementTraitActivations(ctx.traitId))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character.SetAuraTrait(ctx._character, "fury", 3);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Laios
        static void EnforcerTrait0(TraitContext ctx)
        {
            // Gain 1 zeal every turn.
            ctx._character.SetAuraTrait(ctx._character, "zeal", 1);
        }

        static void EnforcerTrait2b(TraitContext ctx)
        {
            // "enforcertrait2b":
            // When you play an Attack or Spell, reduce the cost of your highest cost Defense by 1 until discarded. 
            // When you play a Defense, reduce the cost of your highest cost Attack or Spell by 1 until discarded.
            int bonusActivations = ctx._character.HaveTrait("enforcertrait4a") ? 1 : 0;
            DualityCardType(ctx._character, ctx._castedCard, [Enums.CardType.Attack, Enums.CardType.Spell], [Enums.CardType.Defense], ctx.traitId, bonusActivations: bonusActivations);
        }

        static void EnforcerTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Zeal on heroes is not lost at end of turn. When you hit an enemy, 
            // suffer 2 burn. 
            // Enforcer Duality can activate an extra time.                
            ctx._character.SetAuraTrait(ctx._character, "burn", 2);
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
        }

        static void EnforcerTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // When you play a Defense, add a random Defense that costs 1 more to your hand. 
            // This card costs 0 and Vanish. (1 time/turn). 
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                int cost = MatchManager.Instance.energyJustWastedByHero + 1;
                bool vanish = true;
                // int costReduction = 3;
                bool costZero = true;
                bool permanentCostReduction = true;
                Enums.HeroClass cardClass = MatchManager.Instance.Random.GetRandomIntRange(1, 100) % 2 == 0 ? Enums.HeroClass.Warrior : Enums.HeroClass.Healer;
                string randomCard = "";
                while (randomCard.IsNullOrWhiteSpace())
                {
                    randomCard = GetRandomCardOfTypeAndCost(cardClass, [Enums.CardType.Defense], cost);//, costReduction: -3, vanish: false, permanentCostReduction: true);
                    cost -= 1;
                    if (cost < 0)
                    {
                        break;
                    }
                }
                if (randomCard.IsNullOrWhiteSpace())
                {
                    LogError($"No card found for trait {ctx.traitId} with cost {cost}");
                    return;
                }
                AddCardToHand(randomCard, randomlyUpgraded: false, vanish: vanish, costZero: costZero, permanentCostReduction: permanentCostReduction);

                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Malakir
        static void TransmuterTrait0(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // "transmutertrait0":
            // if (MatchManager.Instance.GetCurrentRound() != 0)
            // {
            //     return;
            // }
            // At the start of combat, gain 1 inspire
            ctx._character.SetAuraTrait(ctx._character, "inspire", 1);
        }

        static void TransmuterTrait2a(TraitContext ctx)
        {
            // "transmutertrait2a"
            // When you play a book, gain 1 energy, apply 1 thorns, 1 block to all heroes.

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Book))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character.ModifyEnergy(1);
                ApplyAuraCurseToAll("block", 1, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
                ApplyAuraCurseToAll("thorns", 1, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void TransmuterTrait2b(TraitContext ctx)
        {
            // trait 2b:  
            // +1 to all Curse Charges
            // At the start of your turn, apply 1 Burn/Chill/Spark to a random monster.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Character randNPC = GetRandomCharacter(ctx.teamNpc);
            randNPC.SetAuraTrait(ctx._character, "burn", 1);
            randNPC.SetAuraTrait(ctx._character, "chill", 1);
            randNPC.SetAuraTrait(ctx._character, "spark", 1);
        }

        static void TransmuterTrait4b(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // "transmutertrait4b":
            // at end of your turn, transorm elemental to dark
            foreach (Character npc in ctx.teamNpc)
            {
                if (!IsLivingNPC(npc))
                {
                    continue;
                }
                int nDark = npc.GetAuraCharges("burn") + npc.GetAuraCharges("spark") + npc.GetAuraCharges("chill");
                npc.HealAuraCurse(GetAuraCurseData("burn"));
                npc.HealAuraCurse(GetAuraCurseData("chill"));
                npc.HealAuraCurse(GetAuraCurseData("spark"));
                npc.SetAuraTrait(ctx._character, "dark", nDark);
            }
        }

        #endregion

        #region Malia
        static void SnakeTrait2a(TraitContext ctx)
        {
            // "snaketrait2a"
            // At the start of your turn, if you have more than 10 Poison, gain Stanza 1.
            if (ctx._character.HaveTrait(ctx.traitId) && ctx._character.GetAuraCharges("poison") > 10)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character.SetAuraTrait(ctx._character, "stanzai", 1);
                ctx._character?.HeroItem?.ScrollCombatText(ctx.traitName, Enums.CombatScrollEffectType.Trait);
            }
        }

        static void SnakeTrait2b(TraitContext ctx)
        {
            // "snaketrait2b":
            // At the start of your turn, every 4 Stacks of Reinforce on you gain 1 Infuse and restore 5% of your Max Health.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            int nRepeats = ctx._character.GetAuraCharges("reinforce") / 3;
            if (nRepeats <= 0)
            {
                return;
            }
            ctx._character.SetAuraTrait(ctx._character, "infuse", nRepeats);
            ctx._character.IndirectHeal(Mathf.RoundToInt(ctx._character.GetMaxHP() * 0.05f * nRepeats));
            ctx._character?.HeroItem?.ScrollCombatText(ctx.traitName, Enums.CombatScrollEffectType.Trait);
        }

        static void SnakeTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // When you play a Defense, advance your Stanza. Advancing past Stanza 3 grants 4 Powerful, 2 Inspire and Stanza 1 to all Heroes but you suffer 2 Shackles (once per turn).

            if (ctx._castedCard.HasCardType(Enums.CardType.Defense) && CanIncrementTraitActivations(ctx.traitId))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ProgressStanza(ctx._character);
                IncrementTraitActivations(ctx.traitId);

                if (ctx._character.HasEffect("stanzai") || ctx._character.HasEffect("stanzaii") || ctx._character.HasEffect("stanzaiii"))
                {
                    return;
                }
                ApplyAuraCurseToAll("powerful", 4, AppliesTo.Heroes, ctx._character, true);
                ApplyAuraCurseToAll("inspire", 2, AppliesTo.Heroes, ctx._character, true);
                ApplyAuraCurseToAll("stanzai", 1, AppliesTo.Heroes, ctx._character, true);
                ctx._character.SetAuraTrait(ctx._character, "shackle", 2);
                ctx._character?.HeroItem?.ScrollCombatText(ctx.traitName, Enums.CombatScrollEffectType.Trait);
            }
        }

        static void SnakeTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // Immune to Slow. Chill no longer reduces your Speed. When you play a Defense with cost >=3, dispel Chill and Slow on all other heroes (once per turn).
            if (ctx._castedCard.HasCardType(Enums.CardType.Defense) && CanIncrementTraitActivations(ctx.traitId) && MatchManager.Instance.energyJustWastedByHero >= 3)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

                for (int i = 0; i < ctx.teamHero.Count; i++)
                {
                    if (IsLivingHero(ctx.teamHero[i]) && ctx.teamHero[i] != ctx._character)
                    {
                        ctx.teamHero[i].HealAuraCurse(GetAuraCurseData("chill"));
                        ctx.teamHero[i].HealAuraCurse(GetAuraCurseData("slow"));
                    }
                }
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region MalukahRework
        static void VoodoowitchTrait2a(TraitContext ctx)
        {
            // Dark +2. When you apply Dark, apply 1 Sanctify to all monsters. (2 times/turn)
            // "voodoowitchtrait2a"

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._auxString.ToLower() == "dark")// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ApplyAuraCurseToAll("sanctify", 1, AppliesTo.Monsters, ctx._character, useCharacterMods: true);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Medea
        static void WitchTrait0(TraitContext ctx)
        {
            // "witchtrait0": At the start of combat, apply 3 Regeneration to all heroes.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ApplyAuraCurseToAll("regeneration", 3, AppliesTo.Heroes, ctx._character, true);
        }

        static void WitchTrait4a(TraitContext ctx)
        {
            // trait 4a; When you apply Regeneration, apply 1 Vitality. Regeneration on heroes removes 1 Poison and 1 Bleed per charge when applied.

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._auxString == "regeneration" && IsLivingHero(ctx._target))
            {
                ctx._target.SetAuraTrait(ctx._character, "vitality", 1);

                ctx._target.ConsumeEffectCharges("poison", ctx._auxInt);
                ctx._target.ConsumeEffectCharges("bleed", ctx._auxInt);

            }
        }

        #endregion

        #region Monty
        static void BunnyTrait2b(TraitContext ctx)
        {
            // "bunnytrait2b":
            // At the start of your turn reduces the cost of your highest by cost card by 1 until discarded. Repeat for every 3 Fast on you.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            int nFast = 5;
            int nToRepeat = 1 + ctx._character.GetAuraCharges("fast") / nFast;
            for (int i = 0; i < nToRepeat; i++)
            {
                CardRealtimeData highestCost = GetRandomHighestCostCard(Enums.CardType.None, ctx.heroHand);
                ReduceCardCost(ref highestCost, ctx._character, 1, isPermanent: false);
            }
        }

        static void BunnyTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // Evasion +1. 
            // When you apply Buffer, apply 3 times as much Block and Shield to the target.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._auxString == "buffer" && ctx._target != null && ctx._target.Alive)
            {
                int multiplier = 3;
                int nToApply = multiplier * ctx._auxInt;
                ctx._target.SetAuraTrait(ctx._character, "block", nToApply);
                ctx._target.SetAuraTrait(ctx._character, "shield", nToApply);
            }
        }

        #endregion

        #region NenukilUpdate
        static void Loadedgunaltered(TraitContext ctx)
        {
            // Loaded Gun: At the start of your turn, reduce the cost of all \"Ranged Attack\" in your hand by 2 until they are discarded. Also, suffer 1 Burn. Repeat this for every \"Ranged Attack\" in your hand. When you play a \"Ranged Attack\", suffer \"Reloading\".",
            if (!((object)ctx._character.HeroData != null))
            {
                return;
            }
            int num = 2;
            // List<string> _heroHand = MatchManager.Instance.GetHeroHand(ctx._character.HeroIndex);
            List<CardRealtimeData> rangedAttacks = new List<CardRealtimeData>();
            for (int i = 0; i < ctx.heroHand.Count; i++)
            {
                CardRealtimeData cardData = MatchManager.Instance.GetCardData(ctx.heroHand[i]);
                if (cardData.GetCardFinalCost() > 0 && cardData.HasCardType(Enums.CardType.Ranged_Attack))
                {
                    rangedAttacks.Add(cardData);
                }
            }
            // ctx._character.SetAuraTrait(ctx._character, "burn", 1);
            for (int j = 0; j < rangedAttacks.Count; j++)
            {
                CardRealtimeData cardData = rangedAttacks[j];
                cardData.EnergyReductionTemporal += num;
                MatchManager.Instance.UpdateHandCards();
                CardItem cardFromTableByIndex = MatchManager.Instance.GetCardFromTableByIndex(cardData.InternalId);
                cardFromTableByIndex.PlayDissolveParticle();
                cardFromTableByIndex.ShowEnergyModification(-num);
                MatchManager.Instance.CreateLogCardModification(cardData.InternalId, MatchManager.Instance.GetHero(ctx._character.HeroIndex));
                ctx._character.SetAuraTrait(ctx._character, "burn", 1);
            }
            if (rangedAttacks.Count > 0)
            {
                ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_Loaded Gun"), Enums.CombatScrollEffectType.Trait);
            }
        }

        static void Mountedcannonaltered(TraitContext ctx)
        {
            // Mounted Cannon: When you play a \"Defense\" card that costs energy, put a \"Blast!\" in your hand. Halves damage taken by burn. Fast on this hero is lost at end of turn",
            // left unchanged
            if ((object)MatchManager.Instance != null && (object)ctx._castedCard != null)
            {
                if (MatchManager.Instance.CountHeroHand() == 10)
                {
                    Debug.Log((object)"[TRAIT EXECUTION] Broke because player at max cards");
                }
                else if (MatchManager.Instance.energyJustWastedByHero > 0 && ctx._castedCard.GetCardTypes().Contains(Enums.CardType.Defense) && (object)ctx._character.HeroData != null)
                {
                    string id = "blast";
                    string text = MatchManager.Instance.CreateCardInDictionary(id);
                    CardRealtimeData cardData = MatchManager.Instance.GetCardData(text);
                    MatchManager.Instance.GenerateNewCard(1, text, createCard: false, Enums.CardPlace.Hand);
                    ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_Mounted Cannon"), Enums.CombatScrollEffectType.Trait);
                    MatchManager.Instance.ItemTraitActivated();
                    MatchManager.Instance.CreateLogCardModification(cardData.InternalId, MatchManager.Instance.GetHero(ctx._character.HeroIndex));
                }
            }
        }

        static void Exoskeletonaltered(TraitContext ctx)
        {
            // Exoskeleton: Immune to Burn. When you play a \"Defense\" card that costs energy, refund 1 Energy and gain 1 Fortify. (2 times / turn)",

            if (!((object)MatchManager.Instance != null) || !((object)ctx._castedCard != null))
            {
                return;
            }
            // TraitData ctx.traitData = Globals.Instance.GetTraitData(ctx.traitName);
            if ((MatchManager.Instance.activatedTraits == null || !MatchManager.Instance.activatedTraits.ContainsKey(ctx.traitName) || MatchManager.Instance.activatedTraits[ctx.traitName] <= ctx.traitData.TimesPerTurn - 1) && MatchManager.Instance.energyJustWastedByHero > 0 && ctx._castedCard.GetCardTypes().Contains(Enums.CardType.Defense) && (object)ctx._character.HeroData != null)
            {
                if (!MatchManager.Instance.activatedTraits.ContainsKey(ctx.traitName))
                {
                    MatchManager.Instance.activatedTraits.Add(ctx.traitName, 1);
                }
                else
                {
                    Dictionary<string, int> activatedTraits = MatchManager.Instance.activatedTraits;
                    activatedTraits[ctx.traitName] = activatedTraits[ctx.traitName] + 1;
                }
                MatchManager.Instance.SetTraitInfoText();
                ctx._character.SetAuraTrait(ctx._character, "fortify", 1);
                ctx._character.ModifyEnergy(1, showScrollCombatText: true);
                if ((object)ctx._character.HeroItem != null)
                {
                    ctx._character.HeroItem.ScrollCombatText(String.Concat(Texts.Instance.GetText("traits_Exoskeleton"), Functions.TextChargesLeft(MatchManager.Instance.activatedTraits[ctx.traitName], ctx.traitData.TimesPerTurn)), Enums.CombatScrollEffectType.Trait);
                    EffectsManager.Instance.PlayEffectAC("parry", isHero: true, ctx._character.HeroItem.CharImageT, flip: false);
                    EffectsManager.Instance.PlayEffectAC("energy", isHero: true, ctx._character.HeroItem.CharImageT, flip: false);
                }
            }
        }

        #endregion

        #region pestilybiohealer
        static void Pestilybioheal(TraitContext ctx)
        {
            //   - At the end of your turn, heal you and allies for 20% the number of Poison stacks in play.                 
            // string ctx.traitName="pestilybioheal";
            // LogDebug("Binbin - PestilyBiohealer - Pre-Biohealing");

            int poisonStacks = CountAllStacks("poison", ctx.teamHero, ctx.teamNpc);
            // LogDebug("Binbin - PestilyBiohealer - Pre-Biohealing stack count: " + poisonStacks);
            int healAmount = Functions.FuncRoundToInt((float)((double)poisonStacks * 0.20000000298023224));
            for (int index = 0; index < ctx.teamHero.Count; ++index)
            {
                LogDebug("Binbin - PestilyBiohealer - Biohealing");

                TraitHeal(ref ctx._character, ctx.teamHero[index], healAmount, ctx.traitName);
            }
            // LogDebug("Binbin - PestilyBiohealer - Finished Bioheal");
        }

        static void Pestilyshadowpoison(TraitContext ctx)
        {
            // Any Dark applied from this hero now applies Poison x2. +1 Shadow Damage per 10 stacks of Poison on you. 
            if (ctx._auxString == "dark")
            {
                int amountToApply = ctx._auxInt * 2;
                ctx._target.SetAuraTrait(ctx._character, "poison", amountToApply);
            }
        }

        static void Pestilyantidote(TraitContext ctx)
        {
            // You are immune to Poison damage and suffer 20 Poison every turn, but Poison stacks on you are limited to 300. Increase your Innate heal to 40%.

            ctx._character.SetAuraTrait(ctx._character, "poison", 20);
        }

        static void Pestilyhealingtoxins(TraitContext ctx)
        {
            // Whenever you use a Healing spell, permanently reduce the cost of a random Curse spell by 1. Whenever you use a Curse Spell, cast Healing Rain. (3 times/turn)


            PermanentyReduceXWhenYouPlayY(ref ctx._character, ref ctx._castedCard, Enums.CardType.Curse_Spell, Enums.CardType.Healing_Spell, 1, ctx.traitId);
            //LogDebug("Binbin - PestilyBiohealer - CardTypes: " +ctx._castedCard.CardName + " " + String.Join(", ", ctx._castedCard.GetCardTypes()));
            //LogDebug("Binbin - PestilyBiohealer - Contains Healing Spell? " + ctx._castedCard.GetCardTypes().Contains(CardType.Curse_Spell));

            if (pestilyLevel5ActivationCounter < pestilyLevel5MaxActivations)
            {
                if (ctx._castedCard.GetCardTypes().Contains(Enums.CardType.Curse_Spell))
                {
                    PlayCardForFree("healingrainb");
                }
                pestilyLevel5ActivationCounter++;
            }
        }

        static void Pestilytoxichealing(TraitContext ctx)
        {
            //Whenever you use a Curse spell, permanently reduce the cost of a Healing Spell by 1. Whenever you use a Healing Spell, cast Acid Rain. (3 times/turn)

            PermanentyReduceXWhenYouPlayY(ref ctx._character, ref ctx._castedCard, Enums.CardType.Healing_Spell, Enums.CardType.Curse_Spell, 1, ctx.traitId);
            // LogDebug("Binbin - PestilyBiohealer - CardTypes: " + String.Join(", ", ctx._castedCard.GetCardTypes()));
            // LogDebug("Binbin - PestilyBiohealer - Contains Curse? " + ctx._castedCard.GetCardTypes().Contains(CardType.Curse_Spell));
            if (pestilyLevel5ActivationCounter < pestilyLevel5MaxActivations)
            {
                if (ctx._castedCard.GetCardTypes().Contains(Enums.CardType.Healing_Spell))
                {
                    PlayCardForFree("acidrainb");
                }
                pestilyLevel5ActivationCounter++;
            }
        }

        #endregion

        #region Ratone
        static void RatkingTrait2a(TraitContext ctx)
        {
            // "ratkingtrait2a"
            // +8 to stacks needed to explode dark, 
            // Start of turn, for every 3 dark on you, shuffle an infestation into your deck.

            // if (CanIncrementTraitActivations(ctx.traitId))// && MatchManager.Instance.energyJustWastedByHero > 0)
            // {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            int darkPerInfestation = AtOManager.Instance.team.TeamHaveTrait("ratkingtrait4b") ? 5 : 4;
            int nToAdd = ctx._character.GetAuraCharges("dark") / darkPerInfestation;
            string cardToAdd = "ratkingvermininfestation";
            string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardToAdd);
            MatchManager.Instance.GetCardData(cardInDictionary1);
            MatchManager.Instance.GenerateNewCard(nToAdd, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: ctx._character.HeroIndex);
            // }
        }

        static void RatkingTrait4a(TraitContext ctx)
        {
            // trait 4a;
            ctx._character.SetAuraTrait(ctx._character, "dark", 1);
            // LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
        }

        #endregion

        #region Riffy
        static void RiftlingTrait0(TraitContext ctx)
        {
            // At the start of your turn, give a random hero Energize 1
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Character randomHero = GetRandomCharacter(ctx.teamHero);
            randomHero?.SetAuraTrait(null, "energize", 1);
        }

        static void RiftlingTrait2a(TraitContext ctx)
        {
            // "riftlingtrait2a"
            // When you play a Healer card, reduce the cost of the highest cost Mage card in your hand by 1 until discarded. When you play a Mage card, reduce the cost of the highest cost Healer card by 1 until discarded. (3 times/turn)

            Duality(ctx.trait, ctx._character, ctx._castedCard, Enums.CardClass.Mage, Enums.CardClass.Healer, ctx.traitId);
        }

        static void RiftlingTrait2b(TraitContext ctx)
        {
            // "riftlingtrait2b":
            // At the start of your turn, reduce the cost of the \"Skill\" cards in your hand by 1 until they are discarded.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Mastery([Enums.CardType.Skill], 1, ctx.traitName);
        }

        #endregion

        #region Rosalinde
        static void AugurTrait0(TraitContext ctx)
        {
            // Burn, Chill, and Spark Charges on enemies additionally apply -0.2% resistance to Holy Damage per charge. 
            // At the end of your turn, all heroes heal for 11% of the Burn Charges, Chill Charges, and Shock Charges in play. -This heal does not gain bonuses-
            LogDebug($"Trait {ctx.traitId}");
            int nCharges = CountAllStacks("burn", ctx.teamHero, ctx.teamNpc);
            nCharges += CountAllStacks("chill", ctx.teamHero, ctx.teamNpc);
            nCharges += CountAllStacks("spark", ctx.teamHero, ctx.teamNpc);
            LogDebug($"{ctx.traitName}: nCharges = {nCharges}");
            int amountToHeal = Mathf.RoundToInt(nCharges * 0.11f);
            for (int i = 0; i < ctx.teamHero.Count; i++)
            {
                Character hero = ctx.teamHero[i];
                if (!IsLivingHero(hero))
                    continue;

                TraitHeal(ref ctx._character, hero, amountToHeal, ctx.traitName);
            }
            // LogInfo($"Trait {ctx.traitId} end");
        }

        static void AugurTrait2a(TraitContext ctx)
        {
            // When you play a Mage Card, reduce the cost of the highest cost Healer Card in your hand by 1 until discarded. When you play a Healer Card, reduce the cost of the highest cost Mage Card in your hand by 1 until discarded. (3 times / per turn)
            LogDebug($"Trait {ctx.traitId}: {ctx.traitName}");
            Duality(ctx.trait, ctx._character, ctx._castedCard, Enums.CardClass.Mage, Enums.CardClass.Healer, ctx.traitId);
        }

        static void AugurTrait2b(TraitContext ctx)
        {
            // At the start of your turn, Dispel 3 targeting yourself, 
            // reduce the cost of the highest cost card in your hand by 2 until discarded.
            LogDebug($"Trait {ctx.traitId}: {ctx.traitName}");
            // LogDebug($"Trait {ctx.traitId} pre");
            CardRealtimeData highCard = GetRandomHighestCostCard(Enums.CardType.None);
            int amountToReduce = 2;
            // LogDebug($"Trait {ctx.traitId} gotcard");
            ReduceCardCost(ref highCard, ctx._character, amountToReduce);
            // LogDebug($"Trait {ctx.traitId} postreduce");
            ctx._character.HealCurses(2);
            // LogDebug($"Trait {ctx.traitId} end");
            // DisplayTraitScroll(ref ctx._character, ctx.traitData);
        }

        static void AugurTrait4a(TraitContext ctx)
        {
            // When you play a \"Spell\" card, Dispel 1 targeting yourself. (4 times / per turn)
            LogDebug($"Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Spell))
            {
                ctx._character.HealCurses(1);
                IncrementTraitActivations(ctx.traitId);

            }
        }

        static void AugurTrait4b(TraitContext ctx)
        {
            // When you play a \"Healing Spell\" card, Apply 2 Mitigate Charges to All Heroes. (2 times / per turn)
            LogDebug($"Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId))
            {
                ApplyAuraCurseToAll("mitigate", 2, AppliesTo.Heroes, ctx._character, useCharacterMods: true);
                IncrementTraitActivations(ctx.traitId);

            }
        }

        #endregion

        #region Sabel
        static void SuffererTrait2a(TraitContext ctx)
        {
            // "sufferertrait2a"
            // When Damaged by an enemy, deal 6 Blunt damage back to them (3x/turn). Note - This is 3 times per enemy
            if (CanIncrementTraitActivations(ctx.traitId) && IsLivingNPC(ctx._target))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                // Character randomNPC = GetRandomCharacter(ctx.teamNpc);
                int damage = ctx._character.DamageWithCharacterBonus(6, Enums.DamageType.Blunt, Enums.CardClass.Special);
                ctx._target.IndirectDamage(Enums.DamageType.Blunt, damage, ctx._character);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void SuffererTrait2b(TraitContext ctx)
        {
            // "sufferertrait2b":
            // When you play a Defense card that costs Energy, refund 1 and apply 1 Vitality to the most damaged hero. (3x/turn)
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Defense) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                // Character randomNPC = GetRandomCharacter(ctx.teamNpc);
                Character mostDamaged = GetLowestHealthCharacter(ctx.teamNpc);
                ctx._character.ModifyEnergy(1);
                mostDamaged.SetAuraTrait(ctx._character, "vitality", 1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void SuffererTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // At the start of every round, shuffle a \"Thump\" into each hero’s draw pile
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ShuffleCardIntoAllDecks("suffererthump");
        }

        static void SuffererTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // When you damage an enemy, deal 3 Holy damage to yourself. This counts as being damaged by an enemy. 
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._character.GetAuraCharges("block") < 3)
            {
                ctx._character.SetEvent(Enums.EventActivation.Damaged, auxInt: 1);
            }
            ctx._character.IndirectDamage(Enums.DamageType.Holy, 3, ctx._character, null, "");
        }

        #endregion

        #region Salara
        static void SavantTrait2a(TraitContext ctx)
        {
            // "savanttrait2a"
            //When you play a Mind Spell, add a randomly upgraded Prayer of Protection with cost 0 and Vanish to your hand. (1 time/turn)",

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Mind_Spell))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {

                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                if (!(ctx._character.HeroData != null) || MatchManager.Instance.CountHeroHand() == 10)
                    return;
                string str = "prayerofprotection";
                int randomIntRange = MatchManager.Instance.Random.GetRandomIntRange(0, 100, "trait");
                string cardInDictionary = MatchManager.Instance.CreateCardInDictionary(randomIntRange >= 45 ? (randomIntRange >= 90 ? str + "rare" : str + "b") : str + "a");
                CardRealtimeData cardData = MatchManager.Instance.GetCardData(cardInDictionary);
                cardData.Vanish = true;
                cardData.EnergyReductionToZeroPermanent = true;
                MatchManager.Instance.GenerateNewCard(1, cardInDictionary, false, Enums.CardPlace.Hand);
                // ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_Chastise") + Functions.TextChargesLeft(MatchManager.Instance.activatedTraits[nameof(chastise)], ctx.traitData.TimesPerTurn), Enums.CombatScrollEffectType.Trait);
                MatchManager.Instance.ItemTraitActivated();
                MatchManager.Instance.CreateLogCardModification(cardData.InternalId, MatchManager.Instance.GetHero(ctx._character.HeroIndex));
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void SavantTrait2b(TraitContext ctx)
        {
            // trait 2b:  
            // When you play a Healing Spell that costs Energy, refund 1 and gain 3 Shield. (3 times/turn)",

            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Healing_Spell) && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                ctx._character?.SetAuraTrait(ctx._character, "shield", 3);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void SavantTrait4a(TraitContext ctx)
        {
            // "savanttrait4a":
            // When you play a Cold Spell, suffer 2 Chill. When you play a Fire Spell, gain 2 Fury.

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._castedCard.HasCardType(Enums.CardType.Cold_Spell))
            {
                ctx._character.SetAuraTrait(ctx._character, "chill", 2);
            }
            if (ctx._castedCard.HasCardType(Enums.CardType.Fire_Spell))
            {
                ctx._character.SetAuraTrait(ctx._character, "fury", 2);
            }
        }

        static void SavantTrait4b(TraitContext ctx)
        {
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // "savanttrait4b":
            // When you play a Defense, gain 2 Block and apply 2 Chill to all enemies.
            if (ctx._castedCard.HasCardType(Enums.CardType.Defense))
            {
                ctx._character.SetAuraTrait(ctx._character, "block", 2);
                ApplyAuraCurseToAll("chill", 2, AppliesTo.Monsters, ctx._character, useCharacterMods: true);
            }
        }

        #endregion

        #region Senenthia
        static void CastleTrait0(TraitContext ctx)
        {
            // "castletrait0": Block +2. Immune to Stealth and Evasion. Begin combat with 10 Disarm.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (!ctx._character.HaveTrait("castletrait2b"))
            {
                ctx._character.SetAuraTrait(ctx._character, "disarm", 10);
            }
        }

        static void CastleTrait2a(TraitContext ctx)
        {
            // "castletrait2a": At the start of each round, Apply 12 Block and 2 Fortify to all heroes.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ApplyAuraCurseToAll("block", 12, AppliesTo.Heroes, sourceCharacter: ctx._character, useCharacterMods: true);
            ApplyAuraCurseToAll("fortify", 2, AppliesTo.Heroes, sourceCharacter: ctx._character, useCharacterMods: true);
            // DrawCards(1);
        }

        static void CastleTrait2b(TraitContext ctx)
        {
            // "castletrait2b": When you play an Attack that costs Energy, apply 1 energize to a random hero (4x/turn).

            if (ctx._castedCard != null && ctx._castedCard.HasCardType(Enums.CardType.Attack) && (MatchManager.Instance.energyJustWastedByHero > 0 || ctx._character.HaveTrait("castletrait4a")))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                Character randomHero = GetRandomCharacter(ctx.teamHero);
                randomHero.SetAuraTrait(randomHero, "energize", 1);
            }
        }

        static void CastleTrait4a(TraitContext ctx)
        {
            // trait 4a; For every 5 Block you apply, apply 1 Thorns. Armory no longer has an energy requirement.

            int nToApply = ctx._auxInt / 5;
            if (nToApply > 0 && IsLivingHero(ctx._target))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._target.SetAuraTrait(ctx._character, "thorns", nToApply);
            }
        }

        static void CastleTrait4b(TraitContext ctx)
        {
            // trait 4b:
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            int currentBlock = ctx._character.EffectCharges("block");
            int nToConsume = currentBlock / 4;
            ctx._character.ConsumeEffectCharges("block", nToConsume);
        }

        #endregion

        #region Silenus
        static void KingTrait0(TraitContext ctx)
        {
            // At the start of combat, apply 1 Courage to all heroes.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ApplyAuraCurseToAll("courage", 1, AppliesTo.Heroes, sourceCharacter: ctx._character, useCharacterMods: true, isPreventable: true);
        }

        static void KingTrait2a(TraitContext ctx)
        {
            // "kingtrait2a"
            // At the start of your turn, reduce the cost of Healing Spells in your hand by 1 until discarded.
            ReduceCardTypeCostUntilDiscarded(Enums.CardType.Healing_Spell, 1, ref ctx._character, ref ctx.heroHand, ref ctx.cardDataList, ctx.traitName);
        }

        #endregion

        #region Simone
        static void AmbusherTrait0(TraitContext ctx)
        {
            // "ambushertrait0":
            // At the end of your turn, apply 1 Mark for every Stealth on you to the lowest HP monster.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Character lowestHPMonster = GetLowestHealthCharacter(ctx.teamNpc);
            if (IsLivingNPC(lowestHPMonster))
            {
                int nToApply = ctx._character.GetAuraCharges("stealth");
                lowestHPMonster.SetAuraTrait(ctx._character, "mark", nToApply);
            }
        }

        static void AmbusherTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // At the end of your turn, if you didn't play a Small Weapon or Attack this turn, 
            // gain 1 Stealth for every Stealth you gained during the turn.
            // TODO: Implement this

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            // if (!playedAttackingCard)
            // {
            //     ctx._character.SetAuraTrait(ctx._character, "stealth", stealthGained);
            // }
        }

        #endregion

        #region Splody
        static void ThebombTrait2b(TraitContext ctx)
        {
            // TODO trait 2b - Burning Conversion
            // Whenever you cast a spell with a energy cost above 5 lower the cost of all other cards in hand by 2 
            // and suffer 10 burn (max 2 times per turn)
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard != null && ctx._castedCard.EnergyCost >= 6 && ctx._castedCard.HasCardType(Enums.CardType.Spell))
            {
                for (int i = 0; i < ctx.heroHand.Count; i++)
                {
                    CardRealtimeData card = MatchManager.Instance.GetCardData(ctx.heroHand[i]);
                    ReduceCardCost(ref card, ctx._character, 2, isPermanent: false);
                }

                ctx._character.SetAuraTrait(ctx._character, "burn", 10);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void ThebombTrait4a(TraitContext ctx)
        {
            // TODO IMPLEMENT THIS
            // For each Deaths door in your deck at the start of your first turn gain 2 vit 2 powerful gain 10 burn and take 10 damage
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");


            // if (MatchManager.Instance.GameRound() == 1 && IsLivingHero(ctx._character))
            // {
            //     int multiplier = DeathsDoorCount(ctx._character);
            //     ctx._character.SetAuraTrait(ctx._character, "vitality", 2 * (1 + multiplier));
            //     ctx._character.SetAuraTrait(ctx._character, "powerful", 2 * (1 + multiplier));
            //     ctx._character.SetAuraTrait(ctx._character, "burn", 10 * (1 + multiplier));
            //     ctx._character.IndirectDamage(Enums.DamageType.Fire, 10 * (1 + multiplier), ctx._character, null, "");
            // }
        }

        #endregion

        #region Tai
        static void DualistTrait0(TraitContext ctx)
        {
            // When you play a Shadow Spell, apply 2 Sanctify to everyone. When you play a Holy Spell, apply 2 Dark to everyone (once per turn).

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (CanIncrementTraitActivations(ctx.traitId) && (ctx._castedCard.HasCardType(Enums.CardType.Holy_Spell) || ctx._castedCard.HasCardType(Enums.CardType.Shadow_Spell)))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");

                if (ctx._castedCard.HasCardType(Enums.CardType.Shadow_Spell))
                {
                    ApplyAuraCurseToAll("sanctify", 2, AppliesTo.Global, sourceCharacter: ctx._character, useCharacterMods: true);
                }
                if (ctx._castedCard.HasCardType(Enums.CardType.Holy_Spell))
                {
                    ApplyAuraCurseToAll("dark", 2, AppliesTo.Global, sourceCharacter: ctx._character, useCharacterMods: true);
                }
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void DualistTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // On hit, apply 2 Dark and 2 Sanctify. Dark and Sanctify explosions deal 30% more damage.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._target.SetAuraTrait(ctx._character, "sanctify", 2);
            ctx._target.SetAuraTrait(ctx._character, "dark", 2);
        }

        #endregion

        #region Tellann
        static void ExaltedTrait0(TraitContext ctx)
        {
            // When you apply Burn, apply 1 Zeal
            if (ctx._auxString == "burn" && ctx._target.Alive && ctx._target != null)
            {
                ctx._target.SetAuraTrait(ctx._character, "zeal", 1);
            }
        }

        static void ExaltedTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // At the start of your turn, reduce the cost of your highest cost card by one for every 20 Burn on you.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            CardRealtimeData highestCostCard = GetRandomHighestCostCard(Enums.CardType.None, ctx.heroHand);
            if (highestCostCard != null)
            {
                int burnCharges = ctx._character.GetAuraCharges("burn");
                int costReduction = burnCharges / 20;
                ReduceCardCost(ref highestCostCard, amountToReduce: costReduction);
            }
        }

        #endregion

        #region ThePenitent
        static void PenitentTrait2a(TraitContext ctx)
        {
            // TODO trait 2a
            // Draw 2 cards, gain 1 Energy, and gain 1 Vitality when you play an Injury (3x/turn)
            if (CanIncrementTraitActivations(ctx.traitId) && (ctx._castedCard.HasCardType(Enums.CardType.Injury) || ctx._castedCard.CardClass == Enums.CardClass.Injury))
            {
                if (ctx._target == null)
                    LogDebug($"Trait 2a: {ctx.traitId} - null target");
                else
                {
                    LogDebug($"Trait 2a: {ctx.traitId} - target = {ctx._target.SourceName}");
                }
                if (ctx._character == null)
                    LogDebug($"Trait 2a: {ctx.traitId} - null character");
                else
                {
                    LogDebug($"Trait 2a: {ctx.traitId} - character = {ctx._character.SourceName}");
                }

                // LogDebug($"Trait 2a: {ctx.traitId} - Drawing cards");
                DrawCards(2);

                // LogDebug($"Trait 2a: {ctx.traitId} - Gaining energy");                
                GainEnergy(ctx._character, 1);

                // LogDebug($"Trait 2a: {ctx.traitId} - Incrementing");

                // LogDebug($"Trait 2a: {ctx.traitId} - Setting vitality");
                ctx._character.SetAuraTrait(ctx._character, "vitality", 1);
                IncrementTraitActivations(ctx.traitId);

                // LogDebug($"Trait 2a: {ctx.traitId} - Done");
            }
        }

        static void PenitentTrait2b(TraitContext ctx)
        {
            // TODO trait 2b
            // +1 Vitality. When you apply apply Vitality to a different hero, steal 2 curses from them. 

            if (IsLivingHero(ctx._character) && IsLivingHero(ctx._target) && ctx._auxString == "vitality" && ctx._character != ctx._target)
            {
                LogDebug($"Trait: {ctx.traitId}: attempting to steal curses");

                StealAuraCurses(ref ctx._character, ref ctx._target, 2, IsAuraOrCurse.Curse);
            }
        }

        static void PenitentTrait4b(TraitContext ctx)
        {
            // TODO trait 4b
            // Once per turn, when you heal a hero, apply Vitality equal to 10% of all curses on this hero. Increase this by 3% for every injury in your starting deck.
            // LogDebug("Trait 4b - 1");
            if (CanIncrementTraitActivations(ctx.traitId) && IsLivingHero(ctx._character) && IsLivingHero(ctx._target))
            {
                // LogDebug("Trait 4b - 2");
                int nCurseCharges = CountAllACOnCharacter(ctx._character, IsAuraOrCurse.Curse);
                float multiplier = 0.10f + 0.03f * 1;// InjuryCount(ctx._character);
                int toApply = 4 + Mathf.FloorToInt(nCurseCharges * multiplier);
                ctx._target.SetAuraTrait(ctx._character, "vitality", toApply);
                IncrementTraitActivations(ctx.traitId);

            }
        }

        #endregion

        #region TheTrickster
        static void Trickstermagictrick(TraitContext ctx)
        {
            // Front hero starts with 1 Evasion
            LogDebug(ctx.traitName);
            Character frontHero = ctx.teamHero.First();
            LogDebug($"Trait: {ctx.traitName} - Front hero: {frontHero.SourceName}");
            frontHero.SetAuraTrait(ctx._character, "evasion", 1);
            LogDebug($"Trait: {ctx.traitName} - Evasion set");
            ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_" + ctx.traitName), Enums.CombatScrollEffectType.Trait);
        }

        static void Trickstertrickupyoursleeve(TraitContext ctx)
        {
            // When you play your first Small Weapon each turn, draw a card and reduce its cost by 2.
            // cardsPlayedPerTurn += 1;
            LogDebug(ctx.traitName);
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Small_Weapon))
            {
                LogDebug($"Trait: {ctx.traitName} Casted Card - {ctx._castedCard.Id}. IsSmallWeap - {ctx._castedCard.HasCardType(Enums.CardType.Small_Weapon)}");

                PlayCardForFree("tricksterspecialdraw");

                MatchManager matchManager = MatchManager.Instance;
                if (matchManager != null)
                {
                    LogDebug("Decrement globalVanishCardsNum - draw");
                    int globalVanishCardsNum = Traverse.Create(matchManager).Field("GlobalVanishCardsNum").GetValue<int>();
                    globalVanishCardsNum -= 1;
                    Traverse.Create(matchManager).Field("GlobalVanishCardsNum").SetValue(globalVanishCardsNum);
                }

                // DrawCards(1);
                // Globals.Instance.WaitForSeconds(1.5f);

                // CardRealtimeData cardToReduce = GetRightmostCard(ctx.heroHand);
                // if (cardToReduce == null)
                // {
                //     return;
                // }
                // LogDebug($"Trait: {ctx.traitName} Reducing Card Cost. Card to Reduce-{cardToReduce.Id}");

                // ReduceCardCost(ref cardToReduce, ctx._character, 2, isPermanent: true);
                IncrementTraitActivations(ctx.traitId);
                ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_" + ctx.traitName), Enums.CombatScrollEffectType.Trait);

            }
        }

        static void Tricksterlearnrealmagic(TraitContext ctx)
        {
            // Subclass Mage. At the start of your turn, reduce the cost of your highest cost Skill, Spell and Book by 1.
            LogDebug(ctx.traitName);

            CardRealtimeData skill = GetRandomHighestCostCard(Enums.CardType.Skill, ctx.heroHand);
            CardRealtimeData spell = GetRandomHighestCostCard(Enums.CardType.Spell, ctx.heroHand);
            CardRealtimeData book = GetRandomHighestCostCard(Enums.CardType.Book);

            ReduceCardCost(ref skill, ctx._character, 1, isPermanent: false);
            ReduceCardCost(ref spell, ctx._character, 1, isPermanent: false);
            ReduceCardCost(ref book, ctx._character, 1, isPermanent: false);

            ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_" + ctx.traitName), Enums.CombatScrollEffectType.Trait);
        }

        static void Tricksterdistractingact(TraitContext ctx)
        {
            // +2 Stealth, +1 Evasion. After you play a card, gain 1 Stealth if you had none.
            LogDebug(ctx.traitName);
            LogDebug($"{ctx.traitName} nStealth = {ctx._character.GetAuraCharges("stealth")}");
            if (ctx._castedCard != null && ctx._character.GetAuraCharges("stealth") <= 0 && ctx._castedCard.Id != "tricksterspecialstealth" && ctx._castedCard.Id != "tricksterspecialdraw")
            {
                LogDebug($"Trait: {ctx.traitName} Gaining Stealth - {ctx._character.GetAuraCharges("stealth")}");

                PlayCardForFree("tricksterspecialstealth");

                MatchManager matchManager = MatchManager.Instance;
                if (matchManager != null)
                {
                    LogDebug("Decrement globalVanishCardsNum - stealth");
                    int globalVanishCardsNum = Traverse.Create(matchManager).Field("GlobalVanishCardsNum").GetValue<int>();
                    globalVanishCardsNum -= 1;
                    Traverse.Create(matchManager).Field("GlobalVanishCardsNum").SetValue(globalVanishCardsNum);
                }
                // ctx._character.SetAuraTrait(ctx._character, "stealth", 1);
                ctx._character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_" + ctx.traitName), Enums.CombatScrollEffectType.Trait);
            }
        }

        #endregion

        #region Thornton
        static void CactusTrait0(TraitContext ctx)
        {
            // "cactustrait0":
            bool youAppliedThorns = ctx._auxString == "thorns";
            bool fivePercentChance = MatchManager.Instance.Random.GetRandomIntRange(0, 100, "trait") < 5;
            if (youAppliedThorns && fivePercentChance)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ShuffleRandomFruitIntoDeck(ref ctx._target);
            }
        }

        static void CactusTrait2a(TraitContext ctx)
        {
            // "cactustrait2a": When you play a Food card, gain 1 Energy. 3 times per turn.
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Food))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Tripp
        static void TrapperTrait0(TraitContext ctx)
        {
            // "trappertrait0":
            // At the start of combat, apply 1 Slow and 1 Mark to a random enemy
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
            randomEnemy?.SetAuraTrait(ctx._character, "slow", 1);
            randomEnemy?.SetAuraTrait(ctx._character, "mark", 1);
        }

        static void TrapperTrait2a(TraitContext ctx)
        {
            // "trappertrait2a"
            // When you apply Slow, gain 4 Block. (8x/turn)
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._auxString.ToLower() == "slow")// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.SetAuraTrait(ctx._character, "block", 4);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void TrapperTrait4a(TraitContext ctx)
        {
            // trait 4a;
            // When you apply Slow, deal 5 Piercing damage to a random enemy.
            if (ctx._auxString.ToLower() == "slow" && CanIncrementTraitActivations(ctx.traitId))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                Character randomEnemy = GetRandomCharacter(ctx.teamNpc);
                int damage = ctx._character.DamageWithCharacterBonus(5, Enums.DamageType.Piercing, Enums.CardClass.None);
                randomEnemy?.IndirectDamage(Enums.DamageType.Piercing, damage, ctx._character, null, "");
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Tristan
        static void OwlknightTrait2a(TraitContext ctx)
        {
            // "owlknighttrait2a"
            // Mental Fortitude applies to all heroes. 
            // When you apply Insane, apply 2 Block to All Heroes. This Block does not benefit from bonuses

            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            if (ctx._auxString == "insane")
            {
                int amount = ctx._character.HaveTrait("owlknighttrait4a") ? 4 : 2;
                ApplyAuraCurseToAll("block", amount, AppliesTo.Heroes, ctx._character);
            }
        }

        static void OwlknightTrait2b(TraitContext ctx)
        {
            // "owlknighttrait2b":
            // At the start of your turn, reduce the cost of your highest cost Attack by 2. 
            // Repeat for Defense, Mind Spell, and Healing Spell.
            Enums.CardType[] cardTypes = [Enums.CardType.Attack, Enums.CardType.Defense, Enums.CardType.Mind_Spell, Enums.CardType.Healing_Spell];
            foreach (Enums.CardType cardType in cardTypes)
            {
                CardRealtimeData highestCostCard = GetRandomHighestCostCard(cardType, ctx.heroHand);
                if (highestCostCard == null)
                {
                    continue;
                }
                // int energy = highestCostCard.EnergyCost - highestCostCard.EnergyReductionPermanent - highestCostCard.EnergyReductionTemporal;
                // LogDebug($"Highest cost card: {highestCostCard.CardName} with energy {energy}, cost {highestCostCard.EnergyCost}, reduction {highestCostCard.EnergyReductionPermanent}, temporal reduction {highestCostCard.EnergyReductionTemporal}");
                if (highestCostCard != null && IsLivingHero(ctx._character)) //energy >= 6 && 
                {
                    int amountToReduce = ctx._character.HaveTrait("owlknighttrait4a") ? 3 : 2;
                    ReduceCardCost(ref highestCostCard, ctx._character, amountToReduce);
                }
            }
        }

        static void OwlknightTrait4b(TraitContext ctx)
        {
            // trait 4b:
            // Once per turn, when you play a Mind Spell, add a randomly upgraded Pandemonium to your hand (Costs 0 and Vanish). 
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._castedCard.HasCardType(Enums.CardType.Mind_Spell))
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                AddCardToHand("pandemonium");
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region Tusk
        static void WalrusTrait0(TraitContext ctx)
        {
            // "walrustrait0":
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            ctx._character.SetAuraTrait(ctx._character, "evade", 1);
        }

        #endregion

        #region Ulfvitr
        static void CallTheRain(TraitContext ctx)
        {
            // apply 1 wet to all characters at start of turn

            LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");

            ApplyAuraCurseToAll("wet", 1, AppliesTo.Global, sourceCharacter: ctx._character, useCharacterMods: true);

            // DisplayTraitScroll(ref ctx._character, ctx.traitData);
        }

        static void Magnet(TraitContext ctx)
        {
            // 2x per turn, if you play a lightning spell that costs energy, refund 1 energy and apply 1 spark to a random enemy

            int bonusActivations = ctx._character.HaveTrait("") ? 1 : 0;
            if (CanIncrementTraitActivations(ctx.traitId, bonusActivations: bonusActivations) && ctx._castedCard.HasCardType(Enums.CardType.Lightning_Spell) && ctx._castedCard.EnergyCost >= 1)
            {
                LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character.ModifyEnergy(1, true);
                Character randNPC = GetRandomCharacter(ctx.teamNpc);
                // NPC randNPC = ctx.teamNpc[MatchManager.Instance.Random.GetRandomIntRange(0,3)];
                if (IsLivingNPC(randNPC))
                {
                    randNPC.SetAuraTrait(ctx._character, "spark", 1);
                }
                IncrementTraitActivations(ctx.traitId);
            }
            return;
        }

        static void Regenerator(TraitContext ctx)
        {
            // When you apply regen, heal by wet x0.5f and apply 1 wet     
            if (CanIncrementTraitActivations(ctx.traitId) && ctx._auxString == "regeneration" && IsLivingHero(ctx._target) && IsLivingHero(ctx._character))
            {
                LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");
                int targetWet = ctx._target.GetAuraCharges("wet");
                float multiplier = ctx._character.HaveTrait("ulfvitrconductor") ? 1.0f : 0.5f;
                int healAmount = Functions.FuncRoundToInt((float)targetWet * multiplier);
                TraitHeal(ref ctx._character, ctx._target, healAmount, ctx.traitId);
                ctx._target.SetAuraTrait(ctx._character, "wet", 1);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        static void Conductor(TraitContext ctx)
        {
            // When you apply wet to an enemy, deal sparks * 0.5 as indirect damage

            // done in SetEventPrefix? nvmd trying to do it here


            if (IsLivingHero(ctx._character) && IsLivingNPC(ctx._target) && ctx._auxString == "wet")
            {
                LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");
                float multiplier = ctx._character.HaveTrait("") ? 1.0f : 0.5f;
                int amountToDeal = Functions.FuncRoundToInt((float)ctx._target.GetAuraCharges("spark") * multiplier);
                ctx._target.IndirectDamage(Enums.DamageType.Lightning, amountToDeal, ctx._character);
            }
        }

        static void LifeBloom(TraitContext ctx)
        {
            // At end of turn, heal all heroes by wet * 0.70 - Deprecated
            // At end of turn, apply 1 Inspire for every 20 charges of Wet and
            // 1 Mitigate for every 10 charges of Regeneration
            // Increases activations of Magnet by 1.
            LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");

            foreach (Character hero in ctx.teamHero)
            {
                if (!IsLivingHero(hero))
                {
                    continue;
                }
                int nWet = Mathf.FloorToInt(hero.GetAuraCharges("wet"));
                int nRegen = hero.GetAuraCharges("regeneration");
                int inspireToApply = nWet / 20;
                int mitigateToApply = nRegen / 10;
                hero.SetAuraTrait(ctx._character, "inspire", inspireToApply);
                hero.SetAuraTrait(ctx._character, "mitigate", mitigateToApply);
            }

            // if (ctx._character.HeroData!=null){
            //     for (int i = 0; i < ctx.teamHero.Count; i++)
            //     {
            //         if (IsLivingHero(ctx.teamHero[i]))
            //         {
            //             int healAmount = Functions.FuncRoundToInt((float)ctx.teamHero[i].GetAuraCharges("wet") * 0.70f);
            //             LogDebug("Lifebloom Heal Amount: " + healAmount);
            //             TraitHealHero(ref ctx._character, ref ctx.teamHero[i], healAmount, ctx.traitId);

            //         }
            //     }
            // }
        }

        #endregion

        #region Ursur
        static void UrsineBlood(TraitContext ctx)
        {
            // Ursine Blood: Start each combat with 1 extra Energy. 
            // Whenever you play a Defense, suffer 2 Bleed. Whenever you play an Attack, suffer 2 Chill. 
            // The 1 extra energy is taken care of in the subclass json
            //LogDebug("Found Ursine Blood Trait");
            if (ctx._castedCard != null && ctx._character.HeroData != null)
            {
                // string ctx.traitName = "Ursine Blood";
                LogDebug($"Executing Trait {ctx.traitId}: {ctx.traitName}");

                WhenYouPlayXGainY(Enums.CardType.Attack, "chill", 2, ctx._castedCard, ref ctx._character, ctx.traitName);
                WhenYouPlayXGainY(Enums.CardType.Defense, "bleed", 2, ctx._castedCard, ref ctx._character, ctx.traitName);
                // DisplayTraitScroll(ref ctx._character, ctx.traitData);

            }
        }

        static void BristlyHide(TraitContext ctx)
        {
            //Bristly Hide: When you gain Taunt or Fortify, gain twice as many Thorns. 
            // +1 Fortify charge for every 16 stacks of Bleed. 
            // +1 Taunt charge for every 20 stacks of Chill.
            // LogInfo("Found Bristly Hide");
            if (ctx._character.HeroData != null)
            {
                // string ctx.traitName = "Bristly Hide";

                // int n_bonus_taunt = FloorToInt((float)ctx._character.GetAuraCharges("bleed") / 16.0f);
                // int n_bonus_fort = FloorToInt((float)ctx._character.GetAuraCharges("chill") / 20.0f);
                // ctx.traitData.AuracurseBonusValue1 = n_bonus_taunt;
                // ctx.traitData.AuracurseBonusValue2 = n_bonus_fort;

                // LogInfo("Bristly Hide - bonus taunt = " + n_bonus_fort + " actual =" + ctx.traitData.AuracurseBonusValue2);
                WhenYouGainXGainY(ctx._auxString, "taunt", "thorns", ctx._auxInt, 0, 2.0f, ref ctx._character, ctx.traitName);
                WhenYouGainXGainY(ctx._auxString, "fortify", "thorns", ctx._auxInt, 0, 2.0f, ref ctx._character, ctx.traitName);
            }
        }

        static void BearWithIt(TraitContext ctx)
        {
            // Bear With It: At the start of each turn, 
            // reduce the cost of Attacks by 1 for every 16 Bleed on Ursur. 
            // Reduce the cost of all Defenses by 1 for every 20 Chill.
            if (ctx._character.HeroData != null)
            {
                LogDebug("bearwithit 1");
                // string ctx.traitName = "Bear With It";
                bool applyToAllCards = false;

                ReduceCostByStacks(Enums.CardType.Attack, "bleed", 16, ref ctx._character, ref ctx.heroHand, ref ctx.cardDataList, ctx.traitName, applyToAllCards);
                ReduceCostByStacks(Enums.CardType.Defense, "chill", 20, ref ctx._character, ref ctx.heroHand, ref ctx.cardDataList, ctx.traitName, applyToAllCards);
            }
        }

        static void Ursurunbearable(TraitContext ctx)
        {
            // "Thorns +1. Fury +1. 
            // When you play an Attack, gain 2 Thorns. 
            // When you play a Defense, gain 1 Fury. 
            // Chill does not reduce Ursur's Speed.",
            LogDebug(ctx.traitName + " 1");
            if (ctx._castedCard != null && ctx._character.HeroData != null)
            {
                WhenYouPlayXGainY(Enums.CardType.Attack, "thorns", 2, ctx._castedCard, ref ctx._character, ctx.traitName);
                WhenYouPlayXGainY(Enums.CardType.Defense, "fury", 1, ctx._castedCard, ref ctx._character, ctx.traitName);
            }
        }

        #endregion

        #region Wukong
        static void TacticianTrait0(TraitContext ctx)
        {
            // At the start of your turn, Draw 2 cards, then place 2 cards on the top of your Draw Pile.
            LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
            string cardToPlay = "tacticianexpectedprophecy";
            PlayCardForFree(cardToPlay);
        }

        static void TacticianTrait2a(TraitContext ctx)
        {
            // "tacticiantrait2a"
            // When you play a Skill or Book that costs energy, 
            // refund 1 and apply 2 sight to all monsters. (3 times/turn).

            if (CanIncrementTraitActivations(ctx.traitId) && MatchManager.Instance.energyJustWastedByHero > 0 && (ctx._castedCard.HasCardType(Enums.CardType.Skill) || ctx._castedCard.HasCardType(Enums.CardType.Book)))// && MatchManager.Instance.energyJustWastedByHero > 0)
            {
                LogDebug($"Handling Trait {ctx.traitId}: {ctx.traitName}");
                ctx._character?.ModifyEnergy(1);
                ApplyAuraCurseToAll("sight", 2, AppliesTo.Monsters, sourceCharacter: ctx._character, useCharacterMods: true);
                IncrementTraitActivations(ctx.traitId);
            }
        }

        #endregion

        #region AuxiliaryFunctions

        public static List<CardRealtimeData> GetHeroHand(TraitContext ctx)
        {
            return GetHandCards(ctx.trait, ctx._character, null, null);
        }
        public static List<string> GetHeroHandList(TraitContext ctx)
        {
            return GetHandCards(ctx.trait, ctx._character, null, null).Select(card => card.Id).ToList();
        }

        #endregion
    }
}
