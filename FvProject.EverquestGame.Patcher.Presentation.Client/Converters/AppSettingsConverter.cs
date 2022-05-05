using System;
using System.Linq;
using FvProject.EverquestGame.Patcher.Domain.Enums;
using FvProject.EverquestGame.Patcher.Domain.Contracts;
using static FvProject.EverquestGame.Patcher.Presentation.Client.AppSettings;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Converters {
    public class AppSettingsConverter : IConverter<AppSettings, ApplicationConfig> {
        public ApplicationConfig Convert(AppSettings appSettings) {
            var supportedExpansions = appSettings.AvailableExpansions?.Select(Convert).Where(x => x != null) ?? Array.Empty<ExpansionsEnum>();
            var applicationData = new ApplicationConfig(appSettings.GameDirectory, supportedExpansions) {
                PreferredExpansion = Convert(appSettings.PreferredExpansion)
            };
            return applicationData;
        }

        private ExpansionsEnum Convert(ExpansionEnum? expansion) {
            switch (expansion) {
                case ExpansionEnum.Original:
                    return ExpansionsEnum.Original;
                case ExpansionEnum.Kunark:
                    return ExpansionsEnum.RuinsOfKunark;
                case ExpansionEnum.Velious:
                    return ExpansionsEnum.ScarsOfVelious;
                case ExpansionEnum.Luclin:
                    return ExpansionsEnum.ShadowsOfLuclin;
                case ExpansionEnum.PoP:
                    return ExpansionsEnum.PlanesOfPower;
                case ExpansionEnum.Ykesha:
                    return ExpansionsEnum.LegacyOfYekasha;
                default:
                    return null;
            }
        }
    }
}
