using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class EventService : IEventService
    {
        private readonly BetBuddysDbContext _context;

        public EventService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            return await _context.Events
                .Include(e => e.Matches)
                .Include(e => e.Group)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<IEnumerable<Event>> GetGroupEventsAsync(string groupId)
        {
            return await _context.Events
                .Include(e => e.Matches)
                .Where(e => e.GroupId == groupId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<Event> CreateEventAsync(Event eventData)
        {
            if (string.IsNullOrEmpty(eventData.Id))
            {
                eventData.Id = Guid.NewGuid().ToString();
            }

            eventData.CreatedAt = DateTime.UtcNow;
            eventData.UpdatedAt = DateTime.UtcNow;

            _context.Events.Add(eventData);
            await _context.SaveChangesAsync();
            return eventData;
        }

        public async Task<Event> CreateEventAsync(string title, DateTime date, string location, string groupId)
        {
            // Check if group exists
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                throw new ArgumentException("Group not found");
            }

            var eventData = new Event
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Date = date,
                Location = location,
                GroupId = groupId,
                Status = "upcoming",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(eventData);
            await _context.SaveChangesAsync();
            return eventData;
        }

        public async Task<Event> UpdateEventAsync(Event eventData)
        {
            var existingEvent = await _context.Events.FindAsync(eventData.Id);
            if (existingEvent == null)
            {
                throw new InvalidOperationException("Event not found");
            }

            existingEvent.Title = eventData.Title;
            existingEvent.Date = eventData.Date;
            existingEvent.Place = eventData.Place;
            existingEvent.Location = eventData.Location;
            existingEvent.Status = eventData.Status;
            existingEvent.ImageUrl = eventData.ImageUrl;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(string eventId)
        {
            var eventData = await _context.Events.FindAsync(eventId);
            if (eventData == null)
            {
                return false;
            }

            _context.Events.Remove(eventData);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Match?> GetMatchByIdAsync(string matchId)
        {
            return await _context.Matches
                .Include(m => m.Event)
                .Include(m => m.Predictions)
                .FirstOrDefaultAsync(m => m.Id == matchId);
        }

        public async Task<IEnumerable<Match>> GetEventMatchesAsync(string eventId)
        {
            return await _context.Matches
                .Where(m => m.EventId == eventId)
                .OrderBy(m => m.Order)
                .ToListAsync();
        }

        public async Task<Match> CreateMatchAsync(Match match)
        {
            if (string.IsNullOrEmpty(match.Id))
            {
                match.Id = Guid.NewGuid().ToString();
            }

            match.CreatedAt = DateTime.UtcNow;
            match.UpdatedAt = DateTime.UtcNow;

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> UpdateMatchAsync(Match match)
        {
            var existingMatch = await _context.Matches.FindAsync(match.Id);
            if (existingMatch == null)
            {
                throw new InvalidOperationException("Match not found");
            }

            existingMatch.Fighter1Id = match.Fighter1Id;
            existingMatch.Fighter1Name = match.Fighter1Name;
            existingMatch.Fighter1Record = match.Fighter1Record;
            existingMatch.Fighter1Country = match.Fighter1Country;
            existingMatch.Fighter1Logo = match.Fighter1Logo;
            existingMatch.Fighter2Id = match.Fighter2Id;
            existingMatch.Fighter2Name = match.Fighter2Name;
            existingMatch.Fighter2Record = match.Fighter2Record;
            existingMatch.Fighter2Country = match.Fighter2Country;
            existingMatch.Fighter2Logo = match.Fighter2Logo;
            existingMatch.Status = match.Status;
            existingMatch.WinnerId = match.WinnerId;
            existingMatch.ResultWinner = match.ResultWinner;
            existingMatch.ResultMethod = match.ResultMethod;
            existingMatch.Type = match.Type;
            existingMatch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingMatch;
        }

        public async Task<bool> DeleteMatchAsync(string matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
            {
                return false;
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
