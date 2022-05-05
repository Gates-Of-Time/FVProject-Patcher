namespace FvProject.EverquestGame.Patcher.Application.Contracts {
    public interface ICommandHandler<T> where T: ICommand {
        Task Execute(T command, CancellationToken cancellationToken = default);
    }
}
