using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Application.Contracts {
    public interface IExternalApplicationService {
        Result<GameClientsEnum> CanExecute { get; }
        Result Start();
    }
}
