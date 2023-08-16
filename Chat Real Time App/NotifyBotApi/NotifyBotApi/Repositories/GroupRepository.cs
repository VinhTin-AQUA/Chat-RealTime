using Microsoft.EntityFrameworkCore;
using NotifyBotApi.Data;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;

namespace NotifyBotApi.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly NotifyBotContext context;

        public GroupRepository(NotifyBotContext context)
        {
            this.context = context;
        }
        public async Task<bool> Save()
        {
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
        public async Task<Group> GroupNameExist(string groupName, string userId)
        {
            var group = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.GroupUsers)
                .SelectMany(u => u.GroupUsers)
                .Select(gu => gu.Group)
                .Where(g => g.Name == groupName)
                .FirstOrDefaultAsync();

            return group;
        }
        public async Task<Group> GetGroupById(string groupId)
        {
            var group = await context.Groups.FindAsync(groupId);
            return group;
        }
        public async Task<bool> GroupExistById(string groupId)
        {
            var result = await context.Groups.FindAsync(groupId);
            return result != null;
        }
        public async Task<ICollection<Group>> GetAllGroups()
        {
            var groups = await context.Groups.ToListAsync();
            return groups;
        }
        public async Task<ICollection<Group>> GetGroupOfUser(string userId)
        {
            var groupUser = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.GroupUsers)
                .SelectMany(u => u.GroupUsers)
                .Include(g => g.Group)
                .Select(g => g.Group)
                .ToListAsync();
            return groupUser;
        }
        public async Task<ICollection<string>> GetUsersOfGroup(string groupId)
        {
            var groups = await context.Groups
                .Where(g => g.Id == groupId)
                .Include(g => g.GroupUsers)
                .SelectMany(g => g.GroupUsers)
                .Include(gu => gu.User)
                .Select(gu => gu.User)
                .ToListAsync();

            var userNamesOfGroup = groups.Select(u => u.FirstName + " " + u.LastName).ToList();
            return userNamesOfGroup;
        }
        public async Task<bool> AddGroup(Group model)
        {
            context.Add(model);
            return await Save();
        }
        public async Task<bool> AddGroupUser(GroupUser model)
        {
            context.GroupUsers.Add(model);
            return await Save();
        }
        public async Task<bool> AddUserToGroup(GroupUser model)
        {
            context.GroupUsers.Add(model);
            return await Save();
        }
        public async Task<bool> LaaveGroup(GroupUser groupUser)
        {
            context.GroupUsers.Remove(groupUser);
            return await Save();
        }
        public async Task<bool> HasMessage(string groupId)
        {
            var messages = await context.MessageChats
                .Where(m => m.GroupId == groupId)
                .AnyAsync();
            return messages;
        }

        public async Task<bool> DeleteMessages(string groupId)
        {
            var messages = await context.MessageChats
                .Where(m => m.GroupId == groupId)
                .ToListAsync();
            context.MessageChats.RemoveRange(messages);
            return await Save();
        }

        public async Task<bool> DeleteGroupEmpty(string groupId)
        {
            var memeberTotal = await GetUsersOfGroup(groupId);
            if(memeberTotal.ToList().Count() <= 0)
            {
                // delete messages og group if existed
                if(await HasMessage(groupId) == true)
                {
                   await DeleteMessages(groupId);
                }

                var group = await GetGroupById(groupId);
                context.Groups.Remove(group);
                return await Save();
            }
            return false;
        }
        public async Task<bool> SetHasNewMessage(string groupId, bool hasNewMessage)
        {
            var group = await context.Groups.FindAsync(groupId);
            group.HasNewMessage = hasNewMessage;
            return await Save();
        }
    }
}
