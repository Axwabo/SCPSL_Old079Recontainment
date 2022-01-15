using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Old079Recontainment {
    public class Config079 : IConfig {
        public bool IsEnabled { get; set; } = true;

        [Description(
            "The extra generators to be added. Before Parabellum, the map had 5 generators but there's only 3 now by default.")]
        public int ExtraGenerators { get; set; } = 2;

        [Description(
            "If SCP-079 should be recontained after all generators are engaged, without someone having to press the button.")]
        public bool AutoRecontain { get; set; } = true;
    }
}