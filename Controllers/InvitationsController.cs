using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Services;
using MyDotNetProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize]
    public class InvitationsController : BaseController

    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invitation>> GetById(string id)
        {
            var invitation = await _invitationService.GetInvitationByIdAsync(id);
            if (invitation == null)
                return NotFound();

            return Ok(invitation);
        }

        [HttpGet("user/{userEmail}")]
        public async Task<ActionResult<IEnumerable<Invitation>>> GetByUserEmail(string userEmail)
        {
            var invitations = await _invitationService.GetUserInvitationsAsync(userEmail);
            return Ok(invitations);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Invitation>>> GetByGroupId(string groupId)
        {
            var invitations = await _invitationService.GetGroupInvitationsAsync(groupId);
            return Ok(invitations);
        }

        [HttpPost]
        public async Task<ActionResult<Invitation>> Create(CreateInvitationRequest request)
        {

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User not authenticated");
            }


            try
            {
                var invitation = await _invitationService.CreateInvitationAsync(
                    request.GroupId,
                    request.UserEmail,
                    currentUserId
                );

                return CreatedAtAction(nameof(GetById), new { id = invitation.Id }, invitation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/accept")]
        public async Task<ActionResult> Accept(string id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                var result = await _invitationService.RespondToInvitationAsync(id, true, currentUserId);
                if (!result)
                    return BadRequest("Failed to accept invitation");
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/decline")]
        public async Task<ActionResult> Decline(string id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                var result = await _invitationService.RespondToInvitationAsync(id, false, currentUserId);
                if (!result)
                    return BadRequest("Failed to decline invitation");
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _invitationService.DeleteInvitationAsync(id);
            return NoContent();
        }
    }

     public class CreateInvitationRequest
    {
        public required string GroupId { get; set; }
        public required string UserEmail { get; set; }
    }
}
