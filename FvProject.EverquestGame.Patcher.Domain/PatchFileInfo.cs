namespace FvProject.EverquestGame.Patcher.Domain
{
    public class PatchFileInfo
    {
        public string name { get; set; } = "";
        public string md5 { get; set; } = "";
        public string date { get; set; } = "";
        public string zip { get; set; } = "";
        public int size { get; set; }
    }
}
