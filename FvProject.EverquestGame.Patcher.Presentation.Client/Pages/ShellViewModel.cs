using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Application.Queries;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Enums;
using FvProject.EverquestGame.Patcher.Infrastructure;
using FvProject.EverquestGame.Patcher.Presentation.Client.Events;
using Stylet;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Pages {
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<ExpansionSelectedEvent>, IHandle<GameDirectoryChangedEvent> {
        private const string AppName = "Firione Vie Project Patcher";

        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationConfig _applicationConfig;
        private readonly IExternalApplicationService _eqGameApplicationService;
        private readonly HttpClient _httpClient;
        private readonly ExpansionSelectorViewModel _expansionSelectorViewModel;
        private readonly PatchViewModel _patchViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public ShellViewModel(IEventAggregator eventAggregator, IApplicationConfig applicationConfig) {
            _eventAggregator = eventAggregator;
            _applicationConfig = applicationConfig;
            _eqGameApplicationService = new EqGameApplicationService(applicationConfig);
            _httpClient = new HttpClient();

            _eventAggregator?.Subscribe(this);

            _expansionSelectorViewModel = new ExpansionSelectorViewModel(eventAggregator, _applicationConfig);
            _patchViewModel = new PatchViewModel(_applicationConfig, _httpClient);
            _settingsViewModel = new SettingsViewModel(eventAggregator, _applicationConfig);
            Items.Add(_expansionSelectorViewModel);
            Items.Add(_patchViewModel);
            Items.Add(_settingsViewModel);

            ActiveItem = _expansionSelectorViewModel;

            ClientFilesRepository = new ClientFilesRepository(applicationConfig);
            StatusBarViewModel = new StatusBarViewModel(eventAggregator);
        }

        private ConcurrentDictionary<ExpansionsEnum, PatchManifest> ServerFiles { get; } = new ConcurrentDictionary<ExpansionsEnum, PatchManifest>();
        private ClientFilesRepository ClientFilesRepository { get; }
        private ClientPatch CurrentClientPatchFileList { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        public StatusBarViewModel StatusBarViewModel { get; }

        private ExpansionsEnum _selectedExpansion;
        public ExpansionsEnum SelectedExpansion {
            get => _selectedExpansion;
            set {
                _selectedExpansion = value;
                NotifyOfPropertyChange(nameof(Title));
                if (ActiveItem != _expansionSelectorViewModel) {
                    return;
                }
                var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Factory.StartNew(async () => {
                    await CheckExpansionPatch(value);
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        }

        public string Title {
            get {
                var title = $"{AppName}";
                if (CurrentClient != GameClientsEnum.Unknown) {
                    title = $"{title} - {CurrentClient.DisplayName}";
                }

                if (SelectedExpansion != ExpansionsEnum.Unknown) {
                    title = $"{title} - {SelectedExpansion}";
                }

                return title;
            }
        }

        private GameClientsEnum _currentClient = GameClientsEnum.Unknown;
        public GameClientsEnum CurrentClient {
            get => _currentClient;
            set => SetAndNotify(ref _currentClient, value);
        }

        public bool CanPatchClient {
            get => CurrentClientPatchFileList?.HasChanges ?? false;
        }

        private bool _canLaunchClient = false;
        public bool CanLaunchClient {
            get => _canLaunchClient;
            set => SetAndNotify(ref _canLaunchClient, value);
        }

        private bool _canOpenSettings = true;
        public bool CanOpenSettings {
            get => _canOpenSettings;
            set => SetAndNotify(ref _canOpenSettings, value);
        }

        public bool IsPatchButtonVisisble {
            get => ActiveItem == _expansionSelectorViewModel && CancellationTokenSource == null;
        }

        public bool IsCancelButtonVisisble {
            get => ActiveItem == _expansionSelectorViewModel && CancellationTokenSource != null;
        }

        public bool IsExpansionSelectButtonVisisble {
            get => !IsPatchButtonVisisble && !IsCancelButtonVisisble;
        }

        private bool IsAppBusy {
            set {
                CanLaunchClient = !value;
                CanOpenSettings = !value;
                _expansionSelectorViewModel.CanLeft = !value;
                _expansionSelectorViewModel.CanRight = !value;
                NotifyOfPropertyChange(nameof(CanPatchClient));
                NotifyOfPropertyChange(nameof(IsPatchButtonVisisble));
                NotifyOfPropertyChange(nameof(IsCancelButtonVisisble));
                NotifyOfPropertyChange(nameof(IsExpansionSelectButtonVisisble));
            }
        }

        protected override void OnInitialActivate() {
            base.OnInitialActivate();
            CheckClient();
            LoadPatchFiles();
        }

        public async Task ExpansionSelect() {
            ActiveItem = _expansionSelectorViewModel;
            if (CurrentClientPatchFileList == null) {
                await CheckExpansionPatch(SelectedExpansion);
            }

            NotifyOfPropertyChange(nameof(CanPatchClient));
            NotifyOfPropertyChange(nameof(IsPatchButtonVisisble));
            NotifyOfPropertyChange(nameof(IsCancelButtonVisisble));
            NotifyOfPropertyChange(nameof(IsExpansionSelectButtonVisisble));
        }

        public async Task PatchClient() {
            if (CurrentClientPatchFileList == null) {
                PublishApplicationStateChangedEvent("No patch available.", Colors.DeepSkyBlue);
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();

            IsAppBusy = true;
            PublishApplicationStateChangedEvent($"Patching <{_applicationConfig.GameDirectory}>...", Colors.Gold);

            ActiveItem = _patchViewModel;
            await _patchViewModel.PatchClient(CurrentClientPatchFileList, CancellationTokenSource.Token);

            if (CancellationTokenSource?.IsCancellationRequested == false && !_patchViewModel.HasFailed) {
                PublishApplicationStateChangedEvent($"Patched <{_applicationConfig.GameDirectory}> and ready to launch game.", Colors.Green);
            }
            else {
                PublishApplicationStateChangedEvent($"Patching failed, check the patch log for errors.", Colors.Red);
            }

            CancellationTokenSource = null;
            IsAppBusy = false;
        }

        public void CancelPatch() {
            CancellationTokenSource.Cancel();
            _patchViewModel.CancelPatching();
            CancellationTokenSource = null;
            PublishApplicationStateChangedEvent($"Cancelled patching...", Colors.Red);
            IsAppBusy = false;
        }

        public void LaunchClient() {
            var launchResult = _eqGameApplicationService.Start();
            if (launchResult.IsFailure) {
                PublishApplicationStateChangedEvent(launchResult.Error, Colors.Red);
            }
            else {
                System.Environment.Exit(0);
            }
        }

        public void OpenSettings() {
            ActiveItem = _settingsViewModel;
            NotifyOfPropertyChange(nameof(IsPatchButtonVisisble));
            NotifyOfPropertyChange(nameof(IsCancelButtonVisisble));
            NotifyOfPropertyChange(nameof(IsExpansionSelectButtonVisisble));
        }

        private void CheckClient() {
            if (CheckDirectX()) {
                PublishApplicationStateChangedEvent("Checking for supported client.", Colors.Gold);
                var launchResult = _eqGameApplicationService.CanExecute;
                if (launchResult.IsFailure) {
                    CurrentClient = GameClientsEnum.Unknown;
                    PublishApplicationStateChangedEvent(launchResult.Error, Colors.Red);
                    CanLaunchClient = false;
                    ActiveItem = _settingsViewModel;
                }
                else {
                    CurrentClient = launchResult.Value;
                    NotifyOfPropertyChange(nameof(Title));
                    PublishApplicationStateChangedEvent("Supported client found.", Colors.Green);
                    CanLaunchClient = true;
                }
            }
        }

        // https://stackoverflow.com/questions/17130764/check-which-version-of-directx-is-installed
        // https://stackoverflow.com/questions/15948951/how-to-check-whether-directx-is-available
        // https://stackoverflow.com/questions/6159850/how-to-code-to-get-direct-x-version-on-my-machine-in-c
        // https://social.msdn.microsoft.com/Forums/en-US/0f7851a9-23a5-4eda-ab32-a36c17623acf/get-directx-major-and-minor-versions-installed?forum=vbgeneral
        private static ImmutableArray<string> DirectX90c = ImmutableArray.Create("4.09.00.0904", "4.09.0000.0904");
        private bool CheckDirectX() {
            PublishApplicationStateChangedEvent("Checking for supported DirectX.", Colors.Gold);
            using Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX");
            var versionStr = key.GetValue("Version") as string;
            if (!DirectX90c.Contains(versionStr)) {
                PublishApplicationStateChangedEvent("DirectX 9.0c is required but not installed.", Colors.Red);
                CanLaunchClient = false;
                return false;
            }

            return true;
        }

        private void LoadPatchFiles() {
            if (CurrentClient == GameClientsEnum.Unknown) {
                PublishApplicationStateChangedEvent("Unknown client.", Colors.Red);
                return;
            }

            if (_applicationConfig.SupportedExpansions.Count() == 0) {
                PublishApplicationStateChangedEvent("Application config has no supported expansions.", Colors.Red);
                return;
            }

            PublishApplicationStateChangedEvent("Initializing available expansions.", Colors.Gold);
            var repo = new PatchServerRepository(_httpClient);
            var queryHandler = new ServerPatchListQueryHandler(repo);
            var tasks = new List<Task>();
            foreach (var expansion in _applicationConfig.SupportedExpansions) {
                var query = new ServerPatchListQuery(CurrentClient, expansion);
                tasks.Add(Task.Run(async () => {
                    var fileListResult = await queryHandler.ExecuteAsync(query);
                    if (fileListResult.IsSuccess) {
                        ServerFiles.TryAdd(expansion, fileListResult.Value);
                    }
                    else {
                        PublishApplicationStateChangedEvent(fileListResult.Error, Colors.Red);
                    }
                }));
            }

            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(async () => {
                await Task.WhenAll(tasks);
                if (ServerFiles.Count() > 0) {
                    PublishApplicationStateChangedEvent("Initialized available expansions.", Colors.Green);
                    NotifyOfPropertyChange(nameof(CanPatchClient));
                    PublishAvailableExpansionsEvent(ServerFiles);
                }
                else {
                    PublishApplicationStateChangedEvent("No available expansions for given client..", Colors.Red);
                }
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private async Task CheckExpansionPatch(ExpansionsEnum expansion) {
            if (expansion == null || expansion == ExpansionsEnum.Unknown) {
                return;
            }

            IsAppBusy = true;
            PublishApplicationStateChangedEvent($"Checking patch data <{_applicationConfig.GameDirectory}>...", Colors.Gold);
            await Task.Delay(500); // must match animation timer for carousel to avoid animation stutter, should fix diffrently!
            var queryHandler = new ClientPatchListQueryHandler(ClientFilesRepository, ClientFilesRepository);
            var query = new ClientPatchListQuery(expansion, ServerFiles[expansion]);
            CurrentClientPatchFileList = await queryHandler.ExecuteAsync(query);
            if (CanPatchClient) {
                var appState = $"Patch available <{_applicationConfig.GameDirectory}>: {CurrentClientPatchFileList.Downloads.Count()} file(s) <{CurrentClientPatchFileList.DownloadSize}>";
                PublishApplicationStateChangedEvent(appState, Colors.Orange);
            }
            else {
                PublishApplicationStateChangedEvent($"Patched <{_applicationConfig.GameDirectory}> and ready to launch game.", Colors.Green);
            }

            IsAppBusy = false;
        }

        #region Event aggregation
        private void PublishApplicationStateChangedEvent(string newState, Color newColor) {
            _eventAggregator.PublishOnUIThread(new ApplicationStateChangedEvent(newState, newColor));
        }

        private void PublishAvailableExpansionsEvent(IDictionary<ExpansionsEnum, PatchManifest> expansionsFiles) {
            _eventAggregator.Publish(new AvailableExpansionsEvent(expansionsFiles));
        }

        public void Handle(ExpansionSelectedEvent message) {
            SelectedExpansion = message.SelectedExpansion;
        }

        public void Handle(GameDirectoryChangedEvent message) {
            CheckClient();
            LoadPatchFiles();
            CurrentClientPatchFileList = null;
        }
        #endregion Event aggregation
    }

    public class ShellDesignViewModel : ShellViewModel {
        public ShellDesignViewModel() : base(null, new ApplicationConfig()) {
        }
    }
}
