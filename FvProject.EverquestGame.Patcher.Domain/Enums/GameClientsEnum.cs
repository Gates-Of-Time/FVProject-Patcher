using Ardalis.SmartEnum;

namespace FvProject.EverquestGame.Patcher.Domain.Enums {
    public abstract class GameClientsEnum : SmartEnum<GameClientsEnum> {
        public static readonly GameClientsEnum Unknown = new UnknownType(nameof(Unknown), 0, "", "Unknown");
        public static readonly GameClientsEnum SecretsOfFeydwer = new SecretsOfFeydwerType(nameof(SecretsOfFeydwer), 1, "sof", "Secrets Of Feydwer");
        public static readonly GameClientsEnum Underfoot = new UnderfootType(nameof(Underfoot), 2, "underfoot", "Underfoot");
        public static readonly GameClientsEnum Titanium = new TitaniumType(nameof(Titanium), 3, "titanium", "Titanium");
        public static readonly GameClientsEnum ReignOfFear = new ReignOfFearType(nameof(ReignOfFear), 4, "rof", "Reign Of Fear");
        public static readonly GameClientsEnum ReignOfFear2 = new ReignOfFear2Type(nameof(ReignOfFear2), 5, "rof", "Reign Of Fear 2");
        public static readonly GameClientsEnum BrokenMirror = new BrokenMirrorType(nameof(BrokenMirror), 6, "brokenmirror", "Broken Mirror");

        private GameClientsEnum(string name, int value, string shortName, string displayName) : base(name, value)
        {
            ShortName = shortName;
            DisplayName = displayName;
        }

        public string ShortName { get; }
        public string DisplayName { get; }

        public abstract bool IsValidFor(string md5hash);

        private class UnknownType : GameClientsEnum {
            public UnknownType(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                return !SecretsOfFeydwer.IsValidFor(md5hash)
                    && !Underfoot.IsValidFor(md5hash)
                    && !Titanium.IsValidFor(md5hash)
                    && !ReignOfFear.IsValidFor(md5hash)
                    && !ReignOfFear2.IsValidFor(md5hash)
                    && !BrokenMirror.IsValidFor(md5hash);
            }
        }

        private class SecretsOfFeydwerType : GameClientsEnum {
            public SecretsOfFeydwerType(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                return md5hash.ToUpperInvariant() == "85218FC053D8B367F2B704BAC5E30ACC";
            }
        }

        private class UnderfootType : GameClientsEnum {
            public UnderfootType(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                var validHashes = new[] {
                    "859E89987AA636D36B1007F11C2CD6E0",
                    "EF07EE6649C9A2BA2EFFC3F346388E1E78B44B48" // one of the torrented uf clients, used by B&R too
                };
                return validHashes.Contains(md5hash.ToUpperInvariant());
            }
        }

        private class TitaniumType : GameClientsEnum {
            public TitaniumType(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                var validHashes = new[] {
                    "A9DE1B8CC5C451B32084656FCACF1103", // p99 client
                    "BB42BC3870F59B6424A56FED3289C6D4"  // vanilla titanium
                };
                return validHashes.Contains(md5hash.ToUpperInvariant());
            }
        }

        private class ReignOfFearType : GameClientsEnum {
            public ReignOfFearType(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                return md5hash.ToUpperInvariant() == "368BB9F425C8A55030A63E606D184445";
            }
        }

        private class ReignOfFear2Type : GameClientsEnum {
            public ReignOfFear2Type(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                var validHashes = new[] {
                    "240C80800112ADA825C146D7349CE85B",
                    "A057A23F030BAA1C4910323B131407105ACAD14D" // This is a custom ROF2 from a torrent download
                };
                return validHashes.Contains(md5hash.ToUpperInvariant());
            }
        }

        private class BrokenMirrorType : GameClientsEnum {
            public BrokenMirrorType(string name, int value, string shortName, string displayName) : base(name, value, shortName, displayName) {
            }

            public override bool IsValidFor(string md5hash) {
                var validHashes = new[] {
                    "6BFAE252C1A64FE8A3E176CAEE7AAE60", // This is one of the live EQ binaries.
                    "AD970AD6DB97E5BB21141C205CAD6E68"  // 2016/08/27
                };
                return validHashes.Contains(md5hash.ToUpperInvariant());
            }
        }
    }
}
