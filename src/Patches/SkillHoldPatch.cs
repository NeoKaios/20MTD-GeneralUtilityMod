using System;
using HarmonyLib;
using flanne;
using flanne.CharacterPassives;
using flanne.Player;
using UnityEngine.InputSystem;

namespace GeneralUtilityMod.Patches
{
    [HarmonyPatch]
    class SkillHoldPatch
    {
        static Traverse skillCallback = new Traverse(typeof(SkillPassive));

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillPassive), "Start")]
        static void SkillPassiveStartPrefix(SkillPassive __instance)
        {
            skillCallback = Traverse.Create(__instance).Method("PerformSkillCallback", new Type[] { typeof(InputAction.CallbackContext) });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(IdleState), "Enter")]
        static void IdleStateEnterPostfix(PlayerController ___owner)
        {
            if (GUMPlugin.holdSkillTrigger.Value && skillCallback.MethodExists() && ___owner.playerInput.actions["Skill"].ReadValue<float>() != 0f)
            {
                skillCallback.GetValue(new object[] { null });
            }
        }
    }
}

