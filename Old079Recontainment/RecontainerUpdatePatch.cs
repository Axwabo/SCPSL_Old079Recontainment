using System.Diagnostics;
using Axwabo.Util;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079), "UpdateStatus")]
    internal static class RecontainerUpdatePatch {
        private static void Postfix(Recontainer079 __instance, int engagedGenerators) {
            if (Recontainer079.AllGenerators.Count > engagedGenerators || !Plugin079.Cfg.AutoRecontain)
                return;
            var glass = __instance.Get<BreakableWindow>("_activatorGlass");
            glass.Call("ServerDamageWindow", glass.health);
            __instance.Set("_activationDelay", -1);
            __instance.Get<Stopwatch>("_delayStopwatch").Stop();
            Plugin079.OneMinuteStopwatch.Restart();
        }
    }
}