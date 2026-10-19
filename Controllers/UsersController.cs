using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Anagram.Server.Data;
using Anagram.Server.Models;
using System.Security.Cryptography;
using System.Text;

namespace Anagram.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AnagramDbContext _db;

        public UsersController(AnagramDbContext db)
        {
            _db = db;
        }

        // Helpers
        private string CurrentUsername => User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        private int? CurrentUserId
        {
            get
            {
                var idClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(idClaim, out var id) ? id : (int?)null;
            }
        }

        // -------------------- Registration --------------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            var user = new User
            {
                Username = dto.Username,
                Name = dto.Name,
                Email = dto.Email,
                AvatarUrl = dto.AvatarUrl,
                PasswordHash = HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, user.Name });
        }

        // Simple password hashing (replace with ASP.NET Identity in production)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // -------------------- Existing Endpoints --------------------

        // Public profile
        [AllowAnonymous]
        [HttpGet("{username}/profile")]
        public async Task<IActionResult> GetProfile(string username)
        {
            var user = await _db.Users
                .Where(u => u.Username == username)
                .Select(u => new UserProfileDto(
                    u.Username,
                    u.Name,
                    u.AvatarUrl,
                    u.Bio,
                    u.Updates.Count,
                    u.Followers.Count,
                    u.Following.Count))
                .SingleOrDefaultAsync();

            return user == null ? NotFound() : Ok(user);
        }

        // User state (auth required)
        [Authorize]
        [HttpGet("{username}/state")]
        public async Task<IActionResult> GetUserState(string username)
        {
            var target = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (target == null) return NotFound();

            var meId = CurrentUserId;
            if (meId == null) return Ok(new UserStateDto(false, false, false, null));

            var me = await _db.Users.FindAsync(meId.Value);
            if (me == null) return Unauthorized();

            var isFollowing = await _db.Follows.AnyAsync(f => f.FollowerId == me.Id && f.FollowingId == target.Id);

            var (a, b) = me.Id < target.Id ? (me.Id, target.Id) : (target.Id, me.Id);
            var isFriend = await _db.Friendships.AnyAsync(f => f.UserAId == a && f.UserBId == b);

            var hasPendingRequest = await _db.FriendRequests.AnyAsync(fr =>
                ((fr.FromUserId == me.Id && fr.ToUserId == target.Id) ||
                 (fr.FromUserId == target.Id && fr.ToUserId == me.Id)) &&
                 fr.Status == FriendRequestStatus.Pending);

            int? friendshipId = null;
            if (isFriend)
            {
                var friendship = await _db.Friendships.SingleOrDefaultAsync(f => f.UserAId == a && f.UserBId == b);
                friendshipId = friendship?.Id;
            }

            return Ok(new UserStateDto(isFollowing, isFriend, hasPendingRequest, friendshipId));
        }

        // Paged updates
        [AllowAnonymous]
        [HttpGet("{username}/updates")]
        public async Task<IActionResult> GetUserUpdates(string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();

            var updates = await _db.Updates
                .Where(u => u.UserId == user.Id)
                .OrderByDescending(u => u.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UpdateDto(
                    u.Id,
                    user.Username,
                    user.AvatarUrl,
                    u.Content,
                    u.MediaUrl,
                    u.Timestamp,
                    u.Likes.Count,
                    u.Saves.Count))
                .ToListAsync();

            return Ok(updates ?? new List<UpdateDto>());
        }

        // Saved updates
        [Authorize]
        [HttpGet("{username}/saved")]
        public async Task<IActionResult> GetSavedUpdates(string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!string.Equals(username, CurrentUsername, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var me = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (me == null) return NotFound();

            var saved = await _db.UpdateSaves
                .Where(s => s.UserId == me.Id)
                .OrderByDescending(s => s.SavedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Update)
                    .ThenInclude(u => u.User)
                .Select(s => new UpdateDto(
                    s.Update.Id,
                    s.Update.User.Username,
                    s.Update.User.AvatarUrl,
                    s.Update.Content,
                    s.Update.MediaUrl,
                    s.Update.Timestamp,
                    s.Update.Likes.Count,
                    s.Update.Saves.Count))
                .ToListAsync();

            return Ok(saved ?? new List<UpdateDto>());
        }

        // Current user summary
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var me = await _db.Users
                .Where(u => u.Username == CurrentUsername)
                .Select(u => new UserSummaryDto(u.Id, u.Username, u.Name, u.AvatarUrl))
                .SingleOrDefaultAsync();

            return me == null ? Unauthorized() : Ok(me);
        }

        // Available users
        [AllowAnonymous]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var users = await _db.Users
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserSummaryDto(u.Id, u.Username, u.Name, u.AvatarUrl))
                .ToListAsync();

            return Ok(users ?? new List<UserSummaryDto>());
        }
    }

    // DTOs
    public record RegisterDto(string Username, string Name, string Email, string AvatarUrl, string Password);
    public record UserProfileDto(string Username, string Name, string AvatarUrl, string? Bio, int UpdatesCount, int FollowersCount, int FollowingCount);
    public record UserStateDto(bool IsFollowing, bool IsFriend, bool HasPendingRequest, int? FriendshipId);
    public record UpdateDto(int Id, string Username, string AvatarUrl, string Content, string? MediaUrl, DateTime Timestamp, int Likes, int Saves);
    public record UserSummaryDto(int Id, string Username, string Name, string AvatarUrl);
}
