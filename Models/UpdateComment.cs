namespace Anagram.Server.Models
{
    public class UpdateComment
    {
        public int Id { get; set; }

        public int UpdateId { get; set; }

        public Update? Update { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}