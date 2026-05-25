namespace SharedData.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }
    public string Description { get; set; } = string.Empty;
    public byte[]? Picture { get; set; } // Store image as binary (BLOB)
    public ICollection<Episode>? Episodes { get; set; }
}