using HarmonyLib;
using flanne;
using flanne.UI;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;

namespace GeneralUtilityMod.Patches
{
    [HarmonyPatch(typeof(TimerUI), "Update", MethodType.Normal)]
    class ReverseTimerPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!GUMPlugin.hasReverseTimer.Value) return instructions;

            var codes = new List<CodeInstruction>(instructions);
            codes.RemoveRange(0, 4);
            for (var i = 1; i < codes.Count; i++)
            {
                Debug.Log(codes[i]);
            }
            return codes.AsEnumerable();
        }
    }
}

