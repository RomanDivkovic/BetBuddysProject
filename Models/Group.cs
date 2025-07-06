using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class Group
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AdminId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool BettingEnabled { get; set; } = false;

        // Navigation properties
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public ICollection<Series> Series { get; set; } = new List<Series>();
        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }

    public class GroupMember
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string GroupId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string MemberType { get; set; } = "member"; // "member" or "isAdmin"
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
