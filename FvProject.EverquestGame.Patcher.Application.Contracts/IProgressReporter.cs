namespace FvProject.EverquestGame.Patcher.Application.Contracts {
    public interface IProgressReporter {
        void Report(string message);
        IProgress<double> Progress { get; }
    }
}
