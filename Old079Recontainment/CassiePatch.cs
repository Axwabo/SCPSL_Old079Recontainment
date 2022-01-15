using System.Collections.Generic;
using Axwabo.Util;
using HarmonyLib;
using UnityEngine;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), "Update")]
    public class CassiePatch {
        public static bool Prefix() {
            var deaths = typeof(NineTailedFoxAnnouncer).Get<List<NineTailedFoxAnnouncer.ScpDeath>>("scpDeaths");
            if (deaths.Count < 1)
                return true;
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var t in deaths) {
                var death = t;
                if (death.announcement.ToLower().Contains("all scpsubjects have been secured"))
                    continue;
                death.announcement +=
                    Plugin079.CassieGlitchJam(
                        " . . all scpsubjects have been secured . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in . tminus 1 minute",
                        0.05f, 0.05f);
                Object.FindObjectOfType<Recontainer079>().Set("_activationDelay", 60);
            }

            return true;
        }
    }
}