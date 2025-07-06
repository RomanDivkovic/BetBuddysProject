using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class Post
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool Edited { get; set; } = false;

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    }

    public class Comment
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public class PostReaction
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PostId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ReactionType { get; set; } = string.Empty; // "like" or "dislike"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
