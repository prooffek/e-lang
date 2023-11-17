namespace E_Lang.Application.Common.DTOs;

public class UpdateCollectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentCollectionId { get; set; }
}