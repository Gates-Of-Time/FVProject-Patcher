using Ardalis.SmartEnum;

namespace FvProject.EverquestGame.Patcher.Domain.Enums {
    public sealed class ExpansionsEnum : SmartEnum<ExpansionsEnum> {
        public static readonly ExpansionsEnum Unknown = new ExpansionsEnum(nameof(Unknown), 99, "unknown", "Unknown");
        public static readonly ExpansionsEnum Original = new ExpansionsEnum(nameof(Original), 0, "original", "Original");
        public static readonly ExpansionsEnum RuinsOfKunark = new ExpansionsEnum(nameof(RuinsOfKunark), 1, "kunark", "Ruins Of Kunark");
        public static readonly ExpansionsEnum ScarsOfVelious = new ExpansionsEnum(nameof(ScarsOfVelious), 2, "velious", "Scars Of Velious");
        public static readonly ExpansionsEnum ShadowsOfLuclin = new ExpansionsEnum(nameof(ShadowsOfLuclin), 3, "luclin", "Shadows Of Luclin");
        public static readonly ExpansionsEnum PlanesOfPower = new ExpansionsEnum(nameof(PlanesOfPower), 4, "pop", "Planes Of Power");
        public static readonly ExpansionsEnum LegacyOfYekasha = new ExpansionsEnum(nameof(LegacyOfYekasha), 5, "loy", "Legacy Of Yekasha");

        private ExpansionsEnum(string name, int value, string shortName, string longName) : base(name, value) {
            ShortName = shortName;
            LongName = longName;
        }

        public string ShortName { get; }
        public string LongName { get; }
    }
}
