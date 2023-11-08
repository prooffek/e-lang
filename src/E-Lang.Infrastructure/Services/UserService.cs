using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace E_Lang.Infrastructure.Services;

public class UserService : IUserService
{
    // TODO: Remove when Aythentication is implemented
    private const string CURRENT_USER_NAME = "Admin";
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }
    
    // TODO: Remove CURRENT_USER_NAME once authentication is implemented and adjust the below method
    public async Task<User?> GetCurrentUser(CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext.Items.TryGetValue(nameof(User), out var cachedUserObject))
        {
            var cachedUser = cachedUserObject as User;
            return cachedUser;
        }

        var httpContextUser = _httpContextAccessor.HttpContext.User;

        if (httpContextUser is null)
            return null;

        var userName = httpContextUser.Claims.SingleOrDefault(x =>
            x.Type.Equals(nameof(User.UserName), StringComparison.OrdinalIgnoreCase));

        // if (userName is null)
        //     return null;

        var user = await _userRepository.GetUserByUserName(userName?.Value ?? CURRENT_USER_NAME, cancellationToken);

        if (user is null)
            return null;
        
        _httpContextAccessor.HttpContext.Items.Add(nameof(User), user);

        return user;
    }
}