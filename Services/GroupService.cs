using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class GroupService : IGroupService
    {
        private readonly BetBuddysDbContext _context;

        public GroupService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Group?> GetGroupByIdAsync(string groupId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(gm => gm.User)
                .Include(g => g.Events)
                .Include(g => g.Posts)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(gm => gm.User)
                .Where(g => g.Members.Any(gm => gm.UserId == userId))
                .ToListAsync();
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            if (string.IsNullOrEmpty(group.Id))
            {
                group.Id = Guid.NewGuid().ToString();
            }

            group.CreatedAt = DateTime.UtcNow;
            group.UpdatedAt = DateTime.UtcNow;

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            var existingGroup = await _context.Groups.FindAsync(group.Id);
            if (existingGroup == null)
            {
                throw new InvalidOperationException("Group not found");
            }

            existingGroup.Name = group.Name;
            existingGroup.Description = group.Description;
            existingGroup.BettingEnabled = group.BettingEnabled;
            existingGroup.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingGroup;
        }

        public async Task<bool> DeleteGroupAsync(string groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return false;
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMemberAsync(string groupId, string userId, string userName)
        {
            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (existingMember != null)
            {
                return false; // Member already exists
            }

            var groupMember = new GroupMember
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = groupId,
                UserId = userId,
                UserName = userName,
                MemberType = "member",
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberAsync(string groupId, string userId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null)
            {
                return false;
            }

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMemberRoleAsync(string groupId, string userId, string role)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null)
            {
                return false;
            }

            member.MemberType = role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(string groupId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.GroupId == groupId)
                .ToListAsync();
        }
    }
}
