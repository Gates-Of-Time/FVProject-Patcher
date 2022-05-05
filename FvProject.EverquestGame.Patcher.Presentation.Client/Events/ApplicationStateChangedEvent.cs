using System.Windows.Media;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Events {
    public class ApplicationStateChangedEvent {
        public ApplicationStateChangedEvent(string appState, Color color) {
            AppState = appState;
            Color = color;
        }

        public string AppState { get; }
        public Color Color { get; }
    }
}
