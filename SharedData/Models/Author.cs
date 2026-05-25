namespace SharedData.Models;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public byte[]? Picture { get; set; } // Store image as binary (BLOB)
    public ICollection<Episode>? Episodes { get; set; }
    public ICollection<Book>? Books { get; set; }
}