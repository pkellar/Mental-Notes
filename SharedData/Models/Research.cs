
namespace SharedData.Models;

public class Research
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Content { get; set; } = string.Empty; // HTML or Markdown
    public DateTime ReleaseDate { get; set; }
}