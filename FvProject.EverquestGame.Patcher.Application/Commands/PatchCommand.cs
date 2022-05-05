using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;

namespace FvProject.EverquestGame.Patcher.Application.Commands {
    public class PatchCommand : ICommand{
        public PatchCommand(ClientPatch patchList, IProgressReporter progressReporter) {
            PatchList = patchList;
            ProgressReporter = progressReporter;
        }

        public ClientPatch PatchList { get; }
        public IProgressReporter ProgressReporter { get; }
    }
}
