using System.Windows.Media;
using FvProject.EverquestGame.Patcher.Presentation.Client.Events;
using Stylet;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Pages {
    public class StatusBarViewModel : Screen, IHandle<ApplicationStateChangedEvent> {
        private readonly IEventAggregator _eventAggregator;

        public StatusBarViewModel(IEventAggregator eventAggregator) {
            _eventAggregator = eventAggregator;
            _eventAggregator?.Subscribe(this);
        }

        private string _appState;
        public string AppState {
            get => _appState;
            set => SetAndNotify(ref _appState, value);
        }

        private Brush _statusBrush;
        public Brush StatusBrush {
            get => _statusBrush;
            set => SetAndNotify(ref _statusBrush, value);
        }

        #region Event aggregation
        public void Handle(ApplicationStateChangedEvent message) {
            AppState = message.AppState;
            StatusBrush = new SolidColorBrush(message.Color);
        }
        #endregion Event aggregation
    }

    public class StatusBarDesignViewModel : StatusBarViewModel {
        public StatusBarDesignViewModel() : base(null) {
            AppState = "Design mode app state";
            StatusBrush = new SolidColorBrush(Colors.DeepSkyBlue);
        }
    }
}
