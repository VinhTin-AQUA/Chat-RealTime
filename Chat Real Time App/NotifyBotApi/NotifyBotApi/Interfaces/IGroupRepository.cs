using NotifyBotApi.DTOs.Chat;
using NotifyBotApi.Models;

namespace NotifyBotApi.Interfaces
{
    public interface IGroupRepository
    {
        Task<bool> Save();
        Task<Group> GroupNameExist(string groupName, string userId);
        Task<ICollection<Group>> GetAllGroups();
        Task<ICollection<Group>> GetGroupOfUser(string userId);
        Task<Group> GetGroupById(string groupId);
        Task<bool> AddGroup(Group model);
        Task<bool> AddGroupUser(GroupUser model);
        Task<bool> AddUserToGroup(GroupUser model);
        Task<bool> GroupExistById(string groupId);
        Task<bool> LaaveGroup(GroupUser groupUser);
        Task<ICollection<string>> GetUsersOfGroup(string groupId);
        Task<bool> HasMessage(string groupId);
        Task<bool> DeleteMessages(string groupId);
        Task<bool> DeleteGroupEmpty(string groupId);
        Task<bool> SetHasNewMessage(string groupId, bool hasNewMessage);
    }
}
