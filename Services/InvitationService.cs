using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly BetBuddysDbContext _context;

        public InvitationService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Invitation?> GetInvitationByIdAsync(string invitationId)
        {
            return await _context.Invitations
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.Id == invitationId);
        }

        public async Task<IEnumerable<Invitation>> GetUserInvitationsAsync(string userEmail)
        {
            return await _context.Invitations
                .Include(i => i.Group)
                .Where(i => i.UserEmail == userEmail && i.Status == "pending")
                .ToListAsync();
        }

        public async Task<IEnumerable<Invitation>> GetGroupInvitationsAsync(string groupId)
        {
            return await _context.Invitations
                .Include(i => i.Group)
                .Where(i => i.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<Invitation> CreateInvitationAsync(Invitation invitation)
        {
            if (string.IsNullOrEmpty(invitation.Id))
            {
                invitation.Id = Guid.NewGuid().ToString();
            }

            invitation.CreatedAt = DateTime.UtcNow;
            invitation.Status = "pending";

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }

        public async Task<bool> RespondToInvitationAsync(string invitationId, bool accept, string userId)
        {
            var invitation = await _context.Invitations
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.Id == invitationId);

            if (invitation == null)
            {
                return false;
            }

            if (accept)
            {
                invitation.Status = "accepted";

                // Add user to group
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    var groupMember = new GroupMember
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = invitation.GroupId,
                        UserId = userId,
                        UserName = user.DisplayName ?? user.Email,
                        MemberType = "member",
                        JoinedAt = DateTime.UtcNow
                    };

                    _context.GroupMembers.Add(groupMember);
                }
            }
            else
            {
                invitation.Status = "declined";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvitationAsync(string invitationId)
        {
            var invitation = await _context.Invitations.FindAsync(invitationId);
            if (invitation == null)
            {
                return false;
            }

            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class LeaderboardService : ILeaderboardService
    {
        private readonly BetBuddysDbContext _context;

        public LeaderboardService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetGroupLeaderboardAsync(string groupId)
        {
            return await _context.LeaderboardEntries
                .Include(le => le.User)
                .Where(le => le.GroupId == groupId && le.EventId == null)
                .OrderByDescending(le => le.Points)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetEventLeaderboardAsync(string eventId)
        {
            return await _context.LeaderboardEntries
                .Include(le => le.User)
                .Where(le => le.EventId == eventId)
                .OrderByDescending(le => le.Points)
                .ToListAsync();
        }

        public async Task<LeaderboardEntry?> GetUserLeaderboardEntryAsync(string userId, string groupId, string? eventId = null)
        {
            return await _context.LeaderboardEntries
                .FirstOrDefaultAsync(le => le.UserId == userId && le.GroupId == groupId && le.EventId == eventId);
        }

        public async Task<bool> UpdateLeaderboardAsync(string userId, string groupId, string? eventId, int points, int correctPredictions, int totalPredictions)
        {
            var entry = await _context.LeaderboardEntries
                .FirstOrDefaultAsync(le => le.UserId == userId && le.GroupId == groupId && le.EventId == eventId);

            if (entry == null)
            {
                // Create new entry
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                entry = new LeaderboardEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    UserName = user.DisplayName ?? user.Email,
                    GroupId = groupId,
                    EventId = eventId,
                    Points = points,
                    CorrectPredictions = correctPredictions,
                    TotalPredictions = totalPredictions,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.LeaderboardEntries.Add(entry);
            }
            else
            {
                // Update existing entry
                entry.Points = points;
                entry.CorrectPredictions = correctPredictions;
                entry.TotalPredictions = totalPredictions;
                entry.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RecalculateLeaderboardAsync(string groupId, string? eventId = null)
        {
            // Get all predictions for the group/event
            var predictionsQuery = _context.Predictions
                .Include(p => p.User)
                .Where(p => p.Event.GroupId == groupId);

            if (eventId != null)
            {
                predictionsQuery = predictionsQuery.Where(p => p.EventId == eventId);
            }

            var predictions = await predictionsQuery.ToListAsync();

            // Group by user
            var userPredictions = predictions.GroupBy(p => p.UserId);

            foreach (var userGroup in userPredictions)
            {
                var userId = userGroup.Key;
                var userPreds = userGroup.ToList();

                var totalPoints = userPreds.Sum(p => p.PointsEarned ?? 0);
                var correctPredictions = userPreds.Count(p => p.IsCorrect == true);
                var totalPredictions = userPreds.Count;

                await UpdateLeaderboardAsync(userId, groupId, eventId, totalPoints, correctPredictions, totalPredictions);
            }

            return true;
        }
    }
}
