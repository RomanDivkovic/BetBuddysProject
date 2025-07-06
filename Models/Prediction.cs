using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class Prediction
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public string PredictedWinnerId { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool? IsCorrect { get; set; }
        public bool? IsCorrectMethod { get; set; }
        public int? PointsEarned { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        [ForeignKey("EventId")]
        public Event Event { get; set; } = null!;
        [ForeignKey("MatchId")]
        public Match Match { get; set; } = null!;
    }

    public class LeaderboardEntry
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string? EventId { get; set; }
        public int Points { get; set; }
        public int CorrectPredictions { get; set; }
        public int TotalPredictions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}
