namespace FvProject.EverquestGame.Patcher.Domain.Contracts.Repositories {
    public interface IGetAllRepository<TResult>
    {
        Task<TResult> GetAll();
    }
}
