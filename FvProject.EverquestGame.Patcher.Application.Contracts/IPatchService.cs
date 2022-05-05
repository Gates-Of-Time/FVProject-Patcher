using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories;

namespace FvProject.EverquestGame.Patcher.Application.Contracts {
    public interface IPatchService {
        Task ExecuteAsync(IDeleteRepository<PatchFileInfo> deleteRepository, ClientPatch patchList, IProgressReporter progressReporter);
    }
}
