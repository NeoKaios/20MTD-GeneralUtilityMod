using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using System;
using MTDUI;

namespace GeneralUtilityMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class GUMPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<Patches.MainMenuState> menuStartAt;
        public static ConfigEntry<bool> noRune;
        public static ConfigEntry<bool> hasReverseTimer;
        private void Awake()
        {
            menuStartAt = Config.Bind("_General", "Menu Starting State", Patches.MainMenuState.Loadout, "State in which the title screen will load\n/!\\ WaitToBattle: Will directly launch Battle Scene | Cannot reach menu with this option");
            noRune = Config.Bind("Default", "No Rune", false, "Play standard and quickplay without any rune");
            hasReverseTimer = Config.Bind("Default", "Reverse Timer", false, "Reverse in-game timer, starts at 00:00, ends at 20:00");

            try
            {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
                ModOptions.Register("GUM", menuStartAt);
                ModOptions.Register("GUM", noRune);
                ModOptions.Register("GUM", hasReverseTimer);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + '\n' + e.StackTrace);
                Logger.LogError($"{PluginInfo.PLUGIN_GUID} failed to patch methods.");
            }
        }
    }
}
