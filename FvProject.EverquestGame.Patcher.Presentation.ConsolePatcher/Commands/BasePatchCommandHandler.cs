using System.CommandLine.Invocation;
using System.Net.Security;
using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Commands;
using FvProject.EverquestGame.Patcher.Application.Queries;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Enums;
using FvProject.EverquestGame.Patcher.Infrastructure;

namespace FvProject.EverquestGame.Patcher.Presentation.ConsolePatcher.Commands {
    public abstract class BasePatchCommandHandler : ICommandHandler {
        private HttpClient HttpClient { get; }
        private ExpansionsEnum Expansion { get; }

        public string EQDir { get; set; } // Conventional binding
        public bool MD5 { get; set; } // Conventional binding

        public BasePatchCommandHandler(ExpansionsEnum expansion) {
            var handler = new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    // Allow expired certificates (ignore errors)
                    if (errors == SslPolicyErrors.None)
                        return true;

                    // Here you can specifically allow "certificate expired" while blocking others
                    if (errors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors) ||
                        errors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch) ||
                        errors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable)) {
                        // Decide if you want to block these
                        //return false;
                    }

                    Console.WriteLine($"Ignoring certificate errors: {errors}");
                    return true;
                }
            };

            HttpClient = new HttpClient(handler);
            Expansion = expansion;
        }

        public async Task<int> InvokeAsync(InvocationContext context) {
            var applicationConfig = new ApplicationConfig(EQDir, MD5);
            var eqGameApplicationService = new EqGameApplicationService(applicationConfig);
            var result = eqGameApplicationService.CanExecute;
            if (result.IsSuccess) {
                var repo = new PatchServerRepository(HttpClient);
                var queryHandler = new ServerPatchListQueryHandler(repo);
                Console.WriteLine($"Using everquest directory {applicationConfig.GameDirectory}");
                Console.WriteLine($"Enforcing MD5 checksum {applicationConfig.EnforceMD5Checksum}");
                Console.WriteLine("Fetching patch manifest from server");
                var serverResult = await queryHandler.ExecuteAsync(new ServerPatchListQuery(result.Value, Expansion));

                if (serverResult.IsSuccess) {
                    var clientFilesRepository = new ClientFilesRepository(applicationConfig);
                    var currentClientPatchFileList = await CheckForUpdates(serverResult, clientFilesRepository);
                    if (currentClientPatchFileList.HasChanges) {
                        await PatchClient(context, applicationConfig, repo, clientFilesRepository, currentClientPatchFileList);
                    }
                    else {
                        Console.WriteLine($"Patched <{applicationConfig.GameDirectory}> and ready to launch game.");
                    }
                }
                else {
                    Console.WriteLine($"Failed fetching server patch manifest...\n{result.Error}");
                }
            }
            else {
                Console.WriteLine($"Failed validating eqgame.exe in {applicationConfig.GameDirectory}");
            }
            return 0;
        }

        private async Task<ClientPatch> CheckForUpdates(Result<PatchManifest> serverResult, ClientFilesRepository clientFilesRepository) {
            Console.WriteLine($"Checking client files against patch manifest");
            var queryHandler = new ClientPatchListQueryHandler(clientFilesRepository, clientFilesRepository);
            var query = new ClientPatchListQuery(Expansion, serverResult.Value);
            var currentClientPatchFileList = await queryHandler.ExecuteAsync(query);
            return currentClientPatchFileList;
        }

        private static async Task PatchClient(InvocationContext context, ApplicationConfig applicationConfig, PatchServerRepository repo, ClientFilesRepository clientFilesRepository, ClientPatch currentClientPatchFileList) {
            Console.WriteLine($"Patch available: {currentClientPatchFileList.Downloads.Count()} file(s) <{currentClientPatchFileList.DownloadSize}>\n");
            var reporter = new ConsoleReporter();
            var command = new PatchCommand(currentClientPatchFileList, reporter, applicationConfig.EnforceMD5Checksum);
            var commandHandler = new PatchCommandHandler(clientFilesRepository, repo, clientFilesRepository);
            await commandHandler.Execute(command, context.GetCancellationToken());
        }
    }
}
