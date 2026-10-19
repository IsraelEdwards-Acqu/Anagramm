using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Anagram.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // Authentication
        [Required]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Profile
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "/images/anagramUser.png";

        // Status
        public string Status { get; set; } = "Offline";
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        // <-- Add Bio (nullable)
        public string? Bio { get; set; }

        // ✅ Add CreatedAt (new property)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ICollection<FileTransfer> SentFiles { get; set; } = new List<FileTransfer>();
        public ICollection<FileTransfer> ReceivedFiles { get; set; } = new List<FileTransfer>();

        public ICollection<VoiceNote> SentVoiceNotes { get; set; } = new List<VoiceNote>();
        public ICollection<VoiceNote> ReceivedVoiceNotes { get; set; } = new List<VoiceNote>();
        public ICollection<Call> CallsMade { get; set; } = new List<Call>();
        public ICollection<Call> CallsReceived { get; set; } = new List<Call>();
        public ICollection<Update>? Updates { get; set; }

        public ICollection<FriendRequest>? SentFriendRequests { get; set; }
        public ICollection<FriendRequest>? ReceivedFriendRequests { get; set; }
        public ICollection<Follow>? Followers { get; set; }
        public ICollection<Follow>? Following { get; set; }
        public ICollection<UpdateLike>? LikedUpdates { get; set; }
        public ICollection<UpdateSave>? SavedUpdates { get; set; }
        public ICollection<UpdateShare>? SentShares { get; set; }
        public ICollection<UpdateShare>? ReceivedShares { get; set; }
    }
}
