using System.Linq;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain.Contracts;
using FvProject.EverquestGame.Patcher.Domain.Enums;
using static FvProject.EverquestGame.Patcher.Presentation.Client.AppSettings;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Converters {
    public class ApplicationConfigConverter : IConverter<(IApplicationConfig applicationData, ExpansionsEnum selectedExpansion), AppSettings> {
        public AppSettings Convert((IApplicationConfig applicationData, ExpansionsEnum selectedExpansion) settingsData) {
            var applicationData = settingsData.applicationData;
            var appSettings = new AppSettings() {
                GameDirectory = applicationData.GameDirectory,
                PreferredExpansion = Convert(settingsData.selectedExpansion),
                AvailableExpansions = applicationData.SupportedExpansions.Select(Convert).Where(x => x != null),
                EnforceMD5Checksum = applicationData.EnforceMD5Checksum,
            };

            return appSettings;
        }

        private ExpansionEnum? Convert(ExpansionsEnum expansion) {
            switch (expansion?.Name) {
                case nameof(ExpansionsEnum.Original):
                    return ExpansionEnum.Original;
                case nameof(ExpansionsEnum.RuinsOfKunark):
                    return ExpansionEnum.Kunark;
                case nameof(ExpansionsEnum.ScarsOfVelious):
                    return ExpansionEnum.Velious;
                case nameof(ExpansionsEnum.ShadowsOfLuclin):
                    return ExpansionEnum.Luclin;
                case nameof(ExpansionsEnum.PlanesOfPower):
                    return ExpansionEnum.PoP;
                case nameof(ExpansionsEnum.LegacyOfYekasha):
                    return ExpansionEnum.Ykesha;
                default:
                    return null;
            }
        }
    }
}
