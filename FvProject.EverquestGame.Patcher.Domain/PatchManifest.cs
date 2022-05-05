namespace FvProject.EverquestGame.Patcher.Domain
{
    public class PatchManifest
    {
        public string version { get; set; } = "";
        public List<PatchFileInfo> deletes { get; set; } = new List<PatchFileInfo>();
        public string downloadprefix { get; set; } = "";
        public List<PatchFileInfo> downloads { get; set; } = new List<PatchFileInfo>();
        public List<PatchFileInfo> unpacks { get; set; } = new List<PatchFileInfo>();
    }
}
