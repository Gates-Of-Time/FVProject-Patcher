using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Infrastructure {
    public class EqGameApplicationService : IExternalApplicationService {
        private readonly IApplicationConfig _applicationConfig;

        public EqGameApplicationService(IApplicationConfig applicationConfig) {
            _applicationConfig = applicationConfig;
        }

        public Result<GameClientsEnum> CanExecute {
            get {
                if (!Directory.Exists(_applicationConfig.GameDirectory)) {
                    return Result.Failure<GameClientsEnum>("Please select a valid Everquest directory.");
                }

                var di = new System.IO.DirectoryInfo(_applicationConfig.GameDirectory);
                var files = di.GetFiles("eqgame.exe");
                if (files == null || files.Length == 0) {
                    return Result.Failure<GameClientsEnum>("Please select a valid Everquest directory.");
                }

                var clientHash = MD5Hasher.CreateHash(File.OpenRead(files[0].FullName));
                var gameClient = GameClientsEnum.List.First(x => x.IsValidFor(clientHash));
                if (gameClient == GameClientsEnum.Unknown) {
                    var failureMessage = $"Unsupported game client, please use one of the following: {string.Join(", ", GameClientsEnum.List.Where(x => x != GameClientsEnum.Unknown).Select(x => x.Name))}.";
                    return Result.Failure<GameClientsEnum>(failureMessage);
                }

                return Result.Success<GameClientsEnum>(gameClient);
            }
        }

        public Result Start() {
            try {
                var processInfo = new System.Diagnostics.ProcessStartInfo($@"{_applicationConfig.GameDirectory}\eqgame.exe", "patchme") {
                    WorkingDirectory = $"{_applicationConfig.GameDirectory}\\"
                };

                var process = System.Diagnostics.Process.Start(processInfo);
                if (process == null) {
                    return Result.Failure("The process failed to start.");
                }
            }
            catch (Exception err) {
                return Result.Failure($"An error occured while trying to start everquest:  {err.Message}");
            }

            return Result.Success();
        }
    }
}
