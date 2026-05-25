using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Pages;

public partial class ResearchBase : ComponentBase
{
    [Inject] 
    IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

    protected List<Research>? Researches { get; set; }

    protected bool TableOfContentsVisible { get; set; } = true;

    protected void ToggleTableOfContents()
    {
        TableOfContentsVisible = !TableOfContentsVisible;
    }

    protected override async Task OnInitializedAsync()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();
        Researches = await dbContext.Researches
            .OrderBy(r => r.Order)
            .ToListAsync();
        await base.OnInitializedAsync();
    }
}
