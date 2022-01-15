using Axwabo.Util;
using Exiled.API.Features;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079), "Start")]
    internal static class RecontainerStartPatch {
        private static void Postfix(Recontainer079 __instance) {
            if (!Plugin079.Cfg.AutoRecontain || !Plugin079.Cfg.IsEnabled ||
                __instance.Get<bool>("_alreadyRecontained") ||
                Warhead.Controller != null && Warhead.Controller.detonated)
                return;
            var original = __instance.Get<string>("_announcementAllActivated");
            if (Plugin079.ClearCassieMessage(original).Contains("scp 0 7 9 recontainment procedure commencing"))
                return;
            var newPart = Plugin079.CassieGlitchJam(
                " . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in tminus . 1 minute",
                0.03f, 0.03f);
            Plugin079.DelayDuration = Cassie.CalculateDuration(newPart) + 69; //not perfect but funny number haha lol xd
            __instance.Set("_announcementAllActivated", original + newPart);
        }
    }
}