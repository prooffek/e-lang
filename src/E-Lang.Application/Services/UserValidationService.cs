using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Services;

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _userRepository;

    public UserValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void ValidateUserId(Guid? userId)
    {
        if (!userId.HasValue || userId.Value == Guid.Empty)
            throw new ArgumentNullException(nameof(User.Id), "User not found.");
    }
}