using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using MapEvents = Exiled.Events.Handlers.Map;

namespace Old079Recontainment {
    public class Plugin079 : Plugin<Config079> {
        public static Plugin079 Singleton { get; private set; }
        internal static Config079 Cfg => Singleton.Config;
        private Harmony _harmony;
        internal static readonly Stopwatch OneMinuteStopwatch = new Stopwatch();

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

            MapEvents.Generated += EventHandlers.MapGenerated;
            Log.Info("Old SCP-079 recontainment enabled.");
        }

        public override void OnDisabled() {
            base.OnDisabled();
            MapEvents.Generated -= EventHandlers.MapGenerated;
            _harmony.UnpatchAll();
        }

        public override string Name { get; } = "Old079Recontainment";
        public override string Author { get; } = "Axwabo";
        public override PluginPriority Priority { get; } = PluginPriority.Highest;
        public override Version Version { get; } = new Version(1, 0, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(4, 2);

        public static string CassieGlitchJam(string message, float glitchChance, float jamChance,
            bool skipSilence = true) {
            var strArray = message.Split(' ');
            var newWords = new List<string>();
            newWords.EnsureCapacity(strArray.Length);
            for (var index = 0; index < strArray.Length; ++index) {
                var s = strArray[index];
                newWords.Add(s);
                if (index >= strArray.Length - 1 || s.Equals(".") && skipSilence)
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
    }
}