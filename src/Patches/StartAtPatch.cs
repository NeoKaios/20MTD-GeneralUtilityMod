using System.ComponentModel;
using UnityEngine;
using HarmonyLib;
using flanne;
using flanne.RuneSystem;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using flanne.TitleScreen;
using System;

namespace GeneralUtilityMod.Patches
{
    public enum MainMenuState
    {
        [DescriptionAttribute("Descriptionde ed ede Attribute")]
        Main,
        Loadout,
        Mode,
        WaitToBattle,
        Rune,
        Option,
        Language
    }


    [HarmonyPatch]
    class StartAtPatch
    {
        static Dictionary<MainMenuState, Type> stringToState = new Dictionary<MainMenuState, Type>() {
                {MainMenuState.Loadout, typeof(LoadoutSelectState)},
                {MainMenuState.Main, typeof(TitleMainMenuState)},
                {MainMenuState.Option, typeof(OptionsMenuState)},
                {MainMenuState.Language, typeof(LangaugeState)},
                {MainMenuState.Rune, typeof(RuneMenuState)},
                {MainMenuState.WaitToBattle, typeof(WaitToLoadIntoBattleState)},
                {MainMenuState.Mode, typeof(ModeSelectState)}
        };

        [HarmonyPatch(typeof(WaitToLoadIntoBattleState), "Enter")]
        [HarmonyPrefix]
        static void WaitEnter_prefix(TitleScreenController ___owner)
        {
            // Avoid loading default character, map and gun
            if (GUMPlugin.menuStartAt.Value == MainMenuState.WaitToBattle)
            {
                SelectedMap.MapData = ___owner.modeSelectMenu.toggledData;
            }
            if (GUMPlugin.menuStartAt.Value == MainMenuState.Mode || GUMPlugin.menuStartAt.Value == MainMenuState.WaitToBattle)
            {
                LoadoutSelectState lss = ___owner.GetState<LoadoutSelectState>();
                AccessTools.DeclaredMethod(typeof(LoadoutSelectState), "SetLoadout").Invoke(lss, null);
            }
            // Remove runes
            if (GUMPlugin.noRune.Value)
            {
                Loadout.RuneSelection = null;
            }
        }


        [HarmonyPatch(typeof(InitState), "WaitToLoadCR", MethodType.Enumerator)]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Type state;
            if (!stringToState.TryGetValue(GUMPlugin.menuStartAt.Value, out state)) // No val
            {
                Debug.LogError("Wrong config string: \"" + GUMPlugin.menuStartAt.Value + "\" State unknown -> no modification done");
                return instructions;
            }
            if (state == typeof(TitleMainMenuState)) // Default val
            {
                return instructions;
            }
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                // Finding the correct spot to change in WaitToLoadCR IL code
                if (codes[i].opcode == OpCodes.Callvirt && codes[i - 1].opcode == OpCodes.Ldfld)
                {
                    codes[i].operand = AccessTools.Method(typeof(StateMachine), nameof(StateMachine.ChangeState), null, new Type[] { state });
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
}

