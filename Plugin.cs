// These are your imports, mostly you'll be needing these 5 for every plugin. Some will need more.

using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using System;
using static Obeliskial_Essentials.CardDescriptionNew;


// The Plugin csharp file is used to specify some general info about your plugin. and set up things for 


// Make sure all your files have the same namespace and this namespace matches the RootNamespace in the .csproj file
// All files that are in the same namespace are compiled together and can "see" each other more easily.

namespace StormJavelin
{
    // These are used to create the actual plugin. If you don't need Obeliskial Essentials for your mod, 
    // delete the BepInDependency and the associated code "RegisterMod()" below.

    // If you have other dependencies, such as obeliskial content, make sure to include them here.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials")] // this is the name of the .dll in the !libs folder.
    [BepInDependency("com.stiffmeds.obeliskialcontent")] // this is the name of the .dll in the !libs folder.
    [BepInProcess("AcrossTheObelisk.exe")] //Don't change this

    // If PluginInfo isn't working, you are either:
    // 1. Using BepInEx v6
    // 2. Have an issue with your csproj file (not loading the analyzer or BepInEx appropriately)
    // 3. You have an issue with your solution file (not referencing the correct csproj file)


    public class Plugin : BaseUnityPlugin
    {

        // If desired, you can create configs for users by creating a ConfigEntry object here, 
        // Configs allows users to specify certain things about the mod. 
        // The most common would be a flag to enable/disable portions of the mod or the entire mod.

        // You can use: config = Config.Bind() to set the title, default value, and description of the config.
        // It automatically creates the appropriate configs.

        public static ConfigEntry<bool> EnableMod { get; set; }
        public static ConfigEntry<bool> EnableDebugging { get; set; }
        public static ConfigEntry<bool> EnableRandomJavelins { get; set; }
        // public static ConfigEntry<bool> EnableIncreasedRods { get; set; }
        public static ConfigEntry<bool> EnableBonusJavelins { get; set; }
        public static ConfigEntry<bool> ChangeAllNames { get; set; }

        internal int ModDate = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;

        public static string debugBase = $"{PluginInfo.PLUGIN_GUID} ";


        private void Awake()
        {

            // The Logger will allow you to print things to the LogOutput (found in the BepInEx directory)
            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");

            // Sets the title, default values, and descriptions
            EnableMod = Config.Bind(new ConfigDefinition("StormJavelin", "EnableMod"), true, new ConfigDescription("Enables the mod. If false, the mod will not work then next time you load the game."));
            EnableDebugging = Config.Bind(new ConfigDefinition("StormJavelin", "EnableDebugging"), true, new ConfigDescription("Enables the debugging"));
            EnableRandomJavelins = Config.Bind(new ConfigDefinition("StormJavelin", "Random Javelins"), true, new ConfigDescription("Storm Javelin is now a card reward for all."));
            ChangeAllNames = Config.Bind(new ConfigDefinition("StormJavelin", "ChangeAllNames"), true, new ConfigDescription("Makes it so that all cards are named Storm Javelin. Restart the game upon changing this."));
            EnableBonusJavelins = Config.Bind(new ConfigDefinition("StormJavelin", "Bonus Javelins"), true, new ConfigDescription("Chace to shuffle Javelins into your deck each turn."));


            // DevMode = Config.Bind(new ConfigDefinition("StormJavelin", "DevMode"), false, new ConfigDescription("Enables all of the things for testing."));


            // Register with Obeliskial Essentials, delete this if you don't need it.
            RegisterMod(
                _name: PluginInfo.PLUGIN_NAME,
                _author: "binbin",
                _description: "Storm Javelin",
                _version: PluginInfo.PLUGIN_VERSION,
                _date: ModDate,
                _link: @"https://github.com/binbinmods/StormJavelin",
                _contentFolder: "StormJavelin"
            );

            // ring of sparks - DONE
            // sacred sparks - done
            // rods from god - done
            // thunder shield - TBD
            // crackling gauntlet 
            // electric armor
            // storm necklace


            string text = $"{medsSpriteText("sharp")} on heroes increases {medsSpriteText("lightning")} damage by 2% per charge";
            string cardId = "javringofsparks";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);

            text = $"{medsSpriteText("sharp")} on heroes increases {medsSpriteText("lightning")} damage by 4% per charge";
            cardId = "javringofsparksrare";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);

            text = $"{medsSpriteText("spark")} on monsters heals heroes by 1 when attacked";
            cardId = "javsacredsparks";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: true);

            text = $"{medsSpriteText("spark")} on monsters reduces {medsSpriteText("piercing")} resistance by 0.5% per charge";
            cardId = "javrodsfromgod";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);

            text = $"{medsSpriteText("spark")} on monsters reduces {medsSpriteText("piercing")} resistance by 1% per charge";
            cardId = "javrodsfromgodrare";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);

            text = $"+5% All Damage and Healing Done for each Storm Javelin in your Draw Pile";
            cardId = "javthundershield";
            AddTextToCardDescription(text, TextLocation.ItemBeginning, cardId, includeRare: false);

            text = $"+10% All Damage and Healing Done for each Storm Javelin in your Draw Pile";
            cardId = "javthundershieldrare";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);


            text = $"{medsSpriteText("powerful")} on heroes have -15 Max. charges and +1 Max. charge for each card in their Draw Pile";
            cardId = "javcracklinggauntlet";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);


            text = $"{medsSpriteText("powerful")} on heroes have -10 Max. charges and +1 Max. charge for each card in their Draw Pile";
            cardId = "javcracklinggauntletrare";
            AddTextToCardDescription(text, TextLocation.End, cardId, includeRare: false);
            // apply patches, this functionally runs all the code for Harmony, running your mod
            if (EnableMod.Value) { harmony.PatchAll(); }
        }


        // These are some functions to make debugging a tiny bit easier.
        internal static void LogDebug(string msg)
        {
            if (EnableDebugging.Value)
            {
                Log.LogDebug(debugBase + msg);
            }

        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }
    }
}