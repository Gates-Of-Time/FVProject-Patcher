using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using FvProject.EverquestGame.Patcher.Application.Commands;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Infrastructure;
using Stylet;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Pages {
    public class PatchViewModel : Screen, IProgressReporter {
        private static readonly Color DefaultProgressColor = Color.FromRgb(6, 176, 37);
        private readonly IApplicationConfig _applicationConfig;
        private readonly HttpClient _httpClient;
        private readonly StringBuilder _stringBuilder;
        private readonly ClientFilesRepository _clientFileRepository;
        private readonly PatchServerRepository _patchServerRepository;

        public PatchViewModel(IApplicationConfig applicationConfig, HttpClient httpClient) {
            _applicationConfig = applicationConfig;
            _httpClient = httpClient;
            _stringBuilder = new StringBuilder();
            _clientFileRepository = new ClientFilesRepository(_applicationConfig);
            _patchServerRepository = new PatchServerRepository(_httpClient);
            Progress = new Progress<double>(progress => {
                if (_progressValue < 100) {
                    ProgressValue = progress;
                }
            });
        }

        public bool HasFailed { get; set; }

        private double _progressValue = 0;
        public double ProgressValue {
            get => _progressValue;
            set => SetAndNotify(ref _progressValue, value);
        }

        private Brush _progressColor = new SolidColorBrush(DefaultProgressColor);
        public Brush ProgressColor {
            get => _progressColor;
            set => SetAndNotify(ref _progressColor, value);
        }

        private string _patchLog = "";
        public string PatchLog {
            get => _patchLog;
            set => SetAndNotify(ref _patchLog, value);
        }

        public async Task PatchClient(ClientPatch clientPatchFileList, CancellationToken cancellationToken) {
            HasFailed = false;
            ProgressColor = new SolidColorBrush(DefaultProgressColor);
            PatchLog = "";
            ProgressValue = 0;
            _stringBuilder.Clear();

            var command = new PatchCommand(clientPatchFileList, this, _applicationConfig.EnforceMD5Checksum);
            var commandHandler = new PatchCommandHandler(_clientFileRepository, _patchServerRepository, _clientFileRepository);
            await commandHandler.Execute(command, cancellationToken);
        }

        public void CancelPatching() {
            FailedPatch();
            ProgressValue = 100;
        }

        #region IProgressReporter

        public void FailedPatch() {
            HasFailed = true;
            ProgressColor = new SolidColorBrush(Colors.DarkRed);
        }

        public void Report(string message) {
            _stringBuilder.AppendLine(message);
            PatchLog = _stringBuilder.ToString();
        }

        public IProgress<double> Progress { get; }

        #endregion IProgressReporter
    }

    public class PatchDesignViewModel : PatchViewModel {
        public PatchDesignViewModel() : base(new ApplicationConfig(), null) {
        }
    }
}
