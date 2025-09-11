using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(string email, string password, string? displayName);
        Task<string> GenerateJwtTokenAsync(User user);
        Task<bool> VerifyPasswordAsync(string password, string passwordHash);
        Task<string> HashPasswordAsync(string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<bool> VerifyPasswordResetTokenAsync(string token);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> VerifyEmailAsync(string token);
        Task<string> GenerateEmailVerificationTokenAsync(User user);
    }

    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> UserExistsAsync(string userId);
    }

    public interface IGroupService
    {
        Task<Group?> GetGroupByIdAsync(string groupId);
        Task<IEnumerable<Group>> GetUserGroupsAsync(string userId);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<Group> CreateGroupAsync(Group group);
        Task<Group> UpdateGroupAsync(Group group);
        Task<bool> DeleteGroupAsync(string groupId);
        Task<bool> AddMemberAsync(string groupId, string userId, string userName);
        Task<bool> RemoveMemberAsync(string groupId, string userId);
        Task<bool> UpdateMemberRoleAsync(string groupId, string userId, string role);
        Task<IEnumerable<GroupMember>> GetGroupMembersAsync(string groupId);
    }

    public interface IEventService
    {
        Task<Event?> GetEventByIdAsync(string eventId);
        Task<IEnumerable<Event>> GetGroupEventsAsync(string groupId);
        Task<Event> CreateEventAsync(Event eventData);
        Task<Event> CreateEventAsync(string title, DateTime date, string location, string groupId);
        Task<Event> UpdateEventAsync(Event eventData);
        Task<bool> DeleteEventAsync(string eventId);
        Task<Match?> GetMatchByIdAsync(string matchId);
        Task<IEnumerable<Match>> GetEventMatchesAsync(string eventId);
        Task<Match> CreateMatchAsync(Match match);
        Task<Match> UpdateMatchAsync(Match match);
        Task<bool> DeleteMatchAsync(string matchId);
    }

    public interface IPredictionService
    {
        Task<Prediction?> GetPredictionByIdAsync(string predictionId);
        Task<IEnumerable<Prediction>> GetUserPredictionsAsync(string userId, string eventId);
        Task<IEnumerable<Prediction>> GetMatchPredictionsAsync(string matchId);
        Task<Prediction> CreatePredictionAsync(Prediction prediction);
        Task<Prediction> UpdatePredictionAsync(Prediction prediction);
        Task<bool> DeletePredictionAsync(string predictionId);
        Task<bool> ScorePredictionsAsync(string matchId, string winnerId, string method);
    }

    public interface IPostService
    {
        Task<Post?> GetPostByIdAsync(string postId);
        Task<IEnumerable<Post>> GetGroupPostsAsync(string groupId);
        Task<Post> CreatePostAsync(Post post);
        Task<Post> UpdatePostAsync(Post post);
        Task<bool> DeletePostAsync(string postId);
        Task<Comment?> GetCommentByIdAsync(string commentId);
        Task<IEnumerable<Comment>> GetPostCommentsAsync(string postId);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(string commentId);
        Task<bool> ReactToPostAsync(string postId, string userId, string reactionType);
        Task<bool> RemoveReactionAsync(string postId, string userId);
    }

    public interface IInvitationService
    {
        Task<Invitation?> GetInvitationByIdAsync(string invitationId);
        Task<IEnumerable<Invitation>> GetUserInvitationsAsync(string userEmail);
        Task<IEnumerable<Invitation>> GetGroupInvitationsAsync(string groupId);
        Task<Invitation> CreateInvitationAsync(Invitation invitation);
        Task<Invitation> CreateInvitationAsync(string groupId, string userEmail, string invitedByUserId);
        Task<bool> RespondToInvitationAsync(string invitationId, bool accept, string userId);
        Task<bool> DeleteInvitationAsync(string invitationId);
    }

    public interface ILeaderboardService
    {
        Task<IEnumerable<LeaderboardEntry>> GetGroupLeaderboardAsync(string groupId);
        Task<IEnumerable<LeaderboardEntry>> GetEventLeaderboardAsync(string eventId);
        Task<LeaderboardEntry?> GetUserLeaderboardEntryAsync(string userId, string groupId, string? eventId = null);
        Task<bool> UpdateLeaderboardAsync(string userId, string groupId, string? eventId, int points, int correctPredictions, int totalPredictions);
        Task<bool> RecalculateLeaderboardAsync(string groupId, string? eventId = null);
    }

    public interface IMessageService
    {
        Task<Message?> GetMessageByIdAsync(string messageId);
        Task<IEnumerable<Message>> GetConversationMessagesAsync(string senderId, string receiverId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
        Task<Message> SendMessageAsync(string senderId, string receiverId, string content);
        Task<bool> MarkMessageAsReadAsync(string messageId);
        Task<bool> DeleteMessageAsync(string messageId);
    }

    public interface INotificationService
    {
        Task<Notification?> GetNotificationByIdAsync(string notificationId);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<bool> MarkNotificationAsReadAsync(string notificationId);
        Task<bool> MarkAllNotificationsAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(string notificationId);
    }
}
