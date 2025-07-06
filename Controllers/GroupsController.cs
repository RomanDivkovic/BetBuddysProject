using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Models;
using MyDotNetProject.Services;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(string id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return Ok(group);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Group>>> GetUserGroups(string userId)
        {
            var groups = await _groupService.GetUserGroupsAsync(userId);
            return Ok(groups);
        }

        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup([FromBody] Group group)
        {
            try
            {
                var createdGroup = await _groupService.CreateGroupAsync(group);
                return CreatedAtAction(nameof(GetGroup), new { id = createdGroup.Id }, createdGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Group>> UpdateGroup(string id, [FromBody] Group group)
        {
            if (id != group.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedGroup = await _groupService.UpdateGroupAsync(group);
                return Ok(updatedGroup);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGroup(string id)
        {
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<GroupMember>>> GetGroupMembers(string id)
        {
            var members = await _groupService.GetGroupMembersAsync(id);
            return Ok(members);
        }

        [HttpPost("{groupId}/members/{userId}")]
        public async Task<ActionResult> AddMember(string groupId, string userId, [FromBody] string userName)
        {
            var result = await _groupService.AddMemberAsync(groupId, userId, userName);
            if (!result)
            {
                return BadRequest("Member already exists or invalid data");
            }
            return Ok();
        }

        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<ActionResult> RemoveMember(string groupId, string userId)
        {
            var result = await _groupService.RemoveMemberAsync(groupId, userId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{groupId}/members/{userId}/role")]
        public async Task<ActionResult> UpdateMemberRole(string groupId, string userId, [FromBody] string role)
        {
            var result = await _groupService.UpdateMemberRoleAsync(groupId, userId, role);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
