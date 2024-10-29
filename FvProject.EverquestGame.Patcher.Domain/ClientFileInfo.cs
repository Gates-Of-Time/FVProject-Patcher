namespace FvProject.EverquestGame.Patcher.Domain {
    public class ClientFileInfo {
        public ClientFileInfo(string name, string path) {
            Name = name;
            FullPath = path;
        }

        public string Name { get; }
        public string FullPath { get; }

        public bool HasSameName(PatchFileInfo patchFileInfo) {
            return string.Equals(Name, patchFileInfo.name.Replace('/', Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
        }
    }
}
