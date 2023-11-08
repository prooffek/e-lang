using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class UserConfigurations
{
    public static ModelBuilder ConfigureUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e =>
                    new {e.Id},
                $"{nameof(User)}_{nameof(User.Id)}"
            );
            
            entity.HasIndex(e =>
                    new {e.UserName},
                $"{nameof(User)}_{nameof(User.UserName)}"
            ).IsUnique();

            entity.HasIndex(e =>
                    new {e.Email},
                $"{nameof(User)}_{nameof(User.Email)}"
            ).IsUnique();
        });
        
        return modelBuilder;
    }
}