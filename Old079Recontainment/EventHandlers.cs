using System.Diagnostics;
using System.Linq;
using Axwabo.Util;
using Exiled.API.Features;
using MapGeneration.Distributors;
using PlayerStatsSystem;
using UnityEngine;

namespace Old079Recontainment {
    internal static class EventHandlers {
        internal static void MapGenerated() {
            Plugin079.DelayStopwatch.Reset();
            var distributor = Object.FindObjectOfType<StructureDistributor>();
            for (var i = 0; i < Plugin079.Cfg.ExtraGenerators; i++) {
                var point = StructureSpawnpoint.AvailableInstances.Where(e =>
                    e.CompatibleStructures.Contains(StructureType.Scp079Generator)).Random();
                if (point == null)
                    return;
                StructureSpawnpoint.AvailableInstances.Remove(point);
                distributor.Call("SpawnStructure",
                    distributor.Get<SpawnablesDistributorSettings>("Settings").SpawnableStructures
                        .FirstOrDefault(e => e is Scp079Generator), point.transform, point.TriggerDoorName);
            }

            if (!Plugin079.Cfg.AutoRecontain || !Plugin079.Cfg.IsEnabled)
                return;
            var recontainer = Object.FindObjectOfType<Recontainer079>();
            recontainer.Get<Stopwatch>("_delayStopwatch").Stop();
            recontainer.Set("_activationDelay", -1);
            var glass = recontainer.Get<BreakableWindow>("_activatorGlass");
            glass.Call("ServerDamageWindow", glass.health);
        }

        public static void PlayerDied(ReferenceHub hub, DamageHandlerBase damage) {
            if (hub.characterClassManager.IsAnyScp() && !hub.characterClassManager.Scp079.iAm079 &&
                Plugin079.Any079() && ReferenceHub.GetAllHubs().Values.All(e =>
                    (!e.characterClassManager.IsAnyScp() || e.characterClassManager.Scp079.iAm079) && e != hub) &&
                !NineTailedFoxAnnouncer.singleton.queue.Any(e =>
                    Plugin079.ClearCassieMessage(e.collection).StartsWith("$$$ all scpsubjects have been secured")))
                Cassie.DelayedGlitchyMessage(
                    "$$$ all scpsubjects have been secured . scp 0 7 9 recontainment procedure commencing . heavy containment zone overcharge in tminus 1 minute",
                    0, 0.035f, 0.03f);
        }
    }
}