using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public interface IBettingService
    {
        Task<GroupEvent> CreateGroupEventAsync(string groupId, string eventTitle, DateTime eventDate, List<MmaFight> fights);
        Task<FightBet> PlaceBetAsync(string userId, string groupEventId, string fightId, string predictedWinner, string method, int confidence);
        Task<IEnumerable<FightBet>> GetUserBetsAsync(string userId, string groupEventId);
        Task<IEnumerable<FightBet>> GetGroupEventBetsAsync(string groupEventId);
        Task<GroupEvent?> GetGroupEventByIdAsync(string groupEventId);
        Task<IEnumerable<GroupEvent>> GetGroupEventsAsync(string groupId);
    }

    public class BettingService : IBettingService
    {
        private readonly BetBuddysDbContext _context;

        public BettingService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<GroupEvent> CreateGroupEventAsync(string groupId, string eventTitle, DateTime eventDate, List<MmaFight> fights)
        {
            var groupEvent = new GroupEvent
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = groupId,
                EventTitle = eventTitle,
                EventDate = eventDate,
                Status = "upcoming",
                CreatedAt = DateTime.UtcNow
            };

            _context.GroupEvents.Add(groupEvent);

            // Add fights to the event
            foreach (var fight in fights)
            {
                fight.GroupEventId = groupEvent.Id;
                _context.MmaFights.Add(fight);
            }

            await _context.SaveChangesAsync();
            return groupEvent;
        }

        public async Task<FightBet> PlaceBetAsync(string userId, string groupEventId, string fightId, string predictedWinner, string method, int confidence)
        {
            // Check if user already has a bet on this fight
            var existingBet = await _context.FightBets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.FightId == fightId);

            if (existingBet != null)
            {
                // Update existing bet
                existingBet.PredictedWinner = predictedWinner;
                existingBet.Method = method;
                existingBet.Confidence = confidence;
                existingBet.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existingBet;
            }

            var bet = new FightBet
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                GroupEventId = groupEventId,
                FightId = fightId,
                PredictedWinner = predictedWinner,
                Method = method,
                Confidence = confidence,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.FightBets.Add(bet);
            await _context.SaveChangesAsync();
            return bet;
        }

        public async Task<IEnumerable<FightBet>> GetUserBetsAsync(string userId, string groupEventId)
        {
            return await _context.FightBets
                .Include(b => b.Fight)
                .Where(b => b.UserId == userId && b.GroupEventId == groupEventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FightBet>> GetGroupEventBetsAsync(string groupEventId)
        {
            return await _context.FightBets
                .Include(b => b.User)
                .Include(b => b.Fight)
                .Where(b => b.GroupEventId == groupEventId)
                .ToListAsync();
        }

        public async Task<GroupEvent?> GetGroupEventByIdAsync(string groupEventId)
        {
            return await _context.GroupEvents
                .Include(ge => ge.Fights)
                .Include(ge => ge.Bets)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(ge => ge.Id == groupEventId);
        }

        public async Task<IEnumerable<GroupEvent>> GetGroupEventsAsync(string groupId)
        {
            return await _context.GroupEvents
                .Include(ge => ge.Fights)
                .Where(ge => ge.GroupId == groupId)
                .OrderByDescending(ge => ge.EventDate)
                .ToListAsync();
        }
    }
}
