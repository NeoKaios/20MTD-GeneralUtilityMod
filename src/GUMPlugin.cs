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
        public static ConfigEntry<bool> activateMod;
        public static ConfigEntry<Patches.MainMenuState> menuStartAt;
        public static ConfigEntry<bool> noRune;
        public static ConfigEntry<bool> hasReverseTimer;
        private void Awake()
        {
            activateMod = Config.Bind("Activation", "G.U.M.", true, "If false, the mod does not load");
            MTDUI.ModOptions.RegisterOptionInModList(activateMod);
            if (!activateMod.Value)
            {
                Logger.LogInfo("<Inactive>");
                return;
            }
            menuStartAt = Config.Bind("Utility", "Menu Starting State", Patches.MainMenuState.Character, "State in which the title screen will load\n/!\\ WaitToBattle: Will directly launch Battle Scene | Cannot reach menu with this option");
            noRune = Config.Bind("Gameplay", "No Rune", false, "Play standard and quickplay without any rune");
            hasReverseTimer = Config.Bind("Utility", "Reverse Timer", false, "Reverse in-game timer, starts at 00:00, ends at 20:00");

            try
            {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
                string mod = "G.U.M.";
                if (menuStartAt.Value == Patches.MainMenuState.WaitToBattle)
                    ModOptions.Register(menuStartAt, subMenuName: mod); // Not be stuck in WaitToBattle mod
                else ModOptions.Register(menuStartAt, null, ConfigEntryLocationType.MainOnly, subMenuName: mod);
                ModOptions.Register(noRune, null, ConfigEntryLocationType.MainOnly, subMenuName: mod);
                ModOptions.Register(hasReverseTimer, null, ConfigEntryLocationType.MainOnly, subMenuName: mod);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + '\n' + e.StackTrace);
                Logger.LogError($"{PluginInfo.PLUGIN_GUID} failed to patch methods.");
            }
        }
    }
}
