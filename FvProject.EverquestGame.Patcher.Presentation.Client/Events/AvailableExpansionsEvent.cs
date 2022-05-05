using System.Collections.Generic;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Events {
    public class AvailableExpansionsEvent {
        public AvailableExpansionsEvent(IDictionary<ExpansionsEnum, PatchManifest> expansionsFiles) {
            ExpansionsFiles = expansionsFiles;
        }

        public IDictionary<ExpansionsEnum, PatchManifest> ExpansionsFiles { get; }
    }
}
