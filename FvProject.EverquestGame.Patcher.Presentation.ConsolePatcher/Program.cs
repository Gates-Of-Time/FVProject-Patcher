using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using FvProject.EverquestGame.Patcher.Presentation.ConsolePatcher.Commands;
using Microsoft.Extensions.Hosting;

namespace FvProject.EverquestGame.Patcher.Presentation.ConsolePatcher {
    internal class Program {
        static async Task<int> Main(string[] args) {

            var parser = BuildCommandLine()
                .UseHost(_ => Host.CreateDefaultBuilder(args), (builder) => {
                    builder.ConfigureServices((hostContext, services) => {
                        var configuration = hostContext.Configuration;
                        // register other dependencies here
                    })
                    .UseCommandHandler<OriginalPatchCommand, OriginalPatchCommandHandler>()
                    .UseCommandHandler<KunarkPatchCommand, KunarkPatchCommandHandler>();
                }).UseDefaults().Build();
            return await parser.InvokeAsync(args);
        }

        static CommandLineBuilder BuildCommandLine() {
            var root = new RootCommand();
            root.AddCommand(new OriginalPatchCommand());
            root.AddCommand(new KunarkPatchCommand());
            return new CommandLineBuilder(root);
        }
    }
}
