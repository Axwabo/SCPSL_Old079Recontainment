using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using MapEvents = Exiled.Events.Handlers.Map;

namespace Old079Recontainment {
    public class Plugin079 : Plugin<Config079> {
        public static Plugin079 Singleton { get; private set; }
        internal static Config079 Cfg => Singleton.Config;
        private Harmony _harmony;

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
        }

        public override void OnDisabled() {
            base.OnDisabled();
            MapEvents.Generated -= EventHandlers.MapGenerated;
            _harmony.UnpatchAll();
        }

        public override void OnReloaded() {
            base.OnReloaded();
        }

        public override string Name { get; } = "Old079Recontainment";
        public override string Author { get; } = "Axwabo";
        public override PluginPriority Priority { get; } = PluginPriority.Highest;
        public override Version Version { get; } = new Version(1, 0, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(4, 2);

        public static string CassieGlitchJam(string message, float glitchChance, float jamChance) {
            var strArray = message.Split(' ');
            var newWords = new List<string>();
            newWords.EnsureCapacity(strArray.Length);
            for (var index = 0; index < strArray.Length; ++index) {
                newWords.Add(strArray[index]);
                if (index >= strArray.Length - 1) continue;
                if (UnityEngine.Random.value < (double) glitchChance)
                    newWords.Add(".G" + UnityEngine.Random.Range(1, 7));
                if (UnityEngine.Random.value < (double) jamChance)
                    newWords.Add("JAM_" + UnityEngine.Random.Range(0, 70).ToString("000") + "_" +
                                 UnityEngine.Random.Range(2, 6));
            }

            return newWords.Aggregate("", (current, newWord) => current + newWord + " ");
        }
    }
}