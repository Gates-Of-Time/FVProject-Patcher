using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Application.Extensions;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories;

namespace FvProject.EverquestGame.Patcher.Application.Queries {
    public class ClientPatchListQueryHandler : IQueryHandler<ClientPatchListQuery, ClientPatch>
    {
        public ClientPatchListQueryHandler(IGetAllRepository<IEnumerable<ClientFileInfo>> getClientFilesRepository, IGetRepositorySync<ClientFileInfo, Stream> getFileStreamRepository)
        {
            GetClientFilesRepository = getClientFilesRepository;
            GetFileStreamRepository = getFileStreamRepository;
        }

        private IGetAllRepository<IEnumerable<ClientFileInfo>> GetClientFilesRepository { get; }
        public IGetRepositorySync<ClientFileInfo, Stream> GetFileStreamRepository { get; }

        public async Task<ClientPatch> ExecuteAsync(ClientPatchListQuery query)
        {
            var clientFiles = await GetClientFilesRepository.GetAll();
            var clientFileNames = clientFiles.Select(x => x.Name).ToArray();
            var deleteFiles = query.ServerFiles.deletes.Where(serverFile => clientFileNames.Contains(serverFile.name)).ToArray();
            var missingFiles = query.ServerFiles.downloads.Where(ShouldUpdateFile(clientFiles)).ToArray();
            return new ClientPatch() { Expansion = query.Expansion, Downloads = missingFiles, Deletes = deleteFiles, Downloadprefix = query.ServerFiles.downloadprefix };
        }

        Func<PatchFileInfo, bool> ShouldUpdateFile(IEnumerable<ClientFileInfo> clientFiles) {
            bool isFileUpdated(PatchFileInfo serverFile) => clientFiles.Any(clientFile => clientFile.HasSameName(serverFile) && HasDifferentMd5(clientFile, serverFile));
            bool isFileMissing(PatchFileInfo serverFile) => clientFiles.None(clientFile => clientFile.HasSameName(serverFile));
            return (PatchFileInfo serverFile) => isFileMissing(serverFile) || isFileUpdated(serverFile);
        }

        private bool HasDifferentMd5(ClientFileInfo clientFileInfo, PatchFileInfo patchFile) {
            using var fileStream = GetFileStreamRepository.Get(clientFileInfo);
            var clientMd5 = MD5Hasher.CreateHash(fileStream);
            return string.Equals(clientMd5, patchFile.md5, StringComparison.OrdinalIgnoreCase) == false;
        }
    }
}
