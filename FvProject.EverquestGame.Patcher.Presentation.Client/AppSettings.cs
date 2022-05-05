using System.Collections.Generic;

namespace FvProject.EverquestGame.Patcher.Presentation.Client {
    public class AppSettings {
        public string GameDirectory { get; set; }
        public ExpansionEnum? PreferredExpansion { get; set; }
        public IEnumerable<ExpansionEnum?> AvailableExpansions { get; set; }

        public enum ExpansionEnum {
            Original,
            Kunark,
            Velious,
            Luclin,
            PoP,
            Ykesha
        }
    }
}
