using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Models;
using MyDotNetProject.Services;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<LeaderboardEntry>>> GetGroupLeaderboard(string groupId)
        {
            var leaderboard = await _leaderboardService.GetGroupLeaderboardAsync(groupId);
            return Ok(leaderboard);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<LeaderboardEntry>>> GetEventLeaderboard(string eventId)
        {
            var leaderboard = await _leaderboardService.GetEventLeaderboardAsync(eventId);
            return Ok(leaderboard);
        }

        [HttpGet("user/{userId}/group/{groupId}")]
        public async Task<ActionResult<LeaderboardEntry>> GetUserLeaderboardEntry(string userId, string groupId, [FromQuery] string? eventId = null)
        {
            var entry = await _leaderboardService.GetUserLeaderboardEntryAsync(userId, groupId, eventId);
            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }

        [HttpPost("recalculate/group/{groupId}")]
        public async Task<ActionResult> RecalculateGroupLeaderboard(string groupId, [FromQuery] string? eventId = null)
        {
            try
            {
                var result = await _leaderboardService.RecalculateLeaderboardAsync(groupId, eventId);
                if (!result)
                {
                    return BadRequest("Failed to recalculate leaderboard");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}
