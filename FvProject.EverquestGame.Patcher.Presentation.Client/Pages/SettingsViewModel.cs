using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain.Enums;
using FvProject.EverquestGame.Patcher.Presentation.Client.Converters;
using FvProject.EverquestGame.Patcher.Presentation.Client.Events;
using Stylet;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Pages {
    public class SettingsViewModel : Screen, IHandle<AvailableExpansionsEvent> {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationConfig _applicationConfig;

        public SettingsViewModel(IEventAggregator eventAggregator, IApplicationConfig applicationConfig) {
            _eventAggregator = eventAggregator;
            _applicationConfig = applicationConfig;
            _gameDirectory = applicationConfig.GameDirectory;
            _eventAggregator?.Subscribe(this);
        }

        private string _gameDirectory;
        public string GameDirectory {
            get => _gameDirectory;
            set {
                SetAndNotify(ref _gameDirectory, value);
                PublishGameDirectoryChangedEvent();
                PersistChanges();
            }
        }

        private ExpansionsEnum _selectedExpansion;
        public ExpansionsEnum SelectedExpansion {
            get => _selectedExpansion;
            set {
                SetAndNotify(ref _selectedExpansion, value);
                PersistChanges();
            }
        }

        private ObservableCollection<ExpansionsEnum> _expansions = new ObservableCollection<ExpansionsEnum>();
        public ObservableCollection<ExpansionsEnum> Expansions {
            get => _expansions;
            set => SetAndNotify(ref _expansions, value);
        }

        private bool _enforceMD5Checksum;
        public bool EnforceMD5Checksum {
            get => _enforceMD5Checksum;
            set {
                SetAndNotify(ref _enforceMD5Checksum, value);
                PersistChanges();
            }
        }

        public void SelectDirectory() {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = _gameDirectory;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                GameDirectory = dialog.SelectedPath;
            }
        }

        private void PublishGameDirectoryChangedEvent() {
            _applicationConfig.GameDirectory = GameDirectory;
            _eventAggregator.Publish(new GameDirectoryChangedEvent(GameDirectory));
        }

        private void PersistChanges() {
            var jsonWriteOptions = new JsonSerializerOptions() {
                WriteIndented = true
            };
            jsonWriteOptions.Converters.Add(new JsonStringEnumConverter());

            var converter = new ApplicationConfigConverter();
            var appSettings = converter.Convert((_applicationConfig, SelectedExpansion));
            var newJson = JsonSerializer.Serialize(appSettings, jsonWriteOptions);
            var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            File.WriteAllText(appSettingsPath, newJson);
        }

        #region Event aggregation
        public void Handle(AvailableExpansionsEvent message) {
            Expansions = new ObservableCollection<ExpansionsEnum>(message.ExpansionsFiles.Keys);
            if (_applicationConfig.PreferredExpansion != null && message.ExpansionsFiles.Keys.Contains(_applicationConfig.PreferredExpansion)) {
                SelectedExpansion = _applicationConfig.PreferredExpansion;
            }
        }
        #endregion Event aggregation
    }

    public class SettingsDesignViewModel : SettingsViewModel {
        public SettingsDesignViewModel() : base(null, new ApplicationConfig()) {
            var applicationData = new ApplicationConfig();
            Expansions = new ObservableCollection<ExpansionsEnum>(applicationData.SupportedExpansions);
        }
    }
}
