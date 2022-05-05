using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Application.Queries {
    public class ClientPatchListQuery : IQuery {
        public ClientPatchListQuery(ExpansionsEnum expansion, PatchManifest serverFiles) {
            Expansion = expansion;
            ServerFiles = serverFiles;
        }

        public ExpansionsEnum Expansion { get; }
        public PatchManifest ServerFiles { get; }
    }
}
