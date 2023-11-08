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
        if (_instance is null)
        {
            _instance = new MockUserService();
        }

        return _instance;
    }
    
    public Task<User?> GetCurrentUser(CancellationToken cancellationToken)
    {
        return Task.FromResult(CurrentUser);
    }
}