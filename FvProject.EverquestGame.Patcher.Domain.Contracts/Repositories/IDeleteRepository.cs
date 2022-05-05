namespace FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories {
    public interface IDeleteRepository<TQuery, TResult> {
        Task<TResult> Delete(TQuery query);
    }

    public interface IDeleteRepository<TQuery> {
        Task Delete(TQuery query);
    }
}
