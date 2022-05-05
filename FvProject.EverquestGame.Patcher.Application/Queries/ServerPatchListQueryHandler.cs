using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories;

namespace FvProject.EverquestGame.Patcher.Application.Queries {
    public class ServerPatchListQueryHandler : IQueryHandler<ServerPatchListQuery, Result<PatchManifest>>
    {
        public ServerPatchListQueryHandler(IGetRepository<ServerPatchListQuery, Result<PatchManifest>> fileListRepository)
        {
            FileListRepository = fileListRepository;
        }

        private IGetRepository<ServerPatchListQuery, Result<PatchManifest>> FileListRepository { get; }

        public async Task<Result<PatchManifest>> ExecuteAsync(ServerPatchListQuery query)
        {
            return await FileListRepository.Get(query);
        }
    }
}
