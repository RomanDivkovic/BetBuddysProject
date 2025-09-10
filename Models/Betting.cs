using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class GroupEvent
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = "upcoming"; // upcoming, live, completed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
        public ICollection<MmaFight> Fights { get; set; } = new List<MmaFight>();
        public ICollection<FightBet> Bets { get; set; } = new List<FightBet>();
    }

    public class MmaFight
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string GroupEventId { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty; // ID från MMA API
        public DateTime FightDate { get; set; }
        public string Category { get; set; } = string.Empty; // Lightweight, Heavyweight, etc.
        public string Status { get; set; } = "Not Started"; // Not Started, Live, Finished, Cancelled
        public bool IsMain { get; set; } = false;

        // Fighter 1
        public string Fighter1Id { get; set; } = string.Empty;
        public string Fighter1Name { get; set; } = string.Empty;
        public string Fighter1Logo { get; set; } = string.Empty;
        public bool Fighter1Winner { get; set; } = false;

        // Fighter 2
        public string Fighter2Id { get; set; } = string.Empty;
        public string Fighter2Name { get; set; } = string.Empty;
        public string Fighter2Logo { get; set; } = string.Empty;
        public bool Fighter2Winner { get; set; } = false;

        // Result (when fight is finished)
        public string? WinnerName { get; set; }
        public string? WinMethod { get; set; } // KO, TKO, Submission, Decision, etc.

        // Navigation properties
        [ForeignKey("GroupEventId")]
        public GroupEvent GroupEvent { get; set; } = null!;
        public ICollection<FightBet> Bets { get; set; } = new List<FightBet>();
    }

    public class FightBet
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string GroupEventId { get; set; } = string.Empty;
        public string FightId { get; set; } = string.Empty;

        public string PredictedWinner { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty; // KO, TKO, Submission, Decision
        public int Confidence { get; set; } = 50; // 1-100

        public bool IsCorrect { get; set; } = false; // Set when fight is finished
        public int Points { get; set; } = 0; // Points earned from this bet

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        [ForeignKey("GroupEventId")]
        public GroupEvent GroupEvent { get; set; } = null!;
        [ForeignKey("FightId")]
        public MmaFight Fight { get; set; } = null!;
    }
}
