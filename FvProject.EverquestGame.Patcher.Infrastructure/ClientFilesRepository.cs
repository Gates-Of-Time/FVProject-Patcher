using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Application.Extensions;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories;

namespace FvProject.EverquestGame.Patcher.Infrastructure {
    public class ClientFilesRepository : IGetAllRepository<IEnumerable<ClientFileInfo>>, IDeleteRepository<PatchFileInfo, Result>, IUpsertRepository<PatchFile, Result>, IGetRepositorySync<ClientFileInfo, Stream> {
        public ClientFilesRepository(IApplicationConfig applicationConfig) {
            ApplicationConfig = applicationConfig;
        }

        private IApplicationConfig ApplicationConfig { get; }

        public async Task<Result> Delete(PatchFileInfo fileEntry) {
            var filePath = fileEntry.name.Replace("/", @"\");
            filePath = $@"{ApplicationConfig.GameDirectory}\{filePath}";
            return await Task.Run(() => {
                if (File.Exists(filePath)) {
                    try {
                        File.Delete(filePath);
                    }
                    catch {
                        var failureMessage = $"Unable to delete file: {fileEntry.name}";
                        // https://blog.elmah.io/debugging-system-unauthorizedaccessexception/
                        var attributes = File.GetAttributes(filePath);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                            //attributes &= ~FileAttributes.ReadOnly;
                            //File.SetAttributes(filePath, attributes);
                            //File.Delete(filePath);
                            failureMessage += " - file is marked as 'Read Only'";
                        }
                        else {
                            failureMessage += " - file might be in use.";
                        }


                        return Result.Failure(failureMessage);
                    }
                }

                return Result.Success();
            });
        }

        public async Task<Result> Upsert(PatchFile upsert, CancellationToken cancellationToken = default) {
            try {
                var filePath = upsert.FileEntry.name.Replace("/", @"\");
                EnsureDirectory(filePath);
                filePath = $@"{ApplicationConfig.GameDirectory}\{filePath}";
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await upsert.DownloadStream.CopyToAsync(fileStream, 81920, upsert.ProgressReporter, cancellationToken);
            }
            catch (Exception ex) {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        private void EnsureDirectory(string filePath) {
            if (filePath.Contains('\\')) { //Make directory if needed.
                var dir = $@"{ApplicationConfig.GameDirectory}\{filePath.Substring(0, filePath.LastIndexOf(@"\"))}";
                Directory.CreateDirectory(dir);
            }
        }

        public async Task<IEnumerable<ClientFileInfo>> GetAll() {
            return await Task.Run(() =>
                Directory.GetFiles(ApplicationConfig.GameDirectory, "*.*", SearchOption.AllDirectories)
                         .Select(x => new ClientFileInfo(x.Replace(ApplicationConfig.GameDirectory, "", StringComparison.InvariantCultureIgnoreCase).TrimStart('\\'), x))
            );
        }

        public Stream Get(ClientFileInfo fileEntry) {
            return File.OpenRead(fileEntry.FullPath);
        }
    }
}
