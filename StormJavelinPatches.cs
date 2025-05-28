using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;
using static StormJavelin.Plugin;
using static StormJavelin.CustomFunctions;
using static StormJavelin.StormJavelinFunctions;
using System.Collections.Generic;
using static Functions;
using UnityEngine;
// using Photon.Pun;
using TMPro;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine.UIElements;
// using Unity.TextMeshPro;

// Make sure your namespace is the same everywhere
namespace StormJavelin
{

    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod

    public class StormJavelinPatches
    {





        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), nameof(Character.SetEvent))]
        public static void SetEventPostfix(ref Character __instance,
            Enums.EventActivation theEvent,
            Character target = null,
            int auxInt = 0,
            string auxString = "")
        {
            if (__instance == null || MatchManager.Instance == null)
            {
                return;
            }
            int serenadeChance = 10;
            bool addSerenade = EnableBonusSerenades.Value && MatchManager.Instance.GetRandomIntRange(0, 100) < serenadeChance;
            if (addSerenade && theEvent == Enums.EventActivation.BeginRound)
            {
                Character hero = __instance;
                if (!IsLivingHero(hero)
                )
                {
                    return;
                }
                int index = hero.HeroIndex;
                // string seed = AtOManager.Instance.currentMapNode + index + AtOManager.Instance.GetGameId() + MatchManager.Instance.GetCurrentRound();
                int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                string cardName = GetStormJavelinUpgraded(randInt: randInt);

                LogDebug($"Adding {cardName} to {hero.SourceName}");
                string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardName);
                MatchManager.Instance.GetCardData(cardInDictionary1);
                MatchManager.Instance.GenerateNewCard(1, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: index);
            }
            if (theEvent == Enums.EventActivation.BeginTurnAboutToDealCards)
            {
                Character hero = __instance;
                hero?.SetAura(hero, GetAuraCurseData("stanzai"), 1, useCharacterMods: false);
            }


            if (theEvent == Enums.EventActivation.BeginCombat)
            {


                Character hero = __instance;
                if (!IsLivingHero(hero))
                {
                    return;
                }
                if (addSerenade)
                {
                    int index = hero.HeroIndex;
                    // string seed = AtOManager.Instance.currentMapNode + AtOManager.Instance.GetGameId();
                    // int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                    int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                    string cardName = GetStormJavelinUpgraded(randInt: randInt);

                    LogDebug($"Adding {cardName} to {hero?.SourceName}");
                    string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardName);
                    MatchManager.Instance.GetCardData(cardInDictionary1);
                    MatchManager.Instance.GenerateNewCard(1, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: index);
                }


            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModificationByTraitsAndItems")]
        // [HarmonyPriority(Priority.Last)]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfix(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            // LogInfo($"GACM {subclassName}");

            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            string itemOfInterest;
            switch (_acId)
            {
                case "sharp":
                    itemOfInterest = "javringofsparks";
                    string traitOfInterest2 = "javringofsparksrare";
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, traitOfInterest2, AppliesTo.Heroes))
                    {
                        __result = AtOManager.Instance.GlobalAuraCurseModifyDamage(__result, Enums.DamageType.Lightning, 0, 1, 0);
                    }
                    else if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemOfInterest, AppliesTo.Heroes))
                    {
                        __result = AtOManager.Instance.GlobalAuraCurseModifyDamage(__result, Enums.DamageType.Lightning, 0, 1, 0);

                    }
                    break;
                case "spark":
                    itemOfInterest = "javstormnecklacerare";
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemOfInterest, AppliesTo.Monsters))
                    {
                        __result.MaxMadnessCharges += 100;

                    }
                    itemOfInterest = "javsacredsparks";
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemOfInterest, AppliesTo.Heroes))
                    {
                        __result.HealAttackerConsumeCharges = 1;
                        __result.HealAttackerPerStack = 1;
                    }
                    else if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemOfInterest + "rare", AppliesTo.Heroes))
                    {
                        __result.HealAttackerConsumeCharges = 1;
                        __result.HealAttackerPerStack = 1;
                    }
                    break;


            }
        }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(CardData), nameof(CardData.Init))]

        // public static void InitPostfix(ref string ___cardName, string newId)
        // {
        //     if (ChangeAllNames.Value)
        //     {
        //         ___cardName = "Storm Javelin";

        //     }
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardItem), nameof(CardItem.SetCard))]

        public static void SetCardPostfix(
            string id,
            ref TMP_Text ___titleTextTM,
            ref TMP_Text ___titleTextTBlue,
            ref TMP_Text ___titleTextTGold,
            ref TMP_Text ___titleTextTRed,
            ref TMP_Text ___titleTextTPurple,
            bool deckScale = true,
            Hero _theHero = null,
            NPC _theNPC = null,
            bool GetFromGlobal = false,
            bool _generated = false
            )
        {
            if (ChangeAllNames.Value)
            {
                ___titleTextTM.text = "Storm Javelin";
                // ___titleTextTBlue.text = "Storm Javelin";
                // ___titleTextTGold.text = "Storm Javelin";
                // ___titleTextTRed.text = "Storm Javelin";
                // ___titleTextTPurple.text = "Storm Javelin";

            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardCraftManager), nameof(CardCraftManager.ShowElements))]
        public static void ShowElementsPostfix(ref CardCraftManager __instance, string direction, ref BotonGeneric ___BG_Remove, string cardId = "")
        {
            if (__instance == null)
            {
                LogDebug("__instance == null");
                return;
            }

            CardData cardData = Globals.Instance.GetCardData(cardId, false);
            bool isStormJavelin = cardData?.Id?.StartsWith("vitalizingserenade") ?? false;
            if (isStormJavelin && __instance.craftType == 1)
            {
                LogDebug("Preventing vitalizingSerenade from being removed");
                ___BG_Remove.Disable();
                return;
            }
            return;
        }





        [HarmonyPostfix]
        [HarmonyPatch(typeof(Functions), nameof(Functions.GetCardByRarity))]
        public static void GetCardByRarityPostfix(ref string __result, int rarity, CardData _cardData, bool isChallenge = false)
        {

            if (!EnableRandomSerenades.Value)
            {
                return;
            }
            LogDebug($"GetCardByRarityPostfix {rarity} {__result}");
            int serenadeRewardChance = 5;
            string seed = AtOManager.Instance?.currentMapNode ?? "" + AtOManager.Instance.GetGameId() + __result;
            UnityEngine.Random.InitState(seed.GetDeterministicHashCode());
            bool addSerenadeReward = UnityEngine.Random.Range(0, 100) < serenadeRewardChance;
            if (!addSerenadeReward)
            {
                return;
            }


            seed += "1";
            string cardName = GetStormJavelinUpgraded(seed);
            __result = cardName;

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hero), "SetInitialCards")]
        public static void SetInitialCardsPostfix(ref Hero __instance, HeroData heroData)
        {
            LogDebug("SetInitialCardsPostfix");
            // UnityEngine.Random.InitState((AtOManager.Instance.GetGameId() + __instance.SourceName + PluginInfo.PLUGIN_GUID).GetDeterministicHashCode());
            List<string> cards = __instance?.Cards;
            cards?.Add("vitalizingserenadespecialb");
            cards?.Add("vitalizingserenadespecialb");
            __instance.Cards = cards;
        }








    }
}