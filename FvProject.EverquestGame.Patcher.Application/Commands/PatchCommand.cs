using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;

namespace FvProject.EverquestGame.Patcher.Application.Commands {
    public class PatchCommand : ICommand{
        public PatchCommand(ClientPatch patchList, IProgressReporter progressReporter, bool enforceMD5Checksum) {
            PatchList = patchList;
            ProgressReporter = progressReporter;
            EnforceMD5Checksum = enforceMD5Checksum;
        }

        public ClientPatch PatchList { get; }
        public IProgressReporter ProgressReporter { get; }
        public bool EnforceMD5Checksum { get; }
    }
}
