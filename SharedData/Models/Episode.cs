namespace SharedData.Models;

public class Episode
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    // Instead of storing binary, store a blob reference
    public string? AudioBlobUrl { get; set; }

    public DateTime Published { get; set; }
    public string Description { get; set; } = string.Empty;

    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }

    public Guid BookId { get; set; }
    public Book? Book { get; set; }
}

