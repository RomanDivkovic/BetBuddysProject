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

    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invitation>> GetInvitation(string id)
        {
            var invitation = await _invitationService.GetInvitationByIdAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }
            return Ok(invitation);
        }

        [HttpGet("user/{userEmail}")]
        public async Task<ActionResult<IEnumerable<Invitation>>> GetUserInvitations(string userEmail)
        {
            var invitations = await _invitationService.GetUserInvitationsAsync(userEmail);
            return Ok(invitations);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Invitation>>> GetGroupInvitations(string groupId)
        {
            var invitations = await _invitationService.GetGroupInvitationsAsync(groupId);
            return Ok(invitations);
        }

        [HttpPost]
        public async Task<ActionResult<Invitation>> CreateInvitation([FromBody] Invitation invitation)
        {
            try
            {
                var createdInvitation = await _invitationService.CreateInvitationAsync(invitation);
                return CreatedAtAction(nameof(GetInvitation), new { id = createdInvitation.Id }, createdInvitation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/respond")]
        public async Task<ActionResult> RespondToInvitation(string id, [FromBody] InvitationResponse response)
        {
            try
            {
                var result = await _invitationService.RespondToInvitationAsync(id, response.Accept, response.UserId);
                if (!result)
                {
                    return BadRequest("Failed to respond to invitation");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInvitation(string id)
        {
            var result = await _invitationService.DeleteInvitationAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class InvitationResponse
    {
        public bool Accept { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
