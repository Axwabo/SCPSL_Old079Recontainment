using System.Diagnostics;
using Axwabo.Util;
using Exiled.API.Features;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079), "RefreshActivator")]
    internal static class RecontainerActivatorUpdatePatch {
        private static bool Prefix(Recontainer079 __instance) {
            if (!Plugin079.Cfg.AutoRecontain || !Plugin079.Cfg.IsEnabled ||
                __instance.Get<bool>("_alreadyRecontained") ||
                !Plugin079.DelayStopwatch.IsRunning || !Plugin079.Any079())
                return true;
            if (Plugin079.DelayStopwatch.Elapsed.TotalSeconds < Plugin079.DelayDuration) {
                __instance.Get<Stopwatch>("_unlockStopwatch").Stop();
                __instance.Get<Stopwatch>("_delayStopwatch").Stop();
                return false;
            }

            __instance.Get<Stopwatch>("_delayStopwatch").Restart();
            __instance.Get<Stopwatch>("_unlockStopwatch").Restart();
            __instance.Set("_activationDelay", 7f);
            Plugin079.DelayStopwatch.Stop();
            return true;
        }

        private static void Postfix(Recontainer079 __instance) {
            if (Plugin079.DelayStopwatch.Elapsed.TotalSeconds < Plugin079.DelayDuration)
                __instance.Get<Stopwatch>("_unlockStopwatch").Stop();
        }
    }
}