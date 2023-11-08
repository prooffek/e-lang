namespace E_Lang.Builder.Interfaces
{
    public interface IBaseBuilder
    {
        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
