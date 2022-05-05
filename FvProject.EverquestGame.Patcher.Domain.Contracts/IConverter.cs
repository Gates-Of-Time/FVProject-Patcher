namespace FvProject.EverquestGame.Patcher.Domain.Contracts {
    public interface IConverter<From, To> {
        To Convert(From from);
    }
}
