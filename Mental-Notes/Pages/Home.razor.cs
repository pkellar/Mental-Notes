using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using SharedData.Models;
using System.Text.RegularExpressions;

namespace Pages;

public partial class HomeBase : ComponentBase
{
    [Inject] 
    IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    protected Episode? MostRecentEpisode { get; set; }
    protected Research? MostRecentResearch { get; set; }
    protected bool IsMobile { get; private set; }

    private List<LatestBlock>? _ordered;

    protected override async Task OnInitializedAsync()
    {
        await using var dbContext = await DbFactory.CreateDbContextAsync();

        MostRecentEpisode = await dbContext.Episodes
            .Include(e => e.Author)
            .Include(e => e.Book)
            .OrderByDescending(e => e.Published)
            .FirstOrDefaultAsync();

        MostRecentResearch = await dbContext.Researches
            .OrderByDescending(r => r.ReleaseDate)
            .FirstOrDefaultAsync();

        BuildOrdered();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                IsMobile = await JSRuntime.InvokeAsync<bool>("isMobile");
                StateHasChanged();
            }
            catch
            {
                IsMobile = false;
                StateHasChanged();
            }
        }
    }

    private void BuildOrdered()
    {
        _ordered = new();
        if (MostRecentEpisode != null)
        {
            _ordered.Add(new LatestBlock
            {
                Type = "episode",
                Episode = MostRecentEpisode,
                SortDate = MostRecentEpisode.Published
            });
        }
        if (MostRecentResearch != null)
        {
            _ordered.Add(new LatestBlock
            {
                Type = "research",
                Research = MostRecentResearch,
                SortDate = MostRecentResearch.ReleaseDate,
                PreviewHtml = BuildResearchPreview(MostRecentResearch.Content)
            });
        }
        _ordered = _ordered
            .OrderByDescending(b => b.SortDate)
            .ToList();
    }

    protected IEnumerable<LatestBlock> GetOrderedLatest() => _ordered ?? Enumerable.Empty<LatestBlock>();

    private string BuildResearchPreview(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        // Strip tags for preview (keep simple inline) then truncate
        var text = Regex.Replace(html, "<[^>]+>", " ");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (text.Length > 500) text = text[..500] + "…";
        // Return as simple paragraph
        return System.Net.WebUtility.HtmlEncode(text);
    }

    protected void NavToEpisode(Guid id) => NavigationManager.NavigateTo($"/episodeDetails/{id}");

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    protected class LatestBlock
    {
        public string Type { get; set; } = string.Empty; // episode | research
        public Episode? Episode { get; set; }
        public Research? Research { get; set; }
        public DateTime SortDate { get; set; }
        public string? PreviewHtml { get; set; }
    }
}
