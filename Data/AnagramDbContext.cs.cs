using Anagram.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Anagram.Server.Data
{
    public class AnagramDbContext : DbContext
    {
        public AnagramDbContext(DbContextOptions<AnagramDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FileTransfer> FileTransfers { get; set; }
        public DbSet<VoiceNote> VoiceNotes { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<UpdateLike> UpdateLikes { get; set; }
        public DbSet<UpdateSave> UpdateSaves { get; set; }
        public DbSet<UpdateShare> UpdateShares { get; set; }
        public DbSet<UpdateComment> UpdateComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------------
            // USER
            // ------------------------------------

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ------------------------------------
            // MESSAGE
            // ------------------------------------

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // FILE TRANSFERS
            // ------------------------------------

            modelBuilder.Entity<FileTransfer>()
                .HasOne(f => f.Sender)
                .WithMany(u => u.SentFiles)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileTransfer>()
                .HasOne(f => f.Receiver)
                .WithMany(u => u.ReceivedFiles)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // VOICE NOTES
            // ------------------------------------

            modelBuilder.Entity<VoiceNote>()
                .HasOne(v => v.Sender)
                .WithMany(u => u.SentVoiceNotes)
                .HasForeignKey(v => v.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VoiceNote>()
                .HasOne(v => v.Receiver)
                .WithMany(u => u.ReceivedVoiceNotes)
                .HasForeignKey(v => v.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // CALLS
            // ------------------------------------

            modelBuilder.Entity<Call>()
                .HasOne(c => c.Caller)
                .WithMany(u => u.CallsMade)
                .HasForeignKey(c => c.CallerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Call>()
                .HasOne(c => c.Receiver)
                .WithMany(u => u.CallsReceived)
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // UPDATES
            // ------------------------------------

            modelBuilder.Entity<Update>()
                .HasOne(u => u.User)
                .WithMany(u => u.Updates)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------------------------
            // FRIEND REQUESTS
            // ------------------------------------

            modelBuilder.Entity<FriendRequest>()
                .HasIndex(fr => new
                {
                    fr.FromUserId,
                    fr.ToUserId
                })
                .IsUnique();

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.FromUser)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fr => fr.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.ToUser)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // FRIENDSHIPS
            // ------------------------------------

            modelBuilder.Entity<Friendship>()
                .HasIndex(f => new
                {
                    f.UserAId,
                    f.UserBId
                })
                .IsUnique();

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserA)
                .WithMany()
                .HasForeignKey(f => f.UserAId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserB)
                .WithMany()
                .HasForeignKey(f => f.UserBId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // FOLLOWS
            // ------------------------------------

            modelBuilder.Entity<Follow>()
                .HasIndex(f => new
                {
                    f.FollowerId,
                    f.FollowingId
                })
                .IsUnique();

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // UPDATE LIKES
            // ------------------------------------

            modelBuilder.Entity<UpdateLike>()
                .HasIndex(x => new
                {
                    x.UpdateId,
                    x.UserId
                })
                .IsUnique();

            modelBuilder.Entity<UpdateLike>()
                .HasOne(x => x.Update)
                .WithMany(u => u.Likes)
                .HasForeignKey(x => x.UpdateId);

            modelBuilder.Entity<UpdateLike>()
                .HasOne(x => x.User)
                .WithMany(u => u.LikedUpdates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // UPDATE SAVES
            // ------------------------------------

            modelBuilder.Entity<UpdateSave>()
                .HasIndex(x => new
                {
                    x.UpdateId,
                    x.UserId
                })
                .IsUnique();

            modelBuilder.Entity<UpdateSave>()
                .HasOne(x => x.Update)
                .WithMany(u => u.Saves)
                .HasForeignKey(x => x.UpdateId);

            modelBuilder.Entity<UpdateSave>()
                .HasOne(x => x.User)
                .WithMany(u => u.SavedUpdates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // UPDATE SHARES
            // ------------------------------------

            modelBuilder.Entity<UpdateShare>()
                .HasOne(x => x.Update)
                .WithMany(u => u.Shares)
                .HasForeignKey(x => x.UpdateId);

            modelBuilder.Entity<UpdateShare>()
                .HasOne(x => x.FromUser)
                .WithMany(u => u.SentShares)
                .HasForeignKey(x => x.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UpdateShare>()
                .HasOne(x => x.ToUser)
                .WithMany(u => u.ReceivedShares)
                .HasForeignKey(x => x.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UpdateComment>()
            .HasOne(c => c.Update)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UpdateId);

            modelBuilder.Entity<UpdateComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
    }
    }
}
