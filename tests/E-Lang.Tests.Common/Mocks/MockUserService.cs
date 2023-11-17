using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Tests.Common.Mocks;

public class MockUserService : IUserService
{
    private static MockUserService? _instance;

    public static User? CurrentUser { get; set; }

    private MockUserService()
    {
    }

    public static MockUserService GetInstance()
    {
        return _instance ??= new MockUserService();
    }
    
    public Task<User?> GetCurrentUser(CancellationToken cancellationToken)
    {
        return Task.FromResult(CurrentUser);
    }
}