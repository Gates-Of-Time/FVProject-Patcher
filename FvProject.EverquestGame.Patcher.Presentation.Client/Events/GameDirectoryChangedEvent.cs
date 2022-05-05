namespace FvProject.EverquestGame.Patcher.Presentation.Client.Events {
    public class GameDirectoryChangedEvent {
        public GameDirectoryChangedEvent(string gameDirectory) {
            GameDirectory = gameDirectory;
        }

        public string GameDirectory { get; }
    }
}
