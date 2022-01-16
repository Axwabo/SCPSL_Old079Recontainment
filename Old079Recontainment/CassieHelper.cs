using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Axwabo.Util;

namespace Old079Recontainment {
    public static class CassieHelper {
        private static readonly Regex SilenceRegex = new Regex("([^A-z0-9._ ])");

        public static string AddGlitchesAndJams(string message, float glitchChance, float jamChance,
            bool skipSilence = true) {
            var strArray = message.Split(' ');
            var newWords = new List<string>();
            newWords.EnsureCapacity(strArray.Length);
            for (var index = 0; index < strArray.Length; ++index) {
                var s = strArray[index];
                newWords.Add(s);
                if (index >= strArray.Length - 2 || SilenceRegex.Replace(s, "").Equals(".") && skipSilence)
                    continue;
                if (UnityEngine.Random.value < (double) glitchChance)
                    newWords.Add(".G" + UnityEngine.Random.Range(1, 7));
                if (UnityEngine.Random.value < (double) jamChance)
                    newWords.Add("JAM_" + UnityEngine.Random.Range(0, 70).ToString("000") + "_" +
                                 UnityEngine.Random.Range(2, 6));
            }

            return newWords.Aggregate("", (current, newWord) => current + newWord + " ");
        }

        private static readonly Regex NoiseRegex = new Regex("(jam_[0-9][0-9][0-9]_[0-9]|.g[0-9])( ?)");

        public static string RemoveNoise(string s) {
            return NoiseRegex.Replace(s.ToLower(), "");
        }

        // replace EXILED method to make sure the singleton is not null
        public static float CalculateDuration(string message) {
            var cassie = NineTailedFoxAnnouncer.singleton;
            if (cassie == null)
                return 0;
            return cassie.Call<float>("CalculateDuration", message, false);
        }

        // replace EXILED method to make sure the singleton is not null
        public static void Message(string message, float glitchChance = 0f, float jamChance = 0f) {
            var cassie = NineTailedFoxAnnouncer.singleton;
            if (cassie == null)
                return;
            cassie.ServerOnlyAddGlitchyPhrase(message, glitchChance, jamChance);
        }
    }
}