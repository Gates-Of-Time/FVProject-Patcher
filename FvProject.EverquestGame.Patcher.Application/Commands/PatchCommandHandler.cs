using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories;

namespace FvProject.EverquestGame.Patcher.Application.Commands {
    public class PatchCommandHandler : ICommandHandler<PatchCommand> {
        public PatchCommandHandler(IDeleteRepository<PatchFileInfo, Result> deleteRepository, IGetRepository<string, Result<Stream>> getServerFileRepository, IUpsertRepository<PatchFile, Result> upsertRepository) {
            ClientFilesDeleter = deleteRepository;
            FileServer = getServerFileRepository;
            UpsertRepository = upsertRepository;
        }

        private IDeleteRepository<PatchFileInfo, Result> ClientFilesDeleter { get; }
        private IGetRepository<string, Result<Stream>> FileServer { get; }
        private IUpsertRepository<PatchFile, Result> UpsertRepository { get; }

        public async Task Execute(PatchCommand command, CancellationToken cancellationToken = default) {
            var progressReporter = command.ProgressReporter;

            progressReporter.Report($"Patching from {command.PatchList.Downloadprefix}...");
            foreach (var deleteFile in command.PatchList.Deletes) {
                if (cancellationToken.IsCancellationRequested) {
                    return;
                }

                progressReporter.Report($"Deleting {deleteFile.name}...");
                var deleteResult = await ClientFilesDeleter.Delete(deleteFile);
                if (deleteResult.IsFailure) {
                    progressReporter.Report(deleteResult.Error);
                }
            }

            var totalDownloadSize = command.PatchList.Downloads.Sum(x => x.size < 1 ? 1 : x.size);
            var curBytes = 0L;
            progressReporter.Report($"Downloading {totalDownloadSize} bytes for {command.PatchList.Downloads.Count()} file(s)...");
            foreach (var downloadFile in command.PatchList.Downloads) {
                if (cancellationToken.IsCancellationRequested) {
                    return;
                }

                var url = command.PatchList.Downloadprefix + downloadFile.name.Replace("\\", "/");
                var downloadResult = await FileServer.Get(url, cancellationToken);
                if (downloadResult.IsFailure) {
                    curBytes += downloadFile.size < 1 ? 1 : downloadFile.size;
                    progressReporter.Report($"Failed to download <{downloadFile.name}> due to {downloadResult.Error}");
                }
                else if (downloadFile.md5 != MD5Hasher.CreateHash(downloadResult.Value)) {
                    curBytes += downloadFile.size < 1 ? 1 : downloadFile.size;
                    progressReporter.Report($"Incorrect MD5 checksum on downloaded file... {downloadFile.name}");
                }
                else {
                    downloadResult.Value.Position = 0;
                    var fileBytes = 0L;
                    var relativeProgress = new Progress<long>(totalBytes => {
                        if ((totalBytes - fileBytes) * 100 / totalDownloadSize < 1) {
                            return;
                        }

                        curBytes += totalBytes - fileBytes;
                        fileBytes = totalBytes;
                        var progress = curBytes * 100 / totalDownloadSize;
                        progressReporter.Progress.Report(progress);
                    });
                    using var downloadStream = downloadResult.Value;
                    var result = await UpsertRepository.Upsert(new PatchFile(downloadStream, downloadFile, relativeProgress), cancellationToken);
                    if (result.IsSuccess) {
                        curBytes += downloadFile.size - fileBytes;
                        progressReporter.Progress.Report(curBytes * 100 / totalDownloadSize);
                        progressReporter.Report($"{downloadFile.name}...");
                    }
                    else {
                        curBytes += downloadFile.size < 1 ? 1 : downloadFile.size;
                        progressReporter.Report($"Failed writing {downloadFile.name}...");
                        progressReporter.Report($"\t {result.Error}");
                    }
                }

                var progress = curBytes * 100 / totalDownloadSize;
                progressReporter.Progress.Report(progress);
            }
        }
    }
}
