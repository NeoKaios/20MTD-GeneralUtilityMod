using HarmonyLib;
using flanne.UI;
using System.Collections.Generic;
using System.Linq;

namespace GeneralUtilityMod.Patches
{
    [HarmonyPatch(typeof(TimerUI), "Update", MethodType.Normal)]
    class ReverseTimerPatch
    {
        // Could be done with a postfix patch but transpiler is cooler
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!GUMPlugin.hasReverseTimer.Value) return instructions;

            var codes = new List<CodeInstruction>(instructions);
            codes.RemoveRange(0, 4); // remove the if(endless) test, now the method always run TimeToString
            return codes.AsEnumerable();
        }
    }
}

