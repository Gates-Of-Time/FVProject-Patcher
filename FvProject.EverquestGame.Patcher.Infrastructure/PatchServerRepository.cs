using CSharpFunctionalExtensions;
using FvProject.EverquestGame.Patcher.Application.Queries;
using FvProject.EverquestGame.Patcher.Domain;
using FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FvProject.EverquestGame.Patcher.Infrastructure {
    public class PatchServerRepository : IGetRepository<ServerPatchListQuery, Result<PatchManifest>>, IGetRepository<string, Result<Stream>> {
        public PatchServerRepository(HttpClient httpClient) {
            HttpClient = httpClient;
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        private HttpClient HttpClient { get; }
        private IDeserializer Deserializer { get; }

        public async Task<Result<PatchManifest>> Get(ServerPatchListQuery query, CancellationToken cancellationToken = default) {
            var patchFilesList = CreateUrl(query.Expansion.ShortName, query.GameClient.ShortName);
            if (!Uri.IsWellFormedUriString(patchFilesList, UriKind.Absolute)) {
                return Result.Failure<PatchManifest>($"Malformed URL <{patchFilesList}>");
            }

            try {
                var response = await HttpClient.GetAsync(patchFilesList);
                if (response.IsSuccessStatusCode) {
                    var filesList = await response.Content.ReadAsStringAsync();
                    return Result.Success(Deserializer.Deserialize<PatchManifest>(filesList));
                }
                else {
                    return Result.Failure<PatchManifest>($"Error: <{response.StatusCode}> {response}");
                }
            }
            catch (Exception ex) {
                return Result.Failure<PatchManifest>($"Error: {ex}");
            }
        }

        public async Task<Result<Stream>> Get(string url, CancellationToken cancellationToken = default) {
            try {
                var response = await HttpClient.GetAsync(url, cancellationToken);
                if (response.IsSuccessStatusCode) {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return Result.Success(stream);
                }
                else {
                    return Result.Failure<Stream>($"Error: <{response.StatusCode}> {response}");
                }
            }
            catch (TaskCanceledException) {
                return Result.Failure<Stream>($"cancellation.");
            }
            catch (Exception ex) {
                return Result.Failure<Stream>($"Error: {ex}");
            }
        }

        private string CreateUrl(string expansion, string clientVersion) => $"https://{expansion}.fvproject.com/{clientVersion}/filelist_{clientVersion}.yml";
    }
}
