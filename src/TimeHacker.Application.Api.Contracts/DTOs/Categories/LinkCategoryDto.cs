namespace TimeHacker.Application.Api.Contracts.DTOs.Categories;

/// <summary>
/// A category referenced by id only — what a task carries when it is merely linking to one.
/// <see cref="CategoryDto"/> extends it with the category's own data for read paths.
/// </summary>
public record LinkCategoryDto
{
    public Guid Id { get; init; }

    public static LinkCategoryDto EmptyLink(Guid id) => new() { Id = id };

    public TLink GetLinkEntity<TLink>(Func<Guid, TLink> linkFactory)
    {
        ArgumentNullException.ThrowIfNull(linkFactory);
        return linkFactory(Id);
    }
}
