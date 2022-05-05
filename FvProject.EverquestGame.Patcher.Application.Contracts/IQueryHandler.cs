namespace FvProject.EverquestGame.Patcher.Application.Contracts
{
    public interface IQueryHandler<TQuery, TQueryResult> where TQuery: IQuery { 
        Task<TQueryResult> ExecuteAsync(TQuery query);
    }
}