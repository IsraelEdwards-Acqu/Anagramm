namespace Anagram.Server.Models;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public string Type { get; set; } = "";

    public string Message { get; set; } = "";

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}