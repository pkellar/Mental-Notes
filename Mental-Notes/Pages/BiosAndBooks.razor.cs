using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Pages;

public partial class BiosAndBooksBase : ComponentBase
{
    [Inject] IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

    protected List<Author>? Authors { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        Authors = await dbContext.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.Name)
            .ToListAsync();

        await base.OnInitializedAsync();
    }
}
