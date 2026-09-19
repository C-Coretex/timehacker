using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Extensions;

internal static class CategoryLinkExtensions
{
    /// <summary>
    /// Rejects a link to a category the caller cannot see. RLS hides another user's category from every
    /// read, but PostgreSQL runs foreign-key checks as the table owner and so bypasses it: without this
    /// guard the junction row is written happily and the link then reads back as nothing at all.
    /// </summary>
    /// //TODO: check if needed. Maybe add to DBContext interceptor/SaveChanges to check for the visibility of link entities
    public static async Task ThrowIfAnyCategoryIsNotVisibleAsync(
        this ICategoryRepository categoryRepository,
        IEnumerable<LinkCategoryDto> links,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(categoryRepository);
        ArgumentNullException.ThrowIfNull(links);

        var ids = links.Select(link => link.Id).Distinct().ToList();
        if (ids.Count == 0)
            return;

        var visibleIds = await categoryRepository.GetAll()
            .Where(category => ids.Contains(category.Id))
            .Select(category => category.Id)
            .ToListAsync(cancellationToken);

        var missingId = ids.Except(visibleIds).FirstOrDefault();
        if (missingId != default)
            throw new NotFoundException("Category", missingId.ToString());
    }
}
