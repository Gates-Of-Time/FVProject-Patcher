using System.CommandLine.Invocation;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Presentation.ConsolePatcher.Commands {
    public class OriginalPatchCommandHandler : BasePatchCommandHandler {
        public OriginalPatchCommandHandler() : base(ExpansionsEnum.Original) {
        }
    }
}
