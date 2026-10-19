namespace Anagram.Server.Models
{
    public record CommentDto(
    int Id,
    string Username,
    string AvatarUrl,
    string Content,
    DateTime CreatedAt
);
}
