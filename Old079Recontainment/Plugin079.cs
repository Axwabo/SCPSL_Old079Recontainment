using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Axwabo.Util;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
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
            Timing.RunCoroutine(Update(), "mc.axwabo.079");
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
            Timing.KillCoroutines("mc.axwabo.079");
            Exiled.Events.Handlers.Map.Generated -= EventHandlers.MapGenerated;
            _harmony.UnpatchAll();
        }

        public override string Name { get; } = "Old079Recontainment";
        public override string Author { get; } = "Axwabo";
        public override PluginPriority Priority { get; } = PluginPriority.Highest;
        public override Version Version { get; } = new Version(1, 0, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(4, 0);

        private static readonly Regex CassieSilenceRegex = new Regex("([^A-z0-9._ ])");

        public static string CassieGlitchJam(string message, float glitchChance, float jamChance,
            bool skipSilence = true) {
            var strArray = message.Split(' ');
            var newWords = new List<string>();
            newWords.EnsureCapacity(strArray.Length);
            for (var index = 0; index < strArray.Length; ++index) {
                var s = strArray[index];
                newWords.Add(s);
                if (index >= strArray.Length - 2 || CassieSilenceRegex.Replace(s, "").Equals(".") && skipSilence)
                    continue;
                if (UnityEngine.Random.value < (double) glitchChance)
                    newWords.Add(".G" + UnityEngine.Random.Range(1, 7));
                if (UnityEngine.Random.value < (double) jamChance)
                    newWords.Add("JAM_" + UnityEngine.Random.Range(0, 70).ToString("000") + "_" +
                                 UnityEngine.Random.Range(2, 6));
            }

            return newWords.Aggregate("", (current, newWord) => current + newWord + " ");
        }

        private static readonly Regex CassieNoiseRegex = new Regex("(jam_[0-9][0-9][0-9]_[0-9]|.g[0-9])( ?)");

        public static string ClearCassieMessage(string s) {
            return CassieNoiseRegex.Replace(s.ToLower(), "");
        }

        public static bool Any079() {
            return ReferenceHub.GetAllHubs().Values.Any(hub => hub.characterClassManager.Scp079.iAm079);
        }

        internal static void PrepareOvercharge(float duration) {
            DelayDuration = duration;
            DelayStopwatch.Restart();
            var recontainer = Object.FindObjectOfType<Recontainer079>();
            recontainer.Set("_activationDelay", -1);
            recontainer.Get<Stopwatch>("_delayStopwatch").Stop();
            recontainer.Get<Stopwatch>("_unlockStopwatch").Stop();
            var glass = recontainer.Get<BreakableWindow>("_activatorGlass");
            glass.Call("ServerDamageWindow", glass.health);
        }

        public static bool CheckRecontainer(Func<Recontainer079, bool> func) {
            var o = Object.FindObjectOfType<Recontainer079>();
            return o != null && func(o);
        }

        private static IEnumerator<float> Update() {
            try {
                if (Cfg.InfinitePower && DelayStopwatch.IsRunning)
                    foreach (var script in ReferenceHub.GetAllHubs().Values
                        .Where(hub =>
                            hub != ReferenceHub.HostHub && hub != ReferenceHub.LocalHub &&
                            hub.characterClassManager.Scp079.iAm079)
                        .Select(hub => hub.characterClassManager.Scp079))
                        script.Network_curMana = Math.Min(script.levels[script.Network_curLvl].maxMana,
                            script.NetworkmaxMana + Cfg.PowerPerSecond);
            } catch (Exception) {
                // ignored
            }

            yield return Timing.WaitForSeconds(1);
        }
    }
}