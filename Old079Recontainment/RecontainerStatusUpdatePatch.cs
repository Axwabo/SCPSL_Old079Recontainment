using System.Diagnostics;
using Axwabo.Util;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079), "UpdateStatus")]
    internal static class RecontainerStatusUpdatePatch {
        private static bool Prefix(Recontainer079 __instance, int engagedGenerators) {
            if (Recontainer079.AllGenerators.Count <= engagedGenerators || !Plugin079.Any079)
                return true;
            var original = __instance.Get<string>("_announcementAllActivated");
            if (CassieHelper.RemoveNoise(original).Contains("scp 0 7 9 recontainment procedure commencing"))
                return true;
            var newPart = CassieHelper.AddGlitchesAndJams(
                " . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in tminus . 1 minute",
                0.03f, 0.03f);
            Plugin079.DelayDuration = CassieHelper.CalculateDuration(newPart) + 69; //not perfect but funny number haha lol xd
            __instance.Set("_announcementAllActivated", original + newPart);
            return true;
        }

        private static void Postfix(Recontainer079 __instance, int engagedGenerators) {
            if (Recontainer079.AllGenerators.Count > engagedGenerators || !Plugin079.Cfg.AutoRecontain ||
                !Plugin079.Cfg.IsEnabled || __instance.Get<bool>("_alreadyRecontained"))
                return;
            Plugin079.DelayStopwatch.Restart();
            var glass = __instance.Get<BreakableWindow>("_activatorGlass");
            glass.Call("ServerDamageWindow", glass.health);
            __instance.Set("_activationDelay", -1);
            __instance.Get<Stopwatch>("_delayStopwatch").Stop();
            __instance.Get<Stopwatch>("_unlockStopwatch").Stop();
        }
    }
}