using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class MessageService : IMessageService
    {
        private readonly BetBuddysDbContext _context;

        public MessageService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Message?> GetMessageByIdAsync(string messageId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<IEnumerable<Message>> GetConversationMessagesAsync(string senderId, string receiverId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                           (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.Conversations
                .Include(c => c.Participant1)
                .Include(c => c.Participant2)
                .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content)
        {
            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null) throw new InvalidOperationException("Sender not found");

            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = senderId,
                SenderName = sender.DisplayName ?? sender.Email,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                Read = false
            };

            _context.Messages.Add(message);

            // Create or update conversation
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => 
                    (c.Participant1Id == senderId && c.Participant2Id == receiverId) ||
                    (c.Participant1Id == receiverId && c.Participant2Id == senderId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid().ToString(),
                    Participant1Id = senderId,
                    Participant2Id = receiverId,
                    LastMessageAt = DateTime.UtcNow,
                    LastMessageContent = content,
                    UnreadCount = 1
                };
                _context.Conversations.Add(conversation);
            }
            else
            {
                conversation.LastMessageAt = DateTime.UtcNow;
                conversation.LastMessageContent = content;
                conversation.UnreadCount++;
            }

            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> MarkMessageAsReadAsync(string messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) return false;

            message.Read = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly BetBuddysDbContext _context;

        public NotificationService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetNotificationByIdAsync(string notificationId)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == notificationId);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            if (string.IsNullOrEmpty(notification.Id))
            {
                notification.Id = Guid.NewGuid().ToString();
            }

            notification.CreatedAt = DateTime.UtcNow;
            notification.Read = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> MarkNotificationAsReadAsync(string notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.Read = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.Read)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.Read = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(string notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
