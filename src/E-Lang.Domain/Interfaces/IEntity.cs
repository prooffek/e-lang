namespace E_Lang.Domain.Interfaces;

public interface IEntity<TId>
{
    public TId Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}