using HarmonyLib;
using UnityEngine;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Scp079PlayerScript), "ServerUpdateMana")]
    internal static class ManaPatch {
        private static void Postfix(Scp079PlayerScript __instance) {
            if (!Plugin079.DelayStopwatch.IsRunning)
                return;
            __instance.Network_curMana =
                Mathf.Clamp(__instance.Network_curMana + Plugin079.Cfg.PowerPerSecond * Time.deltaTime, 0,
                    __instance.levels[__instance.Lvl].maxMana);
        }
    }
}