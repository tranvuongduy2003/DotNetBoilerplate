namespace Domain.Common.Interfaces;

public interface IAuditable
{
    Guid AuthorId { get; set; }
}