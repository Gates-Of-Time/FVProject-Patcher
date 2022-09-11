using System;
using System.Collections.Generic;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Presentation.Client {
    public class ApplicationConfig : IApplicationConfig {
        public ApplicationConfig(string gameDirectory = null, IEnumerable<ExpansionsEnum> supportedExpansions = null, bool enforceMD5Checksum = false) {
            GameDirectory = gameDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            SupportedExpansions = supportedExpansions ?? Array.Empty<ExpansionsEnum>();
            EnforceMD5Checksum = enforceMD5Checksum;
        }
        public string GameDirectory { get; set; }
        public IEnumerable<ExpansionsEnum> SupportedExpansions { get; }
        public ExpansionsEnum PreferredExpansion { get; set; }
        public bool EnforceMD5Checksum { get; set; }
    }
}
