using Axwabo.Util;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079), "Start")]
    internal static class RecontainerStartPatch {
        private static void Postfix(Recontainer079 __instance) {
            if (!Plugin079.Cfg.AutoRecontain)
                return;
            __instance.Set("_announcementAllActivated",
                __instance.Get<string>("_announcementAllActivated") +
                " . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in tminus . 1 minute");
        }
    }
}