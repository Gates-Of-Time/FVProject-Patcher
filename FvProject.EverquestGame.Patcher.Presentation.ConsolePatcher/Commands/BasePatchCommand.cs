using System.CommandLine;

namespace FvProject.EverquestGame.Patcher.Presentation.ConsolePatcher.Commands {
    public abstract class BasePatchCommand : Command {
        public BasePatchCommand(string name, string description) : base(name, description) {
            var everquestDirectory = new Option<string>("--eqdir") {
                Description = "Path to Everquest (eqgame.exe)",
                IsRequired = true
            };
            everquestDirectory.AddAlias("-d");
            AddOption(everquestDirectory);

            AddOption(new Option<bool>(new string[] { "--md5" }, "Enforce MD5 checksum validation"));
        }
    }
}
