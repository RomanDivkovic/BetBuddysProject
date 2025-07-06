using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class Event
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Place { get; set; }
        public string? Location { get; set; }
        public string GroupId { get; set; } = string.Empty;
        public string Status { get; set; } = "upcoming"; // "upcoming", "live", "completed"
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPartOfSeries { get; set; } = false;
        public string? SeriesId { get; set; }
        public string? ExternalId { get; set; }
        public string? ApiSlug { get; set; }

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        public ICollection<Match> Matches { get; set; } = new List<Match>();
        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
        [ForeignKey("SeriesId")]
        public Series? Series { get; set; }
    }

    public class Match
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string Fighter1Id { get; set; } = string.Empty;
        public string Fighter1Name { get; set; } = string.Empty;
        public string Fighter1Record { get; set; } = string.Empty;
        public string Fighter1Country { get; set; } = string.Empty;
        public string? Fighter1Logo { get; set; }
        public string Fighter2Id { get; set; } = string.Empty;
        public string Fighter2Name { get; set; } = string.Empty;
        public string Fighter2Record { get; set; } = string.Empty;
        public string Fighter2Country { get; set; } = string.Empty;
        public string? Fighter2Logo { get; set; }
        public string Status { get; set; } = "upcoming"; // "upcoming", "live", "completed"
        public int Order { get; set; }
        public string? WinnerId { get; set; }
        public string? ResultWinner { get; set; } // "fighter1" or "fighter2"
        public string? ResultMethod { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("EventId")]
        public Event Event { get; set; } = null!;
        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    }
}
