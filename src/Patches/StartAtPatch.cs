using System.ComponentModel;
using UnityEngine;
using HarmonyLib;
using flanne;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using flanne.TitleScreen;
using System;

namespace GeneralUtilityMod.Patches
{
    public enum MainMenuState
    {
        [DescriptionAttribute("DescriptionAttribute")]
        Main,
        Character,
        Gun,
        Rune,
        Mode,
        WaitToBattle,
    }


    [HarmonyPatch]
    class StartAtPatch
    {
        static readonly string playerPrefsCharacterKey = "LastCharacterSelected";
        static readonly string playerPrefsGunKey = "LastGunSelected";
        static Dictionary<MainMenuState, Type> stringToState = new Dictionary<MainMenuState, Type>() {
                {MainMenuState.Main, typeof(TitleMainMenuState)},
                {MainMenuState.Character, typeof(CharacterSelectState)},
                {MainMenuState.Gun, typeof(GunSelectState)},
                {MainMenuState.Rune, typeof(RuneMenuGunState)},
                {MainMenuState.Mode, typeof(ModeSelectState)},
                {MainMenuState.WaitToBattle, typeof(WaitToLoadIntoBattleState)},
        };

        [HarmonyPatch(typeof(WaitToLoadIntoBattleState), "Enter")]
        [HarmonyPrefix]
        static void LoadoutFix(TitleScreenController ___owner)
        {
            if (Loadout.CharacterSelection == null)
            {
                Loadout.CharacterIndex = PlayerPrefs.GetInt(playerPrefsCharacterKey, 0);
                Loadout.CharacterSelection = ___owner.characterMenu[Loadout.CharacterIndex];
            }
            if (Loadout.GunSelection == null)
            {
                Loadout.GunIndex = PlayerPrefs.GetInt(playerPrefsGunKey, 0);
                Loadout.GunSelection = ___owner.gunMenu[Loadout.GunIndex];
            }
            else
            {
                PlayerPrefs.SetInt(playerPrefsGunKey, Loadout.GunIndex);
            }
            if (SelectedMap.MapData == null)
            {
                if (___owner.modeSelectMenu.currIndex == 1)
                {
                    SelectedMap.MapData = ___owner.mapSelectMenu.toggledData;
                }
                else
                {
                    SelectedMap.MapData = ___owner.modeSelectMenu.toggledData;
                }
            }
        }

        [HarmonyPatch(typeof(ModeSelectState), "Enter")]
        [HarmonyPatch(typeof(RuneMenuGunState), "Enter")]
        [HarmonyPatch(typeof(CharacterSelectState), "Enter")]
        [HarmonyPatch(typeof(GunSelectState), "Enter")]
        [HarmonyPrefix]
        static void UIScreenStateFix(TitleScreenController ___owner)
        {
            Cursor.visible = true;
            ___owner.checkRunesPromptArrow.enabled = false;
            ___owner.selectPanel.Show();
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

