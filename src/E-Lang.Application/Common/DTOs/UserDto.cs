using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.DTOs;

public class UserDto : IMapper<User>
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}