namespace FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories {
    public interface IUpsertRepository<TQuery, TResult> {
        Task<TResult> Upsert(TQuery query, CancellationToken cancellationToken = default);
    }
}
