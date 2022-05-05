namespace FvProject.EverquestGame.Patcher.Domain {
    public class PatchFile {
        public PatchFile(Stream downloadStream, PatchFileInfo fileEntry, IProgress<long> progressReporter) {
            DownloadStream = downloadStream ?? throw new ArgumentNullException(nameof(downloadStream));
            FileEntry = fileEntry ?? throw new ArgumentNullException(nameof(fileEntry));
            ProgressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
        }

        public Stream DownloadStream { get; }
        public PatchFileInfo FileEntry { get; }
        public IProgress<long> ProgressReporter { get; }
    }
}
