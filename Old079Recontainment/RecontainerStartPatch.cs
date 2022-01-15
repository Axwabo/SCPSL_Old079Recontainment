using Axwabo.Util;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079), "Start")]
    internal static class RecontainerStartPatch {
        private static void Postfix(Recontainer079 __instance) {
            if (!Plugin079.Cfg.AutoRecontain)
                return;
            var original = __instance.Get<string>("_announcementAllActivated");
            if (Plugin079.ClearCassieMessage(original).Contains("scp 0 7 9 recontainment procedure commencing"))
                return;
            __instance.Set("_announcementAllActivated",
                original +
                Plugin079.CassieGlitchJam(
                    " . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in tminus . 1 minute",
                    0.05f, 0.05f));
        }
    }
}