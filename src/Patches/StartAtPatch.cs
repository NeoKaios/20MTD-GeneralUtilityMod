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
        static void WaitEnter_prefix(TitleScreenController ___owner)
        {
            // Avoid loading default character, map and gun
            if (GUMPlugin.menuStartAt.Value == MainMenuState.WaitToBattle)
            {
                SelectedMap.MapData = ___owner.modeSelectMenu.toggledData;
            }
            if (GUMPlugin.menuStartAt.Value == MainMenuState.Mode
            || GUMPlugin.menuStartAt.Value == MainMenuState.WaitToBattle)
            {
                GunSelectState gss = ___owner.GetState<GunSelectState>();
                AccessTools.DeclaredMethod(typeof(GunSelectState), "SetLoadout").Invoke(gss, null);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterSelectState), "Enter")]
        [HarmonyPatch(typeof(RuneMenuGunState), "Enter")]
        [HarmonyPatch(typeof(ModeSelectState), "Enter")]
        static void UIScreenStateEnterPrefix()
        {
            Cursor.visible = true;
        }

        [HarmonyPatch(typeof(GunSelectState), "Enter")]
        [HarmonyPrefix]
        static void GunEnter_prefix(TitleScreenController ___owner)
        {
            // Fix used when startAt is set to Gun, Mode or Rune
            Cursor.visible = true;
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

