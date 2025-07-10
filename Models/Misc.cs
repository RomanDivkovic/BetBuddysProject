using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "comment", "reaction", "prediction_result"
        public string Text { get; set; } = string.Empty;
        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Read { get; set; } = false;
        public string? MetaData { get; set; } // JSON string for additional data

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public class Series
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string GroupId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "upcoming"; // "upcoming", "active", "completed"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }

    public class Bet
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CreatorId { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string? EventId { get; set; }
        public string? MatchId { get; set; }
        public string Status { get; set; } = "open"; // "open", "closed", "settled"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ClosesAt { get; set; }
        public DateTime? SettledAt { get; set; }

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        [ForeignKey("CreatorId")]
        public User Creator { get; set; } = null!;
        [ForeignKey("EventId")]
        public Event? Event { get; set; }
        [ForeignKey("MatchId")]
        public Match? Match { get; set; }
        public ICollection<BetOption> Options { get; set; } = new List<BetOption>();
    }

    public class BetOption
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string BetId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("BetId")]
        public Bet Bet { get; set; } = null!;
        public ICollection<UserBet> UserBets { get; set; } = new List<UserBet>();
    }

    public class UserBet
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BetOptionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("BetOptionId")]
        public BetOption BetOption { get; set; } = null!;
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
