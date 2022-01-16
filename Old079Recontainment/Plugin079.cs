using System;
using System.Diagnostics;
using System.Linq;
using Axwabo.Util;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using PlayerStatsSystem;
using Object = UnityEngine.Object;

namespace Old079Recontainment {
    public class Plugin079 : Plugin<Config079> {
        public static Plugin079 Singleton { get; private set; }
        internal static Config079 Cfg => Singleton.Config;
        private Harmony _harmony;
        internal static readonly Stopwatch DelayStopwatch = new Stopwatch();
        internal static float DelayDuration = 80f;

        public Plugin079() {
            Singleton = this;
        }

        public override void OnEnabled() {
            base.OnEnabled();
            _harmony = new Harmony("mc.axwabo.old079");
            try {
                _harmony.PatchAll();
            } catch (Exception e) {
                Log.Error("Patching failed!\n" + e);
                return;
            }

            Exiled.Events.Handlers.Map.Generated += EventHandlers.MapGenerated;
            PlayerStats.OnAnyPlayerDied += EventHandlers.PlayerDied;
            Log.Info($"Config: Extra Generators: {Config.ExtraGenerators}; Auto Recontain: {Config.AutoRecontain}");
        }

        public override void OnDisabled() {
            base.OnDisabled();
            Exiled.Events.Handlers.Map.Generated -= EventHandlers.MapGenerated;
            _harmony.UnpatchAll();
        }

        public override string Name { get; } = "Old079Recontainment";
        public override string Author { get; } = "Axwabo";
        public override PluginPriority Priority { get; } = PluginPriority.Highest;
        public override Version Version { get; } = new Version(1, 0, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(4, 0);

        public static bool Any079 =>
            ReferenceHub.GetAllHubs().Values.Any(hub => hub.characterClassManager.Scp079.iAm079);

        internal static bool PrepareOvercharge(float duration) {
            var recontainer = Object.FindObjectOfType<Recontainer079>();
            if (recontainer == null)
                return false;
            DelayDuration = duration;
            DelayStopwatch.Restart();
            recontainer.Set("_activationDelay", -1);
            recontainer.Get<Stopwatch>("_delayStopwatch").Stop();
            recontainer.Get<Stopwatch>("_unlockStopwatch").Stop();
            var glass = recontainer.Get<BreakableWindow>("_activatorGlass");
            glass.Call("ServerDamageWindow", glass.health);
            return true;
        }

        public static bool CheckRecontainer(Func<Recontainer079, bool> func) {
            var o = Object.FindObjectOfType<Recontainer079>();
            return o != null && func(o);
        }

        public static bool Recontained => CheckRecontainer(e => e.Get<bool>("_alreadyRecontained"));
    }
}