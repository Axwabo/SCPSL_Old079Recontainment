using System.Diagnostics;
using System.Linq;
using Axwabo.Util;
using MapGeneration.Distributors;
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
    }
}