using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Axwabo.Util;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), "Update")]
    internal static class CassiePatch {
        private static bool Prefix() {
            if (Warhead.IsDetonated ||
                Recontainer079.AllGenerators.Count > Recontainer079.AllGenerators.Count(g => g.Engaged))
                return true;
            var deaths = typeof(NineTailedFoxAnnouncer).Get<List<NineTailedFoxAnnouncer.ScpDeath>>("scpDeaths");
            if (deaths.Count < 1)
                return true;
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var t in deaths) {
                var death = t;
                if (death.scpSubjects.Any(r => r.roleId == RoleType.Scp079) || Plugin079
                    .ClearCassieMessage(death.announcement).Contains("all scpsubjects have been secured"))
                    continue;
                death.announcement +=
                    Plugin079.CassieGlitchJam(
                        " . . all scpsubjects have been secured . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in . tminus 1 minute",
                        0.05f, 0.05f);
                var recontainer = Object.FindObjectOfType<Recontainer079>();
                recontainer.Set("_activationDelay", -1);
                recontainer.Get<Stopwatch>("_delayStopwatch").Stop();
                Plugin079.OneMinuteStopwatch.Restart();
            }

            return true;
        }
    }
}