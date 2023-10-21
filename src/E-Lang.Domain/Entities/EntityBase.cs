using E_Lang.Domain.Interfaces;

namespace E_Lang.Domain.Entities;

public abstract class EntityBase : IEntity<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}