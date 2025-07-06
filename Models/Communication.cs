using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetProject.Models
{
    public class Invitation
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // "pending", "accepted", "declined"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
    }

    public class Message
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Read { get; set; } = false;

        // Navigation properties
        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;
    }

    public class Conversation
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Participant1Id { get; set; } = string.Empty;
        public string Participant2Id { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
        public string LastMessageContent { get; set; } = string.Empty;
        public int UnreadCount { get; set; } = 0;

        // Navigation properties
        [ForeignKey("Participant1Id")]
        public User Participant1 { get; set; } = null!;
        [ForeignKey("Participant2Id")]
        public User Participant2 { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
