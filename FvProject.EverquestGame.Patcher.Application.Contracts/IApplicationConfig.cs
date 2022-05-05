using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Application.Contracts {
    public interface IApplicationConfig {
        IEnumerable<ExpansionsEnum> SupportedExpansions { get; }
        string GameDirectory { get; set; }
        ExpansionsEnum PreferredExpansion { get; set; }
    }
}
