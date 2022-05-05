namespace FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories {
    public interface IGetRepository<TQuery, TResult>
    {
        Task<TResult> Get(TQuery query, CancellationToken cancellationToken = default);
    }

    public interface IGetRepositorySync<TQuery, TResult> {
        TResult Get(TQuery query);
    }
}
