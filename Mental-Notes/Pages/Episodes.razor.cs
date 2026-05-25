using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Mental_Notes.Pages;

public partial class EpisodesBase : ComponentBase
{
    [Inject] IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

    protected List<Guid>? EpisodeIds { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        EpisodeIds = await dbContext.Episodes
            .Include(e => e.Author)
            .Include(e => e.Book)
            .OrderByDescending(e => e.Published)
            .Select(ep => ep.Id)
            .ToListAsync();

        await base.OnInitializedAsync();
    }
}
