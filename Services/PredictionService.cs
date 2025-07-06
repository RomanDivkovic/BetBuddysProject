using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly BetBuddysDbContext _context;

        public PredictionService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Prediction?> GetPredictionByIdAsync(string predictionId)
        {
            return await _context.Predictions
                .Include(p => p.User)
                .Include(p => p.Event)
                .Include(p => p.Match)
                .FirstOrDefaultAsync(p => p.Id == predictionId);
        }

        public async Task<IEnumerable<Prediction>> GetUserPredictionsAsync(string userId, string eventId)
        {
            return await _context.Predictions
                .Include(p => p.Match)
                .Where(p => p.UserId == userId && p.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prediction>> GetMatchPredictionsAsync(string matchId)
        {
            return await _context.Predictions
                .Include(p => p.User)
                .Where(p => p.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<Prediction> CreatePredictionAsync(Prediction prediction)
        {
            if (string.IsNullOrEmpty(prediction.Id))
            {
                prediction.Id = Guid.NewGuid().ToString();
            }

            prediction.CreatedAt = DateTime.UtcNow;

            _context.Predictions.Add(prediction);
            await _context.SaveChangesAsync();
            return prediction;
        }

        public async Task<Prediction> UpdatePredictionAsync(Prediction prediction)
        {
            var existingPrediction = await _context.Predictions.FindAsync(prediction.Id);
            if (existingPrediction == null)
            {
                throw new InvalidOperationException("Prediction not found");
            }

            existingPrediction.PredictedWinnerId = prediction.PredictedWinnerId;
            existingPrediction.Method = prediction.Method;
            existingPrediction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPrediction;
        }

        public async Task<bool> DeletePredictionAsync(string predictionId)
        {
            var prediction = await _context.Predictions.FindAsync(predictionId);
            if (prediction == null)
            {
                return false;
            }

            _context.Predictions.Remove(prediction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ScorePredictionsAsync(string matchId, string winnerId, string method)
        {
            var predictions = await _context.Predictions
                .Where(p => p.MatchId == matchId)
                .ToListAsync();

            foreach (var prediction in predictions)
            {
                prediction.IsCorrect = prediction.PredictedWinnerId == winnerId;
                prediction.IsCorrectMethod = prediction.Method.ToLower() == method.ToLower();
                
                // Simple scoring system: 1 point for correct winner, bonus point for correct method
                int points = 0;
                if (prediction.IsCorrect == true)
                {
                    points += 1;
                    if (prediction.IsCorrectMethod == true)
                    {
                        points += 1;
                    }
                }
                
                prediction.PointsEarned = points;
                prediction.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
