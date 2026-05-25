using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Mental_Notes.Components;

public class EpisodeCardBase : ComponentBase
{
    [Inject] 
    protected IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

    [Parameter] 
    public Guid EpisodeId { get; set; }

    protected Episode? ep;

    protected override async Task OnParametersSetAsync()
    {
        if (EpisodeId == Guid.Empty)
        {
            ep = null;
            return;
        }

        await using var db = await DbFactory.CreateDbContextAsync();
        ep = await db.Episodes
            .Include(e => e.Author)
            .Include(e => e.Book)
            .FirstOrDefaultAsync(e => e.Id == EpisodeId);
    }
}
