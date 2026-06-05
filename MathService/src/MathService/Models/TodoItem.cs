namespace MathService.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}