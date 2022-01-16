using System.Collections.Generic;
using System.Linq;
using Axwabo.Util;
using Exiled.API.Features;
using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), "Update")]
    internal static class CassiePatch {
        private static bool Prefix() {
            if (!Plugin079.Cfg.AutoRecontain || !Plugin079.Cfg.IsEnabled ||
                Recontainer079.AllGenerators.All(g => g.Engaged) ||
                Plugin079.Recontained || !Plugin079.Any079 ||
                Warhead.Controller != null && Warhead.Controller.detonated)
                return true;
            var deaths =
                typeof(NineTailedFoxAnnouncer).StaticGet<List<NineTailedFoxAnnouncer.ScpDeath>>("scpDeaths");
            if (deaths.Count < 1) {
                CheckIfAnyScpIsPresent();
                return true;
            }

            var list = new List<NineTailedFoxAnnouncer.ScpDeath>(deaths);
            for (var i = 0; i < list.Count; i++) {
                var t = list[i];
                if (t.scpSubjects.Any(r => r.roleId == RoleType.Scp079) || CassieHelper
                    .RemoveNoise(t.announcement).Contains("all scpsubjects have been secured"))
                    continue;
                var newPart = CassieHelper.AddGlitchesAndJams(
                    " . . all scpsubjects have been secured . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in . tminus 1 minute",
                    0.03f, 0.03f);
                if (!Plugin079.PrepareOvercharge(
                    CassieHelper.CalculateDuration(newPart) + 69)) //not perfect but funny number haha lol xd
                    return true;

                deaths[i] = new NineTailedFoxAnnouncer.ScpDeath {
                    scpSubjects = t.scpSubjects,
                    announcement = t.announcement + newPart
                };
            }

            return true;
        }

        // if an SCP termination is not announced (e.g. SCP-049-2), this code will make sure that the overcharge happens unless any other SCPs are present
        private static void CheckIfAnyScpIsPresent() {
            if (!Plugin079.Cfg.IsEnabled || Recontainer079.AllGenerators.All(g => g.Engaged) ||
                Plugin079.CheckRecontainer(e => e.Get<bool>("_alreadyRecontained")) ||
                Warhead.Controller != null && Warhead.Controller.detonated ||
                !ReferenceHub.GetAllHubs().Values.All(hub => {
                    var ccm = hub.characterClassManager;
                    return hub == ReferenceHub.HostHub || hub == ReferenceHub.LocalHub || !ccm.IsAnyScp() ||
                           ccm.Scp079.iAm079;
                }) || NineTailedFoxAnnouncer.singleton.queue.Any(e =>
                    CassieHelper.RemoveNoise(e.collection)
                        .Contains("scp 0 7 9 recontainment procedure commencing")) ||
                Plugin079.DelayStopwatch.IsRunning ||
                //don't overcharge if only one player is on the server as 079
                RoundSummary.singleton.Get<bool>("_keepRoundOnOne") && PlayerManager.players.Count < 2)
                return;
            var announcement = CassieHelper.AddGlitchesAndJams(
                "all scpsubjects have been secured . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in tminus . 1 minute",
                0.035f, 0.035f);
            if (Plugin079.PrepareOvercharge(CassieHelper.CalculateDuration(announcement) + 69))
                Cassie.Message(announcement);
        }
    }
}