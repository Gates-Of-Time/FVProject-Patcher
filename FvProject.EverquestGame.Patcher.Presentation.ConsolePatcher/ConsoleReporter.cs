using FvProject.EverquestGame.Patcher.Application.Contracts;

namespace FvProject.EverquestGame.Patcher.Presentation.ConsolePatcher {
    public class ConsoleReporter : IProgressReporter {
        public ConsoleReporter() {
            Progress = new Progress<double>(progress => {
                if (_progressValue < 100) {
                    ProgressValue = progress;
                }
            });
        }

        public IProgress<double> Progress { get; }

        private double _progressValue = 0;
        public double ProgressValue {
            get => _progressValue;
            set => _progressValue =  value;
        }

        public void FailedPatch() {
            Console.WriteLine("Failed patching...");
        }

        public void Report(string message) {
            Console.WriteLine(message);
        }
    }
}
