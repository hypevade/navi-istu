namespace Istu.Navigation.Domain.Models.Entities;

public abstract class BaseEntity
{
    public required Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}