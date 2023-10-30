namespace E_Lang.Tests.Common.Interfaces
{
    public interface ITestBuilder
    {
        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
