using System;

namespace Anagram.Server.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Delivery State

        public bool IsDelivered { get; set; } = false;

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        // Future-ready properties

        public bool IsEdited { get; set; } = false;

        public DateTime? EditedAt { get; set; }

        // Navigation

        public User? Sender { get; set; }

        public User? Receiver { get; set; }
    }
}