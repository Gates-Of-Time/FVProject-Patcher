using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain.Enums;

namespace FvProject.EverquestGame.Patcher.Application.Queries {
    public class ServerPatchListQuery : IQuery {
        public ServerPatchListQuery(GameClientsEnum gameClient, ExpansionsEnum expansion) {
            GameClient = gameClient;
            Expansion = expansion;
        }

        public GameClientsEnum GameClient { get; }
        public ExpansionsEnum Expansion { get; }
    }
}
