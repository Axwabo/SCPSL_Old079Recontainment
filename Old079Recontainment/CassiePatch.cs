using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Axwabo.Util;
using Exiled.API.Features;
using HarmonyLib;
using Object = UnityEngine.Object;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), "Update")]
    internal static class CassiePatch {
        private static bool Prefix() {
            if (!Plugin079.Cfg.AutoRecontain || !Plugin079.Cfg.IsEnabled ||
                Recontainer079.AllGenerators.All(g => g.Engaged) ||
                !Plugin079.Any079() || Warhead.Controller != null && Warhead.Controller.detonated)
                return true;
            var deaths =
                typeof(NineTailedFoxAnnouncer).StaticGet<List<NineTailedFoxAnnouncer.ScpDeath>>("scpDeaths");
            if (deaths.Count < 1)
                return true;
            var list = new List<NineTailedFoxAnnouncer.ScpDeath>(deaths);
            for (var i = 0; i < list.Count; i++) {
                var t = list[i];
                if (t.scpSubjects.Any(r => r.roleId == RoleType.Scp079) || Plugin079
                    .ClearCassieMessage(t.announcement).Contains("all scpsubjects have been secured"))
                    continue;
                var newPart = Plugin079.CassieGlitchJam(
                    " . . all scpsubjects have been secured . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in . tminus 1 minute",
                    0.03f, 0.03f);
                Plugin079.DelayDuration =
                    Cassie.CalculateDuration(newPart) + 69; //not perfect but funny number haha lol xd
                deaths[i] = new NineTailedFoxAnnouncer.ScpDeath {
                    scpSubjects = t.scpSubjects,
                    announcement = t.announcement + newPart
                };
                Plugin079.DelayStopwatch.Restart();
                var recontainer = Object.FindObjectOfType<Recontainer079>();
                recontainer.Set("_activationDelay", -1);
                recontainer.Get<Stopwatch>("_delayStopwatch").Stop();
                recontainer.Get<Stopwatch>("_unlockStopwatch").Stop();
                var glass = recontainer.Get<BreakableWindow>("_activatorGlass");
                glass.Call("ServerDamageWindow", glass.health);
            }

            return true;
        }
    }
}